
using System.Collections.Generic;
namespace MOS.Filter
{
    public class HisServiceMatyFilter : FilterBase
    {
        public long? SERVICE_ID { get; set; }
        public long? MATERIAL_TYPE_ID { get; set; }
        public List<long> SERVICE_IDs { get; set; }

        public HisServiceMatyFilter()
            : base()
        {
        }
    }
}
