//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MOS.EFMODEL.DataModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class HIS_CARE_TYPE
    {
        public HIS_CARE_TYPE()
        {
            this.HIS_CARE_DETAIL = new HashSet<HIS_CARE_DETAIL>();
            this.HIS_CARE_TEMP_DETAIL = new HashSet<HIS_CARE_TEMP_DETAIL>();
        }
    
        public long ID { get; set; }
        public Nullable<long> CREATE_TIME { get; set; }
        public Nullable<long> MODIFY_TIME { get; set; }
        public string CREATOR { get; set; }
        public string MODIFIER { get; set; }
        public string APP_CREATOR { get; set; }
        public string APP_MODIFIER { get; set; }
        public Nullable<short> IS_ACTIVE { get; set; }
        public Nullable<short> IS_DELETE { get; set; }
        public string GROUP_CODE { get; set; }
        public string CARE_TYPE_CODE { get; set; }
        public string CARE_TYPE_NAME { get; set; }
    
        public virtual ICollection<HIS_CARE_DETAIL> HIS_CARE_DETAIL { get; set; }
        public virtual ICollection<HIS_CARE_TEMP_DETAIL> HIS_CARE_TEMP_DETAIL { get; set; }
    }
}
