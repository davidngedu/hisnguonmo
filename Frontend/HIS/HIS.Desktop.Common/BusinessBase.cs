﻿using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Token;
using System;

namespace HIS.Desktop.Common
{
    public abstract class BusinessBase : EntityBase
    {
        public BusinessBase()
            : base()
        {
            param = new CommonParam();
            try
            {
                UserName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception)
            {
            }
        }

        public BusinessBase(CommonParam paramBusiness)
            : base()
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
            try
            {
                UserName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception)
            {
            }
        }

        protected CommonParam param { get; set; }
        
        public ApiResultObject<T> PackCollectionResult<T>(T listData)
        {
            ApiResultObject<T> result = new ApiResultObject<T>();
            try
            {
                result.SetValue(listData, listData != null, param);
            }
            catch (Exception ex)
            {
                Logging("Co exception khi dong goi ApiResultObject.", LogType.Error);
                LogSystem.Error(ex);
            }
            return result;
        }

        public ApiResultObject<T> PackSingleResult<T>(T data)
        {
            ApiResultObject<T> result = new ApiResultObject<T>();
            try
            {
                result.SetValue(data, data != null, param);
            }
            catch (Exception ex)
            {
                Logging("Co exception khi dong goi ApiResultObject.", LogType.Error);
                LogSystem.Error(ex);
            }
            return result;
        }

        public static bool TokenCheck()
        {
            bool result = false;
            try
            {
                var tokenData = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData();
                if (tokenData != null && tokenData.ExpireTime <= DateTime.Now.AddMinutes(-1))
                {
                    result =  TokenManager.Renew();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }
    }
}
