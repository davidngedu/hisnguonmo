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
    
    public partial class V_HIS_PTTT_CALENDAR
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
        public string PTTT_CALENDAR_CODE { get; set; }
        public long DEPARTMENT_ID { get; set; }
        public long TIME_FROM { get; set; }
        public long TIME_TO { get; set; }
        public Nullable<long> APPROVAL_TIME { get; set; }
        public string APPROVAL_LOGINNAME { get; set; }
        public string APPROVAL_USERNAME { get; set; }
        public Nullable<decimal> VIR_DATE_FROM { get; set; }
        public Nullable<decimal> VIR_DATE_TO { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public string DEPARTMENT_NAME { get; set; }
    }
}
