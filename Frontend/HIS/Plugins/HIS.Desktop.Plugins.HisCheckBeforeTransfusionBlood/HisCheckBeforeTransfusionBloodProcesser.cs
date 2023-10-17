﻿using Inventec.Core;
using Inventec.Desktop.Common;
using Inventec.Desktop.Core;
using Inventec.Desktop.Common.Modules;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood;


namespace HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
     "HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood",
     "Danh mục",
     "Bussiness",
     4,
     "showproduct_32x32.png",
     "A",
     Module.MODULE_TYPE_ID__FORM,
     true,
     true)
  ]
    public class HisCheckBeforeTransfusionBloodProcessor : ModuleBase, IDesktopRoot
    {

        CommonParam param;
        public HisCheckBeforeTransfusionBloodProcessor()
        {
            param = new CommonParam();
        }
        public HisCheckBeforeTransfusionBloodProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                DelegateSelectData delegateSelect = null;
                long? expMestId = null;

                if (args.GetType() == typeof(object[]))
                {
                    if (args != null && args.Count() > 0)
                    {
                        for (int i = 0; i < args.Count(); i++)
                        {
                            if (args[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)args[i];
                            }
                            else if (args[i] is DelegateSelectData)
                            {
                                delegateSelect = (DelegateSelectData)args[i];
                            }
                            else if (args[i] is long)
                            {
                                expMestId = (long)args[i];
                            }
                           
                        }
                    }
                }

                if (delegateSelect == null)
                {
                    if (expMestId == null)
                    {
                        result = new HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.frmHisCheckBeforeTransfusionBlood(moduleData);
                    }
                    else
                    {
                        result = new HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.frmHisCheckBeforeTransfusionBlood(moduleData, expMestId);
                    }
                }
                else
                {
                    if (expMestId == null)
                    {
                        result = new HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.frmHisCheckBeforeTransfusionBlood(moduleData, delegateSelect);
                    }
                    else
                    {
                        result = new HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.frmHisCheckBeforeTransfusionBlood(moduleData, delegateSelect, expMestId);
                    }
                }
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
