using ACS.DAO.Base;
using ACS.DAO.StagingObject;
using ACS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACS.DAO.AcsRoleBase
{
    partial class AcsRoleBaseGet : EntityBase
    {
        public ACS_ROLE_BASE GetByCode(string code, AcsRoleBaseSO search)
        {
            ACS_ROLE_BASE result = null;
            try
            {
                bool valid = true;
                valid = valid && IsNotNullOrEmpty(code);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.ACS_ROLE_BASE.AsQueryable().Where(p => p.ROLE_BASE_CODE == code);
                        if (search.listAcsRoleBaseExpression != null && search.listAcsRoleBaseExpression.Count > 0)
                        {
                            foreach (var item in search.listAcsRoleBaseExpression)
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
