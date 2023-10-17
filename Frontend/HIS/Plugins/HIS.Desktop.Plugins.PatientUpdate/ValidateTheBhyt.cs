﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LibraryMessage;
using System.Text.RegularExpressions;
namespace HIS.Desktop.Plugins.PatientUpdate
{
    class ValidateTheBhyt : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit theBhyt;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (theBhyt == null) return valid;
                if (!String.IsNullOrWhiteSpace(theBhyt.Text))
                {
                    string bhytNumber = theBhyt.Text.Trim();
                    if (Inventec.Common.String.CountVi.Count(bhytNumber) != 15)
                    {
                        ErrorText = "Mã BHYT phải nhập đủ 15 kí tự";
                        ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
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
