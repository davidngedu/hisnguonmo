//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACS.EFMODEL.DataModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class ACS_AUTHOR_SYSTEM
    {
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
        public string AUTHOR_SYSTEM_CODE { get; set; }
        public string AUTHOR_SYSTEM_NAME { get; set; }
        public string SERCURE_KEY { get; set; }
        public decimal AUTHOR_SYSTEM_TYPE_ID { get; set; }
        public string WEB_ADDRESS { get; set; }
    }
}
