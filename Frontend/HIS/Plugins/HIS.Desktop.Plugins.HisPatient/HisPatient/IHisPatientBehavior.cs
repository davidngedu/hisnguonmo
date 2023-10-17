﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;

namespace HIS.Desktop.Plugins.HisPatient.HisPatient
{
    class HisPatientBehavior : Tool<IDesktopToolContext>, IHisPatient
    {
        object[] entity;
        internal HisPatientBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisPatient.Run()
        {
            try
            {

                Inventec.Desktop.Common.Modules.Module moduleData = null;
                MOS.EFMODEL.DataModels.V_HIS_PATIENT patient = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        if (entity[i] is MOS.EFMODEL.DataModels.V_HIS_PATIENT)
                        {
                            patient = entity[i] as MOS.EFMODEL.DataModels.V_HIS_PATIENT;
                        }
                    }
                }
                if (moduleData != null)
                {
                    return new UCHisPatient(moduleData, patient);
                }
                else
                {
                    return null;
                }
                //return new UCHisPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
