﻿using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.Add.MedicineTypeOther
{
    class MedicineTypeOtherRowAddBehavior : AddAbstract, IAdd
    {
        internal MedicineTypeOtherRowAddBehavior(CommonParam param,
            frmAssignPrescription frmAssignPrescription,
            ValidAddRow validAddRow,
            GetPatientTypeBySeTy getPatientTypeBySeTy,
            CalulateUseTimeTo calulateUseTimeTo,
            ExistsAssianInDay existsAssianInDay,
            object dataRow)
            : base(param,
             frmAssignPrescription,
             validAddRow,
             getPatientTypeBySeTy,
             calulateUseTimeTo,
             existsAssianInDay,
             dataRow)
        {
            this.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC;
            this.Name = frmAssignPrescription.txtMedicineTypeOther.Text.Trim();
            this.ServiceUnitName = frmAssignPrescription.txtUnitOther.Text.Trim();
            if (frmAssignPrescription.spinPrice.EditValue != null)
                this.Price = frmAssignPrescription.spinPrice.Value;
            this.DataRow = this.Name;
        }

        bool IAdd.Run()
        {
            bool success = false;
            try
            {
                if (this.CheckValidPre()
                    && ValidUnitName())
                {
                    this.CreateADO();
                    medicineTypeSDO.AMOUNT = this.Amount;
                    medicineTypeSDO.SERVICE_UNIT_NAME = this.ServiceUnitName;
                    medicineTypeSDO.MEDICINE_TYPE_NAME = this.Name;
                    medicineTypeSDO.PRICE = this.Price;
                    medicineTypeSDO.TotalPrice = (this.Amount * (medicineTypeSDO.PRICE ?? 0));
                    medicineTypeSDO.CONVERT_RATIO = null;
                    medicineTypeSDO.CONVERT_UNIT_CODE = null;
                    medicineTypeSDO.CONVERT_UNIT_NAME = null;

                    this.medicineTypeSDO.PrimaryKey = (this.medicineTypeSDO.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                    this.SaveDataAndRefesh(this.medicineTypeSDO);
                    //frmAssignPrescription.ReloadDataAvaiableMediBeanInCombo();
                    success = true;
                }
                else
                {
                    this.medicineTypeSDO = null;
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.medicineTypeSDO = null;
                MessageManager.Show(Param, success);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        bool ValidUnitName()
        {
            bool valid = true;
            try
            {
                valid = !String.IsNullOrEmpty(this.ServiceUnitName);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}
