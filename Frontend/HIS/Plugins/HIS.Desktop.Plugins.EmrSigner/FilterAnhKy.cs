﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EmrSigner
{
    class FilterAnhKy
    {
        public long ID { get; set; }
        public string FilterTypeName { get; set; }

        public FilterAnhKy(long id, string filterTypeName)
        {
            this.ID = id;
            this.FilterTypeName = filterTypeName;
        }
    }
}
