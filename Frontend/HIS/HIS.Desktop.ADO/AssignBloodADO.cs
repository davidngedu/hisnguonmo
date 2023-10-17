﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.ADO
{
    public class AssignBloodADO
    {
        public delegate void DelegateProcessDataResult(object data);
        public delegate void DelegateProcessRefeshIcd(string icdCode, string icdMainText, string ictExtraCodes, string ictExtraNames);

        public long TreatmentId { get; set; }
        public long IntructionTime { get; set; }
        public long ServiceReqId { get; set; }
        public string PatientName { get; set; }
        public long PatientDob { get; set; }
        public string GenderName { get; set; }
        public DelegateProcessDataResult DgProcessDataResult { get; set; }
        public DelegateProcessRefeshIcd DgProcessRefeshIcd { get; set; }
        public MOS.EFMODEL.DataModels.V_HIS_SERE_SERV SereServ { get; set; }
        public List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_BLOOD> ExpMestBloods { get; set; }
        public MOS.EFMODEL.DataModels.HIS_TRACKING Tracking { get; set; }

        public AssignBloodADO(long _treatmentId, long _intructionTime, long _serviceReqId)
            : this(_treatmentId, _intructionTime, _serviceReqId, null)
        {

        }

        public AssignBloodADO(long _treatmentId, long _intructionTime, long _serviceReqId, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV _sereServ)
            : this(_treatmentId, _intructionTime, _serviceReqId, _sereServ, null)
        {

        }
        public AssignBloodADO(long _treatmentId, long _intructionTime, long _serviceReqId, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV _sereServ, HIS.Desktop.ADO.AssignBloodADO.DelegateProcessDataResult processDataResult)
        {
            this.TreatmentId = _treatmentId;
            this.IntructionTime = _intructionTime;
            this.ServiceReqId = _serviceReqId;
            this.SereServ = _sereServ;
            this.DgProcessDataResult = processDataResult;

        }
    }
}
