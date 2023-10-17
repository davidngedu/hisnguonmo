﻿using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.SdaField;
using SDA.EFMODEL.DataModels;
using Inventec.UC.Paging;
using HIS.Desktop.Plugins.SdaField.SdaField;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.SdaField;
using HIS.Desktop.Plugins.SdaField.Resources;
using HIS.Desktop.LocalStorage.BackendData;
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;

namespace HIS.Desktop.Plugins.SdaField
{
    public partial class frmField : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int ActionType = -1;
        int startPage = -1;
        int positionHandle = -1;

        SdaModuleFieldADO currentEdit;
        PagingGrid pagingGrid;
        SdaModuleFieldADO currentData;
        List<SdaModuleFieldADO> listSSdaModuleFieldADO { get; set; }
        private Inventec.UC.Paging.UcPaging ucPaging;


        public frmField(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;

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
        private void frmField_Load(object sender, EventArgs e)
        {
            try
            {
               // btnEdit1.Enabled = false;
                FillDataToGridControl();

                ValidateForm();
                this.ActionType = GlobalVariables.ActionAdd;//actionedit
                EnabledControlChanged(this.ActionType);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void FillDataToGridControl()
        {
            try
            {
                int numPageSize;
                if (ucpaging1.pagingGrid != null)
                {
                    numPageSize = ucpaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                    //mPageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridSdaModuleField(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucpaging1.Init(FillDataToGridSdaModuleField, param, numPageSize);
                //LoadPaging(new CommonParam(0, numPageSize));
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //Load data lên grid
        private void FillDataToGridSdaModuleField(object data)
        {
            try
            {
                WaitingManager.Show();
                this.start = ((CommonParam)data).Start ?? 0;
                this.limit = ((CommonParam)data).Limit ?? 0;
                CommonParam param = new CommonParam(this.start, this.limit);
                this.listSSdaModuleFieldADO = new List<SdaModuleFieldADO>();
                Inventec.Core.ApiResultObject<List<SDA_MODULE_FIELD>> apiResult = null;
                SdaModuleFieldFilter filter = new SdaModuleFieldFilter();
               
                filter.KEY_WORD = txtFind.Text;
                
                apiResult = new BackendAdapter(param).GetRO<List<SDA_MODULE_FIELD>>("api/SdaModuleField/Get", ApiConsumers.SdaConsumer, filter, param);
                if (apiResult != null)
                {
                    this.listSSdaModuleFieldADO = (from m in ((List<SDA_MODULE_FIELD>)apiResult.Data) select new SdaModuleFieldADO(m)).ToList();
                    gridControl1.DataSource = null;
                    gridControl1.DataSource = this.listSSdaModuleFieldADO;
                    this.rowCount = (this.listSSdaModuleFieldADO == null ? 0 : this.listSSdaModuleFieldADO.Count);
                    this.dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnabledControlChanged(int p)
        {
            try
            {
                btnAdd.Enabled = (p == GlobalVariables.ActionAdd);
                btnEdit1.Enabled = (p == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtField_code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = String.Format(ResourceMessage.ChuaNhapTruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmField()
        {
            InitializeComponent();
            // TODO: Complete member initialization
        }

        //private void LoadPaging(object param)
        //{
        //    try
        //    {
        //        startPage = ((CommonParam)param).Start ?? 0;
        //        int limit = ((CommonParam)param).Limit ?? 0;
        //        CommonParam paramCommon = new CommonParam(startPage, limit);
        //        Inventec.Core.ApiResultObject<List<SDA.EFMODEL.DataModels.SDA_MODULE_FIELD>> apiResult = null;
        //        SdaModuleFieldFilter filter = new SdaModuleFieldFilter();
        //        //SetFilterNavBar(ref filter);
        //        filter.ORDER_DIRECTION = "DESC";
        //        filter.ORDER_FIELD = "MODIFY_TIME";
        //        //dnNavigation.DataSource = null;
        //        gridviewFormList.BeginUpdate();
        //        apiResult = new BackendAdapter(paramCommon).GetRO<List<SDA.EFMODEL.DataModels.SDA_MODULE_FIELD>>("api/sdaModuleField/Get", ApiConsumers.SdaConsumer, filter, paramCommon);
        //        if (apiResult != null)
        //        {
        //            var data = (List<SDA.EFMODEL.DataModels.SDA_MODULE_FIELD>)apiResult.Data;
        //            if (data != null)
        //            {
        //                //dnNavigation.DataSource = data;
        //                gridviewFormList.GridControl.DataSource = data;
        //                rowCount = (data == null ? 0 : data.Count);
        //                dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
        //            }
        //        }
        //        gridviewFormList.EndUpdate();

        //        #region Process has exception
        //        SessionManager.ProcessTokenLost(paramCommon);
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystem.Error(ex);
        //    }
        //}

        private void SaveProcess()
        {
            try
            {
                bool success = false;
                if (!btnEdit1.Enabled && !btnAdd.Enabled)
                {
                    return;
                }
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                SDA_MODULE_FIELD updateDTO = new SDA_MODULE_FIELD();

                if (this.ActionType == GlobalVariables.ActionEdit && this.currentData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SDA_MODULE_FIELD>(updateDTO, currentData);
                }

                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<SDA_MODULE_FIELD>(
                        "api/sdaModuleField/Create", ApiConsumers.SdaConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                        HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();
                    }
                }
                else
                {
                    if (currentData.ID > 0 && currentData.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        updateDTO.ID = currentData.ID;
                        var resultData = new BackendAdapter(param).Post<SDA_MODULE_FIELD>(
                        "api/sdaModuleField/Update", ApiConsumers.SdaConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            UpdateRowDataAfterEdit(resultData);
                            HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu đang bị khóa", "Thông báo");
                        return;
                    }
                }

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }


        //Update dòng dữ liệu thay đổi
        private void UpdateRowDataAfterEdit(SDA_MODULE_FIELD data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(SDA.EFMODEL.DataModels.SDA_MODULE_FIELD) is null");
                var rowData = (SDA_MODULE_FIELD)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SDA_MODULE_FIELD>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //update dữ liệu từ form để lưu
        private void UpdateDTOFromDataForm(ref SDA_MODULE_FIELD updateDTO)
        {
            try
            {
                // updateDTO.VALUE_ALLOW_IN = (string)txtValueAllowIn.EditValue;
                updateDTO.FIELD_CODE = (string)txtField_code.EditValue;
                updateDTO.FIELD_NAME = (string)txtField_name.EditValue;

                //updateDTO.IS_VISIBLE = (int)chk_is_visible.CheckStateChanged;
                //updateDTO.NUM_ORDER = (string)txtedit.EditValue;
                //updateDTO.IS_VISIBLE = (number)txtField_name.EditValue;            
                // updateDTO.Keyword = (string)txtKey.EditValue;
                //updateDTO.DEFAULT_VALUE = (string)txtDefaultValue.EditValue;
                //updateDTO.DESCRIPTION = (string)txtDescription.EditValue;
                //updateDTO.VALUE_TYPE = (string)cboValueType.EditValue;
                //if (txtValueAllowMin.EditValue != null)
                //{
                //    updateDTO.VALUE_ALLOW_MIN = (txtValueAllowMin.EditValue).ToString();
                //}

            }

            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void ResetFormData()
        {
            txtField_code.EditValue = null;
            txtField_name.EditValue = null;
            txtField_code.Focus();
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btndelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (SDA.EFMODEL.DataModels.SDA_MODULE_FIELD)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SdaModuleFieldFilter filter = new SdaModuleFieldFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<SDA_MODULE_FIELD>>(HisRequestUriStore.SDAHIS_FIELD_GET, ApiConsumers.SdaConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.SDAHIS_FIELD_DELETE, ApiConsumers.SdaConsumer, data.ID, param);
                        if (success)
                        {
                            FillDataToGridControl();
                            HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();
                            //currentData = ((List<SDA_MODULE_FIELD>)gridControl1.DataSource).FirstOrDefault();
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


        private void ChangDataRow(SdaModuleFieldADO data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    if (data.IS_ACTIVE == 0)
                    {
                        this.ActionType = -1;
                    }
                    EnabledControlChanged(this.ActionType);
                }
                else
                {
                    ResetFormData();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToEditorControl(SdaModuleFieldADO data)
        {
            try
            {
                txtField_code.EditValue = data.FIELD_CODE;
   
                txtField_name.EditValue = data.FIELD_NAME; ;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
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

        private void btnEdit1_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void SaveProcessGrid()
        {
            try
            {
                gridviewFormList.PostEditor();
                var row = (SdaModuleFieldADO)gridviewFormList.GetFocusedRow();
                if (row != null)
                {
                    bool success = false;
                  
                   // WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    SDA_MODULE_FIELD updateDTO = new SDA_MODULE_FIELD();

                    if (this.ActionType == GlobalVariables.ActionEdit)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<SdaModuleFieldADO>(updateDTO, row);//SDA_MODULE_FIELD
                    }

                    if (row.IS_VISIBLE_STR)
                        updateDTO.IS_VISIBLE = 1;
                    else
                        updateDTO.IS_VISIBLE = null;

                    //UpdateDTOFromDataForm(ref updateDTO);

                    if (row.ID > 0 && row.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        updateDTO.ID = row.ID;
                        var resultData = new BackendAdapter(param).Post<SDA_MODULE_FIELD>(//SDA_MODULE_FIELD
                        "api/SdaModuleField/Update", ApiConsumers.SdaConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            UpdateRowDataAfterEdit(row);//
                            HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();
                            
                        }
                        
                    }

                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu đang bị khóa", "Thông báo");
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void btnRef_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                //FillDataToGridControl();
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
                txtField_code.Text = "";
                txtField_name.Text = "";
                ResetFormData();
                EnableControlChanged(this.ActionType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int p)
        {
            try
            {
                btnEdit1.Enabled = (p == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (p == GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "NUM_ORDER")
                {
                    TextEdit control = ((GridView)sender).ActiveEditor as TextEdit;
                    if (control != null)
                    {
                        SaveProcessGrid();
                    }
                }
                
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex); ;
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
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

        private void Lock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
        }

        private void txtFind_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var data = (SDA.EFMODEL.DataModels.SDA_MODULE_FIELD)gridviewFormList.GetFocusedRow();
                    if (data != null)
                    {
                        ChangedDataRow(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(SDA_MODULE_FIELD data)
        {
            
            //try
            //{
              
            //    if (data != null)
            //    {
                   
            //        this.ActionType = GlobalVariables.ActionEdit;
            //        EnableControlChanged(this.ActionType);

            //        //Disable nút sửa nếu dữ liệu đã bị khóa
            //        btnEdit1.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE);

            //        positionHandle = -1;
            //        Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    var data = (SDA_MODULE_FIELD)gridviewFormList.GetRow(e.ListSourceRowIndex);
                   

                    //SdaModuleFieldADO pData = (SdaModuleFieldADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((data.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        //e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    //else if (e.Column.FieldName == "CREATE_TIME_STR")
                    //{
                    //    try
                    //    {
                    //        string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                    //        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(createTime));

                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                    //    }
                    //}
                    //else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    //{
                    //    try
                    //    {
                    //        string MODIFY_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                    //        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(MODIFY_TIME));

                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                    //    }
                    //}
                    //else if (e.Column.FieldName == "IS_VISIBLE_STR")
                    //{
                    //    try
                    //    {
                    //        e.Value = data != null && data.IS_VISIBLE == 1 ? true : false;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la ngoai dinh suat IS_HEIN_NDS_CHECK", ex);
                    //    }
                    //}

                    //else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    //{
                    //    try
                    //    {
                    //        if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    //            e.Value = "Hoạt động";
                    //        else
                    //            e.Value = "Tạm khóa";
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Error(ex);
                    //    }
                    //}

                    gridControl1.RefreshDataSource();
                    //gridviewFormList.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void chk_is_visible_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionEdit;
                SaveProcessGrid();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtedit_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.currentData = (SdaModuleFieldADO)gridviewFormList.GetFocusedRow();//SDA_MODULE_FIELD
                if (this.currentData != null)
                {
                    //ChangDataRow(this.currentData);
                    WaitingManager.Hide();
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.currentData = (SdaModuleFieldADO)gridviewFormList.GetFocusedRow();//SDA_MODULE_FIELD
                if (this.currentData != null)
                {
                    ChangDataRow(this.currentData);
                    WaitingManager.Hide();
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
