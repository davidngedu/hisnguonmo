﻿using Inventec.Core;
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.HoreHandover;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HoreHandover.HoreHandover
{
    public sealed class HoreHandoverBehavior : Tool<IDesktopToolContext>, IHoreHandover
    {
        object[] entity;
        public HoreHandoverBehavior()
            : base()
        {
        }

        public HoreHandoverBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHoreHandover.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                long? horeHandoverId = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        else if (entity[i] is long)
                        {
                            horeHandoverId = (long)entity[i];
                        }
                    }
                }
                if (moduleData != null && horeHandoverId.HasValue)
                {
                    return new frmHoreHandover(moduleData, horeHandoverId.Value);
                }
                else if (moduleData != null)
                {
                    return new frmHoreHandover(moduleData);
                }
                else
                {
                    return null;
                }
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
