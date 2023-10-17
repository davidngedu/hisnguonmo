﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImpMestCreate.Validation
{
    class TxtSeriNumberValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.ButtonEdit txtText;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool vaild = false;
            try
            {
                if (txtText == null) return vaild;
                if (txtText.Enabled)
                {
                    if (string.IsNullOrEmpty(txtText.Text)) return vaild;
                    if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtText.Text, 50))
                    {
                        this.ErrorText = "Trường dữ liệu vượt quá maxlength( " + 50 + " kí tự)";
                        return vaild;
                    }
                }
                vaild = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return vaild;
        }
    }
}
