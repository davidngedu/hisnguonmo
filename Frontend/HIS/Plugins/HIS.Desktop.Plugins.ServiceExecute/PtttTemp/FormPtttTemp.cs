﻿using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute.PtttTemp
{
    public partial class FormPtttTemp : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module Module;
        private HIS_SERE_SERV_PTTT_TEMP TempData;
        private int positionHandle = -1;

        public FormPtttTemp(Inventec.Desktop.Common.Modules.Module _module, HIS_SERE_SERV_PTTT_TEMP tempData)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this.Module = _module;
                this.TempData = tempData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormPtttTemp_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                ValidateForm();

                txtPtttTempCode.Focus();
                txtPtttTempCode.SelectAll();
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
                ControlMaxLengthValidationRule ptttTempCodeValidate = new ControlMaxLengthValidationRule();
                ptttTempCodeValidate.editor = txtPtttTempCode;
                ptttTempCodeValidate.maxLength = 50;
                ptttTempCodeValidate.IsRequired = true;
                ptttTempCodeValidate.ErrorText = string.Format("Trường dữ liệu vượt quá {0} ký tự", "50");
                ptttTempCodeValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtPtttTempCode, ptttTempCodeValidate);

                ControlMaxLengthValidationRule ptttTempNameValidate = new ControlMaxLengthValidationRule();
                ptttTempNameValidate.editor = txtPtttTempName;
                ptttTempNameValidate.maxLength = 500;
                ptttTempNameValidate.IsRequired = true;
                ptttTempNameValidate.ErrorText = string.Format("Trường dữ liệu vượt quá {0} ký tự", "500");
                ptttTempNameValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtPtttTempName, ptttTempNameValidate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPtttTempCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPtttTempName.Focus();
                    txtPtttTempName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPtttTempName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkPublic.Focus();
                    chkPublic.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPublic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkPublicDepartment.Focus();
                    chkPublicDepartment.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPublicDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (TempData == null)
                {
                    MessageBox.Show("Không có dữ liệu mẫu. Vui lòng thử lại");
                    return;
                }

                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                bool success = false;
                CommonParam param = new CommonParam();
                HIS_SERE_SERV_PTTT_TEMP ptttTemp = new HIS_SERE_SERV_PTTT_TEMP();

                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERE_SERV_PTTT_TEMP>(ptttTemp, this.TempData);

                ptttTemp.SERE_SERV_PTTT_TEMP_CODE = txtPtttTempCode.Text.Trim();
                ptttTemp.SERE_SERV_PTTT_TEMP_NAME = txtPtttTempName.Text.Trim();
                ptttTemp.IS_PUBLIC = chkPublic.Checked ? (short?)1 : null;

                if (this.chkPublicDepartment.Checked)
                {
                    var DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.Module.RoomId).DepartmentId;
                    ptttTemp.DEPARTMENT_ID = DepartmentID;
                    ptttTemp.IS_PUBLIC_IN_DEPARTMENT = chkPublicDepartment.Checked ? (short?)1 : null;
                }

                WaitingManager.Show();
                var ptttTempRS = new BackendAdapter(param).Post<HIS_SERE_SERV_PTTT_TEMP>("api/HisSereServPtttTemp/Create", ApiConsumers.MosConsumer, ptttTemp, param);
                WaitingManager.Hide();
                if (ptttTempRS != null)
                {
                    success = true;
                    this.Close();
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
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

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
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

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormPtttTemp
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourceFormPtttTemp = new ResourceManager("HIS.Desktop.Plugins.ServiceExecute.Resources.Lang", typeof(FormPtttTemp).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResourceFormPtttTemp, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.btnSave.Text", Resources.ResourceLanguageManager.LanguageResourceFormPtttTemp, LanguageManager.GetCulture());
                this.chkPublicDepartment.Properties.Caption = Inventec.Common.Resource.Get.Value("FormPtttTemp.chkPublicDepartment.Properties.Caption", Resources.ResourceLanguageManager.LanguageResourceFormPtttTemp, LanguageManager.GetCulture());
                this.chkPublic.Properties.Caption = Inventec.Common.Resource.Get.Value("FormPtttTemp.chkPublic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResourceFormPtttTemp, LanguageManager.GetCulture());
                this.lciPtttTempCode.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.lciPtttTempCode.Text", Resources.ResourceLanguageManager.LanguageResourceFormPtttTemp, LanguageManager.GetCulture());
                this.lciPtttTempName.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.lciPtttTempName.Text", Resources.ResourceLanguageManager.LanguageResourceFormPtttTemp, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResourceFormPtttTemp, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.bar1.Text", Resources.ResourceLanguageManager.LanguageResourceFormPtttTemp, LanguageManager.GetCulture());
                this.barBtnSave.Caption = Inventec.Common.Resource.Get.Value("FormPtttTemp.barBtnSave.Caption", Resources.ResourceLanguageManager.LanguageResourceFormPtttTemp, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormPtttTemp.Text", Resources.ResourceLanguageManager.LanguageResourceFormPtttTemp, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                positionHandle = 0;
                TempData = null;
                Module = null;
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.chkPublicDepartment.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.chkPublicDepartment_KeyDown);
                this.chkPublic.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.chkPublic_PreviewKeyDown);
                this.txtPtttTempName.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPtttTempName_PreviewKeyDown);
                this.txtPtttTempCode.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPtttTempCode_PreviewKeyDown);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.barBtnSave.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSave_ItemClick);
                this.Load -= new System.EventHandler(this.FormPtttTemp_Load);
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barBtnSave = null;
                bar1 = null;
                barManager1 = null;
                dxValidationProvider1 = null;
                layoutControlItem5 = null;
                emptySpaceItem1 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                lciPtttTempName = null;
                txtPtttTempName = null;
                chkPublic = null;
                chkPublicDepartment = null;
                btnSave = null;
                lciPtttTempCode = null;
                txtPtttTempCode = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
