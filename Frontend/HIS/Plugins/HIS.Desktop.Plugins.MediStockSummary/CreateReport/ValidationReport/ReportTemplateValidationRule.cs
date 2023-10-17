﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MediStockSummary.CreateReport.ValidationReport
{
    class ReportTemplateValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtReportTemplateCode;
        internal DevExpress.XtraEditors.LookUpEdit cboReportTemplate;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtReportTemplateCode == null || cboReportTemplate == null) return valid;
                if (string.IsNullOrEmpty(txtReportTemplateCode.Text) || string.IsNullOrEmpty(cboReportTemplate.Text)) return valid;
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
