﻿using HIS.Desktop.LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RationSchedule.Validtion
{
    class TimeToValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dtFrom;
        internal DevExpress.XtraEditors.DateEdit dtTo;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (dtFrom == null || dtTo == null)
                    return valid;
                if(dtTo.EditValue != null && dtFrom.EditValue != null && dtFrom.DateTime >= dtTo.DateTime)
                {
                    ErrorText = "Thời gian đến không được nhỏ hơn hoặc bằng thời gian từ";
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
