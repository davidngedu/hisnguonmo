﻿using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpBloodChmsCreate.ADO
{
    public class MediMateTypeADO
    {
        public HisMaterialTypeAmountSDO ExpMaterial { get; set; }
        public HisMedicineTypeAmountSDO ExpMedicine { get; set; }
        public HisBloodTypeAmountSDO ExpBlood { get; set; }

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
        public long BLOOD_RH_ID { get; set; }
        public long BLOOD_ABO_ID { get; set; }

        //public decimal? ALERT_MIN_IN_STOCK { get; set; }
        //public decimal MAX_AMOUNT { get; set; }
        public bool IsSupplement { get; set; }


        public decimal EXP_AMOUNT { get; set; }
        public string NOTE { get; set; }

        public bool IsMedicine { get; set; }
        public long NUM_ORDER { get; set; }
        public decimal VOLUME { get; set; }
        public string BLOOD_TYPE_HEIN_NAME { get; set; }

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
                    this.AVAILABLE_AMOUNT = medicine.AvailableAmount;
                    this.REGISTER_NUMBER = medicine.RegisterNumber;
                    this.ExpMedicine = new HisMedicineTypeAmountSDO();
                    this.ExpMedicine.MedicineTypeId = medicine.Id;
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
                    this.ExpMaterial = new HisMaterialTypeAmountSDO();
                    this.ExpMaterial.MaterialTypeId = material.Id;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MediMateTypeADO(HisBloodTypeInStockSDO blood)
        {
            try
            {
                if (blood != null)
                {
                    this.IsMedicine = false;
                    this.SERVICE_ID = blood.ServiceId;
                    this.MEDI_MATE_TYPE_ID = blood.Id;
                    this.MEDI_MATE_TYPE_CODE = blood.BloodTypeCode;
                    this.MEDI_MATE_TYPE_NAME = blood.BloodTypeName;
                    this.BLOOD_TYPE_HEIN_NAME = blood.BloodTypeHeinName;
                    this.VOLUME = blood.Volume;
                    this.AVAILABLE_AMOUNT = blood.Amount;
                    this.ExpBlood = new HisBloodTypeAmountSDO();
                    this.ExpBlood.BloodTypeId = blood.Id;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
