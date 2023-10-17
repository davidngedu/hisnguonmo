﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisCheckBeforeTransfusionBlood.Base
{
    public class ExpBloodADO
    {
        public long? ExpMestBloodId { get; set; }
        public long BloodTypeId { get; set; }
        public long? ServiceId { get; set; }
        public long HisExpId { get; set; }
        public string Key { get; set; }
        public string ParentKey { get; set; }
        public long SERVICE_AMOUNT { get; set; }
        public string BLOOD_CODE { get; set; }

        public string SERVICE_BLOOD_CODE { get; set; }
        public string SERVICE_BLOOD_NAME { get; set; }
        public decimal? VOLUME { get; set; }
        public long AMOUNT { get; set; }
        public long? NUM_ORDER { get; set; }
        public long BLOOD_TYPE_ORDER { get; set; }
        public long? EXPIRED_DATE { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public string GIVE_NAME { get; set; }
        public string GIVE_CODE { get; set; }
        public string BLOOD_ABO_CODE { get; set; }
        public string BLOOD_HR_CODE { get; set; }
        public string BLOOD_ABO_HR_CODE { get; set; }

        public string PATIENT_BLOOD_ABO_CODE { get; set; }
        public string PATIENT_BLOOD_RH_CODE { get; set; }
        public string PUC { get; set; }
        public string SCANGEL_GELCARD { get; set; }
        public string COOMBS { get; set; }

        public string TEST_TUBE { get; set; }
        public long? SALT_ENVI { get; set; }
        public long? ANTI_GLOBULIN { get; set; }

        public string TEST_TUBE_TWO { get; set; }
        public long? SALT_ENVI_TWO { get; set; }
        public long? ANTI_GLOBULIN_TWO { get; set; }
        public bool is_Sevrvice_Blood { get; set; }
        public decimal? AC_SELF_ENVIDENCE { get; set; }
        public decimal? AC_SELF_ENVIDENCE_SECOND { get; set; }
        public string SERVICE_RESULT { get; set; }
        public long EXP_MEST_ID { get; set; }
    }
}
