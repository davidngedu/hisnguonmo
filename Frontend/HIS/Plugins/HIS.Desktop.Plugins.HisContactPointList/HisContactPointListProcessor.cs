﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using Inventec.Core;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.LocalStorage.SdaConfig;
using HIS.Desktop.Plugins.HisContactPointList;


namespace HIS.Desktop.Plugins.HisContactPointList
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
           "HIS.Desktop.Plugins.HisContactPointList",
           "Hồ sơ điều trị",
           "Common",
           31,
           "newitem_32x32.png",
           "A",
           Module.MODULE_TYPE_ID__UC,
           true,
           true)
    ]
    public class HisContactPointListProcessor: ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public HisContactPointListProcessor()
        {
            param = new CommonParam();
        }
        public HisContactPointListProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            CommonParam param = new CommonParam();
            object result = null;
            try
            {
                IHisContactPointList behavior = HisContactPointListFactory.MakeIHisContactPointList(param, args);
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
