﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ReportCountTreatment.ADO
{
    class DepartmentADO
    {
        public string DEPARTMENT_NAME { get; set; }
        public long DEPARTMENT_ID { get; set; }

        public long COUNT_OLD { get; set; }
        public long COUNT_IN { get; set; }
        public long COUNT_OUT { get; set; }
        public long COUNT_CURR { get; set; }
        public long COUNT_DEATH { get; set; }
        public long COUNT_TRAN { get; set; }
        public long COUNT_CC { get; set; }
        public long COUNT_6T { get; set; }
        public long COUNT_80T { get; set; }
        public long COUNT_FEMALE { get; set; }
        public long COUNT_END_DEPARTMENT { get; set; }

        internal List<TreatmentADO> TreatmentOld { get; set; }
        internal List<TreatmentADO> TreatmentIn { get; set; }
        internal List<TreatmentADO> TreatmentOut { get; set; }
        internal List<TreatmentADO> TreatmentCurr { get; set; }
        internal List<TreatmentADO> TreatmentDeath { get; set; }
        internal List<TreatmentADO> TreatmentTran { get; set; }
        internal List<TreatmentADO> TreatmentCc { get; set; }
        internal List<TreatmentADO> TreatmentTreEm { get; set; }
        internal List<TreatmentADO> TreatmentCaoTuoi { get; set; }
        internal List<TreatmentADO> TreatmentFemale { get; set; }
        internal List<TreatmentADO> TreatmentEndDepartment { get; set; }
    }
}
