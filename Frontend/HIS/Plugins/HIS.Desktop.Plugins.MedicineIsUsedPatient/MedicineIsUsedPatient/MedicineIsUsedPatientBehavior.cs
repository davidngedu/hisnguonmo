﻿using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.MedicineIsUsedPatient.MedicineIsUsedPatient
{
    class MedicineIsUsedPatientBehavior : Tool<IDesktopToolContext>, IMedicineIsUsedPatient
    {
        object[] entity;

        internal MedicineIsUsedPatientBehavior()
            : base()
        {

        }

        internal MedicineIsUsedPatientBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IMedicineIsUsedPatient.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                string _treatmentCode="";
                //V_HIS_TREATMENT_FEE hisTreatment = null;
                //V_HIS_PATIENT_TYPE_ALTER resultPatientType = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        else if (entity[i] is string)
                        {
                            _treatmentCode = (string)entity[i];
                        }
                        //else if (entity[i] is V_HIS_PATIENT_TYPE_ALTER)
                        //{
                        //    resultPatientType = (V_HIS_PATIENT_TYPE_ALTER)entity[i];
                        //}
                    }
                }

                if (moduleData != null)
                {
                    result = new frmMedicineIsUsedPatient(moduleData, _treatmentCode);
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
