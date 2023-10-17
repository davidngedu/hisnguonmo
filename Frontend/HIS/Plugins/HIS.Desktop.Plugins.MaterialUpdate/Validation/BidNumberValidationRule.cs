﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MaterialUpdate.Validation
{
    class BidNumberValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtBidNumber;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtBidNumber == null) return valid;

                if (!string.IsNullOrEmpty(txtBidNumber.Text))
                {
                    if (txtBidNumber.Text.Contains(" "))
                    {
                        this.ErrorText = "Trường dữ liệu không được tồn tại khoảng trắng";
                        return valid;
                    }
                }
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
