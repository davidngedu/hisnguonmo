﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRS.Processor.Mrs00507
{
    public class Mrs00507Filter
    {
        public long TIME_FROM { get; set; }
        public long TIME_TO { get; set; }
        public long? MEDI_STOCK_ID { get; set; }
        public List<long> EXP_MEST_TYPE_IDs { get; set; }
        public long? BID_ID { get; set; }
        public bool? IS_MEDICINE { get; set; }
        public bool? IS_MATERIAL { get; set; }
        public bool? IS_CHEMISTRY { get; set; }
    }
}
