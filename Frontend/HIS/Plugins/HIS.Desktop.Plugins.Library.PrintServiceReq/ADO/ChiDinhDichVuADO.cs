﻿using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintServiceReq.ADO
{
    class ChiDinhDichVuADO
    {
        public HIS_TREATMENT treament { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER patientTypeAlter { get; set; }
        public string FirstExamRoomName { get; set; }
        public decimal Ratio { get; set; }
        public string BedRoomName { get; set; }
        public string DepartmentName { get; set; }
        public List<HIS_SERE_SERV_DEPOSIT> ListSereServDeposit { get; set; }
        public List<HIS_SERE_SERV_BILL> ListSereServBill { get; set; }
        public List<V_HIS_TRANSACTION> ListTransaction { get; set; }
        public HIS_DHST _HIS_DHST { get; set; }
        public HIS_WORK_PLACE _WORK_PLACE { get; set; }
        public string Gate { get; set; }
        public List<HIS_CARD> ListCard { get; set; }
    }
}
