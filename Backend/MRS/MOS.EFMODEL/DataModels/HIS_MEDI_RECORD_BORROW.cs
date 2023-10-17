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
    
    public partial class HIS_MEDI_RECORD_BORROW
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
        public long MEDI_RECORD_ID { get; set; }
        public string BORROW_LOGINNAME { get; set; }
        public string BORROW_USERNAME { get; set; }
        public long DEPARTMENT_ID { get; set; }
        public Nullable<long> GIVE_TIME { get; set; }
        public Nullable<long> GIVE_DATE { get; set; }
        public string GIVER_LOGINNAME { get; set; }
        public string GIVER_USERNAME { get; set; }
        public Nullable<long> RECEIVE_TIME { get; set; }
        public Nullable<long> RECEIVE_DATE { get; set; }
        public string RECEIVER_LOGINNAME { get; set; }
        public string RECEIVER_USERNAME { get; set; }
        public Nullable<long> APPOINTMENT_TIME { get; set; }
        public Nullable<long> APPOINTMENT_DATE { get; set; }
        public Nullable<decimal> VIR_GIVE_MONTH { get; set; }
        public Nullable<decimal> VIR_RECEIVE_MONTH { get; set; }
        public Nullable<decimal> VIR_APPOINTMENT_MONTH { get; set; }
        public string BORROW_PHONE { get; set; }
    
        public virtual HIS_DEPARTMENT HIS_DEPARTMENT { get; set; }
        public virtual HIS_MEDI_RECORD HIS_MEDI_RECORD { get; set; }
    }
}
