﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Desktop.Core;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;

namespace HIS.Desktop.Plugins.AppointmentInfoVacxin.AppointmentInfoVacxin
{
    class SignAppointmentInfoVacxinProcessor
    {
        [ExtensionOf(typeof(DesktopRootExtensionPoint),
           "HIS.Desktop.Plugins.AppointmentInfoVacxin",
           "",
           "",
           0,
           "",
           "",
           Module.MODULE_TYPE_ID__FORM,
           true,
           true)
        ]
        public class SignAppointmentInfoVacxinBehavior : ModuleBase, IDesktopRoot
        {
            CommonParam param;
            public SignAppointmentInfoVacxinBehavior()
            {
                param = new CommonParam();
            }
            public SignAppointmentInfoVacxinBehavior(CommonParam paramBusiness)
            {
                param = (paramBusiness != null ? paramBusiness : new CommonParam());
            }

            public object Run(object[] args)
            {
                object result = null;
                try
                {
                    ISignAppointmentInfoVacxin behavior = SignAppointmentInfoVacxinFactory.MakeISignedDocument(param, args);
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
}
