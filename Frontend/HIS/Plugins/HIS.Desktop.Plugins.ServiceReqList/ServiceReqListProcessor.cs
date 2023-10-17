﻿using HIS.Desktop.Plugins.ServiceReqList.ServiceReqList;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceReqList
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
        "HIS.Desktop.Plugins.ServiceReqList",
        "Danh sách yêu cầu dịch vụ",
        "Common",
        68,
        "y-lenh.png",
        "A",
        Module.MODULE_TYPE_ID__FORM,
        true,
        true)]
    public class ServiceReqListProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public ServiceReqListProcessor()
        {
            param = new CommonParam();
        }
        public ServiceReqListProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        object IDesktopRoot.Run(object[] args)
        {
            Inventec.Common.Logging.LogSystem.Info("begin load");
            object result = null;
            try
            {
                IServiceReqList behavior = ServiceReqListFactory.MakeIServiceReqList(param, args);
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
