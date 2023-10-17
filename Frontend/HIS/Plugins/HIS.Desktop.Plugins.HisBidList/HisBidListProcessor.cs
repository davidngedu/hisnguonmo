﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.HisBidList.HisBidList;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;

namespace HIS.Desktop.Plugins.HisBidList
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "HIS.Desktop.Plugins.HisBidList",
       "Danh sách gói thầu",
       "Common",
       16,
       "thau.png",
       "D",
       Module.MODULE_TYPE_ID__UC,
       true,
       true)
    ]

    public class HisBidListProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public HisBidListProcessor()
        {
            param = new CommonParam();
        }
        public HisBidListProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            CommonParam param = new CommonParam();
            object result = null;
            try
            {
                IHisBidList behavior = HisBidListFactory.MakeIHisBidList(param, args);
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
            bool result = true;
            //try
            //{
            //    if (GlobalVariables.CurrentRoomTypeCode.Contains(SdaConfigs.Get<string>(Inventec.Common.LocalStorage.SdaConfig.ConfigKeys.DBCODE__HIS_RS__HIS_ROOM_TYPE__ROOM_TYPE_CODE__STOCK)))
            //    {
            //        result = true;
            //    }
            //    else
            //        result = false;
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //    result = false;
            //}            
            return result;
        }
    }
}
