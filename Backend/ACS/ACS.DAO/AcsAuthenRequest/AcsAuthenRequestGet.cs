using ACS.DAO.Base;
using ACS.DAO.StagingObject;
using ACS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACS.DAO.AcsAuthenRequest
{
    partial class AcsAuthenRequestGet : EntityBase
    {
        public List<ACS_AUTHEN_REQUEST> Get(AcsAuthenRequestSO search, CommonParam param)
        {
            List<ACS_AUTHEN_REQUEST> list = new List<ACS_AUTHEN_REQUEST>();
            try
            {
                bool valid = true;
                valid = valid && IsNotNull(param);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.ACS_AUTHEN_REQUEST.AsNoTracking().AsQueryable();
                        if (search.listAcsAuthenRequestExpression != null && search.listAcsAuthenRequestExpression.Count > 0)
                        {
                            foreach (var item in search.listAcsAuthenRequestExpression)
                            {
                                query = query.Where(item);
                            }
                        }
                        
                        if (!string.IsNullOrWhiteSpace(search.OrderField) && !string.IsNullOrWhiteSpace(search.OrderDirection))
						{
							if (!param.Start.HasValue || !param.Limit.HasValue)
							{
								list = query.OrderByProperty(search.OrderField, search.OrderDirection).ToList(); 
							}
							else
							{
								param.Count = (from r in query select r).Count();
								query = query.OrderByProperty(search.OrderField, search.OrderDirection);
								if (param.Count <= param.Limit.Value && param.Start.Value == 0)
								{
									list = query.ToList();
								}
								else
								{
									list = query.Skip(param.Start.Value).Take(param.Limit.Value).ToList();
								}
							}
						}
						else
						{
							list = query.ToList();
						}
                    }
                }
            }
            catch (Exception ex)
            {
                Logging(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => search), search) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param), LogType.Error);
                LogSystem.Error(ex);
                list.Clear();
            }
            return list;
        }

        public ACS_AUTHEN_REQUEST GetById(long id, AcsAuthenRequestSO search)
        {
            ACS_AUTHEN_REQUEST result = null;
            try
            {
                bool valid = true;
                valid = valid && IsGreaterThanZero(id);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.ACS_AUTHEN_REQUEST.AsQueryable().Where(p => p.ID == id);
                        if (search.listAcsAuthenRequestExpression != null && search.listAcsAuthenRequestExpression.Count > 0)
                        {
                            foreach (var item in search.listAcsAuthenRequestExpression)
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
                Logging(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => id), id) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => search), search), LogType.Error);
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
