﻿using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignNutritionEdit.ADO
{
    public class ChiDinhDichVuADO
    {
        public HIS_TREATMENT treament { get; set; }
        public V_HIS_SERVICE_REQ ServiceReqPrint { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER patientTypeAlter { get; set; }
        public string firstExamRoomName { get; set; }
    }
}
