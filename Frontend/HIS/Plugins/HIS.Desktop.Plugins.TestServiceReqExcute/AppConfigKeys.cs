﻿using HIS.Desktop.LocalStorage.BackendData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TestServiceReqExcute
{
   public class AppConfigKeys
    {
        internal const string HIS_DESKTOP_PLUGINS_TEST_CHECKVALUEMAXLENGOPTION = "HIS.Desktop.Plugins.Test.CheckValueMaxlengthOption";
        internal const string HIS_DESKTOP_PLUGINS_SUBCLINICAL_MACHINE_OPTION = "HIS.Desktop.Plugins.ServiceExecute.SubclinicalMachineOption";

        internal const string CONFIG_KEY__MOS__HIS_SERVICE_REQ__ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR = "MOS.HIS_SERVICE_REQ.ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR";
        private const string CONFIG_KEY__ProcessTimeMustBeGreaterThanTotalProcessTime = "HIS.Desktop.Plugins.ProcessTimeMustBeLessThanMaxTotalProcessTime";
        private const string CONFIG_KEY__HIS_SERVICE_REQ_SAMPLE_INFO_OPTION = "HIS.HIS_SERVICE_REQ.SAMPLE_INFO_OPTION";
        internal const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";
        internal const string CONFIG_KEY__PATIENT_TYPE_OPTION = "HIS.DESKTOP.HIS_MACHINE.MAX_SERVICE_PER_DAY.PATIENT_TYPE_OPTION";
        internal const string CONFIG_KEY__IsMachineWarningOption = "HIS.DESKTOP.HIS_MACHINE.MAX_SERVICE_PER_DAY.WARNING_OPTION";
        internal static bool Is_ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR { get; set; }
        internal static string ProcessTimeMustBeGreaterThanTotalProcessTime;
        internal static string HisServiceReqSampleInfoOption;
        internal static string SubclinicalMachineOption;

        internal static void GetConfigKey()
        {
            try
            {
                ProcessTimeMustBeGreaterThanTotalProcessTime = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__ProcessTimeMustBeGreaterThanTotalProcessTime);
                HisServiceReqSampleInfoOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__HIS_SERVICE_REQ_SAMPLE_INFO_OPTION);
                Is_ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(CONFIG_KEY__MOS__HIS_SERVICE_REQ__ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR) == 1;
                SubclinicalMachineOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS_DESKTOP_PLUGINS_SUBCLINICAL_MACHINE_OPTION);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        static MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE GetPatientTypeByCode(string code)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
        }
        internal static string IsPatientTypeOption
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__PATIENT_TYPE_OPTION);
            }
        }
        internal static long PatientTypeId__BHYT
        {
            get
            {
                return GetPatientTypeByCode(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT)).ID;
            }
        }
        internal static string IsMachineWarningOption
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__IsMachineWarningOption);
            }
        }
    }
}
