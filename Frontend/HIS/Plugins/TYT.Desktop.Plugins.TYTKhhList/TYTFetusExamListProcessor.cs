﻿using DevExpress.Utils;
using DevExpress.XtraEditors;
using Inventec.Core;
using HIS.Desktop.Common;
using Inventec.Desktop.Core;
using Inventec.Desktop.Plugins.TYTKhhList.TYTKhhList;
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

namespace Inventec.Desktop.Plugins.TYTKhhList
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "TYT.Desktop.Plugins.TYTKhhList",
       "Sổ khám thai",
       "Common",
       14,
       "",
       "A",
       Module.MODULE_TYPE_ID__UC,
       true,
       true
       )
    ]
    public class TYTKhhListUpdateProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public TYTKhhListUpdateProcessor()
        {
            param = new CommonParam();
        }
        public TYTKhhListUpdateProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                ITYTKhhList behavior = TYTKhhListFactory.MakeITYTKhhList(param, args);
                result = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Ham tra ve trang thai cua module la enable hay disable
        /// Ghi de gia tri khac theo nghiep vu tung module
        /// </summary>
        /// <returns>true/false</returns>
        public override bool IsEnable()
        {
            bool result = true;
            return result;
        }
    }
}
