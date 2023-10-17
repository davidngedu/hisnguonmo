﻿using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.PrintTreatmentFinish;
using HIS.Desktop.Print;
using HIS.UC.MenuPrint.ADO;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void InitMenuToButtonPrint()
        {
            try
            {
                HIS.UC.MenuPrint.MenuPrintProcessor menuPrintProcessor = new HIS.UC.MenuPrint.MenuPrintProcessor();
                List<HIS.UC.MenuPrint.ADO.MenuPrintADO> menuPrintADOs = new List<HIS.UC.MenuPrint.ADO.MenuPrintADO>();

                HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__ThuocTongHop = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                menuPrintADO__ThuocTongHop.EventHandler = new EventHandler(OnClickInChiDinhDichVu);
                menuPrintADO__ThuocTongHop.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_DON_THUOC_TONG_HOP__MPS000118;
                menuPrintADO__ThuocTongHop.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_DON_THUOC_TONG_HOP__MPS000118;
                menuPrintADOs.Add(menuPrintADO__ThuocTongHop);

                HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__CacDonThuoc = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                menuPrintADO__CacDonThuoc.EventHandler = new EventHandler(OnClickInChiDinhDichVu);
                menuPrintADO__CacDonThuoc.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC__MPS000044;
                menuPrintADO__CacDonThuoc.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC__MPS000044;
                menuPrintADOs.Add(menuPrintADO__CacDonThuoc);

                HIS.UC.MenuPrint.ADO.MenuPrintADO menuPrintADO__YHocCoTruyen = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                menuPrintADO__YHocCoTruyen.EventHandler = new EventHandler(OnClickInChiDinhDichVu);
                menuPrintADO__YHocCoTruyen.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC_Y_HOC_CO_TRUYEN__MPS000050;
                menuPrintADO__YHocCoTruyen.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC_Y_HOC_CO_TRUYEN__MPS000050;
                menuPrintADOs.Add(menuPrintADO__YHocCoTruyen);

                var treatUC = ((this.treatmentFinishProcessor != null && this.ucTreatmentFinish != null) ? this.treatmentFinishProcessor.GetDataOutput(this.ucTreatmentFinish) : null);
                //Nếu người dùng tích chọn kết thúc điều trị > Chọn "Lưu" = Lưu toa thuốc + kết thúc điều trị (tự động in giấy hẹn khám, bảng kê nếu tích chọn) + Tự động close form kê toa + xử lý khám (có option theo user).
                if (treatUC != null && treatUC.IsAutoTreatmentFinish)
                {
                    if (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                    {
                        HIS.UC.MenuPrint.ADO.MenuPrintADO inGiayChuyenVienItem = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                        inGiayChuyenVienItem.EventHandler = new EventHandler(OnClickInChiDinhDichVu);
                        inGiayChuyenVienItem.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011;
                        inGiayChuyenVienItem.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011;

                        menuPrintADOs.Add(inGiayChuyenVienItem);

                        HIS.UC.MenuPrint.ADO.MenuPrintADO inGiayRaVienItem = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                        inGiayRaVienItem.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008;
                        inGiayRaVienItem.EventHandler = new EventHandler(OnClickInChiDinhDichVu);
                        inGiayRaVienItem.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008;

                        menuPrintADOs.Add(inGiayRaVienItem);
                    }
                    else if (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)//Hen kham
                    {
                        HIS.UC.MenuPrint.ADO.MenuPrintADO inGiayHenKhamLaiItem = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                        inGiayHenKhamLaiItem.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010;
                        inGiayHenKhamLaiItem.EventHandler = new EventHandler(OnClickInChiDinhDichVu);
                        inGiayHenKhamLaiItem.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010;

                        menuPrintADOs.Add(inGiayHenKhamLaiItem);
                    }
                    else if (treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN || treatUC.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN)//ra vien
                    {
                        HIS.UC.MenuPrint.ADO.MenuPrintADO inGiayRaVienItem = new HIS.UC.MenuPrint.ADO.MenuPrintADO();
                        inGiayRaVienItem.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008;
                        inGiayRaVienItem.EventHandler = new EventHandler(OnClickInChiDinhDichVu);
                        inGiayRaVienItem.PrintTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008;

                        menuPrintADOs.Add(inGiayRaVienItem);
                    }
                }

                //var printTypeNoGroup = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>()
                //    .FirstOrDefault(o =>
                //        (o.PRINT_TYPE_CODE == PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_DON_THUOC_TONG_HOP__MPS000118
                //        || o.PRINT_TYPE_CODE == PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC__MPS000044
                //        || o.PRINT_TYPE_CODE == PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC_Y_HOC_CO_TRUYEN__MPS000050)
                //        && o.IS_NO_GROUP == 1
                //    );
                lciPrintAssignPrescription.MaxSize = new System.Drawing.Size(440, 30);
                lciPrintAssignPrescription.MinSize = new System.Drawing.Size(140, 30);

                HIS.UC.MenuPrint.ADO.MenuPrintInitADO menuPrintInitADO = new HIS.UC.MenuPrint.ADO.MenuPrintInitADO(menuPrintADOs, BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>());
                menuPrintInitADO.MinSizeHeight = this.lcibtnSave.MinSize.Height;
                menuPrintInitADO.MaxSizeHeight = this.lcibtnSave.MaxSize.Height;
                menuPrintInitADO.ControlContainer = this.layoutControlPrintAssignPrescription;
                var menuResultADO = menuPrintProcessor.Run(menuPrintInitADO) as MenuPrintResultADO;
                if (menuResultADO == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("menuPrintProcessor run fail. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => menuPrintInitADO), menuPrintInitADO));
                }
                this.lciPrintAssignPrescription.Update();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInChiDinhDichVu(object sender, EventArgs e)
        {
            LogTheadInSessionInfo(() => InChiDinhDichVu(sender, e), "PrintAssignServiceClick");
        }

        private void InChiDinhDichVu(object sender, EventArgs e) 
        {
            try
            {
                //khi click nhiều lầu nút in sẽ chỉ hiển thị preview 1 lần
                bool isOpenPreview = false;
                foreach (Form item in Application.OpenForms)
                {
                    if (item.Name == "frmSetupPrintPreview")
                    {
                        isOpenPreview = true;
                        break;
                    }
                }
                if (isOpenPreview)
                {
                    return;
                }

                string printTypeCode = "";
                if (sender is DXMenuItem)
                {
                    var bbtnItem = sender as DXMenuItem;
                    printTypeCode = (bbtnItem.Tag ?? "").ToString();
                }
                else if (sender is SimpleButton)
                {
                    var bbtnItem = sender as SimpleButton;
                    printTypeCode = (bbtnItem.Tag ?? "").ToString();
                }

                PrescriptionPrintShow(printTypeCode, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrescriptionPrintNow()
        {
            try
            {
                PrescriptionSavePrintShow(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_DON_THUOC_Y_HOC_CO_TRUYEN__MPS000050, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrescriptionPrintShow(string printTypeCode, bool isPrintNow)
        {
            try
            {
                if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008
                    || printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010
                    || printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011)
                {
                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, currentTreatmentWithPatientType);

                    PrintTreatmentFinishProcessor printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(treatment, currentModule != null ? currentModule.RoomId : 0);
                    if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010)
                    {
                        printTreatmentFinishProcessor.Print(MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode, isPrintNow);
                    }
                    else if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011)
                    {
                        printTreatmentFinishProcessor.Print(MPS.Processor.Mps000011.PDO.Mps000011PDO.printTypeCode, isPrintNow);
                    }
                    else
                    {
                        printTreatmentFinishProcessor.Print(MPS.Processor.Mps000008.PDO.Mps000008PDO.printTypeCode, isPrintNow);
                    }
                }
                else
                {
                    HIS.Desktop.Plugins.Library.PrintPrescription.PrintPrescriptionProcessor printPrescriptionProcessor;
                    if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                    {
                        List<InPatientPresResultSDO> InPatientPresResultSDOForPrints = new List<InPatientPresResultSDO>();
                        InPatientPresResultSDO inPatientPresResultSDO = new InPatientPresResultSDO();
                        if (inPrescriptionResultSDOs != null)
                            InPatientPresResultSDOForPrints.Add(inPrescriptionResultSDOs);
                        else if (this.oldServiceReq != null)
                        {
                            if (this.oldExpMest != null)
                            {
                                inPatientPresResultSDO.ExpMests = new List<HIS_EXP_MEST>();
                                inPatientPresResultSDO.ExpMests.Add(oldExpMest);
                            }

                            if (this.oldServiceReq != null)
                            {
                                inPatientPresResultSDO.ServiceReqs = new List<HIS_SERVICE_REQ>();
                                inPatientPresResultSDO.ServiceReqs.Add(this.oldServiceReq);
                            }

                            if (this.expMestMedicineEditPrints != null)
                            {
                                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE, HIS_EXP_MEST_MEDICINE>();
                                inPatientPresResultSDO.Medicines = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MEDICINE>>(this.expMestMedicineEditPrints);
                            }

                            if (this.expMestMaterialEditPrints != null)
                            {
                                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL, HIS_EXP_MEST_MATERIAL>();
                                inPatientPresResultSDO.Materials = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MATERIAL>>(this.expMestMaterialEditPrints);
                            }
                            if (this.serviceReqMetys != null && this.serviceReqMetys.Count > 0)
                            {
                                inPatientPresResultSDO.ServiceReqMeties = serviceReqMetys;
                            }
                            if (this.serviceReqMatys != null && this.serviceReqMatys.Count > 0)
                            {
                                inPatientPresResultSDO.ServiceReqMaties = serviceReqMatys;
                            }

                            InPatientPresResultSDOForPrints.Add(inPatientPresResultSDO);
                        }

                        printPrescriptionProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(InPatientPresResultSDOForPrints, this.currentModule);
                    }
                    else
                    {
                        List<OutPatientPresResultSDO> OutPatientPresResultSDOForPrints = new List<OutPatientPresResultSDO>();
                        OutPatientPresResultSDO OutPatientPresResultSDO = new OutPatientPresResultSDO();

                        if (outPrescriptionResultSDOs != null)
                            OutPatientPresResultSDOForPrints.Add(outPrescriptionResultSDOs);
                        else if (this.oldServiceReq != null)
                        {
                            if (this.oldExpMest != null)
                            {
                                OutPatientPresResultSDO.ExpMests = new List<HIS_EXP_MEST>();
                                OutPatientPresResultSDO.ExpMests.Add(oldExpMest);
                            }

                            if (this.oldServiceReq != null)
                            {
                                OutPatientPresResultSDO.ServiceReqs = new List<HIS_SERVICE_REQ>();
                                OutPatientPresResultSDO.ServiceReqs.Add(this.oldServiceReq);
                            }

                            if (this.expMestMedicineEditPrints != null)
                            {
                                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE, HIS_EXP_MEST_MEDICINE>();
                                OutPatientPresResultSDO.Medicines = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MEDICINE>>(this.expMestMedicineEditPrints);
                            }

                            if (this.expMestMaterialEditPrints != null)
                            {
                                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL, HIS_EXP_MEST_MATERIAL>();
                                OutPatientPresResultSDO.Materials = AutoMapper.Mapper.Map<List<HIS_EXP_MEST_MATERIAL>>(this.expMestMaterialEditPrints);
                            }
                            if (this.serviceReqMetys != null && this.serviceReqMetys.Count > 0)
                            {
                                OutPatientPresResultSDO.ServiceReqMeties = serviceReqMetys;
                            }
                            if (this.serviceReqMatys != null && this.serviceReqMatys.Count > 0)
                            {
                                OutPatientPresResultSDO.ServiceReqMaties = serviceReqMatys;
                            }

                            OutPatientPresResultSDOForPrints.Add(OutPatientPresResultSDO);
                        }

                        printPrescriptionProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, this.currentModule);
                    }
                    if (isPrintNow)
                        printPrescriptionProcessor.Print();
                    else
                        printPrescriptionProcessor.Print(printTypeCode, isPrintNow);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrescriptionSavePrintShow(string printTypeCode, bool isPrintNow)
        {
            try
            {
                if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_RA_VIEN__MPS000008
                    || printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010
                    || printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011)
                {
                    HIS_TREATMENT treatment = new HIS_TREATMENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(treatment, currentTreatmentWithPatientType);

                    PrintTreatmentFinishProcessor printTreatmentFinishProcessor = new PrintTreatmentFinishProcessor(treatment, currentModule != null ? currentModule.RoomId : 0);
                    if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_HEN_KHAM__MPS000010)
                    {
                        printTreatmentFinishProcessor.Print(MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode, isPrintNow);
                    }
                    else if (printTypeCode == PrintTypeCodeStore.PRINT_TYPE_CODE__IN_GIAY_CHUYEN_VIEN__MPS000011)
                    {
                        printTreatmentFinishProcessor.Print(MPS.Processor.Mps000011.PDO.Mps000011PDO.printTypeCode, isPrintNow);
                    }
                    else
                    {
                        printTreatmentFinishProcessor.Print(MPS.Processor.Mps000008.PDO.Mps000008PDO.printTypeCode, isPrintNow);
                    }
                }
                else
                {
                    HIS.Desktop.Plugins.Library.PrintPrescription.PrintPrescriptionProcessor printPrescriptionProcessor;
                    if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet)
                    {
                        List<InPatientPresResultSDO> InPatientPresResultSDOForPrints = new List<InPatientPresResultSDO>();
                        InPatientPresResultSDOForPrints.Add(inPrescriptionResultSDOs);
                        printPrescriptionProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(InPatientPresResultSDOForPrints, this.currentModule);
                    }
                    else
                    {
                        List<OutPatientPresResultSDO> OutPatientPresResultSDOForPrints = new List<OutPatientPresResultSDO>();
                        OutPatientPresResultSDO OutPatientPresResultSDO = new OutPatientPresResultSDO();

                        if (String.IsNullOrEmpty(printTypeCode) && ((this.expMestMedicinePrints != null && this.expMestMedicinePrints.Count > 0)
                            || (this.expMestMaterialPrints != null && this.expMestMaterialPrints.Count > 0)))
                        {
                            if (this.expMestPrints != null)
                            {
                                OutPatientPresResultSDO.ExpMests = expMestPrints;
                            }

                            if (this.serviceReqPrints != null)
                            {
                                OutPatientPresResultSDO.ServiceReqs = serviceReqPrints;
                            }

                            if (this.expMestMedicinePrints != null)
                            {
                                OutPatientPresResultSDO.Medicines = expMestMedicinePrints;
                            }

                            if (this.expMestMaterialPrints != null)
                            {
                                OutPatientPresResultSDO.Materials = expMestMaterialPrints;
                            }
                        }

                        if (outPrescriptionResultSDOs != null)
                            OutPatientPresResultSDOForPrints.Add(outPrescriptionResultSDOs);

                        OutPatientPresResultSDOForPrints.Add(OutPatientPresResultSDO);
                        printPrescriptionProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(OutPatientPresResultSDOForPrints, this.currentModule);
                    }
                    //if (isPrintNow)
                    //    printPrescriptionProcessor.Print();
                    //else
                    printPrescriptionProcessor.Print(printTypeCode, isPrintNow);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
