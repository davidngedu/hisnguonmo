﻿using HIS.Desktop.LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PeriodList.Validation
{
    class PeriodMonthValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal System.Windows.Forms.ComboBox cboMonth;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cboMonth == null) return valid;
                if (cboMonth.SelectedIndex < 0)
                {
                    ErrorText = MessageUtil.GetMessage(Message.Enum.TruongDuLieuBatBuoc);
                    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return valid;
                }
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
