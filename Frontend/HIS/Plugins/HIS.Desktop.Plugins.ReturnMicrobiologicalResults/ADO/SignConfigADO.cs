﻿using EMR.TDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ReturnMicrobiologicalResults.ADO
{
    class SignConfigADO
    {
        public bool IsSignParanel { get; set; }
        public List<SignTDO> listSign { get; set; }
    }
}
