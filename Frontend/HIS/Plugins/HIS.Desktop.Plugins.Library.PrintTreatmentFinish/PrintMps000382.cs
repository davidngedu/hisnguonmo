﻿using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintTreatmentFinish
{
    class PrintMps000382
    {
        MPS.Processor.Mps000382.PDO.Mps000382PDO mps000011RDO { get; set; }
        bool printNow { get; set; }

        public PrintMps000382(string printTypeCode, string fileName, ref bool result, MOS.EFMODEL.DataModels.V_HIS_PATIENT HisPatient, MOS.EFMODEL.DataModels.HIS_TREATMENT HisTreatment, MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER VHisPatientTypeAlter, bool _printNow, long? roomId)
        {
            try
            {
                if (HisTreatment == null || HisTreatment.ID <= 0)
                {
                    result = false;
                    return;
                }
                this.printNow = _printNow;

                if (HisConfigs.Get<string>(Config.KEY_IN_CHUYEN_VIEN) == "1" && HisTreatment.IS_ACTIVE == 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(Resources.ResourceLanguageManager.BenhNhanChuaKhoaVienPhi, Resources.ResourceLanguageManager.GiayChuyenVien));
                }
                else
                {

                    List<MPS.Processor.Mps000382.PDO.TranPatiReasonADO> tranpatiReasonADOs = null;

                    //Lý do chuyển viện
                    var tranPatiReasons = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>();
                    if (tranPatiReasons != null && tranPatiReasons.Count > 0)
                    {
                        tranpatiReasonADOs = new List<MPS.Processor.Mps000382.PDO.TranPatiReasonADO>();
                        foreach (var item in tranPatiReasons)
                        {
                            MPS.Processor.Mps000382.PDO.TranPatiReasonADO aTranPatiReason = new MPS.Processor.Mps000382.PDO.TranPatiReasonADO(item);
                            if (item.ID == HisTreatment.TRAN_PATI_REASON_ID)
                            {
                                aTranPatiReason.checkbox = "X";
                            }
                            else
                            {
                                aTranPatiReason.checkbox = "";
                            }
                            tranpatiReasonADOs.Add(aTranPatiReason);
                        }
                    }
                    else
                    {
                        tranpatiReasonADOs = new List<MPS.Processor.Mps000382.PDO.TranPatiReasonADO>();
                    }

                    //Hình thức chuyển
                    var tranPatiForm = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().FirstOrDefault(o => o.ID == (HisTreatment.TRAN_PATI_FORM_ID.HasValue ? HisTreatment.TRAN_PATI_FORM_ID.Value : 0));
                    MPS.Processor.Mps000382.PDO.PatientADO PatientADO = new MPS.Processor.Mps000382.PDO.PatientADO(HisPatient);

                    MPS.Processor.Mps000382.PDO.Mps000382ADO ado = new MPS.Processor.Mps000382.PDO.Mps000382ADO();
                    if (HisTreatment.DEATH_CAUSE_ID != null)
                    {
                        var deathCause = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == HisTreatment.DEATH_CAUSE_ID.Value);
                        if (deathCause != null)
                        {
                            ado.DEATH_CAUSE_CODE = deathCause.DEATH_CAUSE_CODE;
                            ado.DEATH_CAUSE_NAME = deathCause.DEATH_CAUSE_NAME;
                        }
                    }
                    if (HisTreatment.DEATH_WITHIN_ID != null)
                    {
                        var deathWithin = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_WITHIN>().FirstOrDefault(o => o.ID == HisTreatment.DEATH_WITHIN_ID.Value);
                        if (deathWithin != null)
                        {
                            ado.DEATH_WITHIN_CODE = deathWithin.DEATH_WITHIN_CODE;
                            ado.DEATH_WITHIN_NAME = deathWithin.DEATH_WITHIN_NAME;
                        }
                    }
                    if (HisTreatment.TRAN_PATI_FORM_ID.HasValue)
                    {
                        var tranPati = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().FirstOrDefault(o => o.ID == HisTreatment.TRAN_PATI_FORM_ID.Value);
                        if (tranPati != null)
                        {
                            ado.TRAN_PATI_FORM_CODE = tranPati.TRAN_PATI_FORM_CODE;
                            ado.TRAN_PATI_FORM_NAME = tranPati.TRAN_PATI_FORM_NAME;
                        }
                    }
                    if (HisTreatment.TRAN_PATI_REASON_ID.HasValue)
                    {
                        var tranPatiReason = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().FirstOrDefault(o => o.ID == HisTreatment.TRAN_PATI_REASON_ID.Value);
                        if (tranPatiReason != null)
                        {
                            ado.TRAN_PATI_REASON_CODE = tranPatiReason.TRAN_PATI_REASON_CODE;
                            ado.TRAN_PATI_REASON_NAME = tranPatiReason.TRAN_PATI_REASON_NAME;
                        }
                    }
                    if (HisTreatment.END_ROOM_ID.HasValue)
                    {
                        var endRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.END_ROOM_ID.Value);
                        if (endRoom != null)
                        {
                            ado.END_DEPARTMENT_CODE = endRoom.DEPARTMENT_CODE;
                            ado.END_DEPARTMENT_NAME = endRoom.DEPARTMENT_NAME;
                            ado.END_ROOM_CODE = endRoom.ROOM_CODE;
                            ado.END_ROOM_NAME = endRoom.ROOM_NAME;
                        }
                    }
                    if (HisTreatment.TREATMENT_RESULT_ID.HasValue)
                    {
                        var treatmentResult = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_RESULT>().FirstOrDefault(o => o.ID == HisTreatment.TREATMENT_RESULT_ID.Value);
                        if (treatmentResult != null)
                        {
                            ado.TREATMENT_RESULT_CODE = treatmentResult.TREATMENT_RESULT_CODE;
                            ado.TREATMENT_RESULT_NAME = treatmentResult.TREATMENT_RESULT_NAME;
                        }
                    }
                    if (HisTreatment.FEE_LOCK_ROOM_ID.HasValue)
                    {
                        var feelockRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.FEE_LOCK_ROOM_ID.Value);
                        if (feelockRoom != null)
                        {
                            ado.FEE_LOCK_DEPARTMENT_CODE = feelockRoom.DEPARTMENT_CODE;
                            ado.FEE_LOCK_DEPARTMENT_NAME = feelockRoom.DEPARTMENT_NAME;
                            ado.FEE_LOCK_ROOM_CODE = feelockRoom.ROOM_CODE;
                            ado.FEE_LOCK_ROOM_NAME = feelockRoom.ROOM_NAME;
                        }
                    }
                    if (HisTreatment.IN_ROOM_ID.HasValue)
                    {
                        var inRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.IN_ROOM_ID.Value);
                        if (inRoom != null)
                        {
                            ado.IN_DEPARTMENT_CODE = inRoom.DEPARTMENT_CODE;
                            ado.IN_DEPARTMENT_NAME = inRoom.DEPARTMENT_NAME;
                            ado.IN_ROOM_CODE = inRoom.ROOM_CODE;
                            ado.IN_ROOM_NAME = inRoom.ROOM_NAME;
                        }
                    }

                    MPS.Processor.Mps000382.PDO.PatientADO patientADO = new MPS.Processor.Mps000382.PDO.PatientADO(HisPatient);

                    MOS.Filter.HisTranPatiTechFilter filter = new MOS.Filter.HisTranPatiTechFilter();
                    var tranPatiTechs = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_TECH>>("api/HisTranPatiTech/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    HIS_TRAN_PATI_TECH _TranPatiTech = new HIS_TRAN_PATI_TECH();

                    if (tranPatiTechs != null && tranPatiTechs.Count > 0)
                    {
                        _TranPatiTech = tranPatiTechs.FirstOrDefault(p => p.ID == HisTreatment.TRAN_PATI_TECH_ID);
                    }

                    mps000011RDO = new MPS.Processor.Mps000382.PDO.Mps000382PDO(
                        patientADO,
                       VHisPatientTypeAlter,
                       HisTreatment,
                       tranpatiReasonADOs,
                       tranPatiForm,
                       ado,
                       _TranPatiTech);

                    result = Print.RunPrint(printTypeCode, fileName, mps000011RDO, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint, result, _printNow, roomId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private bool RunPrint(string printTypeCode, string fileName, bool result, bool _printNow)
        //{
        //    try
        //    {
        //        string printerName = "";
        //        if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
        //        {
        //            printerName = GlobalVariables.dicPrinter[printTypeCode];
        //        }

        //        if (_printNow)
        //        {
        //            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000011RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint));
        //        }
        //        else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
        //        {
        //            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000011RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint));
        //        }
        //        else
        //        {
        //            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000011RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //        return false;
        //    }
        //    return result;
        //}

        private void EventLogPrint()
        {
            try
            {
                string message = "In phiếu chuyển viện. Mã in : Mps000011" + "  TREATMENT_CODE: " + this.mps000011RDO.currentTreatment.TREATMENT_CODE + "  Thời gian in: " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(DateTime.Now) + "  Người in: " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
