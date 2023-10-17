﻿using ACS.EFMODEL.DataModels;
using ACS.SDO;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using EMR.TDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ADO;
using HIS.Desktop.Plugins.ReturnMicrobiologicalResults.Config;
using HIS.Desktop.Plugins.ReturnMicrobiologicalResults.Sda.SdaEventLogCreate;
using HIS.Desktop.Plugins.ReturnMicrobiologicalResults.Validation;
using HIS.Desktop.Utilities.Extentions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using LIS.SDO;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ReturnMicrobiologicalResults
{
    public partial class UC_ReturnMicrobiologicalResults : UserControlBase
    {
        #region Declare

        public HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        public List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        SignConfigADO SignConfigData;

        V_HIS_ROOM room = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        internal Inventec.Core.ApiResultObject<List<V_LIS_SAMPLE>> apiResult;
        internal List<V_LIS_SAMPLE> currentSample { get; set; }
        internal V_LIS_SAMPLE rowSample { get; set; }
        HIS_EMPLOYEE currentEmployee = null;
        TestLisResultADO currentTestSamResultADO;
        int lastRowHandle = -1;
        internal V_LIS_RESULT currentLisResult { get; set; }
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int limit = 0;
        bool IsLoginameAddmin;
        NumberStyles style;
        internal List<LisResultADO> _LisResults = new List<LisResultADO>();
        List<V_LIS_SAMPLE_SERVICE> sampleServices = new List<V_LIS_SAMPLE_SERVICE>();
        internal HIS_SERVICE_REQ currentServiceReq = new HIS_SERVICE_REQ();
        internal List<TestLisResultADO> lstCheckPrint = new List<TestLisResultADO>();
        List<TestLisResultADO> testLisResultADODetails = new List<TestLisResultADO>();
        internal List<LIS_MACHINE> _Machines { get; set; }
        HideCheckBoxHelper hideCheckBoxHelper__Service;
        BindingList<TestLisResultADO> records;
        public PRINT_OPTION PrintOption { get; set; }
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        List<TestLisResultADO> lstSampleServiceADOs;
        List<TestLisResultADO> lstLisResultDetailForTreeADOs;
        BarManager baManager = null;
        PopupMenuProcessor popupMenuProcessor = null;
        internal PopupMenu _Menu = null;
        List<LIS_ANTIBIOTIC_RANGE> lstAntibioticRange = null;
        List<LIS_ANTIBIOTIC> datasAntibiotic = null;
        List<LIS_BACTERIUM> datasBacterium = null;
        DateTime currentTimer;
        DateTime currentTimerLM;
        TimerSDO timeSync { get; set; }

        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentBySessionControlStateRDO;
        bool isInit = true;
        #endregion

        #region Contructor

        public UC_ReturnMicrobiologicalResults()
            : base()
        {
            InitializeComponent();
            LisConfigCFG.LoadConfig();
            HisConfigCFG.LoadConfig();
        }

        public UC_ReturnMicrobiologicalResults(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                LisConfigCFG.LoadConfig();
                HisConfigCFG.LoadConfig();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UC_ReturnMicrobiologicalResults_Load(object sender, EventArgs e)
        {
            try
            {
                InitBacterium();
                InitAntibiotic();
                LoadUser();
                InitControlState();
                lstAntibioticRange = GetlstAntibioticRange();
                VisibleColumnSample();
                GetControlAcs();
                LoadDataToCombo();
                LoadDefaultData();
                this.gridControlSample.ToolTipController = this.toolTipControllerGrid;
                this.treeListSereServTein.ToolTipController = this.toolTipController1;
                GetNewestBarcode();
                FillDataToGridControl();
                LoadCboMachine();
                SetImageToButtonEditSTT();
                IsLoginameAddmin = CheckEmployIsAdmin();
                style = NumberStyles.Any;
                isInit = false;
                GetTimeSystem();
                RegisterTimer(currentModule.ModuleLink, "timer1", timer1.Interval, timer1_Tick);
                StartTimer(currentModule.ModuleLink, "timer1");
                RegisterTimer(currentModule.ModuleLink, "timer2", timer2.Interval, timer2_Tick);
                StartTimer(currentModule.ModuleLink, "timer2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GetTimeSystem()
        {
            try
            {
                timeSync = new BackendAdapter(new CommonParam()).Get<TimerSDO>(AcsRequestUriStore.ACS_TIMER__SYNC, ApiConsumers.AcsConsumer, 1, new CommonParam());
                currentTimer = currentTimerLM = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(timeSync.LocalTime) ?? DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void timer1_Tick()
        {
            try
            {
                if (dtResultTime.SelectionStart > 0)
                {
                    StopTimer(currentModule.ModuleLink, "timer1");
                }
                else
                {
                    currentTimer = currentTimer.AddSeconds(1);
                    if ((rowSample != null && !rowSample.RESULT_TIME.HasValue) || rowSample == null)
                    {
                        dtResultTime.DateTime = currentTimer;
                    }
                }
                dtResultTime.ToolTip = ConvertStringTime(dtResultTime);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void timer2_Tick()
        {
            try
            {
                if (dtSampleTime.SelectionStart > 0)
                {
                    StopTimer(currentModule.ModuleLink, "timer2");
                }
                else
                {
                    currentTimerLM = currentTimerLM.AddSeconds(1);
                    if ((rowSample != null && !rowSample.SAMPLE_TIME.HasValue) || rowSample == null)
                    {
                        dtSampleTime.DateTime = currentTimerLM;
                    }
                }
                dtSampleTime.ToolTip = ConvertStringTime(dtSampleTime);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string ConvertStringTime(DateEdit dte)
        {
            string rs = null;
            try
            {
                if (dte.EditValue != null && dte.DateTime != DateTime.MinValue)
                {
                    var timeNum = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dte.DateTime).ToString();
                    rs = timeNum.Substring(6, 2) + "/" + timeNum.Substring(4, 2) + "/" + timeNum.Substring(0, 4) + " " + timeNum.Substring(8, 2) + ":" + timeNum.Substring(10, 2) + ":" + timeNum.Substring(12, 2);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
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
                MOS.Filter.HisUserRoomFilter filter = new HisUserRoomFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);

                ControlEditorLoader.Load(cboResultUsername, listResult, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void VisibleColumnSample()
        {
            try
            {
                if (LisConfigCFG.SHOW_BUTTON_APPROVE == "1")
                {
                    gc_ApproveResult.Visible = true;
                    gc_ApproveSample.Visible = true;
                    gc_Reject.Visible = true;
                }
                else
                {
                    gc_ApproveResult.Visible = LisConfigCFG.MUST_APPROVE_RESULT == "1";
                    gc_ApproveSample.Visible = false;
                    gc_Reject.Visible = false;
                }

                if (LisConfigCFG.MUST_TICK_RUN_AGAIN == "1")
                {
                    treeListColumn_ReRun.Visible = true;
                }
                else
                {
                    treeListColumn_ReRun.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
        bool CheckEmployIsAdmin()
        {
            bool result = false;
            try
            {
                if (currentEmployee == null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisEmployeeFilter hisEmployeeFilter = new HisEmployeeFilter();
                    hisEmployeeFilter.LOGINNAME__EXACT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    currentEmployee = new BackendAdapter(param).Get<List<HIS_EMPLOYEE>>("api/HisEmployee/Get", ApiConsumer.ApiConsumers.MosConsumer, hisEmployeeFilter, param).FirstOrDefault();
                }
                if (currentEmployee != null && currentEmployee.IS_ADMIN == 1)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        void SetImageToButtonEditSTT()
        {
            try
            {
                ButtonEdit_ChuaCoKQSTT.Buttons[0].Image = imageList1.Images[0];
                ButtonEdit_DaCoKQSTT.Buttons[0].Image = imageList1.Images[1];
                ButtonEdit_DaTraKQSTT.Buttons[0].Image = imageList1.Images[2];
                ButtonEdit_DangChayXN.Buttons[0].Image = imageList1.Images[3];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationDateResultTime()
        {
            try
            {
                Validation.ValidateTimeResult rule = new Validation.ValidateTimeResult();
                rule.dteKq = dtResultTime;
                rule.dteLm = dtSampleTime;
                this.dxValidationProvider1.SetValidationRule(dtResultTime, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region Gridview Sample

        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlSample)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlSample.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "STATUS")
                            {

                                //text = (view.GetRowCellValue(lastRowHandle, "SAMPLE_STT_NAME") ?? "").ToString();
                                var busyCount = ((V_LIS_SAMPLE)view.GetRow(lastRowHandle)).SAMPLE_STT_ID;
                                if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                                {
                                    text = "Chưa lấy mẫu";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM)
                                {
                                    text = "Đã lấy mẫu";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ)
                                {
                                    text = "Có kết quả";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                                {
                                    text = "Đã trả kết quả";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                                {
                                    text = "Từ chối mẫu";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                                {
                                    text = "Chấp nhận mẫu";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                                {
                                    text = "Duyệt kết quả";
                                }
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSample_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                string Note = mmNote.Text;
                WaitingManager.Show();
                rowSample = null;
                rowSample = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                LoadLisResult(rowSample);
                LoadDataToGridTestResultWithVKKS();
                DataLoadUser(rowSample);
                if (this.lstSampleServiceADOs != null && this.lstSampleServiceADOs.Count > 0)
                {
                    this.currentTestSamResultADO = this.lstSampleServiceADOs[0];
                    LoadDataToTreeTestResult2();
                    SetDataToCommon(rowSample);
                }
                if (sender == null && e == null)
                {
                    mmNote.Text = Note;
                }
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                {
                    btnSave.Enabled = false;
                }
                else
                {
                    btnSave.Enabled = true;
                }
                dxValidationProvider1.RemoveControlError(dtResultTime);

                //btnPrint.Enabled = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private async Task DataLoadUser(V_LIS_SAMPLE sample)
        {

            try
            {
                ACS.EFMODEL.DataModels.ACS_USER acsUser = new ACS_USER();
                if (sample.RESULT_LOGINNAME != null)
                {
                    cboResultUsername.EditValue = sample.RESULT_LOGINNAME;
                    acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboResultUsername.EditValue.ToString());

                    if (acsUser != null)
                    {
                        txtResultLoginName.Text = acsUser.LOGINNAME;
                    }
                }
                else
                {
                    this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();

                    this.currentBySessionControlStateRDO = controlStateWorker.GetDataBySession(ControlStateConstant.MODULE_LINK);
                    if (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0)
                    {

                        foreach (var item_ in this.currentBySessionControlStateRDO)
                        {
                            if (item_.KEY == cboResultUsername.Name)
                            {
                                if (!String.IsNullOrEmpty(item_.VALUE))
                                {
                                    cboResultUsername.EditValue = item_.VALUE;
                                }
                            }
                        }
                    }
                    else
                    {
                        cboResultUsername.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                        txtResultLoginName.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    }
                }
                GetTimeSystem();
                if (sample.RESULT_TIME != null)
                {
                    dtResultTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sample.RESULT_TIME ?? 0) ?? DateTime.Now;
                }
                else
                {
                    dtResultTime.EditValue = currentTimer;
                }
                if (sample.SAMPLE_TIME != null)
                {
                    dtSampleTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sample.SAMPLE_TIME ?? 0) ?? DateTime.Now;
                }
                else
                {
                    dtSampleTime.EditValue = currentTimerLM;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void LoadLisResult(V_LIS_SAMPLE data)
        {
            try
            {
                CommonParam param = new CommonParam();
                _LisResults = new List<LisResultADO>();
                if (data != null)
                {
                    LIS.Filter.LisResultViewFilter resultFilter = new LisResultViewFilter();
                    resultFilter.SAMPLE_ID = data.ID;

                    var lstResult = new BackendAdapter(param).Get<List<V_LIS_RESULT>>("api/LisResult/GetView", ApiConsumer.ApiConsumers.LisConsumer, resultFilter, param);
                    if (lstResult != null && lstResult.Count > 0)
                    {
                        lstResult = lstResult.Where(o => !String.IsNullOrEmpty(o.BACTERIUM_CODE)).ToList();
                        var Groups = lstResult.GroupBy(g => new { g.SERVICE_CODE, g.BACTERIUM_CODE }).ToList();
                        AutoMapper.Mapper.CreateMap<V_LIS_RESULT, LisResultADO>();
                        foreach (var g in Groups)
                        {
                            List<LisResultADO> listAdo = new List<LisResultADO>();
                            foreach (V_LIS_RESULT rs in g.ToList())
                            {
                                LisResultADO ado = AutoMapper.Mapper.Map<LisResultADO>(rs);
                                var anti = BackendDataWorker.Get<LIS_ANTIBIOTIC>().Where(o => o.ANTIBIOTIC_CODE == rs.ANTIBIOTIC_CODE).FirstOrDefault();
                                ado.ANTIBIOTIC_ORDER = anti != null ? anti.NUM_ORDER ?? 999999 : 999999;
                                listAdo.Add(ado);
                            }
                            listAdo = listAdo.OrderBy(o => o.ANTIBIOTIC_ORDER).ToList();
                            _LisResults.AddRange(listAdo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToCommon(V_LIS_SAMPLE data)
        {
            try
            {
                if (data != null)
                {
                    lblPatientName.Text = data.LAST_NAME + " " + data.FIRST_NAME;
                    lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB ?? 0);
                    lblGenderName.Text = (data.GENDER_CODE == "01" ? "Nữ" : "Nam");
                    lblPatientCode.Text = data.PATIENT_CODE;
                    mmNote.Text = rowSample.NOTE;
                }
                else
                {
                    lblPatientName.Text = "";
                    lblDob.Text = "";
                    lblGenderName.Text = "";
                    lblPatientCode.Text = "";
                    mmNote.Text = "";
                }
                dxValidationProvider1.RemoveControlError(mmNote);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSample_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var sampleStt = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSample.GetRowCellValue(e.RowHandle, "SAMPLE_STT_ID")).ToString());
                    var data = (V_LIS_SAMPLE)gridViewSample.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "KET_QUA")
                    {
                        if (LisConfigCFG.MUST_APPROVE_RESULT == "1")
                        {
                            if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)// if (sampleStt == 1 || sampleStt == 2)
                            {
                                e.RepositoryItem = TraKetQuaE;
                            }
                            else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                            {
                                if (((controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnHuyTraKQ) != null) || IsLoginameAddmin))
                                {
                                    e.RepositoryItem = HuyTraKQE;
                                }
                                else
                                {
                                    e.RepositoryItem = HuyTraKQD;
                                }
                            }
                            else
                            {
                                e.RepositoryItem = TraKetQuaD;
                            }
                        }
                        else
                        {
                            if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                               || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                                || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ
                                || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)// if (sampleStt == 1 || sampleStt == 2)
                            {
                                e.RepositoryItem = TraKetQuaE;
                            }
                            else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ
                                && ((controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnHuyTraKQ) != null) || IsLoginameAddmin))
                            {
                                e.RepositoryItem = HuyTraKQE;
                            }
                            else
                            {
                                e.RepositoryItem = TraKetQuaD;
                            }
                        }
                    }
                    else if (e.Column.FieldName == "DUYET")
                    {
                        if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                        {
                            e.RepositoryItem = DuyetE;
                        }
                        else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                        {
                            e.RepositoryItem = HuyDuyetE;
                        }
                        else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                        {
                            e.RepositoryItem = HuyDuyetD;
                        }
                        else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                        {
                            if (LisConfigCFG.MUST_APPROVE_RESULT == "1" || !CheckEmployIsAdmin())
                            {
                                e.RepositoryItem = HuyDuyetD;
                            }
                            else
                            {
                                e.RepositoryItem = HuyDuyetE;
                            }
                        }
                        else
                        {
                            if (CheckEmployIsAdmin())
                            {
                                e.RepositoryItem = HuyDuyetE;
                            }
                            else
                            {
                                e.RepositoryItem = HuyDuyetD;
                            }
                        }
                    }
                    else if (e.Column.FieldName == "UPDATE_BARCODE_TIME")
                    {
                        if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                        {
                            e.RepositoryItem = repositoryItemBtnUpdateBarcodeTime_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnUpdateBarcodeTime_Disable;
                        }
                    }
                    else if (e.Column.FieldName == "NUM_ORDER")
                    {
                        if (data != null && data.NUM_ORDER.HasValue)
                        {
                            e.RepositoryItem = repositoryItemSpinNumOrder;
                        }
                        else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                        {
                            e.RepositoryItem = repositoryItemBtnUpdateNumOrder;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnUpdateNumOrderDisable;
                        }
                    }
                    else if (e.Column.FieldName == "BARCODE")
                    {
                        if (LisConfigCFG.IS_AUTO_CREATE_BARCODE != "1" && sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                        {
                            e.RepositoryItem = ItemTextEdit_BarCodeEnable;
                        }
                        else
                        {
                            e.RepositoryItem = ItemTextEdit_BarCode_Disable;
                        }
                    }
                    else if (e.Column.FieldName == "REJECT")
                    {
                        if ((data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                            || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN))
                        {
                            e.RepositoryItem = repositoryTuChoiMauE;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryTuChoiMauD;
                        }
                    }
                    else if (e.Column.FieldName == "APPROVE_SAMPLE")
                    {
                        if (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM)
                        {
                            e.RepositoryItem = repositoryChapNhanMauE;
                        }
                        else if (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                        {
                            e.RepositoryItem = repUnAppove;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryChapNhanMauD;
                        }
                    }
                    else if (e.Column.FieldName == "APPROVE_RESULT")
                    {
                        if (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                            || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                        {
                            e.RepositoryItem = repositoryDuyetKetQuaE;
                        }
                        else if (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                        {
                            e.RepositoryItem = repositoryHuyDuyetKetQuaE;
                        }
                        else if (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                        {
                            e.RepositoryItem = repositoryHuyDuyetKetQuaD;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryDuyetKetQuaD;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSample_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_LIS_SAMPLE data = (V_LIS_SAMPLE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "STATUS")
                        {
                            //Chua lấy mẫu: mau trang
                            //Đã lấy mẫu: mau vang
                            //Đã có kết quả: mau cam
                            //Đã trả kết quả: mau do
                            long statusId = data.SAMPLE_STT_ID;
                            if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                            {
                                e.Value = imageListIcon.Images[0];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM)
                            {
                                e.Value = imageListIcon.Images[1];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ)
                            {
                                e.Value = imageListIcon.Images[2];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                            {
                                e.Value = imageListIcon.Images[3];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                            {
                                e.Value = imageListIcon.Images[5];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                            {
                                e.Value = imageListIcon.Images[11];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                            {
                                e.Value = imageListIcon.Images[10];
                            }
                        }
                        else if (e.Column.FieldName == "PATIENT_NAME")
                        {
                            e.Value = data.LAST_NAME + " " + data.FIRST_NAME;
                        }
                        else if (e.Column.FieldName == "SAMPLE_LOGINNAME_STR")
                        {
                            e.Value = data.SAMPLE_USERNAME;
                        }
                        else if (e.Column.FieldName == "SAMPLE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.SAMPLE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_LOGINNAME_STR")
                        {
                            e.Value = data.APPROVAL_USERNAME;
                        }
                        else if (e.Column.FieldName == "APPROVAL_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TDL_GENDER_NAME")
                        {
                            e.Value = (data.GENDER_CODE == "01" ? "Nữ" : "Nam");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Method

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ControlStateConstant.MODULE_LINK);
                this.currentBySessionControlStateRDO = controlStateWorker.GetDataBySession(ControlStateConstant.MODULE_LINK);
                if (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0)
                {
                    foreach (var item_ in this.currentBySessionControlStateRDO)
                    {
                        if (item_.KEY == cboResultUsername.Name)
                        {
                            if (!String.IsNullOrEmpty(item_.VALUE))
                            {
                                cboResultUsername.EditValue = item_.VALUE;
                            }
                        }
                    }
                }
                else
                {
                    cboResultUsername.EditValue = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                    txtResultLoginName.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                }

                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.CHECK_SIGN)
                        {
                            chkSign.Checked = item.VALUE == "1";
                        }

                        else if (item.KEY == ControlStateConstant.CHECK_SIGN_PROCESS)
                        {
                            chkSignProcess.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == btnCreateSigner.Name)
                        {
                            SignConfigData = !String.IsNullOrWhiteSpace(item.VALUE) ? Newtonsoft.Json.JsonConvert.DeserializeObject<SignConfigADO>(item.VALUE) : null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillDataToGridControl()
        {
            txtSearchKey.Focus();
            txtSearchKey.SelectAll();

            InitRestoreLayoutGridViewFromXml(gridViewSample);

            this.treeListSereServTein.DataSource = null;
            int pageSize = 0;
            if (ucPaging1.pagingGrid != null)
            {
                pageSize = ucPaging1.pagingGrid.PageSize;
            }
            else
            {
                pageSize = (int)ConfigApplications.NumPageSize;
            }
            FillDataToGridSample(new CommonParam(0, pageSize));

            CommonParam param = new CommonParam();
            param.Limit = rowCount;
            param.Count = dataTotal;
            ucPaging1.Init(FillDataToGridSample, param, pageSize, gridControlSample);
        }

        internal void GetNewestBarcode()
        {
            try
            {
                if (dtBarcodeTimefrom.EditValue != null && dtBarcodeTimefrom.DateTime != DateTime.MinValue && dtBarcodeTimeTo.EditValue != null && dtBarcodeTimeTo.DateTime != DateTime.MinValue)
                {
                    CommonParam paramCommon = new CommonParam();
                    List<long> filter = new List<long>();

                    long fromTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtBarcodeTimefrom.EditValue).ToString("yyyyMMdd") + "000000");
                    filter.Add(fromTime);

                    long toTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtBarcodeTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                    filter.Add(toTime);
                    var result = new BackendAdapter(paramCommon).Get<LIS_SAMPLE>("api/LisSample/GetByBarcodeLatest", ApiConsumer.ApiConsumers.LisConsumer, filter, paramCommon);
                    if (result != null)
                    {
                        lblNewestBarcode.Text = result.BARCODE;
                    }
                    else
                    {
                        lblNewestBarcode.Text = "00";
                    }
                }
                else
                {
                    lblNewestBarcode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillDataToGridSample(object param)
        {
            try
            {
                WaitingManager.Show();
                startPage = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                gridControlSample.DataSource = null;

                LisSampleViewFilter lisSampleFilter = new LisSampleViewFilter();
                if (room == null)
                {
                    room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId);
                }
                if (room != null)
                {
                    lisSampleFilter.EXECUTE_ROOM_CODE__EXACT = room.ROOM_CODE;
                }
                if (!String.IsNullOrEmpty(txtSearchKey.Text))
                {
                    lisSampleFilter.KEY_WORD = txtSearchKey.Text.Trim();
                }
                if (!String.IsNullOrEmpty(txtSERVICE_REQ_CODE__EXACT.Text))
                {
                    string code = txtSERVICE_REQ_CODE__EXACT.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    }
                    lisSampleFilter.SERVICE_REQ_CODE__EXACT = code;
                    txtSERVICE_REQ_CODE__EXACT.Text = code;
                }
                if (dtBarcodeTimefrom != null && dtBarcodeTimefrom.DateTime != DateTime.MinValue)
                    lisSampleFilter.BARCODE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtBarcodeTimefrom.EditValue).ToString("yyyyMMdd") + "000000");
                if (dtBarcodeTimeTo != null && dtBarcodeTimeTo.DateTime != DateTime.MinValue)
                    lisSampleFilter.BARCODE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtBarcodeTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                lisSampleFilter.ORDER_FIELD = "BARCODE_TIME";
                lisSampleFilter.ORDER_DIRECTION = "DESC";

                List<long> lstTestServiceReqSTT = new List<long>();

                //Tất cả 0
                //Chưa lấy mẫu 1
                //Đã lấy mẫu 2
                //Đã có kết quả
                //Đã trả kết quả
                //Filter yeu cau chua lấy mẫu
                if (cboFind.EditValue != null)
                {
                    //Chưa lấy mẫu
                    if ((long)cboFind.EditValue == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                    {
                        lisSampleFilter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI
                            };
                    }
                    //Đã lấy mẫu
                    else if ((long)cboFind.EditValue == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM)
                    {
                        lisSampleFilter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN
                            };
                    }
                    //đã có kết quả
                    else if ((long)cboFind.EditValue == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ)
                    {
                        lisSampleFilter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ
                            };
                    }//đã trả kết quả
                    else if ((long)cboFind.EditValue == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                    {
                        lisSampleFilter.SAMPLE_STT_ID = IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ;
                    }
                    //Chấp nhận mẫu
                    else if ((long)cboFind.EditValue == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                    {
                        lisSampleFilter.SAMPLE_STT_ID = IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN;
                    }
                    else if ((long)cboFind.EditValue == 999) //Chưa trả kq
                    {
                        List<long> sampleSttIds = new List<long>();
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM);//chưa lay mau
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ);
                        lisSampleFilter.SAMPLE_STT_IDs = sampleSttIds;
                    }
                    //Tất cả
                    else
                    {
                        lisSampleFilter.SAMPLE_STT_ID = null;
                    }
                }
                lisSampleFilter.IS_ANTIBIOTIC_RESISTANCE = true;
                apiResult = new ApiResultObject<List<V_LIS_SAMPLE>>();

                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumer.ApiConsumers.LisConsumer, lisSampleFilter, paramCommon);
                gridControlSample.DataSource = null;

                if (apiResult != null)
                {
                    WaitingManager.Hide();
                    var data = (List<V_LIS_SAMPLE>)apiResult.Data;
                    if (data != null)
                    {
                        gridControlSample.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    #region Process has exception
                    SessionManager.ProcessTokenLost((CommonParam)param);
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                List<HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ComboADO> status = new List<HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ComboADO>();
                status.Add(new HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ComboADO(999, "Chưa trả kết quả"));
                status.Add(new HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ComboADO(0, "Tất cả"));
                status.Add(new HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM, "Chưa lấy mẫu"));
                status.Add(new HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM, "Đã lấy mẫu"));
                status.Add(new HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ, "Có kết quả"));
                status.Add(new HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ, "Trả kết quả"));
                status.Add(new HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN, "Chấp nhận mẫu"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "Trạng thái", 50, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
                ControlEditorLoader.Load(cboFind, status, controlEditorADO);

                cboFind.EditValue = status[0].id;


                List<HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ComboADO> results = new List<HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ComboADO>();
                results.Add(new HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ComboADO(0, "Âm tính"));
                results.Add(new HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ComboADO(1, "Dương tính"));
                results.Add(new HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ComboADO(-1, ""));

                List<ColumnInfo> columnInfosResult = new List<ColumnInfo>();
                columnInfosResult.Add(new ColumnInfo("statusName", "Kết quả", 50, 2));
                ControlEditorADO controlEditorADOResult = new ControlEditorADO("statusName", "id", columnInfosResult, false, 50);
                ControlEditorLoader.Load(repositoryItemcboResult, results, controlEditorADOResult);


                List<ColumnInfo> columnInfosMachine = new List<ColumnInfo>();
                columnInfosMachine.Add(new ColumnInfo("MACHINE_CODE", "Mã máy", 50, 2));
                columnInfosMachine.Add(new ColumnInfo("MACHINE_NAME", "Tên máy", 100, 2));
                ControlEditorADO controlEditorADOMachine = new ControlEditorADO("MACHINE_NAME", "ID", columnInfosMachine, false, 150);
                ControlEditorLoader.Load(repositoryItemcboMachineReturnResult, BackendDataWorker.Get<LIS_MACHINE>(), controlEditorADOMachine);

                List<ColumnInfo> columnInfosMachineForDetail = new List<ColumnInfo>();
                columnInfosMachineForDetail.Add(new ColumnInfo("MACHINE_CODE", "Mã máy", 50, 2));
                columnInfosMachineForDetail.Add(new ColumnInfo("MACHINE_NAME", "Tên máy", 100, 2));
                ControlEditorADO controlEditorADOMachineForDetail = new ControlEditorADO("MACHINE_NAME", "ID", columnInfosMachineForDetail, false, 150);
                ControlEditorLoader.Load(repositoryItemcboMachineForDetail, BackendDataWorker.Get<LIS_MACHINE>(), controlEditorADOMachineForDetail);

                List<ColumnInfo> columnTechnique = new List<ColumnInfo>();
                columnTechnique.Add(new ColumnInfo("TECHNIQUE_CODE", "", 50, 1));
                columnTechnique.Add(new ColumnInfo("TECHNIQUE_NAME", "", 150, 2));
                ControlEditorADO controlEditorADOTechnique = new ControlEditorADO("TECHNIQUE_NAME", "ID", columnTechnique, false, 150);
                ControlEditorLoader.Load(repositoryItemCboTechnique, BackendDataWorker.Get<LIS_TECHNIQUE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), controlEditorADOTechnique);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadDefaultData()
        {
            try
            {
                txtSearchKey.Focus();
                txtSearchKey.SelectAll();
                dtBarcodeTimefrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                dtBarcodeTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                gridControlSample.DataSource = null;
                treeListSereServTein.DataSource = null;
                repositoryItemTextEditReadOnly.ReadOnly = true;
                repositoryItemTextEditReadOnly.Enabled = false;
                mmNote.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region UpdateStt
        private void UpdateStt(long sampleSTT)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                LIS.SDO.UpdateSampleSttSDO updateStt = new LIS.SDO.UpdateSampleSttSDO();
                var row = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                if (row != null)
                {
                    updateStt.Id = row.ID;
                    updateStt.SampleSttId = sampleSTT;

                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("/api/LisSample/UpdateStt", ApiConsumer.ApiConsumers.LisConsumer, updateStt, param);
                    if (curentSTT != null)
                    {
                        result = true;
                        FillDataToGridControl();
                        WaitingManager.Hide();
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #endregion

        #region Event Button Sample

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    GetNewestBarcode();
                    FillDataToGridControl();
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

        private void InBarcode_Click(object sender, EventArgs e)
        {
            try
            {
                onClickBtnPrintBarCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DuyetE_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                var row = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                if (row != null && (row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI))
                {
                    if (LisConfigCFG.SHOW_FORM_SAMPLE_INFO == "1")
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();

                        rowSample = null;
                        FillDataToGridControl();
                        gridViewSample.RefreshData();
                    }
                    else
                    {
                        WaitingManager.Show();
                        LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                        sdo.SampleId = row.ID;
                        sdo.RequestRoomCode = room.ROOM_CODE;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        if (curentSTT != null)
                        {
                            rowSample = row;
                            rowSample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                            rowSample.SAMPLE_TYPE_ID = curentSTT.SAMPLE_TYPE_ID;
                            rowSample.SAMPLE_TIME = curentSTT.SAMPLE_TIME;
                            rowSample.SAMPLE_LOGINNAME = curentSTT.SAMPLE_LOGINNAME;
                            rowSample.SAMPLE_USERNAME = curentSTT.SAMPLE_USERNAME;
                            FillDataToGridControl();
                            result = true;
                            gridViewSample.RefreshData();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, result);
                        #endregion

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void HuyDuyetE_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                var row = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                if (row != null && (row.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ))
                {
                    LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                    sdo.SampleId = row.ID;
                    sdo.RequestRoomCode = room.ROOM_CODE;
                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Unsample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                    if (curentSTT != null)
                    {
                        WaitingManager.Hide();
                        row.SAMPLE_ORDER = null;
                        FillDataToGridControl();
                        result = true;
                        gridViewSample.RefreshData();
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, result);
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

        private void TraKetQuaE_Click(object sender, EventArgs e)
        {
            try
            {
                bool result = false;
                CommonParam param = new CommonParam();
                ValidationDateResultTime();
                dxValidationProvider1.SetValidationRule(mmNote, null);
                if (!dxValidationProvider1.Validate()) return;
                V_LIS_SAMPLE row = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                if (row != rowSample)
                {
                    rowSample = row;
                    LoadLisResult(rowSample);
                    LoadDataToGridTestResultWithVKKS();
                    SetDataToCommon(rowSample);
                }
                if (rowSample != null)
                {
                    if (HisConfigCFG.StartTimeMustBeGreaterThanInstructionTime == "1" || HisConfigCFG.StartTimeMustBeGreaterThanInstructionTime == "2")
                    {
                        if (Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmm00")) < rowSample.INTRUCTION_TIME)
                        {
                            MessageBox.Show("Thời gian trả kết quả không được nhỏ hơn thời gian y lệnh.", "Thông báo");
                            return;
                        }
                    }

                    var dataChildNullValue = lstSampleServiceADOs != null && lstSampleServiceADOs.Count() > 0 ?
                        lstSampleServiceADOs.Where(o => (o.IS_PARENT != 1 || (o.IS_PARENT == 1 && o.HAS_ONE_CHILD == 1)) && String.IsNullOrWhiteSpace(o.VALUE_RANGE)).ToList()
                        : null;

                    if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "2" && dataChildNullValue != null && dataChildNullValue.Count() > 0
                         && MessageBox.Show(String.Format("Chỉ số ({0}) chưa có kết quả, bạn có muốn trả kết quả không?", String.Join("; ", dataChildNullValue.Select(o => o.TEST_INDEX_NAME))), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                    else if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "0" && dataChildNullValue != null && dataChildNullValue.Count() > 0)
                    {
                        MessageBox.Show(String.Format("Chỉ số ({0}) chưa có kết quả", String.Join("; ", dataChildNullValue.Select(o => o.TEST_INDEX_NAME))), "Thông báo");
                        return;
                    }
                    bool valid = true;

                    valid = valid && ValidateHisService_MaxTotalProcessTime(rowSample);

                    if (!valid)
                        return;

                    LisSampleReturnResultSDO lisSampleReturnResultSDO = new LIS.SDO.LisSampleReturnResultSDO();
                    lisSampleReturnResultSDO.SampleId = rowSample.ID;
                    lisSampleReturnResultSDO.ResultUsername = cboResultUsername.Text.ToString();
                    lisSampleReturnResultSDO.ResultLoginname = cboResultUsername.EditValue.ToString();
                    lisSampleReturnResultSDO.ResultTime = Inventec.Common.TypeConvert.Parse.ToInt64(dtResultTime.DateTime.ToString("yyyyMMddHHmmss"));
                    WaitingManager.Show();
                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("/api/LisSample/ReturnResult", ApiConsumer.ApiConsumers.LisConsumer, lisSampleReturnResultSDO, param);
                    if (curentSTT != null)
                    {

                        rowSample.SAMPLE_STT_ID = IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ;
                        FillDataToGridControl();
                        result = true;
                        //LoadLisResult(rowSample);

                        string message = string.Format("Trả kết quả toàn phần thành công. SERVICE_REQ_CODE: {0}. BARCODE: {1}", rowSample.SERVICE_REQ_CODE, rowSample.BARCODE);
                        string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        SdaEventLogCreate eventlog = new SdaEventLogCreate();
                        eventlog.Create(login, null, true, message);
                    }

                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, result);
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

        private bool ValidateHisService_MaxTotalProcessTime(V_LIS_SAMPLE sample)
        {
            bool valid = true;
            try
            {
                if (sample == null)
                    return false;
                if (!sample.INTRUCTION_TIME.HasValue || sample.INTRUCTION_TIME <= 0 || String.IsNullOrWhiteSpace(sample.SERVICE_REQ_CODE))
                    return valid;
                if (Config.HisConfigCFG.PROCESS_TIME_MUST_BE_LESS_THAN_MAX_TOTAL_PROCESS_TIME != "1"
                    && Config.HisConfigCFG.PROCESS_TIME_MUST_BE_LESS_THAN_MAX_TOTAL_PROCESS_TIME != "2")
                    return valid;
                CommonParam param = new CommonParam();
                LisSampleServiceViewFilter sampleServiceFilter = new LisSampleServiceViewFilter();
                sampleServiceFilter.SAMPLE_ID = sample.ID;
                var listSampleServices = new BackendAdapter(param).Get<List<V_LIS_SAMPLE_SERVICE>>("/api/LisSampleService/GetView", ApiConsumer.ApiConsumers.LisConsumer, sampleServiceFilter, param);
                if (listSampleServices == null || listSampleServices.Count() == 0)
                    return valid;

                List<HIS_SERE_SERV> dataSereServs = new List<HIS_SERE_SERV>();
                if (this.rowSample != null && !String.IsNullOrEmpty(this.rowSample.SERVICE_REQ_CODE))
                {
                    HisSereServFilter ssFilter = new HisSereServFilter();
                    // lay sereServ tu y lenh
                    ssFilter.TDL_SERVICE_REQ_CODE_EXACT = this.rowSample.SERVICE_REQ_CODE;
                    dataSereServs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);
                }

                var intructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sample.INTRUCTION_TIME.Value);
                if (intructionTime == null)
                    return valid;
                var intructionTime_ToMinute = intructionTime.Value.AddSeconds(-intructionTime.Value.Second);
                TimeSpan processTime = (DateTime.Now - intructionTime_ToMinute);
                Dictionary<long, List<V_HIS_SERVICE>> dicInvalidServices = new Dictionary<long, List<V_HIS_SERVICE>>();

                foreach (var item in listSampleServices)
                {
                    var service = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.SERVICE_CODE) && o.SERVICE_CODE == item.SERVICE_CODE);
                    var ss = (dataSereServs != null && dataSereServs.Count > 0) ? dataSereServs.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.TDL_SERVICE_CODE) && o.TDL_SERVICE_CODE == item.SERVICE_CODE) : null;
                    if (service != null && service.MAX_TOTAL_PROCESS_TIME.HasValue && service.MAX_TOTAL_PROCESS_TIME.Value > 0
                            && processTime.TotalMinutes > service.MAX_TOTAL_PROCESS_TIME.Value && (string.IsNullOrEmpty(service.TOTAL_TIME_EXCEPT_PATY_IDS) || (ss != null && !service.TOTAL_TIME_EXCEPT_PATY_IDS.Split(',').Contains(ss.PATIENT_TYPE_ID.ToString()))))

                    {
                        if (dicInvalidServices.ContainsKey(service.MAX_TOTAL_PROCESS_TIME.Value))
                        {
                            if (!dicInvalidServices[service.MAX_TOTAL_PROCESS_TIME.Value].Contains(service))
                            {
                                dicInvalidServices[service.MAX_TOTAL_PROCESS_TIME.Value].Add(service);
                            }
                        }
                        else
                        {
                            dicInvalidServices.Add(service.MAX_TOTAL_PROCESS_TIME.Value, new List<V_HIS_SERVICE>() { service });
                        }
                    }
                }

                if (dicInvalidServices.Count > 0)
                {
                    string message = "";
                    List<string> listmsg = new List<string>();
                    if (Config.HisConfigCFG.PROCESS_TIME_MUST_BE_LESS_THAN_MAX_TOTAL_PROCESS_TIME == "1")
                    {
                        foreach (var item in dicInvalidServices)
                        {
                            string msg = String.Format(Resources.ResourceMessage.KhongChoPhepTraKetQuaDichVu_Sau_PhutTinhTuThoiDiemRaYLenh
                                                        , String.Join(", ", item.Value.Select(o => o.SERVICE_NAME))
                                                        , item.Key.ToString()
                                                        , Inventec.Common.DateTime.Convert.TimeNumberToTimeString(sample.INTRUCTION_TIME.Value));
                            listmsg.Add(msg);
                        }
                        message = String.Join("; ", listmsg) + ".";
                        valid = false;
                        XtraMessageBox.Show(message, Resources.ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (Config.HisConfigCFG.PROCESS_TIME_MUST_BE_LESS_THAN_MAX_TOTAL_PROCESS_TIME == "2")
                    {
                        foreach (var item in dicInvalidServices)
                        {
                            string msg = String.Format(Resources.ResourceMessage.TraKetQuaDichVu_VuotQua_PhutTinhTuThoiDiemRaYLenh
                                                        , String.Join(", ", item.Value.Select(o => o.SERVICE_NAME))
                                                        , item.Key.ToString()
                                                        , Inventec.Common.DateTime.Convert.TimeNumberToTimeString(sample.INTRUCTION_TIME.Value));
                            listmsg.Add(msg);
                        }
                        message = String.Join("; ", listmsg) + ".";
                        if (XtraMessageBox.Show(String.Format("{0} {1}", message, Resources.ResourceMessage.BanCoMuonTiepTucKhong), Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                        {
                            valid = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private void HuyTraKQE_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateStt(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtnUpdateBarcodeTime_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                if (data != null)
                {

                    if (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ
                        || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                        || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                    {
                        return;
                    }
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/UpdateBarcodeTime", ApiConsumers.LisConsumer, data.ID, param);
                    if (rs != null)
                    {
                        data.SAMPLE_ORDER = rs.SAMPLE_ORDER;
                        success = true;
                    }
                    WaitingManager.Hide();
                    if (success)
                    {
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }

                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchKey_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSERVICE_REQ_CODE__EXACT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #endregion

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ReturnMicrobiologicalResults.Resources.Lang", typeof(HIS.Desktop.Plugins.ReturnMicrobiologicalResults.UC_ReturnMicrobiologicalResults).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnResetNumOrder.Text = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.btnResetNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSERVICE_REQ_CODE__EXACT.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.txtSERVICE_REQ_CODE__EXACT.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.grdCollSTT.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.grdCollSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIndexCode.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.grdColIndexCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIndexName.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.grdColIndexName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCollDonvitinh.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.grdCollDonvitinh.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColVallue.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.grdColVallue.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColValueNormal.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.grdColValueNormal.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListModifier.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.treeListModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboFind.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.cboFind.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchKey.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.txtSearchKey.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnBarCode.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.layoutControlItem11.Tooltip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNewestBarcode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.lciNewestBarcode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UC_ReturnMicrobiologicalResults.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void GetControlAcs()
        {
            try
            {
                CommonParam param = new CommonParam();
                ACS.SDO.AcsTokenLoginSDO tokenLoginSDOForAuthorize = new ACS.SDO.AcsTokenLoginSDO();
                tokenLoginSDOForAuthorize.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                tokenLoginSDOForAuthorize.APPLICATION_CODE = GlobalVariables.APPLICATION_CODE;

                var acsAuthorize = new BackendAdapter(param).Get<ACS.SDO.AcsAuthorizeSDO>(HIS.Desktop.ApiConsumer.AcsRequestUriStore.ACS_TOKEN__AUTHORIZE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, tokenLoginSDOForAuthorize, param);

                if (acsAuthorize != null)
                {
                    controlAcs = acsAuthorize.ControlInRoles.ToList();
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("ACS control", controlAcs));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGridTestResultWithVKKS()
        {
            try
            {
                this.GetSampleServiceBySample(this.rowSample);
                this.lstSampleServiceADOs = new List<TestLisResultADO>();
                this.lstLisResultDetailForTreeADOs = new List<TestLisResultADO>();
                if (this.sampleServices != null && this.sampleServices.Count > 0)
                {
                    foreach (var item in this.sampleServices)
                    {
                        TestLisResultADO hisSereServTeinSDO = new TestLisResultADO();
                        hisSereServTeinSDO.IS_PARENT = 1;
                        hisSereServTeinSDO.SERVICE_CODE = item.SERVICE_CODE;
                        hisSereServTeinSDO.SERVICE_NAME = item.SERVICE_NAME;
                        hisSereServTeinSDO.IS_NO_EXECUTE = item.IS_NO_EXECUTE;
                        hisSereServTeinSDO.ID = item.ID;
                        hisSereServTeinSDO.MODIFIER = item.MODIFIER;
                        hisSereServTeinSDO.MODIFY_TIME = item.MODIFY_TIME;
                        hisSereServTeinSDO.SERVICE_NUM_ORDER = item.SERVICE_NUM_ORDER;
                        hisSereServTeinSDO.MICROBIOLOGICAL_RESULT = item.MICROBIOLOGICAL_RESULT;
                        hisSereServTeinSDO.CREATE_TIME = item.CREATE_TIME;
                        hisSereServTeinSDO.CREATOR = item.CREATOR;
                        hisSereServTeinSDO.SAMPLE_STT_ID = item.SAMPLE_STT_ID;
                        hisSereServTeinSDO.IS_RUN_AGAIN = item.IS_RUN_AGAIN;
                        hisSereServTeinSDO.IS_RUNNING = item.IS_RUNNING;
                        hisSereServTeinSDO.RERUN = item.IS_RUN_AGAIN == 1;
                        hisSereServTeinSDO.MACHINE_ID = item.MACHINE_ID;
                        hisSereServTeinSDO.MACHINE_ID_OLD = item.MACHINE_ID;
                        hisSereServTeinSDO.SAMPLE_ID = item.SAMPLE_ID;
                        hisSereServTeinSDO.SAMPLE_SERVICE_STT_CODE = item.SAMPLE_SERVICE_STT_CODE;
                        hisSereServTeinSDO.SAMPLE_SERVICE_STT_ID = item.SAMPLE_SERVICE_STT_ID;
                        hisSereServTeinSDO.SAMPLE_SERVICE_STT_NAME = item.SAMPLE_SERVICE_STT_NAME;
                        hisSereServTeinSDO.SAMPLE_SERVICE_ID = item.ID;
                        hisSereServTeinSDO.LABORATORY_CODE = item.LABORATORY_CODE;
                        hisSereServTeinSDO.LResultDetails = _LisResults.Where(o => o.SAMPLE_SERVICE_ID == item.ID && !String.IsNullOrEmpty(o.BACTERIUM_CODE)).ToList();
                        this.lstSampleServiceADOs.Add(hisSereServTeinSDO);
                    }
                }

                if (this.lstSampleServiceADOs != null && this.lstSampleServiceADOs.Count() > 0)
                {
                    this.lstSampleServiceADOs = this.lstSampleServiceADOs.OrderBy(o => o.SERVICE_NUM_ORDER).ThenBy(p => p.SERVICE_NAME).ToList();
                }

                gridViewVSKS.BeginUpdate();
                gridViewVSKS.GridControl.DataSource = this.lstSampleServiceADOs;
                gridViewVSKS.EndUpdate();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstSampleServiceADOs), lstSampleServiceADOs));

                btnPrint.Enabled = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                gridViewVSKS.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void GetSampleServiceBySample(V_LIS_SAMPLE lSample)
        {
            try
            {
                CommonParam param = new CommonParam();
                LisSampleServiceViewFilter sampleServiceFilter = new LisSampleServiceViewFilter();
                sampleServiceFilter.SAMPLE_ID = lSample.ID;
                //sampleServiceFilter.SERVICE_CODE__EXACT = lResult.SERVICE_CODE;

                this.sampleServices = new BackendAdapter(param).Get<List<V_LIS_SAMPLE_SERVICE>>("/api/LisSampleService/GetView", ApiConsumer.ApiConsumers.LisConsumer, sampleServiceFilter, param);
            }
            catch (Exception ex)
            {
                this.sampleServices = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToTreeTestResult2()
        {
            try
            {
                CommonParam param = new CommonParam();

                this.lstLisResultDetailForTreeADOs = new List<TestLisResultADO>();
                this.treeListSereServTein.DataSource = null;
                if (this.currentTestSamResultADO.LResultDetails != null && this.currentTestSamResultADO.LResultDetails.Count > 0)
                {
                    //this.currentTestSamResultADO.LResultDetails = this.currentTestSamResultADO.LResultDetails.OrderBy(o => o.SERVICE_NUM_ORDER).ToList();

                    var groupListResult = this.currentTestSamResultADO.LResultDetails.GroupBy(o => new { o.SERVICE_CODE, o.BACTERIUM_CODE }).ToList();

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => groupListResult), groupListResult));
                    foreach (var group in groupListResult)
                    {
                        TestLisResultADO hisSereServTeinSDO = new TestLisResultADO();
                        var fistGroup = group.FirstOrDefault();
                        hisSereServTeinSDO.IS_PARENT = 1;
                        hisSereServTeinSDO.SERVICE_CODE = fistGroup != null ? fistGroup.SERVICE_CODE : "";
                        hisSereServTeinSDO.SERVICE_NAME = fistGroup != null ? fistGroup.SERVICE_NAME : "";
                        hisSereServTeinSDO.BACTERIUM_CODE = fistGroup != null ? fistGroup.BACTERIUM_CODE : "";
                        hisSereServTeinSDO.BACTERIUM_NAME = fistGroup != null ? fistGroup.BACTERIUM_NAME : "";

                        hisSereServTeinSDO.SAMPLE_SERVICE_ID = fistGroup != null ? fistGroup.SAMPLE_SERVICE_ID : null;
                        hisSereServTeinSDO.ID = fistGroup.ID;
                        hisSereServTeinSDO.IS_NO_EXECUTE = fistGroup.IS_NO_EXECUTE;
                        hisSereServTeinSDO.PARENT_ID = ".";
                        hisSereServTeinSDO.MODIFIER = "";
                        hisSereServTeinSDO.CHILD_ID = fistGroup.SAMPLE_ID + "." + fistGroup.SAMPLE_SERVICE_ID + "." + fistGroup.BACTERIUM_CODE;
                        hisSereServTeinSDO.SERVICE_NUM_ORDER = fistGroup.SERVICE_NUM_ORDER;
                        hisSereServTeinSDO.IS_RUN_AGAIN = fistGroup.IS_RUN_AGAIN;
                        hisSereServTeinSDO.IS_RUNNING = fistGroup.IS_RUNNING;
                        hisSereServTeinSDO.RERUN = fistGroup.IS_RUN_AGAIN == 1;
                        //Lay machine_id
                        var lstResultItem = group.Where(o => o.ANTIBIOTIC_CODE != null).ToList();
                        lstResultItem = lstResultItem.OrderBy(o => o.ANTIBIOTIC_ORDER).ThenBy(o => o.ANTIBIOTIC_NAME).ToList();
                        lstLisResultDetailForTreeADOs.Add(hisSereServTeinSDO);

                        if (lstResultItem != null && lstResultItem.Count > 0)
                        {
                            foreach (var ssTein in lstResultItem)
                            {
                                TestLisResultADO hisSereServTein = new TestLisResultADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<TestLisResultADO>(hisSereServTein, ssTein);
                                hisSereServTein.IS_PARENT = 0;
                                hisSereServTein.HAS_ONE_CHILD = 0;
                                hisSereServTein.ANTIBIOTIC_ORDER = ssTein.ANTIBIOTIC_ORDER;
                                hisSereServTein.CHILD_ID = hisSereServTeinSDO.CHILD_ID + "." + ssTein.ANTIBIOTIC_CODE;
                                hisSereServTein.ID = ssTein.ID;
                                hisSereServTein.PARENT_ID = hisSereServTeinSDO.CHILD_ID;
                                hisSereServTein.MODIFIER = ssTein.MODIFIER;
                                hisSereServTein.LIS_RESULT_ID = ssTein.ID;
                                hisSereServTein.MACHINE_ID = ssTein.MACHINE_ID;
                                hisSereServTein.MACHINE_ID_OLD = ssTein.MACHINE_ID;
                                hisSereServTein.SAMPLE_ID = ssTein.SAMPLE_ID;
                                hisSereServTein.SAMPLE_SERVICE_ID = ssTein.SAMPLE_SERVICE_ID;
                                hisSereServTein.SAMPLE_SERVICE_STT_CODE = ssTein.SAMPLE_SERVICE_STT_CODE;
                                hisSereServTein.SAMPLE_SERVICE_STT_ID = ssTein.SAMPLE_SERVICE_STT_ID;
                                hisSereServTein.SAMPLE_SERVICE_STT_NAME = ssTein.SAMPLE_SERVICE_STT_NAME;
                                hisSereServTein.SERVICE_NUM_ORDER = ssTein.SERVICE_NUM_ORDER;
                                hisSereServTein.SAMPLE_STT_ID = ssTein.SAMPLE_STT_ID;
                                hisSereServTein.IS_RUN_AGAIN = ssTein.IS_RUN_AGAIN;
                                hisSereServTein.IS_RUNNING = ssTein.IS_RUNNING;
                                hisSereServTein.SRI_CODE = ssTein.SRI_CODE;
                                hisSereServTein.MIC = ssTein.MIC;
                                hisSereServTein.DESCRIPTION = ssTein.DESCRIPTION;
                                hisSereServTein.TECHNIQUE_ID = ssTein.TECHNIQUE_ID;
                                hisSereServTein.BACTERIUM_ID = ssTein != null && !string.IsNullOrEmpty(ssTein.BACTERIUM_CODE) ? (datasBacterium.FirstOrDefault(o => o.BACTERIUM_CODE == ssTein.BACTERIUM_CODE) != null ? datasBacterium.FirstOrDefault(o => o.BACTERIUM_CODE == ssTein.BACTERIUM_CODE).ID : 0) : 0;
                                hisSereServTein.ANTIBIOTIC_ID = ssTein != null && !string.IsNullOrEmpty(ssTein.ANTIBIOTIC_CODE) ? (datasAntibiotic.FirstOrDefault(o => o.ANTIBIOTIC_CODE == ssTein.ANTIBIOTIC_CODE) != null ? datasAntibiotic.FirstOrDefault(o => o.ANTIBIOTIC_CODE == ssTein.ANTIBIOTIC_CODE).ID : 0) : 0;
                                lstLisResultDetailForTreeADOs.Add(hisSereServTein);
                            }
                        }
                    }
                }

                // treeList
                records = new BindingList<TestLisResultADO>(lstLisResultDetailForTreeADOs);
                //this.treeListSereServTein.RefreshDataSource();
                this.treeListSereServTein.DataSource = records;
                this.treeListSereServTein.KeyFieldName = "CHILD_ID";
                this.treeListSereServTein.ParentFieldName = "PARENT_ID";
                this.treeListSereServTein.ExpandAll();

                this.treeListSereServTein.CheckAll();
                //toggleSwitchSelectAll.IsOn = false;
                lstCheckPrint = lstLisResultDetailForTreeADOs;

                btnPrint.Enabled = true;

                this.hideCheckBoxHelper__Service = new HideCheckBoxHelper(this.treeListSereServTein);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                //if (e.SelectedControl == treeListSereServTein)
                //{
                //    string text = "";
                //    object cellInfo = null;
                //    TreeList tree = (TreeList)e.SelectedControl;
                //    TreeListHitInfo hit = tree.CalcHitInfo(e.ControlMousePosition);
                //    if (hit.HitInfoType == HitInfoType.Cell)
                //    {
                //        TestLisResultADO data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(hit.Node);
                //        cellInfo = new TreeListCellToolTipInfo(hit.Node, hit.Column, null);
                //        if (data != null && data.IS_PARENT != 1)
                //        {
                //            if (data.SAMPLE_SERVICE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__CHUA_CO_KQ)
                //            {
                //                text = "Chưa có kết quả";
                //            }
                //            else if (data.SAMPLE_SERVICE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_CO_KQ)
                //            {
                //                text = "Đã có kết quả";
                //            }
                //            else if (data.SAMPLE_SERVICE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_TRA_KQ)
                //            {
                //                text = "Đã trả kết quả";
                //            }
                //            if (data.IS_RUNNING == 1)
                //            {
                //                text = text + "(Đang chạy xét nghiệm)";
                //            }
                //        }
                //    }
                //    e.Info = new DevExpress.Utils.ToolTipControlInfo(cellInfo, text);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateSyncDataFromTreeToLisResult(ref bool isContinue, bool isSave = false)
        {
            try
            {
                List<string> baterirumWarning = new List<string>();
                List<TestLisResultADO> dataGrid = new List<TestLisResultADO>();
                var AllCheckNodes = treeListSereServTein.GetAllCheckedNodes();
                foreach (var item in AllCheckNodes)
                {
                    var data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(item);
                    dataGrid.Add(data);
                    if (isSave && data.IS_PARENT != 1 && String.IsNullOrWhiteSpace(data.SRI_CODE))
                    {
                        baterirumWarning.Add(data.ANTIBIOTIC_NAME);
                    }
                }
                if (isSave && baterirumWarning != null && baterirumWarning.Count > 0)
                {
                    if (XtraMessageBox.Show(String.Format("Kháng sinh {0} thiếu thông tin SRI. Bạn có muốn tiếp tục?", String.Join(",", baterirumWarning)), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        isContinue = false;
                        return;
                    }
                }
                this.currentTestSamResultADO.LResultDetails = (from m in dataGrid
                                                               select new LisResultADO()
                                                               {
                                                                   ANTIBIOTIC_ORDER = m.ANTIBIOTIC_ORDER,
                                                                   ANTIBIOTIC_CODE = m.ANTIBIOTIC_CODE,
                                                                   ANTIBIOTIC_NAME = m.ANTIBIOTIC_NAME,
                                                                   BACTERIUM_CODE = m.BACTERIUM_CODE,
                                                                   BACTERIUM_FAMILY_CODE = m.BACTERIUM_FAMILY_CODE,
                                                                   BACTERIUM_FAMILY_NAME = m.BACTERIUM_FAMILY_NAME,
                                                                   BACTERIUM_NAME = m.BACTERIUM_NAME,
                                                                   DESCRIPTION = m.DESCRIPTION,
                                                                   ID = m.ID,
                                                                   MACHINE_ID = m.MACHINE_ID,
                                                                   MIC = m.MIC,
                                                                   MODIFIER = m.MODIFIER,
                                                                   MODIFY_TIME = m.MODIFY_TIME,
                                                                   SAMPLE_ID = m.SAMPLE_ID,
                                                                   SAMPLE_SERVICE_ID = m.SAMPLE_SERVICE_ID,
                                                                   SAMPLE_SERVICE_STT_ID = m.SAMPLE_SERVICE_STT_ID,
                                                                   SAMPLE_STT_ID = m.SAMPLE_STT_ID,
                                                                   SERVICE_CODE = m.SERVICE_CODE,
                                                                   SERVICE_NAME = m.SERVICE_NAME,
                                                                   SRI_CODE = m.SRI_CODE,
                                                                   TECHNIQUE_ID = m.TECHNIQUE_ID
                                                               }).ToList();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTestSamResultADO.LResultDetails), currentTestSamResultADO.LResultDetails));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool SaveValue(ref CommonParam param, ref bool isCalledApi)
        {
            bool result = false;
            try
            {
                treeListSereServTein.PostEditor();
                btnSave.Focus();
                bool isContinue = true;
                UpdateSyncDataFromTreeToLisResult(ref isContinue, true);
                if (!isContinue)
                {
                    isCalledApi = false;
                    return false;
                }

                if (this.lstSampleServiceADOs != null && lstSampleServiceADOs.Count > 0)
                {
                    SampleBacteriumSDO sampleBacteriumSDO = new LIS.SDO.SampleBacteriumSDO();
                    sampleBacteriumSDO.SampleId = rowSample.ID;
                    sampleBacteriumSDO.Note = !string.IsNullOrEmpty(mmNote.Text.Trim()) ? mmNote.Text.Trim() : null;
                    sampleBacteriumSDO.SampleTime = Inventec.Common.TypeConvert.Parse.ToInt64(dtSampleTime.DateTime.ToString("yyyyMMddHHmmss"));
                    sampleBacteriumSDO.Services = new List<ServiceBacteriumSDO>();
                    foreach (var item in this.lstSampleServiceADOs)
                    {
                        ServiceBacteriumSDO serviceBacteriumSDO = new ServiceBacteriumSDO();
                        serviceBacteriumSDO.SampleServiceId = item.SAMPLE_SERVICE_ID ?? 0;
                        serviceBacteriumSDO.Results = new List<AntibioticResultSDO>();
                        if (item.LResultDetails != null)
                        {
                            foreach (var itemLRS in item.LResultDetails)
                            {
                                AntibioticResultSDO antibioticResultSDO = new AntibioticResultSDO();
                                antibioticResultSDO.AntibioticCode = itemLRS.ANTIBIOTIC_CODE;
                                antibioticResultSDO.AntibioticName = itemLRS.ANTIBIOTIC_NAME;
                                antibioticResultSDO.BacteriumCode = itemLRS.BACTERIUM_CODE;
                                antibioticResultSDO.BacteriumName = itemLRS.BACTERIUM_NAME;
                                antibioticResultSDO.BacteriumFamilyCode = itemLRS.BACTERIUM_FAMILY_CODE;
                                antibioticResultSDO.BacteriumFamilyName = itemLRS.BACTERIUM_FAMILY_NAME;
                                antibioticResultSDO.MachineId = itemLRS.MACHINE_ID;
                                antibioticResultSDO.Mic = itemLRS.MIC;
                                antibioticResultSDO.Note = itemLRS.DESCRIPTION;
                                antibioticResultSDO.ResultId = itemLRS.ID > 0 ? (long?)itemLRS.ID : null;
                                antibioticResultSDO.Interpretation = itemLRS.SRI_CODE;
                                antibioticResultSDO.TechniquesId = itemLRS.TECHNIQUE_ID;

                                serviceBacteriumSDO.Results.Add(antibioticResultSDO);
                            }
                        }

                        sampleBacteriumSDO.Services.Add(serviceBacteriumSDO);
                    }

                    param = new CommonParam();

                    Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sampleBacteriumSDO), sampleBacteriumSDO));
                    var rs = new BackendAdapter(param).Post<bool>("api/LisSample/BacteriumResult", ApiConsumers.LisConsumer, sampleBacteriumSDO, param);
                    if (rs)
                    {
                        result = true;
                        UpdateDataForSaveSuccess();
                    }
                    WaitingManager.Hide();
                }
                else
                {
                    param.Messages.Add("Chưa nhập giá trị cho kháng sinh đồ hoặc không thay đổi");
                    Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstSampleServiceADOs), lstSampleServiceADOs));
                    return false;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                ValidationMaxLengthNote();
                dxValidationProvider1.SetValidationRule(dtResultTime, null);
                if (!dxValidationProvider1.Validate()) return;
                bool isCalledApi = true;
                bool success = SaveValue(ref param, ref isCalledApi);
                if (isCalledApi)
                    MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void UpdateDataForSaveSuccess()
        {
            try
            {
                LisSampleFilter samFilter = new LisSampleFilter();
                samFilter.ID = rowSample.ID;
                var datas = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<LIS_SAMPLE>>("api/LisSample/Get", ApiConsumers.LisConsumer, samFilter, null);
                if (datas == null || datas.Count != 1)
                {
                    FillDataToGridControl();
                    throw new Exception("Khong lay duoc LIS_SAMPLE theo id: " + samFilter.ID);
                }
                rowSample.SAMPLE_STT_ID = datas[0].SAMPLE_STT_ID;
                rowSample.NOTE = datas[0].NOTE;
                rowSample.SAMPLE_TIME = datas[0].SAMPLE_TIME;
                gridControlSample.RefreshDataSource();
                LoadLisResult(rowSample);
                LoadDataToGridTestResultWithVKKS();
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                {
                    btnSave.Enabled = false;
                }
                else
                {
                    btnSave.Enabled = true;
                }
                btnPrint.Enabled = false;
                dxValidationProvider1.RemoveControlError(dtResultTime);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkSignProcess.Checked && (SignConfigData.listSign == null || SignConfigData.listSign.Count == 0))
                {
                    if (XtraMessageBox.Show("Bạn chưa thiết lập luồng ký. Bạn có muốn thiết lập luồng ký không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        SignConfigData = new SignConfigADO();
                        HIS.Desktop.Plugins.ReturnMicrobiologicalResults.AddSigner.frmSignerAdd frmAddSigner = new HIS.Desktop.Plugins.ReturnMicrobiologicalResults.AddSigner.frmSignerAdd(SignConfigData.listSign, UpdateAfterAddSignThread, SignConfigData.IsSignParanel);
                        frmAddSigner.ShowDialog(this.ParentForm);
                    }
                }
                //ProcessSaveAndReturnValue(ref print);
                this.PrintOption = PRINT_OPTION.IN_TACH_THEO_NHOM;
                PrintProcess(PrintTypeKXN.IN_KET_QUA_XET_NGHIEM);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SAVE()
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void PRINT()
        {
            try
            {
                if (btnPrint.Enabled)
                {
                    btnPrint_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SEARCH()
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusF1()
        {
            try
            {
                txtSERVICE_REQ_CODE__EXACT.Focus();
                txtSERVICE_REQ_CODE__EXACT.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnUpdateNumOrder_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (LIS_SAMPLE)gridViewSample.GetFocusedRow();
                if (data != null)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/UpdateNumOrder", ApiConsumers.LisConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                        CommonParam paramGet = new CommonParam(startPage, limit);
                        FillDataToGridSample(paramGet);
                    }
                    WaitingManager.Hide();
                    if (success)
                    {
                        MessageManager.ShowAlert(this.ParentForm, param, success);
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchKey_Click(object sender, EventArgs e)
        {
            try
            {
                txtSearchKey.Focus();
                txtSearchKey.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboMachine()
        {
            try
            {
                LIS.Filter.LisMachineFilter filter = new LisMachineFilter();
                this._Machines = new List<LIS_MACHINE>();
                this._Machines = new BackendAdapter(new CommonParam()).Get<List<LIS_MACHINE>>("api/LisMachine/Get", ApiConsumers.LisConsumer, filter, null
                    );
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(this.GridLookUpEdit__Machine, this._Machines, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void treeList1_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                TestLisResultADO testLisResultADO = new TestLisResultADO();
                var data = this.treeListSereServTein.GetDataRecordByNode(e.Node);
                if (data != null && data is TestLisResultADO)
                {
                    testLisResultADO = (TestLisResultADO)data;
                    if (testLisResultADO.IS_PARENT == 1 && testLisResultADO.IS_NO_EXECUTE == 1)
                    {
                        e.Appearance.FontStyleDelta = FontStyle.Strikeout;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void treeList1_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                TestLisResultADO testLisResultADO = new TestLisResultADO();
                var data = this.treeListSereServTein.GetDataRecordByNode(e.Node);
                if (data != null && data is TestLisResultADO)
                {
                    testLisResultADO = (TestLisResultADO)data;
                }
                if (e.Column.FieldName == "MACHINE_ID")
                {
                    if (((TestLisResultADO)data).IS_PARENT == 0)
                    {
                        e.RepositoryItem = GridLookUpEdit__Machine;
                    }
                    else
                    {
                        e.RepositoryItem = GridLookUpEdit__Machine_Disable;
                    }
                }
                else if ((e.Column.FieldName == "DESCRIPTION") && data != null)
                {
                    if (testLisResultADO.IS_PARENT == 1)
                    {
                        e.RepositoryItem = TextEditValueRange__Disable;
                        e.Column.OptionsColumn.AllowEdit = false;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItemText__Description__200;
                        e.Column.OptionsColumn.AllowEdit = true;
                    }
                }
                else if ((e.Column.FieldName == "MIC") && data != null)
                {
                    if (testLisResultADO.IS_PARENT == 1)
                    {
                        e.RepositoryItem = TextEditValueRange__Disable;
                        e.Column.OptionsColumn.AllowEdit = false;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItemText__Description__50;
                        e.Column.OptionsColumn.AllowEdit = true;
                    }
                }
                else if ((e.Column.FieldName == "SRI_CODE") && data != null)
                {
                    if (testLisResultADO.IS_PARENT == 1)
                    {
                        e.RepositoryItem = TextEditValueRange__Disable;
                        e.Column.OptionsColumn.AllowEdit = false;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItemText__Description__2;
                        e.Column.OptionsColumn.AllowEdit = true;
                    }
                }
                if (e.Column.FieldName == "TECHNIQUE_ID")
                {
                    if (((TestLisResultADO)data).IS_PARENT == 0)
                    {
                        e.RepositoryItem = repositoryItemCboTechnique;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItemCboTechniqueDisable;
                    }
                }
                else if (e.Column.FieldName == "IMAGE")
                {
                    e.RepositoryItem = ButtonEdit_Delete;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<LIS_ANTIBIOTIC_RANGE> GetlstAntibioticRange()
        {
            List<LIS_ANTIBIOTIC_RANGE> lst = null;
            try
            {
                CommonParam commonParam = new CommonParam();
                lst = new BackendAdapter(commonParam).Get<List<LIS_ANTIBIOTIC_RANGE>>("api/LisAntibioticRange/Get", ApiConsumer.ApiConsumers.LisConsumer, new LisAntibioticFilter(), commonParam);
                if (lst != null && lst.Count > 0)
                {
                    lst = lst.Where(o => o.IS_ACTIVE == (short?)1).ToList();
                }
            }
            catch (Exception ex)
            {
                lst = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return lst;
        }

        private void InitAntibiotic()
        {
            try
            {
                if (BackendDataWorker.IsExistsKey<LIS_ANTIBIOTIC>())
                {
                    datasAntibiotic = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<LIS_ANTIBIOTIC>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datasAntibiotic = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<LIS_ANTIBIOTIC>>("api/LisAntibiotic/Get", HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer, filter, paramCommon);
                    if (datasAntibiotic != null) BackendDataWorker.UpdateToRam(typeof(LIS_ANTIBIOTIC), datasAntibiotic, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                datasAntibiotic = datasAntibiotic != null ? datasAntibiotic.Where(o => o.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitBacterium()
        {
            try
            {
                if (BackendDataWorker.IsExistsKey<LIS_BACTERIUM>())
                {
                    datasBacterium = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<LIS_BACTERIUM>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datasBacterium = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<LIS_BACTERIUM>>("api/LisBacterium/Get", HIS.Desktop.ApiConsumer.ApiConsumers.LisConsumer, filter, paramCommon);
                    if (datasBacterium != null) BackendDataWorker.UpdateToRam(typeof(LIS_BACTERIUM), datasBacterium, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                datasBacterium = datasBacterium != null ? datasBacterium.Where(o => o.IS_ACTIVE == IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeList1_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            try
            {

                if (e.Column.FieldName == "MIC")
                {
                    var data = this.treeListSereServTein.GetDataRecordByNode(e.Node);
                    if (data != null && data is TestLisResultADO)
                    {
                        List<LIS_ANTIBIOTIC_RANGE> lstTemp = null;
                        string Sri = "";
                        var RowData = (TestLisResultADO)data;
                        if (lstAntibioticRange != null && lstAntibioticRange.Count > 0)
                        {
                            lstTemp = lstAntibioticRange.Where(o => o.ANTIBIOTIC_ID == RowData.ANTIBIOTIC_ID && o.BACTERIUM_ID == RowData.BACTERIUM_ID && o.TECHNIQUE_ID == RowData.TECHNIQUE_ID).OrderBy(o => o.ID).ToList();
                        }
                        if (lstTemp != null && !string.IsNullOrEmpty(RowData.MIC))
                        {
                            decimal Mic = Convert.ToDecimal(RowData.MIC, new CultureInfo("en-US"));
                            foreach (var item in lstTemp)
                            {
                                decimal Min = 0;
                                decimal Max = 0;
                                string strMin = item.MIN_VALUE;
                                string strMax = item.MAX_VALUE;
                                try
                                {
                                    if (!string.IsNullOrEmpty(strMin))
                                        Min = Convert.ToDecimal(strMin, new CultureInfo("en-US"));
                                    if (!string.IsNullOrEmpty(strMax))
                                        Max = Convert.ToDecimal(strMax, new CultureInfo("en-US"));
                                }
                                catch (Exception)
                                {
                                    Inventec.Common.Logging.LogSystem.Error("Sai định dạng__________");
                                    continue;
                                }

                                bool IsEqualMin = item.IS_ACCEPT_EQUAL_MIN == (short?)1 ? true : false;
                                bool IsEqualMax = item.IS_ACCEPT_EQUAL_MAX == (short?)1 ? true : false;
                                if (IsEqualMin && IsEqualMax &&
                                    ((!string.IsNullOrEmpty(strMin) && !string.IsNullOrEmpty(strMax) && Min <= Mic && Mic <= Max)
                                    || (string.IsNullOrEmpty(strMin) && !string.IsNullOrEmpty(strMax) && Mic <= Max)
                                    || (!string.IsNullOrEmpty(strMin) && string.IsNullOrEmpty(strMax) && Min <= Mic))

                                    || (IsEqualMin && !IsEqualMax &&
                                    ((!string.IsNullOrEmpty(strMin) && !string.IsNullOrEmpty(strMax) && Min <= Mic && Mic < Max)
                                    || (string.IsNullOrEmpty(strMin) && !string.IsNullOrEmpty(strMax) && Mic < Max)
                                    || (!string.IsNullOrEmpty(strMin) && string.IsNullOrEmpty(strMax) && Min <= Mic)))

                                    || (!IsEqualMin && IsEqualMax &&
                                    ((!string.IsNullOrEmpty(strMin) && !string.IsNullOrEmpty(strMax) && Min < Mic && Mic <= Max)
                                    || (string.IsNullOrEmpty(strMin) && !string.IsNullOrEmpty(strMax) && Mic <= Max)
                                    || (!string.IsNullOrEmpty(strMin) && string.IsNullOrEmpty(strMax) && Min < Mic)))

                                    || (!IsEqualMin && !IsEqualMax &&
                                    ((!string.IsNullOrEmpty(strMin) && !string.IsNullOrEmpty(strMax) && Min < Mic && Mic < Max)
                                    || (string.IsNullOrEmpty(strMin) && !string.IsNullOrEmpty(strMax) && Mic < Max)
                                    || (!string.IsNullOrEmpty(strMin) && string.IsNullOrEmpty(strMax) && Min < Mic)))
                                    )
                                {
                                    Sri = item.SRI_VALUE;
                                }
                            }
                        }
                        ((TestLisResultADO)data).SRI_CODE = Sri;
                    }
                }
                if (e.Column.FieldName == "MIC" || e.Column.FieldName == "SRI_CODE" || e.Column.FieldName == "DESCRIPTION" || e.Column.FieldName == "MACHINE_ID")
                {
                    try
                    {
                        treeListSereServTein.RefreshDataSource();
                    }
                    catch { }
                    bool isContinue = true;
                    UpdateSyncDataFromTreeToLisResult(ref isContinue);
                }

                //else if (e.Column.FieldName == "RERUN")
                //{
                //    var data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(e.Node);
                //    if (data != null && data.IS_PARENT == 1)
                //    {
                //        WaitingManager.Show();
                //        bool success = false;
                //        CommonParam param = new CommonParam();
                //        LisSampleServiceSDO sdo = new LisSampleServiceSDO();
                //        sdo.Id = data.SAMPLE_SERVICE_ID ?? 0;
                //        sdo.IsRunAgain = data.RERUN.HasValue ? data.RERUN.Value : false;
                //        var rs = new BackendAdapter(param).Post<LIS_SAMPLE_SERVICE>("api/LisSampleService/UpdateRetest", ApiConsumers.LisConsumer, sdo, param);
                //        if (rs != null)
                //        {
                //            data.IS_RUNNING = rs.IS_RUNNING;
                //            data.IS_RUN_AGAIN = rs.IS_RUN_AGAIN;
                //            data.RERUN = rs.IS_RUN_AGAIN == 1;
                //            success = true;
                //        }
                //        else
                //        {
                //            data.RERUN = data.IS_RUN_AGAIN == 1;
                //        }
                //        treeListSereServTein.RefreshDataSource();
                //        WaitingManager.Hide();
                //        MessageManager.Show(this.ParentForm, param, success);
                //    }
                //}

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(e.Node);
                if (data != null)
                {
                    //if (e.Column.FieldName == "VALUE_RANGE")
                    //{
                    //    ProcessCheckNormal(ref data);
                    //}

                    if (data.IS_PARENT == 1 || data.HAS_ONE_CHILD == 1)
                    {
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }

                    if (data.IS_LOWER == true && data.IS_HIGHER == true)
                    {
                        e.Appearance.ForeColor = Color.Green;
                    }
                    else if (data.IS_LOWER == true)
                    {
                        e.Appearance.ForeColor = Color.Blue;
                    }
                    else if (data.IS_HIGHER == true)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Black;
                    }

                    //if (e.Column.FieldName == "TEST_INDEX_NAME" && data.IS_IMPORTANT > 0)
                    //{
                    //    e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    //    e.Appearance.ForeColor = Color.Black;
                    //}
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_AfterCheckNode(object sender, NodeEventArgs e)
        {
            try
            {
                bool isContinue = true;
                UpdateSyncDataFromTreeToLisResult(ref isContinue);
                btnSave.Enabled = !(rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ);
                btnPrint.Enabled = (this.lstSampleServiceADOs != null && this.lstSampleServiceADOs.Count > 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_BeforeCheckNode(object sender, CheckNodeEventArgs e)
        {
            try
            {
                treeListSereServTein.FocusedNode = e.Node;
                e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEdit_TraKetQua_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var dataFocus = this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                if (dataFocus != null)
                {
                    TestLisResultADO testLisResultADO = (TestLisResultADO)dataFocus;
                    List<TestLisResultADO> dataChildOfFocus = new List<TestLisResultADO>();
                    var AllCheckNodes = treeListSereServTein.GetAllCheckedNodes();

                    foreach (var item in AllCheckNodes)
                    {
                        var data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(item);
                        if (data.PARENT_ID == testLisResultADO.CHILD_ID || (data.HAS_ONE_CHILD == 1 && data.CHILD_ID == testLisResultADO.CHILD_ID))
                        {
                            dataChildOfFocus.Add(data);
                        }
                    }

                    if (testLisResultADO != null
                        && testLisResultADO.SAMPLE_SERVICE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_TRA_KQ)
                    {
                        var dataChildNullValue = dataChildOfFocus.Where(o => String.IsNullOrWhiteSpace(o.VALUE_RANGE)).ToList();

                        if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "2" && dataChildNullValue != null && dataChildNullValue.Count() > 0
                            && MessageBox.Show(String.Format("Chỉ số ({0}) chưa có kết quả, bạn có muốn trả kết quả không?", String.Join("; ", dataChildNullValue.Select(o => o.TEST_INDEX_NAME))), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }
                        else if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "0" && dataChildNullValue != null && dataChildNullValue.Count() > 0)
                        {
                            MessageBox.Show(String.Format("Chỉ số ({0}) chưa có kết quả", String.Join("; ", dataChildNullValue.Select(o => o.TEST_INDEX_NAME))), "Thông báo");
                            return;
                        }

                        WaitingManager.Show();
                        bool success = false;
                        CommonParam param = new CommonParam();

                        List<long> lisResultIds = new List<long>();
                        var dataChilds = dataChildOfFocus.Where(p => p.LIS_RESULT_ID.HasValue).ToList();
                        lisResultIds.AddRange(dataChilds.Select(o => o.LIS_RESULT_ID.Value).Distinct().ToList());

                        var rs = new BackendAdapter(param).Post<List<LIS_SAMPLE_SERVICE>>("api/LisSampleService/ReturnResult", ApiConsumers.LisConsumer, lisResultIds, param);
                        if (rs != null)
                        {
                            success = true;
                            FillDataToGridControl();
                            gridViewSample_RowCellClick(null, null);

                            string testIndexStr = "";
                            foreach (var item in dataChilds)
                            {
                                testIndexStr += item.TEST_INDEX_NAME + " - " + item.VALUE_RANGE + "; ";
                            }

                            string message = string.Format("Trả kết quả từng phần. SERVICE_REQ_CODE: {0}. BARCODE: {1}.  TEST_INDEX_NAME - VALUE: [{2}]", rowSample.SERVICE_REQ_CODE, rowSample.BARCODE, testIndexStr);
                            string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                            SdaEventLogCreate eventlog = new SdaEventLogCreate();
                            eventlog.Create(login, null, true, message);
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else
                    {
                        MessageManager.Show(String.Format("({0}) chưa nhập giá trị cho chỉ số.", testLisResultADO.TEST_INDEX_NAME));
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TextEditValueRange__Enable_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    treeListSereServTein.PostEditor();
                    TreeListNode node = treeListSereServTein.FocusedNode;
                    if (node != null && node.NextNode != null)
                    {
                        if (node.NextVisibleNode != null && node.NextVisibleNode.HasChildren)
                        {
                            treeListSereServTein.FocusedNode = node.NextVisibleNode.Nodes[0];
                        }
                        else if (node.NextVisibleNode.NextNode != null)
                        {
                            if (node.NextVisibleNode.NextVisibleNode != null
                                && node.NextVisibleNode.NextVisibleNode.HasChildren)
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode.Nodes[0];
                            }
                            else
                            {
                                treeListSereServTein.FocusedNode = node.NextNode;//.NextVisibleNode.NextNode;
                            }
                        }
                        else
                            treeListSereServTein.FocusedNode = node.NextNode;
                    }
                    else if (node != null && node.NextVisibleNode != null)
                    {
                        if (node.NextVisibleNode.HasChildren)
                        {
                            treeListSereServTein.FocusedNode = node.NextVisibleNode.Nodes[0];
                        }
                        else if (node.NextVisibleNode.NextNode != null)
                        {
                            if (node.NextVisibleNode.NextVisibleNode != null && node.NextVisibleNode.NextVisibleNode.HasChildren)
                            {
                                if (node.NextVisibleNode.NextVisibleNode.NextVisibleNode != null
                                    && node.NextVisibleNode.NextVisibleNode.NextVisibleNode.HasChildren)
                                {
                                    treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode.NextVisibleNode.Nodes[0];
                                }
                                else
                                    treeListSereServTein.FocusedNode = node.NextVisibleNode;
                            }
                            else if (node.NextVisibleNode.NextVisibleNode != null)
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode;
                            }
                            else
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode;
                            }
                        }
                    }
                    treeListSereServTein.FocusedColumn = treeListSereServTein.Columns[grdColVallue.FieldName];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemText__Description_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    treeListSereServTein.PostEditor();
                    //treeListSereServTein.FocusedColumn = treeListSereServTein.Columns[grdColVallue.FieldName];
                    TreeListNode node = treeListSereServTein.FocusedNode;
                    if (node != null && node.NextNode != null)
                    {
                        if (node.NextVisibleNode != null && node.NextVisibleNode.HasChildren)
                        {
                            treeListSereServTein.FocusedNode = node.NextVisibleNode.Nodes[0];
                        }
                        else if (node.NextVisibleNode.NextNode != null)
                        {
                            if (node.NextVisibleNode.NextVisibleNode != null
                                && node.NextVisibleNode.NextVisibleNode.HasChildren)
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode.Nodes[0];
                            }
                            else
                            {
                                treeListSereServTein.FocusedNode = node.NextNode;//.NextVisibleNode.NextNode;
                            }
                        }
                        else
                            treeListSereServTein.FocusedNode = node.NextNode;
                    }
                    else if (node != null && node.NextVisibleNode != null)
                    {
                        if (node.NextVisibleNode.HasChildren)
                        {
                            treeListSereServTein.FocusedNode = node.NextVisibleNode.Nodes[0];
                        }
                        else if (node.NextVisibleNode.NextNode != null)
                        {
                            if (node.NextVisibleNode.NextVisibleNode != null && node.NextVisibleNode.NextVisibleNode.HasChildren)
                            {
                                if (node.NextVisibleNode.NextVisibleNode.NextVisibleNode != null
                                    && node.NextVisibleNode.NextVisibleNode.NextVisibleNode.HasChildren)
                                {
                                    treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode.NextVisibleNode.Nodes[0];
                                }
                                else
                                    treeListSereServTein.FocusedNode = node.NextVisibleNode;
                            }
                            else if (node.NextVisibleNode.NextVisibleNode != null)
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode.NextVisibleNode;
                            }
                            else
                            {
                                treeListSereServTein.FocusedNode = node.NextVisibleNode;
                            }
                        }
                    }
                    treeListSereServTein.FocusedColumn = treeListSereServTein.Columns[treeListColDescription.FieldName];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ItemTextEdit_BarCodeEnable_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var focus = (LIS_SAMPLE)gridViewSample.GetFocusedRow();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSample_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BARCODE")
                {
                    var focus = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                    if (LisConfigCFG.IS_AUTO_CREATE_BARCODE == "0" && focus.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                    {
                        var sampleStt = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSample.GetRowCellValue(e.RowHandle, "SAMPLE_STT_ID")).ToString());
                        LisSampleUpdateBarcodeSDO input = new LisSampleUpdateBarcodeSDO();
                        input.SampleId = focus.ID;
                        input.Barcode = focus.BARCODE;
                        CommonParam param = new CommonParam();
                        bool success = false;
                        WaitingManager.Show();
                        var rs = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/UpdateBarcode", ApiConsumers.LisConsumer, input, param);
                        if (rs != null)
                        {
                            success = true;
                            lblNewestBarcode.Text = focus.BARCODE;
                        }
                        else
                        {
                            FillDataToGridControl();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSample_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_LIS_SAMPLE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "BARCODE")
                        {

                            if (LisConfigCFG.IS_AUTO_CREATE_BARCODE == "0" && data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                            {

                            }
                            else
                            {
                                e.Appearance.BackColor = Color.LightGray;
                            }
                        }

                        if (HisConfigCFG.WARNING_TIME_RETURN_RESULT == "1"
                            && data.APPOINTMENT_TIME.HasValue && data.APPOINTMENT_TIME < Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now)
                            && (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ))
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListSereServTein_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                if (ModifierKeys == Keys.None
                    && tree.State == TreeListState.Regular)
                {
                    Point pt = tree.PointToClient(MousePosition);
                    TreeListHitInfo info = tree.CalcHitInfo(pt);
                    if (info.HitInfoType == HitInfoType.Cell || info.HitInfoType == HitInfoType.RowIndicator)
                        tree.FocusedNode = info.Node;

                    if (tree.FocusedNode != null)
                    {
                        TestLisResultADO data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);
                        if (data.IS_PARENT != 1)
                        {
                            return;
                        }
                        if (data.IS_NO_EXECUTE == 1)
                        {
                            // bỏ không thực hiện
                            DXMenuItem menuItem = new DXMenuItem("Bỏ không thực hiện", new EventHandler(this.MenuItemClick_BoKhongThucHien), imageCollection1.Images[9]);
                            menuItem.Tag = info.Column;
                            e.Menu.Items.Add(menuItem);
                        }
                        else
                        {
                            DXMenuItem menuItem = new DXMenuItem("Không thực hiện", new EventHandler(this.MenuItemClick_KhongThucHien), imageCollection1.Images[10]);
                            menuItem.Tag = info.Column;
                            e.Menu.Items.Add(menuItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MenuItemClick_BoKhongThucHien(object sender, EventArgs e)
        {
            try
            {
                TestLisResultADO data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);

                var ChildData = this.lstSampleServiceADOs.Where(o => o.PARENT_ID == data.CHILD_ID || (data.HAS_ONE_CHILD == 1 && o.CHILD_ID == data.CHILD_ID));

                if (MessageBox.Show(String.Format("Bạn có xác nhận dịch vụ {0} bỏ không thực hiện không?", data.SERVICE_NAME), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    UpdateSampleServiceNoExecuteSDO updateSampleServiceNoExecuteSDO = new LIS.SDO.UpdateSampleServiceNoExecuteSDO();
                    updateSampleServiceNoExecuteSDO.ServiceCodes = new List<string>();
                    updateSampleServiceNoExecuteSDO.ServiceCodes.Add(data.SERVICE_CODE);
                    updateSampleServiceNoExecuteSDO.ServiceReqCode = rowSample.SERVICE_REQ_CODE;
                    updateSampleServiceNoExecuteSDO.IsNoExecute = false;

                    CommonParam param = new CommonParam();
                    bool success = false;
                    WaitingManager.Show();
                    var result = new BackendAdapter(param).Post<bool>("api/LisSampleService/UpdateNoExecute", ApiConsumers.LisConsumer, updateSampleServiceNoExecuteSDO, param);
                    if (result)
                    {
                        success = true;
                        gridViewSample_RowCellClick(null, null);
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MenuItemClick_KhongThucHien(object sender, EventArgs e)
        {
            try
            {
                TestLisResultADO data = (TestLisResultADO)this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode);

                var ChildData = this.lstSampleServiceADOs.Where(o => o.PARENT_ID == data.CHILD_ID || (data.HAS_ONE_CHILD == 1 && o.CHILD_ID == data.CHILD_ID));

                var childHasValue = ChildData.Where(o => !String.IsNullOrWhiteSpace(o.VALUE_RANGE));
                if (childHasValue != null && childHasValue.Count() > 0)
                {
                    string message = String.Join("; ", childHasValue.Select(o => o.TEST_INDEX_NAME));
                    MessageManager.Show(String.Format("Các chỉ số đã có giá trị: {0} ", message));
                    return;
                }

                if (MessageBox.Show(String.Format("Bạn có xác nhận dịch vụ {0} không thực hiện không?", data.SERVICE_NAME), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    UpdateSampleServiceNoExecuteSDO updateSampleServiceNoExecuteSDO = new LIS.SDO.UpdateSampleServiceNoExecuteSDO();
                    updateSampleServiceNoExecuteSDO.ServiceCodes = new List<string>();
                    updateSampleServiceNoExecuteSDO.ServiceCodes.Add(data.SERVICE_CODE);
                    updateSampleServiceNoExecuteSDO.ServiceReqCode = rowSample.SERVICE_REQ_CODE;
                    updateSampleServiceNoExecuteSDO.IsNoExecute = true;

                    CommonParam param = new CommonParam();
                    bool success = false;
                    WaitingManager.Show();
                    var result = new BackendAdapter(param).Post<bool>("api/LisSampleService/UpdateNoExecute", ApiConsumers.LisConsumer, updateSampleServiceNoExecuteSDO, param);
                    if (result)
                    {
                        success = true;
                        gridViewSample_RowCellClick(null, null);
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListSereServTein_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                Point pt = new Point(e.X, e.Y);
                TreeListHitInfo hit = tree.CalcHitInfo(pt);
                if (hit.Column != null && hit.Column.VisibleIndex == 0)
                {
                    DevExpress.XtraTreeList.ViewInfo.ColumnInfo info = tree.ViewInfo.ColumnsInfo[hit.Column];
                    Rectangle checkRect = new Rectangle(info.Bounds.Left + 3, info.Bounds.Top + 3, 12, 12);
                    if (checkRect.Contains(pt))
                    {
                        hit.Column.OptionsColumn.AllowSort = false;
                        EmbeddedCheckBoxChecked(tree);
                    }
                    else
                    {
                        hit.Column.OptionsColumn.AllowSort = true;
                    }
                }
                else if (e.Button == MouseButtons.Right && ModifierKeys == Keys.None
                    && tree.State == TreeListState.Regular && (hit.HitInfoType == HitInfoType.Cell || hit.HitInfoType == HitInfoType.RowIndicator))
                {
                    tree.FocusedNode = hit.Node;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlSample_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                ColumnView view = (sender as GridControl).FocusedView as ColumnView;
                if (view == null) return;

                rowSample = null;

                //if (e.KeyCode != Keys.Down || e.KeyCode != Keys.Up)
                //{
                //    return;
                //}

                if (e.KeyCode == Keys.Down)
                {
                    rowSample = (V_LIS_SAMPLE)view.GetRow(view.FocusedRowHandle + 1);
                }
                else if (e.KeyCode == Keys.Up)
                {
                    rowSample = (V_LIS_SAMPLE)view.GetRow(view.FocusedRowHandle - 1);
                }
                else
                {
                    return;
                }

                if (rowSample == null)
                    return;

                if (view.ActiveEditor != null) return;//Prevent record deletion when an in-place editor is invoked
                WaitingManager.Show();
                LoadLisResult(rowSample);
                LoadDataToGridTestResultWithVKKS();
                SetDataToCommon(rowSample);
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                {
                    btnSave.Enabled = false;
                }
                else
                {
                    btnSave.Enabled = true;
                }

                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryTuChoiMauE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var row = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                if (row != null && (row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                    || row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN))
                {
                    frmReasonReject frm = new frmReasonReject(row);
                    frm.ShowDialog();
                    FillDataToGridControl();
                    gridViewSample.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryChapNhanMauE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                var row = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                if (row != null && row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM)
                {
                    if (LisConfigCFG.SHOW_FORM_SAMPLE_INFO == "1"
                        && (!row.SAMPLE_TIME.HasValue
                        || String.IsNullOrWhiteSpace(row.SAMPLE_LOGINNAME)
                        || !row.SAMPLE_TYPE_ID.HasValue))
                    {
                        WaitingManager.Hide();
                        var rs = XtraMessageBox.Show("Chưa có thông tin lấy mẫu. Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo, DefaultBoolean.True);
                        if (rs != DialogResult.Yes)
                        {
                            return;
                        }
                        WaitingManager.Show();
                    }
                    LisSampleApproveSDO sdo = new LisSampleApproveSDO();
                    sdo.SampleId = row.ID;
                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Approve", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                    if (curentSTT != null)
                    {
                        WaitingManager.Hide();
                        FillDataToGridControl();
                        result = true;
                        gridViewSample.RefreshData();
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryDuyetKetQuaE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                var row = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                if (row != rowSample)
                {
                    rowSample = row;
                    LoadLisResult(rowSample);
                    LoadDataToGridTestResultWithVKKS();
                    SetDataToCommon(rowSample);
                }
                if (row != null && (row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                    || row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN))
                {
                    var dataChildNullValue = lstSampleServiceADOs != null && lstSampleServiceADOs.Count() > 0 ?
                        lstSampleServiceADOs.Where(o => (o.IS_PARENT != 1 || (o.IS_PARENT == 1 && o.HAS_ONE_CHILD == 1)) && String.IsNullOrWhiteSpace(o.VALUE_RANGE)).ToList()
                        : null;

                    if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "2" && dataChildNullValue != null && dataChildNullValue.Count() > 0
                        && MessageBox.Show(String.Format("Chỉ số ({0}) chưa có kết quả, bạn có muốn duyệt kết quả không?", String.Join("; ", dataChildNullValue.Select(o => o.TEST_INDEX_NAME))), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                    else if (LisConfigCFG.IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT == "0" && dataChildNullValue != null && dataChildNullValue.Count() > 0)
                    {
                        MessageBox.Show(String.Format("Chỉ số ({0}) chưa có kết quả", String.Join("; ", dataChildNullValue.Select(o => o.TEST_INDEX_NAME))), "Thông báo");
                        return;
                    }
                    if (LisConfigCFG.ALLOW_TO_EDIT_APPROVE_RESULT_TIME)
                    {
                        WaitingManager.Hide();
                        frmEditApproveResultTime frm = new frmEditApproveResultTime(getResultFromDialog, row.ID);
                        frm.ShowDialog();
                    }
                    else
                    {
                        LisSampleApproveResultSDO lisSampleApproveResultSDO = new LisSampleApproveResultSDO();
                        lisSampleApproveResultSDO.SampleId = row.ID;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/ApproveResult", ApiConsumer.ApiConsumers.LisConsumer, lisSampleApproveResultSDO, param);
                        if (curentSTT != null)
                        {
                            FillDataToGridControl();
                            result = true;
                            gridViewSample.RefreshData();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, result);
                        #endregion
                    }
                }

                if (result && LisConfigCFG.IS_PRINT_WHEN_APPROVE_RESULT == "1")
                {
                    this.PrintOption = PRINT_OPTION.IN;
                    this.PrintProcess(PrintTypeKXN.IN_KET_QUA_XET_NGHIEM);
                }

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void getResultFromDialog(bool rs)
        {
            try
            {
                if (rs)
                {
                    FillDataToGridControl();
                    gridViewSample.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void repositoryHuyDuyetKetQuaE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                var row = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                if (row != null && (row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ))
                {
                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/UnapproveResult", ApiConsumer.ApiConsumers.LisConsumer, row.ID, param);
                    if (curentSTT != null)
                    {
                        WaitingManager.Hide();
                        FillDataToGridControl();
                        result = true;
                        gridViewSample.RefreshData();
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSample_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    var row = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                    if (this.baManager == null)
                    {
                        this.baManager = new BarManager();
                        this.baManager.Form = this;
                    }
                    if (row != null)
                    {
                        this.popupMenuProcessor = new PopupMenuProcessor(row, this.baManager, MouseRightItemClick);
                        this.popupMenuProcessor.InitMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MouseRightItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var row = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                if ((e.Item is BarButtonItem) && row != null)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.CapNhatTinhTrangMau:
                            this.CapNhatTinhTrangMau(row);
                            break;
                        case PopupMenuProcessor.ItemType.LichSuXetNghiem:
                            this.LichSuXetNghiemCuaBenhNha(row);
                            break;
                        default:
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if ((e.Item is BarButtonItem))
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.AmTinh:
                            this.AmTinh();
                            break;
                        case PopupMenuProcessor.ItemType.DuongTinh:
                            this.DuongTinh();
                            break;
                        default:
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DuongTinh()
        {
            try
            {
                //TODO
                if (this.gridViewVSKS.IsEditing)
                    this.gridViewVSKS.CloseEditor();

                if (this.gridViewVSKS.FocusedRowModified)
                    this.gridViewVSKS.UpdateCurrentRow();

                TestLisResultADO testLisResultADO = (TestLisResultADO)gridViewVSKS.GetFocusedRow();
                if (testLisResultADO != null && testLisResultADO.SAMPLE_SERVICE_ID > 0 && testLisResultADO.ErrorTypeLabCode == ErrorType.None)
                {
                    LisSampleServiceSDO sampleServiceResultSDO = new LisSampleServiceSDO();
                    sampleServiceResultSDO.Id = testLisResultADO.SAMPLE_SERVICE_ID.Value;
                    sampleServiceResultSDO.MicrobiologicalResult = "Dương tính";
                    sampleServiceResultSDO.LaboratoryCode = testLisResultADO.LABORATORY_CODE;
                    CommonParam param = new CommonParam();
                    LIS_SAMPLE_SERVICE resultData = new BackendAdapter(param)
                        .Post<LIS_SAMPLE_SERVICE>("api/LisSampleService/BacteriumCulture", ApiConsumers.LisConsumer, sampleServiceResultSDO, param);
                    WaitingManager.Hide();
                    if (resultData != null)
                    {
                        GetSampleServiceBySample(this.rowSample);
                        CapNhatThoiGianLayMau();
                        if (!sampleServiceResultSDO.IsNegativePositive.HasValue || (!sampleServiceResultSDO.IsNegativePositive.Value && testLisResultADO.LResultDetails != null && testLisResultADO.LResultDetails.Count > 0))
                        {
                            foreach (var item in testLisResultADO.LResultDetails)
                            {
                                if (item.ID > 0)
                                {
                                    CommonParam paramDel = new CommonParam();
                                    bool resultDataDel = new BackendAdapter(paramDel)
                                        .Post<bool>("api/LisResult/Delete", ApiConsumers.LisConsumer, item.ID, paramDel);
                                }

                                lstLisResultDetailForTreeADOs.RemoveAll(o => o.SAMPLE_SERVICE_ID == testLisResultADO.SAMPLE_SERVICE_ID);
                            }
                            lstLisResultDetailForTreeADOs.Remove(testLisResultADO);
                            treeListSereServTein.RefreshDataSource();
                            LoadLisResult(this.rowSample);
                            this.currentTestSamResultADO.LResultDetails = null;
                        }
                    }
                    //else
                    //{
                    Inventec.Common.Logging.LogSystem.Debug("Call api api/LisSampleService/BacteriumCulture " + (resultData != null) + " => " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sampleServiceResultSDO), sampleServiceResultSDO));
                    //}
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("repositoryItemcboMachineReturnResult_EditValueChanged Khong co du lieu sampleservice => " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => testLisResultADO), testLisResultADO) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sampleServices), sampleServices));
                }

                gridViewSample_RowCellClick(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void AmTinh()
        {
            try
            {
                //TODO
                if (this.gridViewVSKS.IsEditing)
                    this.gridViewVSKS.CloseEditor();

                if (this.gridViewVSKS.FocusedRowModified)
                    this.gridViewVSKS.UpdateCurrentRow();

                TestLisResultADO testLisResultADO = (TestLisResultADO)gridViewVSKS.GetFocusedRow();
                if (testLisResultADO != null && testLisResultADO.SAMPLE_SERVICE_ID > 0 && testLisResultADO.ErrorTypeLabCode == ErrorType.None)
                {
                    LisSampleServiceSDO sampleServiceResultSDO = new LisSampleServiceSDO();
                    sampleServiceResultSDO.Id = testLisResultADO.SAMPLE_SERVICE_ID.Value;
                    sampleServiceResultSDO.MicrobiologicalResult = "Âm tính";
                    sampleServiceResultSDO.LaboratoryCode = testLisResultADO.LABORATORY_CODE;
                    CommonParam param = new CommonParam();
                    LIS_SAMPLE_SERVICE resultData = new BackendAdapter(param)
                        .Post<LIS_SAMPLE_SERVICE>("api/LisSampleService/BacteriumCulture", ApiConsumers.LisConsumer, sampleServiceResultSDO, param);
                    WaitingManager.Hide();
                    if (resultData != null)
                    {
                        GetSampleServiceBySample(this.rowSample);
                        CapNhatThoiGianLayMau();
                        if (!sampleServiceResultSDO.IsNegativePositive.HasValue || (!sampleServiceResultSDO.IsNegativePositive.Value && testLisResultADO.LResultDetails != null && testLisResultADO.LResultDetails.Count > 0))
                        {
                            foreach (var item in testLisResultADO.LResultDetails)
                            {
                                if (item.ID > 0)
                                {
                                    CommonParam paramDel = new CommonParam();
                                    bool resultDataDel = new BackendAdapter(paramDel)
                                        .Post<bool>("api/LisResult/Delete", ApiConsumers.LisConsumer, item.ID, paramDel);
                                }

                                lstLisResultDetailForTreeADOs.RemoveAll(o => o.SAMPLE_SERVICE_ID == testLisResultADO.SAMPLE_SERVICE_ID);
                            }
                            lstLisResultDetailForTreeADOs.Remove(testLisResultADO);
                            treeListSereServTein.RefreshDataSource();
                            LoadLisResult(this.rowSample);
                            this.currentTestSamResultADO.LResultDetails = null;
                        }
                    }
                    //else
                    //{
                    Inventec.Common.Logging.LogSystem.Debug("Call api api/LisSampleService/BacteriumCulture " + (resultData != null) + " => " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sampleServiceResultSDO), sampleServiceResultSDO));
                    //}
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("repositoryItemcboMachineReturnResult_EditValueChanged Khong co du lieu sampleservice => " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => testLisResultADO), testLisResultADO) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sampleServices), sampleServices));
                }

                gridViewSample_RowCellClick(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CapNhatThoiGianLayMau()
        {
            try
            {
                if (this.rowSample != null
                            && (this.rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                                || this.rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                                || this.rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI))
                {
                    CommonParam param = new CommonParam();
                    LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                    sdo.SampleId = this.rowSample.ID;
                    sdo.RequestRoomCode = room.ROOM_CODE;
                    sdo.SampleTime = Inventec.Common.TypeConvert.Parse.ToInt64(dtSampleTime.DateTime.ToString("yyyyMMddHHmmss"));
                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                    if (curentSTT != null)
                    {
                        rowSample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                        rowSample.SAMPLE_TYPE_ID = curentSTT.SAMPLE_TYPE_ID;
                        rowSample.SAMPLE_TIME = curentSTT.SAMPLE_TIME;
                        rowSample.SAMPLE_LOGINNAME = curentSTT.SAMPLE_LOGINNAME;
                        rowSample.SAMPLE_USERNAME = curentSTT.SAMPLE_USERNAME;
                        rowSample.SAMPLE_ORDER = curentSTT.SAMPLE_ORDER;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CapNhatTinhTrangMau(V_LIS_SAMPLE data)
        {
            try
            {
                frmUpdateCondition frm = new frmUpdateCondition(this.currentModule, data);
                frm.ShowDialog();
                FillDataToGridControl();
                gridViewSample.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LichSuXetNghiemCuaBenhNha(V_LIS_SAMPLE data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.TestHistory").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.TestHistory'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.TestHistory' is not plugins");
                    List<object> listArgs = new List<object>();
                    listArgs.Add(data.PATIENT_CODE);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewVSKS_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    TestLisResultADO data = (TestLisResultADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data == null)
                        return;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewVSKS_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    string MICROBIOLOGICAL_RESULT = (gridViewVSKS.GetRowCellValue(e.RowHandle, "MICROBIOLOGICAL_RESULT") ?? "").ToString().Trim();

                    if (e.Column.FieldName == "BtnChooseVSKS")
                    {
                        if (!string.IsNullOrEmpty(MICROBIOLOGICAL_RESULT))
                        {
                            e.RepositoryItem = repositoryItembtnChooseKSVS;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemTextEditReadOnly;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemcboMachineReturnResult_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //TODO
                if (this.gridViewVSKS.IsEditing)
                    this.gridViewVSKS.CloseEditor();

                if (this.gridViewVSKS.FocusedRowModified)
                    this.gridViewVSKS.UpdateCurrentRow();

                TestLisResultADO testLisResultADO = (TestLisResultADO)gridViewVSKS.GetFocusedRow();
                if (testLisResultADO != null && testLisResultADO.SAMPLE_SERVICE_ID > 0)
                {
                    LisSampleServiceMachineSDO sampleServiceMachineSDO = new LisSampleServiceMachineSDO();
                    sampleServiceMachineSDO.MachineId = testLisResultADO.MACHINE_ID;
                    sampleServiceMachineSDO.SampleServiceId = testLisResultADO.SAMPLE_SERVICE_ID.Value;

                    CommonParam param = new CommonParam();
                    LIS_SAMPLE_SERVICE resultData = new BackendAdapter(param)
                        .Post<LIS_SAMPLE_SERVICE>("api/LisSampleService/UpdateMachine", ApiConsumers.LisConsumer, sampleServiceMachineSDO, param);
                    WaitingManager.Hide();
                    if (resultData != null)
                    {
                        GetSampleServiceBySample(this.rowSample);
                    }
                    //else
                    //{
                    Inventec.Common.Logging.LogSystem.Debug("Call api api/LisSampleService/UpdateMachine " + (resultData != null) + " => " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultData), resultData) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sampleServiceMachineSDO), sampleServiceMachineSDO));
                    //}
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("repositoryItemcboMachineReturnResult_EditValueChanged Khong co du lieu sampleservice => " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => testLisResultADO), testLisResultADO) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sampleServices), sampleServices));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnChooseKSVS_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                this.currentTestSamResultADO = (TestLisResultADO)gridViewVSKS.GetFocusedRow();
                if (this.currentTestSamResultADO != null && this.currentTestSamResultADO.SAMPLE_SERVICE_ID > 0)
                {
                    frmBacAntiBioticMap frmBacAntiBioticMap = new frmBacAntiBioticMap(this.currentTestSamResultADO, UpdateTestLisResultDetail);
                    frmBacAntiBioticMap.ShowDialog();
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("repositoryItembtnChooseKSVS_ButtonClick Khong co du lieu sampleservice => " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTestSamResultADO), currentTestSamResultADO) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sampleServices), sampleServices));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEdit_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = this.treeListSereServTein.GetDataRecordByNode(this.treeListSereServTein.FocusedNode) as TestLisResultADO;
                if (data != null)
                {
                    if (data.IS_PARENT == 1)
                    {
                        var listDel = lstLisResultDetailForTreeADOs.Where(o => o.PARENT_ID == data.CHILD_ID).ToList();
                        if (listDel != null && listDel.Count > 0)
                        {
                            foreach (var item in listDel)
                            {
                                if (item.ID > 0)
                                {
                                    CommonParam param = new CommonParam();
                                    bool resultData = new BackendAdapter(param)
                                        .Post<bool>("api/LisResult/Delete", ApiConsumers.LisConsumer, item.ID, param);
                                }

                                lstLisResultDetailForTreeADOs.Remove(item);
                            }
                            lstLisResultDetailForTreeADOs.Remove(data);
                            treeListSereServTein.RefreshDataSource();
                            LoadLisResult(this.rowSample);
                            this.currentTestSamResultADO.LResultDetails = null;
                        }
                        else
                        {
                            if (data.ID > 0)
                            {
                                CommonParam param = new CommonParam();
                                LIS_SAMPLE_SERVICE resultData = new BackendAdapter(param)
                                    .Post<LIS_SAMPLE_SERVICE>("api/LisResult/Delete", ApiConsumers.LisConsumer, data.ID, param);
                                WaitingManager.Hide();
                                if (resultData != null)
                                {
                                    LoadLisResult(this.rowSample);
                                    var itemDel = this.currentTestSamResultADO.LResultDetails.Where(o => o.ID == data.ID).FirstOrDefault();
                                    if (itemDel != null)
                                        this.currentTestSamResultADO.LResultDetails.Remove(itemDel);
                                }
                            }
                            lstLisResultDetailForTreeADOs.Remove(data);
                            treeListSereServTein.RefreshDataSource();
                        }
                    }
                    else
                    {
                        if (data.ID > 0)
                        {
                            CommonParam param = new CommonParam();
                            LIS_SAMPLE_SERVICE resultData = new BackendAdapter(param)
                                .Post<LIS_SAMPLE_SERVICE>("api/LisResult/Delete", ApiConsumers.LisConsumer, data.ID, param);
                            WaitingManager.Hide();
                            if (resultData != null)
                            {
                                LoadLisResult(this.rowSample);
                                var itemDel = this.currentTestSamResultADO.LResultDetails.Where(o => o.ID == data.ID).FirstOrDefault();
                                if (itemDel != null)
                                    this.currentTestSamResultADO.LResultDetails.Remove(itemDel);
                            }
                        }

                        lstLisResultDetailForTreeADOs.Remove(data);
                        treeListSereServTein.RefreshDataSource();
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateTestLisResultDetail(List<LisResultADO> _testLisResultADODetails)
        {
            try
            {
                if (this.currentTestSamResultADO == null)
                    this.currentTestSamResultADO = (TestLisResultADO)gridViewVSKS.GetFocusedRow();

                if (this.currentTestSamResultADO.LResultDetails != null && this.currentTestSamResultADO.LResultDetails.Count > 0)
                {
                    var baterium = _testLisResultADODetails.GroupBy(o => o.BACTERIUM_CODE).ToList();
                    foreach (var bater in baterium)
                    {
                        string bacteriumCode = bater.First().BACTERIUM_CODE;
                        var exists = this.currentTestSamResultADO.LResultDetails.Where(o => o.BACTERIUM_CODE == bacteriumCode).ToList();
                        if (exists != null && exists.Count > 0)
                        {
                            foreach (var item in exists)
                            {
                                this.currentTestSamResultADO.LResultDetails.Remove(item);
                            }
                        }
                    }
                    this.currentTestSamResultADO.LResultDetails.AddRange(_testLisResultADODetails);

                }
                else
                {
                    this.currentTestSamResultADO.LResultDetails = _testLisResultADODetails;
                    if (this.currentTestSamResultADO.LResultDetails != null && this.currentTestSamResultADO.LResultDetails.Count > 0)
                    {
                        this.currentTestSamResultADO.LResultDetails = this.currentTestSamResultADO.LResultDetails.OrderBy(o => o.SERVICE_NUM_ORDER).ToList();
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentTestSamResultADO), this.currentTestSamResultADO));
                LoadDataToTreeTestResult2();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewVSKS_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                this.currentTestSamResultADO = (TestLisResultADO)gridViewVSKS.GetFocusedRow();
                if (this.currentTestSamResultADO != null)
                {
                    LoadDataToTreeTestResult2();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListSereServTein_CustomUnboundColumnData(object sender, TreeListCustomColumnDataEventArgs e)
        {
            //_DISPLAY
            try
            {
                if (e.IsGetData)
                {
                    TestLisResultADO data = e.Row as TestLisResultADO;
                    if (data == null) return;
                    if (e.Column.FieldName == "BACTERIUM_CODE_DISPLAY")
                    {
                        e.Value = data.IS_PARENT == 1 ? data.BACTERIUM_CODE : data.ANTIBIOTIC_CODE;
                    }
                    else if (e.Column.FieldName == "BACTERIUM_NAME_DISPLAY")
                    {
                        e.Value = data.IS_PARENT == 1 ? data.BACTERIUM_NAME : data.ANTIBIOTIC_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemcboResultEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (this.baManager == null)
                {
                    this.baManager = new BarManager();
                    this.baManager.Form = this;
                }
                Point pointPopup = Cursor.Position;
                TestLisResultADO testLisResultADO = (TestLisResultADO)gridViewVSKS.GetFocusedRow();
                if (String.IsNullOrEmpty(testLisResultADO.LABORATORY_CODE))
                {
                    if ((XtraMessageBox.Show("Bạn chưa nhập số hiệu mẫu. Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No))
                        return;
                }
                this.popupMenuProcessor = new PopupMenuProcessor(this.baManager, MouseRightClick, pointPopup);
                this.popupMenuProcessor.InitMenuResult();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewVSKS_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                TestLisResultADO testLisResultADO = (TestLisResultADO)gridViewVSKS.GetFocusedRow();

                if (testLisResultADO != null)
                {
                    if (e.Column.FieldName == "LABORATORY_CODE")
                    {
                        if (Encoding.UTF8.GetByteCount(testLisResultADO.LABORATORY_CODE) > 100)
                        {
                            testLisResultADO.ErrorMessageLabCode = "Trường dữ liệu vượt quá ký tự cho phép 100 ký tự"; ;
                            testLisResultADO.ErrorTypeLabCode = ErrorType.Warning;
                        }
                        else
                        {
                            testLisResultADO.ErrorMessageLabCode = "";
                            testLisResultADO.ErrorTypeLabCode = ErrorType.None;
                        }
                    }
                    if (e.Column.FieldName == "MICROBIOLOGICAL_RESULT")
                    {
                        if (String.IsNullOrEmpty(testLisResultADO.LABORATORY_CODE) && XtraMessageBox.Show("Bạn chưa nhập số hiệu mẫu. Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                            return;
                        if (testLisResultADO.SAMPLE_SERVICE_ID > 0 && testLisResultADO.ErrorTypeLabCode == ErrorType.None)
                        {
                            LisSampleServiceSDO sampleServiceResultSDO = new LisSampleServiceSDO();
                            sampleServiceResultSDO.Id = testLisResultADO.SAMPLE_SERVICE_ID.Value;
                            sampleServiceResultSDO.MicrobiologicalResult = testLisResultADO.MICROBIOLOGICAL_RESULT;
                            sampleServiceResultSDO.LaboratoryCode = testLisResultADO.LABORATORY_CODE;
                            CommonParam param = new CommonParam();
                            LIS_SAMPLE_SERVICE resultData = new BackendAdapter(param)
                                .Post<LIS_SAMPLE_SERVICE>("api/LisSampleService/BacteriumCulture", ApiConsumers.LisConsumer, sampleServiceResultSDO, param);
                            WaitingManager.Hide();
                            if (resultData != null)
                            {
                                GetSampleServiceBySample(this.rowSample);
                                CapNhatThoiGianLayMau();
                                if (!sampleServiceResultSDO.IsNegativePositive.HasValue || (!sampleServiceResultSDO.IsNegativePositive.Value && testLisResultADO.LResultDetails != null && testLisResultADO.LResultDetails.Count > 0))
                                {
                                    foreach (var item in testLisResultADO.LResultDetails)
                                    {
                                        if (item.ID > 0)
                                        {
                                            CommonParam paramDel = new CommonParam();
                                            bool resultDataDel = new BackendAdapter(paramDel)
                                                .Post<bool>("api/LisResult/Delete", ApiConsumers.LisConsumer, item.ID, paramDel);
                                        }

                                        lstLisResultDetailForTreeADOs.RemoveAll(o => o.SAMPLE_SERVICE_ID == testLisResultADO.SAMPLE_SERVICE_ID);
                                    }
                                    lstLisResultDetailForTreeADOs.Remove(testLisResultADO);
                                    treeListSereServTein.RefreshDataSource();
                                    LoadLisResult(this.rowSample);
                                    this.currentTestSamResultADO.LResultDetails = null;
                                }
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug("repositoryItemcboMachineReturnResult_EditValueChanged Khong co du lieu sampleservice => " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => testLisResultADO), testLisResultADO) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sampleServices), sampleServices));
                        }

                        gridViewSample_RowCellClick(null, null);
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSignProcess_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isInit)
                {
                    return;
                }
                if (chkSignProcess.Checked)
                    chkSign.Checked = !chkSignProcess.Checked;

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_SIGN_PROCESS && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSignProcess.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_SIGN_PROCESS;
                    csAddOrUpdate.VALUE = (chkSignProcess.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isInit)
                {
                    return;
                }
                if (chkSign.Checked)
                    chkSignProcess.Checked = !chkSign.Checked;

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.CHECK_SIGN && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.CHECK_SIGN;
                    csAddOrUpdate.VALUE = (chkSign.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCreateSigner_Click(object sender, EventArgs e)
        {
            try
            {
                if (SignConfigData == null)
                {
                    SignConfigData = new SignConfigADO();
                }

                HIS.Desktop.Plugins.ReturnMicrobiologicalResults.AddSigner.frmSignerAdd frmAddSigner = new HIS.Desktop.Plugins.ReturnMicrobiologicalResults.AddSigner.frmSignerAdd(SignConfigData.listSign, UpdateAfterAddSignThread, SignConfigData.IsSignParanel);
                frmAddSigner.ShowDialog(this.ParentForm);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateAfterAddSignThread(List<SignTDO> signs, bool signParanel)
        {
            try
            {
                if (signs != null && signs.Count > 0)
                {
                    if (SignConfigData == null)
                    {
                        SignConfigData = new SignConfigADO();
                    }

                    SignConfigData.listSign = signs;
                    SignConfigData.IsSignParanel = signParanel;

                    string value = Newtonsoft.Json.JsonConvert.SerializeObject(SignConfigData);
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == btnCreateSigner.Name && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = value;
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = btnCreateSigner.Name;
                        csAddOrUpdate.VALUE = value;
                        csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationMaxLengthNote()
        {
            try
            {
                ValidationNote validate = new ValidationNote();
                validate.txtNote = mmNote;
                dxValidationProvider1.SetValidationRule(mmNote, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void repositoryItemCboTechnique_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    var view = sender as GridLookUpEdit;
                    view.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewVSKS_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "LABORATORY_CODE")
                {
                    this.gridViewVSKS_CustomRowError(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void gridViewVSKS_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridViewVSKS.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControlVSKS.DataSource as List<TestLisResultADO>;
                var row = listDatas[index];
                if (e.ColumnName == "LABORATORY_CODE")
                {
                    if (row.ErrorTypeLabCode == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeLabCode);
                        e.Info.ErrorText = (string)(row.ErrorMessageLabCode);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtSampleTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (dtSampleTime.EditValue != null && dtSampleTime.DateTime != DateTime.MinValue)
                    currentTimerLM = dtSampleTime.DateTime;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtSampleTime_Leave(object sender, EventArgs e)
        {
            try
            {
                currentTimerLM = dtSampleTime.DateTime;
                dtSampleTime.SelectionStart = 0;
                StartTimer(currentModule.ModuleLink, "timer2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtResultTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (dtResultTime.EditValue != null && dtResultTime.DateTime != DateTime.MinValue)
                    currentTimer = dtResultTime.DateTime;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtResultTime_Leave(object sender, EventArgs e)
        {
            try
            {
                currentTimer = dtResultTime.DateTime;
                dtResultTime.SelectionStart = 0;
                StartTimer(currentModule.ModuleLink, "timer1");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtResultLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!string.IsNullOrEmpty(txtResultLoginName.Text))
                    {
                        cboResultUsername.EditValue = txtResultLoginName.Text;
                    }
                    cboResultUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboResultUsername_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboResultUsername.EditValue != null && cboResultUsername.EditValue != cboResultUsername.OldEditValue)
                    {
                        ACS.EFMODEL.DataModels.ACS_USER acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboResultUsername.EditValue.ToString());
                        if (acsUser != null)
                        {
                            txtResultLoginName.Text = acsUser.LOGINNAME;
                        }
                        WaitingManager.Show();
                        HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == cboResultUsername.Name && o.MODULE_LINK == ControlStateConstant.MODULE_LINK).FirstOrDefault() : null;
                        if (csAddOrUpdate != null)
                        {
                            csAddOrUpdate.VALUE = cboResultUsername.EditValue.ToString();
                        }
                        else
                        {
                            csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                            csAddOrUpdate.KEY = cboResultUsername.Name;
                            csAddOrUpdate.VALUE = cboResultUsername.EditValue.ToString();
                            csAddOrUpdate.MODULE_LINK = ControlStateConstant.MODULE_LINK;
                            if (this.currentBySessionControlStateRDO == null)
                                this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                            this.currentBySessionControlStateRDO.Add(csAddOrUpdate);
                        }
                        this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);
                        WaitingManager.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboResultUsername_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboResultUsername.EditValue != null && cboResultUsername.EditValue != cboResultUsername.OldEditValue)
                {
                    ACS.EFMODEL.DataModels.ACS_USER acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == cboResultUsername.EditValue.ToString());
                    if (acsUser != null)
                    {
                        txtResultLoginName.Text = acsUser.LOGINNAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeListSereServTein_CustomDrawColumnHeader(object sender, CustomDrawColumnHeaderEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column.VisibleIndex == 0)
                {
                    Rectangle checkRect = new Rectangle(e.Bounds.Left + 3, e.Bounds.Top + 3, 12, 12);
                    DevExpress.XtraTreeList.ViewInfo.ColumnInfo info = (DevExpress.XtraTreeList.ViewInfo.ColumnInfo)e.ObjectArgs;
                    if (info.CaptionRect.Left < 30)
                        info.CaptionRect = new Rectangle(new Point(info.CaptionRect.Left + 15, info.CaptionRect.Top), info.CaptionRect.Size);
                    e.Painter.DrawObject(info);

                    DrawCheckBox(e.Cache, repositoryItemCheckAll, checkRect, IsAllSelected(sender as TreeList));
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repUnAppove_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                if (data != null)
                {

                    if (data.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                    {
                        return;
                    }
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    LisSampleApproveSDO sdo = new LisSampleApproveSDO();
                    sdo.SampleId = data.ID;
                    sdo.WorkingRoomId = currentModule.RoomId;
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/UnApprove", ApiConsumers.LisConsumer, sdo, param);
                    if (rs != null)
                    {
                        data.SAMPLE_STT_ID = rs.SAMPLE_STT_ID;
                        data.APPROVAL_TIME = rs.APPROVAL_TIME;
                        data.APPROVAL_LOGINNAME = rs.APPROVAL_LOGINNAME;
                        data.APPROVAL_USERNAME = rs.APPROVAL_USERNAME;
                        data.IS_SAMPLE_ORDER_REQUEST = rs.IS_SAMPLE_ORDER_REQUEST;
                        gridControlSample.RefreshDataSource();
                        gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle - 1;
                        gridViewSample.FocusedRowHandle = gridViewSample.FocusedRowHandle + 1;
                        success = true;
                    }
                    WaitingManager.Hide();
                    if (success)
                    {
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }

                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
