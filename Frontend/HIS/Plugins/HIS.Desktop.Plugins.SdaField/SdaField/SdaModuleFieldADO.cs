﻿using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.SdaField.SdaField
{
    class SdaModuleFieldADO : SDA_MODULE_FIELD
    {
        public bool IS_VISIBLE_STR { get; set; }

        public SdaModuleFieldADO(SDA_MODULE_FIELD data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<SdaModuleFieldADO>(this, data);
            if (data.IS_VISIBLE == 1)
                this.IS_VISIBLE_STR = true;
            else
                this.IS_VISIBLE_STR = false;
        }
    }
}
