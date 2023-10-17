﻿using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisImportBloodType.ADO
{
    class BloodTypeAdo : V_HIS_BLOOD_TYPE
    {
        public long IdRow { get; set; }
        public string BLOOD_GROUP_CODE { get; set; }
        public string BLOOD_GROUP_NAME { get; set; }

        public string ALERT_EXPIRED_DATE_STR { get; set; }
        public string HEIN_LIMIT_PRICE_STR { get; set; }
        public string HEIN_LIMIT_PRICE_IN_TIME_STR { get; set; }
        public string HEIN_LIMIT_PRICE_INTR_TIME_STR { get; set; }
        public string HEIN_LIMIT_PRICE_OLD_STR { get; set; }
        public string HEIN_LIMIT_RATIO_STR { get; set; }
        public string HEIN_LIMIT_RATIO_OLD_STR { get; set; }
        public string IMP_PRICE_STR { get; set; }
        public string IMP_VAT_RATIO_STR { get; set; }
        public string INTERNAL_PRICE_STR { get; set; }
        public string PARENT_CODE { get; set; }
        public string VOLUME_STR { get; set; }
        public string NUM_ORDER_STR { get; set; }
        public string OUT_PARENT_FEE { get; set; }

        public long? HEIN_LIMIT_PRICE_IN_TIME { get; set; }
        public long? HEIN_LIMIT_PRICE_INTR_TIME { get; set; }
        public decimal? HEIN_LIMIT_PRICE { get; set; }
        public decimal? HEIN_LIMIT_PRICE_OLD { get; set; }
        public decimal? HEIN_LIMIT_RATIO { get; set; }
        public decimal? HEIN_LIMIT_RATIO_OLD { get; set; }

        public string ERROR { get; set; }
    }
}
