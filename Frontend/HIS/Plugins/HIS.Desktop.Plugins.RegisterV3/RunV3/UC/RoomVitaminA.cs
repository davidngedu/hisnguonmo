﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraLayout.Utils;

namespace HIS.Desktop.Plugins.RegisterV3.RunV3.UC
{
    public partial class RoomVitaminA : UserControl
    {
        List<V_HIS_EXECUTE_ROOM> listExecuteRooms;
        DelegateRegister.DelegateFocusNextUserControl delegateFocusNextUserControl;
        int positionHandleControl = -1;

        public RoomVitaminA(DelegateRegister.DelegateFocusNextUserControl dlg)
        {
            InitializeComponent();

            try
            {
                this.delegateFocusNextUserControl = dlg;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RoomVitaminA_Load(object sender, EventArgs e)
        {

            try
            {
                dtTime.DateTime = DateTime.Now;
                layoutControlItem3.Visibility = LayoutVisibility.Always;
                layoutControlItem9.Visibility = LayoutVisibility.Always;
                layoutControlItem2.Visibility = LayoutVisibility.Never;
                chkOneMonthBorn.Checked = false;
                chkSick.Checked = false;
                chkSuyDinhDuong.Checked = false;

                ValidateGridLookupWithTextEdit(cboRoom, txtRoom, dxValidationProvider1);
                ValidateSingleControl(dtTime, dxValidationProvider1);

                listExecuteRooms = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>();
                this.InitComboCommon(this.cboRoom, listExecuteRooms.Where(o => o.IS_VITAMIN_A == 1 && o.IS_ACTIVE == 1).ToList(), "ID", "EXECUTE_ROOM_NAME", "EXECUTE_ROOM_CODE");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommon(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 1));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "", (displayMemberWidth > 0 ? displayMemberWidth : 250), 2));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 250);
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRoom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtRoom.Text))
                    {
                        cboRoom.EditValue = null;
                        cboRoom.Focus();
                        cboRoom.ShowPopup();
                    }
                    else
                    {
                        List<V_HIS_EXECUTE_ROOM> searchs = null;
                        var listData1 = this.listExecuteRooms.Where(o => o.EXECUTE_ROOM_CODE.ToUpper().Contains(txtRoom.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.EXECUTE_ROOM_CODE.ToUpper() == txtRoom.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtRoom.Text = searchs[0].EXECUTE_ROOM_CODE;
                            cboRoom.EditValue = searchs[0].ID;
                            chkTreEm.Focus();
                        }
                        else
                        {
                            cboRoom.Focus();
                            cboRoom.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboRoom.EditValue != null)
                    {
                        var data = this.listExecuteRooms.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboRoom.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtRoom.Text = data.EXECUTE_ROOM_CODE;
                            txtRoom.Focus();
                            txtRoom.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkKhac_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.delegateFocusNextUserControl != null)
                        this.delegateFocusNextUserControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

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

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
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

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateSingleControl(Control cbo, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(cbo, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkSick_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkSuyDinhDuong.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkOneMonthBorn_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.delegateFocusNextUserControl != null)
                        this.delegateFocusNextUserControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkTreEm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkPhuNuSauSinh.Focus();
                }

                if (e.KeyCode == Keys.Space)
                {
                    chkTreEm_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkPhuNuSauSinh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKhac.Focus();
                }

                if (e.KeyCode == Keys.Space)
                {
                    chkPhuNuSauSinh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkKhac_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkTreEm.Checked)
                    {
                        chkSick.Focus();
                    }
                    if (chkPhuNuSauSinh.Checked)
                    {
                        chkOneMonthBorn.Focus();
                    }
                    if (chkKhac.Checked)
                    {
                        if (this.delegateFocusNextUserControl != null)
                            this.delegateFocusNextUserControl();
                    }
                }

                if (e.KeyCode == Keys.Space)
                {
                    chkKhac_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkTreEm_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if (chkTreEm.Checked)
                {
                    layoutControlItem3.Visibility = LayoutVisibility.Always;
                    layoutControlItem9.Visibility = LayoutVisibility.Always;
                    chkOneMonthBorn.Checked = false;
                    layoutControlItem2.Visibility = LayoutVisibility.Never;
                    chkPhuNuSauSinh.Checked = false;
                    chkKhac.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkPhuNuSauSinh_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPhuNuSauSinh.Checked)
                {
                    layoutControlItem3.Visibility = LayoutVisibility.Never;
                    layoutControlItem9.Visibility = LayoutVisibility.Never;
                    chkSick.Checked = false;
                    chkSuyDinhDuong.Checked = false;
                    layoutControlItem2.Visibility = LayoutVisibility.Always;
                    chkTreEm.Checked = false;
                    chkKhac.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkKhac_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkKhac.Checked)
                {
                    layoutControlItem3.Visibility = LayoutVisibility.Never;
                    layoutControlItem2.Visibility = LayoutVisibility.Never;
                    layoutControlItem9.Visibility = LayoutVisibility.Never;
                    chkOneMonthBorn.Checked = false;
                    chkSick.Checked = false;
                    chkPhuNuSauSinh.Checked = false;
                    chkTreEm.Checked = false;
                    chkSuyDinhDuong.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkTreEm_Click(object sender, EventArgs e)
        {

            try
            {
                chkTreEm.ReadOnly = chkTreEm.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkPhuNuSauSinh_Click(object sender, EventArgs e)
        {

            try
            {
                chkPhuNuSauSinh.ReadOnly = chkPhuNuSauSinh.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkKhac_Click(object sender, EventArgs e)
        {

            try
            {
                chkKhac.ReadOnly = chkKhac.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRoom.Focus();
                    txtRoom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }

        private void chkSuyDinhDuong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.delegateFocusNextUserControl != null)
                        this.delegateFocusNextUserControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
