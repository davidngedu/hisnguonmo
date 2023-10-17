﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.Plugins.TreatmentFinish.Config;
using System.Drawing;

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    public partial class FormTreatmentFinish : HIS.Desktop.Utility.FormBase
    {
        private void ValidateForm()
        {
            ValidationTimeFinish();
            if (lciOutPatientDateFrom.Enabled == true
                && lciOutPatientDateTo.Enabled == true)
            {
                ValidationOutPatientDateFrom(ref dxValidationProvider);
                ValidationOutPatientDateTo(ref dxValidationProvider);
                ValidationOutPatientDateFrom(ref dxValidationProvider_ForOutPatientDateFromTo);
                ValidationOutPatientDateTo(ref dxValidationProvider_ForOutPatientDateFromTo);
            }

            ValidationFinishType();
            ValidationResult();
            ValidationMaxLength(txtMethod, 3000);
            ValidationMaxLength(txtAdvised, 500);

            if (ConfigKey.PathologicalProcessOption == "1" && currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)
            {
                layoutControlItem25.AppearanceItemCaption.ForeColor = Color.Maroon;
                ValidationMaxLengthAndRequire(txtDauHieuLamSang, 3000);
            }
            else if (ConfigKey.PathologicalProcessOption == "2" && (currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY))
            {
                layoutControlItem25.AppearanceItemCaption.ForeColor = Color.Maroon;
                ValidationMaxLengthAndRequire(txtDauHieuLamSang, 3000);
            }
            else
            {
                ValidationMaxLength(txtDauHieuLamSang, 3000);
            }
            if ((Config.ConfigKey.SubclinicalResultOption == "2" && (currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU ||
    currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY)) || (Config.ConfigKey.SubclinicalResultOption == "1" && (currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU ||
    currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU)))
            {
                lblKetQuaXetNghiem.Appearance.ForeColor = System.Drawing.Color.Maroon;
                ValidationMaxLengthAndRequire(txtKetQuaXetNghiem, 3000);
            }
            else
            {
                ValidationMaxLength(txtKetQuaXetNghiem, 3000);
            }
            ValidationMaxLength(txtSurgery, 3000);
            ValidationMaxLength(txtMaBHXH, 10, true);
            ValidationComboProgram();
        }

        private void ValidationOutPatientDateFrom(ref DXValidationProvider validationProvider)
        {
            try
            {
                Validation.OutPatientDateValidationRule outPatientDateRule = new Validation.OutPatientDateValidationRule();
                outPatientDateRule.dateEditForValidation = dtOutPatientDateFrom;
                outPatientDateRule.dateEditRequired = dtOutPatientDateTo;
                outPatientDateRule.ErrorText = "Bắt buộc nhập thông tin 'Ngoại trú từ'";
                outPatientDateRule.ErrorType = ErrorType.Warning;
                validationProvider.SetValidationRule(dtOutPatientDateFrom, outPatientDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationOutPatientDateTo(ref DXValidationProvider validationProvider)
        {
            try
            {
                Validation.OutPatientDateValidationRule outPatientDateRule = new Validation.OutPatientDateValidationRule();
                outPatientDateRule.dateEditForValidation = dtOutPatientDateTo;
                outPatientDateRule.dateEditRequired = dtOutPatientDateFrom;
                outPatientDateRule.ErrorText = "Bắt buộc nhập thông tin 'Ngoại trú đến'";
                outPatientDateRule.ErrorType = ErrorType.Warning;
                validationProvider.SetValidationRule(dtOutPatientDateTo, outPatientDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationEndOrder()
        {
            try
            {
                Validation.EndOrderValidationRule endOrderRule = new Validation.EndOrderValidationRule();
                endOrderRule.txtEndOrder = txtEndOrder;
                endOrderRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(txtEndOrder, endOrderRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMaxLength(BaseEdit memoEdit, int? maxLength, bool isBHXH = false)
        {
            try
            {
                Validation.ValidateMaxLength maxLengthValid = new Validation.ValidateMaxLength();
                maxLengthValid.memoEdit = memoEdit;
                maxLengthValid.maxLength = maxLength;
                maxLengthValid.isBHXH = isBHXH;
                this.dxValidationProvider.SetValidationRule(memoEdit, maxLengthValid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMaxLengthAndRequire(BaseEdit memoEdit, int? maxLength)
        {
            try
            {
                Validation.ValidateRequireAndMaxLength maxLengthValid = new Validation.ValidateRequireAndMaxLength();
                maxLengthValid.memoEdit = memoEdit;
                maxLengthValid.maxLength = maxLength;
                this.dxValidationProvider.SetValidationRule(memoEdit, maxLengthValid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationTimeFinish()
        {
            try
            {
                Validation.TimeFinishValidationRule timeFinishRule = new Validation.TimeFinishValidationRule();
                timeFinishRule.dtThoiGianKetThuc = dtEndTime;
                timeFinishRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                timeFinishRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(dtEndTime, timeFinishRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationFinishType()
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule finishTypeRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule();
                finishTypeRule.editor = cboTreatmentEndType;
                finishTypeRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                finishTypeRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(cboTreatmentEndType, finishTypeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationResult()
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule resultRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule();
                resultRule.editor = cboResult;
                resultRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                resultRule.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(cboResult, resultRule);
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
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
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
    }
}
