﻿using Inventec.Core;
using HIS.Desktop.Common;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Common.Modules;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.FormAttachedToRoom;

namespace HIS.Desktop.Plugins.FormAttachedToRoom
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "HIS.Desktop.Plugins.FormAttachedToRoom",
       "Mẫu phiếu gắn theo phòng/buồng",
       "Common",
       23,
       "pivot_32x32.png",
       "A",
       Module.MODULE_TYPE_ID__FORM,
       true,
       true
       )
    ]
    public class FormAttachProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public FormAttachProcessor()
        {
            param = new CommonParam();
        }
        public FormAttachProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IFormAttach behavior = FormAttachFactory.MakeIFormAttach(param, args);
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
