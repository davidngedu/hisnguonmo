﻿using EMR.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMR.Desktop.Plugins.EmrSign
{
    class EmrConfigCFG
    {
        private static List<EMR.EFMODEL.DataModels.EMR_CONFIG> emrConfigs;
        public static List<EMR.EFMODEL.DataModels.EMR_CONFIG> EmrConfigs
        {
            get
            {
                if (emrConfigs == null)
                {
                    CommonParam paramCommon = new CommonParam();
                    EmrConfigFilter filter = new EmrConfigFilter();
                    filter.IS_ACTIVE = 1;
                   var rs = ApiConsumers.EmrConsumer.Get<Inventec.Core.ApiResultObject<List<EMR.EFMODEL.DataModels.EMR_CONFIG>>>(EMR.URI.EmrConfig.GET, paramCommon, filter);
                   emrConfigs = rs != null && rs.Data != null ? rs.Data : null;
                }
                return emrConfigs;
            }
            set
            {
                emrConfigs = value;
            }
        }
    }
}
