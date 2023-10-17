﻿using ACS.EFMODEL.DataModels;
using ACS.Filter;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.ServiceReqUpdateInstruction.Resources;
using HIS.Desktop.Plugins.ServiceReqUpdateInstruction.Validation;
using HIS.Desktop.Plugins.ServiceReqUpdateInstruction.Validtion;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LocalStorage.Location;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceReqUpdateInstruction
{
    public partial class frmServiceReqUpdateInstruction : HIS.Desktop.Utility.FormBase
    {
        private Inventec.Desktop.Common.Modules.Module module;
        private long service_req_id;
        private V_HIS_SERVICE_REQ currentServiceReq = null;
        private HIS_TREATMENT currentTreatment = null;
        //private HIS_SERVICE_REQ currentServiceReq1 = null;
        private HIS.Desktop.Common.RefeshReference refeshData;
        private List<MOS.EFMODEL.DataModels.HIS_ICD> listIcd;
        internal List<HIS_USER_ROOM> _UserRoom { get; set; }
        private IcdProcessor icdProcessor;
        private UserControl ucIcd;
        private SecondaryIcdProcessor subIcdProcessor;
        private UserControl ucSecondaryIcd;
        private int positionHandleControl = -1;
        private string LoggingName = "";
        internal string CheckIcdWhenSave = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<String>("HIS.Desktop.Plugins.CheckIcdWhenSave");

        internal HIS.UC.Icd.IcdProcessor IcdCauseProcessor { get; set; }
        internal UserControl ucIcdCause;
        List<V_HIS_SERVICE> services;

        public frmServiceReqUpdateInstruction(Inventec.Desktop.Common.Modules.Module _module, long _service_req_id, HIS.Desktop.Common.RefeshReference _refeshData)
            : base(_module)
        {
            InitializeComponent();
            this.module = _module;
            this.service_req_id = _service_req_id;
            this.refeshData = _refeshData;
            LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            SetCaptionByLanguageKey();
            listIcd = BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
        }

        private void frmServiceReqUpdateInstruction_Load(object sender, EventArgs e)
        {
            WaitingManager.Show();
            InitUcCauseIcd();
            GetData();
            GetTreatment();
            VisibleLayout();
            InitUcIcd();
            InitUcSecondaryIcd();
            FillDataCommandToControl(this.currentServiceReq);
            LoadUser();
            ValidControlInform();
            SetIcon();
            dtTime.Focus();
            dtTime.SelectAll();
            InitEnabledControl();
            services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>();
            WaitingManager.Hide();
        }

        private void VisibleLayout()
        {
            try
            {
                VisibleResultApprover();
                VisibleAppointmentTimeAndDes();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void VisibleAppointmentTimeAndDes()
        {
            try
            {
                if (currentServiceReq != null && currentServiceReq.APPOINTMENT_TIME != null)
                {
                    lciAppointmentTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciEmptyAppointmentTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciAppointmentDes.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    lciAppointmentTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciEmptyAppointmentTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciAppointmentDes.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void VisibleResultApprover()
        {
            try
            {
                if (currentServiceReq != null && currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                {
                    lciResultApprover.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciComboApprover.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    lciResultApprover.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciComboApprover.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcIcd()
        {
            try
            {
                long autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd");
                icdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.IsUCCause = false;
                ado.DelegateNextFocus = NextForcusSubIcd;
                ado.DelegateRequiredCause = LoadRequiredCause;
                ado.DelegateRefreshSubIcd = LoadSubIcd;
                ado.Width = 440;
                ado.Height = 24;
                ado.IsColor = true;
                ado.DataIcds = listIcd;
                ado.AutoCheckIcd = autoCheckIcd == 1 ? true : false;
                ado.hisTreatment = currentTreatment;
                ucIcd = (UserControl)icdProcessor.Run(ado);

                if (ucIcd != null)
                {
                    this.panelControlUcIcd.Controls.Add(ucIcd);
                    ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadSubIcd(string icdCodes, string icdNames)
        {
            try
            {
                SecondaryIcdDataADO data = new SecondaryIcdDataADO();
                data.ICD_SUB_CODE = icdCodes;
                data.ICD_TEXT = icdNames;
                if (this.subIcdProcessor != null && this.ucSecondaryIcd != null)
                {
                    this.subIcdProcessor.SetAttachIcd(this.ucSecondaryIcd, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusSubIcd()
        {
            try
            {
                NextForcusSubIcdToDo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcCauseIcd()
        {
            try
            {
                long autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd");
                this.IcdCauseProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.IsUCCause = true;
                ado.DelegateNextFocus = NextForcusSubIcdCause;
                ado.Width = 440;
                ado.LblIcdMain = "NN ngoài:";
                ado.ToolTipsIcdMain = "Nguyên nhân ngoài";
                ado.Height = 24;
                ado.IsColor = false;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_CAUSE == 1).OrderBy(o => o.ICD_CODE).ToList();
                ado.AutoCheckIcd = autoCheckIcd == 1 ? true : false;
                this.ucIcdCause = (UserControl)this.IcdCauseProcessor.Run(ado);

                if (this.ucIcdCause != null)
                {
                    this.panelControlCauseIcd.Controls.Add(this.ucIcdCause);
                    this.ucIcdCause.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadRequiredCause(bool isRequired)
        {
            try
            {
                if (this.IcdCauseProcessor != null && this.ucIcdCause != null)
                {
                    this.IcdCauseProcessor.SetRequired(this.ucIcdCause, isRequired);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusSubIcdCause()
        {
            try
            {
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.FocusControl(ucSecondaryIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusSubIcdToDo()
        {
            try
            {
                if (IcdCauseProcessor != null && ucIcdCause != null)
                {
                    IcdCauseProcessor.FocusControl(ucIcdCause);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdCauseToControl(string icdCode, string icdName)
        {
            try
            {
                UC.Icd.ADO.IcdInputADO icd = new UC.Icd.ADO.IcdInputADO();
                icd.ICD_CODE = icdCode;
                icd.ICD_NAME = icdName;
                if (this.ucIcdCause != null)
                {
                    this.IcdCauseProcessor.Reload(this.ucIcdCause, icd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadUser()
        {
            try
            {

                List<ACS_USER> listResult = BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                // Get listemployee
                List<HIS_EMPLOYEE> listHisEmployee;
                if (BackendDataWorker.IsExistsKey<HIS_EMPLOYEE>())
                {
                    listHisEmployee = BackendDataWorker.Get<HIS_EMPLOYEE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic dynamicFilter = new System.Dynamic.ExpandoObject();
                    listHisEmployee = await new BackendAdapter(paramCommon).GetAsync<List<HIS_EMPLOYEE>>("api/HisEmployee/Get", ApiConsumers.MosConsumer, dynamicFilter, paramCommon);

                    if (listHisEmployee != null) BackendDataWorker.UpdateToRam(typeof(HIS_EMPLOYEE), listHisEmployee, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                var listLoginNameEmployee = listHisEmployee != null ? listHisEmployee.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(p => p.LOGINNAME).ToList() : null;

                if (listLoginNameEmployee != null && listLoginNameEmployee.Count > 0)
                {
                    listResult = listResult.Where(o => listLoginNameEmployee.Contains(o.LOGINNAME)).ToList();
                }

                this._UserRoom = new List<HIS_USER_ROOM>();
                MOS.Filter.HisUserRoomFilter filter = new HisUserRoomFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.ROOM_ID = currentServiceReq.EXECUTE_ROOM_ID;

                Inventec.Common.Logging.LogSystem.Debug("currentServiceReq.EXECUTE_ROOM_ID: " + currentServiceReq.EXECUTE_ROOM_ID);
                this._UserRoom = new BackendAdapter(new CommonParam()).Get<List<HIS_USER_ROOM>>("api/HisUserRoom/Get", ApiConsumers.MosConsumer, filter, null);
                List<string> listLoginNameHandler = _UserRoom.Select(o => o.LOGINNAME).ToList();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listLoginNameHandler), listLoginNameHandler));
                List<ACS_USER> listHandler = listResult.Where(o => listLoginNameHandler.Contains(o.LOGINNAME)).ToList();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listHandler), listHandler));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(cboEndServiceReq, listResult, controlEditorADO);
                ControlEditorLoader.Load(cboRequestUser, listResult, controlEditorADO);
                ControlEditorLoader.Load(cboResultApprover, listResult, controlEditorADO);
                ControlEditorLoader.Load(cboConsultant, listResult, controlEditorADO);
                ControlEditorLoader.Load(cboHandler, listHandler, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlInform()
        {
            ValidControlDtInstructionTime();
            ValidControlDtStartTime();
            ValidControlCboUser();
            if (currentServiceReq != null && currentServiceReq.APPOINTMENT_TIME != null)
            {
                ValidationSingleControl(dtAppointmentTime, dxValidationProvider1);
            }
        }

        private void ValidControlDtInstructionTime()
        {
            try
            {
                InstructionTimeValidationRule instructionTimeRule = new InstructionTimeValidationRule();
                instructionTimeRule.dtInstructionTime = this.dtTime;
                instructionTimeRule.dtStartTime = this.dtStartTime;
                dxValidationProvider1.SetValidationRule(this.dtTime, instructionTimeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlDtStartTime()
        {
            try
            {
                StartTimeValidationRule startTimeRule = new StartTimeValidationRule();
                startTimeRule.dtStartTime = this.dtStartTime;
                startTimeRule.dtEndTime = this.dtEndTime;
                dxValidationProvider1.SetValidationRule(this.dtStartTime, startTimeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlCboUser()
        {
            try
            {
                RequestUserValidationRule rule = new RequestUserValidationRule();
                rule.cboUser = cboRequestUser;
                rule.txtLoginname = txtRequestUser;
                dxValidationProvider1.SetValidationRule(txtRequestUser, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSecondaryIcd()
        {
            try
            {
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), listIcd);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.Width = 440;
                ado.Height = 24;
                ado.TextLblIcd = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.TextNullValue = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.cboSub.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ado.hisTreatment = currentTreatment;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.panelControlSubIcd.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                cboConsultant.EditValue = LoggingName;
                txtConsultant.Text = LoggingName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPayFormCombo(string _payFormCode)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> listResult = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                listResult = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => (o.LOGINNAME != null && o.LOGINNAME.Equals(_payFormCode))).ToList();

                if (listResult.Count == 1)
                {
                    cboEndServiceReq.EditValue = listResult[0].LOGINNAME;
                    txtLoginname.Text = listResult[0].LOGINNAME;
                    txtRequestUser.Focus();
                    txtRequestUser.SelectAll();
                }
                else if (listResult.Count > 1)
                {
                    cboEndServiceReq.EditValue = null;
                    cboEndServiceReq.Focus();
                    cboEndServiceReq.ShowPopup();
                }
                else
                {
                    cboEndServiceReq.EditValue = null;
                    cboEndServiceReq.Focus();
                    cboEndServiceReq.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPayFormComboRequest(string _payFormCode)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> listResult = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                listResult = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => (o.LOGINNAME != null && o.LOGINNAME.Equals(_payFormCode))).ToList();

                if (listResult.Count == 1)
                {
                    cboRequestUser.EditValue = listResult[0].LOGINNAME;
                    txtRequestUser.Text = listResult[0].LOGINNAME;
                    this.icdProcessor.FocusControl(this.ucIcd);
                }
                else if (listResult.Count > 1)
                {
                    cboRequestUser.EditValue = null;
                    cboRequestUser.Focus();
                    cboRequestUser.ShowPopup();
                }
                else
                {
                    cboRequestUser.EditValue = null;
                    cboRequestUser.Focus();
                    cboRequestUser.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusOut()
        {
            try
            {
                btnSave.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ServiceReqUpdateInstruction.Resources.Lang", typeof(HIS.Desktop.Plugins.ServiceReqUpdateInstruction.frmServiceReqUpdateInstruction).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciThoiGianYLenh.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciStartTime.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.lciStartTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEndTime.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.lciEndTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciUserName.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.lciUserName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmInstructionUpdate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetData()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.ID = service_req_id;
                currentServiceReq = new BackendAdapter(param)
                    .Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataCommandToControl(V_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                if (currentServiceReq != null)
                {
                    LoadDobPatientToForm(serviceReq);
                    IcdInputADO icd = new IcdInputADO();
                    icd.ICD_CODE = serviceReq.ICD_CODE;
                    icd.ICD_NAME = serviceReq.ICD_NAME;

                    if (ucIcd != null)
                    {
                        icdProcessor.Reload(ucIcd, icd);
                    }

                    SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                    subIcd.ICD_SUB_CODE = serviceReq.ICD_SUB_CODE;
                    subIcd.ICD_TEXT = serviceReq.ICD_TEXT;
                    if (ucSecondaryIcd != null)
                    {
                        subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                    }

                    txtLoginname.Text = serviceReq.EXECUTE_LOGINNAME;
                    cboEndServiceReq.EditValue = serviceReq.EXECUTE_LOGINNAME;

                    txtRequestUser.Text = serviceReq.REQUEST_LOGINNAME;
                    cboRequestUser.EditValue = serviceReq.REQUEST_LOGINNAME;
                    mmNOTE.Text = serviceReq.NOTE;

                    if (serviceReq.CONSULTANT_LOGINNAME == null)
                    {
                        SetDefaultValue();
                    }
                    else
                    {
                        txtConsultant.Text = serviceReq.CONSULTANT_LOGINNAME;
                        cboConsultant.EditValue = serviceReq.CONSULTANT_LOGINNAME;
                    }

                    if (serviceReq.ASSIGNED_EXECUTE_LOGINNAME != null)
                    {

                        txtHandler.Text = serviceReq.ASSIGNED_EXECUTE_LOGINNAME;
                        cboHandler.EditValue = serviceReq.ASSIGNED_EXECUTE_LOGINNAME;
                    }
                    if (currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                    {
                        txtResultApproverLoginname.Text = currentServiceReq.RESULT_APPROVER_LOGINNAME ?? "";
                        cboResultApprover.EditValue = currentServiceReq.RESULT_APPROVER_LOGINNAME;
                    }

                    if (serviceReq.START_TIME != null)
                    {
                        dtStartTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.START_TIME ?? 0) ?? DateTime.MinValue;
                    }
                    if (serviceReq.FINISH_TIME != null)
                    {
                        dtEndTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.FINISH_TIME ?? 0) ?? DateTime.MinValue;
                    }

                    if (!string.IsNullOrEmpty(serviceReq.ICD_CAUSE_CODE))
                    {
                        var dataIcd = BackendDataWorker.Get<HIS_ICD>().FirstOrDefault(p => p.ICD_CODE == serviceReq.ICD_CAUSE_CODE.Trim());
                        if (dataIcd != null)
                        {
                            LoadIcdCauseToControl(dataIcd.ICD_CODE, serviceReq.ICD_CAUSE_NAME);
                        }
                    }
                    if (currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)
                    {
                        mmNOTE.Text = currentServiceReq.NOTE;
                        layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                    else
                    {
                        layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        this.Size = new Size(this.Size.Width, this.Size.Height - layoutControlItem16.Size.Height - 20);
                    }
                    chkPriority.Checked = (serviceReq.PRIORITY == 1 ? true : false);
                    chkIsEmergency.Checked = (serviceReq.IS_EMERGENCY == 1 ? true : false);
                    chkIsNotRequireFee.Checked = (serviceReq.IS_NOT_REQUIRE_FEE == 1 ? true : false);
                    chkIsNotUseBHYT.Checked = (serviceReq.IS_NOT_USE_BHYT == 1 ? true : false);
                    //mmNOTE.Enabled = (serviceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN ? true : false);
                    if (serviceReq.USE_TIME != null)
                    {
                        dtUseTime.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.USE_TIME ?? 0);
                    }
                    if (serviceReq.APPOINTMENT_TIME != null)
                    {
                        dtAppointmentTime.DateTime = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReq.APPOINTMENT_TIME ?? 0);
                    }
                    if (serviceReq.APPOINTMENT_DESC != null)
                    {
                        txtAppointmentDes.Text = serviceReq.APPOINTMENT_DESC;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDobPatientToForm(V_HIS_SERVICE_REQ serviceReqDTO)
        {
            try
            {
                if (serviceReqDTO != null)
                {
                    string nthnm = serviceReqDTO.INTRUCTION_TIME.ToString();

                    DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(serviceReqDTO.INTRUCTION_TIME) ?? DateTime.MinValue;
                    dtTime.DateTime = dtNgSinh;
                    int age = Inventec.Common.TypeConvert.Parse.ToInt32(nthnm.Substring(8, 2));
                    bool isGKS = true;
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    DateTime newDate = new DateTime(tongsogiay);

                    int nam = newDate.Year - 1;
                    int thang = newDate.Month - 1;
                    int ngay = newDate.Day - 1;
                    int gio = newDate.Hour;
                    int phut = newDate.Minute;
                    int giay = newDate.Second;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateData()
        {
            try
            {
                var icdValue = icdProcessor.GetValue(ucIcd);
                if (icdValue is IcdInputADO)
                {
                    currentServiceReq.ICD_CODE = ((IcdInputADO)icdValue).ICD_CODE;
                    //currentServiceReq.ICD_ID = ((IcdInputADO)icdValue).ICD_ID;
                    currentServiceReq.ICD_NAME = ((IcdInputADO)icdValue).ICD_NAME;
                }
                if (ucSecondaryIcd != null)
                {
                    var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        currentServiceReq.ICD_SUB_CODE = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        currentServiceReq.ICD_TEXT = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }

                if (this.ucIcdCause != null)
                {
                    var icdCauseValue = this.IcdCauseProcessor.GetValue(this.ucIcdCause);
                    if (icdCauseValue != null && icdCauseValue is UC.Icd.ADO.IcdInputADO)
                    {
                        currentServiceReq.ICD_CAUSE_CODE = ((UC.Icd.ADO.IcdInputADO)icdCauseValue).ICD_CODE;
                        currentServiceReq.ICD_CAUSE_NAME = ((UC.Icd.ADO.IcdInputADO)icdCauseValue).ICD_NAME;
                    }
                }

                currentServiceReq.INTRUCTION_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime) ?? 0;
                if (dtStartTime.EditValue != null && dtStartTime.DateTime != DateTime.MinValue)
                    currentServiceReq.START_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtStartTime.EditValue).ToString("yyyyMMddHHmm") + "00");
                else
                    currentServiceReq.START_TIME = null;

                if (dtEndTime.EditValue != null && dtEndTime.DateTime != DateTime.MinValue)
                    currentServiceReq.FINISH_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtEndTime.EditValue).ToString("yyyyMMddHHmm") + "00");
                else
                    currentServiceReq.FINISH_TIME = null;

                if (cboEndServiceReq.EditValue != null)
                    currentServiceReq.EXECUTE_LOGINNAME = cboEndServiceReq.EditValue.ToString();
                else
                    currentServiceReq.EXECUTE_LOGINNAME = null;

                if (cboRequestUser.EditValue != null)
                    currentServiceReq.REQUEST_LOGINNAME = cboRequestUser.EditValue.ToString();
                else
                    currentServiceReq.REQUEST_LOGINNAME = null;
                if (cboRequestUser.Properties.DataSource != null && cboRequestUser.Properties.DataSource is List<ACS.EFMODEL.DataModels.ACS_USER>)
                {
                    currentServiceReq.REQUEST_USERNAME = ((cboRequestUser.Properties.DataSource as List<ACS.EFMODEL.DataModels.ACS_USER>).FirstOrDefault(o => o.LOGINNAME == currentServiceReq.REQUEST_LOGINNAME) ?? new ACS.EFMODEL.DataModels.ACS_USER()).USERNAME;
                }

                if (cboConsultant.EditValue != null)
                    currentServiceReq.CONSULTANT_LOGINNAME = cboConsultant.EditValue.ToString();
                else
                    currentServiceReq.CONSULTANT_LOGINNAME = null;
                if (cboConsultant.Properties.DataSource != null && cboConsultant.Properties.DataSource is List<ACS.EFMODEL.DataModels.ACS_USER>)
                {
                    currentServiceReq.CONSULTANT_USERNAME = ((cboConsultant.Properties.DataSource as List<ACS.EFMODEL.DataModels.ACS_USER>).FirstOrDefault(o => o.LOGINNAME == currentServiceReq.CONSULTANT_LOGINNAME) ?? new ACS.EFMODEL.DataModels.ACS_USER()).USERNAME;
                }

                if (cboEndServiceReq.Properties.DataSource != null && cboEndServiceReq.Properties.DataSource is List<ACS.EFMODEL.DataModels.ACS_USER>)
                {
                    currentServiceReq.EXECUTE_USERNAME = ((cboEndServiceReq.Properties.DataSource as List<ACS.EFMODEL.DataModels.ACS_USER>).FirstOrDefault(o => o.LOGINNAME == currentServiceReq.EXECUTE_LOGINNAME) ?? new ACS.EFMODEL.DataModels.ACS_USER()).USERNAME;
                }

                if (currentServiceReq.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN && cboResultApprover.EditValue != null)
                {
                    currentServiceReq.RESULT_APPROVER_LOGINNAME = cboResultApprover.EditValue.ToString();
                    ACS_USER u = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == currentServiceReq.RESULT_APPROVER_LOGINNAME);
                    if (u != null)
                    {
                        currentServiceReq.RESULT_APPROVER_USERNAME = u.USERNAME;
                    }
                    else
                    {
                        currentServiceReq.RESULT_APPROVER_USERNAME = null;
                    }
                }
                if (cboHandler.EditValue != null)
                    currentServiceReq.ASSIGNED_EXECUTE_LOGINNAME = cboHandler.EditValue.ToString();
                else
                    currentServiceReq.ASSIGNED_EXECUTE_LOGINNAME = null;
                if (cboHandler.Properties.DataSource != null && cboHandler.Properties.DataSource is List<ACS.EFMODEL.DataModels.ACS_USER>)
                {
                    currentServiceReq.ASSIGNED_EXECUTE_USERNAME = ((cboHandler.Properties.DataSource as List<ACS.EFMODEL.DataModels.ACS_USER>).FirstOrDefault(o => o.LOGINNAME == currentServiceReq.ASSIGNED_EXECUTE_LOGINNAME) ?? new ACS.EFMODEL.DataModels.ACS_USER()).USERNAME;
                }
                else
                {
                    currentServiceReq.RESULT_APPROVER_LOGINNAME = null;
                    currentServiceReq.RESULT_APPROVER_USERNAME = null;
                }
                if (cboRequestUser.Properties.DataSource != null && cboRequestUser.Properties.DataSource is List<ACS.EFMODEL.DataModels.ACS_USER>)
                {
                    currentServiceReq.REQUEST_USERNAME = ((cboRequestUser.Properties.DataSource as List<ACS.EFMODEL.DataModels.ACS_USER>).FirstOrDefault(o => o.LOGINNAME == currentServiceReq.REQUEST_LOGINNAME) ?? new ACS.EFMODEL.DataModels.ACS_USER()).USERNAME;
                }
                if (mmNOTE.EditValue != null)
                {
                    currentServiceReq.NOTE = mmNOTE.EditValue.ToString();
                }
                currentServiceReq.NOTE = mmNOTE.Text;
                currentServiceReq.PRIORITY = (long)(chkPriority.Checked ? 1 : 0);
                currentServiceReq.IS_EMERGENCY = (short)(chkIsEmergency.Checked ? 1 : 0);
                currentServiceReq.IS_NOT_REQUIRE_FEE = (short)(chkIsNotRequireFee.Checked ? 1 : 0);
                currentServiceReq.IS_NOT_USE_BHYT = (short)(chkIsNotUseBHYT.Checked ? 1 : 0);
                currentServiceReq.USE_TIME = dtUseTime.EditValue != null && dtUseTime.DateTime != DateTime.MinValue ? Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtUseTime.DateTime) : null;

                if (lciAppointmentTime.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && lciAppointmentDes.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    currentServiceReq.APPOINTMENT_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtAppointmentTime.DateTime);
                    currentServiceReq.APPOINTMENT_DESC = txtAppointmentDes.Text;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkTime())
                {
                    XtraMessageBox.Show("Thời gian bắt đầu, thời gian kết thúc không được nhỏ hơn thời gian y lệnh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (chkIsNotUseBHYT.Checked)
                {
                    if ((XtraMessageBox.Show("Bạn có chắc không cho bệnh nhân hưởng bhyt các chi phí phát sinh tại phòng khám ", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No))
                        return;
                }
                if (!CheckUseTime())
                {
                    XtraMessageBox.Show("Thời gian dự trù không được nhỏ hơn thời gian y lệnh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtUseTime.Focus();
                    return;
                }

                this.positionHandleControl = -1;
                bool vali = true;
                CommonParam param = new CommonParam();
                bool succes = false;
                vali = (IsValiICDCause() || (panelControlUcIcd.Enabled == false)) && ((bool)subIcdProcessor.GetValidate(ucSecondaryIcd) || (panelControlUcIcd.Enabled == false)) && ((bool)icdProcessor.ValidationIcd(ucIcd) || (panelControlUcIcd.Enabled == false));
                if (!vali || !dxValidationProvider1.Validate())
                    return;

                //if (!string.IsNullOrEmpty(currentServiceReq.TDL_SERVICE_IDS))
                //{
                //    var spltService = services.Where(o => currentServiceReq.TDL_SERVICE_IDS.Split(';').ToList().Exists(p => p.Trim() == o.ID.ToString())).ToList();

                //    //currentServiceReq.TDL_SERVICE_IDS.Split(',').ToList().Where(o => services.Exists(p=>p.ID.ToString() == o.Trim())).ToList();
                //    if (spltService != null && spltService.Count > 0)
                //    {
                //        var checkService = spltService.Where(o => o.MAX_TOTAL_PROCESS_TIME != null && o.MAX_TOTAL_PROCESS_TIME > 0).ToList();
                //        if (checkService != null && checkService.Count() > 0)
                //        {
                //            var timeMin = checkService.Min(o => o.MAX_TOTAL_PROCESS_TIME);
                //            var currentServiceMin = checkService.OrderBy(o => o.MAX_TOTAL_PROCESS_TIME).First();
                //            TimeSpan diff = (TimeSpan)(dtEndTime.DateTime - Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Int64.Parse(currentServiceReq.INTRUCTION_TIME.ToString().Substring(0, currentServiceReq.INTRUCTION_TIME.ToString().Length - 2) + "00")));
                //            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => diff.TotalMinutes), diff.TotalMinutes));

                //            if ((int)diff.TotalMinutes > timeMin)
                //            {
                //                if (HisConfigs.Get<string>("HIS.Desktop.Plugins.ProcessTimeMustBeLessThanMaxTotalProcessTime") == "1")
                //                {
                //                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Không cho phép sửa thời gian kết thúc dịch vụ {0} sau {1} phút tính từ thời điểm ra y lệnh {2}", currentServiceMin.SERVICE_NAME, timeMin, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(currentServiceReq.INTRUCTION_TIME)),
                //               "Thông báo",
                //               MessageBoxButtons.OK);
                //                    return;
                //                }
                //                else if (HisConfigs.Get<string>("HIS.Desktop.Plugins.ProcessTimeMustBeLessThanMaxTotalProcessTime") == "2")
                //                {
                //                    if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Thời gian kết thúc dịch vụ  {0} vượt quá {1} phút tính từ thời điểm ra y lệnh {2}.Bạn có muốn tiếp tục không?", currentServiceMin.SERVICE_NAME, timeMin, Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(currentServiceReq.INTRUCTION_TIME)),
                //                "Thông báo",
                //               MessageBoxButtons.YesNo) == DialogResult.No)
                //                        return;
                //                }
                //            }
                //        }
                //    }
                //}
                vali = vali & CheckIntructionTimeWithInTime() & CheckMinDuration() & CheckHIS_DEPARTMENT_TRAN();
                if (vali)
                {
                    WaitingManager.Show();
                    UpdateData();

                    HIS.Desktop.Plugins.Library.CheckIcd.CheckIcdManager check = new Desktop.Plugins.Library.CheckIcd.CheckIcdManager(null, currentTreatment);
                    string message = null;
                    if (CheckIcdWhenSave == "1" || CheckIcdWhenSave == "2")
                    {
                        if (!check.ProcessCheckIcd(currentServiceReq.ICD_CODE, currentServiceReq.ICD_SUB_CODE, ref message, true))
                        {
                            if (CheckIcdWhenSave == "1" && !String.IsNullOrEmpty(message))
                            {
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("{0}.Bạn có muốn tiếp tục không?", message),
                                "Thông báo",
                               MessageBoxButtons.YesNo) == DialogResult.No)
                                    return;
                            }
                            if (CheckIcdWhenSave == "2" && !String.IsNullOrEmpty(message))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK);
                                return;
                            }
                        }
                    }
                    var update = new HIS_SERVICE_REQ();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(update, this.currentServiceReq);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => update), update));
                    var serviceReqUpdate = new BackendAdapter(param)
                        .Post<HIS_SERVICE_REQ>("api/HisServiceReq/UpdateCommonInfo", ApiConsumers.MosConsumer, update, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqUpdate), serviceReqUpdate));

                    if (serviceReqUpdate != null)
                    {
                        succes = true;
                        if (refeshData != null)
                            refeshData();
                        this.Close();
                    }

                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, succes);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {

                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckUseTime()
        {
            bool rs = true;
            try
            {
                if (dtUseTime.EditValue != null && dtUseTime.DateTime != DateTime.MinValue && dtTime.EditValue != null && dtTime.DateTime != DateTime.MinValue)
                {
                    if (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtUseTime.DateTime) < Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime))
                    {
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        bool checkTime()
        {
            bool success = false;
            try
            {
                if (Config.HisConfigCFG.StartTimeMustBeGreaterThanInstructionTime == "1")
                {
                    if ((dtStartTime.EditValue != null && dtTime.EditValue != null && dtStartTime.DateTime < dtTime.DateTime) || (dtTime.EditValue != null && dtEndTime.EditValue != null && dtEndTime.DateTime < dtTime.DateTime))
                    {
                        return true;
                    }
                }

                if (Config.HisConfigCFG.StartTimeMustBeGreaterThanInstructionTime == "2")
                {
                    List<long> ReqTypeId = new List<long>()
                    {
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, 
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
                    };
                    if (!ReqTypeId.Contains(currentServiceReq.SERVICE_REQ_TYPE_ID)
                        && ((dtStartTime.EditValue != null && dtTime.EditValue != null && dtStartTime.DateTime < dtTime.DateTime)
                        || (dtTime.EditValue != null && dtEndTime.EditValue != null && dtEndTime.DateTime < dtTime.DateTime)))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }
        bool IsValiICDCause()
        {
            bool result = true;
            try
            {
                result = (bool)this.IcdCauseProcessor.ValidationIcd(this.ucIcdCause);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckIntructionTimeWithInTime()
        {
            bool result = true;
            try
            {
                if (currentTreatment != null && dtTime.EditValue != null)
                {
                    long timeInstruction = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime) ?? 0;
                    if (currentTreatment.IN_TIME > timeInstruction)
                    {
                        MessageManager.Show(String.Format(ResourceMessage.KhongChoNhapThoiGianNhoHonThoiGianVaoVien));
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void GetTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = this.currentServiceReq.TREATMENT_ID;
                currentTreatment = new BackendAdapter(param)
                    .Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckMinDuration()
        {
            bool result = true;
            try
            {
                CommonParam param = new CommonParam();

                HisSereServFilter sereServFilter = new HisSereServFilter();
                sereServFilter.SERVICE_REQ_ID = currentServiceReq.ID;
                var servServMinDuration = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param);

                if (servServMinDuration != null && servServMinDuration.Count > 0)
                {
                    var servicesDt = services.Where(o => servServMinDuration.Select(p => p.SERVICE_ID).Contains(o.ID) && o.MIN_DURATION.HasValue).ToList();

                    List<ServiceDuration> serviceDurations = new List<ServiceDuration>();

                    if (servicesDt != null && servicesDt.Count > 0)
                    {
                        foreach (var item in servicesDt)
                        {
                            ServiceDuration sd = new ServiceDuration();
                            sd.MinDuration = item.MIN_DURATION.Value;
                            sd.ServiceId = item.ID;
                            serviceDurations.Add(sd);
                        }
                    }

                    if (serviceDurations != null && serviceDurations.Count > 0)
                    {
                        HisSereServMinDurationFilter filter = new HisSereServMinDurationFilter();
                        filter.PatientId = currentServiceReq.TDL_PATIENT_ID;
                        filter.InstructionTime = currentServiceReq.INTRUCTION_TIME;
                        filter.ServiceDurations = serviceDurations;
                        filter.ServiceReqId = currentServiceReq.ID;
                        var sereServMinduration = new BackendAdapter(param)
                            .Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetExceedMinDuration", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (sereServMinduration != null && sereServMinduration.Count > 0)
                        {
                            List<string> listMessage = new List<string>();

                            foreach (var item in sereServMinduration)
                            {
                                string message = "";
                                message += item.TDL_SERVICE_CODE + "-" + item.TDL_SERVICE_NAME + ";";
                                listMessage.Add(message);
                            }

                            listMessage = listMessage.Distinct().ToList();

                            string serviceMessage = "";
                            foreach (var it in listMessage)
                            {
                                serviceMessage += it;
                            }

                            string messageStr = string.Format("Các dịch vụ sau có thời gian chỉ định nằm trong khoảng thời gian không cho phép: {0} Bạn có muốn tiếp tục?", serviceMessage);
                            if (MessageBox.Show(messageStr, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            {
                                result = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckHIS_DEPARTMENT_TRAN()
        {
            bool result = true;
            try
            {
                if (this.currentServiceReq != null)
                {
                    long checkINTRUCTION_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime) ?? 0;

                    CommonParam param = new CommonParam();
                    HisDepartmentTranFilter filter = new HisDepartmentTranFilter();
                    filter.DEPARTMENT_ID = this.currentServiceReq.REQUEST_DEPARTMENT_ID;
                    filter.TREATMENT_ID = this.currentServiceReq.TREATMENT_ID;
                    var data = new BackendAdapter(param)
                            .Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    if (data != null && data.Count == 1)
                    {
                        var thisDepartmentTran = data.First();

                        HisDepartmentTranFilter filterDt = new HisDepartmentTranFilter();
                        filterDt.PREVIOUS_ID = thisDepartmentTran.ID;
                        filterDt.TREATMENT_ID = this.currentServiceReq.TREATMENT_ID;
                        var checkIfHasNextDepartmentTran = new BackendAdapter(param)
                                .Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, filterDt, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                        if (checkIfHasNextDepartmentTran != null)
                        {
                            var nextDepartmentTran = checkIfHasNextDepartmentTran.FirstOrDefault();
                            if (nextDepartmentTran == null || nextDepartmentTran.DEPARTMENT_IN_TIME == null)
                            {
                                result = true;
                            }
                            else if (nextDepartmentTran.DEPARTMENT_IN_TIME <= checkINTRUCTION_TIME)
                            {
                                var thisDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == thisDepartmentTran.DEPARTMENT_ID).FirstOrDefault();
                                var nextDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == nextDepartmentTran.DEPARTMENT_ID).FirstOrDefault();
                                string messageStr = string.Format("Y lệnh {0} {1} có thời gian y lệnh lớn hơn thời gian vào {2}! Bạn có muốn tiếp tục?", this.currentServiceReq.SERVICE_REQ_CODE, thisDepartment.DEPARTMENT_NAME, nextDepartment.DEPARTMENT_NAME);
                                if (MessageBox.Show(messageStr, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                                {
                                    result = false;
                                }
                                else
                                {
                                    result = true;
                                }
                            }
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else if (data != null && data.Count > 1)
                    {
                        var checkIfHasThisDepartmentTran = data.OrderByDescending(o => o.DEPARTMENT_IN_TIME)
                                                    .Where(o => o.DEPARTMENT_IN_TIME <= this.currentServiceReq.INTRUCTION_TIME);
                        if (checkIfHasThisDepartmentTran == null || checkIfHasThisDepartmentTran.Count() == 0)
                        {
                            return true;
                        }
                        var thisDepartmentTran = checkIfHasThisDepartmentTran.First();

                        HisDepartmentTranFilter filterDt = new HisDepartmentTranFilter();
                        filterDt.PREVIOUS_ID = thisDepartmentTran.ID;
                        filterDt.TREATMENT_ID = this.currentServiceReq.TREATMENT_ID;
                        var checkIfHasNextDepartmentTran = new BackendAdapter(param)
                                .Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, filterDt, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (checkIfHasNextDepartmentTran != null)
                        {
                            var nextDepartmentTran = data.Where(o => o.PREVIOUS_ID == thisDepartmentTran.ID).FirstOrDefault();
                            if (nextDepartmentTran == null || nextDepartmentTran.DEPARTMENT_IN_TIME == null)
                            {
                                result = true;
                            }
                            else if (nextDepartmentTran.DEPARTMENT_IN_TIME <= checkINTRUCTION_TIME)
                            {
                                var thisDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == thisDepartmentTran.DEPARTMENT_ID).FirstOrDefault();
                                var nextDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == nextDepartmentTran.DEPARTMENT_ID).FirstOrDefault();
                                string messageStr = string.Format("Y lệnh {0} {1} có thời gian y lệnh lớn hơn thời gian vào {2}! Bạn có muốn tiếp tục?", this.currentServiceReq.SERVICE_REQ_CODE, thisDepartment.DEPARTMENT_NAME, nextDepartment.DEPARTMENT_NAME);
                                if (MessageBox.Show(messageStr, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                                {
                                    result = false;
                                }
                                else
                                {
                                    result = true;
                                }
                            }
                        }
                        else
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void dtTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtStartTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void cboSub_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtLoginname_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadPayFormCombo(txtLoginname.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndServiceReq_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboEndServiceReq.EditValue != null && cboEndServiceReq.EditValue != cboEndServiceReq.OldEditValue)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboEndServiceReq.EditValue.ToString());
                        if (commune != null)
                        {
                            txtLoginname.Text = commune.LOGINNAME;
                            txtRequestUser.Focus();
                            txtRequestUser.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndServiceReq_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboEndServiceReq.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboEndServiceReq.EditValue.ToString());
                        if (commune != null)
                        {
                            txtLoginname.Text = commune.LOGINNAME;
                            txtRequestUser.Focus();
                            txtRequestUser.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtStartTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtEndTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtEndTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtLoginname.Focus();
                    txtLoginname.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndServiceReq_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboEndServiceReq.EditValue = null;
                    txtLoginname.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtStartTime_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    dtStartTime.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboRequestUser.EditValue = null;
                    txtRequestUser.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRequestUser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadPayFormComboRequest(txtRequestUser.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRequestUser_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboRequestUser.EditValue != null && cboRequestUser.EditValue != cboRequestUser.OldEditValue)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboRequestUser.EditValue.ToString());
                        if (commune != null)
                        {
                            txtRequestUser.Text = commune.LOGINNAME;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRequestUser_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboRequestUser.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboRequestUser.EditValue.ToString());
                        if (commune != null)
                        {
                            txtRequestUser.Text = commune.LOGINNAME;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtResultApproverLoginname_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string searchCode = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(searchCode))
                    {
                        this.cboResultApprover.EditValue = null;
                        this.cboResultApprover.Focus();
                        this.cboResultApprover.ShowPopup();
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<ACS_USER>()
                            .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.ToUpper().Contains(searchCode.ToUpper())).ToList();

                        var searchResult = (data != null && data.Count > 0) ? (data.Count == 1 ? data : data.Where(o => o.LOGINNAME.ToUpper() == searchCode.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            this.cboResultApprover.EditValue = searchResult[0].LOGINNAME;
                            this.txtResultApproverLoginname.Text = searchResult[0].LOGINNAME;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }
                        else
                        {
                            this.cboResultApprover.EditValue = null;
                            this.cboResultApprover.Focus();
                            this.cboResultApprover.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResultApprover_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboResultApprover.EditValue != null)
                    {
                        ACS_USER data = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboResultApprover.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtResultApproverLoginname.Text = data.LOGINNAME;
                        }
                    }
                    this.icdProcessor.FocusControl(this.ucIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResultApprover_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboResultApprover.EditValue != null)
                    {
                        ACS_USER data = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME == ((this.cboResultApprover.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            this.txtResultApproverLoginname.Text = data.LOGINNAME;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }
                    }
                }
                else
                {
                    this.cboResultApprover.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResultApprover_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboResultApprover.EditValue = null;
                    txtResultApproverLoginname.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboConsultant_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboConsultant.EditValue != null && cboConsultant.EditValue != cboConsultant.OldEditValue)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboConsultant.EditValue.ToString());
                        if (commune != null)
                        {
                            txtConsultant.Text = commune.LOGINNAME;
                            cboConsultant.Properties.Buttons[1].Visible = true;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboConsultant_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboConsultant.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboConsultant.EditValue.ToString());
                        if (commune != null)
                        {
                            txtConsultant.Text = commune.LOGINNAME;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }
                    }
                }
                else
                {
                    cboConsultant.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtConsultant_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadFormComboConsultant(txtConsultant.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadFormComboConsultant(string _code)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> listResult = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                listResult = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && (o.LOGINNAME != null && o.LOGINNAME.Equals(_code))).ToList();

                // Get list
                var listLoginNameEmployee = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).Select(p => p.LOGINNAME).ToList();

                if (listLoginNameEmployee != null && listLoginNameEmployee.Count > 0)
                {
                    listResult = listResult.Where(o => listLoginNameEmployee.Contains(o.LOGINNAME)).ToList();
                }

                if (listResult.Count == 1)
                {
                    cboConsultant.EditValue = listResult[0].LOGINNAME;
                    txtConsultant.Text = listResult[0].LOGINNAME;
                    this.icdProcessor.FocusControl(this.ucIcd);
                }
                else if (listResult.Count > 1)
                {
                    cboConsultant.EditValue = null;
                    cboConsultant.Focus();
                    cboConsultant.ShowPopup();
                }
                else
                {
                    cboConsultant.EditValue = null;
                    cboConsultant.Focus();
                    cboConsultant.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboConsultant_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboConsultant.EditValue = null;
                    txtConsultant.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHandler_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHandler.EditValue != null && cboHandler.EditValue != cboHandler.OldEditValue)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboHandler.EditValue.ToString());
                        if (commune != null)
                        {
                            txtHandler.Text = commune.LOGINNAME;
                            cboHandler.Properties.Buttons[1].Visible = true;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHandler_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboHandler.EditValue != null)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER commune = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboHandler.EditValue.ToString());
                        if (commune != null)
                        {
                            txtHandler.Text = commune.LOGINNAME;
                            this.icdProcessor.FocusControl(this.ucIcd);
                        }
                    }
                }
                else
                {
                    cboHandler.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHandler_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadFormComboHandler(txtHandler.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadFormComboHandler(string _code)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> listResult = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                listResult = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => (o.LOGINNAME != null && o.LOGINNAME.Equals(_code))).ToList();

                if (listResult.Count == 1)
                {
                    cboHandler.EditValue = listResult[0].LOGINNAME;
                    txtHandler.Text = listResult[0].LOGINNAME;
                    this.icdProcessor.FocusControl(this.ucIcd);
                }
                else if (listResult.Count > 1)
                {
                    cboHandler.EditValue = null;
                    cboHandler.Focus();
                    cboHandler.ShowPopup();
                }
                else
                {
                    cboHandler.EditValue = null;
                    cboHandler.Focus();
                    cboHandler.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHandler_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboHandler.EditValue = null;
                    txtHandler.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
