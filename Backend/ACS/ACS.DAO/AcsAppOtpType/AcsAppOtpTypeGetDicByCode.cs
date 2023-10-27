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
        public Dictionary<string, ACS_APP_OTP_TYPE> GetDicByCode(AcsAppOtpTypeSO search, CommonParam param)
        {
            Dictionary<string, ACS_APP_OTP_TYPE> dic = new Dictionary<string, ACS_APP_OTP_TYPE>();
            try
            {
                List<ACS_APP_OTP_TYPE> listRecord = Get(search, param);
                if (listRecord != null)
                {
                    foreach (var item in listRecord)
                    {
                        if (!dic.ContainsKey(item.ACTIVITY_TYPE_CODE))
                        {
                            dic.Add(item.ACTIVITY_TYPE_CODE, item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => search), search) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param), LogType.Error);
                LogSystem.Error(ex);
                dic.Clear();
            }
            return dic;
        }
    }
}
