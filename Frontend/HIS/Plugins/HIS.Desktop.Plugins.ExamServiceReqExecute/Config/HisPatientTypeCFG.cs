﻿using Inventec.Common.Logging;
using Inventec.Core;
using MOS.Filter;
using System;
using System.Linq;
using System.Collections.Generic;
using Inventec.Common.LocalStorage.SdaConfig;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute.Config
{
    public class HisPatientTypeCFG
    {
        private const string SDA_CONFIG__PATIENT_TYPE_CODE__IS_FREE = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.IS_FREE";//Doi tuong mien phi
        private const string SDA_CONFIG__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string SDA_CONFIG__PATIENT_TYPE_CODE__KSK = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK";//Doi tuong khám sức khỏe DN
        private const string SDA_CONFIG__PATIENT_TYPE_CODE__FEE = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";
        private const string SDA_CONFIG__PATIENT_TYPE_CODE__BILL_INVOICE = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BILL_INVOICE";

        private const string SDA_CONFIG__PATIENT_TYPE_CODE__SERVICE = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.SERVICE";
        private const string SDA_CONFIG__PATIENT_TYPE_CODE__THUPHI = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.THUPHI";

        private static long patientTypeIdIsService;
        public static long PATIENT_TYPE_ID__IS_SERIVCE
        {
            get
            {
                if (patientTypeIdIsService == 0)
                {
                    patientTypeIdIsService = GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.SERVICE"));
                }
                return patientTypeIdIsService;
            }
            set
            {
                patientTypeIdIsService = value;
            }
        }

        private static long patientTypeIdIsThuPhi;
        public static long PATIENT_TYPE_ID__IS_THUPHI
        {
            get
            {
                if (patientTypeIdIsThuPhi == 0)
                {
                    patientTypeIdIsThuPhi = GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.THUPHI"));
                }
                return patientTypeIdIsThuPhi;
            }
            set
            {
                patientTypeIdIsThuPhi = value;
            }
        }

        private static long patientTypeIdIsFee;
        public static long PATIENT_TYPE_ID__IS_FEE
        {
            get
            {
                if (patientTypeIdIsFee == 0)
                {
                    patientTypeIdIsFee = GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE"));
                }
                return patientTypeIdIsFee;
            }
            set
            {
                patientTypeIdIsFee = value;
            }
        }


        private static long patientTypeIdIsHein;
        public static long PATIENT_TYPE_ID__BHYT
        {
            get
            {
                if (patientTypeIdIsHein == 0)
                {
                    patientTypeIdIsHein = GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT"));
                }
                return patientTypeIdIsHein;
            }
            set
            {
                patientTypeIdIsHein = value;
            }
        }

        private static long patientTypeIdIsFree;
        public static long PATIENT_TYPE_ID__IS_FREE
        {
            get
            {
                if (patientTypeIdIsFree == 0)
                {
                    patientTypeIdIsFree = GetId("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.IS_FREE");
                }
                return patientTypeIdIsFree;
            }
            set
            {
                patientTypeIdIsFree = value;
            }
        }

        private static long patientTypeIdKsk;
        public static long PATIENT_TYPE_ID__KSK
        {
            get
            {
                if (patientTypeIdKsk == 0)
                {
                    patientTypeIdKsk = GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.SERVICE"));
                }
                return patientTypeIdKsk;
            }
            set
            {
                patientTypeIdKsk = value;
            }
        }

        private static long patientTypeIdBillInvoice;
        public static long PATIENT_TYPE_ID__BILL_INVOICE
        {
            get
            {
                if (patientTypeIdBillInvoice == 0)
                {
                    patientTypeIdBillInvoice = GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BILL_INVOICE"));
                }
                return patientTypeIdBillInvoice;
            }
            set
            {
                patientTypeIdBillInvoice = value;
            }
        }

        //Code
        private static string patientTypeCodeIsHein;
        public static string PATIENT_TYPE_CODE__BHYT
        {
            get
            {
                if (String.IsNullOrEmpty(patientTypeCodeIsHein))
                {
                    patientTypeCodeIsHein = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT");
                }
                return patientTypeCodeIsHein;
            }
            set
            {
                patientTypeCodeIsHein = value;
            }
        }

        private static string patientTypeCodeIsFree;
        public static string PATIENT_TYPE_CODE__IS_FREE
        {
            get
            {
                if (String.IsNullOrEmpty(patientTypeCodeIsFree))
                {
                    patientTypeCodeIsFree = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.IS_FREE");
                }
                return patientTypeCodeIsFree;
            }
            set
            {
                patientTypeCodeIsFree = value;
            }
        }

        private static string patientTypeCodeKsk;
        public static string PATIENT_TYPE_CODE__KSK
        {
            get
            {
                if (String.IsNullOrEmpty(patientTypeCodeKsk))
                {
                    patientTypeCodeKsk = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK");
                }
                return patientTypeCodeKsk;
            }
            set
            {
                patientTypeCodeKsk = value;
            }
        }

        private static long GetId(string code)
        {
            long result = 0;
            try
            {
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                result = data.ID;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
                result = 0;
            }
            return result;
        }
    }
}
