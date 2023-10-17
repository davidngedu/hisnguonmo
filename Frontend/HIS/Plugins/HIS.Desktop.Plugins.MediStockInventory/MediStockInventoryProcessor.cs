﻿using HIS.Desktop.Plugins.MediStockInventory.MediStockInventory;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MediStockInventory
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "HIS.Desktop.Plugins.MediStockInventory",
       "Kiểm kê kho",
       "Common",
       25,
       "mobaImp.jpg",
       "A",
       Module.MODULE_TYPE_ID__FORM,
       true,
       true)
    ]
    public class MediStockInventoryProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public MediStockInventoryProcessor()
        {
            param = new CommonParam();
        }
        public MediStockInventoryProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            CommonParam param = new CommonParam();
            object result = null;
            try
            {
                IMediStockInventory behavior = MediStockInventoryFactory.MakeIMobaImpMestCreate(param, args);
                result = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
