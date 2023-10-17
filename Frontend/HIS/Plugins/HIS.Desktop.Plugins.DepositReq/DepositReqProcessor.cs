﻿using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.DepositReq.DepositReq;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.LocalStorage.SdaConfig;

namespace HIS.Desktop.Plugins.DepositReq
{
   [ExtensionOf(typeof(DesktopRootExtensionPoint),
           "HIS.Desktop.Plugins.DepositReq",
           "Buồng bệnh",
           "Common",
           14,
           "pivot_32x32.png",
           "A",
           Module.MODULE_TYPE_ID__UC,
           true,
           true
           )
        ]
    public class DepositReqProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public DepositReqProcessor()
        {
            param = new CommonParam();
        }
        public DepositReqProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IDepositReq behavior = DepositReqFactory.MakeIBedRoomPartial(param, args);
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
                if (GlobalVariables.CurrentRoomTypeCode == SdaConfigs.Get<string>(Inventec.Common.LocalStorage.SdaConfig.ConfigKeys.DBCODE__HIS_RS__HIS_ROOM_TYPE__ROOM_TYPE_CODE__STOCK))
                {
                    result = true;
                }
                else
                    result = false;
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
