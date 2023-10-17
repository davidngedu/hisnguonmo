﻿using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.ADO
{
    public class MediMateTypeADO
    {
        //public HisMaterialTypeAmountWithPriceSDO ExpMaterial { get; set; }
        //public HisMedicineTypeAmountWithPriceSDO ExpMedicine { get; set; }

        public long SERVICE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }

        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public decimal? AVAILABLE_AMOUNT { get; set; }

        public decimal EXP_AMOUNT { get; set; }
        public string NOTE { get; set; }
        public string DESCRIPTION { get; set; }
        public decimal? DISCOUNT { get; set; }
        public decimal? DISCOUNT_RATIO { get; set; }
        public decimal? EXP_PRICE { get; set; }
        public decimal? EXP_VAT_RATIO { get; set; }
        public string TUTORIAL { get; set; }
        public long? PATIENT_TYPE_ID { get; set; }
        public string SUPPLIER_NAME { get; set; }
        public string BID_NUMBER { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public decimal AMOUNT { get; set; }
        public bool IsMedicine { get; set; }

        public bool IsNotHasMest { get; set; }
        public bool IsGreatAvailable { get; set; }

        public MediMateTypeADO() { }

        public MediMateTypeADO(HisMedicineTypeInStockSDO medicine)
        {
            try
            {
                if (medicine != null)
                {
                    this.IsMedicine = true;
                    this.SERVICE_ID = medicine.ServiceId;
                    this.MEDI_MATE_TYPE_ID = medicine.Id;
                    this.MEDI_MATE_TYPE_CODE = medicine.MedicineTypeCode;
                    this.MEDI_MATE_TYPE_NAME = medicine.MedicineTypeName;
                    this.SERVICE_UNIT_NAME = medicine.ServiceUnitName;
                    this.NATIONAL_NAME = medicine.NationalName;
                    this.MANUFACTURER_NAME = medicine.ManufacturerName;
                    this.REGISTER_NUMBER = medicine.RegisterNumber;
                    this.AVAILABLE_AMOUNT = medicine.AvailableAmount;
                    //this.ExpMedicine = new HisMedicineTypeAmountWithPriceSDO();
                    //this.ExpMedicine.MedicineTypeId = medicine.Id;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(HisMaterialTypeInStockSDO material)
        {
            try
            {
                if (material != null)
                {
                    this.IsMedicine = false;
                    this.SERVICE_ID = material.ServiceId;
                    this.MEDI_MATE_TYPE_ID = material.Id;
                    this.MEDI_MATE_TYPE_CODE = material.MaterialTypeCode;
                    this.MEDI_MATE_TYPE_NAME = material.MaterialTypeName;
                    this.SERVICE_UNIT_NAME = material.ServiceUnitName;
                    this.NATIONAL_NAME = material.NationalName;
                    this.MANUFACTURER_NAME = material.ManufacturerName;
                    this.AVAILABLE_AMOUNT = material.AvailableAmount;
                    //this.ExpMaterial = new HisMaterialTypeAmountWithPriceSDO();
                    //this.ExpMaterial.MaterialTypeId = material.Id;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
