﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Desktop.Common.Controls.ValidationRule;

namespace HIS.Desktop.Plugins.PatientInfo
{
    public class txtDOBValidate : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {

        internal DevExpress.XtraEditors.TextEdit txtdob;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtdob == null) return valid;
                if (string.IsNullOrEmpty(txtdob.Text)) return valid;
                string strDob = "";
                if (txtdob.Text.Length == 2 || txtdob.Text.Length == 1)
                {
                    int patientDob = Int32.Parse(txtdob.Text);
                    if (patientDob < 7)
                    {
                        return valid;
                    }
                    else
                    {
                        txtdob.Text = (DateTime.Now.Year - patientDob).ToString();
                    }
                }
                else if (txtdob.Text.Length == 4)
                {
                    if (Inventec.Common.TypeConvert.Parse.ToInt64(txtdob.Text) <= DateTime.Now.Year)
                    {
                        strDob = "01/01/" + txtdob.Text;
                    }
                    else
                    {
                        return valid;
                    }
                }
                else if (txtdob.Text.Length < 4)
                {
                    return valid;
                }
                else if (txtdob.Text.Length == 8)
                {
                    strDob = txtdob.Text.Substring(0, 2) + "/" + txtdob.Text.Substring(2, 2) + "/" + txtdob.Text.Substring(4, 4);
                    if (HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(strDob).Value.Date <= DateTime.Now.Date)
                    {
                        txtdob.Text = strDob;
                    }
                    else
                    {
                        txtdob.Text = strDob;
                        return valid;
                    }
                }
                else if (txtdob.Text.Length > 4 && txtdob.Text.Length < 8)
                {
                    return valid;
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
