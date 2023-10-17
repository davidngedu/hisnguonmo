﻿using HIS.Desktop.Common;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisQcNormation.HisQcNormation
{
    class HisQcNormationBehavior : BusinessBase, IHisQcNormation
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module moduleData;
        HIS_MACHINE HisMachine;
        internal HisQcNormationBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisQcNormation.Run()
        {
            try
            {
                if (entity.GetType() == typeof(object[]))
                {
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                            else if (entity[i] is HIS_MACHINE)
                            {
                                HisMachine = (HIS_MACHINE)entity[i];
                            }
                        }
                    }
                }

                return new frmHisQcNormation(moduleData, HisMachine);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
