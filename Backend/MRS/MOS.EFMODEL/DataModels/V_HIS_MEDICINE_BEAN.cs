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
    
    public partial class V_HIS_MEDICINE_BEAN
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
        public long MEDICINE_ID { get; set; }
        public decimal AMOUNT { get; set; }
        public Nullable<long> MEDI_STOCK_ID { get; set; }
        public Nullable<long> SOURCE_ID { get; set; }
        public Nullable<decimal> BK_DECREASE_AMOUNT { get; set; }
        public Nullable<long> BK_MEDI_STOCK_ID { get; set; }
        public Nullable<long> EXP_MEST_MEDICINE_ID { get; set; }
        public Nullable<decimal> DETACH_AMOUNT { get; set; }
        public string DETACH_KEY { get; set; }
        public Nullable<short> IS_TH { get; set; }
        public Nullable<short> IS_CK { get; set; }
        public Nullable<short> IS_USE { get; set; }
        public string SESSION_KEY { get; set; }
        public Nullable<long> SESSION_TIME { get; set; }
        public long TDL_MEDICINE_TYPE_ID { get; set; }
        public Nullable<short> TDL_MEDICINE_IS_ACTIVE { get; set; }
        public decimal TDL_MEDICINE_IMP_PRICE { get; set; }
        public decimal TDL_MEDICINE_IMP_VAT_RATIO { get; set; }
        public Nullable<long> TDL_MEDICINE_IMP_TIME { get; set; }
        public Nullable<long> TDL_MEDICINE_EXPIRED_DATE { get; set; }
        public Nullable<short> TDL_IS_SALE_EQUAL_IMP_PRICE { get; set; }
        public long TDL_SERVICE_ID { get; set; }
        public string TDL_PACKAGE_NUMBER { get; set; }
        public string TDL_MEDICINE_REGISTER_NUMBER { get; set; }
        public string LOCKING_REASON { get; set; }
        public long MEDICINE_TYPE_ID { get; set; }
        public decimal IMP_PRICE { get; set; }
        public decimal IMP_VAT_RATIO { get; set; }
        public Nullable<long> BID_ID { get; set; }
        public Nullable<long> IMP_TIME { get; set; }
        public Nullable<decimal> INTERNAL_PRICE { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public Nullable<long> SUPPLIER_ID { get; set; }
        public Nullable<short> MEDICINE_IS_ACTIVE { get; set; }
        public string TDL_BID_NUMBER { get; set; }
        public Nullable<long> EXPIRED_DATE { get; set; }
        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string MEDI_LOCKING_REASON { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public long SERVICE_ID { get; set; }
        public Nullable<long> ALERT_EXPIRED_DATE { get; set; }
        public Nullable<decimal> ALERT_MIN_IN_STOCK { get; set; }
        public Nullable<long> PARENT_ID { get; set; }
        public string NATIONAL_NAME { get; set; }
        public Nullable<long> NUM_ORDER { get; set; }
        public Nullable<short> MEDICINE_TYPE_IS_ACTIVE { get; set; }
        public string CONCENTRA { get; set; }
        public Nullable<long> MEDICINE_GROUP_ID { get; set; }
        public Nullable<short> IS_AUTO_EXPEND { get; set; }
        public string METY_LOCKING_REASON { get; set; }
        public Nullable<decimal> USE_ON_DAY { get; set; }
        public string PACKING_TYPE_NAME { get; set; }
        public string TUTORIAL { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<long> MEDICINE_USE_FORM_ID { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public long SERVICE_UNIT_ID { get; set; }
        public Nullable<short> IS_BUSINESS { get; set; }
        public Nullable<decimal> LAST_EXP_PRICE { get; set; }
        public Nullable<decimal> LAST_EXP_VAT_RATIO { get; set; }
        public Nullable<long> LAST_EXPIRED_DATE { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string SERVICE_UNIT_SYMBOL { get; set; }
        public string MEDI_STOCK_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public Nullable<short> IS_GOODS_RESTRICT { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
    }
}
