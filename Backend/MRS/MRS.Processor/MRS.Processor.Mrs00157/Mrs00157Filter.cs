using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.Threading.Tasks;

namespace MRS.Processor.Mrs00157
{
    public class Mrs00157Filter
    {
        public long IMP_TIME_FROM { get;  set;  }
        public long IMP_TIME_TO { get;  set;  }
        public List<long> MEDI_STOCK_IDs { get; set; }
 
    }
}
