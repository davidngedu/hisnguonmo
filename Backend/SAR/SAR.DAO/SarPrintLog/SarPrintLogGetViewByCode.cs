using SAR.DAO.Base;
using SAR.DAO.StagingObject;
using SAR.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAR.DAO.SarPrintLog
{
    partial class SarPrintLogGet : EntityBase
    {
        public V_SAR_PRINT_LOG GetViewByCode(string code, SarPrintLogSO search)
        {
            V_SAR_PRINT_LOG result = null;
            try
            {
                bool valid = true;
                valid = valid && IsNotNullOrEmpty(code);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.V_SAR_PRINT_LOG.AsQueryable().Where(p => p.PRINT_LOG_CODE == code);
                        if (search.listVSarPrintLogExpression != null && search.listVSarPrintLogExpression.Count > 0)
                        {
                            foreach (var item in search.listVSarPrintLogExpression)
                            {
                                query = query.Where(item);
                            }
                        }
                        result = query.SingleOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => search), search), LogType.Error);
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
