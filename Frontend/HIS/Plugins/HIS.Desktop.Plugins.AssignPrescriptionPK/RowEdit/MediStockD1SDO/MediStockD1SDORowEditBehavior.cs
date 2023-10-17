﻿using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Base;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.MessageBoxForm;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Worker;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Edit.MediStockD1SDO
{
    class MediStockD1SDORowEditBehavior : EditAbstract, IEdit
    {
        decimal AmountInBean { get; set; }
        MediMatyTypeADO medicineTypeSDO__Category__SameMediAcin;
        long expMestId;

        internal MediStockD1SDORowEditBehavior(CommonParam param,
            frmAssignPrescription frmAssignPrescription,
            ValidAddRow validAddRow,
            HIS.Desktop.Plugins.AssignPrescriptionPK.MediMatyCreateWorker.ChoosePatientTypeDefaultlService choosePatientTypeDefaultlService,
            HIS.Desktop.Plugins.AssignPrescriptionPK.MediMatyCreateWorker.ChoosePatientTypeDefaultlServiceOther choosePatientTypeDefaultlServiceOther,
            CalulateUseTimeTo calulateUseTimeTo,
            ExistsAssianInDay existsAssianInDay,
            object dataRow)
            : base(param,
             frmAssignPrescription,
             validAddRow,
             choosePatientTypeDefaultlService,
             choosePatientTypeDefaultlServiceOther,
             calulateUseTimeTo,
             existsAssianInDay,
             dataRow)
        {
            this.DataType = (currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC ? HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC : HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU);
            this.Code = currentMedicineTypeADOForEdit.MEDICINE_TYPE_CODE;
            this.Name = currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
            this.ManuFacturerName = currentMedicineTypeADOForEdit.MANUFACTURER_NAME;
            this.ServiceUnitName = currentMedicineTypeADOForEdit.SERVICE_UNIT_NAME;
            this.NationalName = currentMedicineTypeADOForEdit.NATIONAL_NAME;
            this.ServiceId = currentMedicineTypeADOForEdit.SERVICE_ID;
            this.Concentra = currentMedicineTypeADOForEdit.CONCENTRA;
            this.MediStockId = currentMedicineTypeADOForEdit.MEDI_STOCK_ID;
            this.MediStockCode = currentMedicineTypeADOForEdit.MEDI_STOCK_CODE;
            this.MediStockName = currentMedicineTypeADOForEdit.MEDI_STOCK_NAME;
            this.HeinServiceTypeId = currentMedicineTypeADOForEdit.HEIN_SERVICE_TYPE_ID;
            this.ServiceTypeId = currentMedicineTypeADOForEdit.SERVICE_TYPE_ID;
            this.ActiveIngrBhytCode = currentMedicineTypeADOForEdit.ACTIVE_INGR_BHYT_CODE;
            this.ActiveIngrBhytName = currentMedicineTypeADOForEdit.ACTIVE_INGR_BHYT_NAME;
            this.IsOutKtcFee = ((currentMedicineTypeADOForEdit.IS_OUT_PARENT_FEE ?? -1) == 1);
            this.IsUseOrginalUnitForPres = currentMedicineTypeADOForEdit.IsUseOrginalUnitForPres;
            this.IS_SPLIT_COMPENSATION = currentMedicineTypeADOForEdit.IS_SPLIT_COMPENSATION;
            this.IS_OUT_HOSPITAL = frmAssignPrescription.currentMedicineTypeADOForEdit.IS_OUT_HOSPITAL;
            if (!String.IsNullOrEmpty(frmAssignPrescription.txtPreviousUseDay.Text) && Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtPreviousUseDay.Text) > 0)
                this.PREVIOUS_USING_COUNT = Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.txtPreviousUseDay.Text);
            else
                this.PREVIOUS_USING_COUNT = null;
            //Chi dinh tu man hinh phau thuat, thu thuat
            var stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == this.MediStockId);
            if (stock != null && stock.IS_EXPEND == 1)
            {
                this.IsExpend = true;
                this.IsDisableExpend = true;
            }
            else if(
                ((currentMedicineTypeADOForEdit.IS_AUTO_EXPEND ?? -1) == 1)
                || (HisConfigCFG.IsAutoTickExpendWithAssignPresPTTT && frmAssignPrescription.isAutoCheckExpend == true
                    && (this.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                        || (this.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                            && currentMedicineTypeADOForEdit.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM
                            || currentMedicineTypeADOForEdit.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM))))
            {
                this.IsExpend = true;
            }
            else
            {
                this.IsExpend = currentMedicineTypeADOForEdit.IsExpend;
                this.IsDisableExpend = currentMedicineTypeADOForEdit.IsDisableExpend;
            }
            this.AmountInBean = (this.currentMedicineTypeADOForEdit.AMOUNT ?? 0);
            this.AmountAvaiable = this.AmountOutOfStock(this.ServiceId, (this.MediStockId ?? 0));
            this.expMestId = frmAssignPrescription.oldExpMestId;
            this.Speed = frmAssignPrescription.spinTocDoTruyen.EditValue != null ? (decimal?)frmAssignPrescription.spinTocDoTruyen.Value : null;
            this.PackageNumber = currentMedicineTypeADOForEdit.TDL_PACKAGE_NUMBER;
            this.ExpiredDate = currentMedicineTypeADOForEdit.EXPIRED_DATE;
            this.IsAssignPackage = currentMedicineTypeADOForEdit.IsAssignPackage;
            this.MAME_ID = frmAssignPrescription.currentMedicineTypeADOForEdit.MAME_ID;
            this.SERVICE_CONDITION_ID = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_CONDITION_ID;
            this.SERVICE_CONDITION_NAME = frmAssignPrescription.currentMedicineTypeADOForEdit.SERVICE_CONDITION_NAME;
            this.IS_SUB_PRES = frmAssignPrescription.currentMedicineTypeADOForEdit.IS_SUB_PRES;
        }

        bool IEdit.Run()
        {
            bool success = true;
            try
            {
                //this.CreateADO(true);//tiennv old input false
                if (this.ValidMetyMatyType__Add())
                {
                    if (medicineTypeSDO__Category__SameMediAcin != null)
                    {
                        return false;
                    }
                    else
                    {
                        medicineTypeSDO = frmAssignPrescription.mediMatyTypeADOs.FirstOrDefault(o => o.PrimaryKey == PrimaryKey);
                        if (frmAssignPrescription.intructionTimeSelected != null && frmAssignPrescription.intructionTimeSelected.Count < 2 && !frmAssignPrescription.IsSelectMultiPatient())
                        {
                            if (!frmAssignPrescription.GetOverReason(medicineTypeSDO, false, true))
                                return success;
                        }
                        this.CreateADO();
                        this.medicineTypeSDO.UpdateAmountAutoRoundUpByAllowOddInDataRow(this.medicineTypeSDO);
                        this.medicineTypeSDO.UpdateAutoRoundUpByConvertUnitRatioInDataRow(this.medicineTypeSDO,frmAssignPrescription.VHistreatment);
                        this.UpdateMedicinePackageInfoInDataRow(this.medicineTypeSDO);
                        this.UpdatePatientTypeInDataRow(this.medicineTypeSDO);
                        this.UpdateExpMestReasonInDataRow(this.medicineTypeSDO);
                        this.UpdateMedicineUseFormInDataRow(this.medicineTypeSDO);
                        this.UpdateUseTimeInDataRow(this.medicineTypeSDO);
                        this.SetValidError();

                        if (medicineTypeSDO.ErrorMessageMedicineUseForm == ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung
                           && medicineTypeSDO.ErrorTypeMedicineUseForm == ErrorType.Warning)
                        {
                            MessageManager.Show(ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung);
                            frmAssignPrescription.cboMedicineUseForm.Focus();
                            frmAssignPrescription.cboMedicineUseForm.ShowPopup();
                            success = false;
                        }
                        else
                        {
                            List<MediMatyTypeADO> mediMatyTypeADOTemps = new List<MediMatyTypeADO>();
                            if (this.medicineTypeSDO != null && medicineTypeSDO.IsStent == true
                                && this.medicineTypeSDO.AMOUNT > 1)
                            {
                                mediMatyTypeADOTemps = MediMatyProcessor.MakeMaterialSingleStent(medicineTypeSDO);
                            }
                            else
                            {
                                mediMatyTypeADOTemps.Add(this.medicineTypeSDO);
                            }
                            CommonParam param = new CommonParam();
                            foreach (var item in mediMatyTypeADOTemps)
                            {
                                if ((!GlobalStore.IsTreatmentIn && ((frmAssignPrescription.serviceReqMain != null && frmAssignPrescription.serviceReqMain.IS_MAIN_EXAM == 1) || (frmAssignPrescription.serviceReqMain != null && frmAssignPrescription.serviceReqMain.IS_SUB_PRES != 1)) && (HisConfigCFG.IsUsingSubPrescriptionMechanism == "1")) || (!GlobalStore.IsTreatmentIn && HisConfigCFG.IsUsingSubPrescriptionMechanism != "1") || GlobalStore.IsCabinet)
                                {
                                    frmAssignPrescription.lstOutPatientPres = frmAssignPrescription.lstOutPatientPres.Where(o => o.PrimaryKey != item.PrimaryKey).ToList();
                                    if (mediMatyTypeADOTemps.IndexOf(item) != 0)
                                    {
                                        item.MedicineBean1Result = null;
                                        item.MaterialBean1Result = null;
                                        item.ExpMestDetailIds = null;
                                        if (!TakeOrReleaseBeanWorker.TakeForCreateBean(frmAssignPrescription.intructionTimeSelecteds, this.expMestId, item, true, param,frmAssignPrescription.UseTimeSelecteds,frmAssignPrescription.lstOutPatientPres))
                                        {
                                            MessageManager.Show(Param, success);
                                            return success = false;
                                        }
                                    }
                                    else
                                    {
                                        //decimal amount = ((item.IsUseOrginalUnitForPres ?? false) == false && (item.CONVERT_RATIO ?? 0) > 0) ? (item.AMOUNT ?? 0) / (item.CONVERT_RATIO ?? 0) : (item.AMOUNT ?? 0);
                                        if (!TakeOrReleaseBeanWorker.TakeForUpdateBean(frmAssignPrescription.intructionTimeSelecteds, this.expMestId, item, (item.AMOUNT ?? 0), true, param, this.frmAssignPrescription.UseTimeSelecteds, this.frmAssignPrescription.lstOutPatientPres))
                                        {
                                            MessageManager.Show(Param, success);
                                            return success = false;
                                        }
                                    }
                                }
                                else
                                {
                                    item.TotalPrice = frmAssignPrescription.CalculatePrice(item);
                                    //if (frmAssignPrescription.servicePatyAllows != null && frmAssignPrescription.servicePatyAllows.ContainsKey(item.SERVICE_ID))
                                    //{
                                    //    var data_ServicePrice = frmAssignPrescription.servicePatyAllows[item.SERVICE_ID].Where(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID).OrderByDescending(m => m.MODIFY_TIME).ToList();
                                    //    if (data_ServicePrice != null && data_ServicePrice.Count > 0)
                                    //    {
                                    //        item.TotalPrice = (data_ServicePrice[0].PRICE * item.AMOUNT) ?? 0;
                                    //    }
                                    //}                                    
                                }
                                var primaryOld = item.PrimaryKey;
                                item.PrimaryKey = (item.SERVICE_ID + "__" + Inventec.Common.DateTime.Get.Now() + "__" + Guid.NewGuid().ToString());
                                foreach (var itemTakeBean in frmAssignPrescription.lstOutPatientPres)
                                {
                                    if (itemTakeBean.PrimaryKey == primaryOld)
                                        itemTakeBean.PrimaryKey = item.PrimaryKey;
                                }
                            }

                            if (success)
                            {
                                frmAssignPrescription.mediMatyTypeADOs.Remove(this.medicineTypeSDO);
                                frmAssignPrescription.mediMatyTypeADOs.AddRange(mediMatyTypeADOTemps);
                            }

                            this.SaveDataAndRefesh();
                            frmAssignPrescription.ReloadDataAvaiableMediBeanInCombo();
                        }
                    }
                }
                else
                {
                    LogSystem.Debug("IEdit.Run => CheckValidPre = false");
                }
            }
            catch (Exception ex)
            {
                success = false;
                MessageManager.Show(Param, success);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        protected void UpdateMedicinePackageInfoInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                if (this.IsAssignPackage.HasValue && this.IsAssignPackage.Value)
                {
                    medicineTypeSDO.MAME_ID = this.MAME_ID;
                    medicineTypeSDO.IsAssignPackage = true;
                    medicineTypeSDO.TDL_PACKAGE_NUMBER = this.PackageNumber;
                    medicineTypeSDO.EXPIRED_DATE = this.ExpiredDate;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool ValidMetyMatyType__Add()
        {
            bool valid = true;
            try
            {
                valid = valid && this.CheckValidPre();
                if (this.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    valid = valid && this.ValidTutorial();
                    valid = valid && ValidAcinInteractiveWorker.ValidGrade(this.DataRow, MediMatyTypeADOs, ref frmAssignPrescription.txtInteractionReason, frmAssignPrescription);
                    valid = valid && MedicineAgeWorker.ValidThuocCoGioiHanTuoi(this.ServiceId, frmAssignPrescription.patientDob);
                    valid = valid && WarningOddConvertWorker.CheckWarningOddConvertAmount(frmAssignPrescription.currentMedicineTypeADOForEdit, this.Amount, frmAssignPrescription.ResetFocusMediMaty);
                    valid = valid && ValidAcinInteractiveWorker.ValidSameAcin(MediMatyTypeADOs, frmAssignPrescription.currentMedicineTypeADOForEdit);
                    valid = valid && ValidThuocWithContraindicaterWarningOption();
                }

                valid = valid && this.ValidKhaDungThuocTrongKho();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        /// <summary>
        ///Khi kê số lượng vượt quá số lượng tồn, mà trong kho có thuốc cùng hoạt chất bhyt đang còn tồn thì hiển thị 
        ///thông báo để người dùng chọn, thông báo dưới dạng option:
        ///"Thuốc trong kho không đủ để kê. Bạn muốn sử dụng thuốc thay thế:
        ///1. Thuốc cùng hoạt chất bhyt
        ///2. Thuốc ngoài kho"
        ///nếu người dùng chọn thuốc cùng hoạt chất, thì copy tên hoạt chất vào ô tìm kiếm ==> tìm ra các thuốc cùng hoạt chất khác để người dùng chọn
        ///Trong trường hợp, số lượng vượt quá tồn, mà trong kho cũng không có thuốc nào cùng hoạt chất đang còn tồn thì hiển thị thông báo kiêu 
        ///"Thuốc đã chọn và các thuốc cùng hoạt chất khác trong kho không đủ để kê. Bạn có muốn kê thuốc ngoài kho không". Nếu chọn ok thì lấy thuốc ngoài kho, nếu ko thì ko xử lý j cả
        /// </summary>
        /// <returns>bool</returns>
        private bool ValidKhaDungThuocTrongKho()
        {
            bool valid = true;
            CommonParam param = new CommonParam();
            try
            {
                try
                {
                    if ((frmAssignPrescription.currentMedicineTypeADOForEdit.BK_AMOUNT > 0 && frmAssignPrescription.currentMedicineTypeADOForEdit.BK_AMOUNT == this.Amount))
                    {
                        return true;
                    }

                    //Lay thuoc trong kho va kiem tra thuoc co con trong kho khong
                    decimal damount = AmountOutOfStock(this.ServiceId, (this.MediStockId ?? 0));
                    if (damount <= 0)
                    {
                        param.Messages.Add(ResourceMessage.ThuocKhongCoTrongKho);
                        throw new ArgumentNullException("medicinetypeStockSDO is null");
                    }
                    Rectangle buttonBounds = new Rectangle(frmAssignPrescription.txtMediMatyForPrescription.Bounds.X, frmAssignPrescription.txtMediMatyForPrescription.Bounds.Y, frmAssignPrescription.txtMediMatyForPrescription.Bounds.Width, frmAssignPrescription.txtMediMatyForPrescription.Bounds.Height);
                    var medicineTypeTempSDO = frmAssignPrescription.mediMatyTypeADOs.FirstOrDefault(o => o.PrimaryKey == PrimaryKey);
                    if (this.Amount > (this.AmountAvaiable + (medicineTypeTempSDO.BK_AMOUNT ?? 0)))
                    {
                        var medicineTypeAcin__SameAcinBhyt = GetDataByActiveIngrBhyt();
                        if (!String.IsNullOrEmpty(medicineTypeAcin__SameAcinBhyt))
                        {
                            frmMessageBoxChooseAcinBhyt form = new frmMessageBoxChooseAcinBhyt(ChonThuocTrongKhoCungHoatChat);
                            form.ShowDialog();

                            switch (this.ChonThuocThayThe)
                            {
                                case OptionChonThuocThayThe.None:
                                    frmAssignPrescription.spinAmount.SelectAll();
                                    frmAssignPrescription.spinAmount.Focus();
                                    valid = false;
                                    break;
                                case OptionChonThuocThayThe.ThuocCungHoatChat:
                                    //thì copy tên hoạt chất vào ô tìm kiếm ==> tìm ra các thuốc cùng hoạt chất khác để người dùng chọn
                                    frmAssignPrescription.txtMediMatyForPrescription.Text = medicineTypeAcin__SameAcinBhyt;
                                    frmAssignPrescription.gridViewMediMaty.ActiveFilterString = "[MEDICINE_TYPE_NAME] Like '%" + frmAssignPrescription.txtMediMatyForPrescription.Text
                                        + "%' OR [MEDICINE_TYPE_CODE] Like '%" + frmAssignPrescription.txtMediMatyForPrescription.Text + "%'"
                                    + " OR [ACTIVE_INGR_BHYT_NAME] Like '%" + frmAssignPrescription.txtMediMatyForPrescription.Text + "%'";
                                    //+ " OR [CONCENTRA] Like '%" + txtMediMatyForPrescription.Text + "%'"
                                    //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
                                    frmAssignPrescription.gridViewMediMaty.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                                    frmAssignPrescription.gridViewMediMaty.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                                    frmAssignPrescription.gridViewMediMaty.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                                    frmAssignPrescription.gridViewMediMaty.FocusedRowHandle = 0;
                                    frmAssignPrescription.gridViewMediMaty.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
                                    frmAssignPrescription.gridViewMediMaty.OptionsFind.HighlightFindResults = true;


                                    frmAssignPrescription.popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                                    frmAssignPrescription.txtMediMatyForPrescription.Focus();
                                    frmAssignPrescription.txtMediMatyForPrescription.SelectAll();
                                    valid = false;
                                    break;
                                case OptionChonThuocThayThe.ThuocNgoaiKho:
                                    //Trong trường hợp, số lượng vượt quá tồn, mà trong kho cũng không có thuốc nào cùng hoạt chất đang còn tồn thì hiển thị thông báo kiêu 
                                    //"Thuốc đã chọn và các thuốc cùng hoạt chất khác trong kho không đủ để kê. Bạn có muốn kê thuốc ngoài kho không". 
                                    //Nếu chọn ok thì lấy thuốc ngoài kho, nếu ko thì ko xử lý j cả
                                    if (frmAssignPrescription.currentMedicineTypes == null || frmAssignPrescription.currentMedicineTypes.Count == 0)
                                    {
                                        frmAssignPrescription.currentMedicineTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>();
                                        long isOnlyDisplayMediMateIsBusiness = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS.Desktop.Plugins.AssignPrescriptionPK.Config.HisConfigCFG.ONLY_DISPLAY_MEDIMATE_IS_BUSINESS));
                                        if (isOnlyDisplayMediMateIsBusiness == 1 && frmAssignPrescription.currentMedicineTypes != null && frmAssignPrescription.currentMedicineTypes.Count > 0)
                                            frmAssignPrescription.currentMedicineTypes = frmAssignPrescription.currentMedicineTypes.Where(o => o.IS_BUSINESS.HasValue && o.IS_BUSINESS.Value == 1).ToList();
                                    }

                                    var medicineType = frmAssignPrescription.currentMedicineTypes.FirstOrDefault(o => o.SERVICE_ID == this.ServiceId);
                                    if (medicineType == null)
                                        throw new ArgumentNullException("Khong tim thay medicineType SERVICE_ID = " + this.ServiceId + " tu danh muc thuoc.");
                                    if (frmAssignPrescription.rdOpionGroup.Properties.Items.Count > 1 && frmAssignPrescription.rdOpionGroup.Properties.Items[1].Enabled)
                                    {
                                        frmAssignPrescription.rdOpionGroup.SelectedIndex = 1;
                                        frmAssignPrescription.MedicineType_RowClick(medicineType);
                                        frmAssignPrescription.popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                                        frmAssignPrescription.txtMediMatyForPrescription.Focus();
                                    }
                                    valid = false;
                                    break;
                            }
                        }
                        else
                        {
                            param.Messages.Add(ResourceMessage.SoLuongXuatLonHonSpoLuongKhadungTrongKho);
                            throw new ArgumentNullException("medicinetypeStockSDO is null");
                        }
                    }
                }
                catch (ArgumentNullException ex)
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                catch (Exception ex)
                {
                    valid = false;
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

                if (!String.IsNullOrEmpty(param.GetMessage()))
                    MessageManager.Show(param.GetMessage());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return valid;
        }

        private void AddMedicineTypeCategoryBySameMediAcin(V_HIS_MEDICINE_TYPE addMedicineTypeADO)
        {
            try
            {
                if (addMedicineTypeADO == null) throw new ArgumentNullException("currentMedicineTypeADO");

                medicineTypeSDO__Category__SameMediAcin = new MediMatyTypeADO();
                Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(medicineTypeSDO__Category__SameMediAcin, addMedicineTypeADO);
                medicineTypeSDO__Category__SameMediAcin.DataType = HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM;
                medicineTypeSDO__Category__SameMediAcin.AMOUNT = this.Amount;
                medicineTypeSDO__Category__SameMediAcin.BK_AMOUNT = this.Amount;
                medicineTypeSDO__Category__SameMediAcin.NUM_ORDER = this.NumOrder;
                medicineTypeSDO__Category__SameMediAcin.Sang = this.Sang;
                medicineTypeSDO__Category__SameMediAcin.Trua = this.Trua;
                medicineTypeSDO__Category__SameMediAcin.Chieu = this.Chieu;
                medicineTypeSDO__Category__SameMediAcin.Toi = this.Toi;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChonThuocTrongKhoCungHoatChat(OptionChonThuocThayThe chonThuocThayThe)
        {
            try
            {
                this.ChonThuocThayThe = chonThuocThayThe;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
