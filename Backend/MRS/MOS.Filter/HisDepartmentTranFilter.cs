
using System.Collections.Generic;
namespace MOS.Filter
{
    public class HisDepartmentTranFilter : FilterBase
    {
        public long? DEPARTMENT_ID { get; set; }
        public List<long> DEPARTMENT_IDs { get; set; }
        public long? TREATMENT_ID { get; set; }
        public List<long> TREATMENT_IDs { get; set; }
        public long? DEPARTMENT_IN_TIME_TO { get; set; }
        public long? DEPARTMENT_IN_TIME_FROM { get; set; }
        public long? PREVIOUS_ID { get; set; }
        public List<long> PREVIOUS_IDs { get; set; }
        public HisDepartmentTranFilter()
            : base()
        {
        }
    }
}
