﻿using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApplicationFont;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignServiceTestMulti.ADO;
using HIS.Desktop.Plugins.AssignServiceTestMulti.Config;
using HIS.Desktop.Plugins.AssignServiceTestMulti.Resources;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utilities.Extentions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.PopupLoader;
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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignServiceTestMulti.AssignService
{
    ///Thực tế, kho máu là do khoa xét nghiệm quản lý.
    ///Khi bác sỹ ở khoa lâm sàng/cấp cứu yêu cầu xuất máu thì chỉ yêu cầu chủng loại, số lượng. Khi nhận được yêu cầu xuất, khoa xét nghiệm sẽ lấy túi máu tương ứng với chủng loại để xuất. Tại thời điểm này, khoa XN cần phải thực hiện xét nghiệm lên chế phẩm máu (tương ứng với túi máu vừa lấy) để xác định xem túi máu đó có phù hợp với BN hay không. Xét nghiệm máu này sẽ được tính chi phí cho BN và ghi nhận cho khoa lâm sàng/cấp cứu.
    ///==> Giải pháp:
    ///Bổ sung chức năng nút chỉ định xét nghiệm ở màn hình thực xuất máu
    ///- Nút này chỉ enable sau khi thực xuất máu thành công
    ///- Khi nhấn nút này, mở ra form chỉ định xét nghiệm:
    ///+ Chỉ hiển thị dịch vụ xét nghiệm cho người dùng chọn
    ///+ Nếu phòng làm việc của người dùng có thể xử lý được dịch vụ xét nghiệm đó, thì mặc định hiển thị phòng xử lý dịch vụ là phòng của người dùng.
    ///+ Khi chỉ định thành công thì ghi nhận người chỉ định là người dùng, nhưng phòng/khoa chỉ định lấy theo phòng/khoa yêu cầu xuất máu chứ không lấy theo phòng/khoa của người dùng.
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        #region Reclare variable
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();

        const string hasPatientTypeDefault = "1";
        const string hasServiceReqObligateIcd = "1";
        const string hasAutoEnableEmergency = "1";
        const string isSingleCheckservice__True = "1";
        long? serviceReqParentId;
        int positionHandleControl = -1;
        int actionType = 0;
        long treatmentId = 0;
        long intructionTime = 0;
        long expMestId = 0;
        long serviceReqId = 0;

        V_HIS_SERE_SERV currentSereServ { get; set; }
        V_HIS_SERE_SERV currentSereServInEkip { get; set; }
        HIS.Desktop.ADO.AssignServiceTestADO.DelegateProcessDataResult processDataResult;
        HIS.Desktop.ADO.AssignServiceTestADO.DelegateProcessRefeshIcd processRefeshIcd;
        bool isInKip;
        string patientName;
        long patientDob;
        string genderName;
        bool isAutoEnableEmergency;

        HisTreatmentWithPatientTypeInfoSDO currentHisTreatment { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ currentHisServiceReq { get; set; }
        MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter = null;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> currentPatientTypeWithPatientTypeAlter = null;
        List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6> currentPreServiceReqs;

        HisServiceReqListResultSDO serviceReqComboResultSDO;

        HideCheckBoxHelper hideCheckBoxHelper__Service;
        BindingList<ServiceADO> records;
        List<SereServADO> ServiceIsleafADOs;
        List<ServiceADO> ServiceParentADOs;
        List<ServiceADO> ServiceAllADOs;
        ServiceADO SereServADO__Main;

        List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1> sereServWithTreatment = new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1>();
        Inventec.Desktop.Common.Modules.Module currentModule;

        const string commonString__true = "1";
        bool isLoad = false;
        ChiDinhDichVuADO chiDinhDichVuADO;
        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ currentServiceReq;
        List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> currentSereServs;
        Dictionary<long, List<V_HIS_SERVICE_PATY>> servicePatyInBranchs;
        Dictionary<long, V_HIS_SERVICE> dicServices;

        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        int lastRowHandle = -1;

        long currentServiceTypeId__Bed;
        private decimal currentExpendInServicePackage = 0;
        bool isSaveAndPrint = false;
        bool isInitForm = true;
        List<HIS_SERVICE_GROUP> selectedSeviceGroups;
        List<SereServADO> listDatasFix;
        #endregion

        #region Construct
        public frmAssignService(Inventec.Desktop.Common.Modules.Module Module, HIS.Desktop.ADO.AssignServiceTestADO dataADO)
        {
            try
            {
                InitializeComponent();

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

                this.actionType = GlobalVariables.ActionAdd;
                this.currentModule = Module;

                this.processDataResult = dataADO.DgProcessDataResult;
                this.processRefeshIcd = dataADO.DgProcessRefeshIcd;
                this.treatmentId = dataADO.TreatmentId;
                this.serviceReqParentId = dataADO.ServiceReqId;
                this.intructionTime = dataADO.IntructionTime;
                this.expMestId = dataADO.ExpMestId;
                this.serviceReqId = (dataADO.ServiceReqId ?? 0);
                this.isInKip = dataADO.IsInKip;
                if (this.isInKip)
                {
                    this.currentSereServInEkip = dataADO.SereServ;
                }
                else
                {
                    this.currentSereServ = dataADO.SereServ;
                }
                this.patientName = dataADO.PatientName;
                this.patientDob = dataADO.PatientDob;
                this.genderName = dataADO.GenderName;
                this.isAutoEnableEmergency = dataADO.IsAutoEnableEmergency;
                this.isInitForm = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignServiceTest.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignServiceTestMulti.AssignService.frmAssignService).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTomLuocVienPhi.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnTomLuocVienPhi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnServiceReqList.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnServiceReqList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.txtIcdExtraCodes.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssignService.txtIcdExtraCodes.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmAssignService.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barbtnSaveShortcut.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.barbtnSaveShortcut.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barbtnSaveAndPrintShortcut.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.barbtnSaveAndPrintShortcut.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barbtnPrintShortcut.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.barbtnPrintShortcut.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barbtnNew.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.barbtnNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSereservInTreatmentPreview.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnSereservInTreatmentPreview.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.chkExpendAll.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkExpendAll.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.cboServiceGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.chkEmergency.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkEmergency.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.chkPriority.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkPriority.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.txtIcdExtraNames.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssignService.txtIcdExtraNames.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.chkIcdServiceReq.Properties.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.chkIcdServiceReq.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.cboIcdServiceReq.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.cboIcdServiceReq.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSaveAndPrint.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnSaveAndPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnShowDetail.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnShowDetail.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExpendAll.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnExpendAll.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnUnExpendAll.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnUnExpendAll.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewServiceProcess.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmAssignService.gridViewServiceProcess.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcChecked_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcChecked_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcChecked_TabService.ToolTip = Inventec.Common.Resource.Get.Value("frmAssignService.grcChecked_TabService.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcServiceCode_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcServiceCode_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcServiceName_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcServiceName_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPatientTypeName__TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnPatientTypeName__TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcExpend_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcExpend_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnIsKH__TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnIsKH__TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnExecuteRoomName__TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnExecuteRoomName__TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemcboExcuteRoom_TabService.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemcboExcuteRoom_TabService.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcAmount_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcAmount_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcPrice_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcPrice_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grcPrice_ServicePatyPrpo.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.grcPrice_ServicePatyPrpo.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnServiceTypeName.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnServiceTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnChiPhiNgoaiGoi_TabService.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnChiPhiNgoaiGoi_TabService.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEstimateDuration.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.gridColumnEstimateDuration.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemBtnChecked__TabService.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemBtnChecked__TabService.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemcboPatientType_TabService1.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemcboPatientType_TabService1.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemcboPatientType_TabService.NullText = Inventec.Common.Resource.Get.Value("frmAssignService.repositoryItemcboPatientType_TabService.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmAssignService.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeService.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("frmAssignService.treeService.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.treeListServiceReqCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListServiceReqName.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.treeListServiceReqName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListPrice.Caption = Inventec.Common.Resource.Get.Value("frmAssignService.treeListPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciIcdMain.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciIcdMain.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lcitxtIcdExtraNames.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lcitxtIcdExtraNames.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGenderName.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciGenderName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDob.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciDob.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTotalServicePrice.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciTotalServicePrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIcdMainTextWithSereServ.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciIcdMainTextWithSereServ.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientTypeName.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciPatientTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmAssignService.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lcichkPriority.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lcichkPriority.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciEmergency.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciEmergency.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lcichkExpendAll.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lcichkExpendAll.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lcitxtIcdExtraCodes.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lcitxtIcdExtraCodes.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.toggleSwitchDataChecked.Properties.OffText = Inventec.Common.Resource.Get.Value("frmAssignService.toggleSwitchDataChecked.Properties.OffText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.toggleSwitchDataChecked.Properties.OnText = Inventec.Common.Resource.Get.Value("frmAssignService.toggleSwitchDataChecked.Properties.OnText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPreServiceReq.Text = Inventec.Common.Resource.Get.Value("frmAssignService.lciPreServiceReq.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Load data
        private void frmAssignService_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.Text = (this.currentModule != null ? this.currentModule.text : "");
                LogSystem.Debug("Starting...");
                this.gridControlServiceProcess.ToolTipController = this.tooltipService;
                this.SetDefaultData();

                long intructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtInstructionTime.DateTime).Value;
                this.currentHisTreatment = LoadDataToCurrentTreatmentData(this.treatmentId, intructionTime);
                LogSystem.Debug("Loaded currentHisTreatmentWithPatientTypeInfoSDO info (Truy van thong tin can cu theo treatment_id truyen vao lay thong tin ho so dieu trị, chan doan chinh phu, doi tuong dieu tri hien tai, thong tin tuyen & dang ky kcb ban dau neu doi tuong dieu tri hien tai la bhyt ==> su dung HisTreatmentWithPatientTypeInfoSDO)");

                ProcessDataWithTreatmentWithPatientTypeInfo();
                LogSystem.Debug("Loaded PatientType With HisTreatmentWithPatientTypeInfoSDO info");
                this.LoadServicePaty();

                this.CreateThreadLoadDataSereServWithTreatment(this.currentHisTreatment);
                LogSystem.Debug("Loaded CreateThreadLoadDataSereServWithTreatment (Truy vấn danh sách các loại thuốc đã kê trong ngày, lấy từ view v_his_sere_serv_8)");

                this.LoadTreatmentInfo__PatientType();//tinh toan va hien thi thong tin ve tong tien tat ca cac dich vu dang chi dinh
                LogSystem.Debug("Loaded LoadTotalPriceDataToTabMedicineReq (Gan du lieu luy ke tien thuoc lay tu DSereServ1)");

                //this.FillDataToComboPriviousServiceReq(this.currentHisTreatment);
                //LogSystem.Info("Loaded FillDataToComboPriviousServiceReq (lay du lieu cac dich vu chi dinh ngay hom truoc)");

                this.InitConfig();
               // this.SetCaptionByLanguageKey();
                this.InitTabIndex();
                LogSystem.Debug("Loaded default data");
                this.FillDataToControlsForm();
                LogSystem.Debug("Loaded fillDataToControlsForm");
                this.InitMenuToButtonPrint();
                LogSystem.Debug("Loaded InitMenuToButtonPrint");
                this.LoadIcdDefault();
                this.LoadDefaultUser();
                LogSystem.Debug("Loaded LoadIcdDefault, LoadDefaultUser");
                //Build tree service - grid sereserv
                this.BindTree();
                LogSystem.Debug("Loaded BindTree");

                if (this.currentSereServ != null)
                {
                    //Chỉ định từ màn hình xử lý PTTT
                    //lciIcdMain.Enabled = true;
                    this.lblDichVuChinh.Text = this.currentSereServ.TDL_SERVICE_NAME;
                    this.CreateThreadLoadDataByPackageService(this.currentSereServ);
                    this.SereServADO__Main = this.ServiceAllADOs.FirstOrDefault(o => o.ID == this.currentSereServ.SERVICE_ID);
                    LogSystem.Debug("Loaded CreateThreadLoadDataByPackageService");
                }
                else if (this.currentSereServInEkip != null)
                {
                    //Chỉ định từ màn hình xử lý PTTT
                    //lciIcdMain.Enabled = true;
                    this.lblDichVuChinh.Text = this.currentSereServInEkip.TDL_SERVICE_NAME;
                    this.CreateThreadLoadDataByPackageService(this.currentSereServInEkip);
                    this.SereServADO__Main = this.ServiceAllADOs.FirstOrDefault(o => o.ID == this.currentSereServInEkip.SERVICE_ID);
                    LogSystem.Debug("Loaded CreateThreadLoadDataByPackageService");
                }
                else
                {
                    this.LoadDataToGrid(false);
                }
                this.isInitForm = false;
                this.listDatasFix = this.gridControlServiceProcess.DataSource as List<SereServADO>;
                LogSystem.Debug("Loaded LoadDataToGrid");
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDefaultUser()
        {
            try
            {
                //string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                //var data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.LOGINNAME.ToUpper().Equals(loginName.ToUpper())).ToList();
                //if (data != null)
                //{
                //    cboUser.EditValue = data[0].LOGINNAME;
                //    txtLoginName.Text = data[0].LOGINNAME;
                //}

                ////Cấu hình để ẩn/hiện trường người chỉ định tai form chỉ định, kê đơn
                ////- Giá trị mặc định (hoặc ko có cấu hình này) sẽ ẩn
                ////- Nếu có cấu hình, đặt là 1 thì sẽ hiển thị
                //cboUser.Enabled = (AssignServiceConfig.ShowRequestUser == "1");
                //txtLoginName.Enabled = (AssignServiceConfig.ShowRequestUser == "1");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Service tree

        private void LoadServicePaty()
        {
            try
            {
                var patientTypeIdAls = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID);
                this.servicePatyInBranchs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>()
                    .Where(t => patientTypeIdAls.Contains(t.PATIENT_TYPE_ID))
                    .GroupBy(o => o.SERVICE_ID)
                    .ToDictionary(o => o.Key, o => o.ToList());

                this.dicServices = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>()
                    .Where(t => t.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .ToDictionary(o => o.ID);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToGrid(bool isAutoSetPaty)
        {
            try
            {
                this.gridViewServiceProcess.BeginUpdate();
                this.gridViewServiceProcess.ClearGrouping();
                var allDatas = this.ServiceIsleafADOs.AsQueryable();
                List<SereServADO> listSereServADO = null;
                if (this.toggleSwitchDataChecked.EditValue != null && (this.toggleSwitchDataChecked.EditValue ?? "").ToString().ToLower() == "true")
                {
                    listSereServADO = allDatas.Where(o => o.IsChecked).ToList();
                    this.ChangeStateGroupInGrid(this.groupType__ServiceTypeName);
                }
                else
                {
                    //Lay tat ca cac node duoc check tren tree
                    var nodeCheckeds = this.treeService.GetAllCheckedNodes();
                    if (nodeCheckeds != null && nodeCheckeds.Count > 0)
                    {
                        List<ServiceADO> parentNodes = new List<ServiceADO>();
                        listSereServADO = new List<SereServADO>();
                        List<long?> parentIds = new List<long?>();
                        List<long> serviceTypeIds = new List<long>();

                        //lay data cua cac dong tuong ung voi cac node duoc check
                        foreach (var node in nodeCheckeds)
                        {
                            var data = this.treeService.GetDataRecordByNode(node) as ServiceADO;
                            if (data != null)
                            {
                                parentNodes.Add(data);
                            }
                        }

                        //LogSystem.Info("parent nodes checked:" + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => parentNodes), parentNodes));

                        if (parentNodes.Count > 0)
                        {
                            var checkPtttSelected = parentNodes.Any(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT);
                            if (checkPtttSelected)
                            {
                                this.ChangeStateGroupInGrid(groupType__PtttGroupName);
                            }
                            else
                            {
                                this.ChangeStateGroupInGrid(groupType__ServiceTypeName);
                            }
                            var parentIdAllows = parentNodes.Select(o => o.ID).ToList();

                            //Lay tat ca cac dich vụ khong co cha cua tat ca cac loai dich vụ duoc check tren tree
                            var parentRootSetys = parentNodes.Where(o => String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
                            if (parentRootSetys != null && parentRootSetys.Count > 0)
                            {
                                foreach (var item in parentRootSetys)
                                {
                                    if (item != null)
                                    {
                                        var childOfParentNodeNoParents = allDatas.Where(o =>
                                        (o.PARENT_ID == null || item.ID == o.PARENT_ID)
                                        && o.TDL_SERVICE_TYPE_ID == item.SERVICE_TYPE_ID
                                        && parentIdAllows.Contains(o.PARENT_ID ?? 0)
                                        ).ToList();
                                        if (childOfParentNodeNoParents != null && childOfParentNodeNoParents.Count > 0)
                                        {
                                            listSereServADO.AddRange(childOfParentNodeNoParents);
                                        }
                                    }
                                }
                            }

                            //Lay ra tat ca cac dich vụ con cua dich vu cha va cac dich vu con cua con cua no neu co -> duyet de quy cho den het cay dich vu,..
                            var parentRoots = parentNodes.Where(o => !String.IsNullOrEmpty(o.PARENT_ID__IN_SETY)).ToList();
                            if (parentRoots != null && parentRoots.Count > 0)
                            {
                                foreach (var item in parentRoots)
                                {
                                    var childs = this.GetChilds(item);
                                    if (childs != null && childs.Count > 0)
                                    {
                                        listSereServADO.AddRange(childs);
                                    }
                                }
                            }
                            listSereServADO = listSereServADO.Distinct().ToList();
                        }
                    }
                    else
                    {
                        this.ChangeStateGroupInGrid(this.groupType__ServiceTypeName);
                        listSereServADO = allDatas.ToList();
                    }
                }

                this.gridViewServiceProcess.GridControl.DataSource = listSereServADO.Distinct().OrderBy(o => o.TDL_SERVICE_TYPE_ID).ThenByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
                this.gridViewServiceProcess.EndUpdate();
                this.SetEnableButtonControl(this.actionType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        int groupType__ServiceTypeName = 1;
        int groupType__PtttGroupName = 2;
        void ChangeStateGroupInGrid(int type)
        {
            try
            {
                if (type == groupType__ServiceTypeName)
                {
                    this.gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].GroupIndex = 0;
                    this.gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].SortOrder = ColumnSortOrder.Ascending;
                    this.gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].Visible = true;

                    this.gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].GroupIndex = -1;
                    this.gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].SortOrder = ColumnSortOrder.None;
                    this.gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].Visible = false;
                }
                else if (type == groupType__PtttGroupName)
                {
                    this.gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].GroupIndex = -1;
                    this.gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].SortOrder = ColumnSortOrder.None;
                    this.gridViewServiceProcess.Columns["SERVICE_TYPE_NAME"].Visible = false;

                    this.gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].GroupIndex = 0;
                    this.gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].SortOrder = ColumnSortOrder.Ascending;
                    this.gridViewServiceProcess.Columns["PTTT_GROUP_NAME"].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<SereServADO> GetChilds(ServiceADO parentNode)
        {
            List<SereServADO> result = new List<SereServADO>();
            try
            {
                if (parentNode != null)
                {
                    var childs = this.ServiceIsleafADOs.Where(o => o.PARENT_ID == parentNode.ID && o.TDL_SERVICE_TYPE_ID == parentNode.SERVICE_TYPE_ID).ToList();
                    if (childs != null && childs.Count > 0)
                    {
                        result.AddRange(childs);
                    }

                    var childOfParents = this.ServiceParentADOs.Where(o => o.PARENT_ID == parentNode.ID && o.SERVICE_TYPE_ID == parentNode.SERVICE_TYPE_ID).ToList();
                    if (childOfParents != null && childOfParents.Count > 0)
                    {
                        foreach (var item in childOfParents)
                        {
                            var childOfChilds = this.GetChilds(item);
                            if (childOfChilds != null && childOfChilds.Count > 0)
                            {
                                result.AddRange(childOfChilds);
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

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    var rowCount = (this.gridViewServiceProcess.DataSource as List<SereServADO>).Count();
                    if (rowCount == 1)
                    {
                        this.gridViewServiceProcess.Focus();
                        this.gridViewServiceProcess.FocusedRowHandle = 0;
                    }
                    else if (rowCount > 1)
                    {
                        this.gridViewServiceProcess.Focus();
                        this.gridViewServiceProcess.FocusedRowHandle = 1;
                    }
                    else
                    {
                        //Nothing
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    var rowCount = (this.gridViewServiceProcess.DataSource as List<SereServADO>).Count();
                    if (rowCount > 0)
                    {
                        var rowUpdate = this.gridViewServiceProcess.GetFocusedRow() as SereServADO;
                        rowUpdate.IsChecked = true;
                    }
                }
                else
                {
                    this.LoadDataToGrid(false);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE ChoosePatientTypeDefaultlService(long patientTypeId, long serviceId, SereServADO sereServADO)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                var patientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                if (patientTypes != null && patientTypes.Count > 0)
                {
                    var arrPatientTypeCode = this.servicePatyInBranchs[serviceId].Select(o => o.PATIENT_TYPE_CODE).ToList();

                    if (this.currentPatientTypeWithPatientTypeAlter != null
                        && arrPatientTypeCode != null
                        && arrPatientTypeCode.Count > 0)
                    {
                        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> dataCombo = this.currentPatientTypeWithPatientTypeAlter.Where(o => arrPatientTypeCode.Contains(o.PATIENT_TYPE_CODE)).ToList();
                        if (dataCombo != null && dataCombo.Count > 0)
                        {
                            var pt = dataCombo.FirstOrDefault(o => o.ID == patientTypeId);
                            if (pt != null)
                            {
                                result = patientTypes.FirstOrDefault(o => o.ID == pt.ID);
                            }
                            else
                            {
                                result = dataCombo[0];
                            }

                            if (result != null && sereServADO != null)
                            {
                                sereServADO.PATIENT_TYPE_ID = result.ID;
                                sereServADO.PATIENT_TYPE_CODE = result.PATIENT_TYPE_CODE;
                                sereServADO.PATIENT_TYPE_NAME = result.PATIENT_TYPE_NAME;
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool IsAssignDay(long serviceId)
        {
            bool result = false;
            try
            {
                if (this.sereServWithTreatment != null)
                {
                    long instructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((this.dtInstructionTime.EditValue ?? "").ToString()).ToString("yyyyMMdd"));
                    var sereServReplate = this.sereServWithTreatment.Where(o => o.SERVICE_ID == serviceId && o.INTRUCTION_TIME.ToString().Substring(0, 8) == instructionTime.ToString()).ToList();
                    if (sereServReplate != null && sereServReplate.Count > 0)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        #endregion

        #region Sereserv grid
        private void SetDefaultSerServTotalPrice()
        {
            try
            {
                //List<SereServADO> datas = ServiceIsleafADOs;
                decimal totalPrice = 0;
                foreach (var item in this.ServiceIsleafADOs)
                {
                    if (item.IsChecked && item.PATIENT_TYPE_ID != 0 && (item.IsExpend ?? false) == false)
                    {
                        var data_ServicePrice = this.servicePatyInBranchs[item.SERVICE_ID].Where(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID).OrderByDescending(m => m.MODIFY_TIME).ToList();
                        if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                        {
                            totalPrice += item.AMOUNT * (data_ServicePrice[0].PRICE * (1 + data_ServicePrice[0].VAT_RATIO));
                        }
                    }
                }
                this.lblTotalServicePrice.Text = Inventec.Common.Number.Convert.NumberToString(totalPrice, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnExpendAll_Click(object sender, EventArgs e)
        {
            try
            {
                this.treeService.CollapseAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnExpendAll_Click(object sender, EventArgs e)
        {
            try
            {
                this.treeService.ExpandAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeService_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.treeService.FocusedNode != null)
                {
                    //Process expand node
                    var parent = this.treeService.FocusedNode.ParentNode;
                    //Trường hợp node đang chọn có cha
                    if (parent != null)
                    {
                        this.ProcessExpandTree(this.treeService.FocusedNode);
                    }
                    //Trường hợp node đang chọn không có cha
                    else
                    {
                        this.treeService.CollapseAll();
                        this.treeService.FocusedNode.Expanded = true;
                        bool checkState = this.treeService.FocusedNode.Checked;
                        this.treeService.UncheckAll();
                        if (checkState)
                            this.treeService.FocusedNode.CheckAll();
                    }

                    //Process check state node is leaf
                    var data = this.treeService.GetDataRecordByNode(this.treeService.FocusedNode);
                    if (this.treeService.FocusedNode != null && !this.treeService.FocusedNode.HasChildren && data != null && data is ServiceADO)
                    {
                        //Cấu hình cho phép chọn một/nhiều nhóm dịch vụ trên cây là node lá
                        //Nếu không có cấu hình thì mặc định là chọn nhiều
                        //Nếu có cấu hình thì xử lý theo cấu hình
                        if (HisConfigCFG.IsSingleCheckservice == isSingleCheckservice__True)
                        {
                            if (parent != null)
                            {
                                parent.UncheckAll();
                            }
                            this.treeService.FocusedNode.Checked = true;
                        }
                        else
                        {
                            this.treeService.FocusedNode.Checked = !this.treeService.FocusedNode.Checked;
                        }
                    }

                    this.toggleSwitchDataChecked.EditValue = false;
                    this.LoadDataToGrid(true);
                    this.SetDefaultSerServTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessExpandTree(TreeListNode focusedNode)
        {
            try
            {
                TreeListNode parent = focusedNode.ParentNode;
                if (parent != null)
                {
                    this.treeService.CollapseAll();
                    List<TreeListNode> allParentNodes = new List<TreeListNode>();
                    this.GetParent(focusedNode, allParentNodes);
                    if (allParentNodes != null && allParentNodes.Count > 0)
                    {
                        var nodes = this.treeService.GetNodeList();
                        foreach (var item in nodes)
                        {
                            item.Checked = false;
                            if (focusedNode == item)
                            {
                                focusedNode.Expanded = true;
                                var childNodes = nodes.Where(o => o.ParentNode == focusedNode).ToList();
                                if (childNodes != null && childNodes.Count > 0)
                                {
                                    foreach (var childOfChild in childNodes)
                                    {
                                        //childOfChild.Expanded = true;
                                    }
                                }
                            }
                            else if (allParentNodes.Contains(item))
                            {
                                item.Expanded = true;
                            }
                            else
                            {
                                item.Expanded = false;
                            }
                        }
                    }
                }
                this.treeService.FocusedNode = focusedNode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetParent(TreeListNode focusedNode, List<TreeListNode> parentNodes)
        {
            try
            {
                if (focusedNode != null && focusedNode.ParentNode != null)
                {
                    parentNodes.Add(focusedNode.ParentNode);
                    this.GetParent(focusedNode.ParentNode, parentNodes);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeService_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = treeService.GetDataRecordByNode(e.Node);
                if (data != null && data is ServiceADO)
                {
                    var noteData = (ServiceADO)data;
                    if (String.IsNullOrEmpty(noteData.PARENT_ID__IN_SETY) && noteData.ID == 0)
                    {
                        //if (IsExistsCheckedChildNodes__TabService(e.Node))
                        //{
                        //    e.Appearance.ForeColor = Color.Red;
                        //}
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeService_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            try
            {
                if (HisConfigCFG.IsSingleCheckservice == isSingleCheckservice__True)
                {
                    this.treeService.FocusedNode = e.Node;
                }
                this.toggleSwitchDataChecked.EditValue = false;
                this.LoadDataToGrid(true);
                this.SetDefaultSerServTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeService_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeService_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.treeService.FocusedNode != null)
                    {
                        this.treeService.FocusedNode.Checked = true;
                        this.gridControlServiceProcess.Focus();
                        this.gridViewServiceProcess.FocusedRowHandle = DevExpress.XtraGrid.GridControl.AutoFilterRowHandle;
                    }
                }
                else if (e.KeyCode == Keys.Space)
                {
                    var node = this.treeService.FocusedNode;
                    var data = this.treeService.GetDataRecordByNode(node);
                    if (node != null && node.HasChildren && data != null && data is ServiceADO)
                    {
                        node.Expanded = !node.Expanded;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeService_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                //var node = treeService.FocusedNode;
                //var data = treeService.GetDataRecordByNode(node);
                //if (node != null && !node.HasChildren && data != null && data is ServiceADO)
                //{
                //    //node.CheckState = (node.Checked ? CheckState.Unchecked : CheckState.Checked);
                //    node.Checked = !node.Checked;

                //    //if (node.Checked)
                //    //{
                //    //    node.CheckAll();
                //    //}
                //    //else
                //    //{
                //    //    node.UncheckAll();
                //    //}
                //    toggleSwitchDataChecked.EditValue = false;
                //    LoadDataToGrid(true);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toggleSwitchDataChecked_Toggled(object sender, EventArgs e)
        {
            try
            {
                ToggleSwitch toggleSwitch = sender as ToggleSwitch;
                if (toggleSwitch != null)
                {
                    this.LoadDataToGrid(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                //SereServADO data = null;
                //if (e.RowHandle > -1)
                //{
                //    data = (SereServADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                //}
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "PATIENT_TYPE_ID")
                    {
                        e.RepositoryItem = this.repositoryItemcboPatientType_TabService;
                    }
                    else if (e.Column.FieldName == "IsExpend")
                    {
                        e.RepositoryItem = this.repositoryItemChkIsExpend_TabService;
                    }
                    else if (e.Column.FieldName == "IsKHBHYT")
                    {
                        if (this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                        {
                            e.RepositoryItem = this.repositoryItemChkIsKH_TabService;
                        }
                        else
                        {
                            //e.RepositoryItem = this.repositoryItemChkIsKH_Disable__Testpage;
                        }
                    }
                    else if (e.Column.FieldName == "AMOUNT")
                    {
                        short isMultiRequest = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(e.RowHandle, view.Columns["IS_MULTI_REQUEST"]) ?? "").ToString());
                        if (isMultiRequest == 1)
                        {
                            e.RepositoryItem = this.repositoryItemSpinAmount_TabService;
                        }
                        else
                        {
                            e.RepositoryItem = this.repositoryItemSpinAmount__Disable_TabService;
                        }
                    }
                    else if (e.Column.FieldName == "TDL_EXECUTE_ROOM_ID")
                    {
                        e.RepositoryItem = this.repositoryItemcboExcuteRoom_TabService;
                    }
                    else if (e.Column.FieldName == "IsOutKtcFee")
                    {
                        //if (data.HEIN_SERVICE_TYPE_ID == HisServiceReportCFG.SERVICE_REPORT_ID__HIGHTECH)  
                        e.RepositoryItem = this.repositoryItemChkIsOutKtcFee_Enable_TabService;
                        //else
                        //{
                        //    e.RepositoryItem = this.repositoryItemChkIsOutKtcFee_Disable_TabService;
                        //}
                    }
                    else if (e.Column.FieldName == "ShareCount")
                    {
                        long serviceTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(e.RowHandle, view.Columns["SERVICE_TYPE_ID"]) ?? "").ToString());
                        if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G)
                        {
                            e.RepositoryItem = this.repositoryItemcboShareCount;
                        }
                        else
                        {
                            e.RepositoryItem = this.repositoryItemTxtReadOnly;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultExecuteRoom(SereServADO sereServADO)
        {
            try
            {
                var serviceRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                var executeRoomViews = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                if (executeRoomViews != null && serviceRoomViews != null && serviceRoomViews.Count > 0)
                {
                    var arrExcuteRoomCode = serviceRoomViews.Where(o => sereServADO != null && o.SERVICE_ID == sereServADO.SERVICE_ID).Select(o => o.ROOM_ID).ToList();
                    if (arrExcuteRoomCode != null && arrExcuteRoomCode.Count > 0)
                    {
                        List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM> executeRooms = executeRoomViews.Where(o => arrExcuteRoomCode.Contains(o.ROOM_ID)).ToList();
                        var roomCheck = executeRooms.Any(o => o.ROOM_ID == currentModule.RoomId);
                        if (roomCheck && sereServADO.TDL_EXECUTE_ROOM_ID <= 0)
                        {
                            sereServADO.TDL_EXECUTE_ROOM_ID = currentModule.RoomId;
                        }
                        else if (executeRooms != null && executeRooms.Count == 1)
                        {
                            sereServADO.TDL_EXECUTE_ROOM_ID = executeRooms[0].ROOM_ID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
                if (sereServADO != null)
                {
                    if (e.Column.FieldName == this.grcChecked_TabService.FieldName
                        || e.Column.FieldName == this.grcExpend_TabService.FieldName
                        || e.Column.FieldName == this.grcAmount_TabService.FieldName
                        || e.Column.FieldName == this.gridColumnExecuteRoomName__TabService.FieldName)
                    {
                        //if (e.Column.FieldName == grcChecked_TabService.FieldName)
                        //{
                        //    //ResetCheckStateInGridService();
                        //    sereServADO.IsChecked = true;
                        //}
                        if (sereServADO.IsChecked)
                        {
                            long instructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((this.dtInstructionTime.EditValue ?? "").ToString()).ToString("yyyyMMdd"));
                            this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);
                            this.SetDefaultExecuteRoom(sereServADO);
                            this.ValidServiceDetailProcessing(sereServADO, true);

                            //Kiểm tra dịch vụ đã chỉ định trong ngày thì cảnh báo
                            //sereServADO.ErrorMessageIsAssignDay = "";
                            //sereServADO.ErrorTypeIsAssignDay = ErrorType.None;
                            //if (this.sereServWithTreatment != null && this.sereServWithTreatment.Count > 0)
                            //{
                            //    var sereServReplate = this.sereServWithTreatment.Where(o => o.SERVICE_ID == sereServADO.SERVICE_ID
                            //        && o.INTRUCTION_TIME.ToString().Substring(0, 8) == instructionTime.ToString()).ToList();
                            //    if (sereServReplate != null && sereServReplate.Count > 0)
                            //    {
                            //        sereServADO.ErrorMessageIsAssignDay = ResourceMessage.CanhBaoDichVuDaChiDinhTrongNgay;
                            //        sereServADO.ErrorTypeIsAssignDay = ErrorType.Warning;
                            //    }
                            //}
                        }
                        else
                        {
                            this.ResetOneService(sereServADO);
                        }
                        this.gridControlServiceProcess.RefreshDataSource();
                        this.SetEnableButtonControl(this.actionType);
                    }

                    this.SetDefaultSerServTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
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

        private void gridViewServiceProcess_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                SereServADO data = view.GetFocusedRow() as SereServADO;
                if (view.FocusedColumn.FieldName == "PATIENT_TYPE_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        this.FillDataIntoPatientTypeCombo(data, editor);
                        editor.EditValue = data.PATIENT_TYPE_ID;
                    }
                }
                else if (view.FocusedColumn.FieldName == "TDL_EXECUTE_ROOM_ID" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    this.FillDataIntoExcuteRoomCombo(data, editor);
                    editor.EditValue = data.TDL_EXECUTE_ROOM_ID;
                }
                else if (view.FocusedColumn.FieldName == "IsKHBHYT" && view.ActiveEditor is CheckEdit)
                {
                    CheckEdit editor = view.ActiveEditor as CheckEdit;
                    editor.ReadOnly = true;
                    // Kiểm tra các điều kiện: 
                    //1. Đối tượng BN = BHYT
                    //2. Loại hình thanh toán !=BHYT
                    //3. Dịch vụ đó có giá bán = BHYT
                    //4. Dịch vụ đó có giá bán BHYT<giá bán của loại đối tượng TT (xemlai...)
                    if (this.currentHisPatientTypeAlter != null
                        && this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                        && data.PATIENT_TYPE_ID != this.currentHisPatientTypeAlter.PATIENT_TYPE_ID)
                    {
                        var isExistsService = this.servicePatyInBranchs[data.SERVICE_ID].Any(o => o.PATIENT_TYPE_ID == this.currentHisPatientTypeAlter.PATIENT_TYPE_ID);
                        if (isExistsService)
                        {
                            editor.ReadOnly = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "AMOUNT"
                    || e.ColumnName == "PATIENT_TYPE_ID"
                    || e.ColumnName == "TDL_SERVICE_NAME"
                    //|| e.ColumnName == "TDL_EXECUTE_ROOM_ID"
                    )
                {
                    this.gridViewServiceProcess_CustomRowError(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = this.gridViewServiceProcess.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listDatas = this.gridControlServiceProcess.DataSource as List<SereServADO>;
                var row = listDatas[index];
                if (e.ColumnName == "AMOUNT")
                {
                    if (row.IsChecked && row.ErrorTypeAmount == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeAmount);
                        e.Info.ErrorText = (string)(row.ErrorMessageAmount);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "PATIENT_TYPE_ID")
                {
                    if (row.IsChecked && row.ErrorTypePatientTypeId == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypePatientTypeId);
                        e.Info.ErrorText = (string)(row.ErrorMessagePatientTypeId);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                else if (e.ColumnName == "TDL_SERVICE_NAME")
                {
                    if (row.ErrorTypeIsAssignDay == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeIsAssignDay);
                        e.Info.ErrorText = (string)(row.ErrorMessageIsAssignDay);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
                //else if (e.ColumnName == "TDL_EXECUTE_ROOM_ID")
                //{
                //    if (row.ErrorTypeExecuteRoom == ErrorType.Warning)
                //    {
                //        e.Info.ErrorType = (ErrorType)(row.ErrorTypeExecuteRoom);
                //        e.Info.ErrorText = (string)(row.ErrorMessageExecuteRoom);
                //    }
                //    else
                //    {
                //        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                //        e.Info.ErrorText = "";
                //    }
                //}
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

        private void gridViewServiceProcess_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        SereServADO oneServiceSDO = (SereServADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (oneServiceSDO != null)
                        {
                            if (e.Column.FieldName == "PRICE_DISPLAY")
                            {
                                if (oneServiceSDO.PATIENT_TYPE_ID != 0 && this.servicePatyInBranchs.ContainsKey(oneServiceSDO.SERVICE_ID))
                                {
                                    var oneServicePatyPrice = this.servicePatyInBranchs[oneServiceSDO.SERVICE_ID]
                                    .Where(o => o.PATIENT_TYPE_ID == oneServiceSDO.PATIENT_TYPE_ID)
                                    .OrderByDescending(m => m.MODIFY_TIME).FirstOrDefault();
                                    if (oneServicePatyPrice != null)
                                    {
                                        e.Value = (oneServicePatyPrice.PRICE * (1 + oneServicePatyPrice.VAT_RATIO));
                                    }
                                }
                            }
                            if (e.Column.FieldName == "PRICE_PRPO_DISPLAY")
                            {
                                if (oneServiceSDO.PATIENT_TYPE_ID != 0 && this.servicePatyInBranchs.ContainsKey(oneServiceSDO.SERVICE_ID))
                                {
                                    if (this.dicServices.ContainsKey(oneServiceSDO.SERVICE_ID))
                                    {
                                        var oneServicePrice = this.dicServices[oneServiceSDO.SERVICE_ID];
                                        if (oneServicePrice != null)
                                        {
                                            e.Value = (oneServicePrice.PACKAGE_PRICE);
                                        }
                                    }
                                }
                            }
                            else if (e.Column.FieldName == "ESTIMATE_DURATION_DISPLAY")
                            {
                                var oneService = this.ServiceAllADOs.Where(o => o.ID == oneServiceSDO.SERVICE_ID).FirstOrDefault();
                                if (oneService != null)
                                {
                                    e.Value = oneService.ESTIMATE_DURATION;
                                }
                            }
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (!ValidAdd()) return;

                GridView view = (GridView)sender;
                Point pt = view.GridControl.PointToClient(Control.MousePosition);
                GridHitInfo info = view.CalcHitInfo(pt);
                if ((info.InRow || info.InRowCell)
                    && info.Column.FieldName != this.grcChecked_TabService.FieldName
                    && info.Column.FieldName != this.gridColumnPatientTypeName__TabService.FieldName
                    && info.Column.FieldName != this.grcAmount_TabService.FieldName
                    && info.Column.FieldName != this.grcExpend_TabService.FieldName)
                {
                    var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
                    if (sereServADO != null)
                    {
                        sereServADO.IsChecked = !sereServADO.IsChecked;
                        if (sereServADO.IsChecked)
                        {
                            long instructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((this.dtInstructionTime.EditValue ?? "").ToString()).ToString("yyyyMMdd"));
                            this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);
                            sereServADO.IsKHBHYT = null;
                            this.SetDefaultExecuteRoom(sereServADO);
                            this.ValidServiceDetailProcessing(sereServADO, true);
                        }
                        else
                        {
                            this.ResetOneService(sereServADO);
                        }
                        this.gridControlServiceProcess.RefreshDataSource();
                        this.SetEnableButtonControl(this.actionType);
                        this.SetDefaultSerServTotalPrice();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
                {
                    var sereServADO = (SereServADO)this.gridViewServiceProcess.GetFocusedRow();
                    if (sereServADO != null)
                    {
                        sereServADO.IsChecked = !sereServADO.IsChecked;
                        if (sereServADO.IsChecked)
                        {
                            this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, sereServADO.SERVICE_ID, sereServADO);
                            this.SetDefaultExecuteRoom(sereServADO);
                            this.ValidServiceDetailProcessing(sereServADO, true);
                        }
                        else
                        {
                            this.ResetOneService(sereServADO);
                        }
                        this.gridControlServiceProcess.RefreshDataSource();
                        this.SetEnableButtonControl(this.actionType);
                        this.SetDefaultSerServTotalPrice();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ResetOneService(SereServADO item)
        {
            try
            {
                item.PATIENT_TYPE_ID = 0;
                item.PATIENT_TYPE_CODE = null;
                item.PATIENT_TYPE_NAME = null;
                item.TDL_EXECUTE_ROOM_ID = 0;
                item.PRICE = 0;

                item.ErrorMessageAmount = "";
                item.ErrorTypeAmount = ErrorType.None;
                item.ErrorMessagePatientTypeId = "";
                item.ErrorTypePatientTypeId = ErrorType.None;
                item.ErrorMessageExecuteRoom = "";
                item.ErrorTypeExecuteRoom = ErrorType.None;
                item.ErrorMessageIsAssignDay = "";
                item.ErrorTypeIsAssignDay = ErrorType.None;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ResetCheckStateInGridService()
        {
            try
            {
                var listDatas = this.gridControlServiceProcess.DataSource as List<SereServADO>;
                if (listDatas != null && listDatas.Count > 0)
                {
                    foreach (var item in listDatas)
                    {
                        item.IsChecked = false;
                        this.ResetOneService(item);
                    }
                    this.gridControlServiceProcess.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewServiceProcess_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                var index = this.gridViewServiceProcess.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    return;
                }
                var listDatas = this.gridControlServiceProcess.DataSource as List<SereServADO>;
                var dataRow = listDatas[index];
                if (dataRow != null && dataRow.PATIENT_TYPE_ID > 0)
                {
                    //Đối tượng điều trị là BHYT, nhưng do ko có chính sách giá theo BHYT nên khi tích chọn dịch vụ, sẽ hiển thị màu tím.
                    //Có chính sách giá nhưng là đối tượng khác, không phải BHYT ==> màu tím
                    if (this.currentHisPatientTypeAlter != null
                        && this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                        && dataRow.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT)
                    {
                        var isExtstsService = this.servicePatyInBranchs[dataRow.SERVICE_ID].Any(o => o.PATIENT_TYPE_ID == this.currentHisPatientTypeAlter.PATIENT_TYPE_ID);
                        if (isExtstsService)
                            e.Appearance.ForeColor = System.Drawing.Color.Violet;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CustomDrawGroupRow(object sender, DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                string rowValue = Convert.ToString(this.gridViewServiceProcess.GetGroupRowValue(e.RowHandle, info.Column));
                info.GroupText = rowValue;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void tooltipService_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == this.gridControlServiceProcess)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = this.gridControlServiceProcess.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        //if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        //{
                        //    lastColumn = info.Column;
                        //    lastRowHandle = info.RowHandle;
                        //    string text = text = (view.GetRowCellValue(lastRowHandle, "SERVICE_REQ_STT_NAME") ?? "").ToString();
                        //    if (info.Column.FieldName == "IMG")
                        //    {
                        //        text = (view.GetRowCellValue(lastRowHandle, "SERVICE_REQ_STT_NAME") ?? "").ToString();
                        //    }
                        //    if (info.Column.FieldName == "PRIORITY_DISPLAY")
                        //    {
                        //        string priority = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCSERVICE_REQUEST_LIST_PRIORITY", EXE.APP.Resources.ResourceLanguageManager.LanguageUCServiceRequestList, EXE.LOGIC.Base.LanguageManager.GetCulture());
                        //        text = priority.ToString();
                        //    }
                        //    lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        //}
                        //e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableButtonControl(int actionType)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
                    this.btnSave.Enabled = this.btnSaveAndPrint.Enabled = (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0);
                    this.pnlPrintAssignService.Enabled = this.btnShowDetail.Enabled = false;
                }
                else
                {
                    this.btnSave.Enabled = this.btnSaveAndPrint.Enabled = false;
                    this.pnlPrintAssignService.Enabled = this.btnShowDetail.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ValidAdd()
        {
            bool valid = true;
            try
            {
                if (this.currentHisPatientTypeAlter == null || this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == 0)
                {
                    MessageManager.Show(String.Format(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh, this.dtInstructionTime.Text));
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void ChangeLockButtonWhileProcess(bool isLock)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionView)
                    return;

                this.btnSave.Enabled = isLock;
                this.btnSaveAndPrint.Enabled = isLock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Button

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.ProcessSaveData(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                this.ProcessSaveData(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSaveData(bool isSaveAndPrint)
        {
            try
            {
                LogSystem.Debug("ProcessSaveData => 1");
                if (this.gridViewServiceProcess.IsEditing)
                    this.gridViewServiceProcess.CloseEditor();

                if (this.gridViewServiceProcess.FocusedRowModified)
                    this.gridViewServiceProcess.UpdateCurrentRow();

                List<SereServADO> serviceCheckeds__Send = this.ServiceIsleafADOs.FindAll(o => o.IsChecked);
                if (this.Valid(serviceCheckeds__Send))
                {
                    LogSystem.Debug("ProcessSaveData => 2");
                    this.ChangeLockButtonWhileProcess(false);
                    AssignTestForBloodSDO serviceReqSDO = new AssignTestForBloodSDO();

                    this.ProcessServiceReqSDO(serviceReqSDO, serviceCheckeds__Send);
                    // check theo trường MIN_DURATION (trong HIS_SERVICE)
                    var services = BackendDataWorker.Get<V_HIS_SERVICE>();
                    serviceCheckeds__Send.ForEach(o => o.MIN_DURATION = services.FirstOrDefault(m => m.ID == o.SERVICE_ID).MIN_DURATION);
                    Inventec.Common.Logging.LogSystem.Warn("Du lieu dau vao serviceCheckeds__Send:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceCheckeds__Send), serviceCheckeds__Send));
                    List<HIS_SERE_SERV> sereServMinDurations = getSereServWithMinDuration(serviceCheckeds__Send);
                    Inventec.Common.Logging.LogSystem.Warn("doi tuong sereServMinDurations sau khi xu ly: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServMinDurations), sereServMinDurations));
                    if (sereServMinDurations != null && sereServMinDurations.Count > 0)
                    {
                        string sereServMinDurationStr = "";
                        foreach (var item in sereServMinDurations)
                        {
                            sereServMinDurationStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + "; ";
                        }
                        if (MessageBox.Show(string.Format(ResourceMessage.DichVuCoThoiGianChiDinhNamTrongKhoangThoiGianKhongChoPhep, sereServMinDurationStr), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.No)
                            return;
                    }
                    LogSystem.Debug("ProcessSaveData => 3");
                    this.ProcessServiceReqSDO_ServiceIcd(serviceReqSDO);
                    LogSystem.Debug("ProcessSaveData => 4");
                    this.SaveServiceReqCombo(serviceReqSDO, isSaveAndPrint);
                    LogSystem.Debug("ProcessSaveData => 5");
                    this.ChangeLockButtonWhileProcess(true);
                }
            }
            catch (Exception ex)
            {
                this.ChangeLockButtonWhileProcess(true);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_SERE_SERV> getSereServWithMinDuration(List<SereServADO> serviceCheckeds)
        {
            List<HIS_SERE_SERV> listSereServResult = null;
            try
            {
                if (serviceCheckeds != null && serviceCheckeds.Count > 0)
                {
                    List<SereServADO> sereServADOExistMinDUration = serviceCheckeds.Where(o => o.MIN_DURATION != null).ToList();
                    if (sereServADOExistMinDUration != null && sereServADOExistMinDUration.Count > 0)
                    {
                        List<ServiceDuration> serviceDurations = new List<ServiceDuration>();
                        foreach (var item in sereServADOExistMinDUration)
                        {
                            ServiceDuration serviceDuration = new ServiceDuration();
                            serviceDuration.ServiceId = item.SERVICE_ID;
                            serviceDuration.MinDuration = (item.MIN_DURATION ?? 0);
                            serviceDurations.Add(serviceDuration);
                        }
                        // gọi api để lấy về thông báo
                        CommonParam param = new CommonParam();
                        HisSereServMinDurationFilter hisSereServMinDurationFilter = new HisSereServMinDurationFilter();
                        hisSereServMinDurationFilter.ServiceDurations = serviceDurations;

                        if (dtInstructionTime.EditValue != null && dtInstructionTime.DateTime != DateTime.MinValue)
                            hisSereServMinDurationFilter.InstructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(
                                Convert.ToDateTime(dtInstructionTime.EditValue).ToString("yyyyMMddHHmmss"));

                        hisSereServMinDurationFilter.PatientId = this.currentHisTreatment.PATIENT_ID;
                        Inventec.Common.Logging.LogSystem.Warn("du lieu dau vao khi goi api HisSereServ/GetExceedMinDuration: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisSereServMinDurationFilter), hisSereServMinDurationFilter));
                        listSereServResult = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetExceedMinDuration", ApiConsumer.ApiConsumers.MosConsumer, hisSereServMinDurationFilter, param);
                        Inventec.Common.Logging.LogSystem.Warn("ket qua tra ve khi goi api HisSereServ/GetExceedMinDuration: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listSereServResult), listSereServResult));
                        var listSereServResultTemp = from SereServResult in listSereServResult
                                                     group SereServResult by SereServResult.SERVICE_ID into g
                                                     orderby g.Key
                                                     select g.FirstOrDefault();
                        listSereServResult = listSereServResultTemp.ToList();

                    }
                    else
                    {
                        listSereServResult = null;
                    }
                }
                else
                {
                    listSereServResult = null;
                }
            }
            catch (Exception ex)
            {
                listSereServResult = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return listSereServResult;
        }

        private void barbtnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barbtnSaveShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.btnSave.Enabled)
                    this.btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barbtnSaveAndPrintShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.btnSaveAndPrint.Enabled)
                    this.btnSaveAndPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barbtnPrintShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.pnlPrintAssignService.Enabled)
                {
                    //Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    //richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037, DelegateRunPrinter);
                    DelegateRunPrinter(PrintTypeCodeStore.PRINT_TYPE_CODE__IN__PHIEU_YEU_CAU_CHI_DINH_TONG_HOP__MPS000037);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetDefaultData();
                this.LoadIcdDefault();
                this.CreateThreadLoadDataSereServWithTreatment(this.currentHisTreatment);
                LogSystem.Debug("Loaded CreateThreadLoadDataSereServWithTreatment (Truy vấn danh sách các loại thuốc đã kê trong ngày, lấy từ view v_his_sere_serv_8)");

                this.treeService.UncheckAll();
                this.gridViewServiceProcess.BeginUpdate();
                foreach (var item in this.ServiceIsleafADOs)
                {
                    item.IsChecked = false;
                    item.ShareCount = null;
                    item.PATIENT_TYPE_ID = 0;
                    item.PRICE = 0;
                    item.TDL_EXECUTE_ROOM_ID = 0;
                    item.HEIN_LIMIT_PRICE = null;
                    item.SERVICE_GROUP_ID_SELECTEDs = null;
                    item.ErrorMessageAmount = "";
                    item.ErrorMessageIsAssignDay = "";
                    item.ErrorMessagePatientTypeId = "";
                    item.ErrorMessageExecuteRoom = "";
                    item.ErrorTypeExecuteRoom = ErrorType.None;
                    item.ErrorTypeAmount = ErrorType.None;
                    item.ErrorTypeIsAssignDay = ErrorType.None;
                    item.ErrorTypePatientTypeId = ErrorType.None;
                }
                this.gridViewServiceProcess.GridControl.DataSource = ServiceIsleafADOs;
                this.gridViewServiceProcess.EndUpdate();
                this.SetEnableButtonControl(this.actionType);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void barbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnShowDetail_Click(object sender, EventArgs e)
        {
            try
            {
                frmDetail frmDetail = new frmDetail(serviceReqComboResultSDO, currentHisPatientTypeAlter, this.currentHisTreatment);
                frmDetail.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSereservInTreatmentPreview_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SereservInTreatment").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SereservInTreatment'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SereservInTreatment' is not plugins");

                SereservInTreatmentADO sereservInTreatmentADO = new SereservInTreatmentADO(treatmentId, intructionTime, serviceReqParentId ?? 0);
                List<object> listArgs = new List<object>();
                listArgs.Add(sereservInTreatmentADO);
                listArgs.Add(this.currentModule);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).Show();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnServiceReqList_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ServiceReqList'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.ServiceReqList' is not plugins");

                MOS.EFMODEL.DataModels.HIS_TREATMENT treatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();//treatmentId, intructionTime, serviceReqParentId ?? 0
                treatment.ID = treatmentId;
                List<object> listArgs = new List<object>();
                listArgs.Add(treatment);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).Show();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTomLuocVienPhi_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrHospitalFees").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.AggrHospitalFees'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.AggrHospitalFees' is not plugins");

                List<object> listArgs = new List<object>();
                listArgs.Add(treatmentId);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).Show();
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Control editor
        private void FocusShowpopup(GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopup(LookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInstructionTime_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!this.isInitForm)
                {
                    long instructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtInstructionTime.DateTime).Value;
                    this.currentHisTreatment = this.LoadDataToCurrentTreatmentData(this.treatmentId, instructionTime);
                    this.LoadDataSereServWithTreatment(this.currentHisTreatment, dtInstructionTime.DateTime);
                    this.ProcessDataWithTreatmentWithPatientTypeInfo();
                    this.InitComboRepositoryPatientType(this.currentPatientTypeWithPatientTypeAlter);
                    this.LoadTreatmentInfo__PatientType();
                    if (this.currentHisPatientTypeAlter == null || this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == 0)
                    {
                        MessageManager.Show(String.Format(ResourceMessage.KhongTimThayDoiTuongThanhToanTrongThoiGianYLenh, this.dtInstructionTime.Text));
                        this.dtInstructionTime.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInstructionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.cboServiceGroup.Focus();
                    this.cboServiceGroup.SelectAll();
                    this.cboServiceGroup.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectOneServiceGroupProcess(List<HIS_SERVICE_GROUP> svgrs)
        {
            try
            {
                List<SereServADO> services = null;
                StringBuilder strMessage = new StringBuilder();
                StringBuilder strMessageTemp__CoDichVuKhongCauHinh = new StringBuilder();
                StringBuilder strMessageTemp__KhongDichVu = new StringBuilder();
                bool hasMessage = false;
                this.ResetServiceGroupSelected();
                if (svgrs != null && svgrs.Count > 0)
                {
                    var idSelecteds = svgrs.Select(o => o.ID).Distinct().ToList();
                    var servSegrAllows = BackendDataWorker.Get<V_HIS_SERV_SEGR>().Where(o => idSelecteds.Contains(o.SERVICE_GROUP_ID)).ToList();
                    if (servSegrAllows != null && servSegrAllows.Count > 0)
                    {
                        var serviceOfGroupsInGroupbys = servSegrAllows.GroupBy(o => o.SERVICE_GROUP_ID).ToDictionary(o => o.Key, o => o.ToList());
                        foreach (var item in serviceOfGroupsInGroupbys)
                        {
                            List<V_HIS_SERV_SEGR> servSegrErrors = new List<V_HIS_SERV_SEGR>();
                            foreach (var svInGr in serviceOfGroupsInGroupbys[item.Key])
                            {
                                var service = this.ServiceIsleafADOs.FirstOrDefault(o => svInGr.SERVICE_ID == o.SERVICE_ID);
                                if (service != null)
                                {
                                    service.IsChecked = true;
                                    service.IsKHBHYT = false;
                                    service.SERVICE_GROUP_ID_SELECTEDs = idSelecteds;
                                    var searchServiceOfGroups = servSegrAllows.Where(o => o.SERVICE_ID == service.SERVICE_ID).ToList();
                                    if (searchServiceOfGroups != null)
                                    {
                                        service.AMOUNT = searchServiceOfGroups.Sum(o => o.AMOUNT);
                                        service.IsExpend = (searchServiceOfGroups[0].IS_EXPEND == 1);
                                    }
                                    this.ChoosePatientTypeDefaultlService(this.currentHisPatientTypeAlter.PATIENT_TYPE_ID, service.SERVICE_ID, service);
                                    this.ValidServiceDetailProcessing(service);
                                }
                                else
                                {
                                    servSegrErrors.Add(svInGr);
                                }
                            }

                            if (servSegrErrors != null && servSegrErrors.Count > 0)
                            {
                                if (String.IsNullOrEmpty(strMessageTemp__CoDichVuKhongCauHinh.ToString()))
                                {
                                    strMessageTemp__CoDichVuKhongCauHinh.Append("; ");
                                }
                                strMessageTemp__CoDichVuKhongCauHinh.Append(String.Format(ResourceMessage.NhomDichVuChiTiet, Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(servSegrErrors[0].SERVICE_GROUP_NAME, Color.Red), FontStyle.Bold), String.Join(",", servSegrErrors.Select(o => Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(o.SERVICE_CODE, Color.Black), FontStyle.Bold)))));

                                hasMessage = true;
                            }
                            servSegrErrors = new List<V_HIS_SERV_SEGR>();
                        }

                        services = this.ServiceIsleafADOs.Where(o => o.IsChecked).OrderByDescending(o => o.SERVICE_NUM_ORDER).ThenBy(o => o.TDL_SERVICE_NAME).ToList();
                    }
                    var sgNotIn = servSegrAllows.Select(o => o.SERVICE_GROUP_ID).Distinct().ToArray();
                    var searchServiceOfGroups__NoService = svgrs.Where(o => !sgNotIn.Contains(o.ID)).ToList();
                    if (searchServiceOfGroups__NoService != null && searchServiceOfGroups__NoService.Count > 0)
                    {
                        strMessageTemp__KhongDichVu.Append(String.Join(",", searchServiceOfGroups__NoService.Select(o => Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(o.SERVICE_GROUP_NAME, Color.Red), FontStyle.Bold))));
                        hasMessage = true;
                    }

                    this.toggleSwitchDataChecked.EditValue = true;

                    if (hasMessage)
                    {
                        strMessage.Append(ResourceMessage.NhomDichVuCoDichVuDuocCauHinhTrongNhomNhungKhongCoCauHinhChinhSach);
                        if (!String.IsNullOrEmpty(strMessageTemp__CoDichVuKhongCauHinh.ToString()))
                        {
                            strMessage.Append("\r\n" + String.Format(ResourceMessage.NhomDichVuCoDichVuKhongCoCauHinh, strMessageTemp__CoDichVuKhongCauHinh.ToString()));
                        }
                        if (!String.IsNullOrEmpty(strMessageTemp__KhongDichVu.ToString()))
                        {
                            strMessage.Append("\r\n" + String.Format(ResourceMessage.NhomDichVuKhongCoDichVu, strMessageTemp__KhongDichVu.ToString()));
                        }
                        strMessage.Append("\r\n" + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertFontStyle(Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.CacDichVuKhongCoChinhSachGiaHoacKhongCoCauHinhSeKhongDuocChon, Color.Maroon), FontStyle.Italic));
                        WaitingManager.Hide();
                        MessageManager.Show(strMessage.ToString());
                    }
                }
                else
                {
                    services = this.ServiceIsleafADOs;
                    this.toggleSwitchDataChecked.EditValue = false;
                }
                this.gridControlServiceProcess.DataSource = services;
                this.SetDefaultSerServTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetServiceGroupSelected()
        {
            try
            {
                foreach (var item in this.ServiceIsleafADOs)
                {
                    if (item.SERVICE_GROUP_ID_SELECTEDs != null && item.SERVICE_GROUP_ID_SELECTEDs.Count > 0)
                    {
                        item.IsChecked = false;
                        item.SERVICE_GROUP_ID_SELECTEDs = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceGroup_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = this.cboServiceGroup.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                        gridCheckMark.ClearSelection(this.cboServiceGroup.Properties.View);
                    this.cboServiceGroup.EditValue = null;
                    this.cboServiceGroup.Properties.Buttons[1].Visible = false;
                    this.gridControlServiceProcess.DataSource = null;
                    this.selectedSeviceGroups = null;
                    this.ResetServiceGroupSelected();
                    this.toggleSwitchDataChecked.EditValue = false;
                    this.SetEnableButtonControl(this.actionType);
                    this.SetDefaultSerServTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceGroup_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboServiceGroup.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboServiceGroup.EditValue ?? "0").ToString()));
                        if (data != null)
                        {
                            this.cboServiceGroup.Properties.Buttons[1].Visible = true;
                            //this.SelectOneServiceGroupProcess(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceGroup_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboServiceGroup.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboServiceGroup.EditValue ?? "0").ToString()));
                        if (data != null)
                        {
                            this.cboServiceGroup.Properties.Buttons[1].Visible = true;
                            //this.SelectOneServiceGroupProcess(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceGroup_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.SERVICE_GROUP_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceGroup__SelectionChange(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_SERVICE_GROUP> sgSelectedNews = new List<HIS_SERVICE_GROUP>();
                    foreach (MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.SERVICE_GROUP_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.selectedSeviceGroups = new List<HIS_SERVICE_GROUP>();
                    this.selectedSeviceGroups.AddRange(sgSelectedNews);
                    this.SelectOneServiceGroupProcess(this.selectedSeviceGroups);
                }

                this.cboServiceGroup.Text = sb.ToString();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriviousServiceReq_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPriviousServiceReq.Properties.Buttons[1].Visible = false;
                    cboPriviousServiceReq.EditValue = null;
                    gridControlServiceProcess.DataSource = null;
                    foreach (var item in this.ServiceIsleafADOs)
                    {
                        item.IsChecked = false;
                    }
                    toggleSwitchDataChecked.EditValue = false;
                    SetEnableButtonControl(this.actionType);
                    SetDefaultSerServTotalPrice();
                }
                else if (e.Button.Kind == ButtonPredefines.Search)
                {
                    WaitingManager.Show();
                    LogSystem.Info("Begin FillDataToComboPriviousServiceReq");
                    FillDataToComboPriviousServiceReq(this.currentHisTreatment);
                    LogSystem.Info("End FillDataToComboPriviousServiceReq");
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriviousServiceReq_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPriviousServiceReq.EditValue != null && this.currentPreServiceReqs != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6 data = this.currentPreServiceReqs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPriviousServiceReq.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            cboPriviousServiceReq.Properties.Buttons[1].Visible = true;
                            ProcessChoiceServiceReqPrevious(data);
                            btnSave.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriviousServiceReq_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_INTRUCTION_TIME")
                {
                    var item = ((List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6>)cboPriviousServiceReq.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.INTRUCTION_TIME));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriviousServiceReq_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPriviousServiceReq.EditValue != null && this.currentPreServiceReqs != null)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_6 data = this.currentPreServiceReqs.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboPriviousServiceReq.EditValue ?? 0).ToString()));
                        if (data != null)
                        {
                            cboPriviousServiceReq.Properties.Buttons[1].Visible = true;
                            ProcessChoiceServiceReqPrevious(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPriviousServiceReq_Leave(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(cboPriviousServiceReq.Text) && cboPriviousServiceReq.EditValue != null)
                {
                    cboPriviousServiceReq.EditValue = null;
                    cboPriviousServiceReq.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void panelControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        #endregion

        private void txtServiceCode_Search_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.gridViewServiceProcess.BeginDataUpdate();
                if (!String.IsNullOrWhiteSpace(txtServiceCode_Search.Text))
                {
                    var listResult = this.listDatasFix.Where(o => o.SERVICE_CODE_HIDDEN.ToLower().Contains(txtServiceCode_Search.Text.ToLower().Trim())).ToList();
                    this.gridControlServiceProcess.DataSource = listResult;
                }
                else
                {
                    this.gridControlServiceProcess.DataSource = this.listDatasFix;
                }
                this.gridViewServiceProcess.EndDataUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtServiceName_Search_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.gridViewServiceProcess.BeginDataUpdate();
                if (!String.IsNullOrWhiteSpace(txtServiceName_Search.Text))
                {
                    var listResult = this.listDatasFix.Where(o => o.SERVICE_NAME_HIDDEN.ToLower().Contains(txtServiceName_Search.Text.ToLower().Trim())).ToList();
                    this.gridControlServiceProcess.DataSource = listResult;
                }
                else
                {
                    this.gridControlServiceProcess.DataSource = this.listDatasFix;
                }
                this.gridViewServiceProcess.EndDataUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_CalcRowHeight(object sender, RowHeightEventArgs e)
        {
            try
            {
                if (gridViewServiceProcess.IsFilterRow(e.RowHandle))
                {
                    var fontSize = ApplicationFontWorker.GetFontSize();
                    if (fontSize == ApplicationFontConfig.FontSize825)
                    {
                        //txtServiceName_Search.Point = Point(124, 20);
                        //txtServiceCode_Search.Point = Point(31, 20);
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize875)
                    {
                        e.RowHeight = 23;
                        txtServiceName_Search.Location = new Point(124, 22);
                        txtServiceCode_Search.Location = new Point(31, 22);
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize925)
                    {
                        e.RowHeight = 25;
                        txtServiceName_Search.Location = new Point(124, 24);
                        txtServiceCode_Search.Location = new Point(31, 24);
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize975)
                    {
                        e.RowHeight = 27;
                        txtServiceName_Search.Location = new Point(124, 26);
                        txtServiceCode_Search.Location = new Point(31, 26);
                    }
                    else if (fontSize == ApplicationFontConfig.FontSize1025)
                    {
                        e.RowHeight = 29;
                        txtServiceName_Search.Location = new Point(124, 28);
                        txtServiceCode_Search.Location = new Point(31, 28);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceProcess_ColumnWidthChanged(object sender, ColumnEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "TDL_SERVICE_NAME")
                {
                    txtServiceName_Search.Width = e.Column.Width - 2;
                }
                else if (e.Column.FieldName == "TDL_SERVICE_CODE")
                {
                    txtServiceCode_Search.Width = e.Column.Width;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Public method

        #endregion

    }
}
