﻿using Inventec.Core;
using HIS.Desktop.Common;
using Inventec.Desktop.Core;
using HIS.Desktop.Plugins.DiimRoom.DiimRoom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Modules;

namespace HIS.Desktop.Plugins.DiimRoom
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "HIS.Desktop.Plugins.DiimRoom",
       "Phòng xem ảnh (*)",
       "Common",
       0,
       "chon-phong.png",
       "A",
       Module.MODULE_TYPE_ID__FORM,
       true,
       true)
    ]
    public class DiimRoomProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public DiimRoomProcessor()
        {
            param = new CommonParam();
        }
        public DiimRoomProcessor(CommonParam paramBusiness)          
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IDiimRoom behavior = DiimRoomFactory.MakeIDiimRoom(param, args);
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
            return true;
        }
    }
}
