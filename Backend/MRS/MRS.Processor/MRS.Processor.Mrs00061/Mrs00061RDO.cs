using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Text; 
using System.Threading.Tasks; 

namespace MRS.Processor.Mrs00061
{
    class Mrs00061RDO
    {
        public string SERVICE_TYPE_NAME { get;  set;  }
        public string SERVICE_CODE { get;  set;  }
        public string SERVICE_NAME { get;  set;  }
        public string SERVICE_UNIT_NAME { get;  set;  }

        public decimal AMOUNT { get;  set;  }
        public decimal VIR_TOTAL_PATIENT_PRICE { get;  set;  }
        public decimal VIR_TOTAL_HEIN_PRICE { get;  set;  }
    }
}
