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
    
    public partial class LIS_SAMPLE_SERVICE_STT
    {
        public LIS_SAMPLE_SERVICE_STT()
        {
            this.LIS_SAMPLE_SERVICE = new HashSet<LIS_SAMPLE_SERVICE>();
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
        public string SAMPLE_SERVICE_STT_CODE { get; set; }
        public string SAMPLE_SERVICE_STT_NAME { get; set; }
    
        public virtual ICollection<LIS_SAMPLE_SERVICE> LIS_SAMPLE_SERVICE { get; set; }
    }
}
