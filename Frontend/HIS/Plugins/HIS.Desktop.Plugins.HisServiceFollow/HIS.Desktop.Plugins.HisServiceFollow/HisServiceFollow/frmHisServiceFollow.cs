using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.HisServiceFollow.Resources;
using HIS.Desktop.Plugins.HisServiceFollow.Validate;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors.Repository;
using System.Text;

namespace HIS.Desktop.Plugins.HisServiceFollow.HisServiceFollow
{
    public partial class frmHisServiceFollow : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        List<HIS_TREATMENT_TYPE> treatmentTypeNameSelecteds;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        private long ServiceId;
        #endregion

        #region Construct
        public frmHisServiceFollow(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void frmHisServiceFollow_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
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
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisServiceFollow.Resources.Lang", typeof(HIS.Desktop.Plugins.HisServiceFollow.HisServiceFollow.frmHisServiceFollow).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.gridColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceId.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColServiceId.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceId.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColServiceId.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFollowId.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColFollowId.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFollowId.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColFollowId.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAmount.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAmount.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColAmount.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsExpend.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColIsExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsExpend.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColIsExpend.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridLookUpEdit2.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.gridLookUpEdit2.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboLoaiKT.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.cboLoaiKT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridLookUpEdit1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.gridLookUpEdit1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboLoaiDV.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.cboLoaiDV.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsExpend.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.chkIsExpend.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAmount.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.lciAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsExpend.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.lciIsExpend.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentType.Text = Inventec.Common.Resource.Get.Value("frmHisServiceFollow.lciTreatmentType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyword.Text = "";
                spAmount.EditValue = null;
                spnAmountCondition.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void FillDataToControlsForm()
        {
            try
            {
                InitLoaiDV();
                InitDV(BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>());
                InitLoaiKT();
                InitKT(BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>());
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        #region Init combo
        private void InitLoaiDV()
        {
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("SERVICE_TYPE_CODE", "", 150, 1));
            columnInfos.Add(new ColumnInfo("SERVICE_TYPE_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_TYPE_NAME", "ID", columnInfos, false, 250);
            ControlEditorLoader.Load(cboLoaiDV, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>(), controlEditorADO);
        }
        private void InitDV(List<V_HIS_SERVICE> data)
        {
            //var ListFormMaterialType = gridviewFormList.DataSource as List<V_HIS_SERVICE_FOLLOW>;
            //List<long> materialTypeIds = ListFormMaterialType.Select(o => o.SERVICE_ID).ToList();
            //data = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => !materialTypeIds.Contains(o.ID)).ToList();
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 150, 1));
            columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 250);
            ControlEditorLoader.Load(gridLookUpEdit1, data, controlEditorADO);
        }
        private void InitLoaiKT()
        {


            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("SERVICE_TYPE_CODE", "", 150, 1));
            columnInfos.Add(new ColumnInfo("SERVICE_TYPE_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_TYPE_NAME", "ID", columnInfos, false, 250);
            ControlEditorLoader.Load(cboLoaiKT, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>(), controlEditorADO);
        }
        private void InitKT(List<V_HIS_SERVICE> data)
        {
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 150, 1));
            columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 250);
            ControlEditorLoader.Load(gridLookUpEdit2, data, controlEditorADO);
        }
        private void InitDienDieuTri()
        {
            InitCheck(cboTreatmentType, SelectionGrid__cboTreatmentType);
            InitCombo(cboTreatmentType, BackendDataWorker.Get<HIS_TREATMENT_TYPE>(), "TREATMENT_TYPE_NAME", "ID");
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__cboTreatmentType(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_TREATMENT_TYPE> sgSelectedNews = new List<HIS_TREATMENT_TYPE>();
                    foreach (MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(rv.TREATMENT_TYPE_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.treatmentTypeNameSelecteds = new List<HIS_TREATMENT_TYPE>();
                    this.treatmentTypeNameSelecteds.AddRange(sgSelectedNews);
                }

                this.cboTreatmentType.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                LoadPaging(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Ham goi api lay du lieu phan trang
        /// </summary>
        /// <param name="param"></param>
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW>> apiResult = null;
                HisServiceFollowViewFilter filter = new HisServiceFollowViewFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW>>(HisRequestUriStore.MOSV_HIS_SERVICE_FOLLOW_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW>)apiResult.Data;
                    if (data != null)
                    {
                        dnNavigation.DataSource = data;
                        gridviewFormList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridviewFormList.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisServiceFollowViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW pData = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1; //+ startPage + ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(createTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            string MODIFY_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(MODIFY_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_EXPEND_STR")
                    {
                        try
                        {
                            ////string IS_CLINICAL = (view.GetRowCellValue(e.ListSourceRowIndex, "IS_CLINICAL") ?? "").ToString();
                            //e.Value = ;
                            e.Value = pData != null && pData.IS_EXPEND == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la khoa lam sang IS_EXPEND_STR", ex);
                        }
                    }
                    gridControlFormList.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    FillDataToControlsForm();
                    ChangedDataRow(rowData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);

                        //Set focus vào control editor đầu tiên
                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dnNavigation_PositionChanged(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW>)[dnNavigation.Position];
                if (this.currentData != null)
                {
                    ChangedDataRow(this.currentData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    SetValueTreatmentType(this.cboTreatmentType, this.treatmentTypeNameSelecteds, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>());
                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW data)
        {
            try
            {
                if (data != null)
                {
                    ServiceId = data.ID;
                    if (data.SERVICE_TYPE_ID == 1)
                        lciTreatmentType.Enabled = false;
                    else
                        lciTreatmentType.Enabled = true;
                    cboLoaiDV.EditValue = data.SERVICE_TYPE_ID;
                    if (cboLoaiDV.EditValue != null)
                        gridLookUpEdit1.EditValue = data.SERVICE_ID;
                    cboLoaiKT.EditValue = data.FOLLOW_TYPE_ID;
                    if (cboLoaiKT.EditValue != null)
                        gridLookUpEdit2.EditValue = data.FOLLOW_ID;
                    spAmount.EditValue = data.AMOUNT;
                    spnAmountCondition.EditValue = data.CONDITIONED_AMOUNT;
                    GridCheckMarksSelection gridCheckMarkTreatmentType = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkTreatmentType.ClearSelection(cboTreatmentType.Properties.View);
                    if (!String.IsNullOrWhiteSpace(data.TREATMENT_TYPE_IDS) && cboTreatmentType.Properties.Tag != null)
                    {
                        ProcessSelectBusiness(data.TREATMENT_TYPE_IDS, gridCheckMarkTreatmentType);
                    }
                    else
                    {
                        cboTreatmentType.EditValue = null;
                        GridCheckMarksSelection gridCheckMarkBusinessCodes = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                        gridCheckMarkBusinessCodes.ClearSelection(cboTreatmentType.Properties.View);
                    }


                    chkIsExpend.Checked = (data.IS_EXPEND == 1 ? true : false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetValueTreatmentType(GridLookUpEdit gridLookUpEdit, List<HIS_TREATMENT_TYPE> listSelect, List<HIS_TREATMENT_TYPE> listAll)
        {
            try
            {
                if (listSelect != null)
                {
                    gridLookUpEdit.Properties.DataSource = listAll;
                    var selectFilter = listAll.Where(o => listSelect.Exists(p => o.ID == p.ID)).ToList();
                    GridCheckMarksSelection gridCheckMark = gridLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.Selection.Clear();
                    gridCheckMark.Selection.AddRange(selectFilter);
                }
                gridLookUpEdit.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ProcessSelectBusiness(string p, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<HIS_TREATMENT_TYPE> ds = cboTreatmentType.Properties.DataSource as List<HIS_TREATMENT_TYPE>;
                string[] arrays = p.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_TREATMENT_TYPE> selects = new List<HIS_TREATMENT_TYPE>();
                    foreach (var item in arrays)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID.ToString() == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetFocusEditor()
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized) return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    treatmentTypeNameSelecteds = new List<HIS_TREATMENT_TYPE>();
                    cboTreatmentType.Text = "";
                    SetValueTreatmentType(this.cboTreatmentType, this.treatmentTypeNameSelecteds, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>());
                    lciTreatmentType.Enabled = true;
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceFollowViewFilter filter = new HisServiceFollowViewFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW>>(HisRequestUriStore.MOSV_HIS_SERVICE_FOLLOW_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Button handler
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSV_HIS_SERVICE_FOLLOW_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                    if (success)
                    {
                        BackendDataWorker.Reset<V_HIS_SERVICE_FOLLOW>();
                        BackendDataWorker.Reset<HIS_SERVICE_FOLLOW>();
                        FillDataToGridControl();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                SetFocusEditor();
                FillDataToControlsForm();
                cboLoaiDV.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                bool vali = true;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                WaitingManager.Show();
                MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW updateDTO = new MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO, ref vali);
                if (vali)
                {
                    if (ActionType == GlobalVariables.ActionAdd)
                    {
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW>(HisRequestUriStore.MOSV_HIS_SERVICE_FOLLOW_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            FillDataToGridControl();
                            ResetFormData();
                            FillDataToControlsForm();
                        }
                        if (resultData == null)
                        {
                            if (gridLookUpEdit1.EditValue == "" || gridLookUpEdit2.EditValue == "")
                            {
                                ValidationSingleControl(gridLookUpEdit2);
                                ValidationSingleControl(gridLookUpEdit1);
                            }
                            else
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu đã tồn tại");
                            }
                            return;
                        }
                    }
                    else
                    {
                        updateDTO.ID = ServiceId;
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW>(HisRequestUriStore.MOSV_HIS_SERVICE_FOLLOW_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            FillDataToGridControl();
                            FillDataToControlsForm();
                        }
                        if (resultData == null)
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu đã tồn tại");
                            return;
                        }
                    }

                    if (success)
                    {
                        BackendDataWorker.Reset<V_HIS_SERVICE_FOLLOW>();
                        BackendDataWorker.Reset<HIS_SERVICE_FOLLOW>();
                        SetFocusEditor();
                    }

                    WaitingManager.Hide();

                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW) is null");
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW currentDTO, ref bool vali)
        {
            try
            {
                if (cboLoaiDV.EditValue != null) currentDTO.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboLoaiDV.EditValue ?? "0").ToString());
                if (gridLookUpEdit1.EditValue != null) currentDTO.SERVICE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((gridLookUpEdit1.EditValue ?? "0").ToString());
                if (cboLoaiKT.EditValue != null) currentDTO.FOLLOW_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboLoaiKT.EditValue ?? "0").ToString());
                if (gridLookUpEdit2.EditValue != null) currentDTO.FOLLOW_ID = Inventec.Common.TypeConvert.Parse.ToInt64((gridLookUpEdit2.EditValue ?? "0").ToString());
                currentDTO.AMOUNT = (long)spAmount.Value;
                if (spnAmountCondition.EditValue != null)
                    currentDTO.CONDITIONED_AMOUNT = (long)spnAmountCondition.Value;
                else
                    currentDTO.CONDITIONED_AMOUNT = null;
                if (spAmount.Value == 0)
                {
                    //MessageManager.Show(String.Format(ResourceMessage.SoLuongPhaiLonHon0));
                    //ValidationSingleAmount(spAmount);
                    vali = false;
                }
                currentDTO.IS_EXPEND = (short)(chkIsExpend.Checked ? 1 : 0);
                currentDTO.IS_ACTIVE = 1;
                if (treatmentTypeNameSelecteds != null && treatmentTypeNameSelecteds.Count > 0)
                {

                    currentDTO.TREATMENT_TYPE_IDS = String.Join(",", treatmentTypeNameSelecteds.Select(o => o.ID).ToList());
                }
                else
                    currentDTO.TREATMENT_TYPE_IDS = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(gridLookUpEdit1);
                ValidationSingleControl(gridLookUpEdit2);
                ValidationSingleAmount(spAmount);
                //ValidationSingleAmount(spnAmountCondition);
                FillDataToControlsForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = String.Format(ResourceMessage.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = String.Format(ResourceMessage.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = String.Format(ResourceMessage.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
                FillDataToControlsForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleAmount(SpinEdit control)
        {
            try
            {
                AmountValidationRule validRule = new AmountValidationRule();
                validRule.spinAmount = control;
                //validRule.ErrorText = String.Format(ResourceMessage.SoLuongPhaiLonHon0);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Tooltip
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #endregion

        #region Public method
        public void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load du lieu
                FillDataToGridControl();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
                //InitTabIndex();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();
                InitDienDieuTri();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Shortcut
        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void cboLoaiDV_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboLoaiDV.EditValue != null)
                    {
                        var load = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == (long)cboLoaiDV.EditValue).ToList();
                        InitDV(load);
                        gridLookUpEdit1.Focus();
                        gridLookUpEdit1.ShowPopup();
                        if (Inventec.Common.TypeConvert.Parse.ToInt64((cboLoaiDV.EditValue ?? "0").ToString()) == 1)
                        {
                            lciTreatmentType.Enabled = false;
                        }
                        else
                            lciTreatmentType.Enabled = true;
                    }
                    else
                    {
                        cboLoaiDV.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiDV_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboLoaiDV.EditValue != null && cboLoaiDV.EditValue != cboLoaiDV.OldEditValue)
                    {
                        //var ListFormMaterialType = gridviewFormList.DataSource as List<V_HIS_SERVICE_FOLLOW>;
                        //List<long> materialTypeIds = ListFormMaterialType.Select(o => o.SERVICE_ID).ToList();
                        //var data = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == (long)cboLoaiDV.EditValue && !materialTypeIds.Contains(o.ID)).ToList();
                        var load = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == (long)cboLoaiDV.EditValue).ToList();
                        InitDV(load);
                        gridLookUpEdit1.Focus();
                        gridLookUpEdit1.ShowPopup();
                        if (Inventec.Common.TypeConvert.Parse.ToInt64((cboLoaiDV.EditValue ?? "0").ToString()) == 1)
                        {
                            lciTreatmentType.Enabled = false;
                        }
                        else
                            lciTreatmentType.Enabled = true;
                    }
                    else
                    {
                        cboLoaiDV.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiKT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboLoaiKT.EditValue != null)
                    {
                        var load = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == (long)cboLoaiKT.EditValue).ToList();
                        InitKT(load);
                        gridLookUpEdit2.Focus();
                        gridLookUpEdit2.ShowPopup();
                    }
                    else
                    {
                        cboLoaiKT.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoaiKT_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboLoaiKT.EditValue != null && cboLoaiKT.EditValue != cboLoaiKT.OldEditValue)
                    {
                        //    CommonParam param = new CommonParam();
                        //    HisServiceViewFilter filter = new HisServiceViewFilter();
                        //    var load = new BackendAdapter(param)
                        //.Get<List<V_HIS_SERVICE>>("api/HisService/Getview", ApiConsumers.MosConsumer, filter, param).Where(o => o.SERVICE_TYPE_ID == (long)cboLoaiKT.EditValue && o.ID != (long)gridLookUpEdit1.EditValue).ToList();
                        var load = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == (long)cboLoaiKT.EditValue).ToList();
                        InitKT(load);
                        gridLookUpEdit2.Focus();
                        gridLookUpEdit2.ShowPopup();
                    }
                    else
                    {
                        cboLoaiKT.ShowPopup();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboTreatmentType.Focus();
                    cboTreatmentType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkIsExpend_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled == false)
                    {
                        btnEdit.Focus();
                    }
                    else
                    {
                        btnAdd.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_SERVICE_FOLLOW data = (V_HIS_SERVICE_FOLLOW)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IS_ACTIVE_FALSE ? btnUnlock : btnLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        if (data.IS_ACTIVE == IS_ACTIVE_TRUE)
                        {
                            e.RepositoryItem = btnGEdit;
                        }
                        else
                            e.RepositoryItem = btnGEdit_Disable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_SERVICE_FOLLOW hisDepertments = new V_HIS_SERVICE_FOLLOW();
            bool notHandler = false;
            try
            {
                V_HIS_SERVICE_FOLLOW dataDepartment = (V_HIS_SERVICE_FOLLOW)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(String.Format(ResourceMessage.KhoaDuLieu), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    V_HIS_SERVICE_FOLLOW data1 = new V_HIS_SERVICE_FOLLOW();
                    data1.ID = dataDepartment.ID;
                    //data1.FOLLOW_TYPE_ID = dataDepartment.FOLLOW_TYPE_ID;
                    //data1.FOLLOW_ID = dataDepartment.FOLLOW_ID;
                    //data1.AMOUNT = dataDepartment.AMOUNT;
                    //data1.SERVICE_ID = dataDepartment.SERVICE_ID;
                    //data1.SERVICE_TYPE_ID = dataDepartment.SERVICE_TYPE_ID;
                    WaitingManager.Show();
                    hisDepertments = new BackendAdapter(param).Post<V_HIS_SERVICE_FOLLOW>("api/HisServiceFollow/ChangeLock", ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (hisDepertments != null)
                    {
                        FillDataToGridControl();
                        BackendDataWorker.Reset<V_HIS_SERVICE_FOLLOW>();
                        BackendDataWorker.Reset<HIS_SERVICE_FOLLOW>();
                    }
                }
                notHandler = true;
                MessageManager.Show(this.ParentForm, param, notHandler);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_SERVICE_FOLLOW hisDepertments = new V_HIS_SERVICE_FOLLOW();
            bool notHandler = false;
            try
            {
                V_HIS_SERVICE_FOLLOW dataDepartment = (V_HIS_SERVICE_FOLLOW)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(String.Format(ResourceMessage.MoKhoaDuLieu), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    V_HIS_SERVICE_FOLLOW data1 = new V_HIS_SERVICE_FOLLOW();
                    data1.ID = dataDepartment.ID;
                    WaitingManager.Show();
                    hisDepertments = new BackendAdapter(param).Post<V_HIS_SERVICE_FOLLOW>("api/HisServiceFollow/ChangeLock", ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (hisDepertments != null) FillDataToGridControl();
                }
                notHandler = true;
                MessageManager.Show(this.ParentForm, param, notHandler);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ucPaging_Load(object sender, EventArgs e)
        {

        }
        private void gridLookUpEdit1_Click(object sender, EventArgs e)
        {
            try
            {
                var load = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == (long)cboLoaiDV.EditValue).ToList();
                InitDV(load);
                //gridLookUpEdit1.Focus();
                gridLookUpEdit1.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit1_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (gridLookUpEdit1.EditValue != null && gridLookUpEdit1.EditValue != gridLookUpEdit1.OldEditValue)
                    {
                        cboLoaiKT.Focus();
                        cboLoaiKT.ShowPopup();
                    }
                    else
                    {
                        gridLookUpEdit1.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (gridLookUpEdit1.EditValue != null)
                    {
                        gridLookUpEdit1.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit2_Click(object sender, EventArgs e)
        {
            try
            {
                var load = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == (long)cboLoaiKT.EditValue && o.ID != (long)gridLookUpEdit1.EditValue).ToList();
                InitKT(load);
                //gridLookUpEdit2.Focus();
                gridLookUpEdit2.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit2_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (gridLookUpEdit2.EditValue != null && gridLookUpEdit2.EditValue != gridLookUpEdit2.OldEditValue)
                    {
                        spAmount.Focus();
                        spAmount.SelectAll();
                    }
                    else
                    {
                        gridLookUpEdit2.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gridLookUpEdit2.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null || gridCheckMark.Selection == null || gridCheckMark.Selection.Count == 0)
                {
                    e.DisplayText = "";
                    return;
                }
                foreach (MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }

                    sb.Append(rv.TREATMENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_Click(object sender, EventArgs e)
        {
            try
            {
                cboTreatmentType.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTreatmentType.EditValue != null && cboTreatmentType.EditValue != cboTreatmentType.OldEditValue)
                    {
                        chkIsExpend.Properties.FullFocusRect = true;
                        chkIsExpend.Focus();
                    }
                    else
                    {
                        gridLookUpEdit2.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
