﻿using HIS.Desktop.Plugins.InformationAllowGoHome.UCCauseOfDeath.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InformationAllowGoHome.UCCauseOfDeath.SetValue
{
    class SetValueFactory
    {
        internal static ISetValue MakeISetValue(CommonParam param, UserControl uc, CauseOfDeathADO data)
        {
            ISetValue result = null;
            try
            {
                if (uc is UserControl)
                {
                    result = new SetValueBehavior(param, (UserControl)uc, data);
                }
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + uc.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => uc), uc), ex);
                result = null;
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
