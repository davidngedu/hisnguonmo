﻿using Inventec.Core;
using HIS.Desktop.Common;
using Inventec.Desktop.Core;
using HIS.Desktop.Plugins.BillTransferAccounting.BillTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Modules;


namespace HIS.Desktop.Plugins.BillTransferAccounting
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "HIS.Desktop.Plugins.BillTransferAccounting",
       "Kết chuyển",
       "Common",
       14,
       "pivot_32x32.png",
       "A",
       Module.MODULE_TYPE_ID__FORM,
       true,
       true
       )]

    public class BillTransferAccountingProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public BillTransferAccountingProcessor()
        {
            param = new CommonParam();
        }
        public BillTransferAccountingProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IBillTransfer behavior = BillTransferFactory.MakeIBillTransfer(param, args);
                result = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Ham tra ve trang thai cua module la enable hay disable
        /// Ghi de gia tri khac theo nghiep vu tung module
        /// </summary>
        /// <returns>true/false</returns>
        public override bool IsEnable()
        {
            return false;
        }
    }
}
