using ACS.DAO.Base;
using ACS.DAO.StagingObject;
using ACS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACS.DAO.AcsAppOtpType
{
    partial class AcsAppOtpTypeGet : EntityBase
    {
        public ACS_APP_OTP_TYPE GetByCode(string code, AcsAppOtpTypeSO search)
        {
            ACS_APP_OTP_TYPE result = null;
            try
            {
                bool valid = true;
                valid = valid && IsNotNullOrEmpty(code);
                if (valid)
                {
                    using (var ctx = new AppContext())
                    {
                        var query = ctx.ACS_APP_OTP_TYPE.AsQueryable().Where(p => p.ACTIVITY_TYPE_CODE == code);
                        if (search.listAcsAppOtpTypeExpression != null && search.listAcsAppOtpTypeExpression.Count > 0)
                        {
                            foreach (var item in search.listAcsAppOtpTypeExpression)
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
