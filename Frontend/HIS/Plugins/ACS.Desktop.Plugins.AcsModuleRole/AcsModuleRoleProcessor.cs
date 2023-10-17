﻿using ACS.Desktop.Plugins.AcsModuleRole.AcsModuleRole;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Inventec.Desktop.Common.Modules;
using Inventec.Core;

namespace ACS.Desktop.Plugins.AcsModuleRole
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
        "ACS.Desktop.Plugins.AcsModuleRole",
        "Thiết lập kho xuất phòng",
        "Common",
        62,
        "technology_32x32.png",
        "A",
        Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__UC,
        true,
        true)]
    public class AcsModuleRoleProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public AcsModuleRoleProcessor()
        {
            param = new CommonParam();
        }
        public AcsModuleRoleProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        object IDesktopRoot.Run(object[] args)
        {
            object result = null;
            try
            {
                IAcsModuleRole behavior = AcsModuleRoleFactory.MakeIAcsModuleRole(param, args);
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
            return false;
        }
    }
}