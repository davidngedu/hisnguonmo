﻿using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignNutrition.Config
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
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong VP
        private const string CONFIG_KEY__IS_VISILBE_EXECUTE_GROUP_KEY = "HIS.Desktop.Plugins.Assign.IsExecuteGroup";
        private const string CONFIG_KEY__ICD_GENERA_KEY = "HIS.Desktop.Plugins.AutoCheckIcd";
        private const string CONFIG_KEY__AssignServicePrintTEST = "HIS.Desktop.Plugins.AssignServicePrintTEST";
        private const string CONFIG_KEY__Icd_Service_Has_Check = "HIS.HIS_ICD_SERVICE.HAS_CHECK";
        private const string CONFIG_KEY__Icd_Service_Allow_Update = "HIS.HIS_ICD_SERVICE.ALLOW_UPDATE";
        private const string Key__WarningOverCeiling__Exam__Out__In = "HIS.Desktop.Plugins.WarningOverCeiling.Exam__Out__In";
        private const string CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE = "HIS.Desktop.WarningOverTotalPatientPrice";
        private const string CONFIG_KEY__WARNING_OVER_TOTAL_PATIENT_PRICE__IS_CHECK = "HIS.Desktop.WarningOverTotalPatientPrice__IsCheck";

        private const string CONFIG_KEY__IS_DEFAULT_TRACKING = "HIS.Desktop.Plugins.AssignPrescription.IsDefaultTracking";
        internal const string SERVICE_HAS_PAYMENT_LIMIT_BHYT = "HIS.Desktop.Plugins.AssignService.ServiceHasPaymentLimitBHYT";
        private const string CONFIG_KEY__IsSearchAll = "HIS.Desktop.Plugins.AssignService.IsSearchAll";
        private const string CONFIG_KEY__HIS_SERE_SERV__SET_PRIMARY = "MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE";

        private const string CONFIG_KEY__IS_MINE_CHECKED_BY_DEFAULT = "HIS.Desktop.Plugins.TrackingCreate.IsMineCheckedByDefault";
        private const string CONFIG_KEY__CHOOSEROOM_GROUP_ROOM_OPTION = "HIS.Desktop.Plugins.ChooseRoom.GroupRoomOption";

        internal static bool IsChooseRoomGroupRoomOption;

        internal static string IsSetPrimaryPatientType;
        public static decimal WarningOverCeiling__Exam { get; set; }
        public static decimal WarningOverCeiling__Out { get; set; }
        public static decimal WarningOverCeiling__In { get; set; }

        internal static string TreatmentTypeCode__Exam;
        internal static string TreatmentTypeCode__TreatIn;
        internal static string TreatmentTypeCode__TreatOut;

        internal static bool AssignPrintTEST;
        internal static string ObligateIcd;
        internal static string SetDefaultDepositPrice;
        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static string PatientTypeCode__VP;
        internal static long PatientTypeId__VP;
        internal static string IsVisibleExecuteGroup;
        internal static string AutoCheckIcd;
        internal static string IcdServiceHasCheck;
        internal static string IcdServiceAllowUpdate;

        internal static string WarningOverTotalPatientPrice;
        internal static string WarningOverTotalPatientPrice__IsCheck;
        internal static long IsDefaultTracking;
        internal static string ServiceHasPaymentLimitBHYT;
        internal static bool IsSearchAll;
        internal static long IsMineCheckedByDefault;

        /// <summary>
        /// Cấu hình để ẩn/hiện trường người chỉ định tai form chỉ định, kê đơn
        //- Giá trị mặc định (hoặc ko có cấu hình này) sẽ ẩn
        //- Nếu có cấu hình, đặt là 1 thì sẽ hiển thị
        /// </summary>
        internal static string ShowRequestUser;
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
                IsSingleCheckservice = GlobalVariables.CommonStringTrue;//Fix chỉ cho phép check chọn 1 nhóm cha
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PatientTypeCode__VP = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                PatientTypeId__VP = GetPatientTypeByCode(PatientTypeCode__VP).ID;
                IsSearchAll = (GetValue(CONFIG_KEY__IsSearchAll) == GlobalVariables.CommonStringTrue);
                IsSetPrimaryPatientType = GetValue(CONFIG_KEY__HIS_SERE_SERV__SET_PRIMARY);

                IsDefaultTracking = HisConfigs.Get<long>(CONFIG_KEY__IS_DEFAULT_TRACKING);
                IsMineCheckedByDefault = HisConfigs.Get<long>(CONFIG_KEY__IS_MINE_CHECKED_BY_DEFAULT);
                IsChooseRoomGroupRoomOption = GetValue(CONFIG_KEY__CHOOSEROOM_GROUP_ROOM_OPTION) == "1"; 
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        static MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE GetTreatmentTypeById(long id)
        {
            MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE();
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE();
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

        public static void InitWarningOverCeiling()
        {

            try
            {

                var vl = GetValue(Key__WarningOverCeiling__Exam__Out__In);

                if (!String.IsNullOrEmpty(vl))
                {

                    var arrVl = vl.Split(new String[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                    if (arrVl != null && arrVl.Length == 3)
                    {

                        WarningOverCeiling__Exam = Inventec.Common.TypeConvert.Parse.ToDecimal(arrVl[0]);

                        WarningOverCeiling__Out = Inventec.Common.TypeConvert.Parse.ToDecimal(arrVl[1]);

                        WarningOverCeiling__In = Inventec.Common.TypeConvert.Parse.ToDecimal(arrVl[2]);

                    }

                }
            }

            catch (Exception ex)
            {

                LogSystem.Warn(ex);

            }

        }
    }
}
