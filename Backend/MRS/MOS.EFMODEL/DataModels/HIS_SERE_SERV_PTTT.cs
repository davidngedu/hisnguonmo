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
    
    public partial class HIS_SERE_SERV_PTTT
    {
        public HIS_SERE_SERV_PTTT()
        {
            this.HIS_SESE_PTTT_METHOD = new HashSet<HIS_SESE_PTTT_METHOD>();
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
        public long SERE_SERV_ID { get; set; }
        public Nullable<long> PTTT_GROUP_ID { get; set; }
        public Nullable<long> PTTT_METHOD_ID { get; set; }
        public Nullable<long> PTTT_CONDITION_ID { get; set; }
        public Nullable<long> PTTT_CATASTROPHE_ID { get; set; }
        public Nullable<long> PTTT_HIGH_TECH_ID { get; set; }
        public Nullable<long> PTTT_PRIORITY_ID { get; set; }
        public Nullable<long> PTTT_TABLE_ID { get; set; }
        public Nullable<long> EMOTIONLESS_METHOD_ID { get; set; }
        public Nullable<long> EMOTIONLESS_METHOD_SECOND_ID { get; set; }
        public Nullable<long> EMOTIONLESS_RESULT_ID { get; set; }
        public string ICD_CODE { get; set; }
        public string ICD_NAME { get; set; }
        public string ICD_SUB_CODE { get; set; }
        public string ICD_TEXT { get; set; }
        public string BEFORE_PTTT_ICD_CODE { get; set; }
        public string BEFORE_PTTT_ICD_NAME { get; set; }
        public string AFTER_PTTT_ICD_CODE { get; set; }
        public string AFTER_PTTT_ICD_NAME { get; set; }
        public Nullable<long> BLOOD_ABO_ID { get; set; }
        public Nullable<long> BLOOD_RH_ID { get; set; }
        public Nullable<long> DEATH_WITHIN_ID { get; set; }
        public string MANNER { get; set; }
        public Nullable<long> TDL_TREATMENT_ID { get; set; }
        public Nullable<long> ICD_ID__DELETE { get; set; }
        public Nullable<long> BEFORE_PTTT_ICD_ID__DELETE { get; set; }
        public string BEFORE_PTTT_ICD_TEXT__DELETE { get; set; }
        public Nullable<long> AFTER_PTTT_ICD_ID__DELETE { get; set; }
        public string AFTER_PTTT_ICD_TEXT__DELETE { get; set; }
        public Nullable<long> REAL_PTTT_METHOD_ID { get; set; }
        public Nullable<long> EYE_SURGRY_DESC_ID { get; set; }
        public Nullable<long> SURGERY_POSITION_ID { get; set; }
        public Nullable<long> SKIN_SURGERY_DESC_ID { get; set; }
        public string ICD_CM_CODE { get; set; }
        public string ICD_CM_NAME { get; set; }
        public string ICD_CM_SUB_CODE { get; set; }
        public string ICD_CM_TEXT { get; set; }
        public string WICK { get; set; }
        public string DRAINAGE { get; set; }
        public Nullable<long> DRAW_DATE { get; set; }
        public Nullable<long> CUT_SEWING_DATE { get; set; }
        public string OTHER { get; set; }
        public string PARTICIPANT_NUMBER { get; set; }
        public string PCI { get; set; }
        public string STENTING { get; set; }
        public string LOCATION_INTERVENTION { get; set; }
        public string REASON_INTERVENTION { get; set; }
        public string INTRODUCER { get; set; }
        public string GUIDING_CATHETER { get; set; }
        public string GUITE_WIRE { get; set; }
        public string BALLON { get; set; }
        public string STENT { get; set; }
        public string CONTRAST_AGENT { get; set; }
        public string INSTRUMENTS_OTHER { get; set; }
        public string STENT_NOTE { get; set; }
    
        public virtual HIS_BLOOD_ABO HIS_BLOOD_ABO { get; set; }
        public virtual HIS_BLOOD_RH HIS_BLOOD_RH { get; set; }
        public virtual HIS_EMOTIONLESS_METHOD HIS_EMOTIONLESS_METHOD { get; set; }
        public virtual HIS_EMOTIONLESS_METHOD HIS_EMOTIONLESS_METHOD1 { get; set; }
        public virtual HIS_EMOTIONLESS_RESULT HIS_EMOTIONLESS_RESULT { get; set; }
        public virtual HIS_EYE_SURGRY_DESC HIS_EYE_SURGRY_DESC { get; set; }
        public virtual HIS_PTTT_CATASTROPHE HIS_PTTT_CATASTROPHE { get; set; }
        public virtual HIS_PTTT_CONDITION HIS_PTTT_CONDITION { get; set; }
        public virtual HIS_PTTT_GROUP HIS_PTTT_GROUP { get; set; }
        public virtual HIS_PTTT_HIGH_TECH HIS_PTTT_HIGH_TECH { get; set; }
        public virtual HIS_PTTT_METHOD HIS_PTTT_METHOD { get; set; }
        public virtual HIS_PTTT_METHOD HIS_PTTT_METHOD1 { get; set; }
        public virtual HIS_PTTT_PRIORITY HIS_PTTT_PRIORITY { get; set; }
        public virtual HIS_PTTT_TABLE HIS_PTTT_TABLE { get; set; }
        public virtual HIS_SERE_SERV HIS_SERE_SERV { get; set; }
        public virtual HIS_SKIN_SURGERY_DESC HIS_SKIN_SURGERY_DESC { get; set; }
        public virtual ICollection<HIS_SESE_PTTT_METHOD> HIS_SESE_PTTT_METHOD { get; set; }
    }
}
