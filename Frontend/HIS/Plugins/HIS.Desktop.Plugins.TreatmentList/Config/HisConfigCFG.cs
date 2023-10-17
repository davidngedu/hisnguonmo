﻿using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentList.Config
{
    class HisConfigCFG
    {
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong VP

        private const string CONFIG_KEY__OLD_SYSTEM_INTEGRATION_TYPE = "MOS.OLD_SYSTEM.INTEGRATION_TYPE";
        private const string CONFIG_KEY__IS_TREATMENT_DAY_COUNT_6556 = "XML.EXPORT.4210.IS_TREATMENT_DAY_COUNT_6556";
        private const string CONFIG_KEY_MOS_HIS_HEIN_APPROVAL_SYNC_XML_FPT_OPTION = "MOS.HIS_HEIN_APPROVAL.SYNC_XML_FPT_OPTION";
        private const string HSSKAddressCFG = "HIS.Desktop.EhrViewer.LinkAddress";
        private const string HSSKBase64UrlParamInputCFG = "HIS.Desktop.EhrViewer.Base64UrlParamInput";
        private const string CONFIG_KEY__IS_ALLOW_PRINT_NO_MEDICINE = "HIS.Desktop.Plugins.ExamServiceReqExecute.IsAllowPrintNoMedicinePrescription";
        private const string CONFIG_KEY__MPS_PrintPrescription = "HIS.Desktop.Plugins.Library.PrintPrescription.Mps";
        private const string CONFIG_KEY__UnlockConditionOption = "HIS.Desktop.Plugins.TreatmentList.UnlockConditionOption";
        internal static bool IsUnlockConditionOption;
        internal static string HSSKAddress;
        internal static string HSSKBase64UrlParamInput;

        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static string PatientTypeCode__VP;
        internal static long PatientTypeId__VP;
        internal static string SYNC_XML_FPT_OPTION;
        internal static bool IsTreatmentDayCount6556;
        internal static bool IsAllowPrintNoMedicine;
        internal static string OldSystemIntegrationType;
        internal static string MPS_PrintPrescription;

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

        internal static void LoadConfig()
        {
            try
            {
                IsUnlockConditionOption = GetValue(CONFIG_KEY__UnlockConditionOption) == "1";
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PatientTypeCode__VP = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                PatientTypeId__VP = GetPatientTypeByCode(PatientTypeCode__VP).ID;
                OldSystemIntegrationType = GetValue(CONFIG_KEY__OLD_SYSTEM_INTEGRATION_TYPE);
                IsTreatmentDayCount6556 = GetValue(CONFIG_KEY__IS_TREATMENT_DAY_COUNT_6556) == "1";
                HSSKAddress = GetValue(HSSKAddressCFG);
                HSSKBase64UrlParamInput = GetValue(HSSKBase64UrlParamInputCFG);
                SYNC_XML_FPT_OPTION = GetValue(CONFIG_KEY_MOS_HIS_HEIN_APPROVAL_SYNC_XML_FPT_OPTION);
                IsAllowPrintNoMedicine = GetValue(CONFIG_KEY__IS_ALLOW_PRINT_NO_MEDICINE) == GlobalVariables.CommonStringTrue;
                MPS_PrintPrescription = GetValue(CONFIG_KEY__MPS_PrintPrescription);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }
    }
}
