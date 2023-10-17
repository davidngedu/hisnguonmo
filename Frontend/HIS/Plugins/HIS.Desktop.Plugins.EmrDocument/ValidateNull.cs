﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.Desktop.Plugins.EmrDocument
{
    class ValidateNull : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.MemoEdit textEdit;
        internal int? maxLength;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (string.IsNullOrEmpty(textEdit.Text) || string.IsNullOrWhiteSpace(textEdit.Text))
                {
                    this.ErrorText = "Trường dữ liệu bắt buộc";
                    this.ErrorType = ErrorType.Warning;
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
