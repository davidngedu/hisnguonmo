﻿using DevExpress.Data;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    public partial class frmDetail : HIS.Desktop.Utility.FormBase
    {
        HisServiceReqListResultSDO serviceReqComboResultSDO;
        HIS_SERVICE_REQ_TYPE serviceReqType__Test;
        HIS_SERVICE_REQ_TYPE serviceReqType__GPBL;
        HIS_SERVICE_REQ_TYPE serviceReqType__Exam;
        HIS_SERVICE_REQ_TYPE serviceReqType__Diim;
        HIS_SERVICE_REQ_TYPE serviceReqType__Fuex;
        HIS_SERVICE_REQ_TYPE serviceReqType__Endo;
        HIS_SERVICE_REQ_TYPE serviceReqType__Suim;
        HIS_SERVICE_REQ_TYPE serviceReqType__Misu;
        HIS_SERVICE_REQ_TYPE serviceReqType__Surg;
        HIS_SERVICE_REQ_TYPE serviceReqType__Other;
        HIS_SERVICE_REQ_TYPE serviceReqType__Bed;
        HIS_SERVICE_REQ_TYPE serviceReqType__Reha;
        const string commonString__true = "1";
        //bool isLoad = false;
        MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter;
        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ currentServiceReq;
        List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> currentSereServs;
        HisTreatmentWithPatientTypeInfoSDO currentHisTreatment;
        PopupMenu menu;
        Dictionary<string, object> dicParamPlus { get; set; }
        Dictionary<string, Inventec.Common.BarcodeLib.Barcode> dicImageBarcodePlus { get; set; }
        Dictionary<string, System.Drawing.Image> dicImagePlus { get; set; }
        Inventec.Desktop.Common.Modules.Module currentModule;
        /// <summary>
        /// </summary>
        public enum PrintTypeBMK
        {
            IN_PHIEU_YEU_CAU,
            PHIEU_XET_NGHIEM_DOM_SOI,
            TAO_BIEU_MAU_KHAC
        }

        public frmDetail(HisServiceReqListResultSDO serviceReqComboResultSDO,
            MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter,
            HisTreatmentWithPatientTypeInfoSDO _currentHisTreatment, Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            this.serviceReqComboResultSDO = serviceReqComboResultSDO;
            this.currentHisPatientTypeAlter = currentHisPatientTypeAlter;
            this.currentHisTreatment = _currentHisTreatment;
            this.currentModule = currentModule;
            this.IsUseApplyFormClosingOption = false;
        }

        private void frmDetail_Load(object sender, EventArgs e)
        {

            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

            //SetCaptionByLanguageKey();
            SetCaptionByLanguageKeyNew();
            serviceReqType__Test = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN);
            serviceReqType__GPBL = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL);
            serviceReqType__Exam = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH);
            serviceReqType__Diim = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA);
            serviceReqType__Fuex = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN);
            serviceReqType__Endo = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS);
            serviceReqType__Suim = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA);
            serviceReqType__Misu = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT);
            serviceReqType__Surg = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT);
            serviceReqType__Other = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC);
            serviceReqType__Bed = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G);
            serviceReqType__Reha = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().SingleOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN);

            //isLoad = false;
            if (this.serviceReqComboResultSDO != null)
            {
                gridControlServiceReqView.DataSource = this.serviceReqComboResultSDO.ServiceReqs;
                //LayDuLieuChungInCacPhieuChiDinh();
                gridViewServiceReqView__TabService.SelectAll();
            }

            List<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE> testSampleType = LoadTestSampleType();
            InitComboRepositoryTestSampleType(testSampleType);

        }



        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmDetail
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignService.Resources.Lang", typeof(frmDetail).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.grcStt__TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcStt__TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmDetail.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.ToolTip = Inventec.Common.Resource.Get.Value("frmDetail.gridColumn7.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemButtonTabServiceViewReq_Print.NullText = Inventec.Common.Resource.Get.Value("frmDetail.repositoryItemButtonTabServiceViewReq_Print.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcServiceReqCode_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcServiceReqCode_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcServiceReqTypeName_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcServiceReqTypeName_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcIntructionTime_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcIntructionTime_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcExecuteRoomName_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcExecuteRoomName_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmDetail.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcCreator_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcCreator_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcTotalAmount_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcTotalAmount_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcTotalPrice_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcTotalPrice_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemGridLookUpEdit_TestSampleType.NullText = Inventec.Common.Resource.Get.Value("frmDetail.repositoryItemGridLookUpEdit_TestSampleType.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmDetail.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmDetail.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmDetail.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnPrint.Caption = Inventec.Common.Resource.Get.Value("frmDetail.bbtnPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmDetail.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        internal List<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE> LoadTestSampleType()
        {
            List<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE> datas = null;
            try
            {

                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    HisTestSampleTypeFilter filter = new HisTestSampleTypeFilter();
                    datas = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE>>("api/HisTestSampleType/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                datas = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList() : null;

            }
            catch (Exception ex)
            {
                datas = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return datas;
        }

        private void InitComboRepositoryTestSampleType(List<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TEST_SAMPLE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TEST_SAMPLE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TEST_SAMPLE_TYPE_NAME", "ID", columnInfos, false, 350);
                if (data != null)
                {
                    ControlEditorLoader.Load(this.repositoryItemGridLookUpEdit_TestSampleType, (data != null ? data.OrderBy(o => o.TEST_SAMPLE_TYPE_NAME).ToList() : null), controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRepositoryTestSampleType(List<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE> data, DevExpress.XtraEditors.GridLookUpEdit patientTypeCombo)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TEST_SAMPLE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TEST_SAMPLE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TEST_SAMPLE_TYPE_NAME", "ID", columnInfos, false, 350);
                if (data != null)
                {
                    ControlEditorLoader.Load(patientTypeCombo, (data != null ? data.OrderBy(o => o.TEST_SAMPLE_TYPE_NAME).ToList() : null), controlEditorADO);
                }
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
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__frmDetail = new ResourceManager("HIS.Desktop.Plugins.AssignService.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignService.AssignService.frmDetail).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.grcStt__TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcStt__TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmDetail.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.gridColumn7.ToolTip = Inventec.Common.Resource.Get.Value("frmDetail.gridColumn7.ToolTip", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.repositoryItemButtonTabServiceViewReq_Print.NullText = Inventec.Common.Resource.Get.Value("frmDetail.repositoryItemButtonTabServiceViewReq_Print.NullText", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.grcServiceReqCode_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcServiceReqCode_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.grcServiceReqTypeName_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcServiceReqTypeName_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.grcIntructionTime_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcIntructionTime_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.grcExecuteRoomName_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcExecuteRoomName_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.grcCreator_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcCreator_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.grcTotalAmount_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcTotalAmount_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.grcTotalPrice_TabService.Caption = Inventec.Common.Resource.Get.Value("frmDetail.grcTotalPrice_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmDetail.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmDetail.Text", Resources.ResourceLanguageManager.LanguageResource__frmDetail, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReqView__TabService_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERVICE_REQ dataRow = (V_HIS_SERVICE_REQ)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    //else if (e.Column.FieldName == "PRIORIRY_DISPLAY")
                    //{
                    //    long priority = (dataRow.PRIORITY ?? 0);
                    //    if ((priority == 1))
                    //    {
                    //        e.Value = imageListPriority.Images[0];
                    //    }
                    //}
                    else if (e.Column.FieldName == "INTRUCTION_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.INTRUCTION_TIME);
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReqView__TabService_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ data = null;

                if (e.RowHandle > -1)
                {
                    data = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "PRINT_DOM_SOI")
                    {
                        //if (data.SERVICE_REQ_TYPE_ID == (serviceReqType__Test != null ? serviceReqType__Test.ID : 0))
                        //{
                        //    e.RepositoryItem = ButtonEditPrintDomSoi;
                        //}
                    }
                    if (e.Column.FieldName == "TEST_SAMPLE_TYPE_ID")
                    {
                        if (data.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)
                            e.RepositoryItem = repositoryItemGridLookUpEdit_TestSampleType;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintByServiceReqType(MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ currentServiceReq)
        {
            try
            {
                if (currentServiceReq != null)
                {
                    currentSereServs = this.serviceReqComboResultSDO.SereServs.Where(o => o.SERVICE_REQ_ID == currentServiceReq.ID).ToList();
                    if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Exam.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Test.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_XET_NGHIEM__MPS000026);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__GPBL.ID)
                    {
                        DelegateRunPrinter("Mps000167");
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Diim.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_CHUAN_DOAN_HINH_ANH__MPS000028);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Fuex.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THAM_DO_CHUC_NANG__MPS000038);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Endo.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_NOI_SOI__MPS000029);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Suim.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_SIEU_AM__MPS000030);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Misu.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THU_THUAT__MPS000031);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Surg.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT__MPS000036);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Other.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_DICH_VU_KHAC__MPS000040);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Bed.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_GIUONG__MPS000042);
                    }
                    else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Reha.ID)
                    {
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHUC_HOI_CHUC_NANG__MPS000053);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuYeuCau(string printTypeCode, bool printNow)
        {
            try
            {
                CommonParam param = new CommonParam();
                V_HIS_BED_LOG bedLog = new V_HIS_BED_LOG();
                if (currentServiceReq != null)
                {
                    var sdo = new HisServiceReqListResultSDO();
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("private void InPhieuYeuCau(string printTypeCode, bool printNow) currentServiceReq ", currentServiceReq));
                    if (currentServiceReq.TEST_SAMPLE_TYPE_ID.HasValue)
                    {
                        var testSampleType = BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>().FirstOrDefault(o => o.ID == currentServiceReq.TEST_SAMPLE_TYPE_ID.Value);
                        currentServiceReq.TEST_SAMPLE_TYPE_CODE = testSampleType != null ? testSampleType.TEST_SAMPLE_TYPE_CODE : "";
                        currentServiceReq.TEST_SAMPLE_TYPE_NAME = testSampleType != null ? testSampleType.TEST_SAMPLE_TYPE_NAME : "";
                    }
                    sdo.ServiceReqs = new List<V_HIS_SERVICE_REQ>() { currentServiceReq };
                    sdo.SereServs = serviceReqComboResultSDO.SereServs.Where(o => o.SERVICE_REQ_ID == currentServiceReq.ID).ToList();
                    // get bedLog
                    List<V_HIS_BED_LOG> bedLogs = new List<V_HIS_BED_LOG>();
                    if (currentServiceReq != null && currentHisTreatment != null)
                    {
                        MOS.Filter.HisBedLogViewFilter bedLogViewFilter = new MOS.Filter.HisBedLogViewFilter();
                        bedLogViewFilter.DEPARTMENT_IDs = new List<long>() { currentServiceReq.REQUEST_DEPARTMENT_ID };
                        bedLogViewFilter.TREATMENT_ID = currentHisTreatment.ID;
                        bedLogs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogViewFilter, param);
                    }

                    var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(sdo, currentHisTreatment, bedLogs, currentModule != null ? currentModule.RoomId : 0);
                    PrintServiceReqProcessor.Print(printTypeCode, printNow);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__YEU_CAU_KHAM_CHUYEN_KHOA__MPS000071:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_XET_NGHIEM__MPS000026:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case "Mps000167":
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_CHUAN_DOAN_HINH_ANH__MPS000028:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THAM_DO_CHUC_NANG__MPS000038:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_NOI_SOI__MPS000029:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_SIEU_AM__MPS000030:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THU_THUAT__MPS000031:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT__MPS000036:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_DICH_VU_KHAC__MPS000040:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_GIUONG__MPS000042:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHUC_HOI_CHUC_NANG__MPS000053:
                        InPhieuYeuCau(printTypeCode, false);
                        break;
                    case
PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_PHIEU_XET_NGHIEM_DOM_SOI_TRUC_TIEP__MPS000027:
                        InPhieuYeuCau(printTypeCode, false);
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

        private void LoadBtnPrintGridServiceReqView(DevExpress.XtraBars.BarManager barManager)
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager);

                menu.ItemLinks.Clear();

                BarButtonItem itemInPhieuYeuCau = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEFRMASSIGNSERVICE_MERGER_PRINT_IN_PHIEU_YEU_CAU", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), 1);
                itemInPhieuYeuCau.Tag = PrintTypeBMK.IN_PHIEU_YEU_CAU;
                itemInPhieuYeuCau.ItemClick += new ItemClickEventHandler(onClickItemPrint);

                BarButtonItem itemInPhieuXetNghiemDomSoi = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEFRMASSIGNSERVICE_MERGER_PRINT_IN_PHIEU_XET_NGHIEM_DOM_SOI", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), 2);
                itemInPhieuXetNghiemDomSoi.Tag = PrintTypeBMK.PHIEU_XET_NGHIEM_DOM_SOI;
                itemInPhieuXetNghiemDomSoi.ItemClick += new ItemClickEventHandler(onClickItemPrint);

                BarButtonItem itemTaoBieuMauKhac = new BarButtonItem(barManager, Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEFRMASSIGNSERVICE_MERGER_PRINT_IN_BIEU_MAU_KHAC", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), 3);
                itemTaoBieuMauKhac.Tag = PrintTypeBMK.TAO_BIEU_MAU_KHAC;
                itemTaoBieuMauKhac.ItemClick += new ItemClickEventHandler(onClickItemPrint);

                if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Test.ID)
                {
                    menu.AddItems(new BarItem[] { itemInPhieuYeuCau, itemInPhieuXetNghiemDomSoi, itemTaoBieuMauKhac });
                }
                else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Endo.ID)
                {
                    menu.AddItems(new BarItem[] { itemInPhieuYeuCau, itemTaoBieuMauKhac });
                }
                else
                {
                    PrintProcess(PrintTypeBMK.IN_PHIEU_YEU_CAU);
                }

                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintProcess(PrintTypeBMK printType)
        {
            try
            {
                switch (printType)
                {
                    case PrintTypeBMK.IN_PHIEU_YEU_CAU:
                        #region aa
                        if (currentServiceReq != null)
                        {
                            currentSereServs = serviceReqComboResultSDO.SereServs.Where(o => o.SERVICE_REQ_ID == currentServiceReq.ID).ToList();

                            if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Exam.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Test.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_XET_NGHIEM__MPS000026);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Diim.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_CHUAN_DOAN_HINH_ANH__MPS000028);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Fuex.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THAM_DO_CHUC_NANG__MPS000038);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Endo.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_NOI_SOI__MPS000029);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Suim.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_SIEU_AM__MPS000030);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Misu.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THU_THUAT__MPS000031);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Surg.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT__MPS000036);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Other.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_DICH_VU_KHAC__MPS000040);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Bed.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_GIUONG__MPS000042);
                            }
                            else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Reha.ID)
                            {
                                DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHUC_HOI_CHUC_NANG__MPS000053);
                            };
                        }
                        #endregion
                        break;
                    case PrintTypeBMK.PHIEU_XET_NGHIEM_DOM_SOI:
                        DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_PHIEU_XET_NGHIEM_DOM_SOI_TRUC_TIEP__MPS000027);
                        break;
                    case PrintTypeBMK.TAO_BIEU_MAU_KHAC:
                        if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Test.ID)
                        {
                            onClickInKetQuaKhacPlus(null, null);
                        }
                        else if (currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Endo.ID)
                        {
                            onClickTaoPhieuNoiSoiKhacPlus(null, null);
                        }
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

        private void onClickInKetQuaKhacPlus(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                dicParamPlus = new Dictionary<string, object>();
                dicImageBarcodePlus = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
                dicImagePlus = new Dictionary<string, Image>();

                string printTypeCode = "Mps000231";
                dicParamPlus = new Dictionary<string, object>();
                SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == printTypeCode);// new SarPrintTypeLogic().Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>(printTypeCode);
                if (printTemplate != null)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                    //EXE.LOGIC.HisSereServ.HisSereServLogic sereServLogic = new LOGIC.HisSereServ.HisSereServLogic();
                    MOS.Filter.HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                    sereServFilter.SERVICE_REQ_ID = this.currentServiceReq.ID;
                    var sereSevs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, sereServFilter, ProcessLostToken, new CommonParam());
                    //var sereSevs = sereServLogic.GetView<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>>(sereServFilter);
                    string serviceNames = "";
                    foreach (var sereServ in sereSevs)
                        serviceNames += sereServ.TDL_SERVICE_NAME + "....." + "\r\n";
                    AddKeyIntoDictionaryPrint<V_HIS_SERVICE_REQ>(this.currentServiceReq, dicParamPlus);
                    dicParamPlus.Add("SERVICE_NAME", serviceNames);

                    MOS.Filter.HisTreatmentView1Filter treatmentViewFilter = new HisTreatmentView1Filter();
                    treatmentViewFilter.ID = currentServiceReq.TREATMENT_ID;
                    var treatment = new BackendAdapter(null).Get<List<V_HIS_TREATMENT_1>>("api/HisTreatment/GetView1", ApiConsumer.ApiConsumers.MosConsumer, treatmentViewFilter, null).FirstOrDefault();

                    AddKeyIntoDictionaryPrint<V_HIS_TREATMENT_1>(treatment, dicParamPlus);
                    var currentDepartmentTran = PrintGlobalStore.getDepartmentTran(currentServiceReq.TREATMENT_ID);
                    if (currentDepartmentTran != null)
                    {
                        dicParamPlus.Add("DEPARTMENT_NAME", currentDepartmentTran.DEPARTMENT_NAME);
                    }
                    else
                    {
                        dicParamPlus.Add("DEPARTMENT_NAME", WorkPlace.GetDepartmentName());
                    }
                    dicParamPlus.Add("AGE", HIS.Desktop.Print.AgeHelper.CalculateAgeFromYear(treatment.TDL_PATIENT_DOB));
                    dicParamPlus.Add("CURRENT_DATE_SEPARATE_STR", HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentTime());
                    dicParamPlus.Add("BED_ROOM_NAME", WorkPlace.GetRoomName());
                    dicParamPlus.Add("BED_NAME", "");
                    dicParamPlus.Add("INSTRUCTION_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.currentServiceReq.INTRUCTION_TIME));
                    dicParamPlus.Add("INSTRUCTION_DATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentServiceReq.INTRUCTION_TIME));
                    dicParamPlus.Add("INSTRUCTION_DATE_SEPAREATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.currentServiceReq.INTRUCTION_TIME));
                    dicParamPlus.Add("FINISH_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.currentServiceReq.FINISH_TIME ?? 0));
                    dicParamPlus.Add("FINISH_DATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentServiceReq.FINISH_TIME ?? 0));
                    dicParamPlus.Add("FINISH_DATE_SEPAREATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.currentServiceReq.FINISH_TIME ?? 0));
                    dicParamPlus.Add("AGE_STRING", Inventec.Common.DateTime.Calculation.AgeString(treatment.TDL_PATIENT_DOB, "", "", "", "", treatment.IN_TIME));
                    WaitingManager.Hide();
                    richEditorMain.RunPrintTemplate(printTemplate.PRINT_TYPE_CODE, printTemplate.FILE_PATTERN, "Phiếu xét nghiệm", UpdateSereServJsonPrint, GetListPrintIdByServiceReq, dicParamPlus, dicImagePlus);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddKeyIntoDictionaryPrint<T>(T data, Dictionary<string, object> dicParamPlus)
        {
            try
            {
                PropertyInfo[] pis = typeof(T).GetProperties();
                if (pis != null && pis.Length > 0)
                {
                    foreach (var pi in pis)
                    {
                        var searchKey = dicParamPlus.SingleOrDefault(o => o.Key == pi.Name);
                        if (String.IsNullOrEmpty(searchKey.Key))
                        {
                            dicParamPlus.Add(pi.Name, pi.GetValue(data));
                        }
                        else
                        {
                            dicParamPlus[pi.Name] = pi.GetValue(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClickTaoPhieuNoiSoiKhacPlus(object sender, EventArgs e)
        {
            try
            {
                dicParamPlus = new Dictionary<string, object>();
                dicImageBarcodePlus = new Dictionary<string, Inventec.Common.BarcodeLib.Barcode>();
                dicImagePlus = new Dictionary<string, Image>();

                string printTypeCode = "Mps000232";

                SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printTemplate = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().FirstOrDefault(o => o.PRINT_TYPE_CODE == printTypeCode);// new SarPrintTypeLogic().Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>(printTypeCode);
                if (printTemplate != null)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                    string serviceNames = "";
                    foreach (var sereServ in serviceReqComboResultSDO.SereServs)
                        serviceNames += sereServ.TDL_SERVICE_NAME + "....." + "\r\n";
                    AddKeyIntoDictionaryPrint<V_HIS_SERVICE_REQ>(this.currentServiceReq, dicParamPlus);
                    dicParamPlus.Add("SERVICE_NAME", serviceNames);

                    MOS.Filter.HisTreatmentView1Filter treatmentViewFilter = new HisTreatmentView1Filter();
                    treatmentViewFilter.ID = currentServiceReq.TREATMENT_ID;
                    var treatment = new BackendAdapter(null).Get<List<V_HIS_TREATMENT_1>>("api/HisTreatment/GetView1", ApiConsumer.ApiConsumers.MosConsumer, treatmentViewFilter, null).FirstOrDefault();

                    AddKeyIntoDictionaryPrint<V_HIS_TREATMENT_1>(treatment, dicParamPlus);
                    var currentDepartmentTran = PrintGlobalStore.getDepartmentTran(currentServiceReq.TREATMENT_ID);
                    if (currentDepartmentTran != null)
                    {
                        dicParamPlus.Add("DEPARTMENT_NAME", currentDepartmentTran.DEPARTMENT_NAME);
                    }
                    else
                    {
                        dicParamPlus.Add("DEPARTMENT_NAME", WorkPlace.GetDepartmentName());
                    }
                    dicParamPlus.Add("AGE", HIS.Desktop.Print.AgeHelper.CalculateAgeFromYear(treatment.TDL_PATIENT_DOB));
                    dicParamPlus.Add("CURRENT_DATE_SEPARATE_STR", HIS.Desktop.Utilities.GlobalReportQuery.GetCurrentTime());
                    dicParamPlus.Add("AGE_STRING", Inventec.Common.DateTime.Calculation.AgeString(treatment.TDL_PATIENT_DOB, "", "", "", "", treatment.IN_TIME));
                    //dicParamPlus.Add("BED_ROOM_NAME", EXE.LOGIC.Token.TokenManager.GetRoomName());
                    //dicParamPlus.Add("BED_NAME", "");
                    //dicParamPlus.Add("INSTRUCTION_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.hisServiceReq.INTRUCTION_TIME));
                    //dicParamPlus.Add("INSTRUCTION_DATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.hisServiceReq.INTRUCTION_TIME));
                    //dicParamPlus.Add("INSTRUCTION_DATE_SEPAREATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.hisServiceReq.INTRUCTION_TIME));
                    //dicParamPlus.Add("FINISH_TIME_STR", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.hisServiceReq.FINISH_TIME ?? 0));
                    //dicParamPlus.Add("FINISH_DATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.hisServiceReq.FINISH_TIME ?? 0));
                    //dicParamPlus.Add("FINISH_DATE_SEPAREATE_STR", Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.hisServiceReq.FINISH_TIME ?? 0));
                    WaitingManager.Hide();

                    richEditorMain.RunPrintTemplate(printTemplate.PRINT_TYPE_CODE, printTemplate.FILE_PATTERN, "Phiếu nội soi", UpdateSereServJsonPrint, GetListPrintIdByServiceReq, dicParamPlus, dicImagePlus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<long> GetListPrintIdByServiceReq()
        {
            List<long> result = new List<long>();
            try
            {
                if (this.currentServiceReq != null)
                {
                    if (!String.IsNullOrEmpty(this.currentServiceReq.JSON_PRINT_ID))
                    {
                        var arrIds = this.currentServiceReq.JSON_PRINT_ID.Split(',', ';');
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool UpdateSereServJsonPrint(SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            bool success = false;
            try
            {
                //call api update JSON_PRINT_ID for current sereserv
                bool valid = true;
                valid = valid && (this.currentServiceReq != null);
                if (valid)
                {
                    List<FileHolder> listFileHolder = new List<FileHolder>();
                    HIS_SERVICE_REQ hisServiceReq = new HIS_SERVICE_REQ();
                    var listOldPrintIdOfSereServs = GetListPrintIdByServiceReq();
                    ProcessServiceReqExecuteForUpdateJsonPrint(hisServiceReq, listOldPrintIdOfSereServs, sarPrintCreated);
                    SaveTestServiceReq(listFileHolder, hisServiceReq);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        private void SaveTestServiceReq(List<FileHolder> listFileHolder, HIS_SERVICE_REQ hisServiceReq)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();

                HIS_SERVICE_REQ serviceReqUpdateSDO = new HIS_SERVICE_REQ();
                AutoMapper.Mapper.CreateMap<V_HIS_SERVICE_REQ, HIS_SERVICE_REQ>();
                serviceReqUpdateSDO = AutoMapper.Mapper.Map<V_HIS_SERVICE_REQ, HIS_SERVICE_REQ>(this.currentServiceReq);
                hisServiceReq.ID = this.currentServiceReq.ID;
                var hisSereServWithFileResultSDO = new BackendAdapter(param).Post<HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UPDATE_JSON, ApiConsumers.MosConsumer, hisServiceReq, ProcessLostToken, param);
                if (hisSereServWithFileResultSDO != null)
                {
                    success = true;
                }
                WaitingManager.Hide();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide(); ;
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void ProcessServiceReqExecuteForUpdateJsonPrint(HIS_SERVICE_REQ hisServiceReq, List<long> jsonPrintId, SAR.EFMODEL.DataModels.SAR_PRINT sarPrintCreated)
        {
            try
            {
                if (this.currentServiceReq != null)
                {
                    if (jsonPrintId == null)
                    {
                        jsonPrintId = new List<long>();
                    }
                    jsonPrintId.Add(sarPrintCreated.ID);

                    string printIds = "";
                    foreach (var item in jsonPrintId)
                    {
                        printIds += item.ToString() + ",";
                    }

                    hisServiceReq.JSON_PRINT_ID = printIds;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void onClickItemPrint(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var bbtnItem1 = e.Item as BarButtonItem;
                PrintTypeBMK type = (PrintTypeBMK)(bbtnItem1.Tag);
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ButtonEditPrintDomSoi_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (barManager2 == null)
                {
                    barManager2 = new DevExpress.XtraBars.BarManager();

                }
                barManager2.Form = this;
                LoadBtnPrintGridServiceReqView(barManager2);
                //if (lstHisServiceReqSDOResult == null || lstHisServiceReqSDOResult.Count <= 0)
                //{
                //    return;
                //}

                //hisServiceReq = (EXE.ADO.HisServiceReqCombo)gridViewServiceReqView__TabService.GetFocusedRow();

                ////In phieu xet nghiem dom soi
                //InPhieuXetNghiemDomSoi();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnPrint_Click(null, null);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewServiceReqView__TabService.PostEditor();

                List<V_HIS_SERVICE_REQ> ServiceReqSelects = new List<V_HIS_SERVICE_REQ>();

                foreach (var item in gridViewServiceReqView__TabService.GetSelectedRows())
                {
                    currentServiceReq = (V_HIS_SERVICE_REQ)gridViewServiceReqView__TabService.GetRow(item);
                    PrintByServiceReqType(currentServiceReq);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReqView__TabService_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        this.currentServiceReq = (V_HIS_SERVICE_REQ)gridViewServiceReqView__TabService.GetRow(hi.RowHandle);
                        if (hi.Column.FieldName == "PrintDetail")
                        {
                            if (this.currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Test.ID || currentServiceReq.SERVICE_REQ_TYPE_ID == serviceReqType__Endo.ID)
                            {
                                if (barManager2 == null)
                                {
                                    barManager2 = new DevExpress.XtraBars.BarManager();

                                }
                                barManager2.Form = this;
                                LoadBtnPrintGridServiceReqView(barManager2);
                            }
                            else
                            {
                                PrintByServiceReqType(this.currentServiceReq);
                            }
                        }
                        else if (hi.Column.FieldName == "TEST_SAMPLE_TYPE_ID")
                        {
                            //view.FocusedRowHandle = hi.RowHandle;
                            //view.FocusedColumn = hi.Column;
                            //view.ShowEditor();
                            //GridLookUpEdit lookUp = view.ActiveEditor as GridLookUpEdit;
                            //if (lookUp == null)
                            //    return;
                            //else
                            //{
                            //    lookUp.Focus();
                            //    lookUp.ShowPopup();
                            //}
                            //view.CloseEditor();
                            //(e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }

                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;

                            int rowHandle = gridViewServiceReqView__TabService.GetVisibleRowHandle(hi.RowHandle);
                            var dataRow = (V_HIS_SERVICE_REQ)gridViewServiceReqView__TabService.GetRow(rowHandle);
                            if (dataRow != null)
                            {
                                //if (hi.Column.FieldName == "IsChecked" && (dataRow.IsAllowChecked == false))
                                //{
                                //    (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                                //    return;
                                //}
                            }

                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            if (checkEdit == null)
                                return;

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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookUpEdit_TestSampleType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                V_HIS_SERVICE_REQ vServiceReq = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ)gridViewServiceReqView__TabService.GetFocusedRow();
                if (vServiceReq != null)
                {
                    HIS_SERVICE_REQ serviceReq = new MOS.EFMODEL.DataModels.HIS_SERVICE_REQ();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>(serviceReq, vServiceReq);

                    GridLookUpEdit edit = sender as GridLookUpEdit;
                    if (edit == null) return;
                    if (edit.EditValue != null)
                    {
                        var pt = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker
                            .Get<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE>(false, true)
                            .FirstOrDefault(o =>
                                (o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((edit.EditValue ?? "").ToString())
                                || o.TEST_SAMPLE_TYPE_NAME == (edit.EditValue ?? "").ToString()));
                        if (pt != null)
                        {
                            WaitingManager.Show();
                            CommonParam param = new CommonParam();
                            bool? success = null;
                            serviceReq.TEST_SAMPLE_TYPE_ID = pt.ID;
                            HIS_SERVICE_REQ result = new BackendAdapter(param).Post<HIS_SERVICE_REQ>("api/HisServiceReq/UpdateSampleType", ApiConsumers.MosConsumer, serviceReq, ProcessLostToken, param);
                            WaitingManager.Hide();
                            if (result != null)
                            {
                                success = true;
                                gridViewServiceReqView__TabService.FocusedColumn = gridViewServiceReqView__TabService.Columns[1];
                                //Load lai du lieu cu
                                gridControlServiceReqView.BeginUpdate();
                                foreach (var item in this.serviceReqComboResultSDO.ServiceReqs)
                                {
                                    if (item.ID == serviceReq.ID)
                                    {
                                        item.TEST_SAMPLE_TYPE_ID = result.TEST_SAMPLE_TYPE_ID;
                                        break;
                                    }
                                }
                                gridControlServiceReqView.RefreshDataSource();
                                gridControlServiceReqView.EndUpdate();
                            }
                            else
                            {
                                success = false;
                                long? oldTestSampleTypeId = null;
                                if (edit.OldEditValue != null)
                                    oldTestSampleTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(edit.OldEditValue.ToString());
                                gridControlServiceReqView.BeginUpdate();
                                gridViewServiceReqView__TabService.FocusedColumn = gridViewServiceReqView__TabService.Columns[1];
                                //Load lai du lieu cu
                                foreach (var item in this.serviceReqComboResultSDO.ServiceReqs)
                                {
                                    if (item.ID == serviceReq.ID)
                                    {
                                        item.TEST_SAMPLE_TYPE_ID = oldTestSampleTypeId;
                                        break;
                                    }
                                }
                                gridControlServiceReqView.RefreshDataSource();
                                gridControlServiceReqView.EndUpdate();
                            }
                            MessageManager.Show(this, param, success);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookUpEdit_TestSampleType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    V_HIS_SERVICE_REQ vServiceReq = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ)gridViewServiceReqView__TabService.GetFocusedRow();
                    if (vServiceReq != null)
                    {
                        HIS_SERVICE_REQ serviceReq = new MOS.EFMODEL.DataModels.HIS_SERVICE_REQ();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>(serviceReq, vServiceReq);

                        GridLookUpEdit edit = sender as GridLookUpEdit;
                        if (edit == null) return;
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        bool? success = null;
                        serviceReq.TEST_SAMPLE_TYPE_ID = null;
                        HIS_SERVICE_REQ result = new BackendAdapter(param).Post<HIS_SERVICE_REQ>("api/HisServiceReq/UpdateSampleType", ApiConsumers.MosConsumer, serviceReq, ProcessLostToken, param);
                        WaitingManager.Hide();
                        if (result != null)
                        {
                            success = true;
                            gridControlServiceReqView.BeginUpdate();

                            gridViewServiceReqView__TabService.FocusedColumn = gridViewServiceReqView__TabService.Columns[1];
                            //Load lai du lieu cu
                            foreach (var item in this.serviceReqComboResultSDO.ServiceReqs)
                            {
                                if (item.ID == serviceReq.ID)
                                {
                                    item.TEST_SAMPLE_TYPE_ID = null;
                                    break;
                                }
                            }
                            gridControlServiceReqView.RefreshDataSource();
                            gridControlServiceReqView.EndUpdate();
                        }
                        else
                        {
                            success = false;
                            long? oldTestSampleTypeId = null;
                            if (edit.OldEditValue != null)
                                oldTestSampleTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(edit.OldEditValue.ToString());

                            gridControlServiceReqView.BeginUpdate();
                            gridViewServiceReqView__TabService.FocusedColumn = gridViewServiceReqView__TabService.Columns[1];
                            //Load lai du lieu cu
                            foreach (var item in this.serviceReqComboResultSDO.ServiceReqs)
                            {
                                if (item.ID == serviceReq.ID)
                                {
                                    item.TEST_SAMPLE_TYPE_ID = oldTestSampleTypeId;
                                    break;
                                }
                            }
                            gridControlServiceReqView.RefreshDataSource();
                            gridControlServiceReqView.EndUpdate();
                        }

                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlServiceReqView_Click(object sender, EventArgs e)
        {

        }

        private void gridViewServiceReqView__TabService_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                V_HIS_SERVICE_REQ data = view.GetFocusedRow() as V_HIS_SERVICE_REQ;
                if (view.FocusedColumn.FieldName == "TEST_SAMPLE_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    List<MOS.EFMODEL.DataModels.HIS_TEST_SAMPLE_TYPE> testSampleType = LoadTestSampleType();
                    InitComboRepositoryTestSampleType(testSampleType, editor);
                    if (data != null)
                    {
                        if (data.TEST_SAMPLE_TYPE_ID != null)
                        {
                            editor.EditValue = data.TEST_SAMPLE_TYPE_ID;
                        }
                    }
                    editor.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
