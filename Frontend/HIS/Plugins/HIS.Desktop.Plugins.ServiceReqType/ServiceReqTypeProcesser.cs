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
using HIS.Desktop.Plugins.ServiceReqType;


namespace HIS.Desktop.Plugins.ServiceReqType
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
     "HIS.Desktop.Plugins.ServiceReqType",
     "Danh mục",
     "Bussiness",
     4,
     "showproduct_32x32.png",
     "A",
     Module.MODULE_TYPE_ID__FORM,
     true,
     true)
  ]
    public class ServiceReqTypeProcessor : ModuleBase, IDesktopRoot
    {

        CommonParam param;
        public ServiceReqTypeProcessor()
        {
            param = new CommonParam();
        }
        public ServiceReqTypeProcessor(CommonParam paramBusiness)
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
                            if (args[i] is DelegateSelectData)                            {
                                delegateSelect = (DelegateSelectData)args[i];
                            }
                           
                        }
                    }
                }

                if (delegateSelect == null)
                    result = new HIS.Desktop.Plugins.ServiceReqType.ServiceReqTypeForm(moduleData);
                else
                    result = new HIS.Desktop.Plugins.ServiceReqType.ServiceReqTypeForm(moduleData, delegateSelect);
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
