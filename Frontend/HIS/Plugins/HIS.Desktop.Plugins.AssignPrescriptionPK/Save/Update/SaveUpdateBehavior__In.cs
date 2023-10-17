﻿using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription;
using Inventec.Common.Logging;
using MOS.SDO;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Save.Update
{
    partial class SaveUpdateBehavior : SaveAbstract, ISave
    {
        object Run__In()
        {
            InPatientPresResultSDO result = null;
            if (this.CheckValid() && OldServiceReq != null)
            {
                frmAssignPrescription.VerifyWarningOverCeiling();

                this.InitBase();

                InPatientPresSDO prescriptionSDO = new InPatientPresSDO();
                prescriptionSDO.Medicines = this.InPatientPresMedicineSDOs;
                prescriptionSDO.Materials = this.InPatientPresMaterialSDOs;
                prescriptionSDO.ServiceReqMaties = this.PresOutStockMatySDOs;
                prescriptionSDO.ServiceReqMeties = this.PresOutStockMetySDOs;
                prescriptionSDO.SerialNumbers = this.PatientPresMaterialBySerialNumberSDOs;
                prescriptionSDO.Id = OldServiceReq.ID;
                prescriptionSDO.TreatmentId = this.TreatmentId;
                prescriptionSDO.PrescriptionTypeId = PrescriptionType.NEW;
                prescriptionSDO.IsTemporaryPres = (short?)this.IsTemporaryPres;
                this.ProcessPrescriptionUpdateSDO(prescriptionSDO);
                this.ProcessPrescriptionUpdateSDOICD(prescriptionSDO);
                this.ProcessPrescriptionSDOForSereServInKip(prescriptionSDO);
                LogSystem.Debug("Goi api sua don thuoc Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prescriptionSDO), prescriptionSDO));

                result = new Inventec.Common.Adapter.BackendAdapter(Param).Post<InPatientPresResultSDO>(RequestUriStore.HIS_SERVICE_REQ__IN_PATIENT_PRES_UPDATE, ApiConsumers.MosConsumer, prescriptionSDO, Param);
                if (result == null
                    || result.ServiceReqs == null || result.ServiceReqs.Count == 0
                    || ((result.ServiceReqMaties == null || result.ServiceReqMaties.Count == 0)
                        && (result.ServiceReqMeties == null || result.ServiceReqMeties.Count == 0)
                        && (result.Materials == null || result.Materials.Count == 0)
                        && (result.Medicines == null || result.Medicines.Count == 0))
                    )
                {
                    Inventec.Common.Logging.LogSystem.Debug("Goi api sua don thuoc that bai. Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prescriptionSDO), prescriptionSDO) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Param), Param));
                    result = null;
                }
            }

            return result;
        }

        private void ProcessPrescriptionUpdateSDO(InPatientPresSDO prescriptionSDO)
        {
            try
            {
                prescriptionSDO.DrugStoreId = this.DrugStoreId;
                if (this.RemedyCount > 0)
                    prescriptionSDO.RemedyCount = this.RemedyCount;
                prescriptionSDO.Advise = this.Advise;
                prescriptionSDO.RequestRoomId = this.RequestRoomId;
                prescriptionSDO.RequestLoginName = this.RequestLoginname;
                prescriptionSDO.RequestUserName = this.RequestUserName;
                //prescriptionSDO.ExpMestReasonId = this.ExpMestReasonId;
                if (this.ParentServiceReqId > 0)
                    prescriptionSDO.ParentServiceReqId = this.ParentServiceReqId;
                if (frmAssignPrescription.lciPhieuDieuTri.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    //&& frmAssignPrescription.Listtrackings != null && frmAssignPrescription.Listtrackings.Count > 0)
                    && frmAssignPrescription.cboPhieuDieuTri.EditValue != null)
                {
                    prescriptionSDO.TrackingId = Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.cboPhieuDieuTri.EditValue.ToString());
                    //prescriptionSDO.TrackingInfos = new System.Collections.Generic.List<TrackingInfoSDO>();
                    //foreach (var item in frmAssignPrescription.intructionTimeSelecteds)
                    //{
                    //    var trackings = frmAssignPrescription.Listtrackings.FirstOrDefault(o => o.TRACKING_TIME.ToString().Substring(0, 8) == item.ToString().Substring(0, 8));

                    //    if (trackings != null)
                    //    {
                    //        TrackingInfoSDO TrackingInfo = new TrackingInfoSDO();
                    //        TrackingInfo.IntructionTime = item;
                    //        TrackingInfo.TrackingId = trackings.ID;
                    //        prescriptionSDO.TrackingInfos.Add(TrackingInfo);
                    //    }
                    //}
                }
                prescriptionSDO.InstructionTimes = this.InstructionTimes;
                if (this.UseTimes != null && this.UseTimes.Count > 0)
                {
                    prescriptionSDO.UseTime = this.UseTimes.FirstOrDefault();
                }
                prescriptionSDO.IsHomePres = this.IsHomePres;
                prescriptionSDO.IsKidney = this.IsKidney;
                prescriptionSDO.KidneyTimes = this.KidneyTimes;
                prescriptionSDO.IsExecuteKidneyPres = this.IsExecuteKidneyPres;
                prescriptionSDO.ProvisionalDiagnosis = this.ProvisionalDiagnosis;

                prescriptionSDO.InteractionReason = this.InteractionReason;

                if (frmAssignPrescription.ServiceReqEye != null)
                {
                    prescriptionSDO.TreatEyesightGlassLeft = this.frmAssignPrescription.ServiceReqEye.TREAT_EYESIGHT_GLASS_LEFT;
                    prescriptionSDO.TreatEyesightGlassRight = this.frmAssignPrescription.ServiceReqEye.TREAT_EYESIGHT_GLASS_RIGHT;
                    prescriptionSDO.TreatEyesightLeft = this.frmAssignPrescription.ServiceReqEye.TREAT_EYESIGHT_LEFT;
                    prescriptionSDO.TreatEyesightRight = this.frmAssignPrescription.ServiceReqEye.TREAT_EYESIGHT_RIGHT;
                    prescriptionSDO.TreatEyeTensionLeft = this.frmAssignPrescription.ServiceReqEye.TREAT_EYE_TENSION_LEFT;
                    prescriptionSDO.TreatEyeTensionRight = this.frmAssignPrescription.ServiceReqEye.TREAT_EYE_TENSION_RIGHT;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrescriptionUpdateSDOICD(InPatientPresSDO prescriptionSDO)
        {
            try
            {
                prescriptionSDO.IcdName = this.IcdName;
                prescriptionSDO.IcdCode = this.IcdCode;
                prescriptionSDO.IcdCauseName = this.IcdCauseName;
                prescriptionSDO.IcdCauseCode = this.IcdCauseCode;
                prescriptionSDO.IcdText = this.IcdText;
                prescriptionSDO.IcdSubCode = this.IcdSubCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessPrescriptionSDOForSereServInKip(InPatientPresSDO prescriptionSDO)
        {
            try
            {
                if (prescriptionSDO.Materials.Count > 0
                    || prescriptionSDO.Medicines.Count > 0
                    )
                {
                    if (frmAssignPrescription.currentSereServ != null)
                    {
                        foreach (var item in prescriptionSDO.Materials)
                        {
                            item.SereServParentId = frmAssignPrescription.currentSereServ.ID;
                        }

                        foreach (var item in prescriptionSDO.Medicines)
                        {
                            item.SereServParentId = frmAssignPrescription.currentSereServ.ID;
                        }
                    }

                    if (frmAssignPrescription.currentSereServInEkip != null)
                    {
                        foreach (var item in prescriptionSDO.Materials)
                        {
                            item.SereServParentId = frmAssignPrescription.currentSereServInEkip.ID;
                        }

                        foreach (var item in prescriptionSDO.Medicines)
                        {
                            item.SereServParentId = frmAssignPrescription.currentSereServInEkip.ID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
