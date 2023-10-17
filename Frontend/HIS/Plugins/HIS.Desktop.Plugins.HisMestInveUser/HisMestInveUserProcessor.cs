﻿using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using Inventec.Desktop.Plugins.HisMestInveUser.Run;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisMestInveUser
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "HIS.Desktop.Plugins.HisMestInveUser",
       "Hội đồng kiểm kê",
       "Common",
       14,
       "pivot_32x32.png",
       "A",
       Module.MODULE_TYPE_ID__FORM,
       true,
       true
       )
    ]
    public class HisMestInveUserProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public HisMestInveUserProcessor()
        {
            param = new CommonParam();
        }
        public HisMestInveUserProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IHisMestInveUser behavior = HisMestInveUserFactory.MakeIHisMestInveUser(param, args);
                result = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public override bool IsEnable()
        {
            bool result = false;
            try
            {
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }

            return result;
        }
    }
}
