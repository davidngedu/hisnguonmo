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
    
    public partial class V_HIS_TREATMENT_FEE_1
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
        public string TREATMENT_CODE { get; set; }
        public Nullable<long> TREATMENT_STT_ID { get; set; }
        public long PATIENT_ID { get; set; }
        public long BRANCH_ID { get; set; }
        public Nullable<short> IS_PAUSE { get; set; }
        public Nullable<short> IS_LOCK_HEIN { get; set; }
        public Nullable<short> IS_TEMPORARY_LOCK { get; set; }
        public Nullable<short> IS_LOCK_FEE { get; set; }
        public Nullable<short> IS_REMOTE { get; set; }
        public Nullable<long> ICD_ID__DELETE { get; set; }
        public string ICD_CODE { get; set; }
        public string ICD_NAME { get; set; }
        public string ICD_SUB_CODE { get; set; }
        public string ICD_TEXT { get; set; }
        public string ICD_CAUSE_CODE { get; set; }
        public string ICD_CAUSE_NAME { get; set; }
        public Nullable<short> IS_HOLD_BHYT_CARD { get; set; }
        public Nullable<short> IS_AUTO_DISCOUNT { get; set; }
        public Nullable<decimal> AUTO_DISCOUNT_RATIO { get; set; }
        public Nullable<long> FEE_LOCK_TIME { get; set; }
        public Nullable<long> FEE_LOCK_ORDER { get; set; }
        public Nullable<long> FEE_LOCK_ROOM_ID { get; set; }
        public Nullable<long> FEE_LOCK_DEPARTMENT_ID { get; set; }
        public long IN_TIME { get; set; }
        public long IN_DATE { get; set; }
        public Nullable<long> CLINICAL_IN_TIME { get; set; }
        public Nullable<long> OUT_TIME { get; set; }
        public Nullable<short> IS_IN_CODE_REQUEST { get; set; }
        public string IN_CODE { get; set; }
        public Nullable<long> IN_ROOM_ID { get; set; }
        public Nullable<long> IN_DEPARTMENT_ID { get; set; }
        public string IN_LOGINNAME { get; set; }
        public string IN_USERNAME { get; set; }
        public Nullable<long> IN_TREATMENT_TYPE_ID { get; set; }
        public Nullable<long> IN_ICD_ID__DELETE { get; set; }
        public string IN_ICD_CODE { get; set; }
        public string IN_ICD_NAME { get; set; }
        public string IN_ICD_SUB_CODE { get; set; }
        public string IN_ICD_TEXT { get; set; }
        public string HOSPITALIZATION_REASON { get; set; }
        public string DOCTOR_LOGINNAME { get; set; }
        public string DOCTOR_USERNAME { get; set; }
        public string END_LOGINNAME { get; set; }
        public string END_USERNAME { get; set; }
        public Nullable<long> END_ROOM_ID { get; set; }
        public Nullable<long> END_DEPARTMENT_ID { get; set; }
        public string END_CODE { get; set; }
        public string EXTRA_END_CODE { get; set; }
        public Nullable<short> IS_END_CODE_REQUEST { get; set; }
        public Nullable<decimal> TREATMENT_DAY_COUNT { get; set; }
        public Nullable<long> TREATMENT_RESULT_ID { get; set; }
        public Nullable<long> TREATMENT_END_TYPE_ID { get; set; }
        public Nullable<long> TREATMENT_END_TYPE_EXT_ID { get; set; }
        public string ADVISE { get; set; }
        public Nullable<long> APPOINTMENT_TIME { get; set; }
        public string APPOINTMENT_DESC { get; set; }
        public string APPOINTMENT_CODE { get; set; }
        public Nullable<long> OUT_DATE { get; set; }
        public string OUT_CODE { get; set; }
        public Nullable<short> IS_OUT_CODE_REQUEST { get; set; }
        public Nullable<short> IS_CHRONIC { get; set; }
        public Nullable<short> IS_YDT_UPLOAD { get; set; }
        public Nullable<long> OWE_TYPE_ID { get; set; }
        public Nullable<long> OWE_MODIFY_TIME { get; set; }
        public Nullable<long> STORE_TIME { get; set; }
        public Nullable<long> DATA_STORE_ID { get; set; }
        public string STORE_CODE { get; set; }
        public Nullable<short> IS_NOT_CHECK_LHMP { get; set; }
        public Nullable<short> IS_NOT_CHECK_LHSP { get; set; }
        public string TDL_HEIN_CARD_NUMBER { get; set; }
        public string JSON_PRINT_ID { get; set; }
        public string JSON_FORM_ID { get; set; }
        public Nullable<short> IS_EMERGENCY { get; set; }
        public Nullable<long> EMERGENCY_WTIME_ID { get; set; }
        public Nullable<long> KSK_ORDER { get; set; }
        public Nullable<long> TDL_KSK_CONTRACT_ID { get; set; }
        public string HRM_KSK_CODE { get; set; }
        public string CLINICAL_NOTE { get; set; }
        public string SUBCLINICAL_RESULT { get; set; }
        public string TREATMENT_DIRECTION { get; set; }
        public string TREATMENT_METHOD { get; set; }
        public string PATIENT_CONDITION { get; set; }
        public string MEDI_ORG_CODE { get; set; }
        public string MEDI_ORG_NAME { get; set; }
        public Nullable<long> TRAN_PATI_FORM_ID { get; set; }
        public Nullable<long> TRAN_PATI_REASON_ID { get; set; }
        public Nullable<long> TRAN_PATI_TECH_ID { get; set; }
        public string USED_MEDICINE { get; set; }
        public string TRANSPORT_VEHICLE { get; set; }
        public string TRANSPORTER { get; set; }
        public Nullable<short> IS_TRANSFER_IN { get; set; }
        public string TRANSFER_IN_MEDI_ORG_CODE { get; set; }
        public string TRANSFER_IN_MEDI_ORG_NAME { get; set; }
        public string TRANSFER_IN_CODE { get; set; }
        public Nullable<long> TRANSFER_IN_ICD_ID__DELETE { get; set; }
        public string TRANSFER_IN_ICD_CODE { get; set; }
        public string TRANSFER_IN_ICD_NAME { get; set; }
        public Nullable<long> TRANSFER_IN_CMKT { get; set; }
        public Nullable<long> TRANSFER_IN_FORM_ID { get; set; }
        public Nullable<long> TRANSFER_IN_REASON_ID { get; set; }
        public Nullable<decimal> SICK_LEAVE_DAY { get; set; }
        public Nullable<long> SICK_LEAVE_FROM { get; set; }
        public Nullable<long> SICK_LEAVE_TO { get; set; }
        public Nullable<long> DEATH_TIME { get; set; }
        public Nullable<long> DEATH_CAUSE_ID { get; set; }
        public Nullable<long> DEATH_WITHIN_ID { get; set; }
        public string DEATH_PLACE { get; set; }
        public string DEATH_DOCUMENT_TYPE { get; set; }
        public string DEATH_DOCUMENT_NUMBER { get; set; }
        public string DEATH_DOCUMENT_PLACE { get; set; }
        public Nullable<long> DEATH_DOCUMENT_DATE { get; set; }
        public string MAIN_CAUSE { get; set; }
        public string SURGERY { get; set; }
        public Nullable<short> IS_HAS_AUPOPSY { get; set; }
        public Nullable<long> TDL_FIRST_EXAM_ROOM_ID { get; set; }
        public Nullable<long> TDL_TREATMENT_TYPE_ID { get; set; }
        public Nullable<long> TDL_PATIENT_TYPE_ID { get; set; }
        public string TDL_HEIN_MEDI_ORG_CODE { get; set; }
        public string TDL_HEIN_MEDI_ORG_NAME { get; set; }
        public string XML4210_URL { get; set; }
        public Nullable<long> FUND_ID { get; set; }
        public string FUND_NUMBER { get; set; }
        public Nullable<long> FUND_FROM_TIME { get; set; }
        public Nullable<long> FUND_TO_TIME { get; set; }
        public Nullable<long> FUND_ISSUE_TIME { get; set; }
        public string FUND_TYPE_NAME { get; set; }
        public string FUND_COMPANY_NAME { get; set; }
        public Nullable<decimal> FUND_BUDGET { get; set; }
        public Nullable<long> FUND_PAY_TIME { get; set; }
        public Nullable<long> FUND_SEND_FILE_TIME { get; set; }
        public string FUND_CUSTOMER_NAME { get; set; }
        public Nullable<short> IS_INTEGRATE_HIS_SENT { get; set; }
        public string TDL_PATIENT_CODE { get; set; }
        public string TDL_PATIENT_NAME { get; set; }
        public string TDL_PATIENT_FIRST_NAME { get; set; }
        public string TDL_PATIENT_LAST_NAME { get; set; }
        public long TDL_PATIENT_DOB { get; set; }
        public Nullable<short> TDL_PATIENT_IS_HAS_NOT_DAY_DOB { get; set; }
        public string TDL_PATIENT_AVATAR_URL { get; set; }
        public string TDL_PATIENT_ADDRESS { get; set; }
        public long TDL_PATIENT_GENDER_ID { get; set; }
        public string TDL_PATIENT_GENDER_NAME { get; set; }
        public string TDL_PATIENT_CAREER_NAME { get; set; }
        public string TDL_PATIENT_WORK_PLACE { get; set; }
        public string TDL_PATIENT_WORK_PLACE_NAME { get; set; }
        public string TDL_PATIENT_DISTRICT_CODE { get; set; }
        public string TDL_PATIENT_PROVINCE_CODE { get; set; }
        public string TDL_PATIENT_COMMUNE_CODE { get; set; }
        public string TDL_PATIENT_MILITARY_RANK_NAME { get; set; }
        public string TDL_PATIENT_NATIONAL_NAME { get; set; }
        public string TDL_PATIENT_RELATIVE_TYPE { get; set; }
        public string TDL_PATIENT_RELATIVE_NAME { get; set; }
        public string TDL_PATIENT_ACCOUNT_NUMBER { get; set; }
        public string TDL_PATIENT_TAX_CODE { get; set; }
        public Nullable<long> TRANSFER_IN_TIME_FROM { get; set; }
        public Nullable<long> TRANSFER_IN_TIME_TO { get; set; }
        public Nullable<long> SURGERY_APPOINTMENT_TIME { get; set; }
        public string APPOINTMENT_SURGERY { get; set; }
        public Nullable<long> MEDI_RECORD_TYPE_ID { get; set; }
        public string APPOINTMENT_EXAM_ROOM_IDS { get; set; }
        public string DEPARTMENT_IDS { get; set; }
        public string CO_DEPARTMENT_IDS { get; set; }
        public Nullable<long> LAST_DEPARTMENT_ID { get; set; }
        public string PROVISIONAL_DIAGNOSIS { get; set; }
        public Nullable<long> TREATMENT_ORDER { get; set; }
        public string TDL_PATIENT_MOBILE { get; set; }
        public string TDL_PATIENT_PHONE { get; set; }
        public string SICK_HEIN_CARD_NUMBER { get; set; }
        public Nullable<short> NEED_SICK_LEAVE_CERT { get; set; }
        public Nullable<long> MEDI_RECORD_ID { get; set; }
        public Nullable<long> PROGRAM_ID { get; set; }
        public Nullable<short> IS_SYNC_EMR { get; set; }
        public Nullable<long> XML4210_RESULT { get; set; }
        public string XML4210_DESC { get; set; }
        public string COLLINEAR_XML4210_URL { get; set; }
        public Nullable<long> COLLINEAR_XML4210_RESULT { get; set; }
        public string COLLINEAR_XML4210_DESC { get; set; }
        public string REJECT_STORE_REASON { get; set; }
        public Nullable<short> IS_APPROVE_FINISH { get; set; }
        public string APPROVE_FINISH_NOTE { get; set; }
        public string TRADITIONAL_ICD_CODE { get; set; }
        public string TRADITIONAL_ICD_NAME { get; set; }
        public string TRADITIONAL_IN_ICD_CODE { get; set; }
        public string TRADITIONAL_IN_ICD_NAME { get; set; }
        public string TRADITIONAL_ICD_SUB_CODE { get; set; }
        public string TRADITIONAL_ICD_TEXT { get; set; }
        public string TRADITIONAL_IN_ICD_SUB_CODE { get; set; }
        public string TRADITIONAL_IN_ICD_TEXT { get; set; }
        public string TRADITIONAL_TRANS_IN_ICD_CODE { get; set; }
        public string TRADITIONAL_TRANS_IN_ICD_NAME { get; set; }
        public Nullable<long> OTHER_PAY_SOURCE_ID { get; set; }
        public Nullable<short> IS_KSK_APPROVE { get; set; }
        public Nullable<long> TDL_HEIN_CARD_FROM_TIME { get; set; }
        public Nullable<long> TDL_HEIN_CARD_TO_TIME { get; set; }
        public Nullable<long> APPOINTMENT_DATE { get; set; }
        public string EYE_TENSION_LEFT { get; set; }
        public string EYE_TENSION_RIGHT { get; set; }
        public string EYESIGHT_LEFT { get; set; }
        public string EYESIGHT_RIGHT { get; set; }
        public string EYESIGHT_GLASS_LEFT { get; set; }
        public string EYESIGHT_GLASS_RIGHT { get; set; }
        public Nullable<long> APPOINTMENT_PERIOD_ID { get; set; }
        public string SICK_LOGINNAME { get; set; }
        public string SICK_USERNAME { get; set; }
        public Nullable<short> IS_EXPORTED_XML2076 { get; set; }
        public Nullable<long> DOCUMENT_BOOK_ID { get; set; }
        public Nullable<long> SICK_NUM_ORDER { get; set; }
        public string TDL_DOCUMENT_BOOK_CODE { get; set; }
        public Nullable<long> APPROVAL_STORE_STT_ID { get; set; }
        public Nullable<long> TDL_PATIENT_CLASSIFY_ID { get; set; }
        public Nullable<decimal> VIR_IN_MONTH { get; set; }
        public Nullable<decimal> VIR_OUT_MONTH { get; set; }
        public string IN_CODE_SEED_CODE { get; set; }
        public string EXTRA_END_CODE_SEED_CODE { get; set; }
        public string XML2076_URL { get; set; }
        public string XML2076_DESC { get; set; }
        public Nullable<decimal> VIR_IN_YEAR { get; set; }
        public Nullable<decimal> VIR_OUT_YEAR { get; set; }
        public string FEE_LOCK_LOGINNAME { get; set; }
        public string FEE_LOCK_USERNAME { get; set; }
        public Nullable<long> EMR_COVER_TYPE_ID { get; set; }
        public string CONTRAINDICATION_IDS { get; set; }
        public Nullable<short> IS_CREATING_TRANSACTION { get; set; }
        public string CO_TREAT_DEPARTMENT_IDS { get; set; }
        public Nullable<long> RECORD_INSPECTION_STT_ID { get; set; }
        public string RECORD_INSPECTION_REJECT_NOTE { get; set; }
        public string TDL_SOCIAL_INSURANCE_NUMBER { get; set; }
        public Nullable<long> APPOINTMENT_EXAM_SERVICE_ID { get; set; }
        public string TRANSFER_IN_URL { get; set; }
        public Nullable<long> HOSPITALIZE_DEPARTMENT_ID { get; set; }
        public Nullable<long> DEATH_CERT_BOOK_ID { get; set; }
        public Nullable<long> DEATH_CERT_NUM { get; set; }
        public string TDL_PATIENT_CMND_NUMBER { get; set; }
        public Nullable<long> TDL_PATIENT_CMND_DATE { get; set; }
        public string TDL_PATIENT_CMND_PLACE { get; set; }
        public string TDL_PATIENT_CCCD_NUMBER { get; set; }
        public Nullable<long> TDL_PATIENT_CCCD_DATE { get; set; }
        public string TDL_PATIENT_CCCD_PLACE { get; set; }
        public Nullable<long> NUM_ORDER_ISSUE_ID { get; set; }
        public Nullable<long> NEXT_EXAM_NUM_ORDER { get; set; }
        public string NEXT_EXAM_FROM_TIME { get; set; }
        public string NEXT_EXAM_TO_TIME { get; set; }
        public string TDL_PATIENT_RELATIVE_ADDRESS { get; set; }
        public string TDL_RELATIVE_CMND_NUMBER { get; set; }
        public string TDL_PATIENT_RELATIVE_MOBILE { get; set; }
        public string TDL_PATIENT_RELATIVE_PHONE { get; set; }
        public string TDL_PATIENT_MOTHER_NAME { get; set; }
        public string TDL_PATIENT_FATHER_NAME { get; set; }
        public string PERMISION_UPDATE { get; set; }
        public string TDL_PATIENT_NATIONAL_CODE { get; set; }
        public string TDL_PATIENT_PROVINCE_NAME { get; set; }
        public string TDL_PATIENT_DISTRICT_NAME { get; set; }
        public string TDL_PATIENT_COMMUNE_NAME { get; set; }
        public Nullable<long> TDL_PATIENT_POSITION_ID { get; set; }
        public Nullable<short> IS_BHYT_HOLDED { get; set; }
        public string TDL_PATIENT_PASSPORT_NUMBER { get; set; }
        public Nullable<long> TDL_PATIENT_PASSPORT_DATE { get; set; }
        public string TDL_PATIENT_PASSPORT_PLACE { get; set; }
        public Nullable<long> TDL_PATIENT_WORK_PLACE_ID { get; set; }
        public string SHOW_ICD_CODE { get; set; }
        public string SHOW_ICD_NAME { get; set; }
        public string SHOW_ICD_SUB_CODE { get; set; }
        public string SHOW_ICD_TEXT { get; set; }
        public Nullable<long> TRAN_PATI_BOOK_TIME { get; set; }
        public Nullable<long> TRAN_PATI_BOOK_NUMBER { get; set; }
        public string TRAN_PATI_DOCTOR_LOGINNAME { get; set; }
        public string TRAN_PATI_DOCTOR_USERNAME { get; set; }
        public string TRAN_PATI_DEPARTMENT_LOGINNAME { get; set; }
        public string TRAN_PATI_DEPARTMENT_USERNAME { get; set; }
        public string TRAN_PATI_HOSPITAL_LOGINNAME { get; set; }
        public string TRAN_PATI_HOSPITAL_USERNAME { get; set; }
        public Nullable<decimal> VIR_TRAN_PATI_BOOK_YEAR { get; set; }
        public string TDL_PATIENT_UNSIGNED_NAME { get; set; }
        public Nullable<long> OUTPATIENT_DATE_FROM { get; set; }
        public Nullable<long> OUTPATIENT_DATE_TO { get; set; }
        public Nullable<long> VACCINE_ID { get; set; }
        public Nullable<long> VACINATION_ORDER { get; set; }
        public Nullable<long> EPIDEMILOGY_CONTACT_TYPE { get; set; }
        public string EPIDEMILOGY_SYMPTOM { get; set; }
        public string TDL_PATIENT_ETHNIC_NAME { get; set; }
        public Nullable<short> IS_TUBERCULOSIS { get; set; }
        public Nullable<long> DOCUMENT_VIEW_COUNT { get; set; }
        public Nullable<long> EXIT_DEPARTMENT_ID { get; set; }
        public string COVID_PATIENT_CODE { get; set; }
        public string STORE_BORDEREAU_CODE { get; set; }
        public Nullable<long> HEIN_LOCK_TIME { get; set; }
        public Nullable<short> HAS_AUTO_CREATE_RATION { get; set; }
        public Nullable<short> IS_OLD_TEMP_BED { get; set; }
        public Nullable<short> TDL_KSK_CONTRACT_IS_RESTRICTED { get; set; }
        public Nullable<long> NUMBER_OF_FULL_TERM_BIRTH { get; set; }
        public Nullable<long> NUMBER_OF_PREMATURE_BIRTH { get; set; }
        public Nullable<long> NUMBER_OF_MISCARRIAGE { get; set; }
        public Nullable<short> NUMBER_OF_TESTS { get; set; }
        public Nullable<short> TEST_HIV { get; set; }
        public Nullable<short> TEST_SYPHILIS { get; set; }
        public Nullable<short> IS_TEST_BLOOD_SUGAR { get; set; }
        public Nullable<short> IS_EARLY_NEWBORN_CARE { get; set; }
        public Nullable<short> NEWBORN_CARE_AT_HOME { get; set; }
        public Nullable<short> TEST_HEPATITIS_B { get; set; }
        public Nullable<long> RECEPTION_FORM { get; set; }
        public string END_DEPARTMENT_HEAD_LOGINNAME { get; set; }
        public string END_DEPARTMENT_HEAD_USERNAME { get; set; }
        public string HOSPITAL_DIRECTOR_LOGINNAME { get; set; }
        public string HOSPITAL_DIRECTOR_USERNAME { get; set; }
        public string END_DEPT_SUBS_HEAD_LOGINNAME { get; set; }
        public string END_DEPT_SUBS_HEAD_USERNAME { get; set; }
        public string HOSP_SUBS_DIRECTOR_LOGINNAME { get; set; }
        public string HOSP_SUBS_DIRECTOR_USERNAME { get; set; }
        public string TUBERCULOSIS_ISSUED_ORG_CODE { get; set; }
        public string TUBERCULOSIS_ISSUED_ORG_NAME { get; set; }
        public Nullable<long> TUBERCULOSIS_ISSUED_DATE { get; set; }
        public Nullable<short> HAS_CARD { get; set; }
        public string APPROVAL_LOGINNAME { get; set; }
        public string APPROVAL_USERNAME { get; set; }
        public Nullable<long> APPROVAL_TIME { get; set; }
        public string UNAPPROVAL_LOGINNAME { get; set; }
        public string UNAPPROVAL_USERNAME { get; set; }
        public Nullable<long> UNAPPROVAL_TIME { get; set; }
        public Nullable<short> TRANSFER_IN_REVIEWS { get; set; }
        public string HOSPITALIZE_REASON_CODE { get; set; }
        public string HOSPITALIZE_REASON_NAME { get; set; }
        public string END_TYPE_EXT_NOTE { get; set; }
        public string TDL_PATIENT_MPS_NATIONAL_CODE { get; set; }
        public Nullable<short> IS_PREGNANCY_TERMINATION { get; set; }
        public string PREGNANCY_TERMINATION_REASON { get; set; }
        public Nullable<long> GESTATIONAL_AGE { get; set; }
        public Nullable<long> DEATH_CERT_BOOK_FIRST_ID { get; set; }
        public Nullable<long> DEATH_CERT_NUM_FIRST { get; set; }
        public string DEATH_CERT_ISSUER_LOGINNAME { get; set; }
        public string DEATH_CERT_ISSUER_USERNAME { get; set; }
        public string DEATH_SYNC_FAILD_REASON { get; set; }
        public Nullable<long> DEATH_SYNC_RESULT_TYPE { get; set; }
        public Nullable<long> DEATH_SYNC_TIME { get; set; }
        public Nullable<long> PREGNANCY_TERMINATION_TIME { get; set; }
        public Nullable<short> IS_HIV { get; set; }
        public string XML_CHECKIN_URL { get; set; }
        public string XML_CHECKIN_DESC { get; set; }
        public Nullable<decimal> TOTAL_BILL_AMOUNT { get; set; }
        public Nullable<decimal> TOTAL_BILL_OTHER_AMOUNT { get; set; }
        public Nullable<decimal> TOTAL_BILL_TRANSFER_AMOUNT { get; set; }
        public Nullable<decimal> TOTAL_DEPOSIT_AMOUNT { get; set; }
        public Nullable<decimal> TOTAL_REPAY_AMOUNT { get; set; }
        public Nullable<decimal> TOTAL_PATIENT_PRICE { get; set; }
        public Nullable<decimal> TOTAL_DEBT_AMOUNT { get; set; }
    }
}
