﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisMedicalContractCreate.ADO
{
    public class MetyMatyADO : MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE
    {
        public long CONTRACT_MATY_METY_ID { get; set; }

        public long? BID_ID { get; set; }
        public long? BID_METY_MATY_ID { get; set; }
        public long? EXPIRED_DATE { get; set; }
        public long? IMP_EXPIRED_DATE { get; set; }
        public string BID_GROUP_CODE { get; set; }

        public long? MonthLifespan { get; set; }
        public long? DayLifespan { get; set; }
        public long? HourLifespan { get; set; }

        public decimal? ImpVatRatio { get; set; }
        public decimal? CONTRACT_PRICE { get; set; }
        public decimal? AMOUNT { get; set; }

        //import
        public string IS_MEDICINE { get; set; }
        public string EXPIRED_DATE_STR { get; set; }

        //
        public string BID_NUMBER { get; set; }
        public string NOTE { get; set; }

    }
}
