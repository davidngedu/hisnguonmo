﻿using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.PrintBordereau.Base;
using HIS.Desktop.Plugins.Library.PrintBordereau.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000122.PDO;
using MPS.Processor.Mps000122.PDO.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintBordereau.Mps000122
{
    public class Mps000122Behavior : MpsDataBase, ILoad
    {
        private string documentName;

        public Mps000122Behavior(long? roomId, List<HIS_SERE_SERV> _sereServs, List<V_HIS_DEPARTMENT_TRAN> _departmentTrans, List<V_HIS_TREATMENT_FEE> _treamentFees, V_HIS_TREATMENT _treatment, List<V_HIS_ROOM> _rooms, List<V_HIS_SERVICE> _services, List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes, long _totalDayTreatment, string _statusTreatmentOut, string _departmentName, string _userNameReturnResult, string documentname)
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
            this.documentName = documentname;
        }

        bool ILoad.Load(string printTypeCode, string fileName, Inventec.Common.FlexCelPrint.DelegateReturnEventPrint returnEventPrint)
        {
            bool result = false;
            CommonParam param = new CommonParam();
            try
            {
                MPS.Processor.Mps000122.PDO.Config.HeinServiceTypeCFG heinServiceType = new MPS.Processor.Mps000122.PDO.Config.HeinServiceTypeCFG();
                heinServiceType.HEIN_SERVICE_TYPE__HIGHTECH_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;
                heinServiceType.HEIN_SERVICE_TYPE__EXAM_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__KH;

                MPS.Processor.Mps000122.PDO.Config.PatientTypeCFG patientTypeCFG = new MPS.Processor.Mps000122.PDO.Config.PatientTypeCFG();
                patientTypeCFG.PATIENT_TYPE__BHYT = HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT;
                patientTypeCFG.PATIENT_TYPE__FEE = HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE;

                HisTransactionFilter tranFilter = new HisTransactionFilter();
                tranFilter.TREATMENT_ID = this.Treatment.ID;
                List<HIS_TRANSACTION> transactions = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, tranFilter, param);
                TransactionTypeCFG transactionTypeCFG = new TransactionTypeCFG();
                transactionTypeCFG.TRANSACTION_TYPE_ID__BILL = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT;

                transactions = transactions.Where(o => !o.IS_CANCEL.HasValue && !o.SALE_TYPE_ID.HasValue).ToList();

                SingleKeyValue singleValue = new SingleKeyValue();
                singleValue.departmentName = DepartmentName;
                singleValue.statusTreatmentOut = StatusTreatmentOut;
                singleValue.today = TotalDayTreatment;
                singleValue.userNameReturnResult = UserNameReturnResult;

                List<HIS_MATERIAL_TYPE> materialTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MATERIAL_TYPE>();

                MPS.Processor.Mps000122.PDO.Mps000122PDO rdo = null;
                long isShowMedicineLine = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.IS_SHOW_MEDICINE_LINE));
                if (isShowMedicineLine == 1)
                {
                    List<HIS_MEDICINE_TYPE> medicineTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_TYPE>();
                    List<HIS_MEDICINE_LINE> medicineLines = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_LINE>();

                    HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                    serviceReqFilter.TREATMENT_ID = this.Treatment.ID;
                    serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT };
                    List<HIS_SERVICE_REQ> serviceReqs = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);

                    rdo = new MPS.Processor.Mps000122.PDO.Mps000122PDO(DepartmentTrans, TreatmentFees, heinServiceType, patientTypeCFG, transactionTypeCFG, SereServs, transactions, Treatment, HeinServiceTypes, Rooms, Services, medicineTypes, materialTypes, medicineLines, serviceReqs, singleValue);
                }
                else
                {
                    rdo = new MPS.Processor.Mps000122.PDO.Mps000122PDO(DepartmentTrans, TreatmentFees, heinServiceType, patientTypeCFG, transactionTypeCFG, SereServs, transactions, Treatment, HeinServiceTypes, Rooms, Services, materialTypes, singleValue);
                }

                PrintCustomShow<Mps000122PDO> printShow = new PrintCustomShow<Mps000122PDO>(printTypeCode, fileName, rdo, returnEventPrint, this.isPreview);
                result = printShow.SignRun(Treatment.TREATMENT_CODE, this.RoomId, documentName);
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
