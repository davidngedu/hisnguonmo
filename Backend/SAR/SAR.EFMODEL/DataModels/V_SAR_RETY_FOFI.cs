//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAR.EFMODEL.DataModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class V_SAR_RETY_FOFI
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
        public long REPORT_TYPE_ID { get; set; }
        public long FORM_FIELD_ID { get; set; }
        public Nullable<long> NUM_ORDER { get; set; }
        public string DESCRIPTION { get; set; }
        public string JSON_OUTPUT { get; set; }
        public Nullable<short> IS_REQUIRE { get; set; }
        public Nullable<short> WIDTH_RATIO { get; set; }
        public Nullable<long> HEIGHT { get; set; }
        public Nullable<long> ROW_COUNT { get; set; }
        public Nullable<long> COLUMN_COUNT { get; set; }
        public Nullable<long> ROW_INDEX { get; set; }
        public string REPORT_TYPE_CODE { get; set; }
        public string REPORT_TYPE_NAME { get; set; }
        public string FORM_FIELD_CODE { get; set; }
        public string FORM_FIELD_DESCRIPTION { get; set; }
    }
}
