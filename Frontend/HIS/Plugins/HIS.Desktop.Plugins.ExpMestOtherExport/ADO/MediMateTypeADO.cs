﻿using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestOtherExport.ADO
{
    public class MediMateTypeADO
    {
        public ExpMaterialSDO ExpMaterial { get; set; }
        public ExpMedicineSDO ExpMedicine { get; set; }
        public ExpBloodSDO ExpBlood { get; set; }

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
        public decimal? DISCOUNT { get; set; }
        public decimal? DISCOUNT_RATIO { get; set; }
        public decimal? EXP_PRICE { get; set; }
        public decimal? EXP_VAT_RATIO { get; set; }
        public decimal IMP_PRICE { get; set; }
        public decimal IMP_VAT_RATIO { get; set; }
        public string TUTORIAL { get; set; }
        public long? PATIENT_TYPE_ID { get; set; }

        public bool IsMedicine { get; set; }
        public bool IsBlood { get; set; }
        public bool IsReuse { get; set; }


        public bool IsNotHasMest { get; set; }
        public bool IsGreatAvailable { get; set; }

        public decimal? ADVISORY_PRICE { get; set; }
        public decimal? ADVISORY_TOTAL_PRICE { get; set; }

        public decimal VOLUME { get; set; }
        public long BLOOD_ABO_ID { get; set; }
        public long? BLOOD_RH_ID { get; set; }
        public string BLOOD_ABO_CODE { get; set; }
        public string BLOOD_RH_CODE { get; set; }
        public string BLOOD_CODE { get; set; }
        public long BLOOD_ID { get; set; }
        public long? EXPIRED_DATE { get; set; }

        public string SERIAL_NUMBER { get; set; }
        public long? TDL_MATERIAL_MAX_REUSE_COUNT { get; set; }

        public MediMateTypeADO() { }

        public MediMateTypeADO(HisMedicineInStockSDO medicine)
        {
            try
            {
                if (medicine != null)
                {
                    this.IsMedicine = true;
                    this.IsBlood = false;
                    this.SERVICE_ID = medicine.SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = medicine.ID;
                    this.MEDI_MATE_TYPE_CODE = medicine.MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
                    //this.CONCENTRA = medicine.CONCENTRA;
                    this.SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = medicine.NATIONAL_NAME;
                    this.MANUFACTURER_NAME = medicine.MANUFACTURER_NAME;
                    this.REGISTER_NUMBER = medicine.REGISTER_NUMBER;
                    this.AVAILABLE_AMOUNT = medicine.AvailableAmount;
                    this.IMP_PRICE = medicine.IMP_PRICE;
                    this.IMP_VAT_RATIO = medicine.IMP_VAT_RATIO;
                    this.ExpMedicine = new ExpMedicineSDO();
                    this.ExpMedicine.MedicineId = medicine.ID;                   
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(HisMaterialInStockSDO material)
        {
            try
            {
                if (material != null)
                {
                    this.IsMedicine = false;
                    this.IsBlood = false;
                    this.SERVICE_ID = material.SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = material.ID;
                    this.MEDI_MATE_TYPE_CODE = material.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = material.NATIONAL_NAME;
                    this.MANUFACTURER_NAME = material.MANUFACTURER_NAME;
                    this.AVAILABLE_AMOUNT = material.AvailableAmount;
                    this.IMP_PRICE = material.IMP_PRICE;
                    this.IMP_VAT_RATIO = material.IMP_VAT_RATIO;
                    this.ExpMaterial = new ExpMaterialSDO();
                    this.ExpMaterial.MaterialId = material.ID;
                    //this.ExpMaterial.MaterialTypeId = material.Id;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(V_HIS_BLOOD blood)
        {
            try
            {
                if (blood != null)
                {
                    this.IsMedicine = false;
                    this.IsBlood = true;
                    this.SERVICE_ID = blood.SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = blood.ID;
                    this.MEDI_MATE_TYPE_CODE = blood.BLOOD_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = blood.BLOOD_TYPE_NAME;

                    this.VOLUME = blood.VOLUME;
                    this.BLOOD_ABO_ID = blood.BLOOD_ABO_ID;
                    this.BLOOD_RH_ID = blood.BLOOD_RH_ID;
                    this.BLOOD_ABO_CODE = blood.BLOOD_ABO_CODE;
                    this.BLOOD_RH_CODE = blood.BLOOD_RH_CODE;
                    this.BLOOD_ID = blood.ID;
                    this.BLOOD_CODE = blood.BLOOD_CODE;
                    this.EXPIRED_DATE = blood.EXPIRED_DATE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(V_HIS_EXP_MEST_MEDICINE medicine)
        {
            try
            {
                if (medicine != null)
                {
                    this.IsMedicine = true;
                    this.IsBlood = false;
                    this.SERVICE_ID = medicine.SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = medicine.MEDICINE_ID ?? 0;
                    this.MEDI_MATE_TYPE_CODE = medicine.MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
                   // this.CONCENTRA = medicine.CONCENTRA;
                    this.SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = medicine.NATIONAL_NAME;
                    this.MANUFACTURER_NAME = medicine.MANUFACTURER_NAME;
                    this.REGISTER_NUMBER = medicine.REGISTER_NUMBER;
                    this.EXP_AMOUNT = medicine.AMOUNT;
                    this.EXP_PRICE = medicine.PRICE;
                    this.EXP_VAT_RATIO = medicine.VAT_RATIO;
                    this.EXPIRED_DATE = medicine.EXPIRED_DATE;
                    this.DISCOUNT = medicine.DISCOUNT;
                    this.NOTE = medicine.DESCRIPTION;
                    this.IMP_PRICE = medicine.IMP_PRICE;
                    this.IMP_VAT_RATIO = medicine.IMP_VAT_RATIO;

                    this.ExpMedicine = new ExpMedicineSDO();
                    
                    this.ExpMedicine.MedicineId = medicine.MEDICINE_ID ?? 0;
                    this.ExpMedicine.Amount = medicine.AMOUNT;
                    this.ExpMedicine.Description = medicine.DESCRIPTION;
                    this.ExpMedicine.DiscountRatio = medicine.DISCOUNT;
                    this.ExpMedicine.Price = medicine.PRICE;
                    this.ExpMedicine.VatRatio = medicine.VAT_RATIO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(V_HIS_EXP_MEST_MATERIAL material)
        {
            try
            {
                if (material != null)
                {
                    this.IsMedicine = false;
                    this.IsBlood = false;
                    this.SERVICE_ID = material.SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = material.MATERIAL_ID ?? 0;
                    this.MEDI_MATE_TYPE_CODE = material.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = material.NATIONAL_NAME;
                    this.MANUFACTURER_NAME = material.MANUFACTURER_NAME;
                    this.EXP_AMOUNT = material.AMOUNT;
                    this.EXP_PRICE = material.PRICE;
                    this.EXP_VAT_RATIO = material.VAT_RATIO;
                    this.EXPIRED_DATE = material.EXPIRED_DATE;
                    this.DISCOUNT = material.DISCOUNT;
                    this.NOTE = material.DESCRIPTION;
                    this.IMP_PRICE = material.IMP_PRICE;
                    this.IMP_VAT_RATIO = material.IMP_VAT_RATIO;

                    this.ExpMaterial = new ExpMaterialSDO();
                    this.ExpMaterial.MaterialId = material.MATERIAL_ID ?? 0;
                    this.ExpMaterial.Amount = material.AMOUNT;
                    this.ExpMaterial.Description = material.DESCRIPTION;
                    this.ExpMaterial.DiscountRatio = material.DISCOUNT;
                    this.ExpMaterial.Price = material.PRICE;
                    this.ExpMaterial.VatRatio = material.VAT_RATIO;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(V_HIS_EXP_MEST_BLOOD blood)
        {
            try
            {
                if (blood != null)
                {
                    this.IsMedicine = false;
                    this.IsBlood = true;
                    this.SERVICE_ID = blood.SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = blood.BLOOD_ID;
                    this.MEDI_MATE_TYPE_CODE = blood.BLOOD_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = blood.BLOOD_TYPE_NAME;

                    this.VOLUME = blood.VOLUME;
                    this.BLOOD_ABO_ID = blood.BLOOD_ABO_ID;
                    this.BLOOD_RH_ID = blood.BLOOD_RH_ID;
                    this.BLOOD_ABO_CODE = blood.BLOOD_ABO_CODE;
                    this.BLOOD_RH_CODE = blood.BLOOD_RH_CODE;
                    this.BLOOD_ID = blood.BLOOD_ID;
                    this.BLOOD_CODE = blood.BLOOD_CODE;
                    this.EXPIRED_DATE = blood.EXPIRED_DATE;
                    this.EXP_AMOUNT = 1;

                    this.ExpBlood = new ExpBloodSDO();
                    this.ExpBlood.BloodId = blood.BLOOD_ID;
                    this.ExpBlood.Description = blood.DESCRIPTION;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(V_HIS_MATERIAL_BEAN_1 material)
        {
            try
            {
                if (material != null)
                {
                    this.IsMedicine = false;
                    this.IsBlood = false;
                    this.IsReuse = true;
                    this.SERVICE_ID = material.SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = material.MATERIAL_TYPE_ID;
                    this.MEDI_MATE_TYPE_CODE = material.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = material.NATIONAL_NAME;
                    this.SERIAL_NUMBER = material.SERIAL_NUMBER;
                    this.TDL_MATERIAL_MAX_REUSE_COUNT = material.TDL_MATERIAL_MAX_REUSE_COUNT;
                    this.ExpMaterial = new ExpMaterialSDO();
                    this.ExpMaterial.MaterialId = material.MATERIAL_ID;
                    this.ExpMaterial.Amount = material.AMOUNT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
