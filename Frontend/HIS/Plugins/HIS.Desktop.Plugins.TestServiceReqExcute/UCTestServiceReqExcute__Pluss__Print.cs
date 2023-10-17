﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using HIS.Desktop.LocalStorage.BackendData;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using MOS.TDO;
using HIS.Desktop.Controls.Session;
using DevExpress.Utils.Menu;
using HIS.Desktop.Print;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.TestServiceReqExcute
{
    public partial class UCTestServiceReqExcute : HIS.Desktop.Utility.UserControlBase
    {
        private void FillDataToButtonPrintTest()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemInPhieuKQXetNghiem = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_TEST_SERVICE_REQ_EXCUTE_CONTROL_IN_PHIEU_KET_QUA_XET_NGHIEM", Base.ResourceLangManager.LanguageUCTestServiceReqExcute, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKetQua));
                itemInPhieuKQXetNghiem.Tag = PrintTypeTest.IN_PHIEU_KET_QUA_XET_NGHIEM;
                menu.Items.Add(itemInPhieuKQXetNghiem);

                //btnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal enum PrintTypeTest
        {
            IN_PHIEU_KET_QUA_XET_NGHIEM,
            IN_PHIEU_KET_QUA_XET_NGHIEM_KY,
            PREVIEW_IN_PHIEU_KET_QUA_XET_NGHIEM
        }

        private void onClickInPhieuKetQua(object sender, EventArgs e)
        {
            try
            {

                var bbtnItem = sender as DXMenuItem;
                PrintTypeTest type = (PrintTypeTest)(bbtnItem.Tag);
                PrintProcess(type);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        void PrintProcess(PrintTypeTest printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeTest.IN_PHIEU_KET_QUA_XET_NGHIEM:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_XET_NGHIEM__MPS000014, DelegateRunPrinterTest);
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

        bool DelegateRunPrinterTest(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_XET_NGHIEM__MPS000014:
                        LoadBieuMauPhieuYCInKetQuaXetNghiem(printTypeCode, fileName, ref result);
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

        private void LoadBieuMauPhieuYCInKetQuaXetNghiem(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();

                //XetNghiem
                List<MPS.ADO.ExeSereServ> ExesereServs = new List<MPS.ADO.ExeSereServ>();
                List<V_HIS_SERE_SERV_TEIN> lstSereServTein = new List<V_HIS_SERE_SERV_TEIN>();
                if (lstSereServ != null && lstSereServ.Count > 0)
                {
                    List<long> sereServIDs = lstSereServ.Select(o => o.ID).ToList();
                    MOS.Filter.HisSereServTeinViewFilter sereServTeinFilter = new MOS.Filter.HisSereServTeinViewFilter();
                    sereServTeinFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    sereServTeinFilter.SERE_SERV_IDs = sereServIDs;
                    sereServTeinFilter.ORDER_FIELD = "NUM_ORDER";
                    sereServTeinFilter.ORDER_DIRECTION = "DESC";
                    lstSereServTein = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>(HisRequestUriStore.HIS_SERE_SERV_TEIN_GET, ApiConsumers.MosConsumer, sereServTeinFilter, param);
                }


                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                HisPatientTypeAlterViewAppliedFilter paiententBhytFilter = new HisPatientTypeAlterViewAppliedFilter();
                paiententBhytFilter.TreatmentId = this.currentServiceReq.TREATMENT_ID;
                paiententBhytFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                patientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, paiententBhytFilter, param);

                MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = this.currentServiceReq.TREATMENT_ID;
                var _Treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("/api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
                if (this.currentServiceReq != null)
                {
                    var _Depart = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p => p.ID == this.currentServiceReq.REQUEST_DEPARTMENT_ID);
                    this.currentServiceReq.REQUEST_DEPARTMENT_NAME = (_Depart != null) ? _Depart.DEPARTMENT_NAME : null;
                    this.currentServiceReq.REQUEST_DEPARTMENT_CODE = (_Depart != null) ? _Depart.DEPARTMENT_CODE : null;

                    var _Room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ID == this.currentServiceReq.REQUEST_ROOM_ID);
                    this.currentServiceReq.REQUEST_ROOM_NAME = (_Room != null) ? _Room.ROOM_NAME : null;
                    this.currentServiceReq.REQUEST_ROOM_CODE = (_Room != null) ? _Room.ROOM_CODE : null;
                }

                // get serviceReq
                MOS.Filter.HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                serviceReqFilter.ID = this.currentServiceReq.ID;
                var serviceReqList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, serviceReqFilter, null);
                V_HIS_SERVICE_REQ serviceReqPrint = new V_HIS_SERVICE_REQ();
                if (serviceReqList != null && serviceReqList.Count > 0)
                {
                    serviceReqPrint = serviceReqList.FirstOrDefault();
                }

                List<HIS_DHST> hisdhst = new List<HIS_DHST>();
                HIS_DHST hisdhst_ = new HIS_DHST();
                MOS.Filter.HisDhstFilter DhstFilter = new HisDhstFilter();
                DhstFilter.TREATMENT_ID = this.currentServiceReq.TREATMENT_ID;
                hisdhst = new BackendAdapter(new CommonParam()).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumer.ApiConsumers.MosConsumer, DhstFilter, null);

                if (hisdhst != null && hisdhst.Count > 0)
                {
                   
                    hisdhst_ = hisdhst.OrderByDescending(item => item.EXECUTE_TIME).First();
                }


                List<object> obj = new List<object>();
                obj.Add(patientTypeAlter);
                obj.Add(serviceReqPrint);
                obj.Add(_Treatment);

                //Mức hưởng BHYT
                decimal ratio_text = 0;
                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                string levelCode = branch != null ? branch.HEIN_LEVEL_CODE : null;
                if (patientTypeAlter != null)
                {
                    // string levelCode = patientTypeAlter.LEVEL_CODE;
                    ratio_text = GetDefaultHeinRatioForView(patientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, levelCode, patientTypeAlter.RIGHT_ROUTE_CODE);
                }

                List<MPS.Processor.Mps000014.PDO.SereServNumOder> _SereServNumOders2 = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                List<MPS.Processor.Mps000014.PDO.SereServNumOder> _SereServNumOders4 = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                Dictionary<long, List<MPS.Processor.Mps000014.PDO.SereServNumOder>> _SereServNumOderss = new Dictionary<long, List<MPS.Processor.Mps000014.PDO.SereServNumOder>>();


                if (chkInTach.Checked)
                {
                    foreach (var item in this._SereServNumOders)
                    {
                        //var parent = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.ServiceParentId);

                        if (item.ServiceParentId == null)
                        {
                            if (!_SereServNumOderss.ContainsKey(0))
                            {
                                _SereServNumOderss[0] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                            }
                            _SereServNumOderss[0].Add(item);
                        }
                        else
                        {
                            _SereServNumOders2.Add(item);
                            //_SereServNumOderss[parent.ID].Add(item);
                        }
                    }
                    foreach (var item in _SereServNumOders2)
                    {
                        var parent = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.ServiceParentId);
                        if (parent.PARENT_ID == null)
                        {
                            if (!_SereServNumOderss.ContainsKey(parent.ID))
                            {
                                _SereServNumOderss[parent.ID] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                            }
                            _SereServNumOderss[parent.ID].Add(item);
                        }
                        else
                        {
                            _SereServNumOders4.Add(item);
                            //if (!_SereServNumOderss.ContainsKey(parent.PARENT_ID.Value))
                            //{
                            //    _SereServNumOderss[parent.PARENT_ID.Value] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                            //}
                            //_SereServNumOderss[parent.PARENT_ID.Value].Add(item);
                        }
                        _SereServNumOders4.GroupBy(o => o.GrandParentID);

                    }
                    foreach (var item in _SereServNumOders4)
                    {
                        if (!_SereServNumOderss.ContainsKey(item.GrandParentID.Value))
                        {
                            _SereServNumOderss[item.GrandParentID.Value] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                        }
                        _SereServNumOderss[item.GrandParentID.Value].Add(item);
                    }
                }
                else
                {
                    if (!_SereServNumOderss.ContainsKey(0))
                    {
                        _SereServNumOderss[0] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                    }
                    _SereServNumOderss[0].AddRange(this._SereServNumOders);
                }

                foreach (var item in _SereServNumOderss.Keys)
                {
                    MPS.Processor.Mps000014.PDO.Mps000014PDO mps000014RDO = new MPS.Processor.Mps000014.PDO.Mps000014PDO(
                    obj.ToArray(),
                    _SereServNumOderss[item],
                    lstSereServTein,
                    ratio_text,
                    BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                    genderId,
                    BackendDataWorker.Get<V_HIS_SERVICE>(),
                    hisdhst_
                    );

                    //else
                    //{
                    //    mps000014RDO = new MPS.Processor.Mps000014.PDO.Mps000014PDO(
                    //        obj.ToArray(),
                    //        this._SereServNumOders,
                    //        lstSereServTein,
                    //        ratio_text,
                    //        BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                    //        genderId
                    //        );
                    //}
                    WaitingManager.Hide();

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_Treatment != null ? _Treatment.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    //if (chkPreviewPrint.Checked && !chkKy.Checked)
                    //{
                    //    LogSystem.Debug("LoadBieuMauPhieuYCInKetQuaXetNghiem.0");
                    //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    //}
                    //if (chkPreviewPrint.Checked && chkKy.Checked)
                    //{
                    //    LogSystem.Debug("LoadBieuMauPhieuYCInKetQuaXetNghiem.0");
                    //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow, printerName) { EmrInputADO = inputADO });
                    //}
                    if (chkKy.Checked)
                    {
                        if (chkPrint.Checked)
                        {
                            LogSystem.Debug("LoadBieuMauPhieuYCInKetQuaXetNghiem.1");
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow, printerName) { EmrInputADO = inputADO });
                        }
                        else if (chkPreviewPrint.Checked)
                        {
                            LogSystem.Debug("LoadBieuMauPhieuYCInKetQuaXetNghiem.1");
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview, printerName) { EmrInputADO = inputADO });
                        }
                        else
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName) { EmrInputADO = inputADO });
                        }
                    }
                    else
                    {
                        LogSystem.Debug("LoadBieuMauPhieuYCInKetQuaXetNghiem.2");
                        if (this._IsKeyPrintNow)
                        {
                            LogSystem.Debug("LoadBieuMauPhieuYCInKetQuaXetNghiem.2.1");
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                        }
                        else
                        {
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                LogSystem.Debug("LoadBieuMauPhieuYCInKetQuaXetNghiem.2.2");
                                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                            }
                            else
                            {
                                LogSystem.Debug("LoadBieuMauPhieuYCInKetQuaXetNghiem.2.3");
                                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                            }
                        }
                    }
                    //if (chkPreviewPrint.Checked)
                    //{
                    //    if (chkKy.Checked)
                    //    {
                    //        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview, printerName) { EmrInputADO = inputADO });
                    //    }
                    //    else
                    //    {
                    //        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                    //    }
                    //}

                }
                this._IsKeyPrintNow = false;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBieuMauPhieuYCInKetQuaXetNghiemForKyCheckBox(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();

                //XetNghiem
                List<MPS.ADO.ExeSereServ> ExesereServs = new List<MPS.ADO.ExeSereServ>();
                List<V_HIS_SERE_SERV_TEIN> lstSereServTein = new List<V_HIS_SERE_SERV_TEIN>();
                if (lstSereServ != null && lstSereServ.Count > 0)
                {
                    List<long> sereServIDs = lstSereServ.Select(o => o.ID).ToList();
                    MOS.Filter.HisSereServTeinViewFilter sereServTeinFilter = new MOS.Filter.HisSereServTeinViewFilter();
                    sereServTeinFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    sereServTeinFilter.SERE_SERV_IDs = sereServIDs;
                    sereServTeinFilter.ORDER_FIELD = "NUM_ORDER";
                    sereServTeinFilter.ORDER_DIRECTION = "DESC";
                    lstSereServTein = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>(HisRequestUriStore.HIS_SERE_SERV_TEIN_GET, ApiConsumers.MosConsumer, sereServTeinFilter, param);
                }


                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                HisPatientTypeAlterViewAppliedFilter paiententBhytFilter = new HisPatientTypeAlterViewAppliedFilter();
                paiententBhytFilter.TreatmentId = this.currentServiceReq.TREATMENT_ID;
                paiententBhytFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                patientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, paiententBhytFilter, param);

                MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = this.currentServiceReq.TREATMENT_ID;
                var _Treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("/api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
                if (this.currentServiceReq != null)
                {
                    var _Depart = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(p => p.ID == this.currentServiceReq.REQUEST_DEPARTMENT_ID);
                    this.currentServiceReq.REQUEST_DEPARTMENT_NAME = (_Depart != null) ? _Depart.DEPARTMENT_NAME : null;
                    this.currentServiceReq.REQUEST_DEPARTMENT_CODE = (_Depart != null) ? _Depart.DEPARTMENT_CODE : null;

                    var _Room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(p => p.ID == this.currentServiceReq.REQUEST_ROOM_ID);
                    this.currentServiceReq.REQUEST_ROOM_NAME = (_Room != null) ? _Room.ROOM_NAME : null;
                    this.currentServiceReq.REQUEST_ROOM_CODE = (_Room != null) ? _Room.ROOM_CODE : null;
                }

                // get serviceReq
                MOS.Filter.HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                serviceReqFilter.ID = this.currentServiceReq.ID;
                var serviceReqList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, serviceReqFilter, null);
                V_HIS_SERVICE_REQ serviceReqPrint = new V_HIS_SERVICE_REQ();
                if (serviceReqList != null && serviceReqList.Count > 0)
                {
                    serviceReqPrint = serviceReqList.FirstOrDefault();
                }

                List<object> obj = new List<object>();
                obj.Add(patientTypeAlter);
                obj.Add(serviceReqPrint);
                obj.Add(_Treatment);

                //Mức hưởng BHYT
                decimal ratio_text = 0;
                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                string levelCode = branch != null ? branch.HEIN_LEVEL_CODE : null;
                if (patientTypeAlter != null)
                {
                    // string levelCode = patientTypeAlter.LEVEL_CODE;
                    ratio_text = GetDefaultHeinRatioForView(patientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, levelCode, patientTypeAlter.RIGHT_ROUTE_CODE);
                }

                List<MPS.Processor.Mps000014.PDO.SereServNumOder> _SereServNumOders2 = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                List<MPS.Processor.Mps000014.PDO.SereServNumOder> _SereServNumOders4 = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                Dictionary<long, List<MPS.Processor.Mps000014.PDO.SereServNumOder>> _SereServNumOderss = new Dictionary<long, List<MPS.Processor.Mps000014.PDO.SereServNumOder>>();


                if (chkInTach.Checked)
                {
                    foreach (var item in this._SereServNumOders)
                    {
                        //var parent = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.ServiceParentId);

                        if (item.ServiceParentId == null)
                        {
                            if (!_SereServNumOderss.ContainsKey(0))
                            {
                                _SereServNumOderss[0] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                            }
                            _SereServNumOderss[0].Add(item);
                        }
                        else
                        {
                            _SereServNumOders2.Add(item);
                            //_SereServNumOderss[parent.ID].Add(item);
                        }
                    }
                    foreach (var item in _SereServNumOders2)
                    {
                        var parent = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == item.ServiceParentId);
                        if (parent.PARENT_ID == null)
                        {
                            if (!_SereServNumOderss.ContainsKey(parent.ID))
                            {
                                _SereServNumOderss[parent.ID] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                            }
                            _SereServNumOderss[parent.ID].Add(item);
                        }
                        else
                        {
                            _SereServNumOders4.Add(item);
                            //if (!_SereServNumOderss.ContainsKey(parent.PARENT_ID.Value))
                            //{
                            //    _SereServNumOderss[parent.PARENT_ID.Value] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                            //}
                            //_SereServNumOderss[parent.PARENT_ID.Value].Add(item);
                        }
                        _SereServNumOders4.GroupBy(o => o.GrandParentID);

                    }
                    foreach (var item in _SereServNumOders4)
                    {
                        if (!_SereServNumOderss.ContainsKey(item.GrandParentID.Value))
                        {
                            _SereServNumOderss[item.GrandParentID.Value] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                        }
                        _SereServNumOderss[item.GrandParentID.Value].Add(item);
                    }
                }
                else
                {
                    if (!_SereServNumOderss.ContainsKey(0))
                    {
                        _SereServNumOderss[0] = new List<MPS.Processor.Mps000014.PDO.SereServNumOder>();
                    }
                    _SereServNumOderss[0].AddRange(this._SereServNumOders);
                }

                foreach (var item in _SereServNumOderss.Keys)
                {
                    MPS.Processor.Mps000014.PDO.Mps000014PDO mps000014RDO = new MPS.Processor.Mps000014.PDO.Mps000014PDO(
                    obj.ToArray(),
                    _SereServNumOderss[item],
                    lstSereServTein,
                    ratio_text,
                    BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                    genderId,
                    BackendDataWorker.Get<V_HIS_SERVICE>()
                    );

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_Treatment != null ? _Treatment.TREATMENT_CODE : ""), printTypeCode);

                    LogSystem.Debug("LoadBieuMauPhieuYCInKetQuaXetNghiem Ký");
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrShow, printerName) { EmrInputADO = inputADO });

                }
                this._IsKeyPrintNow = false;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public decimal GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            decimal result = 0;
            try
            {
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
