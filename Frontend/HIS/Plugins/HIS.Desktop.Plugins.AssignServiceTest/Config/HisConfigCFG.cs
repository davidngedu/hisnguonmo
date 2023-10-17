﻿using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignServiceTest.Config
{
    class HisConfigCFG
    {
        private const string CONFIG_KEY__IsSingleCheckservice = "HIS.Desktop.Plugins.AssignService.IsSingleCheckservice";
        private const string CONFIG_KEY__ShowRequestUser = "HIS.Desktop.Plugins.AssignConfig.ShowRequestUser";
        private const string CONFIG_KEY__NoDifference = "HIS.Desktop.Plugins.AssignService.NoDifference";
        private const string CONFIG_KEY__HeadCardNumberNoDifference = "HIS.Desktop.Plugins.AssignService.HeadCardNumberNoDifference";
        private const string CONFIG_KEY__OBLIGATE_ICD = "EXE.ASSIGN_SERVICE_REQUEST__OBLIGATE_ICD";
        public const string CONFIG_KEY__HIS_DEPOSIT__DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT = "HIS_RS.HIS_DEPOSIT.DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT";//tính tiền dịch vụ cần tạm ứng(ct MOS.LibraryHein.Bhyt.BhytPriceCalculator.DefaultPriceForBhytOutPatient) ở chức năng tạm ứng dịch vụ theo dịch vụ.
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string CONFIG_KEY__IS_VISILBE_EXECUTE_GROUP_KEY = "HIS.Desktop.Plugins.Assign.IsExecuteGroup";
        private const string CONFIG_KEY__ICD_GENERA_KEY = "HIS.Desktop.Plugins.AutoCheckIcd";
        private const string CONFIG_KEY__AssignServicePrintTEST = "HIS.Desktop.Plugins.AssignServicePrintTEST";

        internal static bool AssignPrintTEST;
        internal static string ObligateIcd;
        internal static string SetDefaultDepositPrice;
        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static string IsVisibleExecuteGroup;
        internal static string AutoCheckIcd;


        /// <summary>
        /// Cấu hình để ẩn/hiện trường người chỉ định tai form chỉ định, kê đơn
        ///- Giá trị mặc định (hoặc ko có cấu hình này) sẽ ẩn
        ///- Nếu có cấu hình, đặt là 1 thì sẽ hiển thị
        /// </summary>
        internal static string ShowRequestUser;

        /// <summary>
        /// cấu hình hệ thống để hiển thị tủ trực hay không
        ///Đặt 1 là chỉ hiển thị các kho là tủ trực, giá trị khác là hiển thị tất cả các kho
        /// </summary>
        internal static string IsSingleCheckservice;

        internal static string HeadCardNumberNoDifference;
        internal static string NoDifference;

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
                ShowRequestUser = GetValue(CONFIG_KEY__ShowRequestUser);
                IsSingleCheckservice = GetValue(CONFIG_KEY__IsSingleCheckservice);
                HeadCardNumberNoDifference = GetValue(CONFIG_KEY__HeadCardNumberNoDifference);
                NoDifference = GetValue(CONFIG_KEY__NoDifference);
                ObligateIcd = GetValue(CONFIG_KEY__OBLIGATE_ICD);
                SetDefaultDepositPrice = GetValue(CONFIG_KEY__HIS_DEPOSIT__DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT);
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                IsVisibleExecuteGroup = GetValue(CONFIG_KEY__IS_VISILBE_EXECUTE_GROUP_KEY);
                AutoCheckIcd = GetValue(CONFIG_KEY__ICD_GENERA_KEY);
                AssignPrintTEST = (GetValue(CONFIG_KEY__AssignServicePrintTEST) == "1");
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
