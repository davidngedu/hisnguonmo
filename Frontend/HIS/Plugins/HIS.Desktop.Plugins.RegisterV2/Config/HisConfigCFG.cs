﻿using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RegisterV2.Config
{
    internal class HisConfigCFG
    {
        //private const string CONFIG_KEY__IS_AUTO_FILL_DATA_RECENT_SERVICE_ROOM = "HIS.IS_AUTO_FILL_DATA_RECENT_SERVICE_ROOM";
        private const string CONFIG_KEY__IS_CHECK_EXAM_HISTORY_TODAY = "HIS.Desktop.Plugins.Register.IS_CHECK_EXAM_HISTORY_TODAY";
        private const string CONFIG_KEY__IS_CHECK_HEIN_CARD = "HIS.Desktop.Plugins.Register.IsCheckHeinCard";
        private const string CONFIG_KEY__IS_CHECK_PREVIOUS_DEBT = "MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_DEBT";
        private const string CONFIG_KEY__IS_CHECK_PREVIOUS_PRESCRIPTION = "MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_PRESCRIPTION";
        private const string CONFIG_KEY__IS_DEFAULT_RIGHT_ROUTE_TYPE = "HIS.Desktop.Plugins.Register.IsDefaultRightRouteType";
        private const string CONFIG_KEY__VISIBILITY_CONTROL = "HIS.HIS_DESKTOP_REGISTER.VISIBILITY_CONTROL_FOR_TIM";
        private const string CONFIG_KEY__NOT_CHECK_EXPIRED_IS_SHOW = "HIS.DESKTOP.REGISTER.HEIN_CARD.NOT_CHECK_EXPIRED.IS_SHOW";
        private const string CONFIG_KEY__ICD_GENERATE = "HIS.Desktop.Plugins.AutoCheckIcd";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__QN = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.QN"; // Doi tuong Quan Nhan
        private const string CONFIG_KEY__IS_PRINT_AFTER_SAVE__KEY = "EXE.SERVICE_REQUEST_REGISTER.IS_PRINT_AFTER_SAVE";
        private const string CONFIG_KEY__IS_VISIBLE_BILL__KEY = "EXE.SERVICE_REQUEST_REGISTER.IS_VISIBLE_BILL";
        private const string CONFIG_KEY__MOS__HIS_PATIENT__MUST_HAVE_NCS_INFO_FOR_CHILD = "MOS.HIS_PATIENT.MUST_HAVE_NCS_INFO_FOR_CHILD";
        private const string CONFIG_KEY__HIS_DEPOSIT__DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT = "HIS_RS.HIS_DEPOSIT.DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT";//tính tiền dịch vụ cần tạm ứng(ct MOS.LibraryHein.Bhyt.BhytPriceCalculator.DefaultPriceForBhytOutPatient) ở chức năng tạm ứng dịch vụ theo dịch vụ.
        private const string CONFIG_KEY__GENDER_CODE_BASE = "RAE.HIS_GENDER_CODE__BASE";
        private const string CONFIG_KEY__HIS_CAREER_CODE__BASE = "EXE.HIS_CAREER_CODE__BASE";
        private const string CONFIG_KEY__ETHNIC_CODE_BASE = "EXE.ETHNIC_CODE_BASE";
        private const string CONFIG_KEY__NATIONAL_CODE_BASE = "EXE.NATIONAL_CODE_BASE";
        private const string CONFIG_KEY__CAREER_CODE__UNDER_6_AGE = "EXE.HIS_CAREER_CODE__UNDER_6_AGE";
        private const string CONFIG_KEY__CAREER_CODE__HOC_SINH = "HIS.DESKTOP.REGISTER.HIS_CAREER.CARRER_CODE_HS";
        private const string HIS_DESKTOP_REGISER__EXECUTE_ROOM_SHOW = "HIS.HIS_DESKTOP_REGISTER.EXECUTE_ROOM_CODE.SHOW";
        private const string HIS_UC_UCHein_IS_OBLIGATORY_TRANFER_MEDI_ORG = "HIS.UC.UCHein.IS_OBLIGATORY_TRANFER_MEDI_ORG";

        private const string CONFIG_KEY__IS_USE_HID_SYNC = "CONFIG_KEY__IS_USE_HID_SYNC";
        private const string CONFIG_KEY__WarningOverExamBhyt = "HIS.Desktop.WarningOverExamBhyt";

        const string valueString__true = "1";
        const int valueInt__true = 1;

        internal static string IsShowCheckExpired;
        internal static bool IsCheckHeinCard;
        internal static bool IsCheckPreviousDebt;
        internal static bool IsCheckPreviousPrescription;
        internal static string IsDefaultRightRouteType;
        internal static bool VisibilityControl;
        internal static bool IsObligatoryTranferMediOrg;
        public static bool IsSyncHID { get; set; } // cau hinh nhap tinh khai sinh
        internal static bool IsWarningOverExamBhyt;

        /// <summary>
        /// - Cấu hình chế độ kiểm tra thẻ bhyt trên cổng bhxh. Đặt 1 là tự động kiểm tra khi tìm thấy bệnh nhân cũ, đặt số khác là không kiểm tra.
        /// </summary>
        internal static bool IsCheckExamHistory;
        internal static string AutoCheckIcd;
        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;
        internal static string PatientTypeCode__QN;
        internal static long PatientTypeId__QN;
        internal static string IsPrintAfterSave;
        internal static string IsVisibleBill;
        internal static bool IsSetDefaultDepositPrice;
        internal static List<string> ExecuteRoomShow;

        /// <summary>
        /// true - Bắt buộc nhập thông tin Người nhà, quan hệ, địa chỉ với trẻ em nhỏ hơn 6t
        /// false - Không bắt buộc nhập thông tin Người nhà, quan hệ, địa chỉ với mọi đối tượng
        /// </summary>
        internal static bool MustHaveNCSInfoForChild;

        /// <summary>
        /// Cấu hình tự động fill yêu cầu - phòng khám gần nhất khi tìm bệnh nhân cũ
        /// Đặt 1 là tự động fill
        /// Mặc định là không tự động
        /// </summary>
        // internal static string IsAutoFillDataRecentServiceRoom;//Chuyen sang AppConfig

        internal static MOS.EFMODEL.DataModels.HIS_GENDER GenderBase;
        internal static MOS.EFMODEL.DataModels.HIS_CAREER CareerBase;
        internal static MOS.EFMODEL.DataModels.HIS_CAREER CareerHS;
        internal static MOS.EFMODEL.DataModels.HIS_CAREER CareerUnder6Age;
        internal static SDA.EFMODEL.DataModels.SDA_NATIONAL NationalBase;
        internal static SDA.EFMODEL.DataModels.SDA_ETHNIC EthinicBase;

        internal static void LoadConfig()
        {
            try
            {
                LogSystem.Debug("LoadConfig => 1");
                ExecuteRoomShow = GetListValue(HIS_DESKTOP_REGISER__EXECUTE_ROOM_SHOW);
                IsSetDefaultDepositPrice = (Inventec.Common.TypeConvert.Parse.ToInt32(GetValue(CONFIG_KEY__HIS_DEPOSIT__DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT)) == valueInt__true);
                MustHaveNCSInfoForChild = (GetValue(CONFIG_KEY__MOS__HIS_PATIENT__MUST_HAVE_NCS_INFO_FOR_CHILD) == valueString__true);
                IsPrintAfterSave = GetValue(CONFIG_KEY__IS_PRINT_AFTER_SAVE__KEY);
                IsVisibleBill = GetValue(CONFIG_KEY__IS_VISIBLE_BILL__KEY);
                IsObligatoryTranferMediOrg = (GetValue(HIS_UC_UCHein_IS_OBLIGATORY_TRANFER_MEDI_ORG) == valueString__true);
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PatientTypeCode__QN = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__QN);
                PatientTypeId__QN = GetPatientTypeByCode(PatientTypeCode__QN).ID;
                AutoCheckIcd = GetValue(CONFIG_KEY__ICD_GENERATE);
                IsShowCheckExpired = GetValue(CONFIG_KEY__NOT_CHECK_EXPIRED_IS_SHOW);
                IsCheckHeinCard = (GetValue(CONFIG_KEY__IS_CHECK_HEIN_CARD) == valueString__true);
                IsCheckPreviousDebt = (GetValue(CONFIG_KEY__IS_CHECK_PREVIOUS_DEBT) == valueString__true);
                IsCheckPreviousPrescription = (GetValue(CONFIG_KEY__IS_CHECK_PREVIOUS_PRESCRIPTION) == valueString__true);
                IsDefaultRightRouteType = GetValue(CONFIG_KEY__IS_DEFAULT_RIGHT_ROUTE_TYPE);
                VisibilityControl = (GetValue(CONFIG_KEY__VISIBILITY_CONTROL) == valueString__true);
                // IsAutoFillDataRecentServiceRoom = GetValue(CONFIG_KEY__IS_AUTO_FILL_DATA_RECENT_SERVICE_ROOM);
                IsCheckExamHistory = (GetValue(CONFIG_KEY__IS_CHECK_EXAM_HISTORY_TODAY) == valueString__true);
                GenderBase = GetGenderByCode(GetValue(CONFIG_KEY__GENDER_CODE_BASE));
                CareerBase = GetCareerByCode(GetValue(CONFIG_KEY__HIS_CAREER_CODE__BASE));
                CareerHS = GetCareerByCode(GetValue(CONFIG_KEY__CAREER_CODE__HOC_SINH));
                CareerUnder6Age = GetCareerByCode(GetValue(CONFIG_KEY__CAREER_CODE__UNDER_6_AGE));
                EthinicBase = GetEthnicByCode(GetValue(CONFIG_KEY__ETHNIC_CODE_BASE));
                NationalBase = GetNationalByCode(GetValue(CONFIG_KEY__NATIONAL_CODE_BASE));

                IsSyncHID = (GetValue(CONFIG_KEY__IS_USE_HID_SYNC) == valueString__true);
                IsWarningOverExamBhyt = (GetValue(CONFIG_KEY__WarningOverExamBhyt) == valueString__true);

                LogSystem.Debug("LoadConfig => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        static MOS.EFMODEL.DataModels.HIS_CAREER GetCareerByCode(string code)
        {
            MOS.EFMODEL.DataModels.HIS_CAREER result = new MOS.EFMODEL.DataModels.HIS_CAREER();
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().FirstOrDefault(o => o.CAREER_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new MOS.EFMODEL.DataModels.HIS_CAREER();
        }

        static SDA.EFMODEL.DataModels.SDA_ETHNIC GetEthnicByCode(string code)
        {
            SDA.EFMODEL.DataModels.SDA_ETHNIC result = new SDA.EFMODEL.DataModels.SDA_ETHNIC();
            try
            {
                result = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().FirstOrDefault(o => o.ETHNIC_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new SDA.EFMODEL.DataModels.SDA_ETHNIC();
        }

        static SDA.EFMODEL.DataModels.SDA_NATIONAL GetNationalByCode(string code)
        {
            SDA.EFMODEL.DataModels.SDA_NATIONAL result = new SDA.EFMODEL.DataModels.SDA_NATIONAL();
            try
            {
                result = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>().FirstOrDefault(o => o.NATIONAL_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new SDA.EFMODEL.DataModels.SDA_NATIONAL();
        }

        static MOS.EFMODEL.DataModels.HIS_GENDER GetGenderByCode(string code)
        {
            MOS.EFMODEL.DataModels.HIS_GENDER result = new MOS.EFMODEL.DataModels.HIS_GENDER();
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().FirstOrDefault(o => o.GENDER_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new MOS.EFMODEL.DataModels.HIS_GENDER();
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

        private static string GetValue(string key)
        {
            try
            {
                return HisConfigs.Get<string>(key);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return "";
        }

        private static List<string> GetListValue(string key)
        {
            try
            {
                return HisConfigs.Get<List<string>>(key);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
    }
}
