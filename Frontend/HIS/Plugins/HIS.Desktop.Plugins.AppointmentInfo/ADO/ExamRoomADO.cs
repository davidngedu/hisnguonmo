﻿using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AppointmentInfo.ADO
{
    public class ExamRoomADO
    {
        public bool IsCheck { get; set; }
        public long ID { get; set; }
        public long EXECUTE_ROOM_ID { get; set; }
        public string EXECUTE_ROOM_CODE { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public long? MAX_APPOINTMENT_BY_DAY { get; set; }

        public ExamRoomADO() { }

        public ExamRoomADO(HIS_EXECUTE_ROOM data)
        {
            try
            {
                if (data != null)
                {
                    this.EXECUTE_ROOM_ID = data.ID;
                    this.ID = data.ROOM_ID;
                    this.EXECUTE_ROOM_CODE = data.EXECUTE_ROOM_CODE;
                    this.EXECUTE_ROOM_NAME = data.EXECUTE_ROOM_NAME;
                    this.MAX_APPOINTMENT_BY_DAY = data.MAX_APPOINTMENT_BY_DAY;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
