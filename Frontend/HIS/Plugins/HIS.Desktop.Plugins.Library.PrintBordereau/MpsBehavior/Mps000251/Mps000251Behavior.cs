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

namespace HIS.Desktop.Plugins.Library.PrintBordereau.Mps000251
{
    public class Mps000251Behavior : MpsDataBase, ILoad
    {
        private string documentName;
        public Mps000251Behavior(long? roomId, List<HIS_SERE_SERV> _sereServs, List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_TREATMENT_FEE> _treamentFees, V_HIS_TREATMENT _treatment, List<V_HIS_ROOM> _rooms,
            List<V_HIS_SERVICE> _services, List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes, long _totalDayTreatment,
            string _statusTreatmentOut, string _departmentName, string _userNameReturnResult, string documentname,
            List<HIS_SERE_SERV_BILL> listSsBill)
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
            this.SereServBills = listSsBill;
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

                List<HIS_SERE_SERV> sereServNotBills = new List<HIS_SERE_SERV>();

                if (this.SereServBills != null && this.SereServBills.Count > 0)
                {
                    List<long> sereServIdBills = this.SereServBills.Select(o => o.SERE_SERV_ID).ToList();
                    sereServNotBills = this.SereServs.Where(o => !sereServIdBills.Contains(o.ID)).ToList();
                }
                else
                {
                    sereServNotBills = this.SereServs;
                }

                SingleKeyValue singleValue = new SingleKeyValue();
                singleValue.departmentName = DepartmentName;
                singleValue.statusTreatmentOut = StatusTreatmentOut;
                singleValue.today = TotalDayTreatment;
                singleValue.userNameReturnResult = UserNameReturnResult;

                List<HIS_MATERIAL_TYPE> materialTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MATERIAL_TYPE>();

                MPS.Processor.Mps000122.PDO.Mps000122PDO rdo = new MPS.Processor.Mps000122.PDO.Mps000122PDO(DepartmentTrans, TreatmentFees, heinServiceType, patientTypeCFG, transactionTypeCFG, sereServNotBills, transactions, Treatment, HeinServiceTypes, Rooms, Services, materialTypes, singleValue);

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
