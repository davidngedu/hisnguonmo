﻿using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.ElectronicBillTotal.ElectronicBillTotal
{
    class ElectronicBillTotalBehavior : Tool<IDesktopToolContext>, IElectronicBillTotal
    {
        object[] entity;

        internal ElectronicBillTotalBehavior()
            : base()
        {

        }

        internal ElectronicBillTotalBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IElectronicBillTotal.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                V_HIS_TREATMENT_FEE hisTreatment = null;
                V_HIS_PATIENT_TYPE_ALTER resultPatientType = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        else if (entity[i] is V_HIS_TREATMENT_FEE)
                        {
                            hisTreatment = (V_HIS_TREATMENT_FEE)entity[i];
                        }
                        else if (entity[i] is V_HIS_PATIENT_TYPE_ALTER)
                        {
                            resultPatientType = (V_HIS_PATIENT_TYPE_ALTER)entity[i];
                        }
                    }
                }

                if (moduleData != null)
                {
                    result = new FormElectronicBillTotal(moduleData, hisTreatment, resultPatientType);
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
