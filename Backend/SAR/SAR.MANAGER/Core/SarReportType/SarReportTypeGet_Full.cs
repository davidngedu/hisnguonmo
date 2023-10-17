using SAR.EFMODEL.DataModels;
using SAR.MANAGER.Core.SarReportType.Get.Ev;
using SAR.MANAGER.Core.SarReportType.Get.ListEv;
using SAR.MANAGER.Core.SarReportType.Get.ListV;
using SAR.MANAGER.Core.SarReportType.Get.V;
using Inventec.Core;
using System;
using System.Collections.Generic;

namespace SAR.MANAGER.Core.SarReportType
{
    partial class SarReportTypeGet : BeanObjectBase, IDelegacyT
    {
        object entity;

        internal SarReportTypeGet(CommonParam param, object data)
            : base(param)
        {
            entity = data;
        }

        T IDelegacyT.Execute<T>()
        {
            T result = default(T);
            try
            {
                if (typeof(T) == typeof(List<SAR_REPORT_TYPE>))
                {
                    ISarReportTypeGetListEv behavior = SarReportTypeGetListEvBehaviorFactory.MakeISarReportTypeGetListEv(param, entity);
                    result = behavior != null ? (T)System.Convert.ChangeType(behavior.Run(), typeof(T)) : default(T);
                }
                else if (typeof(T) == typeof(SAR_REPORT_TYPE))
                {
                    ISarReportTypeGetEv behavior = SarReportTypeGetEvBehaviorFactory.MakeISarReportTypeGetEv(param, entity);
                    result = behavior != null ? (T)System.Convert.ChangeType(behavior.Run(), typeof(T)) : default(T);
                }
                else if (typeof(T) == typeof(List<V_SAR_REPORT_TYPE>))
                {
                    ISarReportTypeGetListV behavior = SarReportTypeGetListVBehaviorFactory.MakeISarReportTypeGetListV(param, entity);
                    result = behavior != null ? (T)System.Convert.ChangeType(behavior.Run(), typeof(T)) : default(T);
                }
                else if (typeof(T) == typeof(V_SAR_REPORT_TYPE))
                {
                    ISarReportTypeGetV behavior = SarReportTypeGetVBehaviorFactory.MakeISarReportTypeGetV(param, entity);
                    result = behavior != null ? (T)System.Convert.ChangeType(behavior.Run(), typeof(T)) : default(T);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = default(T);
            }
            return result;
        }
    }
}
