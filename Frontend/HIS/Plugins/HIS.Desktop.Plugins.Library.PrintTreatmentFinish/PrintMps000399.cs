﻿using ACS.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintTreatmentFinish
{
    class PrintMps000399
    {
        MPS.Processor.Mps000399.PDO.Mps000399PDO mps000008RDO { get; set; }

        public PrintMps000399(string printTypeCode, string fileName, ref bool result, MOS.EFMODEL.DataModels.V_HIS_PATIENT HisPatient, MOS.EFMODEL.DataModels.HIS_TREATMENT HisTreatment, bool _printNow, long? roomId)
        {
            try
            {
                if (HisTreatment == null || HisTreatment.ID <= 0)
                {
                    result = false;
                    return;
                }

                MPS.Processor.Mps000399.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000399.PDO.SingleKeyValue();
                singleKeyValue.LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (HisTreatment.ICD_NAME != null)
                {
                    singleKeyValue.Icd_Name = HisTreatment.ICD_NAME;
                }
                else
                {
                    singleKeyValue.Icd_Name = HisTreatment.ICD_NAME;
                }

                if (HisTreatment.END_DEPARTMENT_ID.HasValue)
                {
                    var department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == HisTreatment.END_DEPARTMENT_ID.Value);
                    singleKeyValue.END_DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                }

                mps000008RDO = new MPS.Processor.Mps000399.PDO.Mps000399PDO(
                   HisPatient,
                   HisTreatment,
                   singleKeyValue
                   );

                result = Print.RunPrint(printTypeCode, fileName, mps000008RDO, null
                    , result, _printNow, roomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
