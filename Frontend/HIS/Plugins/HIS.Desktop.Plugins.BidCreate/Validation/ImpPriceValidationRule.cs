﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.BidCreate.Validation
{
    class ImpPriceValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.SpinEdit spinImpPrice;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (spinImpPrice == null) return valid;
                if (string.IsNullOrEmpty(spinImpPrice.Text) || spinImpPrice.Value < 0) return valid;
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
    }
}
