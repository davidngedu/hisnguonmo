//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LIS.EFMODEL.DataModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class LIS_BACTERIUM
    {
        public LIS_BACTERIUM()
        {
            this.LIS_BAC_ANTIBIOTIC = new HashSet<LIS_BAC_ANTIBIOTIC>();
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
        public string BACTERIUM_CODE { get; set; }
        public string BACTERIUM_NAME { get; set; }
        public long BACTERIUM_FAMILY_ID { get; set; }
    
        public virtual ICollection<LIS_BAC_ANTIBIOTIC> LIS_BAC_ANTIBIOTIC { get; set; }
        public virtual LIS_BACTERIUM_FAMILY LIS_BACTERIUM_FAMILY { get; set; }
    }
}
