using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.Threading.Tasks; 

namespace MRS.Processor.Mrs00253
{
    public class Mrs00253Filter
    {
        public long TIME_FROM { get;  set;  }
        public long TIME_TO { get;  set;  }
        public List<long> DEPARTMENT_IDs { get;  set;  }

    }
}
