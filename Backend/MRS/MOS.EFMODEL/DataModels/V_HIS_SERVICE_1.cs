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
    
    public partial class V_HIS_SERVICE_1
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
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public long SERVICE_TYPE_ID { get; set; }
        public long SERVICE_UNIT_ID { get; set; }
        public Nullable<long> PARENT_ID { get; set; }
        public Nullable<short> IS_LEAF { get; set; }
        public Nullable<long> NUM_ORDER { get; set; }
        public Nullable<long> HEIN_SERVICE_TYPE_ID { get; set; }
        public string HEIN_SERVICE_BHYT_CODE { get; set; }
        public string HEIN_SERVICE_BHYT_NAME { get; set; }
        public string HEIN_ORDER { get; set; }
        public Nullable<short> IS_USE_SERVICE_HEIN { get; set; }
        public Nullable<decimal> HEIN_LIMIT_PRICE_OLD { get; set; }
        public Nullable<decimal> HEIN_LIMIT_RATIO_OLD { get; set; }
        public Nullable<decimal> HEIN_LIMIT_PRICE { get; set; }
        public Nullable<decimal> HEIN_LIMIT_RATIO { get; set; }
        public Nullable<long> HEIN_LIMIT_PRICE_IN_TIME { get; set; }
        public Nullable<long> HEIN_LIMIT_PRICE_INTR_TIME { get; set; }
        public string SPECIALITY_CODE { get; set; }
        public Nullable<short> IS_MULTI_REQUEST { get; set; }
        public Nullable<decimal> MAX_EXPEND { get; set; }
        public Nullable<short> BILL_OPTION { get; set; }
        public Nullable<long> BILL_PATIENT_TYPE_ID { get; set; }
        public Nullable<short> IS_OUT_PARENT_FEE { get; set; }
        public Nullable<long> PTTT_GROUP_ID { get; set; }
        public Nullable<long> PTTT_METHOD_ID { get; set; }
        public Nullable<long> ICD_CM_ID { get; set; }
        public Nullable<decimal> COGS { get; set; }
        public Nullable<decimal> ESTIMATE_DURATION { get; set; }
        public Nullable<long> REVENUE_DEPARTMENT_ID { get; set; }
        public Nullable<long> PACKAGE_ID { get; set; }
        public Nullable<decimal> PACKAGE_PRICE { get; set; }
        public Nullable<long> NUMBER_OF_FILM { get; set; }
        public string PACS_TYPE_CODE { get; set; }
        public Nullable<long> MIN_DURATION { get; set; }
        public Nullable<long> EXE_SERVICE_MODULE_ID { get; set; }
        public Nullable<long> GENDER_ID { get; set; }
        public Nullable<long> AGE_FROM { get; set; }
        public Nullable<long> AGE_TO { get; set; }
        public Nullable<long> RATION_GROUP_ID { get; set; }
        public string RATION_SYMBOL { get; set; }
        public string NOTICE { get; set; }
        public Nullable<long> CAPACITY { get; set; }
        public Nullable<short> IS_ALLOW_EXPEND { get; set; }
        public Nullable<short> IS_NO_HEIN_LIMIT_FOR_SPECIAL { get; set; }
        public Nullable<short> IS_KIDNEY { get; set; }
        public Nullable<short> IS_SPECIFIC_HEIN_PRICE { get; set; }
        public Nullable<short> IS_OTHER_SOURCE_PAID { get; set; }
        public Nullable<short> IS_ANTIBIOTIC_RESISTANCE { get; set; }
        public Nullable<long> DIIM_TYPE_ID { get; set; }
        public Nullable<long> FUEX_TYPE_ID { get; set; }
        public Nullable<long> TAX_RATE_TYPE { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<short> IS_SPLIT_SERVICE_REQ { get; set; }
        public Nullable<short> IS_ENABLE_ASSIGN_PRICE { get; set; }
        public Nullable<short> IS_AUTO_FINISH { get; set; }
        public string PROCESS_CODE { get; set; }
        public Nullable<long> TEST_TYPE_ID { get; set; }
        public Nullable<short> IS_OUT_OF_DRG { get; set; }
        public Nullable<short> IS_CONDITIONED { get; set; }
        public Nullable<long> MIN_PROCESS_TIME { get; set; }
        public Nullable<short> IS_NOT_CHANGE_BILL_PATY { get; set; }
        public Nullable<short> IS_SPLIT_SERVICE { get; set; }
        public Nullable<long> OTHER_PAY_SOURCE_ID { get; set; }
        public string OTHER_PAY_SOURCE_ICDS { get; set; }
        public string BODY_PART_IDS { get; set; }
        public string TESTING_TECHNIQUE { get; set; }
        public Nullable<short> IS_OUT_OF_MANAGEMENT { get; set; }
        public Nullable<short> MUST_BE_CONSULTED { get; set; }
        public Nullable<long> SUIM_INDEX_ID { get; set; }
        public string ATTACH_ASSIGN_PRINT_TYPE_CODE { get; set; }
        public Nullable<long> TEST_COVID_TYPE { get; set; }
        public Nullable<long> MAX_PROCESS_TIME { get; set; }
        public Nullable<short> IS_DISALLOWANCE_NO_EXECUTE { get; set; }
        public Nullable<short> IS_NOT_SHOW_TRACKING { get; set; }
        public Nullable<long> FILM_SIZE_ID { get; set; }
        public Nullable<short> IS_AUTO_EXPEND { get; set; }
        public Nullable<short> IS_BLOCK_DEPARTMENT_TRAN { get; set; }
        public Nullable<long> WARNING_SAMPLING_TIME { get; set; }
        public string APPLIED_PATIENT_TYPE_IDS { get; set; }
        public Nullable<long> DEFAULT_PATIENT_TYPE_ID { get; set; }
        public Nullable<long> MAX_TOTAL_PROCESS_TIME { get; set; }
        public string SERVICE_TYPE_CODE { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public Nullable<long> SETY_EXE_SERVICE_MODULE_ID { get; set; }
        public Nullable<short> IS_AUTO_SPLIT_REQ { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string PTTT_GROUP_CODE { get; set; }
        public string PTTT_GROUP_NAME { get; set; }
        public string PTTT_METHOD_CODE { get; set; }
        public string PTTT_METHOD_NAME { get; set; }
        public string ICD_CM_CODE { get; set; }
        public string ICD_CM_NAME { get; set; }
    }
}
