﻿using AutoMapper;
using Bartender.PrintClient;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ConnectionTest.ADO;
using HIS.Desktop.Plugins.ConnectionTest.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using LIS.SDO;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ConnectionTest
{
    public partial class UC_ConnectionTest : UserControlBase
    {

        CommonParam param = new CommonParam();
        public enum PRINT_OPTION
        {
            IN,
            IN_TACH_THEO_NHOM
        }

        #region Print Barcode
        private void onClickBtnPrintBarCode()
        {
            try
            {
                if (LisConfigCFG.PRINT_BARCODE_BY_BARTENDER == "1")
                {
                    this.PrintBarcodeByBartender();
                }
                else
                {
                    PrintType type = new PrintType();
                    type = PrintType.IN_BARCODE;
                    PrintProcess(type);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal enum PrintType
        {
            IN_BARCODE,
        }

        private void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.IN_BARCODE:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000077.PDO.Mps000077PDO.PrintTypeCode.Mps000077, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case MPS.Processor.Mps000077.PDO.Mps000077PDO.PrintTypeCode.Mps000077:
                        LoadBieuMauPhieuYCInBarCode(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        internal void LoadBieuMauPhieuYCInBarCode(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                rowSample = null;
                rowSample = (LisSampleADO)gridViewSample.GetFocusedRow();
                if (rowSample == null)
                    return;
                //bool refresh = false;
                LogSystem.Debug("LoadBieuMauPhieuYCInBarCode. 1");
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                {
                    if (LisConfigCFG.SHOW_FORM_SAMPLE_INFO == "1")
                    {
                        WaitingManager.Hide();
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        List<object> listArgs = new List<object>();
                        listArgs.Add(rowSample);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                        WaitingManager.Show();
                        FillDataToGridControl();
                        gridViewSample.RefreshData();
                    }
                    else
                    {
                        LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                        sdo.SampleId = rowSample.ID;
                        sdo.RequestRoomCode = room.ROOM_CODE;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        if (curentSTT != null)
                        {
                            rowSample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                            rowSample.SAMPLE_TYPE_ID = curentSTT.SAMPLE_TYPE_ID;
                            rowSample.SAMPLE_TIME = curentSTT.SAMPLE_TIME;
                            rowSample.SAMPLE_LOGINNAME = curentSTT.SAMPLE_LOGINNAME;
                            rowSample.SAMPLE_USERNAME = curentSTT.SAMPLE_USERNAME;
                            rowSample.SAMPLE_ORDER = curentSTT.SAMPLE_ORDER;
                            FillDataToGridControl();
                            gridViewSample.RefreshData();
                        }
                    }
                }

                LogSystem.Debug("LoadBieuMauPhieuYCInBarCode. 2");
                MOS.Filter.HisServiceReqFilter srFilter = new HisServiceReqFilter();
                srFilter.SERVICE_REQ_CODE__EXACT = rowSample.SERVICE_REQ_CODE;

                var lstrs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, srFilter, param);
                var rs = lstrs != null ? lstrs.FirstOrDefault() : null;
                LogSystem.Debug("LoadBieuMauPhieuYCInBarCode. 4: Count: " + (lstrs != null ? lstrs.Count : 0));
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                V_HIS_SERVICE_REQ serviceReq = null;
                if (rs != null)
                {
                    Mapper.CreateMap<HIS_SERVICE_REQ, V_HIS_SERVICE_REQ>();
                    serviceReq = Mapper.Map<V_HIS_SERVICE_REQ>(rs);
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((serviceReq != null ? serviceReq.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                MPS.Processor.Mps000077.PDO.Mps000077PDO mps000077RDO = new MPS.Processor.Mps000077.PDO.Mps000077PDO(
                           rowSample,
                           serviceReq
                           );
                WaitingManager.Hide();

                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }

                FillDataToGridControl();
                gridViewSample.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void PrintBarcodeByBartender()
        {
            try
            {
                rowSample = null;
                rowSample = (LisSampleADO)gridViewSample.GetFocusedRow();
                if (rowSample == null)
                    return;
                WaitingManager.Show();
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                {
                    if (LisConfigCFG.SHOW_FORM_SAMPLE_INFO == "1")
                    {
                        WaitingManager.Hide();
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        List<object> listArgs = new List<object>();
                        listArgs.Add(rowSample);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                        WaitingManager.Show();
                        FillDataToGridControl();
                        gridViewSample.RefreshData();
                    }
                    else
                    {
                        CommonParam param = new CommonParam();
                        LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                        sdo.SampleId = rowSample.ID;
                        sdo.RequestRoomCode = room.ROOM_CODE;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        if (curentSTT != null)
                        {
                            var sampleType = this.SampleTypeAllList != null && this.SampleTypeAllList.Count > 0
                                ? this.SampleTypeAllList.FirstOrDefault(o => o.ID == curentSTT.SAMPLE_TYPE_ID)
                                : null;

                            rowSample.SAMPLE_TYPE_CODE = sampleType != null ? sampleType.SAMPLE_TYPE_CODE : "";
                            rowSample.SAMPLE_TYPE_NAME = sampleType != null ? sampleType.SAMPLE_TYPE_NAME : "";
                            rowSample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                            rowSample.SAMPLE_TYPE_ID = curentSTT.SAMPLE_TYPE_ID;
                            rowSample.SAMPLE_TIME = curentSTT.SAMPLE_TIME;
                            rowSample.SAMPLE_LOGINNAME = curentSTT.SAMPLE_LOGINNAME;
                            rowSample.SAMPLE_USERNAME = curentSTT.SAMPLE_USERNAME;
                            rowSample.SAMPLE_ORDER = curentSTT.SAMPLE_ORDER;
                            FillDataToGridControl();
                            gridViewSample.RefreshData();
                        }
                    }
                }
                if (StartAppPrintBartenderProcessor.OpenAppPrintBartender())
                {
                    ClientPrintADO ado = new ClientPrintADO();
                    ado.Barcode = rowSample.BARCODE;
                    if (rowSample.DOB.HasValue)
                    {
                        ado.DobYear = rowSample.DOB.Value.ToString().Substring(0, 4);
                        ado.DobAge = MPS.AgeUtil.CalculateFullAge(rowSample.DOB.Value);
                    }
                    ado.ExecuteRoomCode = rowSample.EXECUTE_ROOM_CODE;
                    ado.ExecuteRoomName = rowSample.EXECUTE_ROOM_NAME ?? "";
                    ado.ExecuteRoomName_Unsign = Inventec.Common.String.Convert.UnSignVNese(rowSample.EXECUTE_ROOM_NAME ?? "");
                    ado.GenderName = (!String.IsNullOrWhiteSpace(rowSample.GENDER_CODE)) ? (rowSample.GENDER_CODE == "01" ? "Nữ" : "Nam") : "";
                    ado.GenderName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.GenderName);
                    ado.PatientCode = rowSample.PATIENT_CODE ?? "";
                    List<string> name = new List<string>();
                    if (!String.IsNullOrWhiteSpace(rowSample.LAST_NAME))
                    {
                        name.Add(rowSample.LAST_NAME);
                    }
                    if (!String.IsNullOrWhiteSpace(rowSample.FIRST_NAME))
                    {
                        name.Add(rowSample.FIRST_NAME);
                    }
                    ado.PatientName = String.Join(" ", name);
                    ado.PatientName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.PatientName);
                    ado.RequestDepartmentCode = rowSample.REQUEST_DEPARTMENT_CODE ?? "";
                    ado.RequestDepartmentName = rowSample.REQUEST_DEPARTMENT_NAME ?? "";
                    ado.PrintTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.DateTime.Get.Now() ?? 0);
                    ado.RequestDepartmentName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.RequestDepartmentName);
                    ado.ServiceReqCode = rowSample.SERVICE_REQ_CODE ?? "";
                    ado.TreatmentCode = rowSample.TREATMENT_CODE;
                    ado.SampleTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowSample.SAMPLE_TIME ?? 0);
                    ado.ResultTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowSample.RESULT_TIME ?? 0);
                    ado.AppointmentTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowSample.APPOINTMENT_TIME ?? 0);

                    BartenderPrintClientManager client = new BartenderPrintClientManager();
                    bool success = client.BartenderPrint(ado);
                    if (!success)
                    {
                        LogSystem.Error("In barcode Bartender that bai. Check log BartenderPrint");
                    }

                }
                else
                {
                    LogSystem.Warn("Khong mo duoc APP Print Bartender");
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Print Result
        internal enum PrintTypeKXN
        {
            IN_KET_QUA_XET_NGHIEM,
            IN_HUYET_HOC,
            IN_VI_SINH,
            IN_SINH_HOA,
            IN_MIEN_DICH,
            IN_XET_NGHIEM_TEST,
            IN_XET_NGHIEM_GIAI_PHAU_BENH,
            IN_XET_NGHIEM_NUOC_TIEU
        }

        void PrintProcess(PrintTypeKXN printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeKXN.IN_KET_QUA_XET_NGHIEM:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, DelegateRunPrinterKXN);
                        break;
                    case PrintTypeKXN.IN_HUYET_HOC:
                        richEditorMain.RunPrintTemplate("Mps000456", DelegateRunPrinterKXN);
                        break;
                    case PrintTypeKXN.IN_VI_SINH:
                        richEditorMain.RunPrintTemplate("Mps000457", DelegateRunPrinterKXN);
                        break;
                    case PrintTypeKXN.IN_MIEN_DICH:
                        richEditorMain.RunPrintTemplate("Mps000458", DelegateRunPrinterKXN);
                        break;
                    case PrintTypeKXN.IN_SINH_HOA:
                        richEditorMain.RunPrintTemplate("Mps000459", DelegateRunPrinterKXN);
                        break;
                    case PrintTypeKXN.IN_XET_NGHIEM_TEST:
                        richEditorMain.RunPrintTemplate("Mps000468", DelegateRunPrinterKXN);
                        break;
                    case PrintTypeKXN.IN_XET_NGHIEM_GIAI_PHAU_BENH:
                        richEditorMain.RunPrintTemplate("Mps000469", DelegateRunPrinterKXN);
                        break;
                    case PrintTypeKXN.IN_XET_NGHIEM_NUOC_TIEU:
                        richEditorMain.RunPrintTemplate("Mps000470", DelegateRunPrinterKXN);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinterKXN(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096:
                        LoadBieuMauInKetQuaXetNghiemV2(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000456":
                        LoadBieuMauInKetQuaXetNghiemV2(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000457":
                        LoadBieuMauInKetQuaXetNghiemV2(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000458":
                        LoadBieuMauInKetQuaXetNghiemV2(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000459":
                        LoadBieuMauInKetQuaXetNghiemV2(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000468":
                        LoadBieuMauInKetQuaXetNghiemV2(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000469":
                        LoadBieuMauInKetQuaXetNghiemV2(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000470":
                        LoadBieuMauInKetQuaXetNghiemV2(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadBieuMauInKetQuaXetNghiemV2(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                long UseSignEmr = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS.HIS.DESKTOP.IS_USE_SIGN_EMR");
                if (UseSignEmr != 1 && (chkSign.Checked))
                {
                    MessageBox.Show("Chưa cấu hình sử dụng hệ thống ký bệnh án điện tử. Không cho phép ký văn bản", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (String.IsNullOrWhiteSpace(rowSample.SERVICE_REQ_CODE))
                {
                    this.InKetQuaXNKhongCoServiceReq(printTypeCode, fileName, ref result);
                }
                else
                {
                    this.InKetQuaXNCoServiceReq(printTypeCode, fileName, ref result);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void SetDataToPrint(bool printSplit)
        {
            try
            {
                WaitingManager.Show();
                lstResultPrint = new List<V_LIS_RESULT>();
                lstResultHH = new List<V_LIS_RESULT>();
                lstResultVS = new List<V_LIS_RESULT>();
                lstResultMD = new List<V_LIS_RESULT>();
                lstResultSH = new List<V_LIS_RESULT>();
                lstResultXNT = new List<V_LIS_RESULT>();
                lstResultXNGPB = new List<V_LIS_RESULT>();
                lstResultXNNT = new List<V_LIS_RESULT>();
                dicServiceTest = new Dictionary<long, List<TestLisResultADO>>();
                if ((this.PrintOption == PRINT_OPTION.IN || this.PrintOption == PRINT_OPTION.IN_TACH_THEO_NHOM) && !String.IsNullOrWhiteSpace(rowSample.SERVICE_REQ_CODE))
                {
                    #region
                    currentServiceReq = GetServiceReq();
                    if (currentServiceReq == null)
                    {
                        return;
                    }
                    currentTreatment = GetTreatment();
                    currentPatient = GetPatient();
                    currentPatientTypeAlter = GetPatientTypeAlter();
                    if (this.PrintOption == PRINT_OPTION.IN)
                    {
                        #region
                        List<V_LIS_RESULT> lst = new List<V_LIS_RESULT>();
                        lst = GetListResult();
                        if (printSplit)
                        {
                            foreach (var item in lst)
                            {
                                var check = lstHisService.FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                                if (check != null)
                                {
                                    if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__HH)
                                    {
                                        lstResultHH.Add(item);
                                    }
                                    else if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__VS)
                                    {
                                        lstResultVS.Add(item);
                                    }
                                    else if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__MD)
                                    {
                                        lstResultMD.Add(item);
                                    }
                                    else if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__SH)
                                    {
                                        lstResultSH.Add(item);
                                    }
                                    else if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__TEST)
                                    {
                                        lstResultXNT.Add(item);
                                    }
                                    else if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__GPB)
                                    {
                                        lstResultXNGPB.Add(item);
                                    }
                                    else if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__NT)
                                    {
                                        lstResultXNNT.Add(item);
                                    }
                                    else
                                    {
                                        lstResultPrint.Add(item);
                                    }
                                }
                            }
                        }
                        else
                        {
                            lstResultPrint = lst;
                        }
                        #endregion
                    }
                    else if (this.PrintOption == PRINT_OPTION.IN_TACH_THEO_NHOM)
                    {
                        Dictionary<long, List<TestLisResultADO>> dic = new Dictionary<long, List<TestLisResultADO>>();
                        dicServiceTest = GetDicResult();

                    }
                    #endregion
                }
                else if ((this.PrintOption == PRINT_OPTION.IN || this.PrintOption == PRINT_OPTION.IN_TACH_THEO_NHOM) && String.IsNullOrWhiteSpace(rowSample.SERVICE_REQ_CODE))
                {
                    #region
                    if (this.PrintOption == PRINT_OPTION.IN)
                    {
                        List<V_LIS_RESULT> lst = new List<V_LIS_RESULT>();
                        lst = GetListResult();
                        if (printSplit)
                        {
                            foreach (var item in lst)
                            {
                                var check = lstHisService.FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                                if (check != null)
                                {
                                    if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__HH)
                                    {
                                        lstResultHH.Add(item);
                                    }
                                    else if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__VS)
                                    {
                                        lstResultVS.Add(item);
                                    }
                                    else if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__MD)
                                    {
                                        lstResultMD.Add(item);
                                    }
                                    else if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__SH)
                                    {
                                        lstResultSH.Add(item);
                                    }
                                    else if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__TEST)
                                    {
                                        lstResultXNT.Add(item);
                                    }
                                    else if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__GPB)
                                    {
                                        lstResultXNGPB.Add(item);
                                    }
                                    else if (check.TEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TEST_TYPE.ID__NT)
                                    {
                                        lstResultXNNT.Add(item);
                                    }
                                    else
                                    {
                                        lstResultPrint.Add(item);
                                    }
                                }
                            }
                        }
                        else
                        {
                            lstResultPrint = lst;
                        }
                    }
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_SERVICE_REQ GetServiceReq()
        {
            HIS_SERVICE_REQ obj = new HIS_SERVICE_REQ();
            try
            {
                MOS.Filter.HisServiceReqFilter ServiceReqViewFilter = new HisServiceReqFilter();
                ServiceReqViewFilter.SERVICE_REQ_CODE__EXACT = rowSample.SERVICE_REQ_CODE;
                obj = new HIS_SERVICE_REQ();
                obj = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, ServiceReqViewFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return obj;
        }

        private HIS_TREATMENT GetTreatment()
        {
            HIS_TREATMENT obj = new HIS_TREATMENT();
            try
            {
                MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = currentServiceReq.TREATMENT_ID;
                obj = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return obj;
        }

        private HIS_PATIENT GetPatient()
        {
            HIS_PATIENT patient = new HIS_PATIENT();
            try
            {
                MOS.Filter.HisPatientFilter patientFilter = new HisPatientFilter();
                patientFilter.ID = currentServiceReq.TDL_PATIENT_ID;
                var lstPatient = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, param);
                patient = lstPatient != null ? lstPatient.FirstOrDefault() : null;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return patient;
        }

        private HIS_PATIENT_TYPE_ALTER GetPatientTypeAlter()
        {
            HIS_PATIENT_TYPE_ALTER obj = new HIS_PATIENT_TYPE_ALTER();
            try
            {
                obj = new BackendAdapter(param).Get<HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumer.ApiConsumers.MosConsumer, currentServiceReq.TREATMENT_ID, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return obj;
        }

        private List<V_LIS_RESULT> GetListResult()
        {
            List<V_LIS_RESULT> lst = new List<V_LIS_RESULT>();
            try
            {
                if (lstCheckPrint != null && lstCheckPrint.Count > 0)
                {
                    List<string> serviceCodes = lstCheckPrint.Select(o => o.SERVICE_CODE).Distinct().ToList();
                    lst = _LisResults.Where(o => serviceCodes.Contains(o.SERVICE_CODE)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return lst;
        }

        private Dictionary<long, List<TestLisResultADO>> GetDicResult()
        {
            Dictionary<long, List<TestLisResultADO>> dicServiceTest = new Dictionary<long, List<TestLisResultADO>>();
            try
            {
                List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>();
                if (services == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach dich vu");
                    return null;
                }
                foreach (var item in lstCheckPrint)
                {
                    long key = 0;
                    V_HIS_SERVICE service = services.FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                    if (service != null)
                    {
                        if (service.PARENT_ID.HasValue && HisConfigCFG.PARENT_SERVICE_ID__GROUP_PRINT != null && HisConfigCFG.PARENT_SERVICE_ID__GROUP_PRINT.Contains(service.PARENT_ID.Value))
                        {
                            key = -1;
                        }
                        else
                        {
                            key = service.PARENT_ID ?? 0;
                        }

                    }

                    if (!dicServiceTest.ContainsKey(key))
                        dicServiceTest[key] = new List<TestLisResultADO>();
                    dicServiceTest[key].Add(item);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return dicServiceTest;
        }

        private void InKetQuaXNCoServiceReq(string printTypeCode, string fileName, ref bool result)
        {
            #region
            V_LIS_SAMPLE samplePrint = new V_LIS_SAMPLE();
            List<TestLisResultADO> lstHisSereServTeinSDO = new List<TestLisResultADO>();
            string printerName = "";
            long isPrint = 0; //1: In 2: xem truoc in
            if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
            {
                printerName = GlobalVariables.dicPrinter[printTypeCode];
            }
            CommonParam param = new CommonParam();
            LisSampleViewFilter sampleFilter = new LisSampleViewFilter();
            sampleFilter.ID = rowSample.ID;
            var apiResult = new BackendAdapter(param).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumers.LisConsumer, sampleFilter, param);
            if (apiResult != null && apiResult.Count > 0)
            {
                samplePrint = apiResult.FirstOrDefault();
            }

            Inventec.Common.Logging.LogSystem.Debug("LoadBieuMauInKetQuaXetNghiemV2 rowSample.PATIENT_CODE: " + rowSample.PATIENT_CODE);

            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((currentTreatment != null ? currentTreatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

            if (PrintOption == PRINT_OPTION.IN)
            {
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (printTypeCode == "Mps000096")
                {
                    #region Mps000096
                    MPS.Processor.Mps000096.PDO.Mps000096PDO mps000096RDO = new MPS.Processor.Mps000096.PDO.Mps000096PDO(
                          currentPatientTypeAlter,
                          currentTreatment,
                          samplePrint,
                          currentServiceReq,
                          this.currentTestIndexs.Where(o => lstResultPrint.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                          lstResultPrint,
                          BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                          genderId,
                          BackendDataWorker.Get<V_HIS_SERVICE>(),
                          currentPatient);

                    if (checkPrintNow.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;

                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else if (chkPrintPreview.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;
                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    else if (chkSign.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        return;
                    }
                    else if (chkSignProcess.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        if (isPrint == 1)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        else if (isPrint == 2)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                        else
                            return;

                    }
                    else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    #endregion

                }
                else if (printTypeCode == "Mps000456")
                {
                    #region Mps000456
                    MPS.Processor.Mps000456.PDO.Mps000456PDO mps000456RDO = new MPS.Processor.Mps000456.PDO.Mps000456PDO(
                          currentPatientTypeAlter,
                          currentTreatment,
                          rowSample,
                          currentServiceReq,
                          this.currentTestIndexs.Where(o => lstResultHH.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                          lstResultHH,
                          BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                          genderId,
                          BackendDataWorker.Get<V_HIS_SERVICE>(),
                          currentPatient);

                    if (checkPrintNow.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else if (chkPrintPreview.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    else if (chkSign.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000456.PDO.PrintTypeCode.Mps000456, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        return;
                    }
                    else if (chkSignProcess.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000456.PDO.PrintTypeCode.Mps000456, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        if (isPrint == 1)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        else if (isPrint == 2)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                        else
                            return;
                    }
                    else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    #endregion
                }
                else if (printTypeCode == "Mps000457")
                {
                    #region Mps000457

                    MPS.Processor.Mps000457.PDO.Mps000457PDO mps000457RDO = new MPS.Processor.Mps000457.PDO.Mps000457PDO(
                          currentPatientTypeAlter,
                          currentTreatment,
                          rowSample,
                          currentServiceReq,
                          this.currentTestIndexs.Where(o => lstResultVS.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                          lstResultVS,
                          BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                          genderId,
                          BackendDataWorker.Get<V_HIS_SERVICE>(),
                          currentPatient);

                    if (checkPrintNow.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else if (chkPrintPreview.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    else if (chkSign.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000457.PDO.PrintTypeCode.Mps000457, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        return;
                    }
                    else if (chkSignProcess.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000457.PDO.PrintTypeCode.Mps000457, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        if (isPrint == 1)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        else if (isPrint == 2)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                        else
                            return;
                    }
                    else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    #endregion
                }
                else if (printTypeCode == "Mps000458")
                {
                    #region Mps000458

                    MPS.Processor.Mps000458.PDO.Mps000458PDO mps000458RDO = new MPS.Processor.Mps000458.PDO.Mps000458PDO(
                          currentPatientTypeAlter,
                          currentTreatment,
                          rowSample,
                          currentServiceReq,
                          this.currentTestIndexs.Where(o => lstResultMD.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                          lstResultMD,
                          BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                          genderId,
                          BackendDataWorker.Get<V_HIS_SERVICE>(),
                          currentPatient);

                    if (checkPrintNow.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else if (chkPrintPreview.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    else if (chkSign.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000458.PDO.PrintTypeCode.Mps000458, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        return;
                    }
                    else if (chkSignProcess.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000458.PDO.PrintTypeCode.Mps000458, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        if (isPrint == 1)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        else if (isPrint == 2)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                        else
                            return;
                    }
                    else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    #endregion
                }
                else if (printTypeCode == "Mps000459")
                {
                    #region Mps000459

                    MPS.Processor.Mps000459.PDO.Mps000459PDO mps000459RDO = new MPS.Processor.Mps000459.PDO.Mps000459PDO(
                          currentPatientTypeAlter,
                          currentTreatment,
                          rowSample,
                          currentServiceReq,
                          this.currentTestIndexs.Where(o => lstResultSH.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                          lstResultSH,
                          BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                          genderId,
                          BackendDataWorker.Get<V_HIS_SERVICE>(),
                          currentPatient);

                    if (checkPrintNow.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else if (chkPrintPreview.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    else if (chkSign.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000459.PDO.PrintTypeCode.Mps000459, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        return;
                    }
                    else if (chkSignProcess.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000459.PDO.PrintTypeCode.Mps000459, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        if (isPrint == 1)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        else if (isPrint == 2)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                        else
                            return;
                    }
                    else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    #endregion
                }
                else if (printTypeCode == "Mps000468")
                {
                    #region Mps000468
                    MPS.Processor.Mps000468.PDO.Mps000468PDO Mps000468RDO = new MPS.Processor.Mps000468.PDO.Mps000468PDO(
                          currentPatientTypeAlter,
                          currentTreatment,
                          rowSample,
                          currentServiceReq,
                          this.currentTestIndexs.Where(o => lstResultXNT.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                          lstResultXNT,
                          BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                          genderId,
                          BackendDataWorker.Get<V_HIS_SERVICE>(),
                          currentPatient);

                    if (checkPrintNow.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;

                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else if (chkPrintPreview.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;
                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    else if (chkSign.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        return;
                    }
                    else if (chkSignProcess.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        if (isPrint == 1)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        else if (isPrint == 2)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                        else
                            return;

                    }
                    else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    #endregion
                }
                else if (printTypeCode == "Mps000469")
                {
                    #region Mps000469
                    MPS.Processor.Mps000469.PDO.Mps000469PDO Mps000469RDO = new MPS.Processor.Mps000469.PDO.Mps000469PDO(
                          currentPatientTypeAlter,
                          currentTreatment,
                          rowSample,
                          currentServiceReq,
                          this.currentTestIndexs.Where(o => lstResultXNGPB.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                          lstResultXNGPB,
                          BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                          genderId,
                          BackendDataWorker.Get<V_HIS_SERVICE>(),
                          currentPatient);

                    if (checkPrintNow.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;

                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else if (chkPrintPreview.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;
                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    else if (chkSign.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        return;
                    }
                    else if (chkSignProcess.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        if (isPrint == 1)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        else if (isPrint == 2)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                        else
                            return;

                    }
                    else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    #endregion
                }
                else if (printTypeCode == "Mps000470")
                {
                    #region Mps000470
                    MPS.Processor.Mps000470.PDO.Mps000470PDO Mps000470RDO = new MPS.Processor.Mps000470.PDO.Mps000470PDO(
                          currentPatientTypeAlter,
                          currentTreatment,
                          rowSample,
                          currentServiceReq,
                          this.currentTestIndexs.Where(o => lstResultXNNT.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                          lstResultXNNT,
                          BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                          genderId,
                          BackendDataWorker.Get<V_HIS_SERVICE>(),
                          currentPatient);

                    if (checkPrintNow.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;

                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else if (chkPrintPreview.Checked)
                    {
                        if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;
                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    else if (chkSign.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        return;
                    }
                    else if (chkSignProcess.Checked)
                    {
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                        string errorMessage = "";
                        PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                        //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                        SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                        ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                        txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        if (isPrint == 1)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        else if (isPrint == 2)
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                        else
                            return;

                    }
                    else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    }
                    #endregion
                }

                PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                PrintData.EmrInputADO = inputADO;
                result = MPS.MpsPrinter.Run(PrintData);
                if (result != null && (PrintData.previewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow || PrintData.previewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow || isPrint > 0))
                {
                    MessageManager.Show(this.ParentForm, new CommonParam(), true);
                }
            }
            #endregion
            else if (PrintOption == PRINT_OPTION.IN_TACH_THEO_NHOM)
            {


                if (printTypeCode == "Mps000096")
                {
                    #region
                    List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>();
                    if (services == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach dich vu");
                        return;
                    }
                    foreach (var item in dicServiceTest)
                    {
                        inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((currentTreatment != null ? currentTreatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                        V_HIS_SERVICE service = services.FirstOrDefault(o => o.ID == item.Key);
                        List<V_LIS_RESULT> testLisResults = new List<V_LIS_RESULT>();
                        if (item.Value != null && item.Value.Count > 0)
                        {
                            testLisResults = _LisResults.Where(o => item.Value.Select(p => p.SERVICE_CODE).Contains(o.SERVICE_CODE)).ToList();
                        }

                        MPS.ProcessorBase.Core.PrintData PrintData = null;

                        MPS.Processor.Mps000096.PDO.Mps000096PDO mps000096RDO = new MPS.Processor.Mps000096.PDO.Mps000096PDO(
                           currentPatientTypeAlter,
                           currentTreatment,
                           samplePrint,
                           currentServiceReq,
                           this.currentTestIndexs,
                           testLisResults,
                           BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                           genderId,
                           BackendDataWorker.Get<V_HIS_SERVICE>(),
                           currentPatient
                           );

                        if (checkPrintNow.Checked)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }

                        if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;
                        }
                        else if (chkPrintPreview.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                continue;
                            }
                        }
                        else if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                            SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            continue;
                        }

                        PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                        result = MPS.MpsPrinter.Run(PrintData);

                        //result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName, (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog) { EmrInputADO = inputADO });
                    }
                    #endregion
                }

            }
        }

        private void InKetQuaXNKhongCoServiceReq(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                long genderId = LoadGenderId();
                string printerName = "";
                long isPrint = 0;
                V_LIS_SAMPLE samplePrint = new V_LIS_SAMPLE();
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((currentTreatment != null ? currentTreatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                CommonParam param = new CommonParam();
                LisSampleViewFilter sampleFilter = new LisSampleViewFilter();
                sampleFilter.ID = rowSample.ID;
                var apiResult = new BackendAdapter(param).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumers.LisConsumer, sampleFilter, param);
                if (apiResult != null && apiResult.Count > 0)
                {
                    samplePrint = apiResult.FirstOrDefault();
                }
                if (PrintOption == PRINT_OPTION.IN)
                {
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (printTypeCode == "Mps000096")
                    {
                        #region Mps000096
                        MPS.Processor.Mps000096.PDO.Mps000096PDO mps000096RDO = new MPS.Processor.Mps000096.PDO.Mps000096PDO(
                                   null,
                                   null,
                                   samplePrint,
                                   null,
                                   this.currentTestIndexs,
                                   lstResultPrint,
                                   BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                                   genderId,
                                   BackendDataWorker.Get<V_HIS_SERVICE>());

                        if (checkPrintNow.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else if (chkSignProcess.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                if (isPrint == 1)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                                else if (isPrint == 2)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                                else
                                    return;
                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else if (chkPrintPreview.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else if (chkSignProcess.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                if (isPrint == 1)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                                else if (isPrint == 2)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                                else
                                    return;
                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        else if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;

                        }
                        else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        #endregion
                    }
                    else if (printTypeCode == "Mps000456")
                    {
                        #region Mps000456
                        MPS.Processor.Mps000456.PDO.Mps000456PDO mps000456RDO = new MPS.Processor.Mps000456.PDO.Mps000456PDO(
                              null,
                              null,
                              rowSample,
                              null,
                              this.currentTestIndexs,
                              lstResultHH,
                              BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                              genderId,
                              BackendDataWorker.Get<V_HIS_SERVICE>(),
                              currentPatient);

                        if (checkPrintNow.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else if (chkPrintPreview.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        else if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000456.PDO.PrintTypeCode.Mps000456, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;
                        }
                        else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000456RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        #endregion
                    }
                    else if (printTypeCode == "Mps000457")
                    {
                        #region Mps000457

                        MPS.Processor.Mps000457.PDO.Mps000457PDO mps000457RDO = new MPS.Processor.Mps000457.PDO.Mps000457PDO(
                              null,
                              null,
                              rowSample,
                              null,
                              this.currentTestIndexs,
                              lstResultVS,
                              BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                              genderId,
                              BackendDataWorker.Get<V_HIS_SERVICE>(),
                              currentPatient);


                        if (checkPrintNow.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else if (chkPrintPreview.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        else if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000457.PDO.PrintTypeCode.Mps000457, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;
                        }
                        else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000457RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        #endregion
                    }
                    else if (printTypeCode == "Mps000458")
                    {
                        #region Mps000458

                        MPS.Processor.Mps000458.PDO.Mps000458PDO mps000458RDO = new MPS.Processor.Mps000458.PDO.Mps000458PDO(
                              null,
                              null,
                              rowSample,
                              null,
                              this.currentTestIndexs,
                              lstResultMD,
                              BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                              genderId,
                              BackendDataWorker.Get<V_HIS_SERVICE>(),
                              currentPatient);


                        if (checkPrintNow.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else if (chkPrintPreview.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        else if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000458.PDO.PrintTypeCode.Mps000458, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;
                        }
                        else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000458RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        #endregion
                    }
                    else if (printTypeCode == "Mps000459")
                    {
                        #region Mps000459

                        MPS.Processor.Mps000459.PDO.Mps000459PDO mps000459RDO = new MPS.Processor.Mps000459.PDO.Mps000459PDO(
                              null,
                              null,
                              rowSample,
                              null,
                              this.currentTestIndexs,
                              lstResultSH,
                              BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                              genderId,
                              BackendDataWorker.Get<V_HIS_SERVICE>(),
                              currentPatient);


                        if (checkPrintNow.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else if (chkPrintPreview.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        else if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000459.PDO.PrintTypeCode.Mps000459, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;
                        }
                        else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000459RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        #endregion
                    }
                    if (printTypeCode == "Mps000468")
                    {
                        #region Mps000468
                        MPS.Processor.Mps000468.PDO.Mps000468PDO Mps000468RDO = new MPS.Processor.Mps000468.PDO.Mps000468PDO(
                              currentPatientTypeAlter,
                              currentTreatment,
                              rowSample,
                              currentServiceReq,
                              this.currentTestIndexs.Where(o => lstResultXNT.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                              lstResultXNT,
                              BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                              genderId,
                              BackendDataWorker.Get<V_HIS_SERVICE>(),
                              currentPatient);

                        if (checkPrintNow.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else if (chkSignProcess.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                if (isPrint == 1)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                                else if (isPrint == 2)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                                else
                                    return;

                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else if (chkPrintPreview.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else if (chkSignProcess.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                if (isPrint == 1)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                                else if (isPrint == 2)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                                else
                                    return;
                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        else if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                            SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;

                        }
                        else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000468RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        #endregion
                    }
                    if (printTypeCode == "Mps000469")
                    {
                        #region Mps000469
                        MPS.Processor.Mps000469.PDO.Mps000469PDO Mps000469RDO = new MPS.Processor.Mps000469.PDO.Mps000469PDO(
                              currentPatientTypeAlter,
                              currentTreatment,
                              rowSample,
                              currentServiceReq,
                              this.currentTestIndexs.Where(o => lstResultXNGPB.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                              lstResultXNGPB,
                              BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                              genderId,
                              BackendDataWorker.Get<V_HIS_SERVICE>(),
                              currentPatient);

                        if (checkPrintNow.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else if (chkSignProcess.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                if (isPrint == 1)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                                else if (isPrint == 2)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                                else
                                    return;

                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else if (chkPrintPreview.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else if (chkSignProcess.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                if (isPrint == 1)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                                else if (isPrint == 2)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                                else
                                    return;
                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        else if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                            SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;

                        }
                        else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000469RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        #endregion
                    }
                    if (printTypeCode == "Mps000470")
                    {
                        #region Mps000470
                        MPS.Processor.Mps000470.PDO.Mps000470PDO Mps000470RDO = new MPS.Processor.Mps000470.PDO.Mps000470PDO(
                              currentPatientTypeAlter,
                              currentTreatment,
                              rowSample,
                              currentServiceReq,
                              this.currentTestIndexs.Where(o => lstResultXNNT.Select(p => p.SERVICE_CODE).Distinct().ToList().Contains(o.SERVICE_CODE)).ToList(),
                              lstResultXNNT,
                              BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                              genderId,
                              BackendDataWorker.Get<V_HIS_SERVICE>(),
                              currentPatient);

                        if (checkPrintNow.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else if (chkSignProcess.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                if (isPrint == 1)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                                else if (isPrint == 2)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                                else
                                    return;

                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else if (chkPrintPreview.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                return;
                            }
                            else if (chkSignProcess.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                if (isPrint == 1)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                                else if (isPrint == 2)
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                                else
                                    return;
                            }
                            else
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        else if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                            SetUpSign(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            return;
                        }
                        else if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            if (isPrint == 1)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                            else if (isPrint == 2)
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                            else
                                return;

                        }
                        else if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, Mps000470RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }
                        #endregion
                    }
                    PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                    result = MPS.MpsPrinter.Run(PrintData);
                    if (result != null && (PrintData.previewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow || PrintData.previewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow || isPrint > 0))
                    {
                        MessageManager.Show(this.ParentForm, new CommonParam(), true);
                    }
                }
                else if (PrintOption == PRINT_OPTION.IN_TACH_THEO_NHOM)
                {
                    List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>();
                    if (services == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong tim thay danh sach dich vu");
                        return;
                    }

                    Dictionary<long, List<TestLisResultADO>> dicServiceTest = new Dictionary<long, List<TestLisResultADO>>();
                    foreach (var item in lstCheckPrint)
                    {

                        long key = 0;
                        V_HIS_SERVICE service = services.FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                        if (service != null)
                        {
                            if (service.PARENT_ID.HasValue && HisConfigCFG.PARENT_SERVICE_ID__GROUP_PRINT != null && HisConfigCFG.PARENT_SERVICE_ID__GROUP_PRINT.Contains(service.PARENT_ID.Value))
                            {
                                key = -1;
                            }
                            else
                            {
                                key = service.PARENT_ID ?? 0;
                            }
                        }

                        if (!dicServiceTest.ContainsKey(key))
                            dicServiceTest[key] = new List<TestLisResultADO>();
                        dicServiceTest[key].Add(item);
                    }

                    foreach (var item in dicServiceTest)
                    {
                        inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((currentTreatment != null ? currentTreatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                        V_HIS_SERVICE service = services.FirstOrDefault(o => o.ID == item.Key);
                        List<V_LIS_RESULT> testLisResults = new List<V_LIS_RESULT>();
                        if (item.Value != null && item.Value.Count > 0)
                        {
                            testLisResults = _LisResults.Where(o => item.Value.Select(p => p.SERVICE_CODE).Contains(o.SERVICE_CODE)).ToList();
                        }

                        MPS.Processor.Mps000096.PDO.Mps000096PDO mps000096RDO = new MPS.Processor.Mps000096.PDO.Mps000096PDO(
                               null,
                               null,
                               samplePrint,
                               null,
                               this.currentTestIndexs,
                               testLisResults,
                               BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                               genderId,
                               BackendDataWorker.Get<V_HIS_SERVICE>()
                               );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (checkPrintNow.Checked)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                        }

                        if (chkSignProcess.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            SetUpSignProcess(inputADO, PrintData, sample, ref result, ref errorMessage, ref isPrint);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                        }
                        else if (chkPrintPreview.Checked)
                        {
                            if (chkSign.Checked)
                            {
                                LIS_SAMPLE sample = new LIS_SAMPLE();
                                Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                                string errorMessage = "";
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, printerName);
                                SetUpSignAndPrintPreview(inputADO, PrintData, sample, ref result, ref errorMessage);
                                ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                                txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                                continue;
                            }
                        }
                        else if (chkSign.Checked)
                        {
                            LIS_SAMPLE sample = new LIS_SAMPLE();
                            Inventec.Common.Mapper.DataObjectMapper.Map<LIS_SAMPLE>(rowSample, sample);
                            string errorMessage = "";
                            PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "");
                            //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName);
                            SetUpSignAndPrint(inputADO, PrintData, sample, ref result, ref errorMessage);
                            ApproveListError.Add(string.Format("Mẫu XN có mã {0} ký thất bại. {1}", rowSample.BARCODE, errorMessage));
                            txtOldValueIntoPopup.Text = string.Join("\r\n", ApproveListError);
                            continue;
                        }

                        PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallModuleShowPrintLog(string printTypeCode, string uniqueCode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(printTypeCode) && !String.IsNullOrWhiteSpace(uniqueCode))
                {
                    //goi modul
                    HIS.Desktop.ADO.PrintLogADO ado = new HIS.Desktop.ADO.PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
