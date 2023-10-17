﻿using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestChmsCreate.ADO
{
    public class MediMateTypeADO
    {
        public ExpMaterialTypeSDO ExpMaterial { get; set; }
        public ExpMedicineTypeSDO ExpMedicine { get; set; }
        public ExpBloodTypeSDO ExpBlood { get; set; }

        public long ID { get; set; }
        public long SERVICE_ID { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public long Type { get; set; }

        public string SERVICE_UNIT_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public decimal? AVAILABLE_AMOUNT { get; set; }
        public long MEDICINE_ID { get; set; }
        public decimal? EXPIRED_DATE { get; set; }
        public long MATERIAL_ID { get; set; }
        public bool IsPackage { get; set; }

        public bool IsSupplement { get; set; }

        
        public decimal EXP_AMOUNT { get; set; }
        public string NOTE { get; set; }

        public bool IsMedicine { get; set; }
        public long NUM_ORDER { get; set; }

        public bool IsBlood { get; set; }
        public long? BLOOD_RH_ID { get; set; }
        public long? BLOOD_ABO_ID { get; set; }
        public decimal VOLUME { get; set; }
        public string BLOOD_TYPE_HEIN_NAME { get; set; }
        public long? MEDI_STOCK_ID { get; set; }

        public long? MEDI_STOCK_ID_IPM { get; set; }
        public string EXPIRED_DATE_STRS { get; set; }

        public ICollection<HisMedicineInStockADO> lstMedicineInStock { get; set; }
        public ICollection<HisMaterialInStockADO> lstMaterialInStock { get; set; }

        public MediMateTypeADO() { }

        public MediMateTypeADO(HisMedicineInStockADO medicine)
        {
            try
            {
                if (medicine != null)
                {
                    this.ID = medicine.ID;
                    this.Type = 1;
                    this.IsMedicine = true;
                    this.SERVICE_ID = medicine.SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = medicine.MEDICINE_TYPE_ID;
                    this.MEDI_MATE_TYPE_CODE = medicine.MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = medicine.MEDICINE_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = medicine.NATIONAL_NAME;
                    this.MANUFACTURER_NAME = medicine.MANUFACTURER_NAME;
                    this.AVAILABLE_AMOUNT = medicine.AvailableAmount;
                    this.REGISTER_NUMBER = medicine.REGISTER_NUMBER;
                    this.ExpMedicine = new ExpMedicineTypeSDO();
                    this.ExpMedicine.MedicineTypeId = medicine.MEDICINE_TYPE_ID;                  
                    this.MEDICINE_ID = medicine.ID;
                    this.PACKAGE_NUMBER = medicine.PACKAGE_NUMBER;
                    this.EXPIRED_DATE = medicine.EXPIRED_DATE;
                    this.MEDI_STOCK_ID = medicine.MEDI_STOCK_ID;
                    this.EXPIRED_DATE_STRS = medicine.EXPIRED_DATE_STRS;
                    this.lstMedicineInStock = new List<HisMedicineInStockADO>();
                    this.lstMedicineInStock = medicine.MedicineInStocks;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(HisMaterialInStockADO material)
        {
            try
            {
                if (material != null)
                {
                    this.ID = material.ID;
                    this.Type = 2;
                    this.IsMedicine = false;
                    this.SERVICE_ID = material.SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = material.MATERIAL_TYPE_ID;
                    this.MEDI_MATE_TYPE_CODE = material.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = material.NATIONAL_NAME;
                    this.MANUFACTURER_NAME = material.MANUFACTURER_NAME;
                    this.AVAILABLE_AMOUNT = material.AvailableAmount;
                    this.REGISTER_NUMBER = material.REGISTER_NUMBER;
                    this.ExpMaterial = new ExpMaterialTypeSDO();
                    this.ExpMaterial.MaterialTypeId = material.MATERIAL_TYPE_ID;
                    this.MATERIAL_ID = material.ID;
                    this.PACKAGE_NUMBER = material.PACKAGE_NUMBER;
                    this.EXPIRED_DATE = material.EXPIRED_DATE;
                    this.MEDI_STOCK_ID = material.MEDI_STOCK_ID;
                    this.EXPIRED_DATE_STRS = material.EXPIRED_DATE_STRS;
                    this.lstMaterialInStock = new List<HisMaterialInStockADO>();
                    this.lstMaterialInStock = material.MaterialInStocks;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(HisBloodTypeInStockADO blood)
        {
            try
            {
                if (blood != null)
                {
                    this.ID = blood.Id;
                    this.Type = 3;
                    this.IsBlood = true;
                    this.SERVICE_ID = blood.ServiceId;
                    this.MEDI_MATE_TYPE_ID = blood.Id;
                    this.MEDI_MATE_TYPE_CODE = blood.BloodTypeCode;
                    this.MEDI_MATE_TYPE_NAME = blood.BloodTypeName;
                    this.BLOOD_TYPE_HEIN_NAME = blood.BloodTypeHeinName;
                    this.VOLUME = blood.Volume;
                    this.AVAILABLE_AMOUNT = blood.Amount;
                    this.ExpBlood = new ExpBloodTypeSDO();
                    this.ExpBlood.BloodTypeId = blood.Id;
                    this.MEDI_STOCK_ID = blood.MediStockId;
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
                    this.SERVICE_ID = material.SERVICE_ID;
                    this.MEDI_MATE_TYPE_ID = material.MATERIAL_TYPE_ID;
                    this.MEDI_MATE_TYPE_CODE = material.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = material.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = material.NATIONAL_NAME;
                    this.AVAILABLE_AMOUNT = material.AMOUNT;
                    this.ExpMaterial = new ExpMaterialTypeSDO();
                    this.ExpMaterial.MaterialTypeId = material.MATERIAL_TYPE_ID;
                    this.MEDI_STOCK_ID = material.MEDI_STOCK_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
