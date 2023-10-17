﻿using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.ExpMestDetailBCS.ADO;
using HIS.Desktop.Plugins.ExpMestDetailBCS.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace HIS.Desktop.Plugins.ExpMestDetailBCS.ExpMestViewDetail
{
    public partial class frmExpMestViewDetail : HIS.Desktop.Utility.FormBase
    {

        /// <summary>
        /// load dữ liệu vào các control chung
        /// </summary>
        private void LoadDataToControlCommon()
        {
            try
            {
                CommonParam param = new CommonParam();
                SetDataToCommonControlDetail(this._CurrentExpMest);
                if (this._CurrentExpMest != null && this._CurrentExpMest.SERVICE_REQ_ID != null)
                {
                    //Review
                    MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                    serviceReqFilter.ID = this._CurrentExpMest.SERVICE_REQ_ID;
                    Inventec.Common.Logging.LogSystem.Info("Bat dau goi api: HisServiceReq/Get");
                    var serviceReqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(RequestUri.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param);
                    Inventec.Common.Logging.LogSystem.Info("Ket thuc goi api: HisServiceReq/Get");
                    if (serviceReqs != null && serviceReqs.Count > 0)
                    {
                        LoadDataToPrescriptionCommonInfo(serviceReqs.FirstOrDefault());
                    }
                }
                else
                {
                    SetDataToExpmestControl(this._CurrentExpMest);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// load dữ liệu vào thông tin chung nếu phiếu xuất là xuất đơn thuốc
        /// </summary>
        /// <param name="_serviceReq"></param>
        private void LoadDataToPrescriptionCommonInfo(HIS_SERVICE_REQ _serviceReq)
        {
            try
            {
                if (_serviceReq != null)
                {
                    // thông tin expMest
                    //Review
                    if (this._CurrentExpMest != null)
                    {
                        lblExpMestCode.Text = this._CurrentExpMest.EXP_MEST_CODE;
                        lblExpMedistock.Text = this._CurrentExpMest.MEDI_STOCK_CODE + " - " + this._CurrentExpMest.MEDI_STOCK_NAME;
                        //lblExpUserName.Text = this._CurrentExpMest.EXP_LOGINNAME + " - " + this._CurrentExpMest.EXP_USERNAME;
                        //lblExpTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(this._CurrentExpMest.EXP_TIME ?? 0);
                    }

                    lblDescription.Text = _serviceReq.DESCRIPTION;

                    lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_serviceReq.TDL_PATIENT_DOB);
                    lblGenderName.Text = _serviceReq.TDL_PATIENT_GENDER_NAME;
                    //lblIcdCode.Text = _serviceReq.ICD_CODE;
                    //lblIcdText.Text = _serviceReq.ICD_TEXT;
                    //if (!String.IsNullOrEmpty(_serviceReq.ICD_NAME))
                    //{
                    //    lblIcdName.Text = _serviceReq.ICD_NAME;
                    //}
                    //else
                    //{
                    //    lblIcdName.Text = "";
                    //}
                    //lblInstructionTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(_serviceReq.INTRUCTION_TIME);
                    lblPatientCode.Text = _serviceReq.TDL_PATIENT_CODE;
                    //lblUserTimeFromTo.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(_serviceReq.USE_TIME ?? 0) + " - " + Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(_serviceReq.USE_TIME_TO ?? 0);
                    lblVirAddress.Text = _serviceReq.TDL_PATIENT_ADDRESS;
                    lblVirPatientName.Text = _serviceReq.TDL_PATIENT_NAME;
                    var room = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == _serviceReq.REQUEST_ROOM_ID).FirstOrDefault();
                    if (room != null)
                    {
                        lblRequestRoom.Text = room.ROOM_CODE + " - " + room.ROOM_NAME;
                    }
                    else
                    {
                        lblRequestRoom.Text = "";
                    }
                    var department = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().Where(o => o.ID == _serviceReq.REQUEST_DEPARTMENT_ID).FirstOrDefault();
                    if (department != null)
                    {
                        lblRequestDepartment.Text = department.DEPARTMENT_CODE + " - " + department.DEPARTMENT_NAME;
                    }
                    else
                    {
                        lblRequestDepartment.Text = "";
                    }

                }
                else
                {
                    ResetControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<string> GetListStringApprovalLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList, List<V_HIS_EXP_MEST_BLOOD> expMestBloodList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                List<string> expMestBloodGroups = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }

                if (expMestBloodList != null && expMestBloodList.Count > 0)
                {
                    expMestBloodGroups = expMestBloodList.Where(p => !string.IsNullOrEmpty(p.APPROVAL_LOGINNAME))
                    .GroupBy(o => o.APPROVAL_LOGINNAME)
                    .Select(p => p.First().APPROVAL_LOGINNAME)
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).Union(expMestBloodGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList, List<V_HIS_EXP_MEST_BLOOD> expMestBloodList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                List<string> expMestBloodGroups = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }
                if (expMestBloodList != null && expMestBloodList.Count > 0)
                {
                    expMestBloodGroups = expMestBloodList.Where(p => !string.IsNullOrEmpty(p.EXP_LOGINNAME))
                    .GroupBy(o => o.EXP_LOGINNAME)
                    .Select(p => p.First().EXP_LOGINNAME)
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).Union(expMestBloodGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        List<string> GetListStringExpTimeLogFromExpMestMedicineMaterial(List<V_HIS_EXP_MEST_MEDICINE> expMestMedicineList, List<V_HIS_EXP_MEST_MATERIAL> expMestMaterialList, List<V_HIS_EXP_MEST_BLOOD> expMestBloodList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> expMestMedicineGroups = new List<string>();
                List<string> expMestMaterialGroups = new List<string>();
                List<string> expMestBloodGroups = new List<string>();
                if (expMestMedicineList != null && expMestMedicineList.Count > 0)
                {
                    expMestMedicineGroups = expMestMedicineList.Where(p => p.EXP_TIME != null)
                    .GroupBy(o => o.EXP_TIME)
                    .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                    .ToList();
                }
                if (expMestMaterialList != null && expMestMaterialList.Count > 0)
                {
                    expMestMaterialGroups = expMestMaterialList.Where(p => p.EXP_TIME != null)
                      .GroupBy(o => o.EXP_TIME)
                      .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                      .ToList();
                }
                if (expMestBloodList != null && expMestBloodList.Count > 0)
                {
                    expMestBloodGroups = expMestBloodList.Where(p => p.EXP_TIME != null)
                    .GroupBy(o => o.EXP_TIME)
                    .Select(p => Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(p.First().EXP_TIME ?? 0))
                    .ToList();
                }
                result = expMestMedicineGroups.Union(expMestMaterialGroups).Union(expMestBloodGroups).ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        void SetDataToCommonControlDetail(V_HIS_EXP_MEST expMest)
        {
            try
            {
                if (expMest != null)
                {
                    lblExpMestCode.Text = expMest.EXP_MEST_CODE;
                    lblExpMedistock.Text = expMest.MEDI_STOCK_CODE + " - " + expMest.MEDI_STOCK_NAME;
                    var approvalLoginnameList = GetListStringApprovalLogFromExpMestMedicineMaterial(this._ExpMestMedicines_Print, this._ExpMestMaterials_Print, this._ExpMestBloods_Print);
                    lblApprovalUserName.Text = String.Join(", ", approvalLoginnameList.Where(p => !String.IsNullOrEmpty(p)).Select(o => o));
                    var expLoginnameList = GetListStringExpLogFromExpMestMedicineMaterial(this._ExpMestMedicines_Print, this._ExpMestMaterials_Print, this._ExpMestBloods_Print);
                    lblExpUserName.Text = String.Join(", ", expLoginnameList);
                    var expTimeList = GetListStringExpTimeLogFromExpMestMedicineMaterial(this._ExpMestMedicines_Print, this._ExpMestMaterials_Print, this._ExpMestBloods_Print);
                    lblExpTime.Text = String.Join(", ", expTimeList);
                    lblDescription.Text = expMest.DESCRIPTION;
                    lblExpMestSttName.Text = expMest.EXP_MEST_STT_NAME;
                    var room = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == expMest.REQ_ROOM_ID).FirstOrDefault();
                    if (room != null)
                    {
                        lblRequestRoom.Text = room.ROOM_CODE + " - " + room.ROOM_NAME;
                    }
                    else
                    {
                        lblRequestRoom.Text = "";
                    }
                    lblRequestDepartment.Text = expMest.REQ_DEPARTMENT_CODE + " - " + expMest.REQ_DEPARTMENT_NAME;
                    //lblReqLoginName.Text = expMest.REQ_LOGINNAME + " - " + expMest.REQ_USERNAME;
                    var reason = BackendDataWorker.Get<HIS_EXP_MEST_REASON>().Where(o => o.ID == expMest.EXP_MEST_REASON_ID).FirstOrDefault();
                    if (reason != null)
                    {
                        lblExpMestReasonName.Text = reason.EXP_MEST_REASON_NAME;
                    }
                    else
                    {
                        lblExpMestReasonName.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Set data control nếu k phải là đơn thuốc
        /// </summary>
        /// <param name="expMest"></param>
        private void SetDataToExpmestControl(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST expMest)
        {
            try
            {
                if (expMest != null)
                {
                    //Review
                    lblVirPatientName.Text = expMest.TDL_PATIENT_NAME;
                    lblPatientCode.Text = expMest.TDL_PATIENT_CODE;
                    lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(expMest.TDL_PATIENT_DOB ?? 0);
                    lblGenderName.Text = expMest.TDL_PATIENT_GENDER_NAME;
                    lblVirAddress.Text = expMest.TDL_PATIENT_ADDRESS;
                    //lblInstructionTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(expMest.TDL_INTRUCTION_TIME ?? 0);

                }
                else
                {
                    lblExpMestCode.Text = "";
                    lblExpMedistock.Text = "";
                    lblExpTime.Text = "";
                    lblExpUserName.Text = "";
                    lblDescription.Text = "";
                    lblExpMestSttName.Text = "";
                    lblApprovalUserName.Text = "";
                    lblRequestRoom.Text = "";
                    lblRequestDepartment.Text = "";
                    //lblReqLoginName.Text = "";
                    lblExpMestReasonName.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataToGridControl()
        {
            try
            {
                gridControlRequestMedicine.DataSource = null;
                gridControlApprovalMedicine.DataSource = null;
                gridControlRequestMaterial.DataSource = null;
                gridControlApprovalMaterial.DataSource = null;
                gridControlRequestExpMestBlood.DataSource = null;
                gridControlAprroveExpMestBlood.DataSource = null;
                gridControlServiceReqMaty.DataSource = null;
                gridControlServiceReqMety.DataSource = null;
                gridControlTestService.DataSource = null;

                //Thuốc ở trạng thái yêu cầu với các loại xuất trừ bean trực tiếp (đơn pk, đơn điều trị, đơn tủ trực, xuất khác...)
                List<V_HIS_EXP_MEST_MEDICINE_1> expMestMetyReqSubs = _ExpMestMedicines != null && _ExpMestMedicines.Count > 0 ? _ExpMestMedicines.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST).ToList() : _ExpMestMedicines;

                if (_ExpMestMetyReqs != null && expMestMetyReqSubs != null && expMestMetyReqSubs.Count > 0)
                {
                    _ExpMestMetyReqs.AddRange(expMestMetyReqSubs);
                }

                var expMestMedicineReqs = GroupExpMestMedicine(_ExpMestMetyReqs, false);
                expMestMedicineReqs = (expMestMedicineReqs != null && expMestMedicineReqs.Count() > 0) ? expMestMedicineReqs.OrderBy(o => o.NUM_ORDER).ToList() : expMestMedicineReqs;
                gridControlRequestMedicine.DataSource = expMestMedicineReqs;

                //Thuốc ở trạng thái đang thực hiện, duyệt với các loại xuất trừ bean trực tiếp (đơn pk, đơn điều trị, đơn tủ trực, xuất khác...)
                List<V_HIS_EXP_MEST_MEDICINE_1> expMestMedicineSubs = _ExpMestMedicines != null && _ExpMestMedicines.Count > 0 ? _ExpMestMedicines.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE || o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE).ToList() : _ExpMestMedicines;

                var expMestMedicine = GroupExpMestMedicine(expMestMedicineSubs, true);
                expMestMedicine = (expMestMedicine != null && expMestMedicine.Count() > 0) ? expMestMedicine.OrderBy(o => o.NUM_ORDER).ToList() : expMestMedicine;
                gridControlApprovalMedicine.DataSource = expMestMedicine;


                // Vat tu ở trạng thái yêu cầu với các loại xuất trừ bean trực tiếp (đơn pk, đơn điều trị, đơn tủ trực, xuất khác...)
                List<V_HIS_EXP_MEST_MATERIAL_1> expMestMatyReqSubs = _ExpMestMaterials != null && _ExpMestMaterials.Count > 0 ? _ExpMestMaterials.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST).ToList() : _ExpMestMaterials;
                if (_ExpMestMatyReqs != null && expMestMatyReqSubs != null && expMestMatyReqSubs.Count > 0)
                {
                    _ExpMestMatyReqs.AddRange(expMestMatyReqSubs);
                }
                var expMestMaterialRequests = GroupExpMestMaterial(_ExpMestMatyReqs, false);
                expMestMaterialRequests = (expMestMaterialRequests != null && expMestMaterialRequests.Count() > 0) ? expMestMaterialRequests.OrderBy(o => o.NUM_ORDER).ToList() : expMestMaterialRequests;
                gridControlRequestMaterial.DataSource = expMestMaterialRequests;

                //Vat tu ở trạng thái đang thực hiện, duyệt với các loại xuất trừ bean trực tiếp (đơn pk, đơn điều trị, đơn tủ trực, xuất khác...)
                List<V_HIS_EXP_MEST_MATERIAL_1> expMestMaterialSubs = _ExpMestMaterials != null && _ExpMestMaterials.Count > 0 ? _ExpMestMaterials.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE || o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE).ToList() : _ExpMestMaterials;
                var expMestMaterialAroval = GroupExpMestMaterial(expMestMaterialSubs, true);
                expMestMaterialAroval = (expMestMaterialAroval != null && expMestMaterialAroval.Count() > 0) ? expMestMaterialAroval.OrderBy(o => o.NUM_ORDER).ToList() : expMestMaterialAroval;
                gridControlApprovalMaterial.DataSource = expMestMaterialAroval;
                _ExpMestBltyReqs = (_ExpMestBltyReqs != null && _ExpMestBltyReqs.Count > 0) ? _ExpMestBltyReqs.OrderBy(o => o.NUM_ORDER).ToList() : _ExpMestBltyReqs;
                gridControlRequestExpMestBlood.DataSource = _ExpMestBltyReqs;
                _ExpMestBloods = (_ExpMestBloods != null && _ExpMestBloods.Count > 0) ? _ExpMestBloods.OrderBy(o => o.NUM_ORDER).ToList() : _ExpMestBloods;
                gridControlAprroveExpMestBlood.DataSource = _ExpMestBloods;
                ServiceReqMatys = (ServiceReqMatys != null && ServiceReqMatys.Count > 0) ? ServiceReqMatys.OrderBy(o => o.NUM_ORDER).ToList() : ServiceReqMatys;
                gridControlServiceReqMaty.DataSource = ServiceReqMatys;
                ServiceReqMetys = (ServiceReqMetys != null && ServiceReqMetys.Count > 0) ? ServiceReqMetys.OrderBy(o => o.NUM_ORDER).ToList() : ServiceReqMetys;
                gridControlServiceReqMety.DataSource = ServiceReqMetys;

                gridControlTestService.DataSource = this.SereServs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<ExpMestMedicineSDODetail> GroupExpMestMedicine(List<V_HIS_EXP_MEST_MEDICINE_1> expMestMedicine1s, bool approve)
        {
            if (expMestMedicine1s == null || expMestMedicine1s.Count == 0)
            {
                return new List<ExpMestMedicineSDODetail>();
            }

            List<ExpMestMedicineSDODetail> expMestmedicineTemps = new List<ExpMestMedicineSDODetail>();

            AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE_1, ExpMestMedicineSDODetail>();
            expMestmedicineTemps = AutoMapper.Mapper.Map<List<ExpMestMedicineSDODetail>>(expMestMedicine1s);

            List<ExpMestMedicineSDODetail> result = new List<ExpMestMedicineSDODetail>();
            try
            {
                if (approve
                    && this._CurrentExpMest != null
                    && this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS
                    && HisConfigs.Get<string>(HisConfigCFG.BCS_APPROVE_OTHER_TYPE_IS_ALLOW) == "1")
                {
                    var dataGroups = expMestmedicineTemps.GroupBy(o => new
                    {
                        o.MEDICINE_TYPE_ID,
                        o.PRICE,
                        o.IMP_PRICE,
                        o.EXP_MEST_METY_REQ_ID
                    }).ToList();

                    foreach (var dataGroup in dataGroups)
                    {
                        ExpMestMedicineSDODetail expMestmedicine = new ExpMestMedicineSDODetail();
                        expMestmedicine = dataGroup.First();
                        expMestmedicine.AMOUNT = dataGroup.Sum(o => o.AMOUNT);
                        expMestmedicine.SUM_BY_MEDICINE_IN_STOCK = dataGroup.Sum(o => o.SUM_BY_MEDICINE_IN_STOCK);
                        expMestmedicine.packageNumbers = String.Join(", ", dataGroup.Select(o => o.PACKAGE_NUMBER).Distinct().ToList());
                        var listExpiredDate = dataGroup.Select(o => o.EXPIRED_DATE).Distinct().ToList();
                        if (listExpiredDate != null && listExpiredDate.Count > 0)
                        {
                            List<string> listTemp = new List<string>();
                            foreach (var item in listExpiredDate)
                            {
                                listTemp.Add(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item ?? 0));
                            }
                            expMestmedicine.expiredDates = String.Join(", ", listTemp);
                        }
                        if (expMestmedicine.EXP_MEST_METY_REQ_ID.HasValue)
                        {
                            var req = this._ExpMestMetyReqs_Print != null ? this._ExpMestMetyReqs_Print.FirstOrDefault(o => o.ID == expMestmedicine.EXP_MEST_METY_REQ_ID.Value) : null;
                            if (req != null && req.MEDICINE_TYPE_ID != expMestmedicine.MEDICINE_TYPE_ID)
                            {
                                var typeName = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == req.MEDICINE_TYPE_ID);
                                if (typeName != null)
                                {
                                    expMestmedicine.REPLACE_FOR_NAME = typeName.MEDICINE_TYPE_NAME;
                                }
                            }
                        }
                        result.Add(expMestmedicine);
                    }
                }
                else
                {
                    var dataGroups = expMestmedicineTemps.GroupBy(o => new
                    {
                        o.MEDICINE_TYPE_ID,
                        o.PRICE,
                        o.IMP_PRICE
                    }).ToList();

                    foreach (var dataGroup in dataGroups)
                    {
                        ExpMestMedicineSDODetail expMestmedicine = new ExpMestMedicineSDODetail();
                        expMestmedicine = dataGroup.First();
                        expMestmedicine.AMOUNT = dataGroup.Sum(o => o.AMOUNT);
                        expMestmedicine.SUM_BY_MEDICINE_IN_STOCK = dataGroup.Sum(o => o.SUM_BY_MEDICINE_IN_STOCK);
                        result.Add(expMestmedicine);
                    }
                }
                result = result.OrderBy(o => o.MEDICINE_TYPE_NAME).ToList();
            }
            catch (Exception ex)
            {
                result = new List<ExpMestMedicineSDODetail>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        List<ExpMestMaterialSDODetail> GroupExpMestMaterial(List<V_HIS_EXP_MEST_MATERIAL_1> expMestMedicine1s, bool approve)
        {
            if (expMestMedicine1s == null || expMestMedicine1s.Count == 0)
            {
                return new List<ExpMestMaterialSDODetail>();
            }

            List<ExpMestMaterialSDODetail> result = new List<ExpMestMaterialSDODetail>();
            try
            {
                List<ExpMestMaterialSDODetail> expMestMaterialTemp = new List<ExpMestMaterialSDODetail>();
                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL_1, ExpMestMaterialSDODetail>();
                expMestMaterialTemp = AutoMapper.Mapper.Map<List<ExpMestMaterialSDODetail>>(expMestMedicine1s);
                if (approve
                    && this._CurrentExpMest != null
                    && this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS
                    && HisConfigs.Get<string>(HisConfigCFG.BCS_APPROVE_OTHER_TYPE_IS_ALLOW) == "1")
                {
                    var dataGroups = expMestMaterialTemp.GroupBy(o => new
                    {
                        o.MATERIAL_TYPE_ID,
                        o.PRICE,
                        o.IMP_PRICE,
                        o.EXP_MEST_MATY_REQ_ID
                    }).ToList();

                    foreach (var dataGroup in dataGroups)
                    {
                        ExpMestMaterialSDODetail expMestmedicine = new ExpMestMaterialSDODetail();
                        expMestmedicine = dataGroup.First();
                        expMestmedicine.AMOUNT = dataGroup.Sum(o => o.AMOUNT);
                        expMestmedicine.packageNumbers = String.Join(", ", dataGroup.Select(o => o.PACKAGE_NUMBER).Distinct().ToList());
                        var listExpiredDate = dataGroup.Select(o => o.EXPIRED_DATE).Distinct().ToList();
                        if (listExpiredDate != null && listExpiredDate.Count > 0)
                        {
                            List<string> listTemp = new List<string>();
                            foreach (var item in listExpiredDate)
                            {
                                listTemp.Add(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item ?? 0));
                            }
                            expMestmedicine.expiredDates = String.Join(", ", listTemp);
                        }
                        if (expMestmedicine.EXP_MEST_MATY_REQ_ID.HasValue)
                        {
                            var req = this._ExpMestMatyReqs_Print != null ? this._ExpMestMatyReqs_Print.FirstOrDefault(o => o.ID == expMestmedicine.EXP_MEST_MATY_REQ_ID.Value) : null;
                            if (req != null && req.MATERIAL_TYPE_ID != expMestmedicine.MATERIAL_TYPE_ID)
                            {
                                var typeName = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == req.MATERIAL_TYPE_ID);
                                if (typeName != null)
                                {
                                    expMestmedicine.REPLACE_FOR_NAME = typeName.MATERIAL_TYPE_NAME;
                                }
                            }
                        }
                        result.Add(expMestmedicine);
                    }
                }
                else
                {
                    var dataGroups = expMestMaterialTemp.GroupBy(o => new
                    {
                        o.MATERIAL_TYPE_ID,
                        o.PRICE,
                        o.IMP_PRICE
                    }).ToList();

                    foreach (var dataGroup in dataGroups)
                    {
                        ExpMestMaterialSDODetail expMestmedicine = new ExpMestMaterialSDODetail();
                        expMestmedicine = dataGroup.First();
                        expMestmedicine.AMOUNT = dataGroup.Sum(o => o.AMOUNT);
                        result.Add(expMestmedicine);
                    }
                }
                result = result != null ? result.OrderBy(o => o.MATERIAL_TYPE_NAME).ToList() : result;
            }
            catch (Exception ex)
            {
                result = new List<ExpMestMaterialSDODetail>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        List<V_HIS_EXP_MEST_MEDICINE_1> _ExpMestMetyReqs { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE_1> _ExpMestMedicines { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL_1> _ExpMestMatyReqs { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL_1> _ExpMestMaterials { get; set; }
        List<ExpMestBloodADODetail> _ExpMestBltyReqs { get; set; }
        List<ExpMestBloodADODetail> _ExpMestBloods { get; set; }

        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReqs_Print { get; set; }
        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReqs_Print { get; set; }
        List<V_HIS_EXP_MEST_BLTY_REQ_1> _ExpMestBltyReqs_Print { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials_Print { get; set; }
        List<V_HIS_EXP_MEST_BLOOD> _ExpMestBloods_Print { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines_Print { get; set; }
        List<HIS_SERVICE_REQ_MATY> ServiceReqMatys { get; set; }
        List<HIS_SERVICE_REQ_METY> ServiceReqMetys { get; set; }
        List<HIS_SERVICE_REQ> ServiceReqTests { get; set; }
        List<V_HIS_SERE_SERV> SereServs { get; set; }


        private void CreateThread()
        {
            Thread thread = new Thread(LoadMedistock);
            Thread thread1 = new Thread(LoadExpMestMetyReq);
            Thread thread2 = new Thread(LoadExpMestMatyReq);
            Thread thread3 = new Thread(LoadExpMestBltyReq);
            Thread thread4 = new Thread(LoadServiceReqMaty);
            Thread thread5 = new Thread(LoadServiceReqMety);
            Thread thread6 = new Thread(LoadTestServiceReq);

            try
            {
                thread.Start();
                thread1.Start();
                thread2.Start();
                thread3.Start();
                thread4.Start();
                thread5.Start();
                thread6.Start();

                thread.Join();
                thread1.Join();
                thread2.Join();
                thread3.Join();
                thread4.Join();
                thread5.Join();
                thread6.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                thread.Abort();
                thread1.Abort();
                thread2.Abort();
                thread3.Abort();
                thread4.Abort();
                thread5.Abort();
            }
        }

        /// <summary>
        /// lấy về medistock với medistockId tuong ung
        /// </summary>
        private void LoadMedistock()
        {
            try
            {
                if (moduleData != null)
                {
                    CommonParam param = new CommonParam();
                    // lấy về medistock với medistockId tuong ung
                    HisMediStockViewFilter medistockFilter = new HisMediStockViewFilter();
                    var medistocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ROOM_ID == moduleData.RoomId);
                    if (medistocks != null && medistocks.Count() > 0)
                    {
                        this.currentMedistockId = medistocks.Select(o => o.ID).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Load dữ liệu thuốc yêu cầu
        /// </summary>
        private void LoadExpMestMetyReq()
        {
            try
            {
                _ExpMestMetyReqs_Print = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMedicines_Print = new List<V_HIS_EXP_MEST_MEDICINE>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMetyReqFilter filter = new HisExpMestMetyReqFilter();
                filter.EXP_MEST_ID = this._CurrentExpMest.ID;
                Inventec.Common.Logging.LogSystem.Info("Bat dau goi api: HisExpMestMetyReq/Get");
                _ExpMestMetyReqs_Print = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Info("Ket thuc goi api: HisExpMestMetyReq/Get");
                _ExpMestMetyReqs = new List<V_HIS_EXP_MEST_MEDICINE_1>();
                if (_ExpMestMetyReqs_Print != null && _ExpMestMetyReqs_Print.Count > 0)
                {
                    var dataGroups = _ExpMestMetyReqs_Print.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        V_HIS_EXP_MEST_MEDICINE_1 ado = new V_HIS_EXP_MEST_MEDICINE_1();
                        AutoMapper.Mapper.CreateMap<HIS_EXP_MEST_METY_REQ, V_HIS_EXP_MEST_MEDICINE_1>();
                        ado = AutoMapper.Mapper.Map<V_HIS_EXP_MEST_MEDICINE_1>(item[0]);
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);

                        var typeName = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == item[0].MEDICINE_TYPE_ID);
                        if (typeName != null)
                        {
                            ado.MEDICINE_TYPE_NAME = typeName.MEDICINE_TYPE_NAME;
                            ado.MEDICINE_TYPE_CODE = typeName.MEDICINE_TYPE_CODE;
                            ado.SERVICE_UNIT_NAME = typeName.SERVICE_UNIT_NAME;
                            ado.MEDICINE_GROUP_ID = typeName.MEDICINE_GROUP_ID;
                            ado.CONVERT_RATIO = typeName.CONVERT_RATIO;
                            ado.CONVERT_UNIT_NAME = typeName.CONVERT_UNIT_NAME;
                            ado.CONCENTRA = typeName.CONCENTRA;
                            //ado.IS_ANTIBIOTIC = typeName.IS_ANTIBIOTIC;
                            //ado.IS_NEUROLOGICAL = typeName.IS_NEUROLOGICAL;
                        }
                        _ExpMestMetyReqs.Add(ado);
                    }
                }
                MOS.Filter.HisExpMestMedicineView1Filter MediFilter = new HisExpMestMedicineView1Filter();
                MediFilter.EXP_MEST_ID = this._CurrentExpMest.ID;
                Inventec.Common.Logging.LogSystem.Info("Bat dau goi api: HisExpMestMedicine/GetView1");
                _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE_1>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW1, ApiConsumers.MosConsumer, MediFilter, param);

                Inventec.Common.Logging.LogSystem.Debug("** frmExpMestMedicineViewDetail ket qua khi goi api HisExpMestMedicine/GetView1: **: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._ExpMestMedicines), this._ExpMestMedicines));

                Inventec.Common.Logging.LogSystem.Info("Ket thuc goi api: HisExpMestMedicine/GetView1");
                if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0)
                {
                    // var dataGroups = _ExpMestMedicines.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in _ExpMestMedicines)
                    {
                        V_HIS_EXP_MEST_MEDICINE ado = new V_HIS_EXP_MEST_MEDICINE();
                        AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MEDICINE_1, V_HIS_EXP_MEST_MEDICINE>();
                        ado = AutoMapper.Mapper.Map<V_HIS_EXP_MEST_MEDICINE>(item);
                        _ExpMestMedicines_Print.Add(ado);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load dữ liệu Vat Tu yêu cầu
        /// </summary>
        private void LoadExpMestMatyReq()
        {
            try
            {
                _ExpMestMatyReqs_Print = new List<HIS_EXP_MEST_MATY_REQ>();
                _ExpMestMaterials_Print = new List<V_HIS_EXP_MEST_MATERIAL>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMatyReqFilter filter = new HisExpMestMatyReqFilter();
                filter.EXP_MEST_ID = this._CurrentExpMest.ID;
                Inventec.Common.Logging.LogSystem.Info("Bat dau goi api: HisExpMestMatyReq/Get");
                _ExpMestMatyReqs_Print = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Info("Ket thuc goi api: HisExpMestMatyReq/Get");
                _ExpMestMatyReqs = new List<V_HIS_EXP_MEST_MATERIAL_1>();
                if (_ExpMestMatyReqs_Print != null && _ExpMestMatyReqs_Print.Count > 0)
                {
                    var dataGroups = _ExpMestMatyReqs_Print.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        V_HIS_EXP_MEST_MATERIAL_1 ado = new V_HIS_EXP_MEST_MATERIAL_1();
                        AutoMapper.Mapper.CreateMap<HIS_EXP_MEST_MATY_REQ, V_HIS_EXP_MEST_MATERIAL_1>();
                        ado = AutoMapper.Mapper.Map<V_HIS_EXP_MEST_MATERIAL_1>(item[0]);
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);

                        var typeName = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == item[0].MATERIAL_TYPE_ID);
                        if (typeName != null)
                        {
                            ado.MATERIAL_TYPE_NAME = typeName.MATERIAL_TYPE_NAME;
                            ado.MATERIAL_TYPE_CODE = typeName.MATERIAL_TYPE_CODE;
                            ado.SERVICE_UNIT_NAME = typeName.SERVICE_UNIT_NAME;
                            ado.IS_CHEMICAL_SUBSTANCE = typeName.IS_CHEMICAL_SUBSTANCE;
                            ado.CONVERT_RATIO = typeName.CONVERT_RATIO;
                            ado.CONVERT_UNIT_NAME = typeName.CONVERT_UNIT_NAME;
                        }
                        _ExpMestMatyReqs.Add(ado);
                    }
                }
                //_ExpMestMaterials_Print
                MOS.Filter.HisExpMestMaterialView1Filter mateFilter = new HisExpMestMaterialView1Filter();
                mateFilter.EXP_MEST_ID = this._CurrentExpMest.ID;
                Inventec.Common.Logging.LogSystem.Info("Bat dau goi api: HisExpMestMaterial/GetView1");
                _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL_1>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW1, ApiConsumers.MosConsumer, mateFilter, param);
                Inventec.Common.Logging.LogSystem.Info("ket thuc goi api: HisExpMestMaterial/GetView1");
                if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0)
                {
                    // var dataGroups = _ExpMestMaterials.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var item in _ExpMestMaterials)
                    {
                        V_HIS_EXP_MEST_MATERIAL ado = new V_HIS_EXP_MEST_MATERIAL();
                        AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL_1, V_HIS_EXP_MEST_MATERIAL>();
                        ado = AutoMapper.Mapper.Map<V_HIS_EXP_MEST_MATERIAL>(item);
                        _ExpMestMaterials_Print.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load dữ liệu Mau yêu cầu
        /// </summary>
        private void LoadExpMestBltyReq()
        {
            try
            {
                _ExpMestBltyReqs_Print = new List<V_HIS_EXP_MEST_BLTY_REQ_1>();
                _ExpMestBloods_Print = new List<V_HIS_EXP_MEST_BLOOD>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestBltyReqView1Filter filter = new HisExpMestBltyReqView1Filter();
                filter.EXP_MEST_ID = this._CurrentExpMest.ID;
                Inventec.Common.Logging.LogSystem.Warn("Bat dau goi api HisExpMestBltyReq/GetView");
                _ExpMestBltyReqs_Print = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLTY_REQ_1>>(RequestUri.HIS_EXP_MEST_BLTY_REQ_GET_VIEW1, ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Warn("Ket thuc goi api HisExpMestBltyReq/GetView");
                _ExpMestBltyReqs = new List<ExpMestBloodADODetail>();
                if (_ExpMestBltyReqs_Print != null && _ExpMestBltyReqs_Print.Count > 0)
                {
                    List<V_HIS_EXP_MEST_BLTY_REQ_1> expMestBltyReq1Temps = new List<V_HIS_EXP_MEST_BLTY_REQ_1>();
                    AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_BLTY_REQ_1, V_HIS_EXP_MEST_BLTY_REQ_1>();
                    expMestBltyReq1Temps = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_BLTY_REQ_1>>(_ExpMestBltyReqs_Print);

                    var dataGroups = expMestBltyReq1Temps.GroupBy(p => p.BLOOD_TYPE_ID).Select(p => p.ToList()).ToList();
                    foreach (var itemGroup in dataGroups)
                    {
                        var _bloodTypes = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>();
                        ExpMestBloodADODetail ado = new ExpMestBloodADODetail();
                        var firstItem = itemGroup.First();
                        ado.AMOUNT = itemGroup.Sum(o => o.AMOUNT);
                        var data = _bloodTypes.FirstOrDefault(p => p.ID == itemGroup[0].BLOOD_TYPE_ID);
                        if (data != null)
                        {
                            ado.BLOOD_TYPE_CODE = firstItem.BLOOD_TYPE_CODE;
                            ado.BLOOD_TYPE_ID = data.ID;
                            ado.BLOOD_RH_CODE = firstItem.BLOOD_RH_CODE;
                            ado.BLOOD_ABO_CODE = firstItem.BLOOD_ABO_CODE;
                            ado.BLOOD_TYPE_NAME = firstItem.BLOOD_TYPE_NAME;
                            ado.SERVICE_UNIT_CODE = firstItem.SERVICE_UNIT_CODE;
                            ado.SERVICE_UNIT_NAME = firstItem.SERVICE_UNIT_NAME;
                            ado.VOLUME = data.VOLUME;
                        }
                        _ExpMestBltyReqs.Add(ado);
                    }
                }

                _ExpMestBloods = new List<ExpMestBloodADODetail>();
                MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                bloodFilter.EXP_MEST_ID = this._CurrentExpMest.ID;
                Inventec.Common.Logging.LogSystem.Info("Bat dau goi api: HisExpMestBlood/GetView");
                _ExpMestBloods_Print = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>(RequestUri.HIS_EXP_MEST_BLOOD_GET_VIEW, ApiConsumers.MosConsumer, bloodFilter, param);
                Inventec.Common.Logging.LogSystem.Info("Ket thuc goi api: HisExpMestBlood/GetView");
                if (_ExpMestBloods_Print != null && _ExpMestBloods_Print.Count > 0)
                {
                    List<V_HIS_EXP_MEST_BLOOD> expMestBloodTemps = new List<V_HIS_EXP_MEST_BLOOD>();
                    AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_BLOOD, V_HIS_EXP_MEST_BLOOD>();
                    expMestBloodTemps = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_BLOOD>>(_ExpMestBloods_Print);
                    var dataGroups = expMestBloodTemps.GroupBy(p => new { p.BLOOD_TYPE_ID, p.PRICE, p.IMP_PRICE, p.VOLUME }).Select(p => p.ToList()).ToList();
                    foreach (var itemGroup in dataGroups)
                    {
                        ExpMestBloodADODetail ado = new ExpMestBloodADODetail(itemGroup[0]);
                        ado.AMOUNT = itemGroup.Count();
                        _ExpMestBloods.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load dữ liệu vật tư ngoài kho
        /// </summary>
        private void LoadServiceReqMaty()
        {
            try
            {
                if (this._CurrentExpMest == null || this._CurrentExpMest.SERVICE_REQ_ID == null)
                    return;

                ServiceReqMatys = new List<HIS_SERVICE_REQ_MATY>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqMatyFilter filter = new HisServiceReqMatyFilter();
                filter.SERVICE_REQ_ID = this._CurrentExpMest.SERVICE_REQ_ID;
                Inventec.Common.Logging.LogSystem.Info("Bat dau goi api: HisServiceReqMaty/Get");
                ServiceReqMatys = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>(RequestUri.HIS_SERVICE_REQ_MATY_GET, ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Info("Ket thuc goi api: HisServiceReqMaty/Get");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load dữ liệu thuốc ngoài kho
        /// </summary>
        private void LoadServiceReqMety()
        {
            try
            {
                if (this._CurrentExpMest == null || this._CurrentExpMest.SERVICE_REQ_ID == null)
                    return;

                ServiceReqMetys = new List<HIS_SERVICE_REQ_METY>();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqMetyFilter filter = new HisServiceReqMetyFilter();
                filter.SERVICE_REQ_ID = this._CurrentExpMest.SERVICE_REQ_ID;
                Inventec.Common.Logging.LogSystem.Info("Bat dau goi api: HisServiceReqMety/Get");
                ServiceReqMetys = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>(RequestUri.HIS_SERVICE_REQ_METY_GET, ApiConsumers.MosConsumer, filter, param);
                Inventec.Common.Logging.LogSystem.Info("Ket thuc goi api: HisServiceReqMety/Get");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTestServiceReq()
        {
            try
            {
                if (this._CurrentExpMest == null || this._CurrentExpMest.SERVICE_REQ_ID == null)
                    return;

                this.ServiceReqTests = new List<HIS_SERVICE_REQ>();

                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.PARENT_ID = this._CurrentExpMest.SERVICE_REQ_ID;
                this.ServiceReqTests = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(RequestUri.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param);

                if (this.ServiceReqTests == null || this.ServiceReqTests.Count() == 0)
                {
                    return;
                }

                MOS.Filter.HisSereServViewFilter filterSs = new HisSereServViewFilter();
                filterSs.ORDER_FIELD = "TDL_INTRUCTION_TIME";
                filterSs.ORDER_DIRECTION = "ASC";
                filterSs.SERVICE_REQ_IDs = this.ServiceReqTests.Select(o => o.ID).ToList();
                this.SereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filterSs, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
