﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Message;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;

namespace HIS.Desktop.Plugins.HisMobaImpMestList
{
    public partial class UCHisMobaImpMestList : UserControl
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        long roomId = 0;
        long roomTypeId = 0;
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock;
        int lastRowHandle = -1;
        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;
        private string LoggingName = "";
        string expMestCode = "";
        string treatmentCode = "";
        #endregion

        #region Construct
        public UCHisMobaImpMestList()
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                gridControl.ToolTipController = this.toolTipController;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCHisMobaImpMestList(long roomId, long roomTypeId)
            : this()
        {
            try
            {
                medistock = Base.GlobalStore.HisListMediStocks.FirstOrDefault(o => o.ROOM_ID == roomId && o.ROOM_TYPE_ID == roomTypeId);
                this.roomId = roomId;
                this.roomTypeId = roomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCHisMobaImpMestList(long roomId, long roomTypeId, string expMestCode)
            : this()
        {
            try
            {
                this.roomId = roomId;
                this.roomTypeId = roomTypeId;
                this.expMestCode = expMestCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCHisMobaImpMestList(long roomId, string treatmentCode, long roomTypeId)
            : this()
        {
            try
            {
                this.roomId = roomId;
                this.roomTypeId = roomTypeId;
                this.treatmentCode = treatmentCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisMobaImpMestList_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                FillDataNavStatus();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                if (!String.IsNullOrEmpty(expMestCode))
                {
                    txtExpMestCode.Text = expMestCode;
                    dtCreateTimeFrom.EditValue = null;
                }
                else if (!String.IsNullOrEmpty(treatmentCode))
                {
                    txtTreatmentCode.Text = treatmentCode;
                    dtCreateTimeFrom.EditValue = null;
                }

                //Load du lieu
                FillDataToGrid();

                txtImpCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void LoadKeysFromlanguage()
        {
            try
            {
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //filter
                txtImpCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__TXT_IMP_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__TXT_KEYWORD",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__NAV_BAR_CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.lciCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__CREATE_TIME_FROM",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__CREATE_TIME_TO",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__BTN_REFRESH",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__BTN_SEARCH",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.navBarGroupStatus.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__NAV_BAR_STATUS",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);

                this.txtExpMestCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__TXT_EXP_MEST_CODE__NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__TXT_TREATMENT_CODE__NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);

                //gridView
                this.GcAggrExpMestCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__AGGR_EXP_MEST_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcApprovalLoginName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__APPROVAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcApprovalTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__APPROVAL_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcCreator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__CREATOR",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcDob.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__DOB",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcGenderName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__GENDER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcImpLoginName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__IMP_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcImpMestCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__IMP_MEST_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcImpTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__IMP_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcMediStockCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__MEDI_STOCK_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcMediStockName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__MEDI_STOCK_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcModifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__MIDIFIER",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcPatientCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__PATIENT_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcReqLoginName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__REQ_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcTreatmentCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__TREATMENT_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.GcVirPatientName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__VIR_PATIENT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.STT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GRID_COLUMN__STT",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);

                //grid button
                this.ButtonActualImportDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GR_BUTTON_IMPORT",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.ButtonActualImportEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GR_BUTTON_IMPORT",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.ButtonApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GR_BUTTON_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.ButtonApprovalEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GR_BUTTON_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.ButtonDisApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GR_BUTTON_DIS_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.ButtonDisApprovalEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GR_BUTTON_DIS_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.ButtonDiscardDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GR_BUTTON_DISCARD",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.ButtonDiscardEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GR_BUTTON_DISCARD",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.ButtonReApproval.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GR_BUTTON_RE_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.ButtonReApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GR_BUTTON_RE_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
                this.repositoryItemButtonViewDetail.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_MOBA_IMP_MEST_LIST__GR_BUTTON_VIEW",
                    Resources.ResourceLanguageManager.LanguageUCHisMobaImpMestList,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataNavStatus()
        {
            try
            {
                navBarControlFilter.BeginUpdate();
                int d = 0;
                foreach (var item in Base.GlobalStore.HisImpMestStts)
                {
                    navBarGroupStatus.GroupClientHeight += 25;
                    DevExpress.XtraEditors.CheckEdit checkEdit = new DevExpress.XtraEditors.CheckEdit();
                    layoutControlContainerStatus.Controls.Add(checkEdit);
                    checkEdit.Location = new System.Drawing.Point(50, 2 + (d * 23));
                    checkEdit.Name = item.ID.ToString();
                    checkEdit.Properties.Caption = item.IMP_MEST_STT_NAME;
                    checkEdit.Size = new System.Drawing.Size(150, 19);
                    checkEdit.StyleController = this.layoutControlContainerStatus;
                    checkEdit.TabIndex = 4 + d;
                    checkEdit.EnterMoveNextControl = false;
                    d++;
                }
                navBarControlFilter.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                txtKeyWord.Text = "";
                txtExpMestCode.Text = "";
                txtImpCode.Text = "";
                dtCreateTimeFrom.DateTime = DateTime.Now;
                dtCreateTimeTo.DateTime = DateTime.Now;
                txtImpCode.Focus();
                SetDefaultStatus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultStatus()
        {
            try
            {
                if (layoutControlContainerStatus.Controls.Count > 0)
                {
                    for (int i = 0; i < layoutControlContainerStatus.Controls.Count; i++)
                    {
                        if (layoutControlContainerStatus.Controls[i] is DevExpress.XtraEditors.CheckEdit)
                        {
                            var checkEdit = layoutControlContainerStatus.Controls[i] as DevExpress.XtraEditors.CheckEdit;
                            checkEdit.Checked = false;
                        }
                    }
                }
                navBarGroupStatus.Expanded = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_MOBA_IMP_MEST>> apiResult = null;
                MOS.Filter.HisMobaImpMestViewFilter filter = new MOS.Filter.HisMobaImpMestViewFilter();
                SetFilter(ref filter);
                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MOBA_IMP_MEST>>
                    (ApiConsumer.HisRequestUriStore.HIS_MOBA_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControl.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridView.EndUpdate();
            }
        }

        private void SetFilter(ref MOS.Filter.HisMobaImpMestViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "IMP_MEST_MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                filter.HAS_TREATMENT = true; //tdl_treatment_id # null
                filter.HAS_AGGR = false;

                if (roomId != 0 && String.IsNullOrEmpty(expMestCode) && String.IsNullOrEmpty(treatmentCode))
                {
                    filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                }

                if (!String.IsNullOrEmpty(txtImpCode.Text))
                {
                    string code = txtImpCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtImpCode.Text = code;
                    }
                    filter.IMP_MEST_CODE__EXACT = code;
                }

                if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                {
                    string code = txtExpMestCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtExpMestCode.Text = code;
                    }
                    filter.EXP_MEST_CODE__EXACT = code;
                }

                if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;
                }


                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                SetFilterStatus(ref filter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void SetFilterStatus(ref MOS.Filter.HisMobaImpMestViewFilter filter)
        {
            try
            {
                if (layoutControlContainerStatus.Controls.Count > 0)
                {
                    for (int i = 0; i < layoutControlContainerStatus.Controls.Count; i++)
                    {
                        if (layoutControlContainerStatus.Controls[i] is DevExpress.XtraEditors.CheckEdit)
                        {
                            var checkEdit = layoutControlContainerStatus.Controls[i] as DevExpress.XtraEditors.CheckEdit;
                            if (checkEdit.Checked)
                            {
                                if (filter.IMP_MEST_STT_IDs == null)
                                    filter.IMP_MEST_STT_IDs = new List<long>();
                                filter.IMP_MEST_STT_IDs.Add(Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name));
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

        private string DisplayName(string loginname, string username)
        {
            string value = "";
            try
            {
                if (String.IsNullOrEmpty(loginname) && String.IsNullOrEmpty(username))
                {
                    value = "";
                }
                else if (loginname != "" && username == "")
                {
                    value = loginname;
                }
                else if (loginname == "" && username != "")
                {
                    value = username;
                }
                else if (loginname != "" && username != "")
                {
                    value = string.Format("{0} - {1}", loginname, username);
                }
                return value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return value;
            }
        }

        private void txtImpCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtImpCode.Text))
                    {
                        FillDataToGrid();
                    }
                    else
                    {
                        txtExpMestCode.Focus();
                        txtExpMestCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMestCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                    {
                        FillDataToGrid();
                    }
                    else
                    {
                        txtTreatmentCode.Focus();
                        txtTreatmentCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        FillDataToGrid();
                    }
                    else
                    {
                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtCreateTimeFrom.EditValue != null)
                    {
                        dtCreateTimeTo.Focus();
                        dtCreateTimeTo.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    long statusIdCheckForButtonEdit = long.Parse((gridView.GetRowCellValue(e.RowHandle, "IMP_MEST_STT_ID") ?? "").ToString());
                    long typeIdCheckForButtonEdit = long.Parse((gridView.GetRowCellValue(e.RowHandle, "IMP_MEST_TYPE_ID") ?? "").ToString());
                    long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "MEDI_STOCK_ID") ?? "").ToString());
                    string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString();
                    if (e.Column.FieldName == "APPROVAL_DISPLAY") // duyệt
                    {
                        if (medistock != null && medistock.ID == mediStockId &&
                            (statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__REJECTED ||
                            statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__UNAPPROVED))
                        {
                            e.RepositoryItem = ButtonApprovalEnable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonApprovalDisable;
                        }
                    }
                    else if (e.Column.FieldName == "DISCARD_DISPLAY")//hủy
                    {
                        if ((creator == LoggingName) &&
                            (statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__DRAFT ||
                            statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__REJECTED ||
                            statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__UNAPPROVED))
                        {
                            e.RepositoryItem = ButtonDiscardEnable;
                        }
                        else
                            e.RepositoryItem = ButtonDiscardDisable;
                    }
                    else if (e.Column.FieldName == "IMPORT_DISPLAY")// thực nhập
                    {
                        if (medistock != null && (statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__APPROVED))
                        {
                            if (medistock.ID == mediStockId)
                                e.RepositoryItem = ButtonActualImportEnable;
                            else
                                e.RepositoryItem = ButtonActualImportDisable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonActualImportDisable;
                        }
                    }
                    else if (e.Column.FieldName == "DIS_APPROVAL")// Không duyệt
                    {
                        if (medistock != null && medistock.ID == mediStockId &&
                            (statusIdCheckForButtonEdit != Base.HisImpMestSttCFG.IMP_MEST_STT_ID__DRAFT &&
                            statusIdCheckForButtonEdit != Base.HisImpMestSttCFG.IMP_MEST_STT_ID__REJECTED &&
                            statusIdCheckForButtonEdit != Base.HisImpMestSttCFG.IMP_MEST_STT_ID__IMPORTED))
                        {
                            e.RepositoryItem = ButtonDisApprovalEnable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonDisApprovalDisable;
                        }
                    }
                    else if (e.Column.FieldName == "RE_APPROVAL")// Hủy duyệt
                    {
                        if (medistock != null && medistock.ID == mediStockId &&
                            (statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__APPROVED ||
                            statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__REJECTED))
                        {
                            e.RepositoryItem = ButtonReApproval;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonReApprovalDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_MOBA_IMP_MEST data = (MOS.EFMODEL.DataModels.V_HIS_MOBA_IMP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        long statusIdCheckForButtonEdit = long.Parse((gridView.GetRowCellValue(e.ListSourceRowIndex, "IMP_MEST_STT_ID") ?? "").ToString());
                        long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.ListSourceRowIndex, "MEDI_STOCK_ID") ?? "").ToString());
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "IMP_MEST_STT_ICON")// trạng thái
                        {
                            if (data.IMP_MEST_STT_ID == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__DRAFT) //nhap
                            {
                                e.Value = imageListStatus.Images[0];
                            }
                            else if (data.IMP_MEST_STT_ID == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__UNAPPROVED)//yeu cau
                            {
                                e.Value = imageListStatus.Images[1];
                            }
                            else if (data.IMP_MEST_STT_ID == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__REJECTED)//// tu choi
                            {
                                e.Value = imageListStatus.Images[2];
                            }
                            else if (data.IMP_MEST_STT_ID == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__APPROVED) // 
                            {
                                e.Value = imageListStatus.Images[3];
                            }
                            else if (data.IMP_MEST_STT_ID == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__IMPORTED) // da nhap
                            {
                                e.Value = imageListStatus.Images[4];
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IMP_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IMP_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_LOGINNAME_DISPLAY")
                        {
                            string APPROVAL_LOGINNAME = data.APPROVAL_LOGINNAME;
                            string APPROVAL_USERNAME = data.APPROVAL_USERNAME;
                            e.Value = DisplayName(APPROVAL_LOGINNAME, APPROVAL_USERNAME);
                        }
                        else if (e.Column.FieldName == "IMP_LOGINNAME_DISPLAY")
                        {
                            string IMP_LOGINNAME = data.IMP_LOGINNAME;
                            string IMP_USERNAME = data.IMP_USERNAME;
                            e.Value = DisplayName(IMP_LOGINNAME, IMP_USERNAME);
                        }
                        else if (e.Column.FieldName == "REQ_LOGINNAME_DISPLAY")
                        {
                            string Req_loginName = data.REQ_LOGINNAME;
                            string Req_UserName = data.REQ_USERNAME;
                            e.Value = DisplayName(Req_loginName, Req_UserName);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toolTipController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControl)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "IMP_MEST_STT_NAME")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "IMP_MEST_STT_NAME") ?? "").ToString();
                            }
                            else if (info.Column.FieldName == "IMP_MEST_STT_ICON")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "IMP_MEST_STT_NAME") ?? "").ToString();
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
        #endregion

        #region Public method
        public void Search()
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

        public void Refreshs()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusCode()
        {
            try
            {
                txtImpCode.Focus();
                txtImpCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
