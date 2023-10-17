﻿using HIS.Desktop.Plugins.MedicineIsUsedPatient.MedicineIsUsedPatient;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicineIsUsedPatient
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
     "HIS.Desktop.Plugins.MedicineIsUsedPatient",
     "Danh mục",
     "Bussiness",
     4,
     "showproduct_32x32.png",
     "A",
     Module.MODULE_TYPE_ID__FORM,
     true,
     true)
    ]
    public class MedicineIsUsedPatientProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public MedicineIsUsedPatientProcessor()
        {
            param = new CommonParam();
        }

        public MedicineIsUsedPatientProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        object IDesktopRoot.Run(object[] args)
        {
            object result = null;
            try
            {
                IMedicineIsUsedPatient behavior = MedicineIsUsedPatientFactory.MakeIMedicineIsUsedPatient(param, args);
                result = (behavior != null) ? behavior.Run() : null;
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
