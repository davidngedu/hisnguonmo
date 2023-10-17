﻿using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SurgServiceReqExecute.Config
{
    class HisConfigCFG
    {
        internal static long PatientTypeId__BHYT
        {
            get
            {
                var ptBHYT = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.PATIENT_TYPE_CODE == HisConfigs.Get<string>(HisConfigKeys.HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT)).FirstOrDefault();
                return ptBHYT != null ? ptBHYT.ID : 0;
            }
        }

        internal static string TAKE_INTRUCTION_TIME_BY_SERVICE_REQ
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.SurgServiceReqExecute.TakeIntrucionTimeByServiceReq");
            }
        }

        internal static string REQUIRED_GROUPPTTT_OPTION
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.SurgServiceReqExecute.RequiredGroupPTTTOption");
            }
        }

        internal static string RequiredEmotionlessMethodOption
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.SurgServiceReqExecute.RequiredEmotionlessMethodOption");
            }
        }

        internal static string IS_NOT_REQUIRED_PTTT_EXECUTE_ROLE
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.SurgServiceReqExecute.IsNotRequiredPtttExecuteRole");
            }
        }

        internal static string PROCESS_TIME_MUST_BE_LESS_THAN_MAX_TOTAL_PROCESS_TIME
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ProcessTimeMustBeLessThanMaxTotalProcessTime");
            }
        }
        
    }
}
