﻿using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using HIS.Desktop.Plugins.Library.PrintBordereau.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000441.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.MpsBehavior.Mps000441
{
    class Mps000441Behavior : MpsDataBase, ILoad
    {
        public Mps000441Behavior(long? roomId, V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter, List<HIS_SERE_SERV> _sereServs,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans, List<V_HIS_TREATMENT_FEE> _treamentFees, V_HIS_TREATMENT _treatment,
            V_HIS_PATIENT _patient, List<V_HIS_ROOM> _rooms, List<V_HIS_SERVICE> _services, List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            long _totalDayTreatment, string _statusTreatmentOut, string _departmentName, string _roomName, string _userNameReturnResult,
            List<HIS_SERE_SERV_BILL> listSsBill, PrintOption.PayType? _payOption)
            : base(roomId, _treatment)
        {
            this.SereServs = _sereServs;
            this.DepartmentTrans = _departmentTrans;
            this.TreatmentFees = _treamentFees;
            this.Treatment = _treatment;
            this.Rooms = _rooms;
            this.Services = _services;
            this.HeinServiceTypes = _heinServiceTypes;
            this.TotalDayTreatment = _totalDayTreatment;
            this.StatusTreatmentOut = _statusTreatmentOut;
            this.DepartmentName = _departmentName;
            this.UserNameReturnResult = _userNameReturnResult;
            this.CurrentPatientTypeAlter = currentPatientTypeAlter;
            this.RoomName = _roomName;
            this.Patient = _patient;
            this.SereServBills = listSsBill;
            this.PayOption = _payOption;
        }

        bool ILoad.Load(string printTypeCode, string fileName, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint returnEventPrint)
        {
            bool result = false;
            try
            {
                PatientTypeCFG patientTypeCFG = new PatientTypeCFG();
                patientTypeCFG.PATIENT_TYPE__BHYT = HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT;
                patientTypeCFG.PATIENT_TYPE__FEE = HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE;

                SingleKeyValue singleValue = new SingleKeyValue();
                singleValue.departmentName = DepartmentName;
                singleValue.STATUS_TREATMENT_OUT = StatusTreatmentOut;
                singleValue.TOTAL_DAY = TotalDayTreatment;
                singleValue.roomName = RoomName;
                singleValue.USERNAME_RETURN_RESULT = UserNameReturnResult;

                int payOptionValue = (int)(this.PayOption != null ? this.PayOption : (PrintOption.PayType?)1);
                singleValue.PAY_VIEW_OPTION = payOptionValue.ToString();

                bool isPriceWithDifference = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.IS_PRICE_WITH_DIFFERENCE)) == 1 ? true : false;
                bool IsNotSameDepartment = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.MOS__BHYT__CALC_MATERIAL_PACKAGE_PRICE_OPTION)) == 1 ? true : false;
                bool isGroupReqDepartment = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.IS_GROUP_REQUEST_DEPARTMENT)) == 1 ? true : false;
                bool notIncludeIsExpend = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(SdaConfigKey.ConfigKey_NotIncludeIsExpend) == 1 ? true : false;
                HisConfigValue hisConfigValue = new HisConfigValue();
                hisConfigValue.IsPriceWithDifference = isPriceWithDifference;
                hisConfigValue.IsNotSameDepartment = IsNotSameDepartment;
                hisConfigValue.IsGroupReqDepartment = isGroupReqDepartment;
                hisConfigValue.NotIncludeIsExpend = notIncludeIsExpend;

                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                List<HIS_TREATMENT_TYPE> treatmentTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>();

                CommonParam param = new CommonParam();

                List<long> sereServIds = this.SereServs.Select(o => o.ID).ToList();
                List<HIS_SERE_SERV_EXT> sereServExts = new List<HIS_SERE_SERV_EXT>();
                int skip = 0;
                while (sereServIds.Count - skip > 0)
                {
                    var listIds = sereServIds.Skip(skip).Take(100).ToList();
                    skip += 100;

                    HisSereServExtFilter sereServExtFilter = new HisSereServExtFilter();
                    sereServExtFilter.SERE_SERV_IDs = listIds;
                    var ext = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, sereServExtFilter, param);
                    if (ext != null && ext.Count > 0)
                    {
                        sereServExts.AddRange(ext);
                    }
                }

                List<HIS_MATERIAL_TYPE> materialTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MATERIAL_TYPE>();
                List<HIS_DEPARTMENT> departments = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>();

                List<HIS_PATIENT_TYPE> patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                List<HIS_MEDI_ORG> mediOrg = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDI_ORG>();

                HisPatientTypeAlterFilter patientTypeAlterFilter = new HisPatientTypeAlterFilter();
                patientTypeAlterFilter.TREATMENT_ID = this.Treatment.ID;
                patientTypeAlterFilter.ORDER_FIELD = "LOG_TIME";
                patientTypeAlterFilter.ORDER_DIRECTION = "ASC";
                List<HIS_PATIENT_TYPE_ALTER> patientTypeAlters = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);
                List<HIS_SERVICE_UNIT> servuceUnit = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_SERVICE_UNIT>();
                List<HIS_OTHER_PAY_SOURCE> otherPaySource = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_OTHER_PAY_SOURCE>();

                MPS.Processor.Mps000441.PDO.Mps000441PDO rdo = new MPS.Processor.Mps000441.PDO.Mps000441PDO(this.CurrentPatientTypeAlter, patientTypeAlters, DepartmentTrans, TreatmentFees, patientTypeCFG, this.SereServs, sereServExts, Treatment, this.Patient, HeinServiceTypes, Rooms, Services, treatmentTypes, branch, materialTypes, departments, singleValue, hisConfigValue, servuceUnit, patientType, mediOrg, this.SereServBills, otherPaySource);

                #region Run Print
                PrintCustomShow<Mps000441PDO> printShow = new PrintCustomShow<Mps000441PDO>(printTypeCode, fileName, rdo, returnEventPrint, this.isPreview);
                result = printShow.SignRun(Treatment.TREATMENT_CODE, this.RoomId);
                #endregion
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
