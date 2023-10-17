﻿using ACS.SDO;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Validate.ValidateRule;
using HIS.UC.DateEditor;
using HIS.UC.Icd.ADO;
using HIS.UC.PatientSelect;
using HIS.UC.PeriousExpMestList;
using HIS.UC.SecondaryIcd;
using HIS.UC.TreatmentFinish;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.CustomControl;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void LoadRequiredCause(bool isRequired)
        {
            try
            {
                ValidationICDCause(10, 500, isRequired);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal object UcIcdCauseGetValue()
        {
            object result = null;
            try
            {
                IcdInputADO outPut = new IcdInputADO();
                if (txtIcdCodeCause.ErrorText == "")
                {
                    if (chkEditIcdCause.Checked)
                        outPut.ICD_NAME = txtIcdMainTextCause.Text;
                    else
                        outPut.ICD_NAME = cboIcdsCause.Text;

                    if (!String.IsNullOrEmpty(txtIcdCodeCause.Text))
                    {
                        outPut.ICD_CODE = txtIcdCodeCause.Text;
                    }
                }
                else
                    outPut = null;
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ValidationICDCause(int? maxLengthCode, int? maxLengthText, bool isRequired)
        {
            try
            {
                if (isRequired)
                {
                    lciIcdTextCause.AppearanceItemCaption.ForeColor = Color.Maroon;

                    IcdValidationRuleControl icdMainRule = new IcdValidationRuleControl();
                    icdMainRule.txtIcdCode = txtIcdCodeCause;
                    icdMainRule.btnBenhChinh = cboIcdsCause;
                    icdMainRule.txtMainText = txtIcdMainTextCause;
                    icdMainRule.chkCheck = chkEditIcdCause;
                    icdMainRule.maxLengthCode = maxLengthCode;
                    icdMainRule.maxLengthText = maxLengthText;
                    icdMainRule.IsObligatoryTranferMediOrg = this.IsObligatoryTranferMediOrg;
                    icdMainRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    icdMainRule.ErrorType = ErrorType.Warning;
                    dxValidationProviderControl.SetValidationRule(txtIcdCodeCause, icdMainRule);
                }
                else
                {
                    lciIcdTextCause.AppearanceItemCaption.ForeColor = new System.Drawing.Color();
                    txtIcdCodeCause.ErrorText = "";
                    dxValidationProviderControl.RemoveControlError(txtIcdCodeCause);
                    dxValidationProviderControl.SetValidationRule(txtIcdCodeCause, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetValidationICDCause()
        {
            try
            {
                lciIcdTextCause.AppearanceItemCaption.ForeColor = Color.Black;
                dxValidationProviderControl.SetValidationRule(txtIcdCodeCause, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCIcdCauseInit()
        {
            try
            {
                DataToComboChuanDoanTD(cboIcdsCause, this.currentIcds.Where(o => o.IS_CAUSE == 1)
                    .OrderBy(p => p.ICD_CODE).ToList());
                chkEditIcdCause.Enabled = (HisConfigCFG.AutoCheckIcd != "2");
                //txtIcdCode.Focus();
                //txtIcdCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadIcdCauseToControl(string icdCode, string icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(icdCode))
                {
                    var icd = this.currentIcds.Where(p => p.ICD_CODE == (icdCode)).FirstOrDefault();
                    if (icd != null)
                    {
                        txtIcdCodeCause.Text = icd.ICD_CODE;
                        cboIcdsCause.EditValue = icd.ID;
                        if ((isAutoCheckIcd) || (!String.IsNullOrEmpty(icdName) && (icdName ?? "").Trim().ToLower() != (icd.ICD_NAME ?? "").Trim().ToLower()))
                        {
                            chkEditIcdCause.Checked = (HisConfigCFG.AutoCheckIcd != "2");
                            txtIcdMainTextCause.Text = icdName;
                        }
                        else
                        {
                            chkEditIcdCause.Checked = false;
                            txtIcdMainTextCause.Text = icd.ICD_NAME;
                        }
                    }
                    else
                    {
                        txtIcdCodeCause.Text = null;
                        cboIcdsCause.EditValue = null;
                        txtIcdMainTextCause.Text = null;
                        chkEditIcdCause.Checked = false;
                    }
                }
                else if (!string.IsNullOrEmpty(icdName))
                {
                    chkEditIcdCause.Checked = (HisConfigCFG.AutoCheckIcd != "2");
                    txtIcdMainTextCause.Text = icdName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UcIcdCauseFocusComtrol()
        {
            try
            {
                txtIcdCodeCause.Focus();
                txtIcdCodeCause.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
