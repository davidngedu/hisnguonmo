﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionDebtCollect.Base
{
    class ResourceLangManager
    {
        internal static ResourceManager LanguageFrmTransactionBill { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageFrmTransactionBill = new ResourceManager("HIS.Desktop.Plugins.TransactionDebtCollect.Resources.Lang", typeof(HIS.Desktop.Plugins.TransactionDebtCollect.frmTransactionDebtCollect).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
