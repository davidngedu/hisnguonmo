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
    
    public partial class ACS_OTP
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
        public string OTP_CODE { get; set; }
        public short OTP_TYPE { get; set; }
        public Nullable<long> EXPIRE_TIME { get; set; }
        public string LOGINAME { get; set; }
        public string USERNAME { get; set; }
        public long OTP_TYPE_ID { get; set; }
        public string EMAIL { get; set; }
        public string MOBILE { get; set; }
        public Nullable<decimal> VIR_EXPIRE_DATE { get; set; }
    
        public virtual ACS_OTP_TYPE ACS_OTP_TYPE { get; set; }
    }
}
