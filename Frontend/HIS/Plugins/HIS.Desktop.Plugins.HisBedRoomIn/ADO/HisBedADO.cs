﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisBedRoomIn.ADO
{
    class HisBedADO : MOS.EFMODEL.DataModels.V_HIS_BED
    {
        public long AMOUNT { get; set; }
        public string AMOUNT_STR { get; set; }
        public long IsKey { get; set; }

        public long? BILL_PATIENT_TYPE_ID { get; set; }

        public HisBedADO() { }

        public HisBedADO(MOS.EFMODEL.DataModels.V_HIS_BED data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisBedADO>(this, data);
                    this.AMOUNT_STR = 0 + "/" + this.MAX_CAPACITY;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
