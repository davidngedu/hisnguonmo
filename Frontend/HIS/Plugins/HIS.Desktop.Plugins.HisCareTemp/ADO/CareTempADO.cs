﻿using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisCareTemp.ADO
{
    class CareTempADO : HIS_CARE_TEMP
    {
        public bool IS_ADD_MEDICINE { get; set; }
        public bool IS_MEDICINE { get; set; }
        public bool IS_TEST { get; set; }
        public bool PUBLIC { get; set; }

        public CareTempADO(HIS_CARE_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<CareTempADO>(this, data);

                    if (data.HAS_ADD_MEDICINE.HasValue && data.HAS_ADD_MEDICINE.Value == 1)
                    {
                        IS_ADD_MEDICINE = true;
                    }

                    if (data.HAS_MEDICINE.HasValue && data.HAS_MEDICINE.Value == 1)
                    {
                        IS_MEDICINE = true;
                    }

                    if (data.HAS_TEST.HasValue && data.HAS_TEST.Value == 1)
                    {
                        IS_TEST = true;
                    }

                    if (data.IS_PUBLIC.HasValue && data.IS_PUBLIC.Value == 1)
                    {
                        PUBLIC = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
