﻿using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TrackingInMediRecord.TrackingInMediRecord
{
    class TrackingInMediRecordBehavior : Tool<IDesktopToolContext>, ITrackingInMediRecord
    { 
        object[] entity;
        V_HIS_MEDI_RECORD MediRecord; 

        internal TrackingInMediRecordBehavior()
            : base()
        {

        }

        internal TrackingInMediRecordBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object ITrackingInMediRecord.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module module = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            module = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        else if (entity[i] is V_HIS_MEDI_RECORD)
                        {
                            MediRecord = (V_HIS_MEDI_RECORD)entity[i];
                        }
                    }
                }
                if (module != null)
                {
                    result = new frmTrackingInMediRecord(module, MediRecord);
                }
                else
                    result = null;

                if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => entity), entity));
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
