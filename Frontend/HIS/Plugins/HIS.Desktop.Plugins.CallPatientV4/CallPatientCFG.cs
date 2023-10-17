﻿using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Common.Logging;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientV4
{
    public class WaitingScreenCFG
    {
        private const string CALL_PATIENT_DEN = "EXE.CALL_PATIENT.DEN";
        private const string CALL_PATIENT_CO_STT = "EXE.CALL_PATIENT.CO_STT";
        private const string CALL_PATIENT_MOI_BENH_NHAN = "EXE.CALL_PATIENT.MOI_BENH_NHAN";
        private const string THIS = "EXE.WAITING_SCREEN.ORGANIZATION_NAME";
        private const string ROOM_NAM_COLOR_CODES = "EXE.WAITING_SCREEN.ROOM_NAME.COLOR_CODES";//Mã màu chữ phòng khám
        private const string USER_NAM_COLOR_CODES = "EXE.WAITING_SCREEN.USER_NAME.COLOR_CODES";//Mã màu chữ tên bác sĩ
        private const string BACKGROUND_PARENT_COLOR_CODES = "EXE.WAITING_SCREEN.BACKGROUND_PARENT.COLOR_CODES";//Mã màu background form
        private const string ORGANIZATION_NAME_COLOR_CODES = "EXE.WAITING_SCREEN.ORGANIZATION_NAME.COLOR_CODES";//Mã màu tên tổ chức
        private const string GRID_PATIENT_BACK_COLOR_CODES = "EXE.WAITING_SCREEN.BACK_COLOR_GRID_PATIENT.COLOR_CODES";//Mã màu nền danh sách bệnh nhân
        private const string GRID_PATIENT_HEADER_BACK_COLOR_CODES = "EXE.WAITING_SCREEN.BACK_COLOR_GRID_PATIENT_HEADER.COLOR_CODES";//Mã màu nền header của danh sách bệnh nhân
        private const string GRID_PATIENT_HEADER_FORCE_COLOR_CODES = "EXE.WAITING_SCREEN.FORCE_COLOR_GRID_PATIENT_HEADER.COLOR_CODES";//Mã màu chữ header của danh sách bệnh nhân
        private const string GRID_PATIENT_BODY_FORCE_COLOR_CODES = "EXE.WAITING_SCREEN.FORCE_COLOR_GRID_PATIENT_BODY.COLOR_CODES";//Mã màu chữ body của danh sách bệnh nhân
        private const string PAGING_LABEL_FORCE_COLOR_CODES = "EXE.WAITING_SCREEN.FORCE_COLOR_PAGING.COLOR_CODES";//Mã màu chữ phân trang
        private const string NEW_STATUS_FORCE_COLOR_CODES = "EXE.WAITING_SCREEN.FORCE_COLOR_NEW_STATUS.COLOR_CODES";//Mã màu chữ trạng thái yêu cầu khám là mới
        private const string TIMER_FOR_AUTO_LOAD_WAITING_SCREEN = "EXE.WAITING_SCREEN.TIMER_FOR_AUTO_LOAD_PATIENTS"; // thời gian tải lại màn hình chờ
        private const string TIMER_FOR_HIGHT_LIGHT_CALL_A_PATIENT = "EXE.WAITING_SCREEN.TIMER_FOR_HIGHT_LIGHT_CALL_A_PATIENT"; // thời gian nhấp nháy khi gọi bệnh nhân
        private const string WATING_EXAM_NAME_COLOR_CODES = "EXE.WAITING_SCREEN.WATING_EXAM_NAME_COLOR_CODES";//Mã màu danh sách khám
        private const string WATING_CLS_NAME_COLOR_CODES = "EXE.WAITING_SCREEN.WATING_CLS_NAME_COLOR_CODES";//Mã màu danh sách cls
        private const string TIMER_FOR_SET_DATA_TO_GRID_PATIENT = "EXE.WAITING_SCREEN.TIMER_FOR_SET_DATA_TO_GRID_PATIENT"; // thời gian set dữ liệu từ RAM vào danh sách bệnh nhân

        private static string callPatientDen;
        public static string CALL_PATIENT_DEN_STR
        {
            get
            {
                callPatientDen = GetName(CALL_PATIENT_DEN);
                return callPatientDen;
            }
            set
            {
                callPatientDen = value;
            }
        }
        private static List<int> pagingForceColorCodes;
        public static List<int> PAGING_FORCE_COLOR_CODES
        {
            get
            {
                pagingForceColorCodes = GetIds(PAGING_LABEL_FORCE_COLOR_CODES);
                return pagingForceColorCodes;
            }
            set
            {
                pagingForceColorCodes = value;
            }
        }
        private static string callPatientCoStt;
        public static string CALL_PATIENT_CO_STT_STR
        {
            get
            {
                callPatientCoStt = GetName(CALL_PATIENT_CO_STT);
                return callPatientCoStt;
            }
            set
            {
                callPatientCoStt = value;
            }
        }
        private static string organizationName;
        public static string ORGANIZATION_NAME
        {
            get
            {
                organizationName = GetName(THIS);
                return organizationName;
            }
            set
            {
                organizationName = value;
            }
        }
        private static List<int> userNameForceColorCodes;
        public static List<int> USER_NAME_FORCE_COLOR_CODES
        {
            get
            {
                userNameForceColorCodes = GetIds(USER_NAM_COLOR_CODES);
                return userNameForceColorCodes;
            }
            set
            {
                userNameForceColorCodes = value;
            }
        }
        private static List<int> watingExamForceColorCodes;
        public static List<int> WAITING_EXAM_FORCE_COLOR_CODES
        {
            get
            {
                watingExamForceColorCodes = GetIds(WATING_EXAM_NAME_COLOR_CODES);
                return watingExamForceColorCodes;
            }
            set
            {
                watingExamForceColorCodes = value;
            }
        }
        private static List<int> watingClsForceColorCodes;
        public static List<int> WAITING_CLS_FORCE_COLOR_CODES
        {
            get
            {
                watingClsForceColorCodes = GetIds(WATING_CLS_NAME_COLOR_CODES);
                return watingClsForceColorCodes;
            }
            set
            {
                watingClsForceColorCodes = value;
            }
        }
        private static List<int> gridPatientColorCodes;
        public static List<int> GRID_PATIENTS_BACK_COLOR_CODES
        {
            get
            {
                gridPatientColorCodes = GetIds(GRID_PATIENT_BACK_COLOR_CODES);
                return gridPatientColorCodes;
            }
            set
            {
                gridPatientColorCodes = value;
            }
        }
        private static List<int> gridPatientHeaderColorCodes;
        public static List<int> GRID_PATIENTS_HEADER_BACK_COLOR_CODES
        {
            get
            {
                gridPatientHeaderColorCodes = GetIds(GRID_PATIENT_HEADER_BACK_COLOR_CODES);
                return gridPatientHeaderColorCodes;
            }
            set
            {
                gridPatientColorCodes = value;
            }
        }
        private static List<int> gridPatientHeaderForceColorCodes;
        public static List<int> GRID_PATIENTS_HEADER_FORCE_COLOR_CODES
        {
            get
            {
                gridPatientHeaderForceColorCodes = GetIds(GRID_PATIENT_HEADER_FORCE_COLOR_CODES);
                return gridPatientHeaderForceColorCodes;
            }
            set
            {
                gridPatientHeaderForceColorCodes = value;
            }
        }
        private static List<int> gridPatientBodyForceColorCodes;
        public static List<int> GRID_PATIENTS_BODY_FORCE_COLOR_CODES
        {
            get
            {
                gridPatientBodyForceColorCodes = GetIds(GRID_PATIENT_BODY_FORCE_COLOR_CODES);
                return gridPatientBodyForceColorCodes;
            }
            set
            {
                gridPatientBodyForceColorCodes = value;
            }
        }
        private static List<int> newStatusForceColorCodes;
        public static List<int> NEW_STATUS_REQUEST_FORCE_COLOR_CODES
        {
            get
            {
                newStatusForceColorCodes = GetIds(NEW_STATUS_FORCE_COLOR_CODES);
                return newStatusForceColorCodes;
            }
            set
            {
                newStatusForceColorCodes = value;
            }
        }

        private static int timerForAutoLoadWaitingScreen;
        public static int TIMER_FOR_AUTO_LOAD_WAITING_SCREENS
        {
            get
            {
                timerForAutoLoadWaitingScreen = GetId(TIMER_FOR_AUTO_LOAD_WAITING_SCREEN);
                return timerForAutoLoadWaitingScreen;
            }
            set
            {
                timerForAutoLoadWaitingScreen = value;
            }
        }

        private static int timerForSetDataToGridPatient;
        public static int TIMER_FOR_SET_DATA_TO_GRID_PATIENTS
        {
            get
            {
                timerForSetDataToGridPatient = GetId(TIMER_FOR_SET_DATA_TO_GRID_PATIENT);
                return timerForSetDataToGridPatient;
            }
            set
            {
                timerForSetDataToGridPatient = value;
            }
        }

        private static int timerForHightLightCallAPatient;
        public static int TIMER_FOR_HIGHT_LIGHT_CALL_PATIENT
        {
            get
            {
                timerForHightLightCallAPatient = GetId(TIMER_FOR_HIGHT_LIGHT_CALL_A_PATIENT);
                return timerForHightLightCallAPatient;
            }
            set
            {
                timerForHightLightCallAPatient = value;
            }
        }
        private static List<int> parentBackColorCodes;
        public static List<int> PARENT_BACK_COLOR_CODES
        {
            get
            {
                parentBackColorCodes = GetIds(BACKGROUND_PARENT_COLOR_CODES);
                return parentBackColorCodes;
            }
            set
            {
                parentBackColorCodes = value;
            }
        }
        private static List<int> roomNameForceColorCodes;
        public static List<int> ROOM_NAME_FORCE_COLOR_CODES
        {
            get
            {
                roomNameForceColorCodes = GetIds(ROOM_NAM_COLOR_CODES);
                return roomNameForceColorCodes;
            }
            set
            {
                roomNameForceColorCodes = value;
            }
        }
        private static string callPatientMoiBenhNhan;
        public static string CALL_PATIENT_MOI_BENH_NHAN_STR
        {
            get
            {
                callPatientMoiBenhNhan = GetName(CALL_PATIENT_MOI_BENH_NHAN);
                return callPatientMoiBenhNhan;
            }
            set
            {
                callPatientMoiBenhNhan = value;
            }
        }
        private static List<int> organizationForceColorCodes;
        public static List<int> ORGANIZATION_FORCE_COLOR_CODES
        {
            get
            {
                organizationForceColorCodes = GetIds(ORGANIZATION_NAME_COLOR_CODES);
                return organizationForceColorCodes;
            }
            set
            {
                organizationForceColorCodes = value;
            }
        }
        private static string GetName(string code)
        {
            string result = "";
            try
            {
                string value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(code);
                if (String.IsNullOrEmpty(value)) return "";
                result = value;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = "";
            }
            return result;
        }
        private static List<int> GetIds(string code)
        {
            List<int> result = new List<int>();
            try
            {
                string value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(code);
                string pattern = ",";
                Regex myRegex = new Regex(pattern);
                string[] Codes = myRegex.Split(value);
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);

                if (!(Codes != null) || Codes.Length <= 0) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                foreach (var item in Codes) ///
                {
                    result.Add(Inventec.Common.TypeConvert.Parse.ToInt32(item));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
        private static int GetId(string code)
        {
            int result = 0;
            try
            {
                int value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>(code);
                result = value;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }
    }
}
