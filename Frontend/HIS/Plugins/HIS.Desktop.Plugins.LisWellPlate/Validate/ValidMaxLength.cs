﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.LisWellPlate.Validate
{
    class ValidMaxLength : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        
        internal DevExpress.XtraEditors.TextEdit txtEdit;
        internal int maxlength;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool vaild = false;
            try
            {            
                if (maxlength > 0 && txtEdit != null && !string.IsNullOrEmpty(txtEdit.Text) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtEdit.Text, maxlength))
                {
                    this.ErrorText = "Trường dữ liệu vượt quá maxlength( " + maxlength + " kí tự)";
                    return vaild;
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
