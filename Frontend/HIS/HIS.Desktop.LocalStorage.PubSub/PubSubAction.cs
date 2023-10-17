﻿using HIS.Desktop.LocalStorage.PubSub.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.LocalStorage.PubSub.Config;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using System.Windows.Forms;
using Inventec.Common.Logging;
using PSS.SDO;
using Inventec.Common.WSPubSub;
using Newtonsoft.Json;
using HIS.Desktop.Common;
using System.Threading;
using DevExpress.XtraBars.Alerter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MOS.SDO;
using System.IO;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.LocalStorage.PubSub
{
    public class PubSubAction
    {
        public static Form frmShowNotifi { get; set; }
        private static PubSubProcessor pubsubProcessor { get; set; }
        private static System.Windows.Forms.Timer timerByConnectPubSubClient { get; set; }
        private static PubSubADO currentPubSub { get; set; }
        private static DateTime ConnectTime { get; set; }
        private static List<string> ListUuid = new List<string>();
        private static string loginName { get; set; }
        private static long? departmentId { get; set; }
        private static HIS_ALERT HisAlert { get; set; }
        private static AlertControl alertControl = new AlertControl();
        private static AlertButton abAccept = new AlertButton();
        private static AlertButton abCancel = new AlertButton();
        private static string ALERT_SOUND_FOLDER = Path.Combine(Application.StartupPath, @"Integrate\AlertSound");
        private static bool IsSpeech { get; set; }
        private static System.Windows.Forms.Timer timerCall { get; set; }
        public static void IntPubSubClient()
        {
            try
            {
                HisConfigCFG.LoadConfig();
                if (HisConfigCFG.PUBSUB_INFO == null)
                {
                    LogSystem.Error("Connect server pubsub failed");
                    return;
                }
                CreateNotifi();
                GetDataUser();
                ConnectPubSub();
                TimerCallMessage();
                if (HisConfigCFG.TimeCheckConnection > 0)
                {
                    timerByConnectPubSubClient = new System.Windows.Forms.Timer();
                    timerByConnectPubSubClient.Interval = HisConfigCFG.TimeCheckConnection * 1000;
                    timerByConnectPubSubClient.Enabled = true;
                    timerByConnectPubSubClient.Tick += timerByConnectPubSubClient_Tick;
                    timerByConnectPubSubClient.Start();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static void GetDataUser()
        {
            try
            {
                ConnectTime = DateTime.Now;
                loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var employee = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == loginName);
                if (employee != null)
                {
                    departmentId = employee.DEPARTMENT_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public static async Task<bool> SendMessage(PubSubADO data)
        {
            bool result = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Info("SendMessage");
                if (pubsubProcessor == null) return result;
                if (data == null)
                {
                    LogSystem.Error("SendMessage. Data is empty: \n" + LogUtil.TraceData("data", data));
                    return result;
                }
                PssPubSubSDO datasend = new PssPubSubSDO();
                datasend.CommandCode = "PubSubADO";
                datasend.JsonData = JsonConvert.SerializeObject(data);
                ListUuid.Add(data.Uuid);
                result = await pubsubProcessor.Publish(datasend);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        public static void DisposePubSub()
        {
            try
            {
                if (pubsubProcessor == null) return;
                Inventec.Common.Logging.LogSystem.Info("DisposePubSub");
                pubsubProcessor.UnsubscribeChannel();
                if (pubsubProcessor.IsConnected)
                    pubsubProcessor.Stop();
                pubsubProcessor = null;
                currentPubSub = null;
                timerByConnectPubSubClient = null;
                ListUuid = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static void timerByConnectPubSubClient_Tick(object sender, EventArgs e)
        {
            try
            {
                Task.Run(() => { ConnectPubSub(); });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static async void ConnectPubSub()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("New pubsubProcessor");
                if (pubsubProcessor != null && pubsubProcessor.IsConnected) return;
                pubsubProcessor = new PubSubProcessor(ReceivedMessage);
                if (await pubsubProcessor.Init())
                {
                    if (!pubsubProcessor.IsSubChanel())
                    {
                        if (!await pubsubProcessor.SubscribeChannel())
                        {
                            LogSystem.Error("SubscribeChannel failed");
                        }
                    }
                }
                else
                {
                    LogSystem.Error("Connect server pubsub failed");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static void ReceivedMessage(PssPubSubSDO data)
        {
            try
            {
                if (data == null)
                {
                    LogSystem.Error("ReceivedMessage. Data is empty: \n" + LogUtil.TraceData("data", data));
                    return;
                }
                if (String.IsNullOrWhiteSpace(data.JsonData))
                {
                    LogSystem.Error("ReceivedMessage. JsonData is empty: \n" + LogUtil.TraceData("data", data));
                    return;
                }
                switch (data.CommandCode)
                {
                    case "PubSubADO":
                        currentPubSub = JsonConvert.DeserializeObject<PubSubADO>(data.JsonData);
                        TimeSpan difference = DateTime.Now - ConnectTime;
                        if (difference.TotalSeconds < 30 || ListUuid.Exists(o => o.Equals(currentPubSub.Uuid))) return;
                        ListUuid.Add(currentPubSub.Uuid);
                        if (currentPubSub.actionType == ActionType.RESET)
                            ActionReset();
                        else
                            ActionRefreshCache();
                        break;
                    case "HIS_ALERT":
                        HisAlert = JsonConvert.DeserializeObject<HIS_ALERT>(data.JsonData);
                        Inventec.Common.Logging.LogSystem.Debug("ReceivedMessage__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisAlert), HisAlert));
                        ThreadNotifi();
                        break;
                    default:
                        LogSystem.Error("ReceivedMessage. CommandCode invalid: \n" + LogUtil.TraceData("data", data));
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static void ThreadNotifi()
        {
            Thread thread = new Thread(new ThreadStart(ProcessNotifi));
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                thread.Abort();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static void ProcessNotifi()
        {
            try
            {
                if (HisAlert == null)
                    return;
                if (loginName.Equals(HisAlert.CREATOR))
                    return;
                if (!("," + HisAlert.RECEIVE_DEPARTMENT_IDS + ",").Contains("," + departmentId + ",") && !WorkPlace.GetDepartmentIds().Exists(o => ("," + HisAlert.RECEIVE_DEPARTMENT_IDS + ",").Contains("," + o + ",")))
                    return;
                if (!string.IsNullOrEmpty(HisAlert.RECEIVER_LOGINNAME))
                {
                    DisposeNotifi(HisAlert.ID);
                }
                else
                {
                    ShowNotifi();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static void CreateNotifi()
        {
            try
            {
                alertControl = new AlertControl();
                abAccept = new AlertButton();
                abCancel = new AlertButton();
                alertControl.ShowCloseButton = false;
                abAccept.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/apply_32x32.png");
                abAccept.Name = "abAccept";
                abAccept.Hint = "Đồng ý";
                abCancel.Image = DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/cancel_32x32.png");
                abCancel.Name = "abCancel";
                abCancel.Hint = "Từ chối";
                alertControl.Buttons.Add(abAccept);
                alertControl.Buttons.Add(abCancel);
                alertControl.FormShowingEffect = DevExpress.XtraBars.Alerter.AlertFormShowingEffect.SlideHorizontal;
                alertControl.ButtonClick += new DevExpress.XtraBars.Alerter.AlertButtonClickEventHandler(alertControl_ButtonClick);
                alertControl.FormLoad += new DevExpress.XtraBars.Alerter.AlertFormLoadEventHandler(alertControl_FormLoad);
                alertControl.FormClosing += new AlertFormClosingEventHandler(alertControl_FormClosing);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static void alertControl_FormClosing(object sender, AlertFormClosingEventArgs e)
        {
            try
            {
                if (e.CloseReason == AlertFormCloseReason.TimeUp)
                    e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static void alertControl_ButtonClick(object sender, DevExpress.XtraBars.Alerter.AlertButtonClickEventArgs e)
        {
            try
            {
                var alertId = Int64.Parse(e.Info.Tag.ToString());
                CommonParam param = new CommonParam();
                bool resultData = false;
                if (e.ButtonName == "abAccept")
                {
                    resultData = new BackendAdapter(param).Post<bool>("api/HisAlert/Receiver", ApiConsumers.MosConsumer, alertId, param);
                    if (!resultData)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("api/HisAlert/Receiver thất bại " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => alertId), alertId));
                    }
                }
                else if (e.ButtonName == "abCancel")
                {
                    resultData = new BackendAdapter(param).Post<bool>("api/HisAlert/Reject", ApiConsumers.MosConsumer, alertId, param);
                    if (!resultData)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("api/HisAlert/Reject thất bại " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => alertId), alertId));
                    }
                }
                if (resultData)
                    DisposeNotifi(alertId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static void alertControl_FormLoad(object sender, AlertFormLoadEventArgs e)
        {
            try
            {
                e.Buttons.PinButton.SetDown(true);
                alertControl.AlertFormList[alertControl.AlertFormList.Count - 1].Tag = HisAlert.ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static void ShowNotifi()
        {
            try
            {
                if (frmShowNotifi.InvokeRequired)
                {
                    frmShowNotifi.Invoke(new MethodInvoker(delegate
                    {
                        AlertInfo info = new AlertInfo(HisAlert.TITLE, HisAlert.CONTENT);
                        info.Tag = HisAlert.ID;
                        alertControl.Show(frmShowNotifi, info);
                    }));
                }
                else
                {
                    AlertInfo info = new AlertInfo(HisAlert.TITLE, HisAlert.CONTENT);
                    info.Tag = HisAlert.ID;
                    alertControl.Show(frmShowNotifi, info);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static void DisposeNotifi(long alertId)
        {
            try
            {
                if (frmShowNotifi.InvokeRequired)
                {
                    frmShowNotifi.Invoke(new MethodInvoker(delegate
                    {
                        var alertForm = alertControl.AlertFormList.FirstOrDefault(o => Int64.Parse(o.Tag.ToString()) == alertId);
                        if (alertForm == null)
                            return;
                        alertControl.AlertFormList[alertControl.AlertFormList.IndexOf(alertForm)].Close();
                    }));
                }
                else
                {
                    var alertForm = alertControl.AlertFormList.FirstOrDefault(o => Int64.Parse(o.Tag.ToString()) == alertId);
                    if (alertForm == null)
                        return;
                    alertControl.AlertFormList[alertControl.AlertFormList.IndexOf(alertForm)].Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static void TimerCallMessage()
        {
            try
            {
                timerCall = new System.Windows.Forms.Timer();
                timerCall.Interval = 100;
                timerCall.Tick += timerCall_Tick;
                timerCall.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static void timerCall_Tick(object sender, EventArgs e)
        {
            try
            {
                if (IsSpeech || alertControl == null || alertControl.AlertFormList == null || alertControl.AlertFormList.Count == 0)
                    return;
                Task taskCall = new Task(PlayAlert);
                taskCall.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static void PlayAlert()
        {
            try
            {
                if (!Directory.Exists(ALERT_SOUND_FOLDER))
                    Directory.CreateDirectory(ALERT_SOUND_FOLDER);
                List<string> fileList = Directory.GetFiles(ALERT_SOUND_FOLDER, "*.wav", SearchOption.TopDirectoryOnly).ToList();
                if (fileList != null && fileList.Count > 0)
                {
                    foreach (string s in fileList)
                    {
                        if (alertControl == null || alertControl.AlertFormList == null || alertControl.AlertFormList.Count == 0)
                            break;
                        else
                            IsSpeech = true;
                        System.Media.SoundPlayer snd = new System.Media.SoundPlayer(s);
                        snd.PlaySync();
                        Thread.Sleep(100);
                    }
                }
                IsSpeech = false;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private static void ActionRefreshCache()
        {
            try
            {
                WaitingManager.Hide();
                MessageManager.Show(String.Format("Phần mềm sẽ tự động tải lại dữ liệu danh mục trong ít phút. {0}", currentPubSub.Message));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static void ActionReset()
        {
            try
            {
                ProcessTokenLostBase();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private static void ProcessTokenLostBase()
        {
            try
            {
                WaitingManager.Hide();
                MessageManager.Show(String.Format("Phần mềm sẽ thực hiện khởi động lại. {0}", currentPubSub.Message));
                System.Diagnostics.Process.Start(Application.ExecutablePath);
                GlobalVariables.isLogouter = true;
                GlobalVariables.IsLostToken = true;
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("ProcessTokenLost fail.", ex);
            }
        }
    }
}
