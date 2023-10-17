﻿using EMR.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisTreatmentFile.Base
{
    class AttackADO : HIS_TREATMENT_FILE
    {
        public string FILE_NAME { get; set; }
        public bool IsChecked { get; set; }
        public string Base64Data { get; set; }
        public long DocumentId { get; set; }
        public string Extension { get; set; }
        public string FullName { get; set; }
        public string Url { get; set; }
        public int Dem { get; set; }
        public int SttPdf { get; set; }
        public bool IsFss { get; set; }
        public System.Drawing.Image image { get; set; }

        public AttackADO() { }
    }
}
