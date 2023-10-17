﻿using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MetyMaty.Change
{
    class PrepaMatyValidationRule : ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtPrepaMetyCode;
        internal DevExpress.XtraEditors.GridLookUpEdit cboPrepaMaty;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtPrepaMetyCode == null || cboPrepaMaty == null) return valid;

                if (String.IsNullOrWhiteSpace(txtPrepaMetyCode.Text) || cboPrepaMaty.EditValue == null)
                {
                    ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return valid;
                }

                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
    }
}
