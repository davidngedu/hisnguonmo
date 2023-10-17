//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TYT.EFMODEL.DataModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class TYT_UNINFECT
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
        public string BRANCH_CODE { get; set; }
        public string PATIENT_CODE { get; set; }
        public string PERSON_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string VIR_PERSON_NAME { get; set; }
        public long DOB { get; set; }
        public Nullable<short> IS_HAS_NOT_DAY_DOB { get; set; }
        public string GENDER_NAME { get; set; }
        public string PERSON_ADDRESS { get; set; }
        public string CAREER_NAME { get; set; }
        public string ETHNIC_NAME { get; set; }
        public string ICD_CODE { get; set; }
        public string ICD_NAME { get; set; }
        public Nullable<long> DIAGNOSIS_TIME { get; set; }
        public string DIAGNOSIS_PLACE { get; set; }
        public string NOTE { get; set; }
    }
}
