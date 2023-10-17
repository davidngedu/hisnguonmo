﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.AnticipateCreate.Validation;

namespace HIS.Desktop.Plugins.AnticipateCreate
{
    public partial class UCAnticipateCreate : HIS.Desktop.Utility.UserControlBase
    {
        private void ValidControls()
        {
            ValidImpPrice();
            ValidAmount();
            ValiCoefficient();
        }

        private void ValiCoefficient()
        {
            try
            {
                CoefficientValidationRule CoefficientValidationRule = new CoefficientValidationRule();
                CoefficientValidationRule.SpinCoefficient = SpinCoefficient;
                CoefficientValidationRule.ErrorText = Resources.ResourceMessage.HeSoKhongDuocAm;
                CoefficientValidationRule.ErrorType = ErrorType.Warning;
                dxValidationProviderRightPanel.SetValidationRule(SpinCoefficient, CoefficientValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void ValidAmount()
        {
            try
            {
                AmountValidationRule amountValidationRule = new AmountValidationRule();
                amountValidationRule.spinAmount = spinAmount;
                amountValidationRule.ErrorText = Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc;
                amountValidationRule.ErrorType = ErrorType.Warning;
                dxValidationProviderLeftPanel.SetValidationRule(spinAmount, amountValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidImpPrice()
        {
            try
            {
                ImpPriceValidationRule impPriceValidationRule = new ImpPriceValidationRule();
                impPriceValidationRule.spinImpPrice = spinImpPrice;
                impPriceValidationRule.ErrorText = Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc;
                impPriceValidationRule.ErrorType = ErrorType.Warning;
                dxValidationProviderLeftPanel.SetValidationRule(spinImpPrice, impPriceValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAmount_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                if ((int)spinAmount.Value < 0)
                {
                    spinAmount.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAmount_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (spinAmount.EditValue != null && (spinAmount.Value < 0 || spinAmount.Value > 1000000000000000000)) e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinImpPrice_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                if ((int)spinImpPrice.Value < 0)
                {
                    spinImpPrice.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinImpPrice_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (spinImpPrice.EditValue != null && (spinImpPrice.Value < 0 || spinImpPrice.Value > 1000000000000000000)) e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
