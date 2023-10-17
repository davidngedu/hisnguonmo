﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.Desktop.Plugins.HisServiceReReInDiffDay.Validtion
{
    class ValidationTime : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TimeSpanEdit tmIntructionTimeFrom;
        internal DevExpress.XtraEditors.TimeSpanEdit tmIntructionTimeTo;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (tmIntructionTimeFrom == null || tmIntructionTimeFrom == null) return valid;

                tmIntructionTimeFrom.DeselectAll();
                tmIntructionTimeTo.DeselectAll();

                if (tmIntructionTimeFrom.EditValue == null || tmIntructionTimeTo.EditValue == null)
                {
                    valid = true;
                    return valid;
                }

                if (tmIntructionTimeFrom.EditValue != null && tmIntructionTimeTo.EditValue != null)
                {
                    if (tmIntructionTimeFrom.TimeSpan.Hours > tmIntructionTimeTo.TimeSpan.Hours)
                    {
                        this.ErrorText = "Giờ chỉ định từ bé hơn hoặc bằng giờ chỉ định đến";
                        this.ErrorType = ErrorType.Warning;
                        return valid;
                    }
                    else if (tmIntructionTimeFrom.TimeSpan.Hours == tmIntructionTimeTo.TimeSpan.Hours && tmIntructionTimeFrom.TimeSpan.Minutes > tmIntructionTimeTo.TimeSpan.Minutes)
                    {
                        this.ErrorText = "Giờ chỉ định từ bé hơn hoặc bằng giờ chỉ định đến";
                        this.ErrorType = ErrorType.Warning;
                        return valid;
                    }
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
