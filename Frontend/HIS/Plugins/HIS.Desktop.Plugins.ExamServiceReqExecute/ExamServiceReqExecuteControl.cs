﻿using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTab;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ModuleExt;
using HIS.Desktop.Plugins.ExamServiceReqExecute.ADO;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Base;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.Desktop.Plugins.ExamServiceReqExecute.ConnectCOM;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Popup;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Resources;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Sda.SdaEventLogCreate;
using HIS.Desktop.Plugins.Library.CheckIcd;
using HIS.Desktop.Plugins.Library.FormMedicalRecord;
using HIS.Desktop.Plugins.Library.OtherTreatmentHistory;
using HIS.Desktop.Plugins.Library.OtherTreatmentHistory.Base;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using HIS.UC.ExamFinish;
using HIS.UC.ExamFinish.ADO;
using HIS.UC.ExamServiceAdd;
using HIS.UC.ExamServiceAdd.ADO;
using HIS.UC.ExamTreatmentFinish;
using HIS.UC.HisExamServiceAdd.ADO;
using HIS.UC.Hospitalize;
using HIS.UC.Hospitalize.ADO;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.CardReader;
using Inventec.Common.Logging;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MPS.ADO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {
        #region Declare
        internal PatyAlterBhytADO PatyAlterBhyt { get; set; }
        internal List<V_HIS_EXP_MEST_MEDICINE> currentExpMestMedicines = null;
        internal V_HIS_SERVICE_REQ HisServiceReqView { get; set; }
        HIS_SERVICE_REQ serviceReq;
        List<L_HIS_TREATMENT_2> treatmentByPatients;
        int positionHandleControlLeft = -1;
        internal MPS.ADO.PatientADO currentPatient { get; set; }
        internal long treatmentId = 0;
        CommonParam param = new CommonParam();
        internal Inventec.Desktop.Common.Modules.Module moduleData;
        internal HIS_TREATMENT treatment { get; set; }
        internal V_HIS_TREATMENT ViewTreatment { get; set; }
        internal V_HIS_TREATMENT treatmentView { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5 sereServ { get; set; }
        internal L_HIS_TREATMENT_2 ltreatment2 { get; set; }
        internal List<HIS_SERE_SERV> SereServsCurrentTreatment { get; set; }
        internal List<HIS_SERE_SERV> ClsSereServ { get; set; }
        internal List<HIS_SERE_SERV> SereServ8s { get; set; }
        internal List<TreatmentExamADO> TreatmentHistorys { get; set; }
        List<HIS_NEXT_TREA_INTR> dataNextTreatmentInstructions;
        List<HIS_CONTRAINDICATION> datas = new List<HIS_CONTRAINDICATION>();
        string _TextNextTreatmentInstructionName = "";
        string _TextIcdName = "";
        string _TextIcdNameCause = "";
        bool IsObligatoryTranferMediOrg = false;
        bool IsAcceptWordNotInData = false;
        bool AutoCheckNextTreatmentInstruction = false;
        string[] icdSeparators = new string[] { ";" };
        long autoCheckIcd;
        bool isAutoCheckIcd;
        bool isAllowNoIcd = false;
        long requiredControl;
        long checkSameHeinCFG;
        bool valiDHST;
        bool isChronic;
        private bool isNotCheckValidateIcdUC;
        private bool isClickSaveFinish;
        bool isPrintAppoinment = false;
        bool isPrintBordereau = false;
        bool isSignAppoinment = false;
        bool isSignBordereau = false;
        bool isPrintBANT = false;
        bool isPrintHospitalizeExam = false;
        bool isSign = false;
        bool isPrintSign = false;
        bool isPrintSurgAppoint = false;
        bool isPrintExamServiceAdd = false;
        bool isSignExamServiceAdd = false;
        bool IsPrintBHXH = false;
        bool IsSignBHXH = false;
        bool IsPrintExam = false;
        bool IsSignExam = false;
        bool isInPhieuPhuLuc = false;
        bool isKyPhieuPhuLuc = false;
        bool isPrintPrescription = false;
        bool isPayment = false;

        bool IsAppointment_ExamServiceAdd = false;
        bool IsPrintAppointment_ExamServiceAdd = false;
        bool IsAppointment_ExamFinish = false;
        bool IsPrintAppointment_ExamFinish = false;
        bool IsReturn = false;
        bool isPrintHosTransfer = false;
        const int PRESCRIPTION_TYPE_ID__YHCT = 2;
        const int PRESCRIPTION_TYPE_ID__THUONG = 1;
        List<HIS_ICD> currentIcds;
        HIS_ICD icdPopupSelect;

        List<HIS.Desktop.Plugins.ExamServiceReqExecute.ADO.IcdADO> icdSubcodeAdoChecks;
        HIS.Desktop.Plugins.ExamServiceReqExecute.ADO.IcdADO subIcdPopupSelect;
        bool isNotProcessWhileChangedTextSubIcd;

        bool isShowContainerMediMaty = false;
        bool isShowContainerMediMatyForChoose = false;
        bool isShow = true;
        DelegateSelectData reLoadServiceReq { get; set; }
        HIS.Desktop.Common.RefeshReference refresh = null;
        internal HisServiceReqExamAdditionSDO ServiceReqExamAddition { get; set; }
        internal HisServiceReqExamUpdateResultSDO HisServiceReqResult { get; set; }
        internal UserControl ucExamAddition;
        internal UserControl ucHospitalize;
        internal UserControl ucTreatmentFinish;
        internal UserControl ucExamFinish;
        internal ExamTreatmentFinishProcessor treatmentFinishProcessor;
        internal ExamServiceAddProcessor examServiceAddProcessor;
        internal HospitalizeProcessor hospitalizeProcessor;
        internal ExamFinishProcessor examFinishProcessor;
        List<HIS_PATIENT_PROGRAM> PatientProgramList = null;
        List<V_HIS_DATA_STORE> DataStoreList = null;
        HIS_MEDI_RECORD MediRecode = null;
        bool isFinishLoad = false;
        long IsRequiredWeightOption;

        HIS_ICD icdDefaultFinish = new HIS_ICD();

        List<HIS_EMR_COVER_CONFIG> LstEmrCoverConfig;
        List<HIS_EMR_COVER_CONFIG> LstEmrCoverConfigDepartment;
        MediRecordMenuPopupProcessor emrMenuPopupProcessor = null;
        BarManager _BarManager = null;
        internal PopupMenu _Menu = null;

        List<HIS_CONTRAINDICATION> contraindicationSelecteds;
        List<long> oldContraindicationSelecteds = null;
        List<string> dataSelectedToPTDT;
        int key = 0;
        bool isShowSubIcd = false;
        HIS_ICD IcdSubChoose;
        bool isShowGridIcdSub;
        bool isShowSubTemp;
        List<string> icdCodeList = new List<string>();
        bool isReturnCheckboxExamFinish = false;
        bool isReturnCheckboxHosTreat = false;
        private List<string> lstModuleLinkApply;
        internal EpaymentBillResultSDO resultEPayment;

        List<HIS_HEALTH_EXAM_RANK> lstHealthExamRank { get; set; }
        HIS_PATIENT CurrentPatient { get; set; }
        HIS_SEVERE_ILLNESS_INFO SevereIllnessInfo { get; set; }

        List<HIS_EVENTS_CAUSES_DEATH> EventsCausesDeaths { get; set; }
        HisServiceReqExamUpdateSDO sdoSendFrmExam { get; set; }
        Timer timeClose { get; set; }
        public bool? PrintMps { get; set; }
        private CheckIcdManager checkIcdManager { get; set; }
        DelegateRefeshIcdChandoanphu dlgSendIcd { get; set; }
        bool IsValidForSave = true;
        #endregion

        #region Construct - Load
        public ExamServiceReqExecuteControl()
            : base()
        {
            try
            {
                InitializeComponent();
                //this.InitLanguage();
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public ExamServiceReqExecuteControl(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_SERVICE_REQ serviceReq, bool _isChronic, DelegateSelectData reLoadServiceReq, List<HIS_SERE_SERV> sereServCurrentTreatment)
            : base(moduleData)
        {
            Inventec.Common.Logging.LogSystem.Info("ExamServiceReqExecuteControl. Init .1");
            InitializeComponent();
            try
            {
                this.SereServsCurrentTreatment = sereServCurrentTreatment;
                this.HisServiceReqView = serviceReq;
                this.moduleData = moduleData;
                this.isChronic = _isChronic;
                this.treatmentId = HisServiceReqView != null ? HisServiceReqView.TREATMENT_ID : 0;
                this.reLoadServiceReq = reLoadServiceReq;
                SetCaptionByLanguageKey();
                //this.InitLanguage();
                Inventec.Common.Logging.LogSystem.Info("ExamServiceReqExecuteControl. Init .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ExamServiceReqExecuteControl_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.moduleData != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecuteControl_Load .1 this.moduleData RoomId" + this.moduleData.RoomId);

                }
                timeClose = new Timer();
                timeClose.Interval = 100;
                timeClose.Tick += new System.EventHandler(this.timerClose_Tick);
                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecuteControl_Load .1");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisServiceReqView), HisServiceReqView));
                HisConfigCFG.LoadConfig();

                this.LoadSdaConfig();
                this.autoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.AutoCheckIcd");
                this.isAutoCheckIcd = (this.autoCheckIcd == 1);
                this.currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();
                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecuteControl_Load .2");
                LoadTreatmentByPatient();
                LoadCurrentPatient();
                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecuteControl_Load .3");

                UCIcdInit();
                UCIcdCauseInit();

                //KeyAllowToEnableIcdSubCode();

                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecuteControl_Load .4");
                this.LoadTreatmentInfomation();

                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecuteControl_Load .4.1");
                if (this.moduleData != null)
                    this.isAllowNoIcd = (BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId).IS_ALLOW_NO_ICD == 1);

                this.ValidControl();
                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecuteControl_Load .4.2");
                this.DHSTLoadDataDefault();
                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecuteControl_Load .5");

                LoadTreatmentHistory();
                if (this.TreatmentHistorys != null && this.TreatmentHistorys.Count > 0)
                {
                    this.xtraTabControlInfo.SelectedTabPage = this.xtraTabPageExamHistory;
                }
                else
                {
                    this.xtraTabControlInfo.SelectedTabPage = this.xtraTabPageExamExecute;
                }
                this.SetTabPageVisible(tabControlDetailData);

                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecuteControl_Load .6");
                this.FillDataToComboNextTreatmentInst();
                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecuteControl_Load .7");
                this.LoadDHSTFromServiceReq();
                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecuteControl_Load .8");
                this.InitComboKsk();
                this.InitComboPatientCase();
                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecuteControl_Load .9");

                this.LoadEmrCoverConfig();

                btnVoBenhAn.Enabled = false;
                //btnPrint_ExamService.Enabled = false;

                InitCombo();
                InitContraindicationCheck();

                FillDatatoComboContraindication();
                this.ProcessCustomizeUI();
                RegisterTimer(moduleData.ModuleLink, "timerInitForm", 5000, InitForm);
                StartTimer(moduleData.ModuleLink, "timerInitForm");
                EnableRadioExamFinish();
                ValidHospitalizationReason();
                RegisterTimer(moduleData.ModuleLink, "timerSetText", 500, SetText);
                StartTimer(moduleData.ModuleLink, "timerSetText");
                ModuleList();
                EnableViaKeyDisablePartExamByExecutor();
                checkIcdManager = new CheckIcdManager(DlgIcdSubCode, treatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DlgIcdSubCode(string icdCodes, string icdNames)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("stringIcds.1");
                this.isNotProcessWhileChangedTextSubIcd = true;
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        ProcessIcdSub(icdCodes, icdNames);
                    }));
                }
                else
                {
                    ProcessIcdSub(icdCodes, icdNames);
                }
                this.isNotProcessWhileChangedTextSubIcd = false;
                Inventec.Common.Logging.LogSystem.Debug("stringIcds.6");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessIcdSub(string icdCodes, string icdNames)
        {
            try
            {
                var lstIcdCode = icdCodes.Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var lstIcdName = icdNames.Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                var lstIcdCodeScreen = txtIcdSubCode.Text.Trim().Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                lstIcdCodeScreen.AddRange(lstIcdCode);
                lstIcdCodeScreen = lstIcdCodeScreen.Distinct().ToList();
                string icdCode = string.Join(";", lstIcdCodeScreen);

                var lstIcdNameScreen = txtIcdText.Text.Trim().Split(IcdUtil.seperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                lstIcdNameScreen.AddRange(lstIcdName);
                lstIcdNameScreen = lstIcdNameScreen.Distinct().ToList();
                string icdName = string.Join(";", lstIcdNameScreen);
                if (!string.IsNullOrEmpty(icdCode))
                {
                    txtIcdSubCode.Text = icdCode;
                }
                else
                {
                    txtIcdSubCode.Text = "";
                }
                if (!string.IsNullOrEmpty(icdName))
                {
                    txtIcdText.Text = icdName;
                }
                else
                {
                    txtIcdText.Text = "";
                }
                ReloadIcdSubContainerByCodeChanged();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void timerClose_Tick(object sender, EventArgs e)
        {
            try
            {
                if (PrintMps == true|| PrintMps == null)
                {
                    timeClose.Stop();
                    PrintMps = null;
                    XtraTabControl main = SessionManager.GetTabControlMain();
                    XtraTabPage page = main.TabPages[GlobalVariables.SelectedTabPageIndex];
                    TabControlBaseProcess.CloseCurrentTabPage(page, main);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableViaKeyDisablePartExamByExecutor()
        {
            try
            {
                var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (HisConfigCFG.isDisablePartExamByExecutor)
                {
                    //if (!(string.IsNullOrEmpty(HisServiceReqView.PAEX_LOGINNAME) || HisServiceReqView.PAEX_LOGINNAME == loginName))
                    //{
                    //    this.txtTuanHoan.Enabled = false;
                    //    this.txtHoHap.Enabled = false;
                    //    this.txtTieuHoa.Enabled = false;
                    //    this.txtThanTietNieu.Enabled = false;
                    //    this.txtThanKinh.Enabled = false;
                    //    this.txtCoXuongKhop.Enabled = false;
                    //    //TMH
                    //    txtTai.Enabled = false;
                    //    txtMui.Enabled = false;
                    //    txtHong.Enabled = false;
                    //    txtPART_EXAM_EAR_RIGHT_NORMAL.Enabled = false;
                    //    txtPART_EXAM_EAR_RIGHT_WHISPER.Enabled = false;
                    //    txtPART_EXAM_EAR_LEFT_NORMAL.Enabled = false;
                    //    txtPART_EXAM_EAR_LEFT_WHISPER.Enabled = false;
                    //    //RHM
                    //    txtPART_EXAM_UPPER_JAW.Enabled = false;
                    //    txtPART_EXAM_LOWER_JAW.Enabled = false;
                    //    txtRHM.Enabled = false;
                    //    //Mat
                    //    txtMat.Enabled = false;
                    //    chkPART_EXAM_EYE_BLIND_COLOR__BT.Enabled = false;
                    //    chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Enabled = false;
                    //    chkPART_EXAM_EYE_BLIND_COLOR__MMD.Enabled = false;
                    //    chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Enabled = false;
                    //    chkPART_EXAM_EYE_BLIND_COLOR__MMV.Enabled = false;
                    //    txtNhanApPhai.Enabled = false;
                    //    txtThiLucKhongKinhPhai.Enabled = false;
                    //    txtThiLucCoKinhPhai.Enabled = false;
                    //    txtNhanApTrai.Enabled = false;
                    //    txtThiLucKhongKinhTrai.Enabled = false;
                    //    txtThiLucCoKinhTrai.Enabled = false;
                    //    chkPART_EXAM_HORIZONTAL_SIGHT__BT.Enabled = false;
                    //    chkPART_EXAM_HORIZONTAL_SIGHT__HC.Enabled = false;
                    //    chkPART_EXAM_VERTICAL_SIGHT__BT.Enabled = false;
                    //    chkPART_EXAM_VERTICAL_SIGHT__HC.Enabled = false;
                    //    this.txtNoiTiet.Enabled = false;
                    //    this.txtPartExamMental.Enabled = false;
                    //    this.txtPartExamNutrition.Enabled = false;
                    //    this.txtPartExamMotion.Enabled = false;
                    //    this.txtPartExanObstetric.Enabled = false;
                    //    this.txtDaLieu.Enabled = false;
                    //}
                    //else
                    //{
                    this.txtKhamBoPhan.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_LOGINNAME) || HisServiceReqView.PAEX_LOGINNAME == loginName;
                    this.txtTuanHoan.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_CIRC_LOGINNAME) || HisServiceReqView.PAEX_CIRC_LOGINNAME == loginName;
                    this.txtHoHap.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_RESP_LOGINNAME) || HisServiceReqView.PAEX_RESP_LOGINNAME == loginName;
                    this.txtTieuHoa.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_DIGE_LOGINNAME) || HisServiceReqView.PAEX_DIGE_LOGINNAME == loginName;
                    this.txtThanTietNieu.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_KIDN_LOGINNAME) || HisServiceReqView.PAEX_KIDN_LOGINNAME == loginName;
                    this.txtThanKinh.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_NEUR_LOGINNAME) || HisServiceReqView.PAEX_NEUR_LOGINNAME == loginName;
                    this.txtCoXuongKhop.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_MUSC_LOGINNAME) || HisServiceReqView.PAEX_MUSC_LOGINNAME == loginName;
                    txtTai.Enabled = txtMui.Enabled = txtHong.Enabled = txtPART_EXAM_EAR_RIGHT_NORMAL.Enabled = txtPART_EXAM_EAR_RIGHT_WHISPER.Enabled = txtPART_EXAM_EAR_LEFT_NORMAL.Enabled = txtPART_EXAM_EAR_LEFT_WHISPER.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_ENT_LOGINNAME) || HisServiceReqView.PAEX_ENT_LOGINNAME == loginName;
                    txtPART_EXAM_UPPER_JAW.Enabled = txtPART_EXAM_LOWER_JAW.Enabled = txtRHM.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_STOM_LOGINNAME) || HisServiceReqView.PAEX_STOM_LOGINNAME == loginName;
                    txtMat.Enabled = chkPART_EXAM_EYE_BLIND_COLOR__BT.Enabled = chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Enabled = chkPART_EXAM_EYE_BLIND_COLOR__MMD.Enabled = chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Enabled = chkPART_EXAM_EYE_BLIND_COLOR__MMV.Enabled = txtNhanApPhai.Enabled = txtThiLucKhongKinhPhai.Enabled = txtThiLucCoKinhPhai.Enabled = txtNhanApTrai.Enabled = txtThiLucKhongKinhTrai.Enabled = txtThiLucCoKinhTrai.Enabled = chkPART_EXAM_HORIZONTAL_SIGHT__BT.Enabled = chkPART_EXAM_HORIZONTAL_SIGHT__HC.Enabled = chkPART_EXAM_VERTICAL_SIGHT__BT.Enabled = chkPART_EXAM_VERTICAL_SIGHT__HC.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_EYE_LOGINNAME) || HisServiceReqView.PAEX_EYE_LOGINNAME == loginName;
                    this.txtNoiTiet.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_OEND_LOGINNAME) || HisServiceReqView.PAEX_OEND_LOGINNAME == loginName;
                    this.txtPartExamMental.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_MENT_LOGINNAME) || HisServiceReqView.PAEX_MENT_LOGINNAME == loginName;
                    this.txtPartExamNutrition.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_NUTR_LOGINNAME) || HisServiceReqView.PAEX_NUTR_LOGINNAME == loginName;
                    this.txtPartExamMotion.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_MOTI_LOGINNAME) || HisServiceReqView.PAEX_MOTI_LOGINNAME == loginName;
                    this.txtPartExanObstetric.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_OBST_LOGINNAME) || HisServiceReqView.PAEX_OBST_LOGINNAME == loginName;
                    this.txtDaLieu.Enabled = string.IsNullOrEmpty(HisServiceReqView.PAEX_DERM_LOGINNAME) || HisServiceReqView.PAEX_DERM_LOGINNAME == loginName;
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetText()
        {
            try
            {
                this.chkExamFinish.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkExamFinish.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                txtHospitalizationReason.Focus();
                StopTimer(moduleData.ModuleLink, "timerSetText");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrentPatient()
        {
            try
            {
                HisPatientFilter patientFilter = new HisPatientFilter();
                patientFilter.ID = treatment.PATIENT_ID;
                CurrentPatient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidHospitalizationReason()
        {
            try
            {
                long hospitalizationReasonRequired = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.HOSPITALIZATION_REASON__REQUIRED));
                if (hospitalizationReasonRequired == 1)
                {
                    lblCaptionHospitalizationReason.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void EnableRadioExamFinish()
        {
            try
            {
                if (HisConfigCFG.enableExamtype == "1")
                {
                    if (this.HisServiceReqView.IS_MAIN_EXAM == 1)
                    {
                        chkExamFinish.Checked = false;
                        chkExamFinish.Properties.Appearance.ForeColor = Color.Gray;
                        chkExamFinish.ToolTip = "Khám chính chỉ cho phép xử trí khám thêm, nhập viện hoặc kết thúc điều trị";
                        isReturnCheckboxExamFinish = true;
                    }
                    else if (this.HisServiceReqView.IS_MAIN_EXAM != 1)
                    {
                        chkTreatmentFinish.Checked = false;
                        chkHospitalize.Checked = false;
                        chkTreatmentFinish.Properties.Appearance.ForeColor = Color.Gray;
                        chkHospitalize.Properties.Appearance.ForeColor = Color.Gray;
                        chkHospitalize.ToolTip = "Khám phụ không cho phép xử trí nhập viện hoặc kết thúc điều trị";
                        chkTreatmentFinish.ToolTip = "Khám phụ không cho phép xử trí nhập viện hoặc kết thúc điều trị";
                        isReturnCheckboxHosTreat = true;
                    }
                }
                else if (HisConfigCFG.enableExamtype == "2")
                {
                    if (this.HisServiceReqView.IS_MAIN_EXAM == 1 && this.HisServiceReqView.TREATMENT_TYPE_ID == 3)
                    {
                        chkExamFinish.Enabled = true;
                        chkTreatmentFinish.Enabled = false;
                    }
                    else if (this.HisServiceReqView.IS_MAIN_EXAM == 1 && this.HisServiceReqView.TREATMENT_TYPE_ID != 3)
                    {
                        chkExamFinish.Checked = false;
                        chkExamFinish.Properties.Appearance.ForeColor = Color.Gray;
                        chkExamFinish.ToolTip = "Khám chính chỉ cho phép xử trí khám thêm, nhập viện hoặc kết thúc điều trị";
                        isReturnCheckboxExamFinish = true;
                    }
                    else if (this.HisServiceReqView.IS_MAIN_EXAM != 1)
                    {
                        chkTreatmentFinish.Checked = false;
                        chkHospitalize.Checked = false;
                        chkTreatmentFinish.Properties.Appearance.ForeColor = Color.Gray;
                        chkHospitalize.Properties.Appearance.ForeColor = Color.Gray;
                        chkHospitalize.ToolTip = "Khám phụ không cho phép xử trí nhập viện hoặc kết thúc điều trị";
                        chkTreatmentFinish.ToolTip = "Khám phụ không cho phép xử trí nhập viện hoặc kết thúc điều trị";
                        isReturnCheckboxHosTreat = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlFastCreateTracking()
        {
            try
            {
                if (treatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
                    layoutControlItem109.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDatatoComboContraindication()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("Treatment: ");
                try
                {
                    if (this.treatmentByPatients != null && this.treatmentByPatients.Count > 0)
                    {
                        this.oldContraindicationSelecteds = new List<long>();
                        var data = this.treatmentByPatients.FirstOrDefault();

                        if (!String.IsNullOrEmpty(data.CONTRAINDICATION_IDS))
                        {
                            string[] str = data.CONTRAINDICATION_IDS.Split(',');

                            foreach (var item in str)
                            {
                                long p = long.Parse(item);
                                this.oldContraindicationSelecteds.Add(p);
                            }
                        }
                    }
                    GridCheckMarksSelection gridCheckMark = cboContraindication.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboContraindication.Properties.View);
                        if (this.oldContraindicationSelecteds != null && this.oldContraindicationSelecteds.Count > 0)
                        {
                            List<HIS_CONTRAINDICATION> seleceds = this.datas.Where(o => this.oldContraindicationSelecteds.Contains(o.ID)).ToList();
                            gridCheckMark.SelectAll(seleceds);

                            string displayText = String.Join(", ", seleceds.Select(s => s.CONTRAINDICATION_NAME).ToList());
                            cboContraindication.Text = displayText;
                            Inventec.Common.Logging.LogSystem.Debug("this.oldContraindicationSelecteds.Count " + this.oldContraindicationSelecteds.Count);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// sửa iss #52065
        /// nhưng khi enable thì nội dung của câu thông báo không hiển thị nên chuyển thành ReadOnly.
        /// </summary>
        private void KeyAllowToEnableIcdSubCode()
        {
            try
            {
                long AllowToEnableIcdSubCode = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.DISABLE_ICD_SUB_CODE_TEXTBOX));
                if (AllowToEnableIcdSubCode == 0)
                {
                    //txtIcdSubCode.Enabled = true;
                    txtIcdSubCode.ReadOnly = false;
                }
                else if (AllowToEnableIcdSubCode == 1)
                {
                    txtIcdSubCode.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadEmrCoverConfig()
        {
            try
            {
                LstEmrCoverConfig = new List<HIS_EMR_COVER_CONFIG>();
                LstEmrCoverConfigDepartment = new List<HIS_EMR_COVER_CONFIG>();

                HIS_TREATMENT ResultTreatment = treatment;


                LstEmrCoverConfig = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    && o.ROOM_ID == moduleData.RoomId && o.TREATMENT_TYPE_ID == ResultTreatment.TDL_TREATMENT_TYPE_ID
                    ).ToList();


                var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == moduleData.RoomId).DepartmentId;

                LstEmrCoverConfigDepartment = BackendDataWorker.Get<HIS_EMR_COVER_CONFIG>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
            && o.DEPARTMENT_ID == DepartmentID && o.TREATMENT_TYPE_ID == ResultTreatment.TDL_TREATMENT_TYPE_ID).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCombo()
        {
            try
            {
                HisContraindicationFilter filter = new HisContraindicationFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                datas = new BackendAdapter(new CommonParam()).Get<List<HIS_CONTRAINDICATION>>("api/HisContraindication/Get", ApiConsumers.MosConsumer, filter, null);
                cboContraindication.Properties.DataSource = datas;
                cboContraindication.Properties.DisplayMember = "CONTRAINDICATION_NAME";
                cboContraindication.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboContraindication.Properties.View.Columns.AddField("CONTRAINDICATION_NAME");

                col2.VisibleIndex = 1;
                col2.Width = 350;
                col2.Caption = "Tất cả";
                cboContraindication.Properties.PopupFormWidth = 350;
                cboContraindication.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboContraindication.Properties.View.OptionsSelection.MultiSelect = true;
                cboContraindication.Properties.View.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DefaultBoolean.True;

                GridCheckMarksSelection gridCheckMark = cboContraindication.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cboContraindication.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitContraindicationCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboContraindication.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__Contraindication);
                cboContraindication.Properties.Tag = gridCheck;
                cboContraindication.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboContraindication.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboContraindication.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Contraindication(object sender, EventArgs e)
        {
            try
            {
                this.contraindicationSelecteds = new List<HIS_CONTRAINDICATION>();
                foreach (MOS.EFMODEL.DataModels.HIS_CONTRAINDICATION rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        this.contraindicationSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboContraindication_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string display = "";
                if (this.contraindicationSelecteds != null && this.contraindicationSelecteds.Count > 0)
                {
                    foreach (var item in this.contraindicationSelecteds)
                    {
                        if (display.Trim().Length > 0)
                        {
                            display += ", " + item.CONTRAINDICATION_NAME;
                        }
                        else
                            display = item.CONTRAINDICATION_NAME;
                    }
                }
                e.DisplayText = display;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitForm()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("timer1_Tick .1");
                StopTimer(moduleData.ModuleLink, "timerInitForm");
                //this.timerRefreshExamFinish.Start();
                //List<Action> methods = new List<Action>();
                //methods.Add(LoadMediRecord);
                //methods.Add(LoadPatientProgram);
                //methods.Add(LoadDataStore);
                //ThreadCustomManager.MultipleThreadWithJoin(methods);
                LoadDataStore();
                LoadMediRecord();
                LoadPatientProgram();
                isFinishLoad = true;
                if (this.ucTreatmentFinish != null && this.treatmentFinishProcessor != null)
                {
                    this.treatmentFinishProcessor.UpdateProgramData(this.ucTreatmentFinish, PatientProgramList, DataStoreList);
                }

                this.LoadTreatmentHistoryTogrid();//TODO
                this.FillDataAllergenic();
                this.FillDataToButtonPrintAndAutoPrint();
                this.InitDrButtonOther();
                LoadgridControlDKPresent();
                Inventec.Common.Logging.LogSystem.Debug("timer1_Tick .2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Shortcut Tab

        public void ShortCutCtrl0()
        {
            xtraTabPageChung.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabPageChung;
            txtKhamBoPhan.Focus();
            txtKhamBoPhan.SelectAll();
        }

        public void ShortCutCtrl1()
        {
            xtraTabTuanHoan.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabTuanHoan;
            txtTuanHoan.Focus();
            txtTuanHoan.SelectAll();
        }
        public void ShortCutCtrl2()
        {
            xtraTabHoHap.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabHoHap;
            txtHoHap.Focus();
            txtHoHap.SelectAll();
        }

        public void ShortCutCtrl3()
        {
            xtraTabTieuHoa.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabTieuHoa;
            txtTieuHoa.Focus();
            txtTieuHoa.SelectAll();
        }
        public void ShortCutCtrl4()
        {
            xtraTabThanTietNieu.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabThanTietNieu;
            txtThanTietNieu.Focus();
            txtThanTietNieu.SelectAll();
        }

        public void ShortCutCtrl5()
        {
            xtraTabThanKinh.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabThanKinh;
            txtThanKinh.Focus();
            txtThanKinh.SelectAll();
        }
        public void ShortCutCtrl6()
        {
            xtraTabCoXuongKhop.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabCoXuongKhop;
            txtCoXuongKhop.Focus();
            txtCoXuongKhop.SelectAll();
        }

        public void ShortCutCtrl7()
        {
            xtraTabTaiMuiHong.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabTaiMuiHong;
            txtTai.Focus();
            txtTai.SelectAll();
        }
        public void ShortCutCtrl8()
        {
            xtraTabRangHamMat.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabRangHamMat;
            txtRHM.Focus();
            txtRHM.SelectAll();
        }

        public void ShortCutCtrl9()
        {
            xtraTabMat.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabMat;
            txtMat.Focus();
            txtMat.SelectAll();
        }
        public void ShortCutCtrlQ()
        {
            xtraTabNoiTiet.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabNoiTiet;
            txtNoiTiet.Focus();
            txtNoiTiet.SelectAll();
        }

        public void ShortCutCtrlW()
        {
            xtraTabTamThan.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabTamThan;
            txtPartExamMental.Focus();
            txtPartExamMental.SelectAll();
        }

        public void ShortCutCtrlU()
        {
            xtraTabDinhDuong.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabDinhDuong;
            txtPartExamNutrition.Focus();
            txtPartExamNutrition.SelectAll();
        }

        public void ShortCutCtrlR()
        {
            xtraTabVanDong.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabVanDong;
            txtPartExamMotion.Focus();
            txtPartExamMotion.SelectAll();
        }

        public void ShortCutCtrlT()
        {
            xtraTabSanPhuKhoa.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabSanPhuKhoa;
            txtPartExanObstetric.Focus();
            txtPartExanObstetric.SelectAll();
        }

        public void ShortCutCtrlI()
        {
            xtraTabDaLieu.PageVisible = true;
            xtraTabControlInfo.SelectedTabPage = xtraTabDaLieu;
            txtDaLieu.Focus();
            txtDaLieu.SelectAll();
        }

        #endregion

        #region Control editor

        private void cboPatientCase_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboPatientCase.Properties.Buttons[1].Visible = cboPatientCase.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientCase_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPatientCase.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientCase_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHospitalizationReason.Focus();
                    txtHospitalizationReason.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void item_click(object sender, EventArgs e)
        {
            try
            {
                DXMenuItem item = sender as DXMenuItem;
                if (item.Tag is XtraTabPage)
                {
                    XtraTabPage tab = item.Tag as XtraTabPage;
                    tab.PageVisible = true;
                    tabControlDetailData.SelectedTabPage = tab;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tabControlDetailData_CustomHeaderButtonClick(object sender, DevExpress.XtraTab.ViewInfo.CustomHeaderButtonEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DXPopupMenu _menu = new DevExpress.Utils.Menu.DXPopupMenu();

                    foreach (XtraTabPage item in tabControlDetailData.TabPages)
                    {
                        if (item.PageVisible == false)
                        {
                            DXMenuItem itemTuanHoan = new DXMenuItem();
                            itemTuanHoan.Caption = item.Text.Trim();
                            itemTuanHoan.Click += item_click;
                            itemTuanHoan.Tag = item;
                            _menu.Items.Add(itemTuanHoan);
                        }
                    }

                    DevExpress.XtraBars.BarManager mobjBarMgr = new DevExpress.XtraBars.BarManager();
                    mobjBarMgr.Form = this;
                    DevExpress.Utils.Menu.MenuManagerHelper.ShowMenu(_menu, tabControlDetailData.LookAndFeel, mobjBarMgr, tabControlDetailData, tabControlDetailData.PointToClient(Control.MousePosition));
                }
                else if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    XtraTabPage selectedTab = tabControlDetailData.SelectedTabPage;
                    if (selectedTab != null)
                    {
                        selectedTab.PageVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNhanApPhai_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                lblMatPhai.Text = "";
                if (!String.IsNullOrWhiteSpace(txtNhanApPhai.Text.Trim()))
                {
                    bool isNum = true;
                    foreach (var item in txtNhanApPhai.Text.Trim())
                    {
                        if (!char.IsNumber(item))
                        {
                            isNum = false;
                            break;
                        }
                    }

                    if (isNum)
                    {
                        lblMatPhai.Text = "mmHg";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtThiLucKhongKinhPhai_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LblKoKinhPhai.Text = "";
                if (!String.IsNullOrWhiteSpace(txtThiLucKhongKinhPhai.Text.Trim()))
                {
                    bool isNum = true;
                    foreach (var item in txtThiLucKhongKinhPhai.Text.Trim())
                    {
                        if (!char.IsNumber(item))
                        {
                            isNum = false;
                            break;
                        }
                    }

                    if (isNum)
                    {
                        LblKoKinhPhai.Text = "/10";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtThiLucCoKinhPhai_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LblCoKinhPhai.Text = "";
                if (!String.IsNullOrWhiteSpace(txtThiLucCoKinhPhai.Text.Trim()))
                {
                    bool isNum = true;
                    foreach (var item in txtThiLucCoKinhPhai.Text.Trim())
                    {
                        if (!char.IsNumber(item))
                        {
                            isNum = false;
                            break;
                        }
                    }

                    if (isNum)
                    {
                        LblCoKinhPhai.Text = "/10";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNhanApTrai_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LblMatTrai.Text = "";
                if (!String.IsNullOrWhiteSpace(txtNhanApTrai.Text.Trim()))
                {
                    bool isNum = true;
                    foreach (var item in txtNhanApTrai.Text.Trim())
                    {
                        if (!char.IsNumber(item))
                        {
                            isNum = false;
                            break;
                        }
                    }

                    if (isNum)
                    {
                        LblMatTrai.Text = "mmHg";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtThiLucKhongKinhTrai_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LblKoKinhTrai.Text = "";
                if (!String.IsNullOrWhiteSpace(txtThiLucKhongKinhTrai.Text.Trim()))
                {
                    bool isNum = true;
                    foreach (var item in txtThiLucKhongKinhTrai.Text.Trim())
                    {
                        if (!char.IsNumber(item))
                        {
                            isNum = false;
                            break;
                        }
                    }

                    if (isNum)
                    {
                        LblKoKinhTrai.Text = "/10";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtThiLucCoKinhTrai_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LblCoKinhTrai.Text = "";
                if (!String.IsNullOrWhiteSpace(txtThiLucCoKinhTrai.Text.Trim()))
                {
                    bool isNum = true;
                    foreach (var item in txtThiLucCoKinhTrai.Text.Trim())
                    {
                        if (!char.IsNumber(item))
                        {
                            isNum = false;
                            break;
                        }
                    }

                    if (isNum)
                    {
                        LblCoKinhTrai.Text = "/10";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPathologicalHistory_Leave(object sender, EventArgs e)
        {
            txtPathologicalHistoryFamily.Focus();
            txtPathologicalHistoryFamily.SelectAll();
        }

        private void txtHospitalizationReason_Leave(object sender, EventArgs e)
        {
            //txtPathologicalProcess.Focus();
            //txtPathologicalProcess.SelectAll();
        }

        private void txtPathologicalProcess_Leave(object sender, EventArgs e)
        {
            //txtPathologicalHistory.Focus();
            //txtPathologicalHistory.SelectAll();
        }

        private void spinBreathRate_Leave(object sender, EventArgs e)
        {
            //spinWeight.Focus();
            //spinWeight.SelectAll();
        }

        private void spinHeight_Leave(object sender, EventArgs e)
        {

        }

        private void spinBelly_Leave(object sender, EventArgs e)
        {
            txtNote.Focus();
            txtNote.SelectAll();
        }

        private void cboContraindication_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                cboContraindication.Text = "";
                string display = "";
                if (this.contraindicationSelecteds != null && this.contraindicationSelecteds.Count > 0)
                {
                    foreach (var item in this.contraindicationSelecteds)
                    {
                        if (display.Trim().Length > 0)
                        {
                            display += ", " + item.CONTRAINDICATION_NAME;
                        }
                        else
                            display = item.CONTRAINDICATION_NAME;
                    }
                }
                cboContraindication.Text = display;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKskCode_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboKskCode.EditValue != null)
                    {
                        txtIcdCode.Focus();
                        txtIcdCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinHeight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinSPO2.Focus();
                    spinSPO2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSPO2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinTemperature.Focus();
                    spinTemperature.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinTemperature_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBreathRate.Focus();
                    spinBreathRate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBreathRate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinChest.Focus();
                    spinChest.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinChest_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBelly.Focus();
                    spinBelly.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBelly_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKskCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            txtIcdCode.Focus();
            txtIcdCode.SelectAll();
        }

        private void spinWeight_Leave(object sender, EventArgs e)
        {
            try
            {
                spinHeight.Focus();
                spinHeight.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboIcds_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void spinPulse_Leave(object sender, EventArgs e)
        {
            try
            {
                spinBloodPressureMax.Focus();
                spinBloodPressureMax.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBloodPressureMax_Leave(object sender, EventArgs e)
        {
            try
            {
                spinBloodPressureMin.Focus();
                spinBloodPressureMin.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBloodPressureMin_Leave(object sender, EventArgs e)
        {
            try
            {
                spinWeight.Focus();
                spinWeight.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SpinKeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
       (e.KeyChar != '.') && (e.KeyChar != ','))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPART_EXAM_EAR_RIGHT_NORMAL_KeyPress(object sender, KeyPressEventArgs e)
        {
            SpinKeyPress(sender, e);
        }

        private void txtPART_EXAM_EAR_RIGHT_WHISPER_KeyPress(object sender, KeyPressEventArgs e)
        {
            SpinKeyPress(sender, e);
        }

        private void txtPART_EXAM_EAR_LEFT_NORMAL_KeyPress(object sender, KeyPressEventArgs e)
        {
            SpinKeyPress(sender, e);
        }

        private void txtPART_EXAM_EAR_LEFT_WHISPER_KeyPress(object sender, KeyPressEventArgs e)
        {
            SpinKeyPress(sender, e);
        }

        private void txtPART_EXAM_EAR_RIGHT_NORMAL_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void txtPART_EXAM_EAR_RIGHT_WHISPER_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void txtPART_EXAM_EAR_LEFT_NORMAL_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void txtPART_EXAM_EAR_LEFT_WHISPER_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void chkPART_EXAM_EYE_BLIND_COLOR__BT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked)
                {
                    chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = false;
                    chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked = !chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked;
                }
                Inventec.Common.Logging.LogSystem.Debug("chkPART_EXAM_EYE_BLIND_COLOR__BT_CheckedChanged: chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked=" + chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked + "____chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked=" + chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPART_EXAM_EYE_BLIND_COLOR__MMTB_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked)
                {
                    chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = false;
                    chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked = !chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked;
                }
                Inventec.Common.Logging.LogSystem.Debug("chkPART_EXAM_EYE_BLIND_COLOR__MMTB_CheckedChanged: chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked=" + chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked + "____chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked=" + chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPART_EXAM_EYE_BLIND_COLOR__MMD_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked)
                    chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPART_EXAM_EYE_BLIND_COLOR__MMXLC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked)
                    chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPART_EXAM_EYE_BLIND_COLOR__MMV_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                    chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtInfomationExecute_Click(object sender, EventArgs e)
        {
            try
            {
                dataSelectedToPTDT = new List<string>();
                string contentShare = txtTreatmentInstruction.Text.Trim();
                if (!string.IsNullOrEmpty(contentShare))
                {
                    if (contentShare.Contains(";"))
                    {
                        string[] serviceName = contentShare.Split(';');
                        foreach (var item in serviceName)
                        {
                            dataSelectedToPTDT.Add(item.Trim());
                        }
                    }
                    else
                    {
                        dataSelectedToPTDT.Add(contentShare);
                    }
                }
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InfomationExecute").FirstOrDefault();

                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.InfomationExecute'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.treatmentId);
                    listArgs.Add(this.dataSelectedToPTDT);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)dataResult);
                    listArgs.Add(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();

                }
                else
                {
                    MessageManager.Show("Chức năng chưa được hỗ trợ ở phiên bản này");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dataResult(object data)
        {
            try
            {
                if (data != null && data is string)
                {
                    string dt = data as string;


                    //if (!string.IsNullOrEmpty(dt))
                    //{
                    //    if (dt.Contains(";"))
                    //    {
                    //        string[] serviceName = dt.Split(';');
                    //        foreach (var item in serviceName)
                    //        {
                    //            dataSelectedToPTDT.Add(item.Trim());
                    //        }
                    //    }
                    //    else
                    //    {
                    //        dataSelectedToPTDT.Add(dt);
                    //    }
                    //}

                    if (!string.IsNullOrEmpty(dt))
                    {
                        txtTreatmentInstruction.Text = string.Join("; ",
                            data
                            // dataSelectedToPTDT.Distinct()
                            );
                    }
                    else
                    {
                        txtTreatmentInstruction.Text = "";
                    }

                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void txtProvisionalDianosis_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdCode.Focus();
                    txtIcdCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinNgayThuCuaBenh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPatientCase.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKhamToanThan_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtKhamToanThan_Leave(object sender, EventArgs e)
        {
            try
            {
                txtKhamBoPhan.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string HistoryTimeFormat(long intime, long? outtime)
        {
            string strTime = "";
            try
            {
                if (!outtime.HasValue)
                    strTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(intime);
                else
                {
                    string dateIn = intime.ToString().Substring(0, 8);
                    string dateOut = outtime.ToString().Substring(0, 8);
                    if (dateIn == dateOut)
                        strTime = intime.ToString().Substring(6, 2) + "/" + intime.ToString().Substring(4, 2) + "/" + intime.ToString().Substring(0, 4)
                            + " " + intime.ToString().Substring(8, 2) + ":" + intime.ToString().Substring(10, 2) + " - " + outtime.Value.ToString().Substring(8, 2) + ":" + outtime.Value.ToString().Substring(10, 2);
                    else
                        strTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(intime) + " " + Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(outtime.Value);
                }
            }
            catch (Exception ex)
            {
                strTime = "";
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return strTime;
        }

        private void txtResultNote_Leave(object sender, EventArgs e)
        {
            try
            {
                UcIcdFocusComtrol();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPathologicalHistoryFamily_Leave(object sender, EventArgs e)
        {
            try
            {
                UcDHSTFocusComtrol();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtThanTietNieu_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void SetCheckExecute(bool set, CheckEdit check, CheckEdit uncheck1, CheckEdit uncheck2, CheckEdit uncheck3)
        {
            try
            {
                check.Checked = set;
                uncheck1.Checked = !set;
                uncheck2.Checked = !set;
                uncheck3.Checked = !set;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkExamServiceAdd_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkExamServiceAdd.Checked)
                {
                    SetCheckExecute(true, chkExamServiceAdd, chkHospitalize, chkTreatmentFinish, chkExamFinish);
                    ResetPrintExecuteExt();
                    examServiceAddProcessor = new HIS.UC.ExamServiceAdd.ExamServiceAddProcessor();
                    ExamServiceAddInitADO examServiceAddADO = new ExamServiceAddInitADO();
                    examServiceAddADO.ServiceReqId = this.HisServiceReqView.ID;
                    examServiceAddADO.treatmentId = this.HisServiceReqView.TREATMENT_ID;
                    examServiceAddADO.roomId = moduleData.RoomId;
                    examServiceAddADO.FinishTime = this.HisServiceReqView.FINISH_TIME;
                    examServiceAddADO.OutTime = this.treatment.OUT_TIME;
                    examServiceAddADO.InTime = this.treatment.IN_TIME;
                    examServiceAddADO.StartTime = this.HisServiceReqView.START_TIME;
                    examServiceAddADO.AppointmentDesc = this.HisServiceReqView.APPOINTMENT_DESC;
                    examServiceAddADO.AppointmentTime = this.HisServiceReqView.APPOINTMENT_TIME;
                    examServiceAddADO.IsMainExam = this.HisServiceReqView.IS_MAIN_EXAM == 1 ? true : false;
                    var dataRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
                    examServiceAddADO.IsBlockNumOrder = dataRoom.IS_BLOCK_NUM_ORDER == 1 ? true : false;
                    examServiceAddADO.DefaultIdRoom = this.moduleData.RoomId;
                    examServiceAddADO.Note = CurrentPatient.NOTE;
                    CommonParam param = new CommonParam();
                    HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                    filter.TreatmentId = treatmentId;
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                    var currentHisPatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, param);
                    if (HisConfigCFG.IsNotRequiredFee && currentHisPatientTypeAlter.PATIENT_TYPE_ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT)
                    {
                        examServiceAddADO.IsNotRequiredFee = true;
                    }
                    Inventec.Common.Logging.LogSystem.Debug("examServiceAddADO.IsNotRequiredFee: " + examServiceAddADO.IsNotRequiredFee);
                    this.ucExamAddition = (UserControl)examServiceAddProcessor.Run(examServiceAddADO);
                    LoadUCToPanelExecuteExt(this.ucExamAddition, chkExamServiceAdd);
                }
                else
                {
                    LoadUCToPanelExecuteExt(null, chkExamServiceAdd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHospitalize_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isReturnCheckboxHosTreat)
                {
                    chkHospitalize.Checked = false;
                    return;
                }

                if (chkHospitalize.Checked)
                {

                    SetCheckExecute(true, chkHospitalize, chkExamServiceAdd, chkTreatmentFinish, chkExamFinish);
                    ResetPrintExecuteExt();
                    long departmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == moduleData.RoomId).DepartmentId;
                    HIS.UC.Hospitalize.ADO.HospitalizeInitADO hospitalizeADO = new UC.Hospitalize.ADO.HospitalizeInitADO();
                    hospitalizeADO.DepartmentId = departmentId;
                    hospitalizeADO.dlgRefeshIcd = DlgIcdSubCode;
                    hospitalizeADO.dlgSendIcd = GetIcdSubCode;
                    hospitalizeADO.Treatment = treatment;
                    hospitalizeADO.TreatmentId = this.HisServiceReqView.TREATMENT_ID;
                    hospitalizeADO.FinishTime = this.HisServiceReqView.FINISH_TIME;
                    hospitalizeADO.OutTime = this.treatment.OUT_TIME;
                    hospitalizeADO.InTime = this.treatment.IN_TIME;
                    hospitalizeADO.StartTime = this.HisServiceReqView.START_TIME;
                    hospitalizeADO.ModuleLink = this.moduleData.ModuleLink;
                    hospitalizeADO.IcdCode = this.icdDefaultFinish.ICD_CODE;
                    hospitalizeADO.IcdName = this.icdDefaultFinish.ICD_NAME;
                    hospitalizeADO.TraditionalIcdCode = this.treatment.TRADITIONAL_ICD_CODE;
                    hospitalizeADO.TraditionalIcdName = this.treatment.TRADITIONAL_ICD_NAME;
                    if (this.patient == null || this.patient.ID == 0)
                    {
                        LoadPatient();
                    }
                    if (patient != null)
                    {
                        hospitalizeADO.RelativeAddress = patient.RELATIVE_ADDRESS;
                        hospitalizeADO.RelativeName = patient.RELATIVE_NAME;
                        hospitalizeADO.RelativePhone = patient.RELATIVE_PHONE;
                        hospitalizeADO.CareerId = patient.CAREER_ID;
                    }
                    else
                    {
                        hospitalizeADO.RelativeAddress = "";
                        hospitalizeADO.RelativeName = "";
                        hospitalizeADO.RelativePhone = "";
                    }
                    hospitalizeADO.isEmergency = this.treatment.IS_EMERGENCY != null ? true : false;
                    hospitalizeADO.InHospitalizationReasonCode = this.treatment.HOSPITALIZE_REASON_CODE;
                    hospitalizeADO.InHospitalizationReasonName = this.treatment.HOSPITALIZE_REASON_NAME;
                    hospitalizeADO.isAutoCheckChkHospitalizeExam = HisConfigCFG.IsAutoCheckPrintHospitalizeExam;
                    hospitalizeADO.Note = CurrentPatient.NOTE;
                    hospitalizeProcessor = new HospitalizeProcessor();
                    this.ucHospitalize = (UserControl)hospitalizeProcessor.Run(hospitalizeADO);
                    LoadUCToPanelExecuteExt(this.ucHospitalize, chkHospitalize);
                }
                else
                {
                    LoadUCToPanelExecuteExt(null, chkHospitalize);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetIcdSubCode()
        {
            string icdSubCode = null;
            try
            {
                icdSubCode = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return icdSubCode;
        }

        private void checkSign_CheckChange(CheckState checkState)
        {

        }

        private void checkPrintDocumentSign_CheckChange(CheckState checkState)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkExamFinish_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isReturnCheckboxExamFinish)
                {
                    chkExamFinish.Checked = false;
                    return;
                }
                if (chkExamFinish.Checked)
                {
                    //this.HisServiceReqView.IS_AUTO_FINISHED = 1;
                    SetCheckExecute(true, chkExamFinish, chkHospitalize, chkExamServiceAdd, chkTreatmentFinish);
                    ResetPrintExecuteExt();
                    HIS.UC.ExamFinish.ADO.ExamFinishInitADO examFinishInitADO = new HIS.UC.ExamFinish.ADO.ExamFinishInitADO();
                    examFinishInitADO.TreatmentId = this.HisServiceReqView.TREATMENT_ID;
                    examFinishInitADO.FinishTime = this.HisServiceReqView.FINISH_TIME;
                    examFinishInitADO.InTime = this.treatment.IN_TIME;
                    examFinishInitADO.OutTime = this.treatment.OUT_TIME;
                    examFinishInitADO.StartTime = this.HisServiceReqView.START_TIME;
                    examFinishInitADO.AppointmentDesc = this.HisServiceReqView.APPOINTMENT_DESC;
                    examFinishInitADO.AppointmentTime = this.HisServiceReqView.APPOINTMENT_TIME;
                    examFinishInitADO.IsMainExam = this.HisServiceReqView.IS_MAIN_EXAM == 1 ? true : false;
                    examFinishInitADO.Note = CurrentPatient.NOTE;
                    examFinishProcessor = new ExamFinishProcessor();
                    this.ucExamFinish = (UserControl)examFinishProcessor.Run(examFinishInitADO);
                    LoadUCToPanelExecuteExt(this.ucExamFinish, chkExamFinish);
                }
                else
                {
                    // this.HisServiceReqView.IS_AUTO_FINISHED = 0;
                    LoadUCToPanelExecuteExt(null, chkExamFinish);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMediRecord()
        {
            try
            {
                if (this.treatment != null && this.treatment.MEDI_RECORD_ID.HasValue)
                {
                    MOS.Filter.HisMediRecordFilter mediRecordFilter = new HisMediRecordFilter();
                    mediRecordFilter.ID = this.treatment.MEDI_RECORD_ID.Value;
                    var MediRecodes = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_RECORD>>("api/HisMediRecord/Get", ApiConsumer.ApiConsumers.MosConsumer, mediRecordFilter, null);
                    MediRecode = MediRecodes != null && MediRecodes.Count > 0 ? MediRecodes.FirstOrDefault() : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPatientProgram()
        {
            try
            {
                if (this.treatment != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadPatientProgram this.treatment.PATIENT_ID " + Inventec.Common.Logging.LogUtil.TraceData("", this.treatment.PATIENT_ID));

                    MOS.Filter.HisPatientProgramFilter patientProgramFilter = new HisPatientProgramFilter();
                    patientProgramFilter.PATIENT_ID = this.treatment.PATIENT_ID;
                    PatientProgramList = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_PROGRAM>>("api/HisPatientProgram/Get", ApiConsumer.ApiConsumers.MosConsumer, patientProgramFilter, null);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadPatientProgram this.treatment NULL");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataStore()
        {
            try
            {
                if (this.moduleData != null)
                {
                    var currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
                    MOS.Filter.HisDataStoreViewFilter dataStoreFilter = new HisDataStoreViewFilter();
                    dataStoreFilter.BRANCH_ID = currentRoom.BRANCH_ID;
                    DataStoreList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_DATA_STORE>>("api/HisDataStore/GetView", ApiConsumer.ApiConsumers.MosConsumer, dataStoreFilter, null);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadDataStore this.moduleData NULL");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkTreatmentFinish_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isReturnCheckboxHosTreat)
                {
                    chkTreatmentFinish.Checked = false;
                    return;
                }

                if (chkTreatmentFinish.Checked)
                {
                    if (!this.ValidForButtonOtherClick())
                    {
                        chkTreatmentFinish.Checked = false;
                        return;
                    }
                    SetCheckExecute(true, chkTreatmentFinish, chkExamServiceAdd, chkHospitalize, chkExamFinish);
                    ResetPrintExecuteExt();

                    this.onClickSaveFormAsyncForOtherButtonClick();

                    treatmentFinishProcessor = new ExamTreatmentFinishProcessor();
                    HIS.UC.ExamTreatmentFinish.ADO.TreatmentFinishInitADO treatmentFinishInitADO = new UC.ExamTreatmentFinish.ADO.TreatmentFinishInitADO();
                    treatmentFinishInitADO.BranchName = BranchDataWorker.Branch.BRANCH_NAME;
                    if (this.moduleData != null)
                        treatmentFinishInitADO.moduleData = this.moduleData;

                    if (this.patient == null || this.patient.ID == 0)
                    {
                        LoadPatient();
                    }
                    if (this.patient != null)
                    {
                        if (!string.IsNullOrEmpty(this.patient.CMND_NUMBER))
                        {
                            treatmentFinishInitADO.CmndDate = this.patient.CMND_DATE;
                            treatmentFinishInitADO.CmndNumber = this.patient.CMND_NUMBER;
                            treatmentFinishInitADO.CmndPlace = this.patient.CMND_PLACE;
                            treatmentFinishInitADO.DocumentType = "CMND";
                        }
                        else if (!string.IsNullOrEmpty(this.patient.CCCD_NUMBER))
                        {
                            treatmentFinishInitADO.CmndDate = this.patient.CCCD_DATE;
                            treatmentFinishInitADO.CmndNumber = this.patient.CCCD_NUMBER;
                            treatmentFinishInitADO.CmndPlace = this.patient.CCCD_PLACE;
                            treatmentFinishInitADO.DocumentType = "CCCD";
                        }
                        treatmentFinishInitADO.CareerId = this.patient.CAREER_ID;
                    }

                    treatmentFinishInitADO.Treatment = treatment;
                    GetTotalIcd();
                    if (dicIcd != null && dicIcd.Count > 0)
                    {
                        Dictionary<string, string> dicNotIcdMain = new Dictionary<string, string>();
                        if (dicIcd.ContainsKey(this.icdDefaultFinish.ICD_CODE))
                            dicNotIcdMain = dicIcd.Where(o => o.Key != this.icdDefaultFinish.ICD_CODE).ToDictionary(o => o.Key, o => o.Value);
                        else
                            dicNotIcdMain = dicIcd;
                        treatmentFinishInitADO.Treatment.ICD_SUB_CODE = String.Join(";", dicNotIcdMain.Keys);
                        treatmentFinishInitADO.Treatment.ICD_TEXT = String.Join(";", dicNotIcdMain.Values);
                    }
                    if (lstIcdText != null && lstIcdText.Count > 0)
                    {
                        treatmentFinishInitADO.Treatment.ICD_TEXT += ";" + String.Join(";", lstIcdText);
                    }
                    if (treatmentFinishInitADO.Treatment != null && String.IsNullOrEmpty(treatmentFinishInitADO.Treatment.SICK_LOGINNAME))
                    {
                        treatmentFinishInitADO.Treatment.SICK_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        treatmentFinishInitADO.Treatment.SICK_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                    }
                    treatmentFinishInitADO.PatientPrograms = PatientProgramList;
                    treatmentFinishInitADO.DataStores = DataStoreList;
                    treatmentFinishInitADO.MediRecord = MediRecode;
                    treatmentFinishInitADO.TreatmentEndTypeExts = GetTreatmentEndTypeExt();
                    treatmentFinishInitADO.IcdCode = this.icdDefaultFinish.ICD_CODE;
                    treatmentFinishInitADO.IcdName = this.icdDefaultFinish.ICD_NAME;
                    treatmentFinishInitADO.TraditionalIcdCode = this.treatment.TRADITIONAL_ICD_CODE;
                    treatmentFinishInitADO.TraditionalIcdName = this.treatment.TRADITIONAL_ICD_NAME;
                    treatmentFinishInitADO.moduleData = this.moduleData;
                    var dataRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
                    treatmentFinishInitADO.IsBlockNumOrder = dataRoom.IS_BLOCK_NUM_ORDER == 1 ? true : false;
                    treatmentFinishInitADO.dlgGetIcdSubCode = GetIcdSubCode;
                    treatmentFinishInitADO.Note = CurrentPatient.NOTE;
                    if (HisConfigCFG.IsAutoSetIcdWhenFinishInOtherExam && this.HisServiceReqView.IS_MAIN_EXAM != 1)
                    {
                        treatmentFinishInitADO.IsAutoSetIcdWhenFinishInOtherExam = true;
                    }
                    if (this.HisServiceReqView != null)
                    {
                        treatmentFinishInitADO.Advise = this.HisServiceReqView.ADVISE;
                        treatmentFinishInitADO.Conclusion = this.HisServiceReqView.CONCLUSION;
                    }
                    CommonParam param = new CommonParam();
                    HisSevereIllnessInfoFilter filter = new HisSevereIllnessInfoFilter();
                    filter.TREATMENT_ID = treatment.ID;
                    var dtSevere = new BackendAdapter(param).Get<List<HIS_SEVERE_ILLNESS_INFO>>("api/HisSevereIllnessInfo/Get", ApiConsumers.MosConsumer, filter, param);
                    if (dtSevere != null && dtSevere.Count > 0)
                    {
                        treatmentFinishInitADO.SevereIllNessInfo = dtSevere[0];
                        HisEventsCausesDeathFilter filterChild = new HisEventsCausesDeathFilter();
                        filterChild.SEVERE_ILLNESS_INFO_ID = dtSevere[0].ID;
                        var dtEventsCausesDeath = new BackendAdapter(param).Get<List<HIS_EVENTS_CAUSES_DEATH>>("api/HisEventsCausesDeath/Get", ApiConsumers.MosConsumer, filterChild, param);
                        treatmentFinishInitADO.ListEventsCausesDeath = dtEventsCausesDeath;
                    }
                    this.ucTreatmentFinish = (UserControl)treatmentFinishProcessor.Run(treatmentFinishInitADO);
                    LoadUCToPanelExecuteExt(this.ucTreatmentFinish, chkTreatmentFinish);

                }
                else
                {
                    LoadUCToPanelExecuteExt(null, chkTreatmentFinish);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DelegateSelectDataContentSubclinical(object data)
        {
            try
            {
                if (data != null && data is String)
                {
                    txtSubclinical.Text = data.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKskCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboKskCode.Properties.Buttons[1].Visible = cboKskCode.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKskCode_Leave(object sender, EventArgs e)
        {
            try
            {
                //txtIcdCode.Focus();
                //txtIcdCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboKskCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboKskCode.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentInstruction_Leave(object sender, EventArgs e)
        {

        }

        private void gridViewDKPresent_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    var dataRow = (V_HIS_SERVICE_REQ)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if (e.Column.FieldName == "INTRUCTION_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.INTRUCTION_TIME);
                        }
                        if (e.Column.FieldName == "ICD_CODE_ICD_NAME")
                        {
                            if (!string.IsNullOrEmpty(dataRow.ICD_CODE) && !string.IsNullOrEmpty(dataRow.ICD_NAME))
                            {
                                e.Value = Convert.ToString(dataRow.ICD_CODE + " - " + dataRow.ICD_NAME);
                            }
                            else if (!string.IsNullOrEmpty(dataRow.ICD_CODE))
                            {
                                e.Value = dataRow.ICD_CODE;
                            }
                            else if (!string.IsNullOrEmpty(dataRow.ICD_NAME))
                            {
                                e.Value = dataRow.ICD_NAME;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDiUng_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_ALLERGENIC data = (V_HIS_ALLERGENIC)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "DOUBT")
                        {
                            e.Value = data.IS_DOUBT == 1 ? true : false;
                        }
                        if (e.Column.FieldName == "SURE")
                        {
                            e.Value = data.IS_SURE == 1 ? true : false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatmentHistory_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TreatmentExamADO data = (TreatmentExamADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "HISTORY_TIME_DISPLAY")
                        {
                            e.Value = HistoryTimeFormat(data.IN_TIME, data.OUT_TIME);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDiseaseRelation_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo viewInfo = view.GetViewInfo() as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlDiseaseRelation_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter)
                {
                    DevExpress.XtraGrid.GridControl grid = sender as DevExpress.XtraGrid.GridControl;
                    DevExpress.XtraGrid.Views.Grid.GridView view = grid.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    if ((e.Modifiers == Keys.None && view.IsLastRow && view.FocusedColumn.VisibleIndex == view.VisibleColumns.Count - 1) || (e.Modifiers == Keys.Shift && view.IsFirstRow && view.FocusedColumn.VisibleIndex == 0))
                    {
                        if (view.IsEditing)
                            view.CloseEditor();
                        txtSubclinical.Focus();
                        txtSubclinical.SelectAll();
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderForLeftPanel_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlLeft == -1)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlLeft > edit.TabIndex)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Buttons click

        private void btnAssignPaan_Click(object sender, EventArgs e)
        {
            try
            {
                btnAssignPaan_Click_Action(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssignPaan_Click_Action(object sender, EventArgs e)
        {
            try
            {
                ValiTemperatureOption();
                if (!ValidForSave())
                {
                    //MessageBox.Show(ResourceMessage.ChuaNhapDayDuThongTinBatBuoc, ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (this.HisServiceReqView != null)
                {

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPaan").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPaan");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS.UC.Icd.ADO.IcdInputADO icdADO = this.UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                        SecondaryIcdDataADO icdSubADO = this.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                        listArgs.Add(icdADO);
                        listArgs.Add(icdSubADO);
                        listArgs.Add(this.HisServiceReqView.TREATMENT_ID);
                        listArgs.Add(this.HisServiceReqView.ID);
                        listArgs.Add(this.HisServiceReqView);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ddbtnOther_Click(object sender, EventArgs e)
        {

        }

        private void btnChooseTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                btnChooseTemplate_Click_Action(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChooseTemplate_Click_Action(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisExamServiceTemp").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisExamServiceTemp'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.HisExamServiceTemp' is not plugins");

                long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                ExamServiceTempADO examServiceTemp = new ExamServiceTempADO();
                examServiceTemp.IsCreatorOrPublic = true;
                examServiceTemp.DelegateSelectData = DataSelectReuslt;

                List<object> listArgs = new List<object>();
                listArgs.Add(examServiceTemp);// Koi tao ExamServiceTempADO va set vao cho nay
                listArgs.Add(SendexamServiceTemp());
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                WaitingManager.Hide();
                ((Form)extenceInstance).ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBedRoomIn_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisBedRoomIn").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisBedRoomIn");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(treatmentId);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveFinish_Click(object sender, EventArgs e)
        {
            try
            {
                LogTheadInSessionInfo(() => btnSaveFinish_Click_Action(sender, e), "btnSaveFinish_Click");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveFinish_Click_Action(object sender, EventArgs e)
        {
            try
            {
                IsPrintExam = false;
                IsSignExam = false;
                isNotCheckValidateIcdUC = false;
                isClickSaveFinish = true;
                isPrintAppoinment = false;
                isPrintBordereau = false;
                isPrintBANT = false;
                isPrintHospitalizeExam = false;
                isSign = false;
                isPrintSign = false;
                isPrintSurgAppoint = false;
                isInPhieuPhuLuc = false;
                isKyPhieuPhuLuc = false;
                isPrintPrescription = false;
                isPrintHosTransfer = false;
                IsReturn = false;
                SevereIllnessInfo = null;
                EventsCausesDeaths = new List<HIS_EVENTS_CAUSES_DEATH>();
                if ((HisConfigCFG.RequiredWeightHeight_Option == "1" || (chkHospitalize.Checked && HisConfigCFG.RequiredWeightHeight_Option == "2")) && !ValidDhstOption())
                    return;
                ValiTemperatureOption();
                if (!this.ValidForButtonOtherClick())
                {
                    isClickSaveFinish = false;
                    return;
                }
                isClickSaveFinish = false;

                if (this.requiredControl != null && this.requiredControl == 1 && string.IsNullOrEmpty(this.txtPathologicalProcess.Text.Trim()))
                {
                    MessageBox.Show("Quá trình bệnh lý bạn nhập không hợp lệ", ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!isNotCheckValidateIcdUC && chkHospitalize.Checked && ucHospitalize != null && !hospitalizeProcessor.ValidCheckIcd(ucHospitalize))
                {
                    return;
                }

                if (!VerifyTreatmentFinish())
                    return;

                if (!CheckExamServiceFinish())
                    return;
                if (!CheckWarningOverTotalPatientPrice())
                    return;
                if (HisConfigCFG.MustChooseSeviceExamOption == "1" && !CheckMustChooseSeviceExamOption())
                    return;
                HisServiceReqExamUpdateSDO hisServiceReqSDO = new HisServiceReqExamUpdateSDO();
                ProcessExamServiceReqExecute(HisServiceReqView, hisServiceReqSDO);
                if (IsReturn) return;
                //Nếu key HIS.HIS_SERVICE_REQ.EXAM.AUTO_FINISH_AFTER_UNFINISH =1 và IS_AUTO_FINISHED = 1 thì tự động kết                          thúc khám (gửi lên IsFinish = true và FinishTime).

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisServiceReqView), HisServiceReqView));
                if (hisServiceReqSDO.TreatmentFinishSDO != null)
                {
                    hisServiceReqSDO.IsFinish = true;
                    hisServiceReqSDO.FinishTime = hisServiceReqSDO.TreatmentFinishSDO.TreatmentFinishTime;
                }
                else if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS.HIS_SERVICE_REQ.EXAM.AUTO_FINISH_AFTER_UNFINISH") == 1 && HisServiceReqView.IS_AUTO_FINISHED == 1)
                {
                    hisServiceReqSDO.IsFinish = true;
                    hisServiceReqSDO.FinishTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                }

                if (this.CheckExecuteExt(hisServiceReqSDO)
                    && this.CheckHospitalizeTime(hisServiceReqSDO)
                    //&& this.CheckSunSatAppointmentTime(hisServiceReqSDO)//đã cảnh báo trong popup chọn thời gian hẹn khám
                    && this.CheckTreatmentFinish(hisServiceReqSDO)
                    && this.CheckExamFinish(hisServiceReqSDO)
                    )
                {
                    //Thảo ngọc bảo bỏ 10577
                    //if (hisServiceReqSDO != null
                    //&& hisServiceReqSDO.TreatmentFinishSDO != null
                    //&& HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.CHECK_FINISH_TIME) == "1"
                    //&& CheckFinishTime(hisServiceReqSDO))
                    //{
                    //    return;
                    //}

                    Inventec.Common.Logging.LogSystem.Debug("Du lieu gui len:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisServiceReqSDO), hisServiceReqSDO));
                    if (hisServiceReqSDO.ExamAdditionSDO != null && hisServiceReqSDO.ExamAdditionSDO.IsNotUseBhyt)
                    {
                        if (MessageBox.Show("Bệnh nhân không được hưởng bhyt các chi phí phát sinh tại phòng khám thêm. Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            SaveExamServiceReq(hisServiceReqSDO);
                        }
                    }
                    else
                    {
                        SaveExamServiceReq(hisServiceReqSDO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool CheckMustChooseSeviceExamOption()
        {
            bool rs = true;
            try
            {
                string serviceReqCode = null;
                if (((chkExamServiceAdd.Checked && examServiceAddProcessor != null && (this.examServiceAddProcessor.GetValueV2(this.ucExamAddition) as ExamServiceAddADO).IsFinishCurrent) || chkExamFinish.Checked) && string.IsNullOrEmpty(HisServiceReqView.TDL_SERVICE_IDS))
                {
                    serviceReqCode = HisServiceReqView.SERVICE_REQ_CODE;
                }
                if (chkTreatmentFinish.Checked)
                {
                    HisServiceReqFilter srFilter = new HisServiceReqFilter();
                    srFilter.TREATMENT_ID = treatment.ID;
                    srFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                    var serviceReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, srFilter, null);
                    if (serviceReqs != null && serviceReqs.Count > 0)
                    {
                        serviceReqs = serviceReqs.Where(o => o.IS_NO_EXECUTE != 1 && o.IS_DELETE != 1 && string.IsNullOrEmpty(o.TDL_SERVICE_IDS)).ToList();
                        if (serviceReqs != null && serviceReqs.Count > 0)
                        {
                            serviceReqCode = string.Join(", ", serviceReqs.Select(o => o.SERVICE_REQ_CODE));
                        }
                    }
                }
                if (!string.IsNullOrEmpty(serviceReqCode) && (XtraMessageBox.Show(String.Format("Y lệnh {0} thiếu dịch vụ khám. Bạn có muốn tiếp tục", serviceReqCode), ResourceMessage.ThongBao, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes))
                    rs = false;
            }
            catch (Exception ex)
            {
                rs = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }
        private void btnContentSubclinical_Click(object sender, EventArgs e)
        {
            try
            {
                btnContentSubclinical_Click_Action(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnContentSubclinical_Click_Action(object sender, EventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
                if (!IsValidForSave)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ContentSubclinical").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ContentSubclinical");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.HisServiceReqView.TREATMENT_ID);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)DelegateSelectDataContentSubclinical);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
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
                LogTheadInSessionInfo(() => btnSave_Click_Action(sender, e), "btnSave_Click");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click_Action(object sender, EventArgs e)
        {
            try
            {
                this.param = new CommonParam();
                if (HisServiceReqView.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                    return;

                if (!ValidForSave()) return;

                if (!CheckExamServiceFinish())
                    return;

                HisServiceReqExamUpdateSDO hisServiceReqSDO = new HisServiceReqExamUpdateSDO();

                ProcessExamServiceReqDTO(ref hisServiceReqSDO);
                ProcessExamSereIcdDTO(ref hisServiceReqSDO);
                ProcessExamSereNextTreatmentIntructionDTO(ref hisServiceReqSDO);
                ProcessExamSereDHST(ref hisServiceReqSDO);

                WaitingManager.Show();
                hisServiceReqSDO.RequestRoomId = moduleData.RoomId;
                HisServiceReqExamUpdateResultSDO HisServiceReqResult = new BackendAdapter(param)
                    .Post<HisServiceReqExamUpdateResultSDO>("api/HisServiceReq/ExamUpdate", ApiConsumers.MosConsumer, hisServiceReqSDO, param);
                WaitingManager.Hide();
                if (HisServiceReqResult != null)
                {
                    this.HisServiceReqView = new V_HIS_SERVICE_REQ();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERVICE_REQ>(this.HisServiceReqView, HisServiceReqResult.ServiceReq);
                    EnableButtonByServiceReq(HisServiceReqResult.ServiceReq.SERVICE_REQ_STT_ID);
                    this.BtnRefreshForFormOther();
                    if (reLoadServiceReq != null)
                        reLoadServiceReq(HisServiceReqResult.ServiceReq);
                    btnPrint_ExamService.Enabled = true;

                    // minhnq
                    // Phòng làm việc được cấu hình “Phòng thu ngân” và “Sổ thanh toán”
                    var _hisRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(p => p.ID == this.moduleData.RoomId);
                    if (HisConfigCFG.executeRoomPaymentOption == "2" && _hisRoom.DEFAULT_CASHIER_ROOM_ID != null && _hisRoom.BILL_ACCOUNT_BOOK_ID != null)
                    {
                        ProcessPayment(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void ProcessPayment(bool showMessage)
        {
            try
            {
                var treatmentFee4 = GetTreatmentFee4();
                if (treatmentFee4 != null)
                {
                    var unpaidAmout = (decimal)treatmentFee4.TOTAL_PATIENT_PRICE - (decimal)treatmentFee4.TOTAL_BILL_AMOUNT - (decimal)treatmentFee4.TOTAL_DEPOSIT_AMOUNT - (decimal)treatmentFee4.TOTAL_DEBT_AMOUNT + (decimal)treatmentFee4.TOTAL_BILL_TRANSFER_AMOUNT + (decimal)treatmentFee4.TOTAL_REPAY_AMOUNT;
                    // Diện điều trị của hồ sơ là "Khám"
                    // Số tiền bệnh nhân còn phải trả > 0
                    // Bệnh nhân có thông tin thẻ khám chữa bệnh thông minh trên hệ thống và chưa bị khóa
                    if (unpaidAmout <= 0 || treatmentFee4.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM || treatmentFee4.CARD_SERVICE_CODE == null || treatmentFee4.CARD_IS_ACTIVE != 1)
                    {
                        if (showMessage) XtraMessageBox.Show("Bệnh nhân không có chi phí cần thanh toán hoặc chưa có thẻ KCB thông minh", "Thông báo");
                        return;
                    }
                    var balance = new BackendAdapter(new CommonParam()).Get<decimal?>("api/HisPatient/GetCardBalance", ApiConsumers.MosConsumer, this.HisServiceReqView.TDL_PATIENT_ID, null);
                    if (balance == null || balance < unpaidAmout)
                    {
                        if (showMessage) XtraMessageBox.Show(string.Format("Tài khoản thẻ của bệnh nhân không đủ số dư để thực hiện thanh toán (số dư: {0}, chi phí cần thanh toán: {1})", balance, unpaidAmout), "Thông báo");
                        return;
                    }

                    var paidAmount = (decimal)treatmentFee4.TOTAL_BILL_AMOUNT + (decimal)treatmentFee4.TOTAL_DEPOSIT_AMOUNT + (decimal)treatmentFee4.TOTAL_DEBT_AMOUNT - (decimal)treatmentFee4.TOTAL_BILL_TRANSFER_AMOUNT - (decimal)treatmentFee4.TOTAL_REPAY_AMOUNT;
                    frmPayment frm = new frmPayment((decimal)treatmentFee4.TOTAL_PATIENT_PRICE, paidAmount, unpaidAmout, (decimal)balance, CheckPayment);
                    frm.ShowDialog();

                    if (!isPayment) return;


                    if (GlobalVariables.portComConnected == null || !GlobalVariables.portComConnected.IsConnected)
                    {
                        frmConnectCOM frmConnect = new frmConnectCOM();
                        frmConnect.ShowDialog();
                    }

                    if (GlobalVariables.portComConnected != null)
                    {
                        var rs = GlobalVariables.portComConnected.SendPos();
                        if (rs == null || !rs.IsSuccess || !rs.Data.Equals(treatmentFee4.CARD_SERVICE_CODE))
                        {
                            XtraMessageBox.Show("Thẻ không hợp lệ hoặc thuộc bệnh nhân khác", "Thông báo");
                            return;
                        }
                        WaitingManager.Show();
                        bool success = false;
                        CommonParam param = new CommonParam();
                        EpaymentBillSDO sdo = new EpaymentBillSDO();
                        sdo.CardServiceCode = rs.Data;
                        sdo.RequestRoomId = this.moduleData.RoomId;
                        sdo.TreatmentId = this.treatmentId;
                        // TODO
                        resultEPayment = new BackendAdapter(param).Post<EpaymentBillResultSDO>("api/HisTransaction/EpaymentBill", ApiConsumers.MosConsumer, sdo, param);
                        if (resultEPayment != null)
                        {
                            success = true;
                            PrintProcess(PrintType.PHIEU_THU_THANH_TOAN);
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);

                    }

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void CheckPayment(bool payment)
        {
            try
            {
                this.isPayment = payment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckExamServiceFinish()
        {
            bool result = true;
            try
            {
                //TODO
                if (this.chkExamFinish.Checked)
                {
                    //result = false;
                    var _hisRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ID == this.moduleData.RoomId);
                    if (_hisRoom != null && _hisRoom != null)
                    {
                        MOS.Filter.HisDepartmentTranFilter filter = new HisDepartmentTranFilter();
                        filter.TREATMENT_ID = this.treatmentId;
                        filter.ORDER_FIELD = "MODIFY_TIME";
                        filter.ORDER_DIRECTION = "DESC";
                        var datas = new BackendAdapter(null).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, filter, null);
                        if (datas != null && datas.Count > 0)
                        {
                            var dataaaa = datas.Where(p => p.DEPARTMENT_IN_TIME != null && p.DEPARTMENT_IN_TIME > 0).OrderByDescending(p => p.DEPARTMENT_IN_TIME ?? 0).FirstOrDefault();
                            if (dataaaa != null && dataaaa.DEPARTMENT_ID == _hisRoom.DEPARTMENT_ID)
                            {
                                MOS.Filter.HisServiceReqFilter _reqFilter = new HisServiceReqFilter();
                                _reqFilter.TREATMENT_ID = this.treatmentId;
                                _reqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;//CHeck lai xem có phai chi lay yc kahm hay k?

                                var dataReqs = new BackendAdapter(null).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, _reqFilter, null);
                                if (dataReqs != null && dataReqs.Count > 0)
                                {
                                    //Nếu KHÔNG tồn tại y/c khám nào (khác với y/c đang xử lý) mà đang ở trạng thái chưa kết thúc
                                    var dataCheck = dataReqs.FirstOrDefault(p => p.ID != this.HisServiceReqView.ID && p.FINISH_TIME == null);
                                    if (dataCheck == null)
                                    {
                                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân chưa kết thúc điều trị, bạn có muốn kết thúc điều trị không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                        {
                                            this.chkTreatmentFinish.Focus();
                                            this.chkTreatmentFinish.Checked = true;
                                            result = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    isNotCheckValidateIcdUC = true;
                    result = treatmentFinishProcessor.Validate(ucTreatmentFinish, isNotCheckValidateIcdUC);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
                if (!IsValidForSave)
                    return;
                //Cap nhat du lieu de in an
                //ExamServiceReqExecuteControlProcess.InitCurrentDataExamServiceReqGetView1(ref SereServExt, HisServiceReqView.ID);

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisServiceReqFilter hisServiceReqFilter = new HisServiceReqFilter();
                hisServiceReqFilter.TREATMENT_ID = HisServiceReqView.TREATMENT_ID;
                long? finishTime = null;

                List<HIS_SERVICE_REQ> hisServiceReqKT = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, hisServiceReqFilter, param);

                WaitingManager.Hide();

                if (hisServiceReqKT == null)
                {
                    return;
                }
                else
                {
                    positionHandleControlLeft = -1;
                    if (!dxValidationProviderForLeftPanel.Validate())
                    {
                        return;
                    }
                }

                bool success = false;
                if (HisServiceReqView == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("HisServiceReqWithOrderSDO is null");
                    return;
                }
                //WaitingManager.Show(); 
                if (chkExamServiceAdd.Checked && this.ucExamAddition != null)
                {

                    ExamServiceAddADO hisServiceReqExamAdditionSDO = this.examServiceAddProcessor.GetValueV2(this.ucExamAddition) as ExamServiceAddADO;
                    if (hisServiceReqExamAdditionSDO != null)
                    {
                        finishTime = hisServiceReqExamAdditionSDO.FinishTime;
                    }
                }
                else if (chkHospitalize.Checked && this.ucHospitalize != null)
                {
                    HospitalizeExamADO hisDepartmentTranHospitalizeSDO = this.hospitalizeProcessor.GetValue(this.ucHospitalize) as HospitalizeExamADO;
                    if (hisDepartmentTranHospitalizeSDO != null)
                    {
                        finishTime = hisDepartmentTranHospitalizeSDO.FinishTime;
                    }
                }
                else if (chkExamFinish.Checked && this.ucExamFinish != null)
                {
                    ExamFinishADO examFinishADO = this.examFinishProcessor.GetValue(this.ucExamFinish) as ExamFinishADO;
                    if (examFinishADO != null)
                    {
                        finishTime = examFinishADO.FinishTime;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("examFinishADO null:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => examFinishADO), examFinishADO));
                        return;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Finish time is not set");
                }
                if (!finishTime.HasValue)
                {
                    MessageBox.Show("Chưa nhập thời gian kết thúc khám", "Thông báo", MessageBoxButtons.OK);
                    return;
                }
                HisServiceReqView.FINISH_TIME = finishTime;
                var result = new BackendAdapter(param)
                    .Post<HIS_SERVICE_REQ>("api/HisServiceReq/FinishWithTime", ApiConsumers.MosConsumer, HisServiceReqView, param);

                if (result != null)
                {
                    success = true;
                    HisServiceReqView.SERVICE_REQ_STT_ID = result.SERVICE_REQ_STT_ID;
                    SuccessLog(HisServiceReqView);
                }

                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void btnAssignService_Click(object sender, EventArgs e)
        {
            try
            {
                btnAssignService.Focus();
                btnAssignService.Select();

                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecute.btnAssignService_Click.1");
                if (HisConfigCFG.RequiredWeightHeight_Option == "1" && !ValidDhstOption())
                    return;
                ValiTemperatureOption();
                if (!this.ValidForButtonOtherClick())
                {
                    return;
                }

                this.onClickSaveFormAsyncForOtherButtonClick();

                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecute.btnAssignService_Click.2");
                HIS.Desktop.Plugins.Library.AlertHospitalFeeNotBHYT.AlertHospitalFeeNotBHYTManager manager = new Library.AlertHospitalFeeNotBHYT.AlertHospitalFeeNotBHYTManager();
                if (manager.Run(this.HisServiceReqView.TREATMENT_ID, this.treatment.TDL_PATIENT_TYPE_ID ?? 0, this.moduleData.RoomId))
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignService").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignService");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();

                        long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;

                        AssignServiceADO assignServiceADO = new AssignServiceADO(this.HisServiceReqView.TREATMENT_ID, intructionTime, this.HisServiceReqView.ID);
                        assignServiceADO.GenderName = HisServiceReqView.TDL_PATIENT_GENDER_NAME;
                        assignServiceADO.PatientName = HisServiceReqView.TDL_PATIENT_NAME;
                        assignServiceADO.PatientDob = HisServiceReqView.TDL_PATIENT_DOB;
                        assignServiceADO.DgProcessRefeshIcd = RefeshIcd;
                        assignServiceADO.SereServsInTreatment = SereServsCurrentTreatment;
                        assignServiceADO.ProvisionalDiagnosis = this.txtProvisionalDianosis.Text.Trim();
                        assignServiceADO.IsPriority = (HisServiceReqView.PRIORITY.HasValue && HisServiceReqView.PRIORITY == 1);
                        assignServiceADO.IsNotUseBhyt = (HisServiceReqView.IS_NOT_USE_BHYT == 1);
                        Inventec.Common.Logging.LogSystem.Warn("IS_NOT_USE_BHYT: " + HisServiceReqView.IS_NOT_USE_BHYT);
                        Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecute.btnAssignService_Click.3");
                        DHSTADO dhstADO = UcDHSTGetValue() as DHSTADO;
                        AutoMapper.Mapper.CreateMap<DHSTADO, HIS_DHST>();
                        HIS_DHST dhst = AutoMapper.Mapper.Map<DHSTADO, HIS_DHST>(dhstADO);
                        assignServiceADO.Dhst = (dhst != null ? dhst : new HIS_DHST());
                        HIS.UC.Icd.ADO.IcdInputADO icdADO = this.UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                        HIS.UC.Icd.ADO.IcdInputADO icdCauseADO = this.UcIcdCauseGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                        SecondaryIcdDataADO icdSubADO = this.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                        assignServiceADO.IcdExam = new HIS_SERVICE_REQ()
                        {
                            ICD_CODE = icdADO != null ? icdADO.ICD_CODE : "",
                            ICD_NAME = icdADO != null ? icdADO.ICD_NAME : "",
                            ICD_CAUSE_CODE = icdCauseADO != null ? icdCauseADO.ICD_CODE : "",
                            ICD_CAUSE_NAME = icdCauseADO != null ? icdCauseADO.ICD_NAME : "",
                            ICD_SUB_CODE = icdSubADO != null ? icdSubADO.ICD_SUB_CODE : "",
                            ICD_TEXT = icdSubADO != null ? icdSubADO.ICD_TEXT : "",
                        };

                        listArgs.Add(assignServiceADO);

                        if (!IsApplyFormClosingOption(moduleData.ModuleLink))
                        {
                            Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecute.btnAssignService_Click.4");
                            var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);

                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                            ((Form)extenceInstance).ShowDialog();
                        }
                        else
                        {
                            if (lstModuleLinkApply.FirstOrDefault(o => o == moduleData.ModuleLink) != null)
                            {
                                if (GlobalVariables.FormAssignService != null)
                                {
                                    GlobalVariables.FormAssignService.WindowState = FormWindowState.Maximized;
                                    GlobalVariables.FormAssignService.ShowInTaskbar = true;
                                    Type classType = GlobalVariables.FormAssignService.GetType();
                                    MethodInfo methodInfo = classType.GetMethod("ReloadModuleByInputData");
                                    methodInfo.Invoke(GlobalVariables.FormAssignService, new object[] { this.moduleData, assignServiceADO });
                                    GlobalVariables.FormAssignService.Activate();
                                }
                                else
                                {
                                    GlobalVariables.FormAssignService = (Form)PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                                    GlobalVariables.FormAssignService.ShowInTaskbar = true;
                                    if (GlobalVariables.FormAssignService == null) throw new ArgumentNullException("moduleData is null");
                                    GlobalVariables.FormAssignService.Show();

                                    Type classType = GlobalVariables.FormAssignService.GetType();
                                    MethodInfo methodInfo = classType.GetMethod("ChangeIsUseApplyFormClosingOption");
                                    methodInfo.Invoke(GlobalVariables.FormAssignService, new object[] { true });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssignPre_Click(object sender, EventArgs e)
        {
            try
            {
                btnAssignPre_Click_Action(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAssignPre_Click_Action(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecute.btnAssignPre_Click.1");
                if (HisConfigCFG.RequiredWeightHeight_Option == "1" && !ValidDhstOption())
                    return;
                ValiTemperatureOption();
                if (!this.ValidForButtonOtherClick())
                {
                    return;
                }

                this.onClickSaveFormAsyncForOtherButtonClick();

                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecute.btnAssignPre_Click.2");
                Inventec.Common.Logging.LogSystem.Debug("HisPatient/GetPreviousPrescription input: " + treatment.PATIENT_ID);
                List<HIS_SERVICE_REQ> serviceReqDons = new List<HIS_SERVICE_REQ>();
                List<HisPreviousPrescriptionDetailSDO> previousPres = new BackendAdapter(param)
                .Get<List<HisPreviousPrescriptionDetailSDO>>("api/HisPatient/GetPreviousPrescriptionDetail", ApiConsumers.MosConsumer, treatment.PATIENT_ID, param);
                Inventec.Common.Logging.LogSystem.Debug("api/HisPatient/GetPreviousPrescriptionDetail output: " + Inventec.Common.Logging.LogUtil.TraceData("", previousPres));
                if (previousPres != null && previousPres.Count > 0)
                {
                    string note = "";

                    var previousGroups = previousPres.GroupBy(o => new { o.REQUEST_ROOM_NAME, o.SERVICE_REQ_CODE }).Distinct().ToList();

                    foreach (var previousGroup in previousGroups)
                    {
                        string requestRoomNames = "";
                        string treatmentCode = "";
                        string serviceReqCode = "";
                        string userTimeTo = "";
                        List<string> medicines = new List<string>();

                        requestRoomNames = previousGroup.First().REQUEST_ROOM_NAME;
                        treatmentCode = previousGroup.First().TREATMENT_CODE;
                        serviceReqCode = previousGroup.First().SERVICE_REQ_CODE;
                        foreach (var item in previousGroup)
                        {
                            var ExpMedicinesGroup = item.ExpMedicines.GroupBy(o => o.USE_TIME_TO).Distinct().ToList();
                            foreach (var expMedi in ExpMedicinesGroup)
                            {
                                userTimeTo = (expMedi.FirstOrDefault().USE_TIME_TO.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(expMedi.FirstOrDefault().USE_TIME_TO.Value) : "null");

                                medicines.Add(String.Format("Thuốc {0} còn sử dụng tới ngày {1}", String.Join(", ", expMedi.Select(o => o.MEDICINE_TYPE_NAME).ToList()), userTimeTo));
                            }
                        }
                        note += String.Format("Phòng yêu cầu: {0}, HSDT: {1}, Mã YC: {2}:\n {3} ", requestRoomNames, treatmentCode, serviceReqCode, String.Join(";\n", medicines));
                    }
                    DialogResult myResult = MessageBox.Show(String.Format(ResourceMessage.DonThuocLanKhamTruoc, note), ResourceMessage.ThongBao, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                    if (myResult == DialogResult.Cancel)
                        return;
                }

                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = HisServiceReqView.TREATMENT_ID;
                serviceReqFilter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK };
                serviceReqDons = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);

                serviceReqDons = serviceReqDons != null ? serviceReqDons.Where(o => o.PRESCRIPTION_TYPE_ID == PRESCRIPTION_TYPE_ID__THUONG).ToList() : null;

                long isAssignPrescriptionByCFG = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__ASSIGN_PRESCRIPTION_BY_TREATMENT);
                if (isAssignPrescriptionByCFG == 1 && serviceReqDons != null && serviceReqDons.Count == 1)
                {
                    #region Sua don thuoc
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(HisServiceReqView.TREATMENT_ID, 0, 0);
                        if (serviceReqDons != null && serviceReqDons.Count == 1)
                        {
                            var pres = serviceReqDons[0];
                            assignServiceADO.TreatmentCode = pres.TDL_TREATMENT_CODE;
                            assignServiceADO.GenderName = pres.TDL_PATIENT_GENDER_NAME;
                            assignServiceADO.PatientDob = pres.TDL_PATIENT_DOB;
                            assignServiceADO.PatientName = pres.TDL_PATIENT_NAME;
                            assignServiceADO.ProvisionalDiagnosis = txtProvisionalDianosis.Text.Trim();
                            assignServiceADO.PatientId = pres.TDL_PATIENT_ID;
                            AssignPrescriptionEditADO assignEditADO = null;

                            HisExpMestFilter expfilter = new HisExpMestFilter();
                            expfilter.SERVICE_REQ_ID = pres.ID;
                            var expMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                            if (expMests != null && expMests.Count == 1)
                            {
                                var expMest = expMests.FirstOrDefault();
                                assignEditADO = new AssignPrescriptionEditADO(pres, expMest, null);
                            }
                            else
                            {
                                assignEditADO = new AssignPrescriptionEditADO(pres, null, null);
                            }
                            assignServiceADO.AssignPrescriptionEditADO = assignEditADO;

                            DHSTADO dhstADO = UcDHSTGetValue() as DHSTADO;
                            AutoMapper.Mapper.CreateMap<DHSTADO, HIS_DHST>();
                            HIS_DHST dhst = AutoMapper.Mapper.Map<DHSTADO, HIS_DHST>(dhstADO);
                            assignServiceADO.Dhst = (dhst != null ? dhst : new HIS_DHST());

                            assignServiceADO.SereServsInTreatment = SereServsCurrentTreatment;
                            assignServiceADO.DgProcessRefeshIcd = RefeshIcd;
                            assignServiceADO.DgProcessDataResult = RefeshServiceReqInfoAfterFinish;
                            if (HisConfigCFG.IsAutoExitAfterFinish)
                            {
                                assignServiceADO.DlgWhileAutoTreatmentEnd = WhileAutoTreatmentEnd;
                            }
                            listArgs.Add(assignServiceADO);
                            Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecute.btnAssignPre_Click.3");
                            //Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecute.btnAssignPre_Click.4");
                            //var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                            //if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                            //((Form)extenceInstance).ShowDialog();

                            if (!IsApplyFormClosingOption(moduleData.ModuleLink))
                            {
                                Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecute.btnAssignPre_Click.4");
                                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                                ((Form)extenceInstance).ShowDialog();
                            }
                            else
                            {
                                if (lstModuleLinkApply.FirstOrDefault(o => o == moduleData.ModuleLink) != null)
                                {
                                    if (GlobalVariables.FormAssignPrescriptionPK != null)
                                    {
                                        GlobalVariables.FormAssignPrescriptionPK.WindowState = FormWindowState.Maximized;
                                        GlobalVariables.FormAssignPrescriptionPK.ShowInTaskbar = true;
                                        Type classType = GlobalVariables.FormAssignPrescriptionPK.GetType();
                                        MethodInfo methodInfo = classType.GetMethod("ReloadModuleByInputData");
                                        methodInfo.Invoke(GlobalVariables.FormAssignPrescriptionPK, new object[] { assignServiceADO, null, this.moduleData });
                                        GlobalVariables.FormAssignPrescriptionPK.Activate();
                                    }
                                    else
                                    {
                                        GlobalVariables.FormAssignPrescriptionPK = (Form)PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                                        GlobalVariables.FormAssignPrescriptionPK.ShowInTaskbar = true;
                                        if (GlobalVariables.FormAssignPrescriptionPK == null) throw new ArgumentNullException("moduleData is null");
                                        GlobalVariables.FormAssignPrescriptionPK.Show();


                                        Type classType = GlobalVariables.FormAssignPrescriptionPK.GetType();
                                        MethodInfo methodInfo = classType.GetMethod("ChangeIsUseApplyFormClosingOption");
                                        methodInfo.Invoke(GlobalVariables.FormAssignPrescriptionPK, new object[] { true });

                                    }
                                }
                            }

                        }
                    }
                    #endregion
                }
                else
                {
                    #region Ke don thuoc
                    if (serviceReqDons != null && serviceReqDons.Count > 1)
                    {
                        var serviceReqExamChild = serviceReqDons.Where(o => o.PARENT_ID.HasValue && o.PARENT_ID == HisServiceReqView.ID).ToList();
                        if (serviceReqExamChild != null && serviceReqExamChild.Count > 0)
                        {
                            DialogResult myResult = MessageBox.Show(ResourceMessage.DaCoDonThuocBanCoMuonTiepTucKhong, ResourceMessage.ThongBao, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (myResult == DialogResult.Cancel)
                                return;
                        }
                    }

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                        AssignPrescriptionADO assignPrescription = new AssignPrescriptionADO(HisServiceReqView.TREATMENT_ID, intructionTime, this.HisServiceReqView.ID);
                        assignPrescription.TreatmentCode = HisServiceReqView.TDL_TREATMENT_CODE;
                        assignPrescription.TreatmentId = HisServiceReqView.TREATMENT_ID;
                        assignPrescription.HeinCardnumber = HisServiceReqView.TDL_HEIN_CARD_NUMBER;
                        assignPrescription.GenderName = HisServiceReqView.TDL_PATIENT_GENDER_NAME;
                        assignPrescription.PatientName = HisServiceReqView.TDL_PATIENT_NAME;
                        assignPrescription.PatientDob = HisServiceReqView.TDL_PATIENT_DOB;
                        assignPrescription.DgProcessRefeshIcd = RefeshIcd;
                        assignPrescription.DgProcessDataResult = RefeshServiceReqInfoAfterFinish;
                        assignPrescription.PatientId = HisServiceReqView.TDL_PATIENT_ID;
                        if (HisConfigCFG.IsAutoExitAfterFinish)
                        {
                            assignPrescription.DlgWhileAutoTreatmentEnd = WhileAutoTreatmentEnd;
                        }
                        assignPrescription.ProvisionalDiagnosis = txtProvisionalDianosis.Text.Trim();

                        //if (HisConfigCFG.IsloadIcdFromExamServiceExecute)
                        //{
                        HIS.UC.Icd.ADO.IcdInputADO icdADO = this.UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                        HIS.UC.Icd.ADO.IcdInputADO icdCauseADO = this.UcIcdCauseGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                        SecondaryIcdDataADO icdSubADO = this.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                        assignPrescription.IcdExam = new HIS_SERVICE_REQ()
                        {
                            ICD_CODE = icdADO != null ? icdADO.ICD_CODE : "",
                            ICD_NAME = icdADO != null ? icdADO.ICD_NAME : "",
                            ICD_CAUSE_CODE = icdCauseADO != null ? icdCauseADO.ICD_CODE : "",
                            ICD_CAUSE_NAME = icdCauseADO != null ? icdCauseADO.ICD_NAME : "",
                            ICD_SUB_CODE = icdSubADO != null ? icdSubADO.ICD_SUB_CODE : "",
                            ICD_TEXT = icdSubADO != null ? icdSubADO.ICD_TEXT : "",
                        };
                        //}

                        DHSTADO dhstADO = UcDHSTGetValue() as DHSTADO;
                        AutoMapper.Mapper.CreateMap<DHSTADO, HIS_DHST>();
                        HIS_DHST dhst = AutoMapper.Mapper.Map<DHSTADO, HIS_DHST>(dhstADO);
                        assignPrescription.Dhst = (dhst != null ? dhst : new HIS_DHST());

                        assignPrescription.SereServsInTreatment = SereServsCurrentTreatment;

                        listArgs.Add(assignPrescription);
                        //Inventec.Common.Logging.LogSystem.Debug("ExamServiceReqExecute.btnAssignPre_Click.4");
                        //var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                        //if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        //((Form)extenceInstance).ShowDialog();
                        if (!IsApplyFormClosingOption(moduleData.ModuleLink))
                        {
                            var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                            ((Form)extenceInstance).ShowDialog();
                        }
                        else
                        {
                            if (lstModuleLinkApply.FirstOrDefault(o => o == moduleData.ModuleLink) != null)
                            {
                                if (GlobalVariables.FormAssignPrescriptionPK != null)
                                {
                                    GlobalVariables.FormAssignPrescriptionPK.WindowState = FormWindowState.Maximized;
                                    GlobalVariables.FormAssignPrescriptionPK.ShowInTaskbar = true;
                                    Type classType = GlobalVariables.FormAssignPrescriptionPK.GetType();
                                    MethodInfo methodInfo = classType.GetMethod("ReloadModuleByInputData");
                                    methodInfo.Invoke(GlobalVariables.FormAssignPrescriptionPK, new object[] { assignPrescription, null, this.moduleData });
                                    GlobalVariables.FormAssignPrescriptionPK.Activate();
                                }
                                else
                                {
                                    GlobalVariables.FormAssignPrescriptionPK = (Form)PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                                    GlobalVariables.FormAssignPrescriptionPK.ShowInTaskbar = true;

                                    if (GlobalVariables.FormAssignPrescriptionPK == null) throw new ArgumentNullException("moduleData is null");
                                    GlobalVariables.FormAssignPrescriptionPK.Show();

                                    Type classType = GlobalVariables.FormAssignPrescriptionPK.GetType();
                                    MethodInfo methodInfo = classType.GetMethod("ChangeIsUseApplyFormClosingOption");
                                    methodInfo.Invoke(GlobalVariables.FormAssignPrescriptionPK, new object[] { true });

                                }
                            }
                        }

                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAccidentHurt_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
                if (!IsValidForSave)
                    return;
                //if (!SaveExamExecute())
                //    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AccidentHurt").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AccidentHurt");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    long id;
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(HisServiceReqView.TREATMENT_ID);
                    listArgs.Add((HIS.Desktop.Common.DelegateRefeshTreatmentPartialData)refreshClick);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExaminationReqEdit_Click(object sender, EventArgs e)
        {

        }

        private void btnPatientProgram_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
                if (!IsValidForSave)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisPatientProgram").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisPatientProgram");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.HisServiceReqView.TDL_PATIENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAggrExam_Click(object sender, EventArgs e)
        {
            try
            {
                btnAggrExam_Click_Action(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnAggrExam_Click_Action(object sender, EventArgs e)
        {
            try
            {
                if (this.moduleData != null && this.treatment != null)
                {
                    List<object> listObj = new List<object>();
                    V_HIS_TREATMENT_4 treatment4 = new V_HIS_TREATMENT_4();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT_4>(treatment4, this.treatment);
                    listObj.Add(treatment4);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ExpMestAggrExam", this.moduleData.RoomId, this.moduleData.RoomTypeId, listObj);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void drBtnOther_Click(object sender, EventArgs e)
        {
            try
            {
                drBtnOther.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_ExamService_Click(object sender, EventArgs e)
        {
            try
            {
                btnPrint_ExamService.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnKeDonYHCT_Click(object sender, EventArgs e)
        {
            try
            {
                btnKeDonYHCT_Click_Action(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnKeDonYHCT_Click_Action(object sender, EventArgs e)
        {
            try
            {

                btnSave_Click(null, null);
                if (!IsValidForSave)
                    return;
                List<HIS_SERVICE_REQ> serviceReqDons = new List<HIS_SERVICE_REQ>();
                List<HisPreviousPrescriptionSDO> previousPres = new BackendAdapter(param)
                .Get<List<HisPreviousPrescriptionSDO>>("api/HisPatient/GetPreviousPrescription", ApiConsumers.MosConsumer, treatment.PATIENT_ID, param);

                if (previousPres != null && previousPres.Count > 0)
                {
                    string requestRoomNames = "";
                    string treatmentCode = "";
                    string serviceReqCode = "";
                    string userTimeTo = "";
                    string note = "";
                    var previousGroups = previousPres.OrderBy(o => o.USE_TIME_TO).GroupBy(o => new { o.REQUEST_ROOM_NAME }).Distinct().ToList();
                    foreach (var previousGroup in previousGroups)
                    {
                        requestRoomNames = previousGroup.First().REQUEST_ROOM_NAME;
                        treatmentCode = previousGroup.First().TREATMENT_CODE;
                        serviceReqCode = previousGroup.First().SERVICE_REQ_CODE;
                        foreach (var item in previousGroup)
                        {
                            userTimeTo += (item.USE_TIME_TO.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.USE_TIME_TO.Value) + ", " : "null");
                        }
                        note += String.Format("Phòng yêu cầu: {0}, HSDT: {1}, Mã YC: {2} ({3}) ", requestRoomNames, treatmentCode, serviceReqCode, userTimeTo);
                    }
                    DialogResult myResult = MessageBox.Show(String.Format(ResourceMessage.DonThuocLanKhamTruoc, note), ResourceMessage.ThongBao, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                    if (myResult == DialogResult.Cancel)
                        return;
                }

                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = HisServiceReqView.TREATMENT_ID;
                serviceReqFilter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK };
                serviceReqDons = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);

                serviceReqDons = serviceReqDons != null ? serviceReqDons.Where(o => o.PRESCRIPTION_TYPE_ID == PRESCRIPTION_TYPE_ID__YHCT).ToList() : null;

                long isAssignPrescriptionByCFG = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__ASSIGN_PRESCRIPTION_BY_TREATMENT);
                if (isAssignPrescriptionByCFG == 1 && serviceReqDons != null && serviceReqDons.Count == 1)
                {
                    #region Sua don thuoc
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionYHCT").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionYHCT");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(HisServiceReqView.TREATMENT_ID, 0, 0);
                        if (serviceReqDons != null && serviceReqDons.Count == 1)
                        {
                            var pres = serviceReqDons[0];
                            assignServiceADO.TreatmentCode = pres.TDL_TREATMENT_CODE;
                            assignServiceADO.GenderName = pres.TDL_PATIENT_GENDER_NAME;
                            assignServiceADO.PatientDob = pres.TDL_PATIENT_DOB;
                            assignServiceADO.PatientName = pres.TDL_PATIENT_NAME;
                            AssignPrescriptionEditADO assignEditADO = null;

                            HisExpMestFilter expfilter = new HisExpMestFilter();
                            expfilter.SERVICE_REQ_ID = pres.ID;
                            var expMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                            if (expMests != null && expMests.Count == 1)
                            {
                                var expMest = expMests.FirstOrDefault();
                                assignEditADO = new AssignPrescriptionEditADO(pres, expMest, null);
                            }
                            else
                            {
                                assignEditADO = new AssignPrescriptionEditADO(pres, null, null);
                            }
                            assignServiceADO.AssignPrescriptionEditADO = assignEditADO;

                            DHSTADO dhstADO = UcDHSTGetValue() as DHSTADO;
                            AutoMapper.Mapper.CreateMap<DHSTADO, HIS_DHST>();
                            HIS_DHST dhst = AutoMapper.Mapper.Map<DHSTADO, HIS_DHST>(dhstADO);
                            assignServiceADO.Dhst = (dhst != null ? dhst : new HIS_DHST());

                            assignServiceADO.SereServsInTreatment = SereServsCurrentTreatment;
                            assignServiceADO.DgProcessRefeshIcd = RefeshIcd;
                            if (HisConfigCFG.IsAutoExitAfterFinish)
                            {
                                assignServiceADO.DlgWhileAutoTreatmentEnd = WhileAutoTreatmentEnd;
                            }
                            assignServiceADO.DgProcessDataResult = RefeshServiceReqInfoAfterFinish;
                            listArgs.Add(assignServiceADO);
                            var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                            ((Form)extenceInstance).ShowDialog();
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Ke don thuoc
                    if (serviceReqDons != null && serviceReqDons.Count > 1)
                    {
                        var serviceReqExamChild = serviceReqDons.Where(o => o.PARENT_ID.HasValue && o.PARENT_ID == HisServiceReqView.ID).ToList();
                        if (serviceReqExamChild != null && serviceReqExamChild.Count > 0)
                        {
                            DialogResult myResult = MessageBox.Show(ResourceMessage.DaCoDonThuocBanCoMuonTiepTucKhong, ResourceMessage.ThongBao, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (myResult == DialogResult.Cancel)
                                return;
                        }
                    }

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionYHCT").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionYHCT");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                        long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                        AssignPrescriptionADO assignPrescription = new AssignPrescriptionADO(HisServiceReqView.TREATMENT_ID, intructionTime, this.HisServiceReqView.ID);
                        assignPrescription.TreatmentCode = HisServiceReqView.TDL_TREATMENT_CODE;
                        assignPrescription.TreatmentId = HisServiceReqView.TREATMENT_ID;
                        assignPrescription.HeinCardnumber = HisServiceReqView.TDL_HEIN_CARD_NUMBER;
                        assignPrescription.GenderName = HisServiceReqView.TDL_PATIENT_GENDER_NAME;
                        assignPrescription.PatientName = HisServiceReqView.TDL_PATIENT_NAME;
                        assignPrescription.PatientDob = HisServiceReqView.TDL_PATIENT_DOB;
                        assignPrescription.DgProcessRefeshIcd = RefeshIcd;
                        if (HisConfigCFG.IsAutoExitAfterFinish)
                        {
                            assignPrescription.DlgWhileAutoTreatmentEnd = WhileAutoTreatmentEnd;
                        }
                        assignPrescription.DgProcessDataResult = RefeshServiceReqInfoAfterFinish;

                        //if (HisConfigCFG.IsloadIcdFromExamServiceExecute)
                        //{
                        HIS.UC.Icd.ADO.IcdInputADO icdADO = this.UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                        HIS.UC.Icd.ADO.IcdInputADO icdCauseADO = this.UcIcdCauseGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                        SecondaryIcdDataADO icdSubADO = this.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                        assignPrescription.IcdExam = new HIS_SERVICE_REQ()
                        {
                            ICD_CODE = icdADO != null ? icdADO.ICD_CODE : "",
                            ICD_NAME = icdADO != null ? icdADO.ICD_NAME : "",
                            ICD_CAUSE_CODE = icdCauseADO != null ? icdCauseADO.ICD_CODE : "",
                            ICD_CAUSE_NAME = icdCauseADO != null ? icdCauseADO.ICD_NAME : "",
                            ICD_SUB_CODE = icdSubADO != null ? icdSubADO.ICD_SUB_CODE : "",
                            ICD_TEXT = icdSubADO != null ? icdSubADO.ICD_TEXT : "",
                        };
                        //}

                        DHSTADO dhstADO = UcDHSTGetValue() as DHSTADO;
                        AutoMapper.Mapper.CreateMap<DHSTADO, HIS_DHST>();
                        HIS_DHST dhst = AutoMapper.Mapper.Map<DHSTADO, HIS_DHST>(dhstADO);
                        assignPrescription.Dhst = (dhst != null ? dhst : new HIS_DHST());

                        assignPrescription.SereServsInTreatment = SereServsCurrentTreatment;

                        listArgs.Add(assignPrescription);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                    }

                    #endregion
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_History_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (TreatmentExamADO)gridViewTreatmentHistory.GetFocusedRow();
                if (this.moduleData != null && row != null)
                {
                    TreatmentHistoryADO ado = new TreatmentHistoryADO();
                    ado.treatment_code = row.TREATMENT_CODE;
                    ado.treatmentId = row.ID;
                    ado.patient_code = row.TDL_PATIENT_CODE;
                    ado.patientId = row.PATIENT_ID;

                    List<object> listObj = new List<object>();
                    listObj.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TreatmentHistory", this.moduleData.RoomId, this.moduleData.RoomTypeId, listObj);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_Bordereau_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (TreatmentExamADO)gridViewTreatmentHistory.GetFocusedRow();
                if (this.moduleData != null && row != null)
                {
                    List<object> listObj = new List<object>();
                    listObj.Add(row.ID);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.Bordereau", this.moduleData.RoomId, this.moduleData.RoomTypeId, listObj);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRoomTran_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ChangeExamRoomProcess").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ChangeExamRoomProcess");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(this.HisServiceReqView);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnServiceReqList_Click(object sender, EventArgs e)
        {
            try
            {
                btnServiceReqList_Click_Action(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnServiceReqList_Click_Action(object sender, EventArgs e)
        {
            try
            {

                btnSave_Click(null, null);
                if (!IsValidForSave)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ServiceReqList");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    treatment.ID = this.HisServiceReqView.TREATMENT_ID;
                    listArgs.Add(treatment);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTuTruc_Click(object sender, EventArgs e)
        {
            try
            {
                btnTuTruc_Click_Action(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTuTruc_Click_Action(object sender, EventArgs e)
        {
            try
            {
                if (HisConfigCFG.RequiredWeightHeight_Option == "1" && !ValidDhstOption())
                    return;
                ValiTemperatureOption();
                if (!this.ValidForButtonOtherClick()) return;
                btnSave_Click(null, null);

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                    AssignPrescriptionADO assignPrescription = new AssignPrescriptionADO(HisServiceReqView.TREATMENT_ID, intructionTime, this.HisServiceReqView.ID);
                    assignPrescription.TreatmentCode = HisServiceReqView.TDL_TREATMENT_CODE;
                    assignPrescription.HeinCardnumber = HisServiceReqView.TDL_HEIN_CARD_NUMBER;
                    assignPrescription.TreatmentId = HisServiceReqView.TREATMENT_ID;
                    assignPrescription.GenderName = HisServiceReqView.TDL_PATIENT_GENDER_NAME;
                    assignPrescription.PatientName = HisServiceReqView.TDL_PATIENT_NAME;
                    assignPrescription.PatientDob = HisServiceReqView.TDL_PATIENT_DOB;
                    assignPrescription.DgProcessRefeshIcd = RefeshIcd;
                    assignPrescription.DgProcessDataResult = RefeshServiceReqInfoAfterFinish;
                    assignPrescription.ProvisionalDiagnosis = txtProvisionalDianosis.Text.Trim();
                    assignPrescription.IsCabinet = true;

                    //if (HisConfigCFG.IsloadIcdFromExamServiceExecute)
                    //{
                    HIS.UC.Icd.ADO.IcdInputADO icdADO = this.UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                    HIS.UC.Icd.ADO.IcdInputADO icdCauseADO = this.UcIcdCauseGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                    SecondaryIcdDataADO icdSubADO = this.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                    assignPrescription.IcdExam = new HIS_SERVICE_REQ()
                    {
                        ICD_CODE = icdADO != null ? icdADO.ICD_CODE : "",
                        ICD_NAME = icdADO != null ? icdADO.ICD_NAME : "",
                        ICD_CAUSE_CODE = icdCauseADO != null ? icdCauseADO.ICD_CODE : "",
                        ICD_CAUSE_NAME = icdCauseADO != null ? icdCauseADO.ICD_NAME : "",
                        ICD_SUB_CODE = icdSubADO != null ? icdSubADO.ICD_SUB_CODE : "",
                        ICD_TEXT = icdSubADO != null ? icdSubADO.ICD_TEXT : "",
                    };
                    listArgs.Add(assignPrescription);
                    //var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    //if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    //((Form)extenceInstance).ShowDialog();
                    if (!IsApplyFormClosingOption(moduleData.ModuleLink))
                    {
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        if (lstModuleLinkApply.FirstOrDefault(o => o == moduleData.ModuleLink) != null)
                        {
                            if (GlobalVariables.FormAssignPrescriptionPK != null)
                            {
                                GlobalVariables.FormAssignPrescriptionPK.WindowState = FormWindowState.Maximized;
                                GlobalVariables.FormAssignPrescriptionPK.ShowInTaskbar = true;
                                Type classType = GlobalVariables.FormAssignPrescriptionPK.GetType();
                                MethodInfo methodInfo = classType.GetMethod("ReloadModuleByInputData");
                                methodInfo.Invoke(GlobalVariables.FormAssignPrescriptionPK, new object[] { assignPrescription, null, this.moduleData });
                                GlobalVariables.FormAssignPrescriptionPK.Activate();
                            }
                            else
                            {
                                GlobalVariables.FormAssignPrescriptionPK = (Form)PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                                GlobalVariables.FormAssignPrescriptionPK.ShowInTaskbar = true;
                                if (GlobalVariables.FormAssignPrescriptionPK == null) throw new ArgumentNullException("moduleData is null");
                                GlobalVariables.FormAssignPrescriptionPK.Show();


                                Type classType = GlobalVariables.FormAssignPrescriptionPK.GetType();
                                MethodInfo methodInfo = classType.GetMethod("ChangeIsUseApplyFormClosingOption");
                                methodInfo.Invoke(GlobalVariables.FormAssignPrescriptionPK, new object[] { true });

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTrackingList_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
                if (!IsValidForSave)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTrackingList").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisTrackingList");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    DHSTADO dhstADO = UcDHSTGetValue() as DHSTADO;
                    AutoMapper.Mapper.CreateMap<DHSTADO, HIS_DHST>();
                    HIS_DHST dhst = AutoMapper.Mapper.Map<DHSTADO, HIS_DHST>(dhstADO);
                    listArgs.Add(HisServiceReqView.TREATMENT_ID);
                    listArgs.Add(dhst);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTrackingCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (HisConfigCFG.RequiredWeightHeight_Option == "1" && !ValidDhstOption())
                    return;
                ValiTemperatureOption();
                btnSave_Click(null, null);
                if (!IsValidForSave)
                    return;
                if (this.dhst == null)
                {
                    LoadDHST();
                }
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TrackingCreate").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TrackingCreate");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(HisServiceReqView.TREATMENT_ID);
                    if (this.dhst != null)
                    {
                        listArgs.Add(this.dhst);
                    }
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();

                    //Load lại tracking
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDichVuHenKham_Click(object sender, EventArgs e)
        {
            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AppointmentService").FirstOrDefault();
            if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AppointmentService");
            if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
            {
                List<object> listArgs = new List<object>();
                Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                listArgs.Add(treatment.ID);
                var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                ((Form)extenceInstance).Show();
            }
        }

        private void btnMedisoftHistory_Click(object sender, EventArgs e)
        {
            try
            {
                btnMedisoftHistory_Click_Action(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnMedisoftHistory_Click_Action(object sender, EventArgs e)
        {
            try
            {
                if (HisServiceReqView != null)
                {
                    InitDataADO ado = new InitDataADO();
                    ado.ProviderType = ProviderType.Medisoft;
                    ado.PatientId = this.HisServiceReqView.TDL_PATIENT_ID;

                    OtherTreatmentHistoryProcessor history = new OtherTreatmentHistoryProcessor(ado);
                    if (history != null)
                    {
                        history.Run(Library.OtherTreatmentHistory.Enum.Xemthuocpk);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnVoBenhAn_Click(object sender, EventArgs e)
        {
            VoBenhAn();
        }

        private void VoBenhAn()
        {
            try
            {
                if (barManager1 == null)
                {
                    barManager1 = new BarManager();
                    barManager1.Form = this;
                }
                this._BarManager = barManager1;
                if (this._BarManager == null)
                {
                    return;
                }
                if (this._Menu == null)
                    this._Menu = new PopupMenu(this._BarManager);
                this._Menu.ItemLinks.Clear();
                if (this.emrMenuPopupProcessor == null) this.emrMenuPopupProcessor = new Library.FormMedicalRecord.MediRecordMenuPopupProcessor();

                HIS.Desktop.Plugins.Library.FormMedicalRecord.Base.EmrInputADO emrInputAdo = new Library.FormMedicalRecord.Base.EmrInputADO();
                emrInputAdo.TreatmentId = this.treatment.ID;
                emrInputAdo.PatientId = this.treatment.PATIENT_ID;
                if (LstEmrCoverConfig != null && LstEmrCoverConfig.Count > 0)
                {
                    if (LstEmrCoverConfig.Count == 1)
                    {
                        emrInputAdo.EmrCoverTypeId = LstEmrCoverConfig.FirstOrDefault().EMR_COVER_TYPE_ID;
                    }
                    else
                    {
                        emrInputAdo.lstEmrCoverTypeId = new List<long>();
                        emrInputAdo.lstEmrCoverTypeId = LstEmrCoverConfig.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                    }
                }
                else
                {
                    if (LstEmrCoverConfigDepartment != null && LstEmrCoverConfigDepartment.Count > 0)
                    {
                        if (LstEmrCoverConfigDepartment.Count == 1)
                        {
                            emrInputAdo.EmrCoverTypeId = LstEmrCoverConfigDepartment.FirstOrDefault().EMR_COVER_TYPE_ID;
                        }
                        else
                        {
                            emrInputAdo.lstEmrCoverTypeId = new List<long>();
                            emrInputAdo.lstEmrCoverTypeId = LstEmrCoverConfigDepartment.Select(o => o.EMR_COVER_TYPE_ID).ToList();
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Info("emrInputAdo: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => emrInputAdo), emrInputAdo));

                this.emrMenuPopupProcessor.InitMenuButton(this._Menu, this._BarManager, emrInputAdo);
                this._Menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChooseIcdText_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, checkIcdManager);
                WaitingManager.Hide();
                FormSecondaryIcd.ShowDialog();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnContentSubclinicalEdit_Click(object sender, EventArgs e)
        {
            try
            {

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ContentSubclinical").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ContentSubclinical");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.HisServiceReqView.TREATMENT_ID);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)DelegateSelectDataContentSubclinical);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnConnectBloodPressure_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_DHST data = HIS.Desktop.Plugins.Library.ConnectBloodPressure.ConnectBloodPressureProcessor.GetData();
                if (data != null)
                {
                    if (data.EXECUTE_TIME != null)
                        dtExecuteTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXECUTE_TIME ?? 0) ?? DateTime.Now;
                    else
                        dtExecuteTime.EditValue = DateTime.Now;

                    if (data.BLOOD_PRESSURE_MAX.HasValue)
                    {
                        spinBloodPressureMax.EditValue = data.BLOOD_PRESSURE_MAX;
                    }
                    if (data.BLOOD_PRESSURE_MIN.HasValue)
                    {
                        spinBloodPressureMin.EditValue = data.BLOOD_PRESSURE_MIN;
                    }
                    if (data.BREATH_RATE.HasValue)
                    {
                        spinBreathRate.EditValue = data.BREATH_RATE;
                    }
                    if (data.HEIGHT.HasValue)
                    {
                        spinHeight.EditValue = data.HEIGHT;
                    }
                    if (data.CHEST.HasValue)
                    {
                        spinChest.EditValue = data.CHEST;
                    }
                    if (data.BELLY.HasValue)
                    {
                        spinBelly.EditValue = data.BELLY;
                    }
                    if (data.PULSE.HasValue)
                    {
                        spinPulse.EditValue = data.PULSE;
                    }
                    if (data.TEMPERATURE.HasValue)
                    {
                        spinTemperature.EditValue = data.TEMPERATURE;
                    }
                    if (data.WEIGHT.HasValue)
                    {
                        spinWeight.EditValue = data.WEIGHT;
                    }
                    if (!String.IsNullOrWhiteSpace(data.NOTE))
                    {
                        txtNote.Text = data.NOTE;
                    }
                    if (data.SPO2.HasValue)
                        spinSPO2.Value = (data.SPO2.Value * 100);
                    else
                        spinSPO2.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region UcDHST
        private void spinPulse_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBloodPressureMax.Focus();
                    spinBloodPressureMax.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBelly_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void spinBloodPressureMin_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinWeight.Focus();
                    spinWeight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBloodPressureMax_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBloodPressureMin.Focus();
                    spinBloodPressureMin.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBreathRate_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void spinWeight_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinHeight.Focus();
                    spinHeight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinHeight_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinChest_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void spinTemperature_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinHeight_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DHSTFillDataToBmiAndLeatherArea();
                LoadMLCT();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinWeight_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DHSTFillDataToBmiAndLeatherArea();
                LoadMLCT();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtExecuteTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinPulse.Focus();
                    spinPulse.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spSPO2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinTemperature.Focus();
                    spinTemperature.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNote_Leave(object sender, EventArgs e)
        {

        }
        #endregion

        #region MLCT

        /// <summary>
        /// - Lúc mở chức năng hoặc có sự thay đổi về cân nặng (nhập trên màn hình xử lý khám) thì thực hiện:
        ///+ Nếu không có thông tin cân nặng => xét MLCT = rỗng.
        ///+ Kiểm tra danh mục chỉ số xét nghiệm (HIS_TEST_INDEX) nếu không có chỉ số nào có trường IS_TO_CALCULATE_EGFR = 1 => xét MLCT = rỗng.
        ///+ Nếu có HIS_TEST_INDEX IS_TO_CALCULATE_EGFR = 1 thì lấy các chỉ số xét nghiệm có IS_TO_CALCULATE_EGFR = 1 của hồ sơ đấy (HIS_SERE_SERV_TEIN theo tdl_treatment_id và test_index_id) và có kết quả (VALUE khác null và lấy chỉ số có modify_time lớn nhất nếu trùng thì order tiếp theo ID lớn nhất).
        ///+ Lấy được chỉ số thì convert value về number (decimal) nếu convert được thì tính không convert được thì set rỗng.
        ///+ Nếu 2 (cân nặng, chỉ số) thông tin trên đều có thì thực hiện tính mức lọc cầu thận theo công thức:
        ///++ Nam: [(140-tuổi)x Cân nặng x 1,23 x 1,73]/(Chỉ số x Diện tích da) (cân nặng người dùng nhập ở màn hình xử lý khám).
        ///++ Nữ: [(140-tuổi)x Cân nặng x 1,04 x 1,73]/(Chỉ số x Diện tích da) (cân nặng người dùng nhập ở màn hình xử lý khám).
        /// </summary>
        private void LoadMLCT()
        {
            try
            {
                string strIsToCalculateEgfr = "";
                var TestIndexData = BackendDataWorker.Get<HIS_TEST_INDEX>().Where(o => o.IS_TO_CALCULATE_EGFR == 1).ToList();

                if (TestIndexData != null && TestIndexData.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    HisSereServTeinFilter filter = new HisSereServTeinFilter();
                    filter.TDL_TREATMENT_ID = treatmentId;
                    filter.TEST_INDEX_IDs = TestIndexData.Select(o => o.ID).ToList();
                    var SereServTeinData = new BackendAdapter(param).Get<List<HIS_SERE_SERV_TEIN>>("/api/HisSereServTein/Get", ApiConsumers.MosConsumer, filter, param);
                    if (SereServTeinData != null && SereServTeinData.Count > 0)
                    {
                        var DataSereServTein = SereServTeinData.Where(o => !String.IsNullOrEmpty(o.VALUE)).OrderByDescending(o => o.MODIFY_TIME).ThenByDescending(o => o.ID).FirstOrDefault();
                        var testIndex = TestIndexData.FirstOrDefault(o => o.ID == (DataSereServTein.TEST_INDEX_ID ?? 0));
                        if (testIndex != null)
                        {
                            decimal chiso;
                            string ssTeinVL = DataSereServTein.VALUE.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                             .Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ssTeinVL), ssTeinVL));
                            if (Decimal.TryParse(ssTeinVL, out chiso))
                            {
                                if (chiso > 0)
                                {
                                    if (testIndex.CONVERT_RATIO_MLCT.HasValue)
                                        chiso *= (testIndex.CONVERT_RATIO_MLCT ?? 0);
                                    decimal mlct = Inventec.Common.Calculate.Calculation.MucLocCauThan(this.HisServiceReqView.TDL_PATIENT_DOB, (decimal)spinWeight.Value, (decimal)spinHeight.Value, chiso, this.HisServiceReqView.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE);
                                    strIsToCalculateEgfr = mlct != 0 ? mlct.ToString() : "";
                                }
                            }
                        }
                        else
                        {
                            strIsToCalculateEgfr = "";
                        }
                    }
                    else
                    {
                        strIsToCalculateEgfr = "";
                    }
                }
                else
                {
                    strIsToCalculateEgfr = "";
                }

                lblIsToCalculateEgfr.Text = strIsToCalculateEgfr;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strIsToCalculateEgfr), strIsToCalculateEgfr));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region UCNextTreatmentInstruction

        private void txtNextTreatmentInstructionCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadNextTreatmentInstructionCombo(txtNextTreatmentInstructionCode.Text.ToUpper());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void LoadNextTreatmentInstructionCombo(string searchCode)
        {
            try
            {
                bool showCbo = true;
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var listData = this.dataNextTreatmentInstructions.Where(o => o.NEXT_TREA_INTR_CODE.Contains(searchCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.NEXT_TREA_INTR_CODE == searchCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        txtNextTreatmentInstructionCode.Text = result.First().NEXT_TREA_INTR_CODE;
                        txtNextTreatmentInstructionMainText.Text = result.First().NEXT_TREA_INTR_NAME;
                        cboNextTreatmentInstructions.EditValue = listData.First().ID;
                        chkEditNextTreatmentInstruction.Checked = this.AutoCheckNextTreatmentInstruction;

                        if (chkEditNextTreatmentInstruction.Checked)
                        {
                            txtNextTreatmentInstructionMainText.Focus();
                            txtNextTreatmentInstructionMainText.SelectAll();
                        }
                        else
                        {
                            cboNextTreatmentInstructions.Focus();
                            cboNextTreatmentInstructions.SelectAll();
                        }
                    }
                }

                if (showCbo)
                {
                    cboNextTreatmentInstructions.Focus();
                    cboNextTreatmentInstructions.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNextTreatmentInstructionMainText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    chkEditNextTreatmentInstruction.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNextTreatmentInstructions_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboNextTreatmentInstructions.EditValue != null)
                        this.ChangecboNextTreatmentInstruction();
                    else if (this.IsAcceptWordNotInData && this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextNextTreatmentInstructionName))
                        this.ChangecboNextTreatmentInstruction_V2_GanICDNAME(this._TextNextTreatmentInstructionName);
                    else
                        SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboNextTreatmentInstruction()
        {
            try
            {
                cboNextTreatmentInstructions.Properties.Buttons[1].Visible = true;
                MOS.EFMODEL.DataModels.HIS_NEXT_TREA_INTR nextTreatmentIntruction = dataNextTreatmentInstructions.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboNextTreatmentInstructions.EditValue ?? 0).ToString()));
                if (nextTreatmentIntruction != null)
                {
                    txtNextTreatmentInstructionCode.Text = nextTreatmentIntruction.NEXT_TREA_INTR_CODE;
                    txtNextTreatmentInstructionMainText.Text = nextTreatmentIntruction.NEXT_TREA_INTR_NAME;
                    chkEditNextTreatmentInstruction.Checked = this.AutoCheckNextTreatmentInstruction;
                    if (chkEditNextTreatmentInstruction.Checked)
                    {
                        this.UcNextTreatmentInstNextForcus();
                    }
                    else
                    {
                        chkEditNextTreatmentInstruction.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboNextTreatmentInstruction_V2_GanICDNAME(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return;
                this.chkEditNextTreatmentInstruction.Enabled = true;
                this.chkEditNextTreatmentInstruction.Checked = true;
                this.txtNextTreatmentInstructionMainText.Text = text;
                this.txtNextTreatmentInstructionMainText.Focus();
                this.txtNextTreatmentInstructionMainText.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNextTreatmentInstructions_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboNextTreatmentInstructions.ClosePopup();
                    cboNextTreatmentInstructions.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboNextTreatmentInstructions.ClosePopup();
                    if (cboNextTreatmentInstructions.EditValue != null)
                        this.ChangecboNextTreatmentInstruction();
                }
                else
                    cboNextTreatmentInstructions.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditNextTreatmentInstruction_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkEditNextTreatmentInstruction.Checked == true)
                {
                    cboNextTreatmentInstructions.Visible = false;
                    txtNextTreatmentInstructionMainText.Visible = true;
                    if (this.IsObligatoryTranferMediOrg)
                        txtNextTreatmentInstructionMainText.Text = this._TextNextTreatmentInstructionName;
                    else
                        txtNextTreatmentInstructionMainText.Text = cboNextTreatmentInstructions.Text.Trim();
                    txtNextTreatmentInstructionMainText.Focus();
                    txtNextTreatmentInstructionMainText.SelectAll();
                }
                else if (chkEditNextTreatmentInstruction.Checked == false)
                {
                    txtNextTreatmentInstructionMainText.Visible = false;
                    cboNextTreatmentInstructions.Visible = true;
                    txtNextTreatmentInstructionMainText.Text = cboNextTreatmentInstructions.Text.Trim();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditNextTreatmentInstruction_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtNextTreatmentInstructionMainText.Text != null)
                    {
                        //this.data.DelegateRefeshNextTreatmentInstructionMainText(txtNextTreatmentInstructionMainText.Text.Trim());
                    }
                    if (cboNextTreatmentInstructions.EditValue != null)
                    {
                        //var hisNextTreatmentInstruction = BackendDataWorker.Get<HIS_NEXT_TREA_INTR>().Where(p => p.ID == (long)cboNextTreatmentInstructions.EditValue).FirstOrDefault();
                        //this.data.DelegateRefeshNextTreatmentInstruction(hisNextTreatmentInstruction);
                    }
                    this.UcNextTreatmentInstNextForcus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNextTreatmentInstructionCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = Resources.ResourceMessage.NextTreatmentInstructionKhongDung;
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNextTreatmentInstructionCode_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text.Trim();
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = dataNextTreatmentInstructions.Where(o => o.NEXT_TREA_INTR_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.NEXT_TREA_INTR_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtNextTreatmentInstructionCode.ErrorText = "";
                        dxValidationProviderForLeftPanel.RemoveControlError(txtNextTreatmentInstructionCode);
                        ValidationNextTreatmentInst(10, 500, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboNextTreatmentInstructions_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboNextTreatmentInstructions.Properties.Buttons[1].Visible)
                        return;
                    this._TextNextTreatmentInstructionName = "";
                    cboNextTreatmentInstructions.EditValue = null;
                    txtNextTreatmentInstructionCode.Text = "";
                    txtNextTreatmentInstructionMainText.Text = "";
                    cboNextTreatmentInstructions.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNextTreatmentInstructions_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboNextTreatmentInstructions.Text.Trim()))
                {
                    cboNextTreatmentInstructions.EditValue = null;
                    txtNextTreatmentInstructionMainText.Text = "";
                    chkEditNextTreatmentInstruction.Checked = false;
                }
                else
                {
                    this._TextNextTreatmentInstructionName = cboNextTreatmentInstructions.Text.Trim();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region UCSecondaryIcd
        internal bool ShowPopupIcdChoose()
        {
            try
            {
                frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, checkIcdManager);
                FormSecondaryIcd.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
            return true;
        }

        private void SetFocusGrid()
        {
            try
            {
                gvIcdSubCode.Focus();
                gvIcdSubCode.FocusedRowHandle = 0;
                this.IcdSubChoose = this.gvIcdSubCode.GetFocusedRow() as HIS_ICD;
                if (this.IcdSubChoose != null)
                {
                    popupControlContainer1.HidePopup();
                    gridViewIcdCode.ActiveFilter.Clear();
                    LoadSubIcd(this.IcdSubChoose.ICD_CODE);
                    txtIcdSubCode.Focus();
                    txtIcdSubCode.Select(txtIcdSubCode.Text.Length, txtIcdSubCode.Text.Length);
                    popupControlContainer1.HidePopup();
                    isShowSubIcd = false;
                }
                else
                {
                    txtIcdText.Focus();
                    txtIcdText.SelectionStart = txtIcdText.Text.Length;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void txtIcdSubCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (isShowSubIcd)
                    {
                        SetFocusGrid();
                    }
                    else
                    {

                        isShowGridIcdSub = false;
                        popupControlContainer1.HidePopup();
                        if (!ProccessorByIcdCode((sender as DevExpress.XtraEditors.TextEdit).Text.Trim()))
                        {
                            e.Handled = true;
                            return;
                        }
                        else
                        {
                            isShowGridIcdSub = false;
                        }
                        ReloadIcdSubContainerByCodeChanged();
                        txtIcdText.Focus();
                        txtIcdText.SelectionStart = txtIcdText.Text.Length;

                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gvIcdSubCode.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(panelControlCauseIcd.Bounds.X, panelControlCauseIcd.Bounds.Y, panelControlCauseIcd.Bounds.Width, panelControlCauseIcd.Bounds.Height);
                    popupControlContainer1.ShowPopup(new Point(buttonBounds.X + 110, buttonBounds.Bottom - 50));
                    gvIcdSubCode.Focus();
                    gvIcdSubCode.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.KeyCode != null)
                {
                    isShowSubIcd = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdSubCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, checkIcdManager);
                    FormSecondaryIcd.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isNotProcessWhileChangedTextSubIcd)
                {
                    return;
                }
                if (!String.IsNullOrEmpty(txtIcdText.Text.Trim()))
                {
                    string strIcdSubText = "";

                    //txtIcdText.Refresh();
                    if (txtIcdText.Text.LastIndexOf(";") > -1)
                    {
                        strIcdSubText = txtIcdText.Text.Substring(txtIcdText.Text.LastIndexOf(";")).Replace(";", "");
                    }
                    else
                        strIcdSubText = txtIcdText.Text.Trim();
                    if (isShowContainerMediMatyForChoose)
                    {
                        customGridViewSubIcdName.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerMediMaty)
                        {
                            isShowContainerMediMaty = true;
                        }

                        //Filter data
                        customGridViewSubIcdName.ActiveFilterString = String.Format("[ICD_NAME_UNSIGN] Like '%{0}%'", Inventec.Common.String.Convert.UnSignVNese(strIcdSubText).ToLower());
                        customGridViewSubIcdName.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        customGridViewSubIcdName.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        customGridViewSubIcdName.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        customGridViewSubIcdName.FocusedRowHandle = 0;
                        customGridViewSubIcdName.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        customGridViewSubIcdName.OptionsFind.HighlightFindResults = true;

                        if (isShow && isShowGridIcdSub)
                        {
                            ShowPopupContainerIcsSub();
                            isShow = false;
                            isShowGridIcdSub = false;
                        }

                        txtIcdText.Focus();
                    }
                    isShowContainerMediMatyForChoose = false;
                }
                else
                {
                    customGridViewSubIcdName.ActiveFilter.Clear();
                    this.subIcdPopupSelect = null;
                    if (!isShowContainerMediMaty)
                    {
                        popupControlContainerSubIcdName.HidePopup();
                    }
                    else
                    {
                        customGridViewSubIcdName.FocusedRowHandle = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //if (e.KeyChar == ((char)System.Windows.Forms.Keys.ControlKey | (char)System.Windows.Forms.Keys.A))
                //{
                //    txtIcdText.Focus();
                //    txtIcdText.SelectAll();
                //}
                //else if (e.KeyChar == ((char)System.Windows.Forms.Keys.Delete) || e.KeyChar == ((char)System.Windows.Forms.Keys.Back))
                //{
                //    this.isNotProcessWhileChangedTextSubIcd = true;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!isShowGridIcdSub)
                    {
                        //if (!isShow)
                        //{
                        //    this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as HIS.Desktop.Plugins.ExamServiceReqExecute.ADO.IcdADO;
                        //    if (this.customGridViewSubIcdName.RowCount == 1 && this.subIcdPopupSelect != null && !this.subIcdPopupSelect.IsChecked)
                        //    {
                        //        this.subIcdPopupSelect.IsChecked = true;
                        //        this.customGridControlSubIcdName.RefreshDataSource();
                        //    }
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerSubIcdName.HidePopup();

                        //    SetCheckedSubIcdsToControl();

                        //    txtIcdText.Focus();
                        //    txtIcdText.SelectionStart = txtIcdText.Text.Length;
                        //}
                        //else
                        //{
                        //    isShowContainerMediMaty = false;
                        //    isShowContainerMediMatyForChoose = true;
                        //    popupControlContainerSubIcdName.HidePopup();
                        //}


                        txtIcdCodeCause.Focus();
                        txtIcdCodeCause.SelectAll();

                    }
                    else
                    {
                        this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as HIS.Desktop.Plugins.ExamServiceReqExecute.ADO.IcdADO;
                        if (this.subIcdPopupSelect != null)
                        {
                            isShowContainerMediMaty = false;
                            isShowContainerMediMatyForChoose = true;

                            this.subIcdPopupSelect.IsChecked = !this.subIcdPopupSelect.IsChecked;
                            this.customGridControlSubIcdName.RefreshDataSource();

                            SetCheckedSubIcdsToControl();
                            popupControlContainerSubIcdName.HidePopup();
                            isShowGridIcdSub = false;
                            txtIcdText.Focus();
                            txtIcdText.SelectionStart = txtIcdText.Text.Length;
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    //int countInGridRows = customGridViewSubIcdName.RowCount;
                    //if (countInGridRows > 1)
                    //{
                    //    rowHandlerNext = 1;
                    //}

                    ShowPopupContainerIcsSub();
                    customGridViewSubIcdName.Focus();
                    customGridViewSubIcdName.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.KeyCode == Keys.Shift || e.KeyCode == Keys.ShiftKey)
                {
                    customGridViewSubIcdName.ActiveFilter.Clear();
                    ShowPopupContainerIcsSub();
                    txtIcdText.Focus();
                    txtIcdText.SelectionStart = txtIcdText.Text.Length;
                    e.Handled = true;
                }
                else if (e.KeyCode != null)
                {
                    isShowGridIcdSub = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdText_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    frmSecondaryIcd FormSecondaryIcd = new frmSecondaryIcd(stringIcds, this.txtIcdSubCode.Text, this.txtIcdText.Text, (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize, checkIcdManager);
                    FormSecondaryIcd.ShowDialog();
                }
                //else if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                //{
                //    this.isNotProcessWhileChangedTextSubIcd = false;
                //}
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadIcdSubContainerByCodeChanged()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ReloadIcdSubContainerByCodeChanged.1");
                string[] codes = this.txtIcdSubCode.Text.Split(IcdUtil.seperator.ToCharArray());
                this.icdSubcodeAdoChecks = (from m in this.currentIcds select new ADO.IcdADO(m, codes)).ToList();
                customGridControlSubIcdName.DataSource = null;
                customGridControlSubIcdName.DataSource = this.icdSubcodeAdoChecks;
                Inventec.Common.Logging.LogSystem.Debug("ReloadIcdSubContainerByCodeChanged.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void stringIcds(string icdCode, string icdName)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("stringIcds.1");
                this.isNotProcessWhileChangedTextSubIcd = true;
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        Inventec.Common.Logging.LogSystem.Debug("stringIcds.2");
                        if (!string.IsNullOrEmpty(icdCode))
                        {
                            txtIcdSubCode.Text = icdCode;
                        }
                        else
                        {
                            txtIcdSubCode.Text = "";
                        }
                        if (!string.IsNullOrEmpty(icdName))
                        {
                            txtIcdText.Text = icdName;
                        }
                        else
                        {
                            txtIcdText.Text = "";
                        }
                        ReloadIcdSubContainerByCodeChanged();
                        Inventec.Common.Logging.LogSystem.Debug("stringIcds.3");
                    }));
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("stringIcds.4");
                    if (!string.IsNullOrEmpty(icdCode))
                    {
                        txtIcdSubCode.Text = icdCode;
                    }
                    else
                    {
                        txtIcdSubCode.Text = "";
                    }
                    if (!string.IsNullOrEmpty(icdName))
                    {
                        txtIcdText.Text = icdName;
                    }
                    else
                    {
                        txtIcdText.Text = "";
                    }
                    ReloadIcdSubContainerByCodeChanged();
                    Inventec.Common.Logging.LogSystem.Debug("stringIcds.5");
                }
                this.isNotProcessWhileChangedTextSubIcd = false;
                Inventec.Common.Logging.LogSystem.Debug("stringIcds.6");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCheckedIcdsToControl(string icdCodes, string icdNames)
        {
            try
            {
                string icdName__Olds = (txtIcdText.Text == txtIcdText.Properties.NullValuePrompt ? "" : txtIcdText.Text.Trim());
                txtIcdText.Text = ProcessIcdNameChanged(icdName__Olds, icdNames);
                if (icdNames.Equals(IcdUtil.seperator))
                {
                    txtIcdText.Text = "";
                }
                if (icdCodes.Equals(IcdUtil.seperator))
                {
                    txtIcdSubCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string ProcessIcdNameChanged(string oldIcdNames, string newIcdNames)
        {
            //Thuat toan xu ly khi thay doi lai danh sach icd da chon
            //1. Gan danh sach cac ten icd dang chon vao danh sach ket qua
            //2. Tim kiem trong danh sach icd cu, neu ten icd do dang co trong danh sach moi thi bo qua, neu
            //   Neu icd do khong xuat hien trogn danh sach dang chon & khong tim thay ten do trong danh sach icd hien thi ra
            //   -> icd do da sua doi
            //   -> cong vao chuoi ket qua
            string result = "";
            try
            {
                result = newIcdNames;

                if (!String.IsNullOrEmpty(oldIcdNames))
                {
                    var arrNames = oldIcdNames.Split(new string[] { IcdUtil.seperator }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrNames != null && arrNames.Length > 0)
                    {
                        foreach (var item in arrNames)
                        {
                            if (!String.IsNullOrEmpty(item)
                                && !newIcdNames.Contains(IcdUtil.AddSeperateToKey(item))
                                )
                            {
                                var checkInList = this.currentIcds.Where(o =>
                                    IcdUtil.AddSeperateToKey(item).Equals(IcdUtil.AddSeperateToKey(o.ICD_NAME))).FirstOrDefault();
                                if (checkInList == null || checkInList.ID == 0)
                                {
                                    result += item + IcdUtil.seperator;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckIcdWrongCode(ref string strIcdNames, ref string strWrongIcdCodes)
        {
            bool valid = true;
            try
            {
                if (!String.IsNullOrEmpty(this.txtIcdSubCode.Text.Trim()))
                {
                    strWrongIcdCodes = "";
                    List<string> arrWrongCodes = new List<string>();
                    List<string> lstIcdCodes = new List<string>();
                    List<string> lstIcdSubName = new List<string>();
                    List<string> arrIcdExtraCodes = this.txtIcdSubCode.Text.Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries).Where(o => !string.IsNullOrEmpty(o)).Select(o => o.Trim()).Distinct().Where(o => !string.IsNullOrEmpty(o)).ToList();
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = this.currentIcds.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                string messErr = null;
                                if (!checkIcdManager.ProcessCheckIcd(null, icdByCode.ICD_CODE, ref messErr))
                                {
                                    XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
                                    continue;
                                }
                                strIcdNames += (IcdUtil.seperator + icdByCode.ICD_NAME);
                                lstIcdCodes.Add(icdByCode.ICD_CODE);
                                lstIcdSubName.Add(icdByCode.ICD_NAME);
                            }
                            else
                            {
                                arrWrongCodes.Add(itemCode);
                                strWrongIcdCodes += (IcdUtil.seperator + itemCode);
                            }
                        }
                        strIcdNames += IcdUtil.seperator;
                        if (lstIcdCodes != null && lstIcdCodes.Count > 0)
                        {
                            this.txtIcdSubCode.Text = String.Join(";", lstIcdCodes);
                            this.txtIcdText.Text = String.Join(";", lstIcdSubName);

                        }
                        else
                        {

                            this.txtIcdSubCode.Text = null;
                            this.txtIcdText.Text = null;
                        }
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            valid = false;
                            this.SetCheckedIcdsToControl(this.txtIcdSubCode.Text, this.txtIcdText.Text.Trim());
                            XtraMessageBox.Show(String.Format(Resources.ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, string.Join(",", arrWrongCodes)), "Thông báo", MessageBoxButtons.OK);
                            ShowPopupIcdChoose();

                            //MessageManager.Show(String.Format(Resources.ResourceMessage.KhongTimThayIcdTuongUngVoiCacMaSau, strWrongIcdCodes));
                            //int startPositionWarm = 0;
                            //int lenghtPositionWarm = this.txtIcdSubCode.Text.Length - 1;
                            //if (arrWrongCodes != null && arrWrongCodes.Count > 0)
                            //{
                            //    startPositionWarm = this.txtIcdSubCode.Text.IndexOf(arrWrongCodes[0]);
                            //    lenghtPositionWarm = arrWrongCodes[0].Length;
                            //}
                            //this.txtIcdSubCode.Focus();
                            //this.txtIcdSubCode.Select(startPositionWarm, lenghtPositionWarm);
                            //valid = false;
                            //isShowGridIcdSub = true;
                        }
                    }
                    else
                    {
                        txtIcdText.Text = txtIcdSubCode.Text = null;
                        txtIcdSubCode.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool ProccessorByIcdCode(string currentValue)
        {
            bool valid = true;
            try
            {
                string strIcdNames = "";
                string strWrongIcdCodes = "";
                if (!CheckIcdWrongCode(ref strIcdNames, ref strWrongIcdCodes))
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Debug("Ma icd nhap vao khong ton tai trong danh muc. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strWrongIcdCodes), strWrongIcdCodes));
                }
                //this.SetCheckedIcdsToControl(this.txtIcdSubCode.Text, strIcdNames);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void ShowPopupContainerIcsSub()
        {
            try
            {
                int width = Screen.PrimaryScreen.Bounds.Width;
                float widthPlus = width < 1900 ? (width > 1440 ? 50 : 100) : 0;
                float sizePlus = width < 1900 ? (width < 1680 && width > 1400 ? 45 : ((width <= 1366) ? 30 : 0)) : 0;
                popupControlContainerSubIcdName.Width = 600;
                popupControlContainerSubIcdName.Height = 250;
                customGridViewSubIcdName.Focus();
                customGridViewSubIcdName.FocusedRowHandle = 0;
                Rectangle buttonBounds = new Rectangle(panelControlCauseIcd.Bounds.X, panelControlCauseIcd.Bounds.Y, panelControlCauseIcd.Bounds.Width, panelControlCauseIcd.Bounds.Height);
                float sizeText = lblCDPhu.Appearance.Font.Size;
                popupControlContainerSubIcdName.ShowPopup(new Point(buttonBounds.X + layoutControlItem50.Location.X, buttonBounds.Bottom - 200 + (sizeText > 11 ? (int)(sizeText * 4) : (sizeText > 9.5 ? (int)(sizeText * 2.5) : (int)(sizeText))) + (int)sizePlus));//buttonBounds.X + 450 - (int)widthPlus
                //popupControlContainerSubIcdName.ShowPopup(new Point(buttonBounds.X + layoutControlItem50.Location.X, layoutControlItem50.Location.Y + layoutControlItem50.Size.Height));
                Inventec.Common.Logging.LogSystem.Debug("buttonBounds.X + 300=" + buttonBounds.X + 300 + ", buttonBounds.Bottom - 80=" + (buttonBounds.Bottom - 80));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCheckedSubIcdsToControl()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("SetCheckedSubIcdsToControl.1");
                this.isNotProcessWhileChangedTextSubIcd = true;
                string strIcdSubText = "";
                if (txtIcdText.Text.LastIndexOf(";") > -1)
                {
                    strIcdSubText = txtIcdText.Text.Substring(txtIcdText.Text.LastIndexOf(";")).Replace(";", "");
                }
                else
                    strIcdSubText = txtIcdText.Text.Trim();

                string icdNames = null;// IcdUtil.seperator;
                string icdCodes = null;// IcdUtil.seperator;
                string icdName__Olds = txtIcdText.Text.Trim();
                var checkList = this.icdSubcodeAdoChecks.Where(o => o.IsChecked == true).ToList();
                int count = 0;
                foreach (var item in checkList)
                {
                    count++;
                    string messErr = null;
                    if (!checkIcdManager.ProcessCheckIcd(null, item.ICD_CODE, ref messErr))
                    {
                        XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
                        item.IsChecked = false;
                        continue;
                    }
                    if (count == checkList.Count)
                    {
                        icdCodes += item.ICD_CODE;
                        icdNames += item.ICD_NAME;
                    }
                    else
                    {
                        icdCodes += item.ICD_CODE + IcdUtil.seperator;
                        icdNames += item.ICD_NAME + IcdUtil.seperator;
                    }
                }

                string newtxtIcdText = ProcessIcdNameChanged(icdName__Olds, icdNames);

                txtIcdText.Text = icdNames;
                txtIcdSubCode.Text = icdCodes;
                //if (!String.IsNullOrEmpty(strIcdSubText))
                //{
                //    txtIcdText.Text = newtxtIcdText.Substring(0, newtxtIcdText.LastIndexOf(IcdUtil.seperator + strIcdSubText + IcdUtil.seperator) + 1);
                //}
                //if (icdNames.Equals(IcdUtil.seperator))
                //{
                //    txtIcdText.Text = "";
                //}
                //if (icdCodes.Equals(IcdUtil.seperator))
                //{
                //    txtIcdSubCode.Text = "";
                //}
                this.isNotProcessWhileChangedTextSubIcd = false;
                Inventec.Common.Logging.LogSystem.Debug("SetCheckedSubIcdsToControl.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void customGridViewSubIcdName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as HIS.Desktop.Plugins.ExamServiceReqExecute.ADO.IcdADO;
                    if (this.subIcdPopupSelect != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;

                        this.subIcdPopupSelect.IsChecked = !this.subIcdPopupSelect.IsChecked;
                        this.customGridControlSubIcdName.RefreshDataSource();

                        SetCheckedSubIcdsToControl();
                        popupControlContainerSubIcdName.HidePopup();

                        txtIcdText.Focus();
                        txtIcdText.SelectionStart = txtIcdText.Text.Length;
                    }
                }
                //if (e.KeyCode == Keys.Space)
                //{
                //    this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as HIS.Desktop.Plugins.ExamServiceReqExecute.ADO.IcdADO;
                //    if (this.subIcdPopupSelect != null)
                //    {
                //        int currentFocusRow = customGridViewSubIcdName.FocusedRowHandle;

                //        this.subIcdPopupSelect.IsChecked = !this.subIcdPopupSelect.IsChecked;
                //        this.customGridControlSubIcdName.RefreshDataSource();
                //        this.customGridViewSubIcdName.FocusedRowHandle = currentFocusRow;
                //        SetCheckedSubIcdsToControl();
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void customGridControlSubIcdName_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                this.subIcdPopupSelect = this.customGridViewSubIcdName.GetFocusedRow() as HIS.Desktop.Plugins.ExamServiceReqExecute.ADO.IcdADO;
                if (this.subIcdPopupSelect != null)
                {
                    isShowContainerMediMaty = false;
                    isShowContainerMediMatyForChoose = true;

                    this.subIcdPopupSelect.IsChecked = !this.subIcdPopupSelect.IsChecked;
                    this.customGridControlSubIcdName.RefreshDataSource();

                    SetCheckedSubIcdsToControl();
                    popupControlContainerSubIcdName.HidePopup();

                    txtIcdText.Focus();
                    txtIcdText.SelectionStart = txtIcdText.Text.Length;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void customGridViewSubIcdName_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "IsChecked")
                {
                    SetCheckedSubIcdsToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void customGridViewSubIcdName_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.1");
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.2");
                        if (hi.Column.FieldName == "IsChecked" && hi.Column.RealColumnEdit != null && hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.3");
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                            Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.4");
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("customGridViewSubIcdName_MouseDown.5");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void popupControlContainerSubIcdName_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion

        #region UcIcd
        private void txtIcdCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!isShow)
                    {
                        this.icdPopupSelect = this.gridViewIcdCode.GetFocusedRow() as HIS_ICD;
                        if (this.icdPopupSelect != null)
                        {
                            isShowContainerMediMaty = false;
                            isShowContainerMediMatyForChoose = true;
                            popupControlContainerMediMaty.HidePopup();
                            LoadIcdCombo(this.icdPopupSelect.ICD_CODE);
                        }
                    }
                    else
                    {
                        LoadIcdCombo(txtIcdCode.Text.ToUpper());
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewIcdCode.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(panelIcd.Bounds.X, panelIcd.Bounds.Y, panelIcd.Bounds.Width, panelIcd.Bounds.Height);
                    popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X + 110, buttonBounds.Bottom));
                    gridViewIcdCode.Focus();
                    gridViewIcdCode.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtIcdCode.SelectAll();
                    isShow = true;
                    //e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode_Leave(object sender, EventArgs e)
        {
            try
            {
                if (isAutoCheckIcd)
                {
                    cboIcds.Focus();
                    cboIcds.SelectAll();
                }
                else
                {
                    txtIcdSubCode.Focus();
                    txtIcdSubCode.SelectionStart = txtIcdSubCode.Text.Length;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtIcdCode.Text.Trim()))
                {
                    txtIcdCode.Refresh();
                    if (isShowContainerMediMatyForChoose)
                    {
                        gridViewIcdCode.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerMediMaty)
                        {
                            isShowContainerMediMaty = true;
                        }

                        //Filter data
                        gridViewIcdCode.ActiveFilterString = String.Format("[ICD_CODE] Like '%{0}%'", txtIcdCode.Text.Trim());
                        gridViewIcdCode.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewIcdCode.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewIcdCode.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewIcdCode.FocusedRowHandle = 0;
                        gridViewIcdCode.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewIcdCode.OptionsFind.HighlightFindResults = true;

                        Rectangle buttonBounds = new Rectangle(panelIcd.Bounds.X, panelIcd.Bounds.Y, panelIcd.Bounds.Width, panelIcd.Bounds.Height);
                        if (isShow)
                        {
                            popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X + 110, buttonBounds.Bottom));
                            isShow = false;
                        }

                        txtIcdCode.Focus();
                    }
                    isShowContainerMediMatyForChoose = false;
                }
                else
                {
                    gridViewIcdCode.ActiveFilter.Clear();
                    this.icdPopupSelect = null;
                    if (!isShowContainerMediMaty)
                    {
                        popupControlContainerMediMaty.HidePopup();
                    }
                    else
                    {
                        gridViewIcdCode.FocusedRowHandle = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void popupControlContainerMediMaty_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.icdPopupSelect = this.gridViewIcdCode.GetFocusedRow() as HIS_ICD;
                    if (this.icdPopupSelect != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();

                        LoadIcdCombo(this.icdPopupSelect.ICD_CODE);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                this.icdPopupSelect = this.gridViewIcdCode.GetFocusedRow() as HIS_ICD;
                if (this.icdPopupSelect != null)
                {
                    popupControlContainerMediMaty.HidePopup();
                    isShowContainerMediMaty = false;
                    isShowContainerMediMatyForChoose = true;

                    LoadIcdCombo(this.icdPopupSelect.ICD_CODE);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadIcdCombo(string searchCode)
        {
            try
            {
                bool showCbo = true;
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var listData = currentIcds.Where(o => o.ICD_CODE.Contains(searchCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == searchCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        txtIcdCode.Text = result.First().ICD_CODE;
                        txtIcdMainText.Text = result.First().ICD_NAME;
                        cboIcds.EditValue = listData.First().ID;
                        chkEditIcd.Checked = (chkEditIcd.Enabled ? this.isAutoCheckIcd : false);
                        string messErr = null;
                        if (!checkIcdManager.ProcessCheckIcd(txtIcdCode.Text.Trim(), txtIcdSubCode.Text.Trim(), ref messErr))
                        {
                            XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
                            if (CheckIcdManager.IcdCodeError.Equals(txtIcdCode.Text.Trim()))
                            {
                                txtIcdCode.Text = txtIcdMainText.Text = null;
                                cboIcds.EditValue = null;
                            }
                            return;
                        }
                        if (chkEditIcd.Checked)
                        {
                            txtIcdMainText.Focus();
                            txtIcdMainText.SelectAll();
                        }
                        else
                        {
                            txtIcdSubCode.Focus();
                            txtIcdSubCode.SelectionStart = txtIcdSubCode.Text.Length;
                        }

                        // reload icd
                        ReloadIcdFinish(result.First());
                    }
                }

                if (showCbo)
                {
                    cboIcds.Focus();
                    cboIcds.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadIcdFinish(HIS_ICD icdInput)
        {
            try
            {

                if (ucHospitalize != null)
                {
                    UC.Hospitalize.ADO.HospitalizeInitADO ado = new UC.Hospitalize.ADO.HospitalizeInitADO();
                    ado.IcdCode = icdInput != null ? icdInput.ICD_CODE : "";
                    ado.IcdName = icdInput != null ? icdInput.ICD_NAME : "";

                    if (HisServiceReqResult != null && HisServiceReqResult.HospitalizeResult != null && HisServiceReqResult.HospitalizeResult.Treatment != null)
                    {
                        ado.InCode = HisServiceReqResult.HospitalizeResult.Treatment.IN_CODE;
                        ado.TraditionalIcdCode = HisServiceReqResult.HospitalizeResult.Treatment.TRADITIONAL_ICD_CODE;
                        ado.TraditionalIcdName = HisServiceReqResult.HospitalizeResult.Treatment.TRADITIONAL_ICD_NAME;
                        ado.isAutoCheckChkHospitalizeExam = HisConfigCFG.IsAutoCheckPrintHospitalizeExam;
                    }
                    hospitalizeProcessor.ReLoad(ucHospitalize, ado);
                }

                if (ucTreatmentFinish != null)
                {
                    UC.ExamTreatmentFinish.ADO.TreatmentFinishInitADO ado = new UC.ExamTreatmentFinish.ADO.TreatmentFinishInitADO();
                    ado.IcdCode = icdInput != null ? icdInput.ICD_CODE : "";
                    ado.IcdName = icdInput != null ? icdInput.ICD_NAME : "";

                    treatmentFinishProcessor.ReLoad(ucTreatmentFinish, ado);
                }


                this.icdDefaultFinish.ICD_CODE = icdInput != null ? icdInput.ICD_CODE : "";
                this.icdDefaultFinish.ICD_NAME = icdInput != null ? icdInput.ICD_NAME : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdMainText_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    if (layoutControlItem61.Visible)
                    {
                        txtIcdSubCode.Focus();
                        txtIcdSubCode.SelectionStart = txtIcdSubCode.Text.Length;
                    }
                    else if (lciIcdTextCause.Visible)
                    {
                        this.NextForcusSubIcd();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcds.EditValue != null)
                        this.ChangecboChanDoanTD();
                    else if (this.IsAcceptWordNotInData && this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextIcdName))
                        this.ChangecboChanDoanTD_V2_GanICDNAME(this._TextIcdName);
                    else
                    {
                        if (layoutControlItem61.Visible)
                        {
                            txtIcdSubCode.Focus();
                            txtIcdSubCode.SelectionStart = txtIcdSubCode.Text.Length;
                        }
                        else if (lciIcdTextCause.Visible)
                        {
                            this.NextForcusSubIcd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD()
        {
            try
            {
                txtIcdCode.ErrorText = "";
                dxValidationProviderForLeftPanel.RemoveControlError(txtIcdCode);
                cboIcds.Properties.Buttons[1].Visible = true;
                MOS.EFMODEL.DataModels.HIS_ICD icd = currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcds.EditValue ?? 0).ToString()));
                if (icd != null)
                {
                    this.isShowContainerMediMatyForChoose = true;
                    txtIcdCode.Text = icd.ICD_CODE;
                    txtIcdMainText.Text = icd.ICD_NAME;
                    string messErr = null;
                    if (!checkIcdManager.ProcessCheckIcd(txtIcdCode.Text.Trim(), txtIcdSubCode.Text.Trim(), ref messErr))
                    {
                        XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
                        if (CheckIcdManager.IcdCodeError.Equals(txtIcdCode.Text.Trim()))
                        {
                            txtIcdCode.Text = txtIcdMainText.Text = null;
                            cboIcds.EditValue = null;
                        }
                        return;
                    }
                    chkEditIcd.Checked = (chkEditIcd.Enabled ? this.isAutoCheckIcd : false);

                    if (layoutControlItem61.Visible)
                    {
                        txtIcdSubCode.Focus();
                        txtIcdSubCode.SelectAll();
                    }
                    else if (lciIcdTextCause.Visible)
                    {
                        this.NextForcusSubIcd();
                    }

                    //if (chkEditIcd.Checked)
                    //{
                    //    this.NextForcusSubIcd();
                    //}
                    //else if (chkEditIcd.Enabled)
                    //{
                    //    chkEditIcd.Focus();
                    //}
                    //else
                    //{
                    //    this.NextForcusSubIcd();
                    //}

                    ReloadIcdFinish(icd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD_V2_GanICDNAME(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return;
                if (this.autoCheckIcd != 2)
                {
                    this.chkEditIcd.Enabled = true;
                    this.chkEditIcd.Checked = true;
                }
                this.txtIcdMainText.Text = text;
                this.txtIcdMainText.Focus();
                this.txtIcdMainText.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboIcds.ClosePopup();
                    cboIcds.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboIcds.ClosePopup();
                    if (cboIcds.EditValue != null)
                        this.ChangecboChanDoanTD();
                    else
                        if (lciIcdTextCause.Visible)
                    {
                        this.NextForcusSubIcd();
                    }
                    else
                    {
                        txtIcdSubCode.Focus();
                        txtIcdSubCode.SelectionStart = txtIcdSubCode.Text.Length;
                    }
                }
                else
                    cboIcds.ShowPopup();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditIcd_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                HIS_ICD icd = new HIS_ICD();

                if (chkEditIcd.Checked == true)
                {
                    cboIcds.Visible = false;
                    txtIcdMainText.Visible = true;
                    if (this.IsObligatoryTranferMediOrg)
                        txtIcdMainText.Text = this._TextIcdName;
                    else
                        txtIcdMainText.Text = cboIcds.Text.Trim();
                    txtIcdMainText.Focus();
                    txtIcdMainText.SelectAll();

                    icd.ICD_CODE = txtIcdCode.Text.Trim();
                    icd.ICD_NAME = txtIcdMainText.Text.Trim();
                }
                else if (chkEditIcd.Checked == false)
                {
                    txtIcdMainText.Visible = false;
                    cboIcds.Visible = true;
                    txtIcdMainText.Text = cboIcds.Text.Trim();

                    if (this.cboIcds.EditValue != null)
                        icd = this.currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboIcds.EditValue.ToString()));
                    else
                        icd = null;
                }

                if (icd != null)
                {
                    ReloadIcdFinish(icd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditIcd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtIcdMainText.Text != null)
                    {
                        //this.data.DelegateRefeshIcdMainText(txtIcdMainText.Text.Trim());
                    }
                    if (cboIcds.EditValue != null)
                    {
                        //var hisIcd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.ID == (long)cboIcds.EditValue).FirstOrDefault();
                        //this.data.DelegateRefeshIcd(hisIcd);
                    }
                    if (layoutControlItem61.Visible)
                    {
                        txtIcdSubCode.Focus();
                        txtIcdSubCode.SelectionStart = txtIcdSubCode.Text.Length;
                    }
                    else if (lciIcdTextCause.Visible)
                    {
                        this.NextForcusSubIcd();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = Resources.ResourceMessage.IcdKhongDung;
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text.Trim();
                if (!String.IsNullOrEmpty(search))
                {
                    var listData = this.currentIcds.Where(o => o.ICD_CODE.Contains(search)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == search).ToList() : listData) : null;
                    if (result == null || result.Count <= 0)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        txtIcdCode.ErrorText = "";
                        dxValidationProviderForLeftPanel.RemoveControlError(txtIcdCode);
                        ValidationICD(10, 500, !this.isAllowNoIcd);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcds_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboIcds.Properties.Buttons[1].Visible)
                        return;
                    this._TextIcdName = "";
                    cboIcds.EditValue = null;
                    txtIcdCode.Text = "";
                    txtIcdMainText.Text = "";
                    this.icdPopupSelect = null;
                    cboIcds.Properties.Buttons[1].Visible = false;
                    ReloadIcdFinish(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcds.Text.Trim()))
                {
                    cboIcds.EditValue = null;
                    txtIcdMainText.Text = "";
                    chkEditIcd.Checked = false;
                }
                else
                {
                    this._TextIcdName = cboIcds.Text.Trim();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcds_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.1");
                HIS_ICD icd = null;
                if (this.cboIcds.EditValue != null)
                    icd = this.currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboIcds.EditValue.ToString()));
                //if (this.isExecuteValueChanged && refeshIcd != null)
                //{
                //    Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.2");
                //    this.refeshIcd(icd);
                //}

                if (icd != null)
                {
                    if (icd != null && icd.IS_REQUIRE_CAUSE == 1 && !this.isAllowNoIcd)
                    {
                        this.LoadRequiredCause(true);
                    }
                    else
                        this.LoadRequiredCause(false);

                    ReloadIcdFinish(icd);
                }
                else
                {
                    this.LoadRequiredCause(false);
                }

                Inventec.Common.Logging.LogSystem.Debug("cboIcds_EditValueChanged.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcds_Leave(object sender, EventArgs e)
        {
            try
            {
                if (layoutControlItem61.Visible)
                {
                    txtIcdSubCode.Focus();
                    txtIcdSubCode.SelectionStart = txtIcdSubCode.Text.Length;
                }
                else if (lciIcdTextCause.Visible)
                {
                    this.NextForcusSubIcd();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCode_Validated(object sender, EventArgs e)
        {
        }

        private void txtIcdMainText_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = ResourceMessage.CanhbaoKhongChoSuaICDName;
                AutoValidate = AutoValidate.EnableAllowFocusChange;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdMainText_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                var search = ((DevExpress.XtraEditors.TextEdit)sender).Text.Trim();
                if (!String.IsNullOrEmpty(search))
                {
                    long AllowToEditIcdName = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.ALLOW_TOEDIT_ICD_NAME));

                    Inventec.Common.Logging.LogSystem.Warn("AllowToEditIcdName: " + AllowToEditIcdName + " _TextIcdName: " + this._TextIcdName);
                    if (AllowToEditIcdName == 1 && !string.IsNullOrEmpty(this._TextIcdName))
                    {
                        if (search.StartsWith(this._TextIcdName) == false)
                        {
                            e.Cancel = true;
                        }
                        else
                        {
                            txtIcdMainText.ErrorText = "";
                            dxValidationProviderForLeftPanel.RemoveControlError(txtIcdMainText);
                            ValidationICD(10, 500, !this.isAllowNoIcd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdMainText_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtIcdMainText_Leave(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("txtIcdMainText_EditValueChanged.1");
                HIS_ICD icd = new HIS_ICD();
                //cboKskCode.Focus();
                icd.ICD_CODE = txtIcdCode.Text.Trim();
                icd.ICD_NAME = txtIcdMainText.Text.Trim();

                if (icd != null)
                {

                    ReloadIcdFinish(icd);
                }

                Inventec.Common.Logging.LogSystem.Debug("txtIcdMainText_EditValueChanged.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region UcIcdCause
        private void txtIcdCodeCause_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadIcdComboCause(txtIcdCodeCause.Text.ToUpper());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdComboCause(string searchCode)
        {
            try
            {
                bool showCbo = true;
                if (!String.IsNullOrEmpty(searchCode))
                {
                    var listData = currentIcds.Where(o => o.ICD_CODE.Contains(searchCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == searchCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {
                        showCbo = false;
                        txtIcdCodeCause.Text = result.First().ICD_CODE;
                        txtIcdMainTextCause.Text = result.First().ICD_NAME;
                        cboIcdsCause.EditValue = listData.First().ID;
                        chkEditIcdCause.Checked = (chkEditIcdCause.Enabled ? this.isAutoCheckIcd : false);

                        if (chkEditIcdCause.Checked)
                        {
                            txtIcdMainTextCause.Focus();
                            txtIcdMainTextCause.SelectAll();
                        }
                        else
                        {
                            cboIcdsCause.Focus();
                            cboIcdsCause.SelectAll();
                        }
                    }
                }

                if (showCbo)
                {
                    Inventec.Common.Logging.LogSystem.Debug("LoadIcdComboCause cboIcdsCause.ShowPopup()");
                    cboIcdsCause.Focus();
                    cboIcdsCause.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdMainTextCause_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    //UcIcdNextForcusOut();
                    //chkEditIcdCause.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcdsCause_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    if (cboIcdsCause.EditValue != null)
                        this.ChangecboChanDoanTDCause();
                    else if (this.IsAcceptWordNotInData && this.IsObligatoryTranferMediOrg && !string.IsNullOrEmpty(this._TextIcdNameCause))
                        this.ChangecboChanDoanTD_V2_GanICDNAMECause(this._TextIcdNameCause);
                    else
                        SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTDCause()
        {
            try
            {
                cboIcdsCause.Properties.Buttons[1].Visible = true;
                MOS.EFMODEL.DataModels.HIS_ICD icd = currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcdsCause.EditValue ?? 0).ToString()));
                if (icd != null)
                {
                    txtIcdCodeCause.Text = icd.ICD_CODE;
                    txtIcdMainTextCause.Text = icd.ICD_NAME;
                    chkEditIcdCause.Checked = (chkEditIcdCause.Enabled ? this.isAutoCheckIcd : false);
                    if (chkEditIcdCause.Enabled)
                    {
                        chkEditIcdCause.Focus();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangecboChanDoanTD_V2_GanICDNAMECause(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text))
                    return;
                if (this.autoCheckIcd != 2)
                {
                    this.chkEditIcdCause.Enabled = true;
                    this.chkEditIcdCause.Checked = true;
                }
                this.txtIcdMainTextCause.Text = text;
                this.txtIcdMainTextCause.Focus();
                this.txtIcdMainTextCause.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcdsCause_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboIcdsCause.ClosePopup();
                    cboIcdsCause.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboIcdsCause.ClosePopup();
                    if (cboIcdsCause.EditValue != null)
                        this.ChangecboChanDoanTDCause();
                    else
                    {
                        //UcIcdNextForcusOut();
                    }
                }
                // if (e.KeyCode == Keys.Enter)
                //{
                //    Inventec.Common.Logging.LogSystem.Debug("cboIcdsCause_KeyUp cboIcdsCause.ShowPopup()");
                //    cboIcdsCause.ShowPopup();
                //}
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditIcdCause_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkEditIcdCause.Checked == true)
                {
                    cboIcdsCause.Visible = false;
                    txtIcdMainTextCause.Visible = true;
                    if (this.IsObligatoryTranferMediOrg)
                        txtIcdMainTextCause.Text = this._TextIcdNameCause;
                    else
                        txtIcdMainTextCause.Text = cboIcdsCause.Text.Trim();
                    txtIcdMainTextCause.Focus();
                    txtIcdMainTextCause.SelectAll();
                }
                else if (chkEditIcdCause.Checked == false)
                {
                    txtIcdMainTextCause.Visible = false;
                    cboIcdsCause.Visible = true;
                    txtIcdMainTextCause.Text = cboIcdsCause.Text.Trim();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkEditIcdCause_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtIcdMainTextCause.Text != null)
                    {
                        //this.data.DelegateRefeshIcdMainText(txtIcdMainText.Text.Trim());
                    }
                    if (cboIcdsCause.EditValue != null)
                    {
                        //var hisIcd = BackendDataWorker.Get<HIS_ICD>().Where(p => p.ID == (long)cboIcds.EditValue).FirstOrDefault();
                        //this.data.DelegateRefeshIcd(hisIcd);
                    }
                    //UcIcdNextForcusOut();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCodeCause_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                if (this.isAllowNoIcd)
                {
                    e.ErrorText = Resources.ResourceMessage.IcdKhongDung;
                    AutoValidate = AutoValidate.EnableAllowFocusChange;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtIcdCodeCause_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (this.isAllowNoIcd)
                {
                    var search = ((DevExpress.XtraEditors.TextEdit)sender).Text.Trim();
                    if (!String.IsNullOrEmpty(search))
                    {
                        var listData = this.currentIcds.Where(o => o.ICD_CODE.Contains(search)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == search).ToList() : listData) : null;
                        if (result == null || result.Count <= 0)
                        {
                            e.Cancel = true;
                        }
                        else
                        {
                            txtIcdCodeCause.ErrorText = "";
                            dxValidationProviderForLeftPanel.RemoveControlError(txtIcdCodeCause);
                            ValidationICDCause(10, 500, !this.isAllowNoIcd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcdsCause_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    if (!cboIcdsCause.Properties.Buttons[1].Visible)
                        return;
                    this._TextIcdNameCause = "";
                    cboIcdsCause.EditValue = null;
                    txtIcdCodeCause.Text = "";
                    txtIcdMainTextCause.Text = "";
                    cboIcdsCause.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcdsCause_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboIcdsCause.Text.Trim()))
                {
                    cboIcdsCause.EditValue = null;
                    txtIcdMainTextCause.Text = "";
                    chkEditIcdCause.Checked = false;
                }
                else
                {
                    this._TextIcdNameCause = cboIcdsCause.Text.Trim();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void OpenModuleTextLibrary(string content, string hashtag)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TextLibrary").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TextLibrary");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    TextLibraryInfoADO ado = new TextLibraryInfoADO();
                    ado.Content = content;
                    ado.Hashtag = hashtag;
                    listArgs.Add(ado);
                    listArgs.Add((HIS.Desktop.Common.DelegateDataTextLib)ProcessDataTextLib);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                    WaitingManager.Hide();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnKhamToanThan_Click(object sender, EventArgs e)
        {
            try
            {
                key = 1;
                OpenModuleTextLibrary(txtKhamToanThan.Text, "KhamToanThan");

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataTextLib(MOS.EFMODEL.DataModels.HIS_TEXT_LIB textLib)
        {
            try
            {
                switch (key)
                {
                    case 1:
                        this.txtKhamToanThan.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 2:
                        this.txtKhamBoPhan.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 3:
                        this.txtThanTietNieu.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 4:
                        this.txtTieuHoa.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 5:
                        this.txtTuanHoan.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 6:
                        this.txtHoHap.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 7:
                        this.txtThanKinh.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 8:
                        this.txtCoXuongKhop.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 9:
                        this.txtTai.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 10:
                        this.txtRHM.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 11:
                        this.txtMat.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 12:
                        this.txtNoiTiet.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 13:
                        this.txtPartExanObstetric.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 14:
                        this.txtDaLieu.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 15:
                        this.txtPartExamNutrition.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 16:
                        this.txtPartExamMental.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 17:
                        this.txtPartExamMotion.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 18:
                        this.txtHospitalizationReason.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 19:
                        this.txtPathologicalProcess.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 20:
                        this.txtPathologicalHistory.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;
                    case 21:
                        this.txtPathologicalHistoryFamily.Text = HIS.Desktop.Utility.TextLibHelper.BytesToString(textLib.CONTENT);
                        break;

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnKhamBoPhan_Click(object sender, EventArgs e)
        {
            try
            {
                if (tabControlDetailData.SelectedTabPage == xtraTabPageChung)
                {
                    key = 2;
                    OpenModuleTextLibrary("", "Chung");

                }
                else if (tabControlDetailData.SelectedTabPage == xtraTabThanTietNieu)
                {
                    key = 3;
                    OpenModuleTextLibrary("", "ThanTietNieu");

                }
                else if (tabControlDetailData.SelectedTabPage == xtraTabTieuHoa)
                {
                    key = 4;
                    OpenModuleTextLibrary("", "TieuHoa");

                }
                else if (tabControlDetailData.SelectedTabPage == xtraTabTuanHoan)
                {
                    key = 5;
                    OpenModuleTextLibrary("", "TuanHoan");

                }
                else if (tabControlDetailData.SelectedTabPage == xtraTabHoHap)
                {
                    key = 6;
                    OpenModuleTextLibrary("", "HoHap");

                }
                else if (tabControlDetailData.SelectedTabPage == xtraTabThanKinh)
                {
                    key = 7;
                    OpenModuleTextLibrary("", "ThanKinh");

                }
                else if (tabControlDetailData.SelectedTabPage == xtraTabCoXuongKhop)
                {
                    key = 8;
                    OpenModuleTextLibrary("", "CoXuongKhop");

                }
                else if (tabControlDetailData.SelectedTabPage == xtraTabTaiMuiHong)
                {
                    key = 9;
                    OpenModuleTextLibrary("", "TaiMuiHong");

                }
                else if (tabControlDetailData.SelectedTabPage == xtraTabRangHamMat)
                {
                    key = 10;
                    OpenModuleTextLibrary("", "RangHamMat");

                }
                else if (tabControlDetailData.SelectedTabPage == xtraTabMat)
                {
                    key = 11;
                    OpenModuleTextLibrary("", "Mat");

                }
                else if (tabControlDetailData.SelectedTabPage == xtraTabNoiTiet)
                {
                    key = 12;
                    OpenModuleTextLibrary("", "NoiTiet");

                }
                else if (tabControlDetailData.SelectedTabPage == xtraTabSanPhuKhoa)
                {
                    key = 13;
                    OpenModuleTextLibrary("", "SanPhuKhoa");

                }
                else if (tabControlDetailData.SelectedTabPage == xtraTabDaLieu)
                {
                    key = 14;
                    OpenModuleTextLibrary("", "DaLieu");

                }
                else if (tabControlDetailData.SelectedTabPage == xtraTabDinhDuong)
                {
                    key = 15;
                    OpenModuleTextLibrary("", "DinhDuong");

                }
                else if (tabControlDetailData.SelectedTabPage == xtraTabTamThan)
                {
                    key = 16;
                    OpenModuleTextLibrary("", "TamThan");

                }
                else
                {
                    key = 17;
                    OpenModuleTextLibrary("", "VanDong");

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnLyDoKham_Click(object sender, EventArgs e)
        {
            try
            {
                key = 18;
                OpenModuleTextLibrary("", "LyDoKham");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnQuaTrinhBenhLy_Click(object sender, EventArgs e)
        {
            try
            {
                key = 19;
                OpenModuleTextLibrary("", "QuaTrinhBenhLy");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTienSuBenhNhan_Click(object sender, EventArgs e)
        {
            try
            {
                key = 20;
                OpenModuleTextLibrary("", "TienSuBenhCuaBenhNhan");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTienSuGiaDinh_Click(object sender, EventArgs e)
        {
            try
            {
                key = 21;
                OpenModuleTextLibrary("", "TienSuBenhCuaGiaDinh");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdSubCode_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtIcdSubCode.Text.Trim()))
                {
                    if (!isShowSubIcd)
                        return;
                    txtIcdSubCode.Refresh();
                    string keyWord = "";
                    if (txtIcdSubCode.Text.Contains(";"))
                    {
                        var arrText = txtIcdSubCode.Text.Split(';');
                        keyWord = arrText[arrText.Length - 1];
                    }
                    else
                    {
                        keyWord = txtIcdSubCode.Text.Trim();
                    }
                    gvIcdSubCode.ActiveFilterString = String.Format("[ICD_CODE] Like '%{0}%'", keyWord);
                    gvIcdSubCode.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                    gvIcdSubCode.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                    gvIcdSubCode.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                    gvIcdSubCode.FocusedRowHandle = 0;
                    gvIcdSubCode.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                    gvIcdSubCode.OptionsFind.HighlightFindResults = true;

                    Rectangle buttonBounds = new Rectangle(panelControlCauseIcd.Bounds.X, panelControlCauseIcd.Bounds.Y, panelControlCauseIcd.Bounds.Width, panelControlCauseIcd.Bounds.Height);
                    if (isShowSubIcd)
                    {
                        popupControlContainer1.ShowPopup(new Point(buttonBounds.X + 110, buttonBounds.Bottom - 90));
                        isShowSubTemp = true;
                    }
                    txtIcdSubCode.Focus();

                }
                else
                {
                    icdCodeList = new List<string>();
                    gvIcdSubCode.ActiveFilter.Clear();
                    this.IcdSubChoose = null;
                    if (!isShowGridIcdSub)
                    {
                        popupControlContainer1.HidePopup();
                    }
                    else
                    {
                        gvIcdSubCode.FocusedRowHandle = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void popupControlContainer1_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShowSubIcd = false;
                isShowSubTemp = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gvIcdSubCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                try
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        this.IcdSubChoose = this.gvIcdSubCode.GetFocusedRow() as HIS_ICD;
                        if (this.IcdSubChoose != null)
                        {
                            popupControlContainer1.HidePopup();
                            isShowSubIcd = false;
                            gridViewIcdCode.ActiveFilter.Clear();
                            LoadSubIcd(this.IcdSubChoose.ICD_CODE);
                            txtIcdSubCode.Focus();
                            txtIcdSubCode.Select(txtIcdSubCode.Text.Length, txtIcdSubCode.Text.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gvIcdSubCode_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                try
                {
                    this.IcdSubChoose = this.gvIcdSubCode.GetFocusedRow() as HIS_ICD;
                    if (this.IcdSubChoose != null)
                    {
                        popupControlContainer1.HidePopup();
                        isShowSubIcd = false;
                        gridViewIcdCode.ActiveFilter.Clear();
                        LoadSubIcd(this.IcdSubChoose.ICD_CODE);
                        txtIcdSubCode.Focus();
                        txtIcdSubCode.Select(txtIcdSubCode.Text.Length, txtIcdSubCode.Text.Length);
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSubIcd(string icdSubCode)
        {
            try
            {
                if (!String.IsNullOrEmpty(icdSubCode))
                {
                    var listData = currentIcds.Where(o => o.ICD_CODE.Contains(icdSubCode)).ToList();
                    var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.ICD_CODE == icdSubCode).ToList() : listData) : null;
                    if (result != null && result.Count > 0)
                    {

                        if (txtIcdSubCode.Text.Contains(";"))
                        {
                            var arrText = txtIcdSubCode.Text.Split(';');
                            string txt = "";
                            for (int i = 0; i < arrText.Length - 1; i++)
                            {
                                txt += arrText[i] + ";";
                            }
                            txtIcdSubCode.Text = txt + result.First().ICD_CODE + ";";
                        }
                        else
                        {
                            txtIcdSubCode.Text = result.First().ICD_CODE + ";";
                        }
                    }
                }
                var arrIcdCode = txtIcdSubCode.Text.Trim().Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                icdCodeList = new List<string>();
                foreach (var item in arrIcdCode)
                {
                    string messErr = null;
                    if (!checkIcdManager.ProcessCheckIcd(null, item, ref messErr))
                    {
                        XtraMessageBox.Show(messErr, "Thông báo", MessageBoxButtons.OK);
                        continue;
                    }
                    icdCodeList.Add(item);
                }
                txtIcdSubCode.Text = string.Join(";", icdCodeList);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFastTrackingCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (HisConfigCFG.RequiredWeightHeight_Option == "1" && !ValidDhstOption())
                    return;
                ValiTemperatureOption();
                if (!this.ValidForButtonOtherClick()) return;


                if (this.requiredControl != null && this.requiredControl == 1 && string.IsNullOrEmpty(this.txtPathologicalProcess.Text.Trim()))
                {
                    MessageBox.Show("Quá trình bệnh lý bạn nhập không hợp lệ", ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!VerifyTreatmentFinish())
                    return;

                if (!CheckExamServiceFinish())
                    return;

                CommonParam param = new CommonParam();
                WaitingManager.Show();
                HisTrackingSDO sdo = new HisTrackingSDO();
                sdo.WorkingRoomId = moduleData.RoomId;
                //--tracking
                sdo.Tracking = new HIS_TRACKING();
                sdo.Tracking.DEPARTMENT_ID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == moduleData.RoomId).DepartmentId;
                sdo.Tracking.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                sdo.Tracking.ICD_TEXT = txtIcdText.Text.Trim();
                sdo.Tracking.ICD_SUB_CODE = txtIcdSubCode.Text.Trim();
                sdo.Tracking.ICD_NAME = txtIcdMainText.Text.Trim();
                sdo.Tracking.ICD_CODE = txtIcdCode.Text.Trim();
                sdo.Tracking.TRACKING_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                sdo.Tracking.TREATMENT_ID = treatment.ID;
                sdo.Tracking.CONTENT = txtPathologicalProcess.Text + "\r\n" + txtPathologicalHistory.Text + "\r\n"
                    + txtPathologicalHistoryFamily.Text + "\r\n" + txtKhamToanThan.Text + "\r\n" + txtKhamBoPhan.Text.Trim();
                sdo.Tracking.SUBCLINICAL_PROCESSES = txtSubclinical.Text.Trim();
                //--dhst
                sdo.Dhst = new HIS_DHST();
                sdo.Dhst.TREATMENT_ID = treatment.ID;
                sdo.Dhst.EXECUTE_ROOM_ID = moduleData.RoomId;
                sdo.Dhst.EXECUTE_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                sdo.Dhst.EXECUTE_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                if (spinBelly.EditValue != null)
                    sdo.Dhst.BELLY = Inventec.Common.Number.Get.RoundCurrency(spinBelly.Value, 2);
                if (spinBloodPressureMax.EditValue != null)
                    sdo.Dhst.BLOOD_PRESSURE_MAX = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMax.Value.ToString());
                if (spinBloodPressureMin.EditValue != null)
                    sdo.Dhst.BLOOD_PRESSURE_MIN = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMin.Value.ToString());
                if (spinBreathRate.EditValue != null)
                    sdo.Dhst.BREATH_RATE = Inventec.Common.Number.Get.RoundCurrency(spinBreathRate.Value, 2);
                sdo.Dhst.CAPILLARY_BLOOD_GLUCOSE = null;
                if (spinChest.EditValue != null)
                    sdo.Dhst.CHEST = Inventec.Common.Number.Get.RoundCurrency(spinChest.Value, 2);
                if (dtExecuteTime.EditValue != null)
                    sdo.Dhst.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExecuteTime.DateTime);
                if (spinHeight.EditValue != null)
                    sdo.Dhst.HEIGHT = Inventec.Common.Number.Get.RoundCurrency(spinHeight.Value, 2);
                sdo.Dhst.NOTE = null;
                if (spinPulse.EditValue != null)
                    sdo.Dhst.PULSE = Inventec.Common.TypeConvert.Parse.ToInt64(spinPulse.Value.ToString());
                if (spinTemperature.EditValue != null)
                    sdo.Dhst.TEMPERATURE = Inventec.Common.Number.Get.RoundCurrency(spinTemperature.Value, 2);
                if (spinWeight.EditValue != null)
                    sdo.Dhst.WEIGHT = Inventec.Common.Number.Get.RoundCurrency(spinWeight.Value, 2);
                if (spinSPO2.EditValue != null)
                    sdo.Dhst.SPO2 = Inventec.Common.Number.Get.RoundCurrency(spinSPO2.Value, 2) / 100;
                //--ServiceReqs
                MOS.Filter.HisServiceReqFilter _reqFilter = new HisServiceReqFilter();
                _reqFilter.TREATMENT_ID = this.treatmentId;
                _reqFilter.INTRUCTION_DATE__EQUAL = Int64.Parse(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now).ToString().Substring(0, 8) + "000000");
                _reqFilter.REQUEST_LOGINNAME__EXACT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                _reqFilter.HAS_EXECUTE = true;

                var dataReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, _reqFilter, param);
                dataReqs = dataReqs.Where(o => o.TRACKING_ID == null).ToList();
                if (dataReqs != null && dataReqs.Count > 0)
                {
                    sdo.ServiceReqs = new List<TrackingServiceReq>();
                    foreach (var item in dataReqs)
                    {
                        TrackingServiceReq ado = new TrackingServiceReq();
                        ado.ServiceReqId = item.ID;
                        ado.IsNotShowMedicine = false;
                        ado.IsNotShowMaterial = false;
                        ado.IsNotShowOutMedi = false;
                        ado.IsNotShowOutMate = false;
                        sdo.ServiceReqs.Add(ado);
                    }
                }
                bool success = false;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));

                var resultData = new BackendAdapter(param).Post<HIS_TRACKING>("api/HisTracking/Create", ApiConsumers.MosConsumer, sdo, param);
                if (resultData != null)
                {
                    success = true;
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ModuleList()
        {
            try
            {
                if (!string.IsNullOrEmpty(HisConfigCFG.ModuleLinkApply))
                {
                    lstModuleLinkApply = new List<string>();
                    foreach (var item in HisConfigCFG.ModuleLinkApply.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList())
                    {
                        lstModuleLinkApply.Add(item.Trim());

                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnViewInformationExam_Click(object sender, EventArgs e)
        {
            try
            {
                SendSDOExam();
                frmInformationExam frm = new frmInformationExam(HisServiceReqView, sdoSendFrmExam, GetSDOExam, tabControlDetailData.SelectedTabPageIndex);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SendSDOExam()
        {
            try
            {
                sdoSendFrmExam = new HisServiceReqExamUpdateSDO();
                sdoSendFrmExam.PartExam = !string.IsNullOrEmpty(txtKhamBoPhan.Text.Trim()) ? txtKhamBoPhan.Text.Trim() : null;
                sdoSendFrmExam.PartExamCirculation = !string.IsNullOrEmpty(txtTuanHoan.Text.Trim()) ? txtTuanHoan.Text.Trim() : null;
                sdoSendFrmExam.PartExamDigestion = !string.IsNullOrEmpty(txtTieuHoa.Text.Trim()) ? txtTieuHoa.Text.Trim() : null;
                sdoSendFrmExam.PartExamEar = !string.IsNullOrEmpty(txtTai.Text.Trim()) ? txtTai.Text.Trim() : null;//TODO
                //TODO
                sdoSendFrmExam.PartExamEarRightNormal = !string.IsNullOrEmpty(txtPART_EXAM_EAR_RIGHT_NORMAL.Text.Trim()) ? txtPART_EXAM_EAR_RIGHT_NORMAL.Text.Trim() : null;
                sdoSendFrmExam.PartExamEarRightWhisper = !string.IsNullOrEmpty(txtPART_EXAM_EAR_RIGHT_WHISPER.Text.Trim()) ? txtPART_EXAM_EAR_RIGHT_WHISPER.Text.Trim() : null;
                sdoSendFrmExam.PartExamEarLeftNormal = !string.IsNullOrEmpty(txtPART_EXAM_EAR_LEFT_NORMAL.Text.Trim()) ? txtPART_EXAM_EAR_LEFT_NORMAL.Text.Trim() : null;
                sdoSendFrmExam.PartExamEarLeftWhisper = !string.IsNullOrEmpty(txtPART_EXAM_EAR_LEFT_WHISPER.Text.Trim()) ? txtPART_EXAM_EAR_LEFT_WHISPER.Text.Trim() : null;

                if (chkPART_EXAM_HORIZONTAL_SIGHT__BT.Checked)
                    sdoSendFrmExam.PartExamHorizontalSight = 1;
                else if (chkPART_EXAM_HORIZONTAL_SIGHT__HC.Checked)
                    sdoSendFrmExam.PartExamHorizontalSight = 2;
                else
                {
                    sdoSendFrmExam.PartExamHorizontalSight = null;
                }

                if (chkPART_EXAM_VERTICAL_SIGHT__BT.Checked)
                    sdoSendFrmExam.PartExamVerticalSight = 1;
                else if (chkPART_EXAM_VERTICAL_SIGHT__HC.Checked)
                    sdoSendFrmExam.PartExamVerticalSight = 2;
                else
                {
                    sdoSendFrmExam.PartExamVerticalSight = null;
                }

                if (chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked)
                    sdoSendFrmExam.PartExamEyeBlindColor = 1;
                else if (chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked)
                    sdoSendFrmExam.PartExamEyeBlindColor = 2;
                else
                {
                    if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                        sdoSendFrmExam.PartExamEyeBlindColor = 9;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                        sdoSendFrmExam.PartExamEyeBlindColor = 8;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                        sdoSendFrmExam.PartExamEyeBlindColor = 7;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked)
                        sdoSendFrmExam.PartExamEyeBlindColor = 6;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                        sdoSendFrmExam.PartExamEyeBlindColor = 5;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked)
                        sdoSendFrmExam.PartExamEyeBlindColor = 4;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked)
                        sdoSendFrmExam.PartExamEyeBlindColor = 3;
                    else
                        sdoSendFrmExam.PartExamEyeBlindColor = null;
                }

                sdoSendFrmExam.PartExamEye = !string.IsNullOrEmpty(txtMat.Text.Trim()) ? txtMat.Text.Trim() : null;

                sdoSendFrmExam.PartExamUpperJaw = !string.IsNullOrEmpty(txtPART_EXAM_UPPER_JAW.Text.Trim()) ? txtPART_EXAM_UPPER_JAW.Text.Trim() : null;
                sdoSendFrmExam.PartExamLowerJaw = !string.IsNullOrEmpty(txtPART_EXAM_LOWER_JAW.Text.Trim()) ? txtPART_EXAM_LOWER_JAW.Text.Trim() : null;
                sdoSendFrmExam.PartExamStomatology = !string.IsNullOrEmpty(txtRHM.Text.Trim()) ? txtRHM.Text.Trim() : null;

                sdoSendFrmExam.PartExamNose = !string.IsNullOrEmpty(txtMui.Text.Trim()) ? txtMui.Text.Trim() : null;
                sdoSendFrmExam.PartExamThroat = !string.IsNullOrEmpty(txtHong.Text.Trim()) ? txtHong.Text.Trim() : null;
                sdoSendFrmExam.PartExamEyeTensionRight = !string.IsNullOrEmpty(txtNhanApPhai.Text.Trim()) ? txtNhanApPhai.Text.Trim() : null;
                sdoSendFrmExam.PartExamEyeTensionLeft = !string.IsNullOrEmpty(txtNhanApTrai.Text.Trim()) ? txtNhanApTrai.Text.Trim() : null;
                sdoSendFrmExam.PartExamEyeSightRight = !string.IsNullOrEmpty(txtThiLucKhongKinhPhai.Text.Trim()) ? txtThiLucKhongKinhPhai.Text.Trim() : null;
                sdoSendFrmExam.PartExamEyeSightLeft = !string.IsNullOrEmpty(txtThiLucKhongKinhTrai.Text.Trim()) ? txtThiLucKhongKinhTrai.Text.Trim() : null;
                sdoSendFrmExam.PartExamEyeSightGlassRight = !string.IsNullOrEmpty(txtThiLucCoKinhPhai.Text.Trim()) ? txtThiLucCoKinhPhai.Text.Trim() : null;
                sdoSendFrmExam.PartExamEyeSightGlassLeft = !string.IsNullOrEmpty(txtThiLucCoKinhTrai.Text.Trim()) ? txtThiLucCoKinhTrai.Text.Trim() : null;
                sdoSendFrmExam.PartExamKidneyUrology = !string.IsNullOrEmpty(txtThanTietNieu.Text.Trim()) ? txtThanTietNieu.Text.Trim() : null;

                sdoSendFrmExam.PartExamDermatology = !string.IsNullOrEmpty(txtDaLieu.Text.Trim()) ? txtDaLieu.Text.Trim() : null;

                sdoSendFrmExam.PartExamMental = !string.IsNullOrEmpty(txtPartExamMental.Text.Trim()) ? txtPartExamMental.Text.Trim() : null;
                sdoSendFrmExam.PartExamNutrition = !string.IsNullOrEmpty(txtPartExamNutrition.Text.Trim()) ? txtPartExamNutrition.Text.Trim() : null;
                sdoSendFrmExam.PartExamMotion = !string.IsNullOrEmpty(txtPartExamMotion.Text.Trim()) ? txtPartExamMotion.Text.Trim() : null;
                sdoSendFrmExam.PartExamObstetric = !string.IsNullOrEmpty(txtPartExanObstetric.Text.Trim()) ? txtPartExanObstetric.Text.Trim() : null;
                sdoSendFrmExam.PartExamMuscleBone = !string.IsNullOrEmpty(txtCoXuongKhop.Text.Trim()) ? txtCoXuongKhop.Text.Trim() : null;
                sdoSendFrmExam.PartExamNeurological = !string.IsNullOrEmpty(txtThanKinh.Text.Trim()) ? txtThanKinh.Text.Trim() : null;
                sdoSendFrmExam.PartExamOend = !string.IsNullOrEmpty(txtNoiTiet.Text.Trim()) ? txtNoiTiet.Text.Trim() : null;
                sdoSendFrmExam.PartExamRespiratory = !string.IsNullOrEmpty(txtHoHap.Text.Trim()) ? txtHoHap.Text.Trim() : null;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetSDOExam(HisServiceReqExamUpdateSDO examServiceReqUpdateSDO)
        {
            try
            {
                txtKhamBoPhan.Text = examServiceReqUpdateSDO.PartExam;
                txtTuanHoan.Text = examServiceReqUpdateSDO.PartExamCirculation;
                txtTieuHoa.Text = examServiceReqUpdateSDO.PartExamDigestion;
                txtTai.Text = examServiceReqUpdateSDO.PartExamEar;
                txtPART_EXAM_EAR_RIGHT_NORMAL.Text = examServiceReqUpdateSDO.PartExamEarRightNormal;
                txtPART_EXAM_EAR_RIGHT_WHISPER.Text = examServiceReqUpdateSDO.PartExamEarRightWhisper;
                txtPART_EXAM_EAR_LEFT_NORMAL.Text = examServiceReqUpdateSDO.PartExamEarLeftNormal;
                txtPART_EXAM_EAR_LEFT_WHISPER.Text = examServiceReqUpdateSDO.PartExamEarLeftWhisper;
                chkPART_EXAM_HORIZONTAL_SIGHT__BT.Checked = examServiceReqUpdateSDO.PartExamHorizontalSight == 1;
                chkPART_EXAM_HORIZONTAL_SIGHT__HC.Checked = examServiceReqUpdateSDO.PartExamHorizontalSight == 2;
                chkPART_EXAM_VERTICAL_SIGHT__BT.Checked = examServiceReqUpdateSDO.PartExamVerticalSight == 1;
                chkPART_EXAM_VERTICAL_SIGHT__HC.Checked = examServiceReqUpdateSDO.PartExamVerticalSight == 2;

                switch (examServiceReqUpdateSDO.PartExamEyeBlindColor)
                {
                    case 1:
                        chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked = examServiceReqUpdateSDO.PartExamEyeBlindColor == 1;
                        break;
                    case 2:
                        chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked = examServiceReqUpdateSDO.PartExamEyeBlindColor == 2;
                        break;
                    case 3:
                        chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = examServiceReqUpdateSDO.PartExamEyeBlindColor == 3;
                        break;
                    case 4:
                        chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = examServiceReqUpdateSDO.PartExamEyeBlindColor == 4;
                        break;
                    case 5:
                        chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = examServiceReqUpdateSDO.PartExamEyeBlindColor == 5;
                        break;
                    case 6:
                        chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = examServiceReqUpdateSDO.PartExamEyeBlindColor == 6;
                        break;
                    case 7:
                        chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = examServiceReqUpdateSDO.PartExamEyeBlindColor == 7;
                        break;
                    case 8:
                        chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = examServiceReqUpdateSDO.PartExamEyeBlindColor == 8;
                        break;
                    case 9:
                        chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = examServiceReqUpdateSDO.PartExamEyeBlindColor == 9;
                        break;
                    default:
                        chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked = false;
                        break;
                }
                txtMat.Text = examServiceReqUpdateSDO.PartExamEye;

                txtPART_EXAM_UPPER_JAW.Text = examServiceReqUpdateSDO.PartExamUpperJaw;
                txtPART_EXAM_LOWER_JAW.Text = examServiceReqUpdateSDO.PartExamLowerJaw;
                txtRHM.Text = examServiceReqUpdateSDO.PartExamStomatology;

                txtMui.Text = examServiceReqUpdateSDO.PartExamNose;
                txtHong.Text = examServiceReqUpdateSDO.PartExamThroat;
                txtNhanApPhai.Text = examServiceReqUpdateSDO.PartExamEyeTensionRight;
                txtNhanApTrai.Text = examServiceReqUpdateSDO.PartExamEyeTensionLeft;
                txtThiLucKhongKinhPhai.Text = examServiceReqUpdateSDO.PartExamEyeSightRight;
                txtThiLucKhongKinhTrai.Text = examServiceReqUpdateSDO.PartExamEyeSightLeft;
                txtThiLucCoKinhPhai.Text = examServiceReqUpdateSDO.PartExamEyeSightGlassRight;
                txtThiLucCoKinhTrai.Text = examServiceReqUpdateSDO.PartExamEyeSightGlassLeft;
                txtThanTietNieu.Text = examServiceReqUpdateSDO.PartExamKidneyUrology;

                txtDaLieu.Text = examServiceReqUpdateSDO.PartExamDermatology;

                txtPartExamMental.Text = examServiceReqUpdateSDO.PartExamMental;
                txtPartExamNutrition.Text = examServiceReqUpdateSDO.PartExamNutrition;
                txtPartExamMotion.Text = examServiceReqUpdateSDO.PartExamMotion;
                txtPartExanObstetric.Text = examServiceReqUpdateSDO.PartExamObstetric;
                txtCoXuongKhop.Text = examServiceReqUpdateSDO.PartExamMuscleBone;
                txtThanKinh.Text = examServiceReqUpdateSDO.PartExamNeurological;
                txtNoiTiet.Text = examServiceReqUpdateSDO.PartExamOend;
                txtHoHap.Text = examServiceReqUpdateSDO.PartExamRespiratory;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
