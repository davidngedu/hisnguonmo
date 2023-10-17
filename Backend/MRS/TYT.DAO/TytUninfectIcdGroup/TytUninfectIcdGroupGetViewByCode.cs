using TYT.DAO.Base;
using TYT.DAO.StagingObject;
using TYT.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TYT.DAO.TytUninfectIcdGroup
{
    partial class TytUninfectIcdGroupGet : EntityBase
    {
        public V_TYT_UNINFECT_ICD_GROUP GetViewByCode(string code, TytUninfectIcdGroupSO search)
        {
            V_TYT_UNINFECT_ICD_GROUP result = null;
            try
            {
                bool valid = true;
                valid = valid && IsNotNullOrEmpty(code);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.V_TYT_UNINFECT_ICD_GROUP.AsQueryable().Where(p => p.UNINFECT_ICD_GROUP_CODE == code);
                        if (search.listVTytUninfectIcdGroupExpression != null && search.listVTytUninfectIcdGroupExpression.Count > 0)
                        {
                            foreach (var item in search.listVTytUninfectIcdGroupExpression)
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
