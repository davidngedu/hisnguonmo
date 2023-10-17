﻿using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ChangeExamRoomProcess.ChangeExamRoomProcess
{
    class ChangeExamRoomProcessFactory
    {
        internal static IChangeExamRoomProcess MakeICrateType(CommonParam param, object[] data)
        {

            IChangeExamRoomProcess result = null;
            //Inventec.Desktop.Common.Modules.Module moduleData = null;
            //V_HIS_TREATMENT_BED_ROOM treatmentBedRoom = null;
            long treatmentId = 0;
            try
            {
                result = new ChangeExamRoomProcessBehavior(param, data);
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
