﻿using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SereServTein
{
    public partial class frmSereServTein : HIS.Desktop.Utility.FormBase
    {
        internal List<HIS_SERE_SERV> lstSereServ { get; set; }
        internal List<MPS.Processor.Mps000014.PDO.SereServNumOder> _SereServNumOders { get; set; }
        internal List<ADO.HisSereServTeinSDO> lstHisSereServTeinSDO { get; set; }
        internal List<ADO.HisSereServTeinSDO> lstSereServTein { get; set; }
        internal List<ADO.HisSereServTeinSDO> HisSereServTeinSDOs { get; set; }
        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE> currentSereServFiles;
        internal MOS.EFMODEL.DataModels.HIS_SERE_SERV currentSereServ;
        internal MOS.EFMODEL.DataModels.HIS_SERVICE_REQ currentServiceReq;
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT sereServExt;
        SAR.EFMODEL.DataModels.SAR_PRINT currentSarPrint;
        List<ADO.ImageADO> imageLoad;
        internal HIS_TREATMENT currentTreatment { get; set; }
        internal HIS_DHST currentDhst { get; set; }


        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.SereServTein";

        public frmSereServTein(Inventec.Desktop.Common.Modules.Module currentModule, MOS.EFMODEL.DataModels.HIS_SERE_SERV sereServ)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.currentSereServ = sereServ;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmSereServTein_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                zoomFactor();
                CommonParam param = new CommonParam();
                if ((this.currentSereServ.SERVICE_REQ_ID ?? 0) <= 0)
                {
                    MOS.Filter.HisSereServFilter filter1 = new MOS.Filter.HisSereServFilter();
                    filter1.ID = this.currentSereServ.ID;
                    var sereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, filter1, param);
                    if (sereServ != null && sereServ.Count == 1)
                    {
                        this.currentSereServ = sereServ.First();
                    }
                }

                //gán mặc định =0 để không get được dữ liệu thay vì get full
                if (!this.currentSereServ.SERVICE_REQ_ID.HasValue)
                {
                    this.currentSereServ.SERVICE_REQ_ID = 0;
                }

                if (this.currentSereServ != null && this.currentSereServ.ID > 0)
                {
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.ID = this.currentSereServ.SERVICE_REQ_ID;
                    this.currentServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                    lblNote.Text = this.currentServiceReq != null ? this.currentServiceReq.NOTE : "";
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this.currentSereServ", this.currentSereServ));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this.currentServiceReq", this.currentServiceReq));

                    Inventec.Common.Logging.LogSystem.Debug("get__Treatment__");
                    MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.ID = this.currentServiceReq.TREATMENT_ID;
                    currentTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
                    if (currentTreatment != null)
                    {
                        HisDhstFilter dhstFilter = new HisDhstFilter();
                        dhstFilter.TREATMENT_ID = currentTreatment.ID;
                        var dhsts = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumer.ApiConsumers.MosConsumer, dhstFilter, null);
                        if (dhsts != null && dhsts.Count() > 0)
                        {
                            int age = this.Age(currentTreatment.TDL_PATIENT_DOB);
                            if (age >= 17)
                            {
                                var datadhst = dhsts.Where(o => o.WEIGHT != null).ToList();
                                if (datadhst != null && datadhst.Count() > 0)
                                {
                                    currentDhst = datadhst.OrderByDescending(o => o.EXECUTE_TIME).ThenByDescending(o => o.ID).First();
                                }
                            }
                            if (1 <= age && age < 17)
                            {
                                var datadhst = dhsts.Where(o => o.HEIGHT != null).ToList();
                                if (datadhst != null && datadhst.Count() > 0)
                                {
                                    currentDhst = datadhst.OrderByDescending(o => o.EXECUTE_TIME).ThenByDescending(o => o.ID).First();
                                }
                            }
                            if (age < 1)
                            {
                                var datadhst = dhsts.Where(o => o.WEIGHT != null && o.HEIGHT != null).ToList();
                                if (datadhst != null && datadhst.Count() > 0)
                                {
                                    currentDhst = datadhst.OrderByDescending(o => o.EXECUTE_TIME).ThenByDescending(o => o.ID).First();
                                }
                            }
                        }
                    }

                    LoadDataToGridV2();
                    LoadDataBySereServId();
                    SetDefaultTabPage();
                }
                PacsCFG.LoadConfig();
                ShowOldValueColume();
                InitControlState();
                MOS.Filter.HisSereServExtFilter filter2 = new MOS.Filter.HisSereServExtFilter();
                filter2.SERE_SERV_ID = this.currentSereServ.ID;
                var sereServExt = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, filter2, param);
                if (sereServExt != null && sereServExt.Count > 0)
                {
                    lblConclude.Text = sereServExt.First().CONCLUDE;
                    lblDescription.Text = sereServExt.First().DESCRIPTION;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this.sereServExt", sereServExt.First().CONCLUDE));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowOldValueColume()
        {
            try
            {
                if ((PacsCFG.MosLisInterGrationVersion == "1" && PacsCFG.MosLisInterGrationOption == "1") || (PacsCFG.MosLisInterGrationVersion == "2" && PacsCFG.MosLisInterGrationType == "1"))
                {
                    gridOldValue.Visible = true;
                }
                else
                {
                    gridOldValue.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultTabPage()
        {
            try
            {
                MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                filter.ID = this.currentSereServ.SERVICE_REQ_ID;
                var lstServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (lstServiceReq != null && lstServiceReq.Count == 1)
                {
                    MOS.Filter.HisExeServiceModuleFilter exeFilter = new HisExeServiceModuleFilter();
                    exeFilter.ID = lstServiceReq[0].EXE_SERVICE_MODULE_ID;
                    var ExeServiceModule = new BackendAdapter(new CommonParam()).Get<List<HIS_EXE_SERVICE_MODULE>>("api/HisExeServiceModule/Get", ApiConsumer.ApiConsumers.MosConsumer, exeFilter, null);

                    if (ExeServiceModule != null && ExeServiceModule.Count() == 1 && ExeServiceModule[0].MODULE_LINK == "HIS.Desktop.Plugins.TestServiceReqExcute")
                    {
                        xtraTabControl1.SelectedTabPageIndex = 0;
                        //LoadDataToGridV2();
                    }
                    else
                    {
                        xtraTabControl1.SelectedTabPageIndex = 1;
                        //LoadDataBySereServId();
                    }
                }
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SereServTein.Resources.Lang", typeof(HIS.Desktop.Plugins.SereServTein.frmSereServTein).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSereServTein.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageTestIndex.Text = Inventec.Common.Resource.Get.Value("frmSereServTein.xtraTabPageTestIndex.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmSereServTein.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmSereServTein.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmSereServTein.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmSereServTein.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmSereServTein.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmSereServTein.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmSereServTein.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmSereServTein.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageOther.Text = Inventec.Common.Resource.Get.Value("frmSereServTein.xtraTabPageOther.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmSereServTein.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrintServiceReq.Text = Inventec.Common.Resource.Get.Value("frmSereServTein.btnPrintServiceReq.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnEmr.Text = Inventec.Common.Resource.Get.Value("frmSereServTein.BtnEmr.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmSereServTein.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Print.Caption = Inventec.Common.Resource.Get.Value("frmSereServTein.barButtonItem__Print.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConclude.Text = Inventec.Common.Resource.Get.Value("frmSereServTein.lciConclude.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNote.Text = Inventec.Common.Resource.Get.Value("frmSereServTein.lciNote.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServTein_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ADO.HisSereServTeinSDO data = (ADO.HisSereServTeinSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;// +((ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServTein_RowCellStyle_1(object sender, RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView vw = (sender as DevExpress.XtraGrid.Views.Grid.GridView);
            try
            {
                ADO.HisSereServTeinSDO data = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (data.IS_PARENT == 1)
                    {
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }
                    else if (e.Column.FieldName == "TEST_INDEX_NAME" && data.IS_IMPORTANT > 0)
                    {
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }
                    else if (!string.IsNullOrEmpty(data.RESULT_CODE))
                    {
                        if (data.RESULT_CODE == "1")
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                        else if (data.RESULT_CODE == "2")
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
                //if (e.RowHandle >= 0)
                //{
                //    long is_parent = Inventec.Common.TypeConvert.Parse.ToInt64(gridViewSereServTein.GetRowCellValue(e.RowHandle, "IS_PARENT").ToString());
                //    if (is_parent == 1)
                //    {
                //        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                //    }

                //    var resultCode = vw.GetRowCellValue(e.RowHandle, "RESULT_CODE");
                //    if (e.Column.FieldName == "VALUE")
                //    {
                //        if (Convert.ToInt64(resultCode) == 1 || Convert.ToInt64(resultCode) == 2)
                //        {
                //            e.Appearance.ForeColor = Color.Red;
                //            e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadDataToGridV2()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("LoadDataToGridV2.1");
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                MOS.Filter.HisSereServViewFilter filter = new MOS.Filter.HisSereServViewFilter();
                //filter.ORDER_FIELD = "SERVICE_NUM_ORDER";
                //filter.ORDER_DIRECTION = "DESC";
                filter.SERVICE_REQ_ID = this.currentSereServ.SERVICE_REQ_ID;
                filter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;
                this.lstSereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Debug("LoadDataToGridV2.2");

                List<long> sereServIds = new List<long>();
                if (this.lstSereServ != null && this.lstSereServ.Count > 0)
                {
                    _SereServNumOders = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                    foreach (var itemss in this.lstSereServ)
                    {
                        MPS.Processor.Mps000014.PDO.SereServNumOder sereServNumOder = new MPS.Processor.Mps000014.PDO.SereServNumOder(itemss, BackendDataWorker.Get<V_HIS_SERVICE>());
                        _SereServNumOders.Add(sereServNumOder);
                    }
                    _SereServNumOders = _SereServNumOders.OrderByDescending(p => p.SERVICE_NUM_ODER).ThenBy(p => p.TDL_SERVICE_NAME).ToList();
                    Inventec.Common.Logging.LogSystem.Debug("LoadDataToGridV2.3");

                    sereServIds = this.lstSereServ.Select(p => p.ID).ToList();
                    List<HIS_SERE_SERV_EXT> listSereServExt = GetListSereServExtBySereServIds(sereServIds);
                    HisSereServTeinViewFilter sereSerTeinFilter = new HisSereServTeinViewFilter();
                    sereSerTeinFilter.SERE_SERV_IDs = sereServIds;
                    sereSerTeinFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    //sereSerTeinFilter.ORDER_FIELD = "NUM_ORDER";
                    //sereSerTeinFilter.ORDER_DIRECTION = "DESC";

                    this.lstSereServTein = await new BackendAdapter(param).GetAsync<List<ADO.HisSereServTeinSDO>>(HisRequestUriStore.HIS_SERE_SERV_TEIN_GET, ApiConsumers.MosConsumer, sereSerTeinFilter, param);
                    this.lstHisSereServTeinSDO = new List<ADO.HisSereServTeinSDO>();
                    Inventec.Common.Logging.LogSystem.Debug("LoadDataToGridV2.4");

                    var TestIndexDatas = BackendDataWorker.Get<HIS_TEST_INDEX>().Where(o => o.IS_TO_CALCULATE_EGFR == 1).ToList();
                    var testIndexIds = TestIndexDatas.Select(o => o.ID).ToList();
                    HisSereServTeinFilter filterSereServTeinToCheck = new HisSereServTeinFilter();
                    filterSereServTeinToCheck.TDL_TREATMENT_ID = this.currentServiceReq.TREATMENT_ID;
                    filterSereServTeinToCheck.TEST_INDEX_IDs = testIndexIds;
                    var sereServTeinToCheck = new BackendAdapter(param).Get<List<HIS_SERE_SERV_TEIN>>("/api/HisSereServTein/Get", ApiConsumers.MosConsumer, filterSereServTeinToCheck, param);

                    if (sereServTeinToCheck != null && sereServTeinToCheck.Count() > 0)
                    {
                        var sereServTein = sereServTeinToCheck.Where(o => !String.IsNullOrEmpty(o.VALUE)).OrderByDescending(o => o.MODIFY_TIME).ThenByDescending(o => o.ID).FirstOrDefault();
                        var testIndex = TestIndexDatas.FirstOrDefault(o => o.ID == (sereServTein.TEST_INDEX_ID ?? 0));
                        if (sereServTein != null && currentDhst != null && currentTreatment != null && testIndex != null)
                        {
                            decimal chiso;
                            string ssTeinVL = sereServTein.VALUE.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                             .Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                            if (Decimal.TryParse(ssTeinVL, out chiso))
                            {
                                if (testIndex.CONVERT_RATIO_MLCT.HasValue)
                                    chiso *= (testIndex.CONVERT_RATIO_MLCT ?? 0);
                                decimal mlct = Inventec.Common.Calculate.Calculation.MucLocCauThan(currentTreatment.TDL_PATIENT_DOB, currentDhst.WEIGHT ?? 0, currentDhst.HEIGHT ?? 0, chiso, currentTreatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE);
                                lblMlct.Text = mlct > 0 ? mlct.ToString() : "";
                                
                            }
                        }
                    }

                    foreach (var item in _SereServNumOders)
                    {
                        ADO.HisSereServTeinSDO hisSereServTeinSDO = new ADO.HisSereServTeinSDO();
                        hisSereServTeinSDO.IS_PARENT = 1;
                        hisSereServTeinSDO.TEST_INDEX_CODE = item.TDL_SERVICE_CODE;
                        hisSereServTeinSDO.TEST_INDEX_NAME = item.TDL_SERVICE_NAME;
                        hisSereServTeinSDO.SERE_SERV_ID = item.ID;
                        hisSereServTeinSDO.SERVICE_ID = item.SERVICE_ID;
                        hisSereServTeinSDO.IS_NO_EXECUTE = (item.IS_NO_EXECUTE == 1);
                        hisSereServTeinSDO.REQUEST_LOGINNAME = item.TDL_REQUEST_LOGINNAME;
                        hisSereServTeinSDO.EXECUTE_LOGINNAME = this.currentServiceReq != null ? this.currentServiceReq.EXECUTE_LOGINNAME : null;
                        hisSereServTeinSDO.SERVICE_REQ_STT_ID = this.currentServiceReq != null ? this.currentServiceReq.SERVICE_REQ_STT_ID : 0;
                        var sereServExt = listSereServExt != null ? listSereServExt.Where(o => o.SERE_SERV_ID == item.ID).FirstOrDefault() : null;
                        hisSereServTeinSDO.SUBCLINICAL_RESULT_LOGINNAME = sereServExt != null ? sereServExt.SUBCLINICAL_RESULT_LOGINNAME : null;
                        this.lstHisSereServTeinSDO.Add(hisSereServTeinSDO);

                        if (lstSereServTein != null && lstSereServTein.Count > 0)
                        {
                            var sst = lstSereServTein.Where(o => o.SERE_SERV_ID == item.ID).OrderByDescending(o => o.NUM_ORDER ?? 0).ToList();
                            foreach (var ssTein in sst)
                            {
                                ssTein.IS_PARENT = 0;
                                ssTein.TEST_INDEX_CODE = "        " + ssTein.TEST_INDEX_CODE;
                                this.lstHisSereServTeinSDO.Add(ssTein);
                                if (!String.IsNullOrWhiteSpace(ssTein.VALUE))
                                {
                                    hisSereServTeinSDO.IsHasResultFrom_HIS_SERE_SERV_TEIN = true;
                                }
                            }
                        }

                    }
                }

                gridControlSereServTein.DataSource = null;
                gridControlSereServTein.DataSource = lstHisSereServTeinSDO;
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Debug("LoadDataToGridV2.5");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        long GetServiceNumOrder(HIS_SERE_SERV sereServNumOder)
        {
            try
            {
                var service = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.ID == sereServNumOder.SERVICE_ID).FirstOrDefault();
                return (service != null ? (service.NUM_ORDER ?? 0) : 99999);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return 0;
        }

        private void gridViewSereServTein_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                //ADO.HisSereServTeinSDO data = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetRow(e.RowHandle);
                //if (data != null)
                //{
                //    HIS_SERE_SERV sereServ = lstSereServ.FirstOrDefault(o => o.ID == data.SERE_SERV_ID);
                //    if (sereServ != null && sereServ.IS_NO_EXECUTE != null)
                //    {
                //        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                //    }
                //}
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
                if (((PacsCFG.MosLisInterGrationVersion == "1" && PacsCFG.MosLisInterGrationOption == "1") || (PacsCFG.MosLisInterGrationVersion == "2" && PacsCFG.MosLisInterGrationType == "1")) && currentServiceReq.IS_SENT_EXT == 1)
                    PrintProcess(PrintTypeTest.PHIEU_KET_XN);
                else 
                    PrintProcess(PrintTypeTest.IN_PHIEU_KET_QUA_XET_NGHIEM);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal enum PrintTypeTest
        {
            IN_PHIEU_KET_QUA_XET_NGHIEM,
            IN_PHIEU_KET_QUA,
            PHIEU_KET_XN
        }

        void PrintProcess(PrintTypeTest printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeTest.IN_PHIEU_KET_QUA_XET_NGHIEM:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_XET_NGHIEM__MPS000014, DelegateRunPrinterTest);
                        break;
                    case PrintTypeTest.IN_PHIEU_KET_QUA:
                        richEditorMain.RunPrintTemplate(PrintTypeCode.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA__MPS000015, DelegateRunPrinterTest);
                        break;
                    case PrintTypeTest.PHIEU_KET_XN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_KET_QUA_XET_NGHIEM__MPS000096, DelegateRunPrinterTest);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinterTest(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_XET_NGHIEM__MPS000014:
                        LoadBieuMauPhieuYCInKetQuaXetNghiem(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCode.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA__MPS000015:
                        LoadBieuMauPhieuYCInKetQua(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_KET_QUA_XET_NGHIEM__MPS000096:
                        LoadBieuMauPhieuXetNghiem(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadBieuMauPhieuXetNghiem(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CreateThreadLoadData();
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                if(!chkInTach.Checked)
                {
                    #region IN
                    MPS.Processor.Mps000096.PDO.Mps000096PDO pdo = new MPS.Processor.Mps000096.PDO.Mps000096PDO(
                       _PatientTypeAlter,
                       _Treatment,
                       _Sample.FirstOrDefault(),
                       _ServiceReq,
                       _Index,
                       _Result,
                       BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                       _Treatment.TDL_PATIENT_GENDER_ID,
                       BackendDataWorker.Get<V_HIS_SERVICE>(),
                       _Patient
                       );

                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_Treatment != null ? _Treatment.TREATMENT_CODE : "", printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);
                    Inventec.Common.Logging.LogSystem.Info(_Treatment.TREATMENT_CODE);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                    #endregion
                }
                else
                {
                    InTachPhieuXetNghiem(printTypeCode, fileName, ref result);
                }    
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            WaitingManager.Hide();
        }
        HIS_PATIENT _Patient { get; set; }
        HIS_SERVICE_REQ _ServiceReq { get; set; }
        List<V_HIS_TEST_INDEX> _Index { get; set; }
        List<V_LIS_RESULT> _Result { get; set; }
        List<V_LIS_SAMPLE> _Sample { get; set; }
        HIS_TREATMENT _Treatment { get; set; }
        HIS_PATIENT_TYPE_ALTER _PatientTypeAlter { get; set; }
        List<V_HIS_SERVICE> _Service { get; set; }
        private void CreateThreadLoadData()
        {
            Thread tTreatment = new Thread(new ThreadStart(LoadTreatment));
            Thread tPatientTypeAlter = new Thread(new ThreadStart(LoadPatientTypeAlter));
            Thread tPatient = new Thread(new ThreadStart(LoadPatient));
            Thread tServiceReq = new Thread(new ThreadStart(LoadServiceReq));
            Thread tSample = new Thread(new ThreadStart(LoadSample));
            try
            {
                tTreatment.Start();
                tPatientTypeAlter.Start();
                tPatient.Start();
                tServiceReq.Start();
                tSample.Start();
                tTreatment.Join();
                tPatientTypeAlter.Join();
                tPatient.Join();
                tServiceReq.Join();
                tSample.Join();
            }
            catch (Exception ex)
            {
                tTreatment.Abort();
                tPatientTypeAlter.Abort();
                tPatient.Abort();
                tServiceReq.Abort();
                tSample.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadSample()
        {
            try
            {
                CommonParam param = new CommonParam();
                LisSampleViewFilter sampleFilter = new LisSampleViewFilter();
                sampleFilter.SERVICE_REQ_CODE__EXACT = lstSereServ.FirstOrDefault().TDL_SERVICE_REQ_CODE;
                _Sample = new BackendAdapter(param).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumers.LisConsumer, sampleFilter, param);
                if (_Sample != null && _Sample.Count > 0)
                {
                    LisResultViewFilter resultFilter = new LisResultViewFilter();
                    resultFilter.SAMPLE_ID = _Sample.First().ID;
                    _Result = new BackendAdapter(param).Get<List<V_LIS_RESULT>>("api/LisResult/GetView", ApiConsumers.LisConsumer, resultFilter, param);
                    if (_Result != null && _Result.Count > 0)
                    {
                        HisTestIndexViewFilter indexFilter = new HisTestIndexViewFilter();
                        indexFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        indexFilter.SERVICE_CODEs = _Result.Select(o => o.SERVICE_CODE).ToList();
                        _Index = new BackendAdapter(param).Get<List<V_HIS_TEST_INDEX>>("api/HisTestIndex/GetView", ApiConsumers.MosConsumer, indexFilter, param);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadServiceReq()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.ID = lstSereServ.FirstOrDefault().SERVICE_REQ_ID;
                _ServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPatient()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientFilter patientFilter = new HisPatientFilter();
                patientFilter.ID = lstSereServ.FirstOrDefault().TDL_PATIENT_ID;
                _Patient = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPatientTypeAlter()
        {
            try
            {
                long treatmentId = lstSereServ.FirstOrDefault().TDL_TREATMENT_ID ?? 0;
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                MOS.Filter.HisPatientTypeAlterFilter patientTypeAlterFilter = new HisPatientTypeAlterFilter();
                patientTypeAlterFilter.TREATMENT_ID = treatmentId;
                patientTypeAlterFilter.ORDER_FIELD = "LOG_TIME";
                patientTypeAlterFilter.ORDER_DIRECTION = "DESC";
                _PatientTypeAlter = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, patientTypeAlterFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatment()
        {
            try
            {
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = lstSereServ.FirstOrDefault().TDL_TREATMENT_ID ?? 0;
                _Treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InTachPhieuXetNghiem(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                var ids = GetIds("HIS.DESKTOP.PRINT_TEST_RESULT.PARENT_SERVICE_CODE.GROUP");
                _Service = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => ids.Contains(o.ID)).ToList();
                Dictionary<long, List<V_HIS_TEST_INDEX>> dicGroup = GetDicResult();
                foreach (var item in dicGroup)
                {
                    List<V_LIS_RESULT> testLisResults = new List<V_LIS_RESULT>();
                    if (item.Value != null && item.Value.Count > 0)
                    {
                        testLisResults = _Result.Where(o => item.Value.Select(p => p.SERVICE_CODE).Contains(o.SERVICE_CODE)).ToList();
                    }
                    MPS.Processor.Mps000096.PDO.Mps000096PDO pdo = new MPS.Processor.Mps000096.PDO.Mps000096PDO(
                       _PatientTypeAlter,
                       _Treatment,
                       _Sample.FirstOrDefault(),
                       _ServiceReq,
                       item.Value,
                       testLisResults,
                       BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                       _Treatment.TDL_PATIENT_GENDER_ID,
                       BackendDataWorker.Get<V_HIS_SERVICE>(),
                       _Patient
                       );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_Treatment != null ? _Treatment.TREATMENT_CODE : "", printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);
                    Inventec.Common.Logging.LogSystem.Info(_Treatment.TREATMENT_CODE);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private Dictionary<long, List<V_HIS_TEST_INDEX>> GetDicResult()
        {
            Dictionary<long, List<V_HIS_TEST_INDEX>> dicServiceTest = new Dictionary<long, List<V_HIS_TEST_INDEX>>();
            try
            {
                List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>();
                if (services == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach dich vu");
                    return null;
                }
                foreach (var item in _Index)
                {
                    long key = 0;
                    V_HIS_SERVICE service = services.FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                    if (service != null)
                    {
                        if (service.PARENT_ID.HasValue && _Service != null && _Service.Count > 0 && _Service.Exists(o=>o.ID == service.PARENT_ID.Value))
                        {
                            key = -1;
                        }
                        else
                        {
                            key = service.PARENT_ID ?? 0;
                        }

                    }

                    if (!dicServiceTest.ContainsKey(key))
                        dicServiceTest[key] = new List<V_HIS_TEST_INDEX>();
                    dicServiceTest[key].Add(item);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return dicServiceTest;
        }
        private static List<long> GetIds(string keyConfig)
        {
            List<long> rs = new List<long>();
            try
            {
                string valueCFG = GetValue(keyConfig);
                if (!String.IsNullOrWhiteSpace(valueCFG))
                {
                    List<string> serviceCodes = valueCFG.Split(',').ToList();
                    foreach (var code in serviceCodes)
                    {
                        if (String.IsNullOrWhiteSpace(code)) continue;
                        V_HIS_SERVICE s = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.SERVICE_CODE == code);
                        if (s != null)
                        {
                            rs.Add(s.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                rs = new List<long>();
            }
            return rs;
        }
        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
        private void LoadBieuMauPhieuYCInKetQuaXetNghiem(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                long treatmentId = lstSereServ.FirstOrDefault().TDL_TREATMENT_ID ?? 0;

                //Loai Patient_type_name
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.ID = lstSereServ.FirstOrDefault().SERVICE_REQ_ID;
                var _ServiceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();

                //Load Data Treatment
                HIS_TREATMENT _Treatment = new HIS_TREATMENT();
                MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = treatmentId;
                _Treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                MOS.Filter.HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                patientTypeAlterFilter.TREATMENT_ID = treatmentId;
                patientTypeAlterFilter.ORDER_FIELD = "LOG_TIME";
                patientTypeAlterFilter.ORDER_DIRECTION = "DESC";
                patientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("/api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, patientTypeAlterFilter, param).FirstOrDefault();

                //Mức hưởng BHYT
                decimal ratio_text = 0;
                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                string levelCode = branch != null ? branch.HEIN_LEVEL_CODE : null;
                if (patientTypeAlter != null)
                {
                    ratio_text = GetDefaultHeinRatioForView(patientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, levelCode, patientTypeAlter.RIGHT_ROUTE_CODE);
                }


                // Dấu hiệu sinh tồn
                List<HIS_DHST> hisdhst = new List<HIS_DHST>();
                HIS_DHST hisdhst_ = new HIS_DHST();
                MOS.Filter.HisDhstFilter DhstFilter = new HisDhstFilter();
                DhstFilter.TREATMENT_ID = treatmentId;
                hisdhst = new BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumer.ApiConsumers.MosConsumer, DhstFilter, null);

                if (hisdhst != null && hisdhst.Count > 0)
                {

                    hisdhst_ = hisdhst.OrderByDescending(item => item.EXECUTE_TIME).First();
                }
                List<object> obj = new List<object>();
                obj.Add(patientTypeAlter);
                obj.Add(_ServiceReq);
                obj.Add(_Treatment);

                AutoMapper.Mapper.CreateMap<ADO.HisSereServTeinSDO, V_HIS_SERE_SERV_TEIN>();
                var sereServTeins = AutoMapper.Mapper.Map<List<V_HIS_SERE_SERV_TEIN>>(lstSereServTein);

                List<MPS.Processor.Mps000014.PDO.SereServNumOder> _SereServNumOders2 = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                List<MPS.Processor.Mps000014.PDO.SereServNumOder> _SereServNumOders4 = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                Dictionary<long, List<MPS.Processor.Mps000014.PDO.SereServNumOder>> _SereServNumOderss = new Dictionary<long, List<MPS.Processor.Mps000014.PDO.SereServNumOder>>();

                Inventec.Common.Logging.LogSystem.Debug("dữ liệu this._SereServNumOders2" + Inventec.Common.Logging.LogUtil.TraceData("", this._SereServNumOders));

                if (chkInTach.Checked)
                {
                    foreach (var item in this._SereServNumOders)
                    {
                        if (item.ServiceParentId == null)
                        {
                            if (!_SereServNumOderss.ContainsKey(0))
                            {
                                _SereServNumOderss[0] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                            }
                            _SereServNumOderss[0].Add(item);
                        }
                        else
                        {
                            _SereServNumOders2.Add(item);
                        }
                    }
                    foreach (var item in _SereServNumOders2)
                    {
                        var parent = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.ServiceParentId);
                        if (parent.PARENT_ID == null)
                        {
                            if (!_SereServNumOderss.ContainsKey(parent.ID))
                            {
                                _SereServNumOderss[parent.ID] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                            }
                            _SereServNumOderss[parent.ID].Add(item);
                        }
                        else
                        {
                            _SereServNumOders4.Add(item);
                            //    if (!_SereServNumOderss.ContainsKey(parent.PARENT_ID.Value))
                            //    {
                            //        _SereServNumOderss[parent.PARENT_ID.Value] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                            //    }
                            //    _SereServNumOderss[parent.PARENT_ID.Value].Add(item);
                            //}
                        }
                        _SereServNumOders4.GroupBy(o => o.GrandParentID);

                    }
                    foreach (var item in _SereServNumOders4)
                    {
                        if (!_SereServNumOderss.ContainsKey(item.GrandParentID.Value))
                        {
                            _SereServNumOderss[item.GrandParentID.Value] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                        }
                        _SereServNumOderss[item.GrandParentID.Value].Add(item);
                    }
                }
                else
                {
                    if (!_SereServNumOderss.ContainsKey(0))
                    {
                        _SereServNumOderss[0] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                    }
                    _SereServNumOderss[0].AddRange(this._SereServNumOders);
                }

                foreach (var item in _SereServNumOderss.Keys)
                {
                    Inventec.Common.Logging.LogSystem.Debug("dữ liệu _SereServNumOderss[item] " + Inventec.Common.Logging.LogUtil.TraceData("", _SereServNumOderss[item]));

                    MPS.Processor.Mps000014.PDO.Mps000014PDO mps000014RDO = new MPS.Processor.Mps000014.PDO.Mps000014PDO(
                        obj.ToArray(),
                        _SereServNumOderss[item],
                        sereServTeins,
                        ratio_text,
                        BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                        _Treatment.TDL_PATIENT_GENDER_ID,
                        BackendDataWorker.Get<V_HIS_SERVICE>(),
                        hisdhst_
                        );

                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(_Treatment != null ? _Treatment.TREATMENT_CODE : "", printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);
                    Inventec.Common.Logging.LogSystem.Info(_Treatment.TREATMENT_CODE);
                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public decimal GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            decimal result = 0;
            try
            {
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void barButtonItem__Print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (xtraTabControl1.SelectedTabPageIndex == 0)
                {
                    btnPrint_Click(null, null);
                }
                else
                {
                    btnPrintServiceReq_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void zoomFactor()
        {
            try
            {
                float zoom = 0;
                if (txtDescription.Document.Sections[0].Page.Landscape)
                    zoom = (float)(txtDescription.Width - 400) / (txtDescription.Document.Sections[0].Page.Height / 3);
                else
                    zoom = (float)(txtDescription.Width - 400) / (txtDescription.Document.Sections[0].Page.Width / 3);
                txtDescription.ActiveView.ZoomFactor = zoom;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void SereServClickRow(V_HIS_SERE_SERV_4 sereServ)
        {
            try
            {
                if (sereServ != null)
                {
                    WaitingManager.Show();
                    sereServExt = GetSereServExtBySereServId(sereServ.ID);
                    WaitingManager.Hide();
                    if (sereServExt != null && sereServExt.ID > 0)
                    {
                        bool isSense = false;
                        if (!String.IsNullOrWhiteSpace(sereServExt.JSON_PRINT_ID))
                        {
                            try
                            {
                                if (!String.IsNullOrWhiteSpace(sereServExt.JSON_PRINT_ID) && sereServExt.JSON_PRINT_ID.Contains("studyID"))
                                {
                                    var studyID = sereServExt.JSON_PRINT_ID.Split(':');
                                    if (studyID != null && studyID.Count() == 2)
                                    {
                                        if (PacsCFG.PACS_ADDRESS != null && PacsCFG.PACS_ADDRESS.Count > 0)
                                        {
                                            var address = PacsCFG.PACS_ADDRESS.FirstOrDefault(o => o.RoomCode == sereServ.EXECUTE_ROOM_CODE);
                                            if (address != null)
                                            {
                                                HIS_PATIENT patient = GetPatientById(sereServ.TDL_PATIENT_ID);
                                                //string url = string.Format("http://{0}:{1}/rpacsSENSE/pacsSENSE?patientID={2}&studyUID={3}", address.Address, "8080", patient.PATIENT_CODE, studyID[1]);
                                                string url = string.Format("http://{0}?patientID={1}&studyUID={2}", address.Address, patient.PATIENT_CODE, studyID[1]);
                                                Inventec.Common.Logging.LogSystem.Info("url: " + url);
                                                System.Diagnostics.Process.Start(url);

                                                isSense = true;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                isSense = false;
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }

                        if (isSense)
                        {
                            this.Close();
                        }
                        else
                        {
                            txtConclude.Text = sereServExt.CONCLUDE;
                            txtNote.Text = sereServExt.NOTE;
                            //this.ActionType = GlobalVariables.ActionEdit;
                            ProcessLoadSereServExtDescriptionPrint(sereServExt);
                        }
                    }
                    else
                    {
                        txtConclude.Text = "";
                        txtNote.Text = "";
                        txtDescription.Text = "";
                        //this.ActionType = GlobalVariables.ActionAdd;
                    }
                    //ProcessLoadSereServExt(sereServ, ref sereServExt);
                    //ProcessLoadSereServFile(sereServ);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_PATIENT GetPatientById(long? patientId)
        {
            HIS_PATIENT result = new HIS_PATIENT();
            try
            {
                if (patientId.HasValue)
                {
                    CommonParam paramCommon = new CommonParam();
                    MOS.Filter.HisPatientFilter filter = new MOS.Filter.HisPatientFilter();
                    filter.ID = patientId;
                    var rs = new Inventec.Common.Adapter.BackendAdapter
                           (paramCommon).Get<List<HIS_PATIENT>>
                           (ApiConsumer.HisRequestUriStore.HIS_PATIENT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                    if (rs != null && rs.Count > 0)
                    {
                        result = rs.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                result = new HIS_PATIENT();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT GetSereServExtBySereServId(long sereServId)
        {
            MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServExtFilter filter = new MOS.Filter.HisSereServExtFilter();
                filter.SERE_SERV_ID = sereServId;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private List<HIS_SERE_SERV_EXT> GetListSereServExtBySereServIds(List<long> sereServIds)
        {
            List<HIS_SERE_SERV_EXT> result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServExtFilter filter = new MOS.Filter.HisSereServExtFilter();
                filter.SERE_SERV_IDs = sereServIds;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessLoadSereServFile(MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_4 sereServ)
        {
            try
            {
                currentSereServFiles = GetSereServFilesBySereServId(sereServ.ID);
                if (currentSereServFiles != null && currentSereServFiles.Count > 0)
                {
                    foreach (MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE item in currentSereServFiles)
                    {
                        System.IO.MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(item.URL);
                        imageLoad = new List<ADO.ImageADO>();
                        if (stream != null && stream.Length > 0)
                        {
                            ADO.ImageADO tileNew = new ADO.ImageADO();
                            tileNew.FileName = item.SERE_SERV_FILE_NAME + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            tileNew.IsChecked = true;
                            tileNew.IMAGE_DISPLAY = Image.FromStream(stream);
                            imageLoad.Add(tileNew);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBySereServId()
        {
            try
            {
                if (this.currentSereServ == null) throw new ArgumentNullException("currentSereServ is null");

                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisSereServView4Filter filter = new MOS.Filter.HisSereServView4Filter();
                filter.ORDER_FIELD = "SERVICE_NUM_ORDER";
                filter.ORDER_DIRECTION = "DESC";
                filter.ID = this.currentSereServ.ID;
                var rs = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).Get<List<V_HIS_SERE_SERV_4>>
                    (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GETVIEW_4, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (rs != null && rs.Count > 0)
                {
                    SereServClickRow(rs[0]);
                }

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE> GetSereServFilesBySereServId(long sereServId)
        {
            List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE> result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServFileFilter filter = new MOS.Filter.HisSereServFileFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.SERE_SERV_ID = sereServId;
                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_FILE>>("/api/HisSereServFile/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void ProcessLoadSereServExtDescriptionPrint(MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT sereServExt)
        {
            try
            {
                if (sereServExt != null && sereServExt.ID > 0)
                {
                    currentSarPrint = GetListPrintByDescriptionPrint(sereServExt);
                    if (currentSarPrint != null && currentSarPrint.ID > 0)
                    {
                        txtDescription.RtfText = Utility.TextLibHelper.BytesToStringConverted(currentSarPrint.CONTENT);

                        btnPrint.Enabled = true;
                    }
                    else
                    {
                        txtDescription.Text = "";
                    }

                    zoomFactor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private SAR.EFMODEL.DataModels.SAR_PRINT GetListPrintByDescriptionPrint(MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT sereServExt)
        {
            SAR.EFMODEL.DataModels.SAR_PRINT result = null;
            try
            {
                List<long> printIds = GetListPrintIdBySereServ(sereServExt);
                if (printIds != null && printIds.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    SAR.Filter.SarPrintFilter filter = new SAR.Filter.SarPrintFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    filter.IDs = printIds;
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<SAR.EFMODEL.DataModels.SAR_PRINT>>(ApiConsumer.SarRequestUriStore.SAR_PRINT_GET, ApiConsumer.ApiConsumers.SarConsumer, filter, param).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private List<long> GetListPrintIdBySereServ(MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT item)
        {
            List<long> result = new List<long>();
            try
            {
                if (!String.IsNullOrEmpty(item.DESCRIPTION_SAR_PRINT_ID))
                {
                    var arrIds = item.DESCRIPTION_SAR_PRINT_ID.Split(',', ';');
                    if (arrIds != null && arrIds.Length > 0)
                    {
                        foreach (var id in arrIds)
                        {
                            long printId = Inventec.Common.TypeConvert.Parse.ToInt64(id);
                            if (printId > 0)
                            {
                                result.Add(printId);
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

        private void LoadBieuMauPhieuYCInKetQua(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                MOS.Filter.HisSereServFilter sereServFilter = new MOS.Filter.HisSereServFilter();
                sereServFilter.ID = this.currentSereServ.ID;
                List<HIS_SERE_SERV> _SereServs = new List<HIS_SERE_SERV>();
                _SereServs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV>>
                   (ApiConsumer.HisRequestUriStore.HIS_SERE_SERV_GET, ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, param);
                long _treatmentId = 0;
                long _serviceReqId = 0;
                List<MPS.Processor.Mps000015.PDO.Mps000015ADO> _Mps000015ADOs = new List<MPS.Processor.Mps000015.PDO.Mps000015ADO>();
                if (_SereServs != null && _SereServs.Count > 0)
                {
                    _treatmentId = _SereServs[0].TDL_TREATMENT_ID ?? 0;
                    _serviceReqId = _SereServs[0].SERVICE_REQ_ID ?? 0;

                    _Mps000015ADOs.AddRange((from r in _SereServs select new MPS.Processor.Mps000015.PDO.Mps000015ADO(r, txtNote.Text, txtConclude.Text)).ToList());

                }

                //Lấy thông tin thẻ BHYT
                MOS.Filter.HisPatientTypeAlterViewAppliedFilter hisPTAlterFilter = new HisPatientTypeAlterViewAppliedFilter();
                hisPTAlterFilter.TreatmentId = _treatmentId;
                hisPTAlterFilter.InstructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(DateTime.Now).ToString("yyyyMMddHHmm") + "00");
                var PatyAlterBhyt = new BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, hisPTAlterFilter, param);

                //Loai Patient_type_name
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.ID = _serviceReqId;
                var ServiceReqPrint = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();

                MPS.Processor.Mps000015.PDO.SingleKeys _SingleKeys = new MPS.Processor.Mps000015.PDO.SingleKeys();

                //Mức hưởng BHYT
                decimal ratio_text = 0;
                if (PatyAlterBhyt != null)
                {
                    string levelCode = PatyAlterBhyt.LEVEL_CODE;
                    ratio_text = GetDefaultHeinRatioForView(PatyAlterBhyt.HEIN_CARD_NUMBER, PatyAlterBhyt.HEIN_TREATMENT_TYPE_CODE, levelCode, PatyAlterBhyt.RIGHT_ROUTE_CODE);
                }

                _SingleKeys.Ratio = ratio_text;

                MOS.Filter.HisTreatmentBedRoomFilter bedRoomFilter = new HisTreatmentBedRoomFilter();
                bedRoomFilter.TREATMENT_ID = _treatmentId;
                var _TreatmentBedRoom = new BackendAdapter(param).Get<List<HIS_TREATMENT_BED_ROOM>>("/api/HisTreatmentBedRoom/Get", ApiConsumers.MosConsumer, bedRoomFilter, param).FirstOrDefault();


                if (_TreatmentBedRoom != null && _TreatmentBedRoom.BED_ID > 0)
                {
                    var bedName = BackendDataWorker.Get<HIS_BED>().FirstOrDefault(p => p.ID == _TreatmentBedRoom.BED_ID);
                    _SingleKeys.BED_NAME = bedName != null ? bedName.BED_NAME : null;
                }
                if (ServiceReqPrint != null)
                {
                    var depart = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p => p.ID == ServiceReqPrint.REQUEST_DEPARTMENT_ID);
                    _SingleKeys.REQUEST_DEPARTMENT_NAME = depart != null ? depart.DEPARTMENT_NAME : null;

                    var roomName = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ID == ServiceReqPrint.REQUEST_ROOM_ID);
                    _SingleKeys.REQUEST_ROOM_NAME = roomName != null ? roomName.ROOM_NAME : null;
                }
                if (_SereServs != null && _SereServs[0].TDL_SERVICE_TYPE_ID > 0)
                {
                    var typeName = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(p => p.ID == _SereServs[0].TDL_SERVICE_TYPE_ID);
                    _SingleKeys.SERVICE_TYPE_NAME = typeName != null ? typeName.SERVICE_TYPE_NAME : null;
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((ServiceReqPrint != null ? ServiceReqPrint.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);
                MPS.Processor.Mps000015.PDO.Mps000015PDO mps000015RDO = new MPS.Processor.Mps000015.PDO.Mps000015PDO(
                    PatyAlterBhyt,
                    ServiceReqPrint,
                    _Mps000015ADOs,
                    _SingleKeys
                    );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000015RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000015RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void btnPrintServiceReq_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtDescription.Text == "")
                {
                    PrintProcess(PrintTypeTest.IN_PHIEU_KET_QUA);
                }
                else
                {
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.ID = this.currentSereServ.SERVICE_REQ_ID;
                    var lstServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    long? finishTime = null;
                    if (lstServiceReq != null && lstServiceReq.Count > 0)
                    {
                        finishTime = lstServiceReq.FirstOrDefault().FINISH_TIME;
                    }
                    var printDocument = new DevExpress.XtraRichEdit.RichEditControl();
                    printDocument.RtfText = txtDescription.RtfText;
                    var tgkt = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.ThoiGianKetThuc");
                    string HideTimePrint = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.HideTimePrint");
                    if (!String.IsNullOrWhiteSpace(tgkt))
                    {
                        foreach (var section in printDocument.Document.Sections)
                        {
                            if (HideTimePrint != "1")
                            {
                                section.Margins.HeaderOffset = 50;
                                section.Margins.FooterOffset = 50;
                                var myHeader = section.BeginUpdateHeader(DevExpress.XtraRichEdit.API.Native.HeaderFooterType.Odd);
                                //xóa header nếu có dữ liệu
                                myHeader.Delete(myHeader.Range);

                                myHeader.InsertText(myHeader.CreatePosition(0),
                                    String.Format(Inventec.Common.Resource.Get.Value("NgayIn",
                                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()),
                                    DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                                myHeader.Fields.Update();
                                section.EndUpdateHeader(myHeader);
                            }

                            string finishTimeStr = "";

                            if (finishTime.HasValue)
                            {
                                finishTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(finishTime.Value);
                            }

                            var rangeSeperators = printDocument.Document.FindAll(
                                tgkt,
                                DevExpress.XtraRichEdit.API.Native.SearchOptions.None);

                            if (rangeSeperators != null && rangeSeperators.Length > 0)
                            {
                                for (int i = 0; i < rangeSeperators.Length; i++)
                                    printDocument.Document.Replace(rangeSeperators[i], finishTimeStr);
                            }
                        }

                    }

                    if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        printDocument.Print();
                    }
                    else
                    {
                        printDocument.ShowPrintPreview();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnEmr_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtDescription.Text != "" && this.currentSereServ.SERVICE_REQ_ID.HasValue)
                {
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.ID = this.currentSereServ.SERVICE_REQ_ID;
                    var lstServiceReq = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    long? finishTime = null;
                    if (lstServiceReq != null && lstServiceReq.Count > 0)
                    {
                        finishTime = lstServiceReq.FirstOrDefault().FINISH_TIME;
                    }
                    var printDocument = new DevExpress.XtraRichEdit.RichEditControl();
                    printDocument.RtfText = txtDescription.RtfText;
                    var tgkt = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.ThoiGianKetThuc");
                    string HideTimePrint = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ServiceExecute.HideTimePrint");
                    if (!String.IsNullOrWhiteSpace(tgkt))
                    {
                        foreach (var section in printDocument.Document.Sections)
                        {
                            if (HideTimePrint != "1")
                            {
                                section.Margins.HeaderOffset = 50;
                                section.Margins.FooterOffset = 50;
                                var myHeader = section.BeginUpdateHeader(DevExpress.XtraRichEdit.API.Native.HeaderFooterType.Odd);
                                //xóa header nếu có dữ liệu
                                myHeader.Delete(myHeader.Range);

                                myHeader.InsertText(myHeader.CreatePosition(0),
                                    String.Format(Inventec.Common.Resource.Get.Value("NgayIn",
                                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()),
                                    DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")));
                                myHeader.Fields.Update();
                                section.EndUpdateHeader(myHeader);
                            }

                            string finishTimeStr = "";

                            if (finishTime.HasValue)
                            {
                                finishTimeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(finishTime.Value);
                            }

                            var rangeSeperators = printDocument.Document.FindAll(
                                tgkt,
                                DevExpress.XtraRichEdit.API.Native.SearchOptions.None);

                            if (rangeSeperators != null && rangeSeperators.Length > 0)
                            {
                                for (int i = 0; i < rangeSeperators.Length; i++)
                                    printDocument.Document.Replace(rangeSeperators[i], finishTimeStr);
                            }
                        }
                    }

                    SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();

                    SignType type = new SignType();
                    if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Library.EmrGenerate.SignType") == "1")
                    {
                        type = SignType.USB;
                    }
                    else if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Library.EmrGenerate.SignType") == "2")
                    {
                        type = SignType.HMS;
                    }

                    InputADO inputADO = new InputADO(null, false, null, type);
                    inputADO.DTI = ConfigSystems.URI_API_ACS + "|" + ConfigSystems.URI_API_EMR + "|" + ConfigSystems.URI_API_FSS;
                    inputADO.IsSave = false;
                    inputADO.IsSign = true;//set true nếu cần gọi ký
                    inputADO.IsReject = true;
                    inputADO.IsPrint = false;
                    inputADO.IsExport = false;
                    inputADO.DlgOpenModuleConfig = OpenSignConfig;

                    String temFile = System.IO.Path.GetTempFileName();
                    temFile = temFile.Replace(".tmp", ".pdf");
                    printDocument.ExportToPdf(temFile);

                    libraryProcessor.ShowPopup(temFile, inputADO);//truyền vào đường dẫn file cần ký, các định dạng hỗ trợ là: pdf,doc,docx,xls,xlsx,rdlc,...

                    System.IO.File.Delete(temFile);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OpenSignConfig(EMR.TDO.DocumentTDO obj)
        {
            try
            {
                if (obj != null)
                {
                    EMR.Filter.EmrDocumentFilter filter = new EMR.Filter.EmrDocumentFilter();
                    filter.DOCUMENT_CODE__EXACT = obj.DocumentCode;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT>>(EMR.URI.EmrDocument.GET, ApiConsumer.ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, null);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        List<object> _listObj = new List<object>();
                        _listObj.Add(apiResult.Max(o => o.ID));//truyền vào id lớn nhất;

                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("EMR.Desktop.Plugins.EmrSign", currentModule.RoomId, currentModule.RoomTypeId, _listObj);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkInTach.Name)
                        {
                            chkInTach.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void chkInTach_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkInTach.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkInTach.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkInTach.Name;
                    csAddOrUpdate.VALUE = (chkInTach.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
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

        private void btnVanBanDaKy_Click(object sender, EventArgs e)
        {

            try
            {
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                long treatmentId = lstSereServ.FirstOrDefault().TDL_TREATMENT_ID ?? 0;
                //Load Data Treatment
                HIS_TREATMENT _Treatment = new HIS_TREATMENT();
                MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = treatmentId;
                _Treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                //Loai Patient_type_name
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.ID = lstSereServ.FirstOrDefault().SERVICE_REQ_ID;
                var _ServiceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();

                List<object> listArgs = new List<object>();
                HIS.Desktop.ADO.EmrDocumentInfoADO ErmDoc = new HIS.Desktop.ADO.EmrDocumentInfoADO();
                ErmDoc.TreatmentCode = _Treatment.TREATMENT_CODE;
                ErmDoc.HisCode = "SERVICE_REQ_CODE:" + _ServiceReq.SERVICE_REQ_CODE;
                listArgs.Add(ErmDoc);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("EMR.Desktop.Plugins.SignedDocument", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServTein_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ADO.HisSereServTeinSDO data = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetRow(e.RowHandle);
                V_HIS_SERVICE dataService = data != null ? (BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.ID == data.SERVICE_ID) != null ? BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.ID == data.SERVICE_ID).FirstOrDefault() : null) : null;
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (e.RowHandle >= 0)
                {
                    bool allowNoExcecute = false;
                    if (e.Column.FieldName == "IS_NO_EXECUTE")
                    {
                        if (data != null && data.IS_PARENT == 1)
                        {
                            allowNoExcecute = dataService != null && dataService.IS_DISALLOWANCE_NO_EXECUTE != 1
                                            && (loginName == data.REQUEST_LOGINNAME || loginName == data.EXECUTE_LOGINNAME || loginName == data.SUBCLINICAL_RESULT_LOGINNAME || CheckLoginAdmin.IsAdmin(loginName))
                                            && (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL || data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                                            && data.IsHasResultFrom_HIS_SERE_SERV_TEIN == false;
                            e.RepositoryItem = allowNoExcecute == true ? repositoryItemCheckEdit_Enable : repositoryItemCheckEdit_Disable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemTextEdit_DoNothing;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEdit_Enable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                ADO.HisSereServTeinSDO data = (ADO.HisSereServTeinSDO)gridViewSereServTein.GetRow(gridViewSereServTein.FocusedRowHandle);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("(HisSereServTeinSDO) data", data));
                HIS_SERE_SERV sereServ = new HIS_SERE_SERV();
                sereServ.ID = data.SERE_SERV_ID;
                if (data.IS_NO_EXECUTE)
                    sereServ.IS_NO_EXECUTE = null;
                else
                    sereServ.IS_NO_EXECUTE = 1;

                List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
                sereServs.Add(sereServ);
                HisSereServPayslipSDO hisSereServPayslipSDO = new HisSereServPayslipSDO();
                hisSereServPayslipSDO.Field = UpdateField.IS_NO_EXECUTE;
                hisSereServPayslipSDO.SereServs = sereServs;
                hisSereServPayslipSDO.TreatmentId = this.currentSereServ.TDL_TREATMENT_ID ?? 0;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("hisSereServPayslipSDO", hisSereServPayslipSDO));
                List<HIS_SERE_SERV> sereServResults = new BackendAdapter(param)
                    .Post<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("api/HisSereServ/UpdatePayslipInfo", ApiConsumers.MosConsumer, hisSereServPayslipSDO, param);
                WaitingManager.Hide();
                if (sereServResults != null && sereServResults.Count > 0)
                {
                    success = true;
                }
                MessageManager.Show(this, param, success);
                LoadDataToGridV2();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServTein_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "IS_NO_EXECUTE")
                {
                    e.DisplayText = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private int Age(long dob)
        {
            int num = -1;
            try
            {
                DateTime? dateTime = TimeNumberToSystemDateTime(dob);
                if (dateTime.HasValue)
                {
                    DateTime today = DateTime.Today;
                    num = today.Year - dateTime.Value.Year;
                    if (today < dateTime.Value.AddYears(num))
                    {
                        num--;
                    }
                }
            }
            catch (Exception)
            {
                num = -1;
            }
            return num;
        }
        private static DateTime? TimeNumberToSystemDateTime(long time)
        {
            DateTime? result = null;
            try
            {
                if (time > 0)
                {
                    return DateTime.ParseExact(time.ToString(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
    }
}
