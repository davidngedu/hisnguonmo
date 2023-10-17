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
    
    public partial class V_HIS_EXP_MEST_MATERIAL_4
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
        public Nullable<decimal> BK_AMOUNT { get; set; }
        public Nullable<long> EXP_MEST_ID { get; set; }
        public Nullable<long> MATERIAL_ID { get; set; }
        public Nullable<long> TDL_MEDI_STOCK_ID { get; set; }
        public Nullable<long> MEDI_STOCK_PERIOD_ID { get; set; }
        public Nullable<long> TDL_MATERIAL_TYPE_ID { get; set; }
        public Nullable<long> TDL_AGGR_EXP_MEST_ID { get; set; }
        public Nullable<long> EXP_MEST_MATY_REQ_ID { get; set; }
        public Nullable<long> CK_IMP_MEST_MATERIAL_ID { get; set; }
        public Nullable<short> IS_EXPORT { get; set; }
        public decimal AMOUNT { get; set; }
        public Nullable<decimal> PRICE { get; set; }
        public Nullable<decimal> VAT_RATIO { get; set; }
        public Nullable<decimal> DISCOUNT { get; set; }
        public Nullable<long> NUM_ORDER { get; set; }
        public string DESCRIPTION { get; set; }
        public string APPROVAL_LOGINNAME { get; set; }
        public string APPROVAL_USERNAME { get; set; }
        public Nullable<long> APPROVAL_TIME { get; set; }
        public Nullable<long> APPROVAL_DATE { get; set; }
        public string EXP_LOGINNAME { get; set; }
        public string EXP_USERNAME { get; set; }
        public Nullable<long> EXP_TIME { get; set; }
        public Nullable<long> EXP_DATE { get; set; }
        public Nullable<decimal> TH_AMOUNT { get; set; }
        public Nullable<long> PATIENT_TYPE_ID { get; set; }
        public Nullable<long> SERE_SERV_PARENT_ID { get; set; }
        public Nullable<short> IS_EXPEND { get; set; }
        public Nullable<short> IS_OUT_PARENT_FEE { get; set; }
        public Nullable<long> TDL_SERVICE_REQ_ID { get; set; }
        public Nullable<long> STENT_ORDER { get; set; }
        public Nullable<long> TDL_TREATMENT_ID { get; set; }
        public Nullable<long> EQUIPMENT_SET_ID { get; set; }
        public Nullable<long> EQUIPMENT_SET_ORDER { get; set; }
        public Nullable<short> IS_USE_CLIENT_PRICE { get; set; }
        public Nullable<decimal> VIR_PRICE { get; set; }
        public Nullable<long> EXPEND_TYPE_ID { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public Nullable<long> REMAIN_REUSE_COUNT { get; set; }
        public Nullable<short> IS_NOT_PRES { get; set; }
        public Nullable<short> USE_ORIGINAL_UNIT_FOR_PRES { get; set; }
        public Nullable<decimal> BCS_REQ_AMOUNT { get; set; }
        public Nullable<decimal> FAILED_AMOUNT { get; set; }
        public string TUTORIAL { get; set; }
        public string EXP_MEST_CODE { get; set; }
        public long EXP_MEST_TYPE_ID { get; set; }
        public string EXP_MEST_TYPE_NAME { get; set; }
        public string EXP_MEST_TYPE_CODE { get; set; }
        public string MEDI_STOCK_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string MATERIAL_TYPE_CODE { get; set; }
        public string MATERIAL_TYPE_NAME { get; set; }
        public string MEDI_STOCK_PERIOD_NAME { get; set; }
    }
}
