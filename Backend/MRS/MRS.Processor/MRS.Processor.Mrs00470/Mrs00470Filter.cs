using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.Threading.Tasks; 

namespace MRS.Processor.Mrs00470
{
    public class Mrs00470Filter
    {
        public long? BRANCH_ID { get;  set;  }

        public long TIME_FROM { get;  set;  }
        public long TIME_TO { get;  set;  }


        public List<long> BRANCH_IDs { get; set; }

        public List<long> END_DEPARTMENT_IDs { get; set; }

        public List<long> EXECUTE_DEPARTMENT_IDs { get; set; }

        public List<long> REQUEST_DEPARTMENT_IDs { get; set; }
    }
}
