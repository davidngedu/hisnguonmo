﻿using Inventec.Core;
using HIS.Desktop.Common;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Common.Modules;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.LocalStorage.SdaConfig;
using SDA.Desktop.Plugins.SdaExecuteSql.SdaExecuteSql;
//using Inventec.Desktop.Plugins.SurgServiceReqExecute.SurgServiceReqExecute;

namespace SDA.Desktop.Plugins.SdaExecuteSql
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "SDA.Desktop.Plugins.SdaExecuteSql",
       "Công cụ cập nhật dữ liệu",
       "Common",
       14,
       "pivot_32x32.png",
       "A",
       Module.MODULE_TYPE_ID__FORM,
       true,
       true
       )
    ]
    public class SdaExecuteSqlProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public SdaExecuteSqlProcessor()
        {
            param = new CommonParam();
        }
        public SdaExecuteSqlProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                ISdaExecuteSql behavior = SdaExecuteSqlFactory.MakeISdaExecuteSql(param, args);
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
