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
    
    public partial class HIS_TRANSFUSION_SUM
    {
        public HIS_TRANSFUSION_SUM()
        {
            this.HIS_TRANSFUSION = new HashSet<HIS_TRANSFUSION>();
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
        public long TREATMENT_ID { get; set; }
        public Nullable<long> DEPARTMENT_ID { get; set; }
        public Nullable<long> ROOM_ID { get; set; }
        public long EXP_MEST_BLOOD_ID { get; set; }
        public Nullable<long> START_TIME { get; set; }
        public Nullable<long> FINISH_TIME { get; set; }
        public Nullable<long> NUM_ORDER { get; set; }
        public string EXECUTE_LOGINNAME { get; set; }
        public string EXECUTE_USERNAME { get; set; }
        public string ICD_CODE { get; set; }
        public string ICD_NAME { get; set; }
        public string ICD_SUB_CODE { get; set; }
        public string ICD_TEXT { get; set; }
        public Nullable<decimal> TRANSFUSION_VOLUME { get; set; }
        public string NOTE { get; set; }
    
        public virtual HIS_DEPARTMENT HIS_DEPARTMENT { get; set; }
        public virtual HIS_EXP_MEST_BLOOD HIS_EXP_MEST_BLOOD { get; set; }
        public virtual HIS_ROOM HIS_ROOM { get; set; }
        public virtual ICollection<HIS_TRANSFUSION> HIS_TRANSFUSION { get; set; }
        public virtual HIS_TREATMENT HIS_TREATMENT { get; set; }
    }
}
