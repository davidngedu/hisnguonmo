﻿using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisQcType
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
                "HIS.Desktop.Plugins.HisQcType",
                "Loại QC",
                "Bussiness",
                4,
                "hoa-don.png",
                "A",
                Module.MODULE_TYPE_ID__FORM,
                true,
                true)
             ]
    public class HisQcTypeProcessor: ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public HisQcTypeProcessor()
        {
            param = new CommonParam();
        }
        public HisQcTypeProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IHisQcType behavior = HisQcTypeFactory.MakeIControl(param, args);
                result = behavior != null ? (object)(behavior.Run()) : null;
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
