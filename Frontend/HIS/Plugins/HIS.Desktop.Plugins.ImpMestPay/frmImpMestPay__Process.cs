﻿using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImpMestPay
{
    public partial class frmImpMestPay : FormBase
    {
        private void CreateData(ref HIS_IMP_MEST_PAY impMestPay)
        {
            try
            {
                if (impMestPay == null) impMestPay = new HIS_IMP_MEST_PAY();
                impMestPay.IMP_MEST_PROPOSE_ID = impMestProposeId ?? 0;
                if (spinAmount.EditValue != null)
                {
                    impMestPay.AMOUNT = spinAmount.Value;
                }
                if (spinNextAmount.EditValue != null)
                {
                    impMestPay.NEXT_AMOUNT = spinNextAmount.Value;
                }
                if (dtNextPayTime.EditValue != null)
                    impMestPay.NEXT_PAY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtNextPayTime.DateTime);
                if (cboPayForm.EditValue != null)
                {
                    impMestPay.PAY_FORM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboPayForm.EditValue.ToString());
                }
                impMestPay.PAY_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(
dtPayForm.DateTime).Value;
                if (cboPayer.EditValue != null)
                {
                    ACS_USER user = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>()
                        .FirstOrDefault(o => o.LOGINNAME == cboPayer.EditValue.ToString());
                    if (user != null)
                    {
                        impMestPay.PAYER_LOGINNAME = user.LOGINNAME;
                        impMestPay.PAYER_USERNAME = user.USERNAME;
                    }
                }

                if (actionType == PEnum.ACTION_TYPE.UPDATE && impMestPayId.HasValue)
                {
                    impMestPay.ID = impMestPayId.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
