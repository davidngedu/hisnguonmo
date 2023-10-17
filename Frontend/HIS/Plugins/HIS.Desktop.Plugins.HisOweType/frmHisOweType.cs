﻿using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.Desktop.Plugins.HisOweType
{
    public partial class frmHisOweType : HIS.Desktop.Utility.FormBase
    {
        public frmHisOweType(Inventec.Desktop.Common.Modules.Module moduleData)
		:base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            SetIcon();
            SetCaptionByLanguageKey();
        }

        #region global
        Inventec.Desktop.Common.Modules.Module moduleData;
        MOS.EFMODEL.DataModels.HIS_OWE_TYPE currentData;
        MOS.EFMODEL.DataModels.HIS_OWE_TYPE resultData;
        int positionHandle = -1;
        int ActionType = -1;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        int rowCount;
        int dataTotal;
        DelegateSelectData delegateSelect = null;
        internal long id;
        int startPage;
        int limit;
        #endregion

        #region private first
        private void FillDataToGridControl()
        {
            WaitingManager.Show();
            LoadPaging(new CommonParam(0, (ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize)));

            CommonParam param = new CommonParam();
            param.Limit = rowCount;
            param.Count = dataTotal;
            //ucPaging.Init(loadPaging, param);
            ucPaging.Init(LoadPaging, param, ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize);

            WaitingManager.Hide();
        }

        private void SaveProcess()
        {
            if (!btnEdit.Enabled && !btnAdd.Enabled)
                return;

            positionHandle = -1;
            if (!dxValidationProvider.Validate())
                return;

            CommonParam param = new CommonParam();
            bool success = false;
            HIS_OWE_TYPE updateDTO = new HIS_OWE_TYPE();

            WaitingManager.Show();

            //bool str = txtCode.Text.IsNormalized(NormalizationForm.FormD);
            //if (!str)
            //{
            //	MessageBox.Show("Mã sai định dạng" + "\n" + "Không được nhập có dấu", "Thông báo");
            //	txtCode.Focus();
            //	txtCode.SelectAll();
            //	return;
            //}

            if (this.currentData != null && this.currentData.ID > 0)
            {
                HisOweTypeFilter filter = new HisOweTypeFilter();

                filter.ID = currentData.ID;
                updateDTO = new BackendAdapter(param).Get<List<HIS_OWE_TYPE>>
                  (HisRequestUriStore.HIS_OWE_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }

            updateDTO.OWE_TYPE_CODE = txtCode.Text.Trim();
            updateDTO.OWE_TYPE_NAME = txtName.Text.Trim();

            if (ActionType == GlobalVariables.ActionAdd)
            {
                updateDTO.IS_ACTIVE = IS_ACTIVE_TRUE;
                resultData = new BackendAdapter(param).Post<HIS_OWE_TYPE>
                  (HisRequestUriStore.HIS_OWE_TYPE_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    FillDataToGridControl();
                    ResetFormData();
                }
            }
            else if (updateDTO != null)
            {
                resultData = new BackendAdapter(param).Post<HIS_OWE_TYPE>
                  (HisRequestUriStore.HIS_OWE_TYPE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    FillDataToGridControl();
                }
            }

            WaitingManager.Hide();
            MessageManager.Show(this, param, success);
            txtCode.Focus();
            txtCode.SelectAll();
            SessionManager.ProcessTokenLost(param);
        }

        private void ChangedDataRow()
        {
            currentData = new HIS_OWE_TYPE();
            currentData = (HIS_OWE_TYPE)gridviewFormList.GetFocusedRow();
            if (currentData != null)
            {
                txtCode.Text = currentData.OWE_TYPE_CODE;
                txtName.Text = currentData.OWE_TYPE_NAME;
                //this.ActionType = GlobalVariables.ActionEdit;
                //EnableControlChanged(this.ActionType);

                //btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                  (dxValidationProvider, dxErrorProvider);
            }
            txtCode.Focus();
        }

        private void EnableControlChanged(int action)
        {
            btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
        }

        private void SetDefaultValue()
        {
            txtCode.Focus();
            txtCode.SelectAll();
            txtSearch.Text = "";
        }

        private void ResetFormData()
        {
            try
            {
                if (!lcEditInfo.IsInitialized) return;
                lcEditInfo.BeginUpdate();

                foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditInfo.Items)
                {
                    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                    {
                        DevExpress.XtraEditors.BaseEdit formatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                        formatFrm.ResetText();
                        formatFrm.EditValue = null;
                        txtCode.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                lcEditInfo.EndUpdate();
            }
        }

        private void ValidateForm()
        {
            ValidationControl(txtCode,2);
            ValidationControl(txtName,100);
            WaitingManager.Hide();
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon
                  (System.IO.Path.Combine
                  (LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory,
                  System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region private second
        private void LoadPaging(object param)
        {
            startPage = ((CommonParam)param).Start ?? 0;
            limit = ((CommonParam)param).Limit ?? 0;

            CommonParam paramCommon = new CommonParam(startPage, limit);

            MOS.Filter.HisOweTypeFilter filterSearch = new HisOweTypeFilter();
            filterSearch.KEY_WORD = txtSearch.Text.Trim();

            Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_OWE_TYPE>> apiResult = null;
            apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_OWE_TYPE>>
              (HisRequestUriStore.HIS_OWE_TYPE_GET, ApiConsumers.MosConsumer, filterSearch, paramCommon);
            if (apiResult != null)
            {
                var data = (List<MOS.EFMODEL.DataModels.HIS_OWE_TYPE>)apiResult.Data;
                gridviewFormList.GridControl.DataSource = data;
                rowCount = (data == null ? 0 : data.Count);
                dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
            }
        }

        private void ValidationControl(BaseEdit control,int maxlength)
        {
            Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
            validRule.editor = control;
            validRule.IsRequired = true;
            validRule.maxLength = maxlength;
            validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            validRule.ErrorType = ErrorType.Warning;
            dxValidationProvider.SetValidationRule(control, validRule);
        }
        #endregion

        #region event
        private void frmHisOweType_Load(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
                SetDefaultValue();
                //this.ActionType = GlobalVariables.ActionAdd;
               // EnableControlChanged(this.ActionType);
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_Click(object sender, EventArgs e)
        {
            try
            {
                ChangedDataRow();
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
                if (e.RowHandle >= 0)
                {
                    HIS_OWE_TYPE data = (HIS_OWE_TYPE)gridviewFormList.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "LOCK")
                        e.RepositoryItem = data.IS_ACTIVE == IS_ACTIVE_TRUE ? btnUnlock : btnLock;
                    if (e.Column.FieldName == "DELETE")
                        e.RepositoryItem = data.IS_ACTIVE == IS_ACTIVE_TRUE ? btnDelete : btnUndelete;
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
                HIS_OWE_TYPE data = (HIS_OWE_TYPE)gridviewFormList.GetRow(e.ListSourceRowIndex);
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                if (e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1 + startPage;
                if (e.Column.FieldName == "STATUS")
                    e.Value = data.IS_ACTIVE == IS_ACTIVE_TRUE ? "Hoạt động" : "tạm khóa";
                if (e.Column.FieldName == "CREATE_TIME_STR")
                {
                    string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(createTime));
                }
                if (e.Column.FieldName == "MODIFY_TIME_STR")
                {
                    string mobdifyTime = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(mobdifyTime));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                HIS_OWE_TYPE data = (HIS_OWE_TYPE)gridviewFormList.GetRow(e.RowHandle);
                if (e.Column.FieldName == "STATUS")
                    e.Appearance.ForeColor = data.IS_ACTIVE == IS_ACTIVE_TRUE ? Color.Green : Color.Red;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar > 127)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                HIS_OWE_TYPE resultLock = new HIS_OWE_TYPE();
                bool notHandler = false;

                HIS_OWE_TYPE currentLock = (HIS_OWE_TYPE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong),
                  "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //resultLock.ID = currentLock.ID;
                    WaitingManager.Show();
                    resultLock = new BackendAdapter(param).Post<HIS_OWE_TYPE>
                      (HisRequestUriStore.HIS_OWE_TYPE_CHANGELOCK, ApiConsumers.MosConsumer, currentLock.ID, param);

                    if (resultLock != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }

                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                HIS_OWE_TYPE resultUnlock = new HIS_OWE_TYPE();
                bool notHandler = false;

                HIS_OWE_TYPE currentUnlock = (HIS_OWE_TYPE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong),
                  "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //resultUnlock.ID = currentUnlock.ID;
                    WaitingManager.Show();
                    resultUnlock = new BackendAdapter(param).Post<HIS_OWE_TYPE>(HisRequestUriStore.HIS_OWE_TYPE_CHANGELOCK, ApiConsumers.MosConsumer,
                      currentUnlock.ID, param);

                    if (resultUnlock != null)
                    {
                        notHandler = true;
                        FillDataToGridControl();
                    }

                    MessageManager.Show(this.ParentForm, param, notHandler);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HIS_OWE_TYPE currentDelete = (HIS_OWE_TYPE)gridviewFormList.GetFocusedRow();
                if (currentDelete != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();

                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage
                      (LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong),
                      "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        success = new BackendAdapter(param).Post<bool>
                        (HisRequestUriStore.HIS_OWE_TYPE_DELETE, ApiConsumers.MosConsumer, currentDelete.ID, param);
                        if (success)
                            FillDataToGridControl();
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
               // this.ActionType = GlobalVariables.ActionAdd;
                //EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProvider, dxErrorProvider);
                ResetFormData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch_Click(null, null);
                if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    currentData = (HIS_OWE_TYPE)gridviewFormList.GetFocusedRow();
                    ChangedDataRow();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtName.Focus();
                    txtName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    if (this.ActionType == GlobalVariables.ActionEdit)
                        btnEdit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
    }
}
