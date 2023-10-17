﻿using AutoMapper;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Plugins.Register.Run;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Register.Register
{
    class ServiceRequestRegisterExamBehavior : ServiceRequestRegisterBehaviorBase, IServiceRequestRegisterExam
    {
        HisServiceReqExamRegisterResultSDO result = null;
        long? priority;
        short? isNotRequireFee;
        int registerNumber = 0;
        DevExpress.XtraEditors.XtraScrollableControl xclServiceType;
        List<ServiceReqDetailSDO> serviceReqDetailSDOs;
        UCRegister _ucServiceRequestRegiter;

        internal ServiceRequestRegisterExamBehavior(CommonParam param, UCRegister ucServiceRequestRegiter, HisPatientSDO patientData)
            : base(param, ucServiceRequestRegiter)
        {
            this.registerNumber = ucServiceRequestRegiter.registerNumber;
            this.priority = (ucServiceRequestRegiter.chkPriority.Checked ? (GlobalVariables.HAS_PRIORITY) : 0);
            this.isNotRequireFee = (ucServiceRequestRegiter.chkIsNotRequireFee.Checked == true ? (short?)1 : null);
            this.xclServiceType = ucServiceRequestRegiter.pnlServiceRoomInfomation;
            this.serviceReqDetailSDOs = ucServiceRequestRegiter.serviceReqDetailSDOs;
            this._ucServiceRequestRegiter = ucServiceRequestRegiter;
        }

        HisServiceReqExamRegisterResultSDO IServiceRequestRegisterExam.Run()
        {
            HisServiceReqExamRegisterSDO serviceReqExamRegister = new HisServiceReqExamRegisterSDO();
            serviceReqExamRegister.HisPatientProfile = new HisPatientProfileSDO();

            //Process common data
            base.InitBase();

            #region #3590
            if (!string.IsNullOrEmpty(patientProfile.HisPatientTypeAlter.LIVE_AREA_CODE))
            {
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                ResourceMessage.BanCoMuonNhapThongTinKhuVuc,
                ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    //Neu chon khong
                    // focus con tro vao o khu vuc
                    this.uCMainHein.SetFocusUserByLiveAreaCode(this.ucHein__BHYT);
                    this._ucServiceRequestRegiter.isShowMess = true;
                    return null;
                }
                Inventec.Desktop.Common.Message.WaitingManager.Show();
            }
            #endregion

            #region#13606
            if (patientProfile.HisPatientTypeAlter != null
                && patientProfile.HisPatientTypeAlter.ID == HisConfigCFG.PatientTypeId__BHYT
                && this.intructionTime > 0
                && patientProfile.HisPatientTypeAlter.HEIN_CARD_TO_TIME != null
                && this.treatmentTypeId != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
            {
                long _intructionDate = Inventec.Common.TypeConvert.Parse.ToInt64(this.intructionTime.ToString().Substring(0, 8) + "000000");
                if (_intructionDate > patientProfile.HisPatientTypeAlter.HEIN_CARD_TO_TIME)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                ResourceMessage.ThoiGianYLenhLonHonThoiGianHanDenCuaTheBHYT,
                ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao);
                    this._ucServiceRequestRegiter.isShowMess = true;
                    return null;
                }
            }
            #endregion

            serviceReqExamRegister.HisPatientProfile = patientProfile;
            serviceReqExamRegister.RequestRoomId = (this.currentModule != null ? this.currentModule.RoomId : 0);
            serviceReqExamRegister.Note = this.Note;
            List<long> serviceIds = new List<long>();
            List<long> _RoomIds = new List<long>();
            //Process examserviceReq from input data
            this.ProcessExamServiceRequestData(ref serviceReqExamRegister, ref serviceIds, ref _RoomIds);

            // #9680
            if (_RoomIds != null && _RoomIds.Count > 0
                && HisConfigCFG.IsWarningOverExamBhyt
                && this.patientTypeId == HisConfigCFG.PatientTypeId__BHYT
                && this.treatmentTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
            {
                HisSereServBhytOutpatientExamFilter _HisSereServBhytOutpatientExamFilter = new HisSereServBhytOutpatientExamFilter();
                _HisSereServBhytOutpatientExamFilter.ROOM_IDs = _RoomIds;
                _HisSereServBhytOutpatientExamFilter.INTRUCTION_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(this.intructionTime.ToString().Substring(0, 8) + "000000");
                var dataSereServs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetSereServBhytOutpatientExam", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, _HisSereServBhytOutpatientExamFilter, null);
                if (dataSereServs != null && dataSereServs.Count > 0)
                {
                    MOS.Filter.HisExecuteRoomFilter executeRoomFilter = new HisExecuteRoomFilter();
                    executeRoomFilter.ROOM_IDs = _RoomIds;
                    var dataExecuteRooms = new BackendAdapter(new CommonParam()).Get<List<HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, executeRoomFilter, null);
                    if (dataExecuteRooms != null && dataExecuteRooms.Count > 0)
                    {
                        foreach (var itemRoom in dataExecuteRooms)
                        {
                            var coutSS = dataSereServs.Count(p => p.TDL_EXECUTE_ROOM_ID == itemRoom.ROOM_ID);
                            if (itemRoom.MAX_REQ_BHYT_BY_DAY.HasValue && coutSS >= itemRoom.MAX_REQ_BHYT_BY_DAY)
                            {
                                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                                string.Format(ResourceMessage.VuotQuaLuotKhamBHYTTrongNgay, itemRoom.EXECUTE_ROOM_NAME, itemRoom.MAX_REQ_BHYT_BY_DAY),
                                ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao,
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                {
                                    this._ucServiceRequestRegiter.isShowMess = true;
                                    return null;
                                }
                                Inventec.Desktop.Common.Message.WaitingManager.Show();
                            }
                        }
                    }
                }
            }

            //xuandv Neu Co dk Kham Qu tong dai
            if (AppConfigs.IsDangKyQuaTongDai == "1")
            {
                serviceReqExamRegister.IsNoExecute = true;
            }

            //#981
            List<HIS_SERE_SERV> sereServWithMinDurations = GetSereServWithMinDuration(this.patientId, serviceIds);
            if (sereServWithMinDurations != null && sereServWithMinDurations.Count > 0)
            {
                string sereServMinDurationStr = "";
                foreach (var item in sereServWithMinDurations)
                {
                    sereServMinDurationStr += item.TDL_SERVICE_CODE + " - " + item.TDL_SERVICE_NAME + "; ";
                }
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceMessage.CanhBaoDichVuDaDuocChiDinhTrongKhoangThoiGianCauHinh, sereServMinDurationStr), ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    this._ucServiceRequestRegiter.isShowMess = true;
                    Inventec.Common.Logging.LogSystem.Warn("Cac dich vu sau co thoi gian chi dinh nam trong khoang thoi gian khong cho phep, ____" + sereServMinDurationStr);
                    return null;
                }
                else
                {
                    this._ucServiceRequestRegiter.isShowMess = false;
                }
            }

            if (HisConfigCFG.IsCheckExamination)
            {
                if (serviceIds != null && serviceIds.Count > 0)
                {
                    //TODO
                }
                else
                {
                    Inventec.Desktop.Common.Message.WaitingManager.Hide();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                                ResourceMessage.BenhNhanChuaChonCongKham,
                                ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao,
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        this._ucServiceRequestRegiter.isShowMess = true;
                        return null;
                    }
                    Inventec.Desktop.Common.Message.WaitingManager.Show();
                }
            }

            //Execute call api
            Inventec.Common.Logging.LogSystem.Info("HisServiceReqExamRegisterResultSDO call api begin");

            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqExamRegister), serviceReqExamRegister));
            result = (HisServiceReqExamRegisterResultSDO)base.RunBase(serviceReqExamRegister, this.ucRequestService);
            Inventec.Common.Logging.LogSystem.Info("HisServiceReqExamRegisterResultSDO call api end");
            if (result != null && (result.HisPatientProfile == null || result.SereServs == null || result.SereServs.Count == 0 || result.ServiceReqs == null || result.ServiceReqs.Count == 0))
            {
                Inventec.Common.Logging.LogSystem.Warn("Goi api dang ky tiep don thanh cong, tuy nhien du lieu tra ve khong hop le, Dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqExamRegister), serviceReqExamRegister) + ", Dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
            }
            else if (result != null)
            {
                this.ucRequestService.serviceReqPrintIds = result.SereServs.Where(o => serviceIds.Contains(o.SERVICE_ID)).Select(o => o.SERVICE_REQ_ID ?? 0).Distinct().ToList(); ;
            }
            else
            {
                Inventec.Common.Logging.LogSystem.Warn("Goi api dang ky tiep don that bai, Dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqExamRegister), serviceReqExamRegister) + ", Dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
            }

            return result;
        }

        void ProcessExamServiceRequestData(ref HisServiceReqExamRegisterSDO ServiceReqData, ref List<long> serviceIds, ref List<long> _roomIds)
        {
            try
            {
                if (ServiceReqData.ServiceReqDetails == null)
                    ServiceReqData.ServiceReqDetails = new List<ServiceReqDetailSDO>();
                if (this.serviceReqDetailSDOs != null && this.serviceReqDetailSDOs.Count > 0)
                {
                    ServiceReqData.ServiceReqDetails.AddRange(this.serviceReqDetailSDOs);
                }
                foreach (var item in ServiceReqData.ServiceReqDetails)
                {
                    serviceIds.Add(item.ServiceId);
                    _roomIds.Add(item.RoomId ?? 0);
                }

                //this.ServiceAttachForServicePrimary(ServiceReqData, ref ServiceReqData);
                ServiceReqData.Priority = priority;
                ServiceReqData.InstructionTime = intructionTime;
                ServiceReqData.IsNotRequireFee = isNotRequireFee;
                ServiceReqData.RequestLoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                ServiceReqData.RequestUserName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ServiceAttachForServicePrimary(HisServiceReqExamRegisterSDO serviceReqExamRegisterSDO, ref HisServiceReqExamRegisterSDO result)
        {
            try
            {
                List<long> serviceIds = serviceReqExamRegisterSDO.ServiceReqDetails.Select(o => o.ServiceId).ToList();
                List<HIS_SERVICE_FOLLOW> sfs = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_FOLLOW>();

                List<HIS_SERVICE_FOLLOW> serviceFollows = sfs != null ? sfs.Where(o => serviceReqExamRegisterSDO != null && serviceReqExamRegisterSDO.ServiceReqDetails.Exists(t => t.ServiceId == o.SERVICE_ID)).ToList() : null;

                if (serviceFollows != null && serviceFollows.Count > 0)
                {
                    List<ServiceReqDetailSDO> serviceReqDetailsAdd = new List<ServiceReqDetailSDO>();
                    long defaultPatientTypeId = serviceReqExamRegisterSDO.HisPatientProfile.HisPatientTypeAlter.PATIENT_TYPE_ID;

                    foreach (ServiceReqDetailSDO sdo in serviceReqExamRegisterSDO.ServiceReqDetails)
                    {
                        List<HIS_SERVICE_FOLLOW> follows = serviceFollows.Where(t => t.SERVICE_ID == sdo.ServiceId).ToList();
                        if (follows != null && follows.Count > 0)
                        {
                            StringBuilder serviceFollow = new StringBuilder();
                            StringBuilder currentService = new StringBuilder();
                            foreach (HIS_SERVICE_FOLLOW f in follows)
                            {
                                V_HIS_SERVICE_PATY servicePaty = null;
                                if (BranchDataWorker.DicServicePatyInBranch != null && BranchDataWorker.DicServicePatyInBranch.ContainsKey(f.FOLLOW_ID))
                                {
                                    servicePaty = BranchDataWorker.DicServicePatyInBranch[f.FOLLOW_ID]
                                    .Where(o => o.PATIENT_TYPE_ID == defaultPatientTypeId)
                                    .OrderByDescending(m => m.MODIFY_TIME).FirstOrDefault();
                                }

                                long? patientTypeId = null;
                                if (servicePaty != null)
                                {
                                    patientTypeId = defaultPatientTypeId;
                                }
                                else
                                {
                                    V_HIS_SERVICE_PATY otherServicePaty = BranchDataWorker.ServicePatyWithListPatientType(f.FOLLOW_ID, GlobalStore.PatientTypeIdAllows).OrderByDescending(m => m.MODIFY_TIME).FirstOrDefault();
                                    patientTypeId = otherServicePaty != null ? new Nullable<long>(otherServicePaty.PATIENT_TYPE_ID) : null;
                                }

                                if (patientTypeId.HasValue)
                                {
                                    ServiceReqDetailSDO attach = new ServiceReqDetailSDO();
                                    attach.ServiceId = f.FOLLOW_ID;
                                    attach.Amount = f.AMOUNT;
                                    attach.IsExpend = f.IS_EXPEND;
                                    attach.PatientTypeId = patientTypeId.Value;
                                    serviceReqDetailsAdd.Add(attach);
                                }
                                else
                                {
                                    serviceFollow.Append(BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().SingleOrDefault(o => o.ID == f.SERVICE_ID).SERVICE_NAME).Append(",");
                                    currentService.Append(BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().SingleOrDefault(o => o.ID == sdo.ServiceId).SERVICE_NAME).Append(",");
                                }
                            }

                            if (!String.IsNullOrEmpty(serviceFollow.ToString()) || !String.IsNullOrEmpty(currentService.ToString()))
                            {
                                MessageManager.Show(string.Format(ResourceMessage.DichVuDinhKemDichVuChuaCoChinhSachGia, serviceFollow.ToString(), currentService.ToString()));
                            }
                        }
                    }

                    if (serviceReqDetailsAdd != null && serviceReqDetailsAdd.Count > 0)
                    {
                        serviceReqExamRegisterSDO.ServiceReqDetails.AddRange(serviceReqDetailsAdd);
                    }
                    result = serviceReqExamRegisterSDO;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_SERE_SERV> GetSereServWithMinDuration(long patientId, List<long> serviceIds)
        {
            List<HIS_SERE_SERV> results = new List<HIS_SERE_SERV>();
            try
            {
                if (patientId <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay thong tin benh nhan voi patientId  " + patientId);
                    return null;
                }
                if (serviceIds == null || serviceIds.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong truyen danh sach serviceids");
                    return null;
                }

                List<V_HIS_SERVICE> services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>()
     .Where(o => serviceIds.Contains(o.ID) && o.MIN_DURATION.HasValue).ToList();
                if (services == null || services.Count <= 0)
                    return null;

                List<ServiceDuration> serviceDurations = new List<ServiceDuration>();
                foreach (var item in services)
                {
                    ServiceDuration sd = new ServiceDuration();
                    sd.MinDuration = item.MIN_DURATION.Value;
                    sd.ServiceId = item.ID;
                    serviceDurations.Add(sd);
                }

                CommonParam param = new CommonParam();
                HisSereServMinDurationFilter hisSereServMinDurationFilter = new HisSereServMinDurationFilter();
                hisSereServMinDurationFilter.ServiceDurations = serviceDurations;
                hisSereServMinDurationFilter.PatientId = patientId;
                hisSereServMinDurationFilter.InstructionTime = this.intructionTime;
                results = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/GetExceedMinDuration", ApiConsumer.ApiConsumers.MosConsumer, hisSereServMinDurationFilter, param);
                if (results != null && results.Count > 0)
                {
                    var listSereServResultTemp = from SereServResult in results
                                                 group SereServResult by SereServResult.SERVICE_ID into g
                                                 orderby g.Key
                                                 select g.FirstOrDefault();
                    results = listSereServResultTemp.ToList();
                }
            }
            catch (Exception ex)
            {
                results = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return results;
        }
    }
}
