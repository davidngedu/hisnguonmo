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
    
    public partial class V_HIS_EXP_MEST
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
        public string EXP_MEST_CODE { get; set; }
        public long EXP_MEST_TYPE_ID { get; set; }
        public long EXP_MEST_STT_ID { get; set; }
        public long MEDI_STOCK_ID { get; set; }
        public string REQ_LOGINNAME { get; set; }
        public string REQ_USERNAME { get; set; }
        public long REQ_ROOM_ID { get; set; }
        public long REQ_DEPARTMENT_ID { get; set; }
        public long CREATE_DATE { get; set; }
        public string LAST_EXP_LOGINNAME { get; set; }
        public string LAST_EXP_USERNAME { get; set; }
        public Nullable<long> LAST_EXP_TIME { get; set; }
        public Nullable<long> FINISH_TIME { get; set; }
        public Nullable<long> FINISH_DATE { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<long> EXP_MEST_REASON_ID { get; set; }
        public Nullable<long> SERVICE_REQ_ID { get; set; }
        public Nullable<decimal> TDL_TOTAL_PRICE { get; set; }
        public string TDL_SERVICE_REQ_CODE { get; set; }
        public Nullable<long> TDL_INTRUCTION_TIME { get; set; }
        public Nullable<long> TDL_INTRUCTION_DATE { get; set; }
        public Nullable<long> TDL_TREATMENT_ID { get; set; }
        public string TDL_TREATMENT_CODE { get; set; }
        public Nullable<long> IMP_MEDI_STOCK_ID { get; set; }
        public Nullable<long> AGGR_EXP_MEST_ID { get; set; }
        public Nullable<long> AGGR_USE_TIME { get; set; }
        public string TDL_AGGR_EXP_MEST_CODE { get; set; }
        public Nullable<long> XBTT_EXP_MEST_ID { get; set; }
        public string TDL_XBTT_EXP_MEST_CODE { get; set; }
        public Nullable<long> MANU_IMP_MEST_ID { get; set; }
        public string TDL_MANU_IMP_MEST_CODE { get; set; }
        public Nullable<long> PRESCRIPTION_ID { get; set; }
        public string TDL_PRESCRIPTION_CODE { get; set; }
        public Nullable<long> DISPENSE_ID { get; set; }
        public string TDL_DISPENSE_CODE { get; set; }
        public Nullable<long> SUPPLIER_ID { get; set; }
        public Nullable<short> IS_NOT_TAKEN { get; set; }
        public Nullable<short> IS_EXPORT_EQUAL_APPROVE { get; set; }
        public Nullable<short> IS_EXPORT_EQUAL_REQUEST { get; set; }
        public Nullable<short> IS_HTX { get; set; }
        public Nullable<long> BILL_ID { get; set; }
        public string CASHIER_LOGINNAME { get; set; }
        public string CASHIER_USERNAME { get; set; }
        public Nullable<long> SALE_PATIENT_TYPE_ID { get; set; }
        public Nullable<decimal> DISCOUNT { get; set; }
        public Nullable<long> VACCINATION_ID { get; set; }
        public string NATIONAL_EXP_MEST_CODE { get; set; }
        public Nullable<long> TDL_PATIENT_ID { get; set; }
        public string TDL_PATIENT_CODE { get; set; }
        public string TDL_PATIENT_NAME { get; set; }
        public string TDL_PATIENT_FIRST_NAME { get; set; }
        public string TDL_PATIENT_LAST_NAME { get; set; }
        public Nullable<long> TDL_PATIENT_DOB { get; set; }
        public Nullable<short> TDL_PATIENT_IS_HAS_NOT_DAY_DOB { get; set; }
        public string TDL_PATIENT_ADDRESS { get; set; }
        public Nullable<long> TDL_PATIENT_GENDER_ID { get; set; }
        public string TDL_PATIENT_GENDER_NAME { get; set; }
        public string TDL_PRESCRIPTION_REQ_LOGINNAME { get; set; }
        public string TDL_PRESCRIPTION_REQ_USERNAME { get; set; }
        public Nullable<short> IS_EXECUTE_KIDNEY_PRES { get; set; }
        public Nullable<short> IS_REQUEST_BY_PACKAGE { get; set; }
        public Nullable<long> TDL_PATIENT_TYPE_ID { get; set; }
        public string TDL_HEIN_CARD_NUMBER { get; set; }
        public Nullable<long> CHMS_TYPE_ID { get; set; }
        public Nullable<short> IS_OLD_BCS { get; set; }
        public string EXP_MEST_SUB_CODE { get; set; }
        public Nullable<short> IS_NOT_IN_WORKING_TIME { get; set; }
        public Nullable<long> BCS_TYPE_ID { get; set; }
        public string LAST_APPROVAL_LOGINNAME { get; set; }
        public string LAST_APPROVAL_USERNAME { get; set; }
        public Nullable<long> LAST_APPROVAL_TIME { get; set; }
        public Nullable<long> LAST_APPROVAL_DATE { get; set; }
        public string TDL_PATIENT_WORK_PLACE { get; set; }
        public string TDL_PATIENT_TAX_CODE { get; set; }
        public string TDL_PATIENT_ACCOUNT_NUMBER { get; set; }
        public Nullable<long> SOURCE_MEST_PERIOD_ID { get; set; }
        public Nullable<long> PRES_GROUP { get; set; }
        public string TDL_PATIENT_MOBILE { get; set; }
        public string TDL_PATIENT_PHONE { get; set; }
        public Nullable<decimal> TOTAL_SERVICE_ATTACH_PRICE { get; set; }
        public Nullable<long> CASHIER_ROOM_ID { get; set; }
        public Nullable<long> PAY_FORM_ID { get; set; }
        public Nullable<decimal> TRANSFER_AMOUNT { get; set; }
        public string TDL_PATIENT_DISTRICT_CODE { get; set; }
        public string TDL_PATIENT_PROVINCE_CODE { get; set; }
        public string TDL_PATIENT_COMMUNE_CODE { get; set; }
        public string TDL_PATIENT_NATIONAL_NAME { get; set; }
        public Nullable<short> IS_STAR_MARK { get; set; }
        public Nullable<long> NOT_TAKEN_TIME { get; set; }
        public Nullable<long> DEBT_ID { get; set; }
        public string ERX_PRESCRIPTION_CODE { get; set; }
        public Nullable<short> IS_SENT_ERX { get; set; }
        public Nullable<short> IS_CONFIRM { get; set; }
        public Nullable<long> CONFIRM_TIME { get; set; }
        public string CONFIRM_LOGINNAME { get; set; }
        public string CONFIRM_USERNAME { get; set; }
        public Nullable<decimal> VIR_CREATE_MONTH { get; set; }
        public Nullable<long> PRES_NUMBER { get; set; }
        public Nullable<short> IS_NOT_TAKEN_FAIL { get; set; }
        public string NOT_TAKEN_DESC { get; set; }
        public Nullable<short> IS_BEING_APPROVED { get; set; }
        public Nullable<long> MACHINE_ID { get; set; }
        public Nullable<long> EXP_MEST_TEST_TYPE_ID { get; set; }
        public Nullable<long> QC_TYPE_ID { get; set; }
        public Nullable<short> IS_HOME_PRES { get; set; }
        public Nullable<short> IS_KIDNEY { get; set; }
        public string ICD_CODE { get; set; }
        public string ICD_NAME { get; set; }
        public string ICD_SUB_CODE { get; set; }
        public string ICD_TEXT { get; set; }
        public Nullable<short> HAS_NOT_PRES { get; set; }
        public string REQ_USER_TITLE { get; set; }
        public string TDL_PRES_REQ_USER_TITLE { get; set; }
        public Nullable<long> SUB_CODE_TYPE { get; set; }
        public string EXP_MEST_SUB_CODE_2 { get; set; }
        public string RECIPIENT { get; set; }
        public string RECEIVING_PLACE { get; set; }
        public Nullable<long> SPECIAL_MEDICINE_NUM_ORDER { get; set; }
        public Nullable<long> SPECIAL_MEDICINE_TYPE { get; set; }
        public Nullable<decimal> VIR_CREATE_YEAR { get; set; }
        public string VIR_SPECIAL_MEDICINE_NUM_ORDER { get; set; }
        public string VIR_HEIN_CARD_PREFIX { get; set; }
        public Nullable<long> NUM_ORDER { get; set; }
        public Nullable<long> ANTIBIOTIC_REQUEST_ID { get; set; }
        public Nullable<short> IS_USING_APPROVAL_REQUIRED { get; set; }
        public Nullable<long> CALL_COUNT { get; set; }
        public Nullable<short> IS_ABSENT { get; set; }
        public string GATE_CODE { get; set; }
        public Nullable<long> PRIORITY { get; set; }
        public Nullable<long> PRIORITY_TYPE_ID { get; set; }
        public Nullable<long> CALL_TIME { get; set; }
        public string TDL_BLOOD_CODE { get; set; }
        public string TDL_AGGR_PATIENT_CODE { get; set; }
        public string TDL_AGGR_TREATMENT_CODE { get; set; }
        public Nullable<long> REMEDY_COUNT { get; set; }
        public string EXP_MEST_TYPE_CODE { get; set; }
        public string EXP_MEST_TYPE_NAME { get; set; }
        public string EXP_MEST_STT_CODE { get; set; }
        public string EXP_MEST_STT_NAME { get; set; }
        public string MEDI_STOCK_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public Nullable<short> IS_CABINET { get; set; }
        public string REQ_DEPARTMENT_CODE { get; set; }
        public string REQ_DEPARTMENT_NAME { get; set; }
        public string REQ_ROOM_CODE { get; set; }
        public string REQ_ROOM_NAME { get; set; }
        public string EXP_MEST_REASON_CODE { get; set; }
        public string EXP_MEST_REASON_NAME { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }
        public string PARENT_MEDI_STOCK_CODE { get; set; }
        public string PARENT_MEDI_STOCK_NAME { get; set; }
        public Nullable<long> PARENT_MEDI_STOCK_ID { get; set; }
        public string CURRENT_BED_IDS { get; set; }
        public string ANTIBIOTIC_REQUEST_CODE { get; set; }
        public Nullable<long> ANTIBIOTIC_REQUEST_STT { get; set; }
    }
}
