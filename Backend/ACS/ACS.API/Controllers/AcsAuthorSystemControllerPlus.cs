using Inventec.Common.Logging;
using Inventec.Core;
using ACS.API.Base;
using ACS.EFMODEL.DataModels;
using ACS.MANAGER.AcsAuthorSystem;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ACS.API.Controllers
{
    public partial class AcsAuthorSystemController : BaseApiController
    {
        [AllowAnonymous]
        [HttpGet]
        [ApiParamFilter(typeof(ApiParam<string>), "param")]
        [ActionName("GetInfoByCode")]
        public ApiResult GetInfoByCode(ApiParam<string> param)
        {
            try
            {
                ApiResultObject<ACS_AUTHOR_SYSTEM> result = new ApiResultObject<ACS_AUTHOR_SYSTEM>(null);
                if (param != null)
                {
                    AcsAuthorSystemManager mng = new AcsAuthorSystemManager(param.CommonParam);
                    result = mng.GetInfoByCode(param.ApiData);
                }
                return new ApiResult(result, this.ActionContext);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                return null;
            }
        }
    }
}
