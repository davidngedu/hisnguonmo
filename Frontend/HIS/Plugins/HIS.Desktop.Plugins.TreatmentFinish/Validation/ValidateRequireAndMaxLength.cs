﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFinish.Validation
{
    class ValidateRequireAndMaxLength : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.BaseControl memoEdit;
        internal int? maxLength;
        internal bool isBHXH;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (memoEdit == null) return valid;
                if (String.IsNullOrEmpty(memoEdit.Text.Trim()))
                {
                    ErrorText = "Trường dữ liệu bắt buộc";
                    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return valid;
                }
                if (!String.IsNullOrEmpty(memoEdit.Text) && Encoding.UTF8.GetByteCount(memoEdit.Text) > maxLength)
                {
                    ErrorText = "Dữ liệu vượt quá độ dài cho phép";
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
