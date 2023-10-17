﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.RegisterExamKiosk.Popup.RegisterExemKiosk;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Core;
using Inventec.Common.Adapter;
using MOS.SDO;
using Inventec.Desktop.Common.Message;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.RegisterExamKiosk.Popup.CheckHeinCardGOV;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.BackendData;
using His.Bhyt.InsuranceExpertise;
using His.Bhyt.InsuranceExpertise.LDO;
using System.Threading;
using HIS.Desktop.Plugins.RegisterExamKiosk.Popup.ChooseObject;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.RegisterExamKiosk.Config;
using HIS.Desktop.Plugins.RegisterExamKiosk.ADO;
using Inventec.Common.QrCodeBHYT;
using DevExpress.XtraEditors;
using Inventec.Common.QrCodeCCCD;
using MOS.LibraryHein.Bhyt;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.RegisterExamKiosk.Popup.RegisteredExam;

namespace HIS.Desktop.Plugins.RegisterExamKiosk
{
    public partial class frmWaitingScreen : HIS.Desktop.Utility.FormBase
    {

        /// <summary>
        /// Nghiệp vụ: 
        /// Thẻ được phát hành tại trung tâm phát hành thẻ
        /// Bệnh nhân quẹt thẻ=> check xem thẻ có hợp lệ hay không. Thẻ hợp lệ là thẻ có Patient_id >0
        /// Nếu thẻ hợp lệ gọi form tiếp theo để xử lí nghiệp vụ tiếp
        /// 
        /// Module sử dụng với phần mềm CDA để quẹt thẻ
        /// </summary>
        /// 

        #region Declare
        int indexImage;
        List<Image> listImage;

        bool isHandling = false;

        Inventec.Desktop.Common.Modules.Module currentModule;

        InformationObjectADO PatientData = null;

        bool isCallPrintForm = false;
        bool IsEmergency = false;
        #endregion

        #region Contructor
        public frmWaitingScreen()
        {
            InitializeComponent();
        }

        public frmWaitingScreen(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void frmWaitingScreen_Load(object sender, EventArgs e)
        {
            try
            {
                AppConfigs.LoadConfig();
                //Lấy ảnh từ thư mục trong HIS.Desktop
                getImageFromFile();

                LoadDefaultScreenSaver();

                //Load ảnh mặc định lên form 
                if (listImage != null && listImage.Count > 0)
                {
                    this.panelControl2.ContentImage = listImage[0];
                }

                //Thời gian hiển thị thông báo khi quẹt thẻ ms
                timerLabel.Interval = 3000;

                //Thời gian đổi hình nền
                timerWallPaper.Interval = 10000;

                RegisterTimer(currentModule.ModuleLink, "timerWallPaper", timerWallPaper.Interval, timerWallPaper_Tick);
                StartTimer(currentModule.ModuleLink, "timerWallPaper");
                RegisterTimer(currentModule.ModuleLink, "timerCheckFocus", timerCheckFocus.Interval, timerCheckFocus_Tick);
                StartTimer(currentModule.ModuleLink, "timerCheckFocus");
                lblMessage.Text = "HỆ THỐNG ĐĂNG KÝ KHÁM CHỮA BỆNH THÔNG MINH.";
                label2.Text = "XIN MỜI QUẸT THẺ HOẶC NHẬP SỐ THẺ\nCMND, CCCD, MÃ BỆNH NHÂN";
                //Mở host CDA
                InitWCFReadCard();
                txtNumberInput.Focus();
            }
            catch (Exception ex)
            {
                StopTimer(currentModule.ModuleLink, "timerWallPaper");
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Method
        private void getImageFromFile()
        {
            try
            {
                if (listImage != null && listImage.Count > 0) return;
                listImage = new List<Image>();
                //URL thư mục đã được cấu hình
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Inventec.UC.ImageLib.Base.LocalStore.LOCAL_STORAGE_PATH_KIOSK), Inventec.UC.ImageLib.Base.LocalStore.LOCAL_STORAGE_PATH_KIOSK));
                if (System.IO.Directory.Exists(Inventec.UC.ImageLib.Base.LocalStore.LOCAL_STORAGE_PATH_KIOSK))
                {
                    string[] fileEntries = System.IO.Directory.GetFiles(Inventec.UC.ImageLib.Base.LocalStore.LOCAL_STORAGE_PATH_KIOSK).OrderBy(f => f).ToArray();

                    if (fileEntries == null || fileEntries.Count() == 0)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Khong ton tai file anh trong thu muc. Path = " + Inventec.UC.ImageLib.Base.LocalStore.LOCAL_STORAGE_PATH_KIOSK + ". " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileEntries), fileEntries));
                    }
                    else
                    {
                        foreach (var item in fileEntries)
                        {
                            listImage.Add(Image.FromFile(item));
                        }
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("Khong ton tai thu muc: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Inventec.UC.ImageLib.Base.LocalStore.LOCAL_STORAGE_PATH_KIOSK), Inventec.UC.ImageLib.Base.LocalStore.LOCAL_STORAGE_PATH_KIOSK));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CardMessageFail()
        {
            try
            {
                WaitingManager.Hide();
                lblMessage.Text = "THẺ KHÔNG HỢP LỆ, HOẶC CHƯA ĐƯỢC KÍCH HOẠT. VUI LÒNG KIỂM TRA LẠI";
                RegisterTimer(currentModule.ModuleLink, "timerLabel", timerLabel.Interval, timerLabel_Tick);
                StartTimer(currentModule.ModuleLink, "timerLabel");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void loadKioskInformationData(object data)
        {
            try
            {
                if (data != null && data.GetType() == typeof(HisExamRegisterKioskSDO))
                {
                    this.PatientData.ExamRegisterKiosk = (HisExamRegisterKioskSDO)data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void closingForm(object patienType)
        {
            try
            {
                frmWaitingScreen_Load(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RegisterExamKiosk.Resources.Lang", typeof(HIS.Desktop.Plugins.RegisterExamKiosk.frmWaitingScreen).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmWaitingScreen.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HisPatientForKioskSDO GetPatientInfoByFilter(HisPatientAdvanceFilter filter)
        {
            try
            {
                CommonParam param = new CommonParam();
                var patientForKioskSDO = new BackendAdapter(param).Get<HisPatientForKioskSDO>("api/HisPatient/GetInformationForKiosk", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (patientForKioskSDO != null)
                {
                    return patientForKioskSDO;
                }

                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
        #endregion

        #region Card
        private void InitWCFReadCard()
        {
            try
            {
                if (CARD.WCF.Service.TapCardService.TapCardServiceManager.OpenHost())
                    CARD.WCF.Service.TapCardService.TapCardServiceManager.SetDelegate(CheckServiceCodeDelegate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetNull()
        {
            try
            {
                //  this.PatientData = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckServiceCodeDelegate(string serviceCode)
        {
            bool success = false;
            try
            {
                //Lấy thông tin của thẻ
                LogSystem.Info("Begin CheckServiceCodeDelegate");
                this.InitThreadGetCard(serviceCode);
                success = true;
                LogSystem.Info("End CheckServiceCodeDelegate");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private void InitThreadGetCard(string serviceCode)
        {
            try
            {
                Thread thread = new Thread(new ParameterizedThreadStart(this.ProcessGetCard));
                thread.Start(serviceCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessGetCard(object data)
        {
            try
            {
                string serviceCode = data.ToString();
                this.Invoke(new MethodInvoker(delegate()
                {
                    this.getCard(serviceCode);
                }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void getCard(string serviceCode)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("ServiceCode =" + serviceCode);
                if (isHandling)
                    return;
                isHandling = true;
                this.isCallPrintForm = false;
                WaitingManager.Show();
                PatientData = new InformationObjectADO(serviceCode);

                List<Task> taskAll = new List<Task>();
                Task tsPatient = Task.Factory.StartNew((object obj) =>
                {
                    CommonParam param_ = new CommonParam();
                    HisPatientAdvanceFilter filter_ = new HisPatientAdvanceFilter();
                    filter_.SERVICE_CODE__EXACT = obj.ToString();
                    var patientForKioskSDO = GetPatientInfoByFilter(filter_);
                    if (patientForKioskSDO != null)
                    {
                        this.PatientData.PatientForKiosk = patientForKioskSDO;
                    }

                }, serviceCode);
                taskAll.Add(tsPatient);

                Task tsCard = Task.Factory.StartNew((object obj) =>
                {
                    CommonParam param = new CommonParam();
                    var hisCardSdo = new BackendAdapter(param).Get<HisCardSDO>("api/HisCard/GetCardSdoByCode", ApiConsumer.ApiConsumers.MosConsumer, obj, param);
                    if (hisCardSdo != null)
                    {
                        this.PatientData.CardInfo = hisCardSdo;
                    }
                }, serviceCode);
                taskAll.Add(tsCard);

                Task.WaitAll(taskAll.ToArray());

                Inventec.Common.Logging.LogSystem.Debug("getCard__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PatientData), PatientData));
                if (this.PatientData.PatientForKiosk != null || this.PatientData.CardInfo != null)
                {
                    OpenFormByPatientData();
                }
                else
                {
                    SetNull();
                    CardMessageFail();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isHandling = false;
        }

        private void OpenFormByPatientData()
        {
            try
            {
                if (this.PatientData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("lỗi dữ liệu this.PatientData null");
                    return;
                }

                string HeinCardNumber = "";
                HeinCardData heinData = null;

                if (this.PatientData.PatientForKiosk != null)
                {
                    HeinCardNumber = this.PatientData.PatientForKiosk.HeinCardNumber;
                    if (!String.IsNullOrWhiteSpace(HeinCardNumber))
                    {
                        heinData = ConvertFromPatientData(this.PatientData.PatientForKiosk);
                    }
                }
                else if (this.PatientData.CardInfo != null)
                {
                    HeinCardNumber = this.PatientData.CardInfo.HeinCardNumber;
                    if (!String.IsNullOrWhiteSpace(HeinCardNumber))
                    {
                        heinData = ConvertFromPatientData(this.PatientData.CardInfo);
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("OpenFormByPatientData__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PatientData), PatientData));
                if (!string.IsNullOrEmpty(HeinCardNumber))
                {
                    WaitingManager.Show();
                    this.CheckheinCardFromHeinInsuranceApi(heinData);
                    WaitingManager.Hide();
                }
                else
                {
                    if (!this.isCallPrintForm && this.PatientData.PatientForKiosk != null && CheckInforPatient(this.PatientData.PatientForKiosk, this.PatientData.HeinInfo))
                    {
                        return;
                    }

                    WaitingManager.Hide();
                    frmChooseObject frmChoose = new frmChooseObject(loadFormRegisterExamKiosk);
                    frmChoose.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region openForm
        private void loadFormRegisterExamKiosk(object patienType)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patienType), patienType));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patienType.GetType()), patienType.GetType()));
                //Ẩn form hiện tại mở form frmRegisterExamKiosk
                NameForm.CloseAllForm();
                if (patienType != null && patienType.GetType() == typeof(long))
                {
                    long patientTypeId = (long)patienType;
                    var frm = new frmRegisterExamKiosk(PatientData, (HIS.Desktop.Common.DelegateRefreshData)SetNull, this.currentModule, (HIS.Desktop.Common.DelegateCloseForm_Uc)closingForm, patientTypeId);
                    frm.ShowDialog();
                }
                else
                {
                    var frm = new frmRegisterExamKiosk(PatientData, (HIS.Desktop.Common.DelegateRefreshData)SetNull, this.currentModule, (HIS.Desktop.Common.DelegateCloseForm_Uc)closingForm, HisConfigCFG.PATIENT_TYPE_ID__IS_FEE);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task CheckheinCardFromHeinInsuranceApi(HeinCardData dataHein)
        {
            try
            {
                long keyCheck = AppConfigs.CheDoTuDongCheckThongTinTheBHYT;
                if (keyCheck > 0)
                {
                    ResultHistoryLDO rsIns = null;
                    if (!this.isCallPrintForm && this.PatientData.HeinInfo == null)
                    {
                        rsIns = await CheckHanSDTheBHYT(dataHein);
                        this.PatientData.HeinInfo = rsIns;
                    }
                    else
                    {
                        rsIns = this.PatientData.HeinInfo;
                    }
                    if (rsIns != null && (rsIns.maKetQua == "000" || (!String.IsNullOrEmpty(rsIns.maTheMoi) && (!rsIns.maThe.Equals(rsIns.maTheMoi) || rsIns.maKetQua == "004"))))
                    {
                        if (this.PatientData.PatientForKiosk == null)
                        {
                            HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                            filter.HEIN_CARD_NUMBER__EXACT = rsIns.maThe;
                            if (!string.IsNullOrEmpty(filter.CCCD_NUMBER__EXACT))
                            {
                                filter.HEIN_CARD_NUMBER_OR_CCCD_NUMBER = new HeinCardNumberOrCccdNumber();
                                filter.HEIN_CARD_NUMBER_OR_CCCD_NUMBER.HEIN_CARD_NUMBER__EXACT = rsIns.maTheMoi ?? rsIns.maThe;
                                filter.HEIN_CARD_NUMBER_OR_CCCD_NUMBER.CCCD_NUMBER__EXACT = filter.CCCD_NUMBER__EXACT;
                            }
                            this.PatientData.PatientForKiosk = GetPatientInfoByFilter(filter);
                        }

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PatientData), PatientData));
                        if (!string.IsNullOrEmpty(rsIns.gtTheTu))
                        {
                            DateTime d;
                            if (DateTime.TryParseExact(rsIns.gtTheTu, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d))
                            {
                                long? fromTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(d);
                                if (this.PatientData.CardInfo != null)
                                {
                                    Inventec.Common.Logging.LogSystem.Warn("fromTime__1_" + fromTime);
                                    this.PatientData.CardInfo.HeinCardFromTime = fromTime;
                                }

                                if (this.PatientData.PatientForKiosk != null)
                                {
                                    Inventec.Common.Logging.LogSystem.Warn("fromTime__2_" + fromTime);
                                    this.PatientData.PatientForKiosk.HeinCardFromTime = fromTime;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(rsIns.gtTheDen))
                        {
                            DateTime d;
                            if (DateTime.TryParseExact(rsIns.gtTheDen, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d))
                            {
                                long? ToTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(d);
                                if (this.PatientData.CardInfo != null)
                                {
                                    Inventec.Common.Logging.LogSystem.Warn("fromTime__1_" + ToTime);
                                    this.PatientData.CardInfo.HeinCardToTime = ToTime;
                                }

                                if (this.PatientData.PatientForKiosk != null)
                                {
                                    Inventec.Common.Logging.LogSystem.Warn("fromTime__2_" + ToTime);
                                    this.PatientData.PatientForKiosk.HeinCardToTime = ToTime;
                                }
                            }
                        }

                        string mathe = "";
                        if (!String.IsNullOrWhiteSpace(rsIns.maTheMoi))
                        {
                            mathe = rsIns.maTheMoi;
                        }
                        else
                        {
                            mathe = rsIns.maThe;
                        }

                        if (this.PatientData.CardInfo != null)
                        {
                            this.PatientData.CardInfo.HeinCardNumber = mathe;
                            this.PatientData.CardInfo.HeinOrgCode = rsIns.maDKBD;
                            this.PatientData.CardInfo.HeinAddress = rsIns.diaChi;
                        }

                        if (this.PatientData.PatientForKiosk != null)
                        {
                            this.PatientData.PatientForKiosk.HeinCardNumber = mathe;
                            this.PatientData.PatientForKiosk.HeinMediOrgCode = rsIns.maDKBD;
                            this.PatientData.PatientForKiosk.HeinAddress = rsIns.diaChi;
                        }

                        if (!string.IsNullOrEmpty(rsIns.maDKBD))
                        {
                            var dataMediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(p => p.MEDI_ORG_CODE == rsIns.maDKBD.Trim());
                            if (dataMediOrg != null)
                            {
                                if (this.PatientData.CardInfo != null)
                                {
                                    this.PatientData.CardInfo.HeinOrgName = dataMediOrg.MEDI_ORG_NAME;
                                }

                                if (this.PatientData.PatientForKiosk != null)
                                {
                                    this.PatientData.PatientForKiosk.HeinMediOrgName = dataMediOrg.MEDI_ORG_NAME;
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(rsIns.ngaySinh))
                        {
                            DateTime d;
                            if (DateTime.TryParseExact(rsIns.ngaySinh, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d))
                            {
                                long Dob = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(d) ?? 0;
                                if (this.PatientData.CardInfo != null)
                                {
                                    this.PatientData.CardInfo.Dob = Dob;
                                }

                                if (this.PatientData.PatientForKiosk != null)
                                {
                                    this.PatientData.PatientForKiosk.DOB = Dob;
                                }
                            }
                        }

                        NameForm.CloseOtherForm();

                        if (!this.isCallPrintForm && this.PatientData.PatientForKiosk != null && CheckInforPatient(this.PatientData.PatientForKiosk, this.PatientData.HeinInfo))
                        {
                            return;
                        }

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PatientData), PatientData));
                        frmCheckHeinCardGOV frmCheck = new frmCheckHeinCardGOV(rsIns, loadKioskInformationData, loadFormRegisterExamKiosk, (HIS.Desktop.Common.DelegateRefreshData)SetNull, closingForm, this.PatientData.PatientForKiosk);
                        frmCheck.ShowDialog();
                    }
                    else
                    {
                        if (!this.isCallPrintForm && this.PatientData.PatientForKiosk != null && CheckInforPatient(this.PatientData.PatientForKiosk, this.PatientData.HeinInfo))
                        {
                            return;
                        }

                        WaitingManager.Hide();
                        frmChooseObject frmChoose = new frmChooseObject(loadFormRegisterExamKiosk);
                        frmChoose.ShowDialog();
                    }
                }
                else
                {
                    //không kiểm tra cổng vẫn check thông tuyến
                    loadFormRegisterExamKiosk(HisConfigCFG.PATIENT_TYPE_ID__BHYT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Check Hein GOV
        public HeinCardData ConvertFromPatientData(HisCardSDO patient)
        {
            HeinCardData hein = null;
            try
            {
                hein = new HeinCardData();
                hein.Address = patient.HeinAddress;
                if (patient.IsHasNotDayDob == 1)
                {
                    hein.Dob = patient.Dob.ToString().Substring(0, 4);
                }
                else
                {
                    hein.Dob = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.Dob);
                }
                hein.FromDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patient.HeinCardFromTime ?? 0));
                hein.ToDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patient.HeinCardToTime ?? 0));
                hein.MediOrgCode = patient.HeinOrgCode;
                hein.HeinCardNumber = patient.HeinCardNumber;
                hein.LiveAreaCode = patient.LiveAreaCode;
                hein.PatientName = patient.LastName + " " + patient.FirstName;
                hein.FineYearMonthDate = patient.Join5Year;
                hein.Gender = patient.GenderId == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? "1" : "2";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return hein;
        }

        private HeinCardData ConvertFromPatientData(HisPatientForKioskSDO patient)
        {
            HeinCardData hein = null;
            try
            {
                hein = new HeinCardData();
                hein.Address = patient.HeinAddress;
                if (patient.IS_HAS_NOT_DAY_DOB == 1)
                {
                    hein.Dob = patient.DOB.ToString().Substring(0, 4);
                }
                else
                {
                    hein.Dob = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.DOB);
                }
                hein.FromDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patient.HeinCardFromTime ?? 0));
                hein.ToDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patient.HeinCardToTime ?? 0));
                hein.MediOrgCode = patient.HeinMediOrgCode;
                hein.HeinCardNumber = patient.HeinCardNumber;
                hein.LiveAreaCode = patient.LiveAreaCode;
                hein.PatientName = patient.LAST_NAME + " " + patient.FIRST_NAME;
                hein.FineYearMonthDate = patient.Join5Year;
                hein.Gender = patient.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? "1" : "2";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return hein;
        }

        private HeinCardData ConvertFromPatientData(object data)
        {
            HeinCardData hein = null;
            try
            {
                hein = new HeinCardData();

                if (data is HeinCardData)
                {
                    hein = (HeinCardData)data;
                }
                else if (data is CccdCardData)
                {
                    var cccdCard = (CccdCardData)data;
                    hein.Dob = cccdCard.Dob;
                    hein.Address = cccdCard.Address;
                    hein.HeinCardNumber = cccdCard.CardData;
                    hein.PatientName = cccdCard.PatientName;
                    hein.Gender = cccdCard.Gender == "Nam" ? "1" : "2";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return hein;
        }

        private async Task<ResultHistoryLDO> CheckHanSDTheBHYT(HeinCardData dataHein)
        {
            ResultHistoryLDO reult = null;
            try
            {
                if (String.IsNullOrEmpty(dataHein.PatientName)
                    || String.IsNullOrEmpty(dataHein.Dob)
                    || String.IsNullOrEmpty(dataHein.HeinCardNumber))
                {
                    Inventec.Common.Logging.LogSystem.Info("Khong goi cong BHXH check thong tin the do du lieu truyen vao chua du du lieu bat buoc___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataHein), dataHein));
                    return reult;
                }

                CommonParam param = new CommonParam();
                ApiInsuranceExpertise apiInsuranceExpertise = new ApiInsuranceExpertise();
                CheckHistoryLDO checkHistoryLDO = new CheckHistoryLDO();
                checkHistoryLDO.maThe = dataHein.HeinCardNumber;
                checkHistoryLDO.ngaySinh = dataHein.Dob;
                checkHistoryLDO.hoTen = Inventec.Common.String.Convert.HexToUTF8Fix(dataHein.PatientName);
                checkHistoryLDO.hoTen = (String.IsNullOrEmpty(checkHistoryLDO.hoTen) ? dataHein.PatientName : checkHistoryLDO.hoTen);
                Inventec.Common.Logging.LogSystem.Info("CheckHanSDTheBHYT => 1");
                reult = await apiInsuranceExpertise.CheckHistory(BHXHLoginCFG.USERNAME, BHXHLoginCFG.PASSWORD, BHXHLoginCFG.ADDRESS, checkHistoryLDO, BHXHLoginCFG.ADDRESS_OPTION);

                Inventec.Common.Logging.LogSystem.Info("CheckHanSDTheBHYT => 2");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => reult), reult));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return reult;
        }
        #endregion

        #region InputNumber
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                CheckDataFromInputNumber();
                txtNumberInput.Texts = "";
                txtNumberInput.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckDataFromInputNumber()
        {
            try
            {
                this.isCallPrintForm = false;
                string txtInput = txtNumberInput.Texts;
                if (string.IsNullOrEmpty(txtInput.Replace(" ", "")))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng nhập thông tin trước khi xác nhận", "Thông báo");
                    return;
                }

                WaitingManager.Show();

                this.PatientData = new InformationObjectADO();

                CommonParam param = new CommonParam();
                HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                //quẹt qr sẽ check cổng và đăng ký theo thông tin cổng trả về
                if (txtInput.Contains("|"))
                {
                    var arrayCode = txtInput.Split('|').ToList();
                    List<int> lst = new List<int>() { 10, 12, 15 };
                    if (!lst.Contains(arrayCode[0].Length))
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy thông tin theo số bạn vừa nhập. Vui lòng qua phòng tiếp đón để đăng ký khám", "Thông báo");
                        return;
                    }
                    var data = SearchByCode(txtInput);
                    if (data == null)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("QRCode không tồn tại", "thông báo");
                        return;
                    }
                    CccdCardData cccdCard = new CccdCardData();
                    HeinCardData heinCardDataForCheckGOV = new HeinCardData();

                    if (data is HeinCardData)
                    {
                        heinCardDataForCheckGOV = (HeinCardData)data;
                        filter.HEIN_CARD_NUMBER__EXACT = heinCardDataForCheckGOV.HeinCardNumber;
                    }
                    else if (data is CccdCardData)
                    {
                        cccdCard = (CccdCardData)data;
                        filter.CCCD_NUMBER__EXACT = cccdCard.CardData;
                    }

                    WaitingManager.Show();
                    this.CheckHanSDTheBHYTVaCccd(ConvertFromPatientData(data), filter);
                    WaitingManager.Hide();
                }
                else
                {
                    txtInput = txtInput.Replace(" ", "");
                    if (txtInput.Length != 16 && txtInput.Length != 12 && txtInput.Length != 10 && txtInput.Length != 9)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Số bạn nhập không hợp lệ, vui lòng kiểm tra lại", "Thông báo");
                        txtNumberInput.Texts = txtNumberInput.Texts.Replace(" ", "");
                        txtNumberInput.Focus();
                        return;
                    }
                    if (txtInput.Length == 16)
                        filter.CARD_CODE__EXACT = txtInput;
                    else if (txtInput.Length == 12)
                    {
                        filter.CCCD_NUMBER__EXACT = txtInput;
                        filter.HEIN_CARD_NUMBER_OR_CCCD_NUMBER = new HeinCardNumberOrCccdNumber();
                        filter.HEIN_CARD_NUMBER_OR_CCCD_NUMBER.CCCD_NUMBER__EXACT = txtInput;
                    }
                    else if (txtInput.Length == 10)
                        filter.PATIENT_CODE__EXACT = txtInput;
                    else if (txtInput.Length == 9)
                        filter.CMND_NUMBER__EXACT = txtInput;

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("filter____api/HisPatient/GetInformationForKiosk____", filter));


                    this.PatientData.PatientForKiosk = GetPatientInfoByFilter(filter);

                    if (this.PatientData.PatientForKiosk == null)
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy thông tin theo số bạn vừa nhập. Vui lòng qua phòng tiếp đón để đăng ký khám", "Thông báo");
                        return;
                    }
                    else
                    {
                        OpenFormByPatientData();
                    }
                }
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefaultScreenSaver()
        {
            try
            {

                List<object> _listObj = new List<object>();
                WaitingManager.Hide();
                var SCREEN_SAVER = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId);
                if (SCREEN_SAVER != null)
                {
                    var dtDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == SCREEN_SAVER.DEPARTMENT_ID);
                    if (dtDepartment != null)
                    {
                        IsEmergency = dtDepartment.IS_EMERGENCY == (short?)1;
                    }
                    if (!string.IsNullOrEmpty(SCREEN_SAVER.SCREEN_SAVER_MODULE_LINK))
                    {
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(SCREEN_SAVER.SCREEN_SAVER_MODULE_LINK, this.currentModule.RoomId, this.currentModule.RoomTypeId, _listObj);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool CheckInforPatient(HisPatientForKioskSDO patientForKioskSDO, ResultHistoryLDO rsIns = null)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("patientForKioskSDO____", patientForKioskSDO));
                if (patientForKioskSDO != null)
                {
                    var dataTime = Inventec.Common.DateTime.Get.StartDay() ?? 99999999999999;
                    WaitingManager.Hide();
                    if (patientForKioskSDO.IsPause != 1 && patientForKioskSDO.ServiceReqs != null && patientForKioskSDO.ServiceReqs.Exists(p => p.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH) && patientForKioskSDO.InDate >= dataTime)
                    {
                        this.isCallPrintForm = true;
                        frmRegisteredExam frm = new frmRegisteredExam(this.currentModule, this.PatientData, loadFormRegisterExamKiosk, OpenFormByPatientData, patientForKioskSDO, IsEmergency);
                        frm.ShowDialog();
                        return true;
                    }
                    string message = "";
                    if (HisConfigCFG.CHECK_PREVIOUS_DEBT_OPTION == "1" && patientForKioskSDO.PreviousDebtTreatments != null
                        && patientForKioskSDO.PreviousDebtTreatments.Count > 0)
                    {
                        string treatmentPrevis = String.Join(",", patientForKioskSDO.PreviousDebtTreatments.Distinct().ToList());
                        message += String.Format("Đợt khám/điều trị trước đó của bệnh nhân còn nợ tiền viện phí. Mã hồ sơ điều trị {0}.", treatmentPrevis);

                        if (DevExpress.XtraEditors.XtraMessageBox.Show(message + " Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            return true;
                        }
                    }
                    else if (HisConfigCFG.CHECK_PREVIOUS_DEBT_OPTION == "3" && patientForKioskSDO.LastTreatmentFee != null)
                    {
                        var soTienBnPhaiNopThem = patientForKioskSDO.LastTreatmentFee.TOTAL_PATIENT_PRICE - patientForKioskSDO.LastTreatmentFee.TOTAL_DEPOSIT_AMOUNT - patientForKioskSDO.LastTreatmentFee.TOTAL_BILL_AMOUNT + patientForKioskSDO.LastTreatmentFee.TOTAL_BILL_TRANSFER_AMOUNT + patientForKioskSDO.LastTreatmentFee.TOTAL_REPAY_AMOUNT;
                        if (patientForKioskSDO.LastTreatmentFee.IS_ACTIVE == 1 || soTienBnPhaiNopThem > 0)
                        {
                            message += String.Format("Đợt khám/điều trị trước đó của bệnh nhân có số tiền phải trả > 0 hoặc chưa duyệt khóa viện phí. Mã hồ sơ điều trị {0}.", patientForKioskSDO.LastTreatmentFee.TREATMENT_CODE);
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                            {
                                return true;
                            }

                        }
                    }
                    else if (HisConfigCFG.CHECK_PREVIOUS_DEBT_OPTION == "2" && !IsEmergency && patientForKioskSDO.PreviousDebtTreatmentDetails != null
                       && patientForKioskSDO.PreviousDebtTreatmentDetails.Count > 0)
                    {
                        var dtTreatmentDetails = patientForKioskSDO.PreviousDebtTreatmentDetails.Where(o => o.PATIENT_TYPE_ID == HisConfigCFG.PATIENT_TYPE_ID__BHYT).ToList();

                        if (dtTreatmentDetails != null && dtTreatmentDetails.Count > 0)
                        {
                            string treatmentPrevis = String.Join(",", dtTreatmentDetails.Select(o => o.TDL_TREATMENT_CODE).ToList());
                            message += String.Format("Đợt khám/điều trị trước đó của bệnh nhân còn nợ viện phí. Mã hồ sơ điều trị {0}. Không cho phép tiếp đón", treatmentPrevis);
                            DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK);
                            return true;
                        }
                    }
                    else if (HisConfigCFG.CHECK_PREVIOUS_DEBT_OPTION == "4" && patientForKioskSDO.PreviousDebtTreatments != null
                            && patientForKioskSDO.PreviousDebtTreatments.Count > 0)
                    {
                        string treatmentPrevis = String.Join(",", patientForKioskSDO.PreviousDebtTreatments.Distinct().ToList());
                        message += String.Format("Đợt khám/điều trị trước đó của bệnh nhân có số tiền phải trả lớn hơn 0 hoặc chưa duyệt khóa viện phí. Mã hồ sơ điều trị {0}.", treatmentPrevis);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        private async Task CheckHanSDTheBHYTVaCccd(HeinCardData card, HisPatientAdvanceFilter filter)
        {
            try
            {
                ResultHistoryLDO rsIns = null;
                if (this.PatientData.HeinInfo == null)
                {
                    rsIns = await CheckHanSDTheBHYT(card);
                    this.PatientData.HeinInfo = rsIns;
                }
                else
                {
                    rsIns = this.PatientData.HeinInfo;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsIns), rsIns));
                if (rsIns != null && (rsIns.maKetQua == "000" || (!String.IsNullOrEmpty(rsIns.maTheMoi) && (!rsIns.maThe.Equals(rsIns.maTheMoi) || rsIns.maKetQua == "004"))))
                {

                    var newFilter = new HisPatientAdvanceFilter();
                    if (!string.IsNullOrEmpty(filter.CCCD_NUMBER__EXACT))
                    {
                        newFilter.HEIN_CARD_NUMBER_OR_CCCD_NUMBER = new HeinCardNumberOrCccdNumber();
                        newFilter.HEIN_CARD_NUMBER_OR_CCCD_NUMBER.HEIN_CARD_NUMBER__EXACT = rsIns.maTheMoi;
                        newFilter.HEIN_CARD_NUMBER_OR_CCCD_NUMBER.CCCD_NUMBER__EXACT = filter.CCCD_NUMBER__EXACT;
                    }
                    if (!String.IsNullOrWhiteSpace(rsIns.maTheMoi))
                        newFilter.HEIN_CARD_NUMBER__EXACT = rsIns.maTheMoi;
                    else
                        newFilter.HEIN_CARD_NUMBER__EXACT = rsIns.maThe;

                    this.PatientData.PatientForKiosk = GetPatientInfoByFilter(newFilter);

                    if (!this.isCallPrintForm && this.PatientData.PatientForKiosk != null && CheckInforPatient(this.PatientData.PatientForKiosk, this.PatientData.HeinInfo))
                    {
                        return;
                    }

                    // QR mới chưa đăng ký tại viện
                    if (this.PatientData.PatientForKiosk == null)
                    {
                        card.MediOrgCode = rsIns.maDKBD;
                        card.Address = rsIns.diaChi;
                        card.HeinCardNumber = rsIns.maThe;
                        this.PatientData.PatientForKiosk = MapDataFromCard(card);
                    }

                    if (!String.IsNullOrWhiteSpace(rsIns.gtTheTu) && rsIns.gtTheTu.Split('/') != null && rsIns.gtTheTu.Split('/').Count() > 2)
                    {
                        this.PatientData.PatientForKiosk.HeinCardFromTime = Int64.Parse(rsIns.gtTheTu.Split('/')[2] + rsIns.gtTheTu.Split('/')[1] + rsIns.gtTheTu.Split('/')[0] + "000000");
                    }
                    if (!String.IsNullOrWhiteSpace(rsIns.gtTheDen) && rsIns.gtTheDen.Split('/') != null && rsIns.gtTheDen.Split('/').Count() > 2)
                    {
                        this.PatientData.PatientForKiosk.HeinCardToTime = Int64.Parse(rsIns.gtTheDen.Split('/')[2] + rsIns.gtTheDen.Split('/')[1] + rsIns.gtTheDen.Split('/')[0] + "000000");
                    }
                    this.PatientData.PatientForKiosk.HeinMediOrgCode = rsIns.maDKBD;
                    var MediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(p => p.MEDI_ORG_CODE == rsIns.maDKBD.Trim());
                    if (MediOrg != null)
                        this.PatientData.PatientForKiosk.HeinMediOrgName = MediOrg.MEDI_ORG_NAME;
                    frmCheckHeinCardGOV frmCheck = new frmCheckHeinCardGOV(rsIns, loadKioskInformationData, loadFormRegisterExamKiosk, (HIS.Desktop.Common.DelegateRefreshData)SetNull, closingForm, this.PatientData.PatientForKiosk);
                    frmCheck.ShowDialog();
                }
                else
                {
                    filter.HEIN_CARD_NUMBER_OR_CCCD_NUMBER = new HeinCardNumberOrCccdNumber();
                    if (rsIns != null)
                    {

                        filter.HEIN_CARD_NUMBER_OR_CCCD_NUMBER.HEIN_CARD_NUMBER__EXACT = rsIns.maTheMoi ?? rsIns.maThe;
                    }
                    if (!string.IsNullOrEmpty(filter.CCCD_NUMBER__EXACT))
                    {
                        filter.HEIN_CARD_NUMBER_OR_CCCD_NUMBER.CCCD_NUMBER__EXACT = filter.CCCD_NUMBER__EXACT;
                    }

                    this.PatientData.PatientForKiosk = GetPatientInfoByFilter(filter);

                    if (!this.isCallPrintForm && this.PatientData.PatientForKiosk != null && CheckInforPatient(this.PatientData.PatientForKiosk, this.PatientData.HeinInfo))
                    {
                        return;
                    }

                    // QR mới chưa đăng ký tại viện
                    if (this.PatientData.PatientForKiosk == null)
                    {
                        card.HeinCardNumber = null;
                        this.PatientData.PatientForKiosk = MapDataFromCard(card);
                    }

                    frmChooseObject frmChoose = new frmChooseObject(loadFormRegisterExamKiosk);
                    frmChoose.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HisPatientForKioskSDO MapDataFromCard(HeinCardData card)
        {
            HisPatientForKioskSDO result = new HisPatientForKioskSDO();
            try
            {

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => card), card));
                result.HeinAddress = card.Address;
                if (!string.IsNullOrEmpty(card.PatientName))
                {
                    if (!card.PatientName.Contains(" "))
                        card.PatientName = Inventec.Common.String.Convert.HexToUTF8Fix(card.PatientName);
                    var spltName = card.PatientName.Split(' ');
                    result.FIRST_NAME = spltName.Last().ToUpper();
                    result.LAST_NAME = card.PatientName.Substring(0, card.PatientName.LastIndexOf(" ")).ToUpper();
                }
                result.GENDER_ID = Int64.Parse(card.Gender == "2" ? "1" : "2");
                result.CCCD_NUMBER = CccdPatientLocalADO.CccdNumber;
                result.CCCD_DATE = CccdPatientLocalADO.CccdDate;
                result.HeinCardNumber = card.HeinCardNumber;
                if (!String.IsNullOrWhiteSpace(card.FromDate) && card.FromDate.Split('/') != null && card.FromDate.Split('/').Count() > 2)
                {
                    result.HeinCardFromTime = Int64.Parse(card.FromDate.Split('/')[2] + card.FromDate.Split('/')[1] + card.FromDate.Split('/')[0] + "000000");
                }
                if (!String.IsNullOrWhiteSpace(card.ToDate) && card.ToDate.Split('/') != null && card.ToDate.Split('/').Count() > 2)
                {
                    result.HeinCardToTime = Int64.Parse(card.ToDate.Split('/')[2] + card.ToDate.Split('/')[1] + card.ToDate.Split('/')[0] + "000000");
                }
                if (!string.IsNullOrEmpty(card.MediOrgCode))
                {
                    result.HeinMediOrgCode = card.MediOrgCode;
                    var MediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(p => p.MEDI_ORG_CODE == card.MediOrgCode.Trim());
                    if (MediOrg != null)
                        result.HeinMediOrgName = MediOrg.MEDI_ORG_NAME;
                }
                result.LiveAreaCode = card.LiveAreaCode;
                result.RELATIVE_NAME = card.ParentName;
                if (!string.IsNullOrEmpty(card.Dob))
                {
                    string dtDate = card.Dob.Replace("/", "");
                    result.DOB = Int64.Parse(dtDate.Substring(4, 4) + dtDate.Substring(2, 2) + dtDate.Substring(0, 2) + "000000");
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private object SearchByCode(string code)
        {
            try
            {
                var arrayCode = code.Split('|').ToList();
                if (arrayCode[0].Length == 10 || arrayCode[0].Length == 15)
                {
                    return GetDataQrCodeHeinCard(code);
                }
                else if (arrayCode[0].Length == 12)
                {
                    return GetDataQrCodeCccdCard(code);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        private HeinCardData GetDataQrCodeHeinCard(string qrCode)
        {
            HeinCardData dataHein = null;
            try
            {
                //Lay thong tin tren th BHYT cua benh nhan khi quet the doc chuoi qrcode
                ReadQrCodeHeinCard readQrCode = new ReadQrCodeHeinCard();
                dataHein = readQrCode.ReadDataQrCode(qrCode);

                BhytHeinProcessor _BhytHeinProcessor = new BhytHeinProcessor();
                if (!_BhytHeinProcessor.IsValidHeinCardNumber(dataHein.HeinCardNumber))
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return dataHein;
        }

        private CccdCardData GetDataQrCodeCccdCard(string qrCode)
        {
            CccdCardData dataCccd = null;
            try
            {
                dataCccd = ReadQrCodeCCCD.ReadDataQrCode(qrCode);
                var dtDate = dataCccd.ReleaseDate.Replace("/", "");
                CccdPatientLocalADO.CccdNumber = dataCccd.CardData;
                CccdPatientLocalADO.CccdDate = Int64.Parse(dtDate.Substring(4, 4) + dtDate.Substring(2, 2) + dtDate.Substring(0, 2) + "000000");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return dataCccd;
        }
        #endregion

        #region Event
        private void frmWaitingScreen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerLabel_Tick()
        {
            try
            {
                lblMessage.Text = "HỆ THỐNG ĐĂNG KÝ KHÁM CHỮA BỆNH THÔNG MINH.";
                label2.Text = "XIN MỜI QUẸT THẺ HOẶC NHẬP SỐ THẺ\nCMND, CCCD, MÃ BỆNH NHÂN";
                StopTimer(currentModule.ModuleLink, "timerLabel");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerWallPaper_Tick()
        {
            try
            {
                if (listImage != null && listImage.Count > 0)
                {
                    indexImage++;
                    if (indexImage == listImage.Count)
                    {
                        indexImage = 0;
                    }
                    this.panelControl2.ContentImage = listImage[indexImage];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmWaitingScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // Đóng host CDA
                Task.Factory.StartNew(() => { CARD.WCF.Service.TapCardService.TapCardServiceManager.CloseHost(); });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNumberInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    btnConfirm.Focus();
                    btnConfirm_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFormatString()
        {
            try
            {
                string check = txtNumberInput.Texts.Replace(" ", "");
                string num1 = "";
                string num2 = "";
                string num3 = "";
                string num4 = "";
                if (check.Length <= 10)
                {
                    for (int i = 0; i < check.Length; i++)
                    {
                        if (i < 3)
                            num1 += check[i];
                        else if (i < 6)
                            num2 += check[i];
                        else
                            num3 += check[i];
                    }

                    txtNumberInput.Texts = string.Format("{0} {1} {2}", num1, num2, num3);
                }
                else
                {
                    for (int i = 0; i < check.Length; i++)
                    {
                        if (i < 4)
                            num1 += check[i];
                        else if (i < 8)
                            num2 += check[i];
                        else if (i < 12)
                            num3 += check[i];
                        else
                            num4 += check[i];
                    }

                    txtNumberInput.Texts = string.Format("{0} {1} {2} {3}", num1, num2, num3, num4);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerCheckFocus_Tick()
        {
            try
            {
                if (!txtNumberInput.Focused) txtNumberInput.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
