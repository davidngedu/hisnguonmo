﻿using HIS.Desktop.LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionDebtCollect.Validtion
{
    class SpinVatBlack : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.SpinEdit spin;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (spin.EditValue != null && spin.Value < 0)
                {
                    this.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuKhongNhanGiaTriAm);
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
