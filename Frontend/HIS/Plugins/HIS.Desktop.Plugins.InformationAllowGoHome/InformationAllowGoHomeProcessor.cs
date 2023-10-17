﻿using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InformationAllowGoHome.InformationAllowGoHome
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
        "HIS.Desktop.Plugins.InformationAllowGoHome",
        "Phiếu tóm tắt thông tin bệnh nặng xin về",
        "Common",
        62,
        "newitem_32x32.png",
        "A",
        Module.MODULE_TYPE_ID__FORM,
        true,
        true)]
    public class InformationAllowGoHomeProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public InformationAllowGoHomeProcessor()
        {
            param = new CommonParam();
        }

        public InformationAllowGoHomeProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        object IDesktopRoot.Run(object[] args)
        {
            object result = null;
            try
            {
                IInformationAllowGoHome behavior = InformationAllowGoHomeFactory.MakeIInformationAllowGoHome(param, args);
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
