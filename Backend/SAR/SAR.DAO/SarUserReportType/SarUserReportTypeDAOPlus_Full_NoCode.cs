using SAR.DAO.StagingObject;
using SAR.EFMODEL.DataModels;
using Inventec.Core;
using System;
using System.Collections.Generic;

namespace SAR.DAO.SarUserReportType
{
    public partial class SarUserReportTypeDAO : EntityBase
    {
        public List<V_SAR_USER_REPORT_TYPE> GetView(SarUserReportTypeSO search, CommonParam param)
        {
            List<V_SAR_USER_REPORT_TYPE> result = new List<V_SAR_USER_REPORT_TYPE>();
            try
            {
                result = GetWorker.GetView(search, param);
            }
            catch (Exception ex)
            {
                param.HasException = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
                result.Clear();
            }
            return result;
        }

        public V_SAR_USER_REPORT_TYPE GetViewById(long id, SarUserReportTypeSO search)
        {
            V_SAR_USER_REPORT_TYPE result = null;

            try
            {
                result = GetWorker.GetViewById(id, search);
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
