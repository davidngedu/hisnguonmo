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
    
    public partial class ACS_APPLICATION
    {
        public ACS_APPLICATION()
        {
            this.ACS_APPLICATION_ROLE = new HashSet<ACS_APPLICATION_ROLE>();
            this.ACS_CONTROL = new HashSet<ACS_CONTROL>();
            this.ACS_MODULE = new HashSet<ACS_MODULE>();
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
        public string APPLICATION_CODE { get; set; }
        public string APPLICATION_NAME { get; set; }
        public string ALLOW_VERSION { get; set; }
        public Nullable<short> IS_CHECK_VERSION { get; set; }
        public string SMS_URL { get; set; }
        public string SMS_ACTIVE_TEMP { get; set; }
        public string SMS_CHANGE_PASS_TEMP { get; set; }
        public Nullable<short> IS_VERIFY_OTP { get; set; }
        public Nullable<short> IS_LICENSE_ISSUED { get; set; }
    
        public virtual ICollection<ACS_APPLICATION_ROLE> ACS_APPLICATION_ROLE { get; set; }
        public virtual ICollection<ACS_CONTROL> ACS_CONTROL { get; set; }
        public virtual ICollection<ACS_MODULE> ACS_MODULE { get; set; }
    }
}
