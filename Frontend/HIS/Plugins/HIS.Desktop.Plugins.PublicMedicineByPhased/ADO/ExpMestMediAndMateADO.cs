﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PublicMedicineByPhased.ADO
{
    public class ExpMestMediAndMateADO : MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE
    {
        //public string MEDI_MATE_TYPE_NAME { get; set; }
        //public string MEDI_MATE_TYPE_CODE { get; set; }
        //public string MEDI_MATE_TYPE_ID { get; set; }
        //public string SERVICE_UNIT_NAME { get; set; }
        //public string SERVICE_UNIT_CODE { get; set; }
        //public long SERVICE_UNIT_ID { get; set; }
        //public decimal AMOUNT { get; set; }
        //public decimal? PRICE { get; set; }
        //public string DESCRIPTION { get; set; }

        public long Service_Type_Id { get; set; }
        public short? IS_CHEMICAL_SUBSTANCE { set; get; }
        public long INTRUCTION_DATE { get; set; }
        public long INTRUCTION_TIME { get; set; }
       // public long REQ_DEPARTMENT_ID { get; set; }
        public long TYPE { get; set; }
    }
}
