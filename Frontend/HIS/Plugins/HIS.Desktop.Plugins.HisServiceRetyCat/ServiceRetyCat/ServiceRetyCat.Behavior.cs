﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisServiceRetyCat.HisServiceRetyCat
{
    class RoomServiceBehavior : Tool<IDesktopToolContext>, IRoomService
    {
        object[] entity;
        long treatmentId;
        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_SERVICE service;

        internal RoomServiceBehavior()
            : base()
        {

        }

        internal RoomServiceBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IRoomService.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is V_HIS_SERVICE)
                        {
                            service = (V_HIS_SERVICE)item;
                        }
                        else if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                    }

                    if (service != null)
                        result = new UCServiceRetyCat(currentModule,service);
                    else
                        result = new UCServiceRetyCat(currentModule);
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
