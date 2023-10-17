﻿using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.ConfigApplication
{
    class SdaConfigAppGet
    {
        internal static List<SDA.EFMODEL.DataModels.SDA_CONFIG_APP> Get()
        {
            try
            {
                CommonParam param = new CommonParam();
                SdaConfigAppFilter configAppFilter = new SdaConfigAppFilter();
                configAppFilter.IS_ACTIVE = 1;
                configAppFilter.APP_CODE_ACCEPT = ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"];
                return new BackendAdapter(param).Get<List<SDA.EFMODEL.DataModels.SDA_CONFIG_APP>>("/api/SdaConfigApp/Get", ApiConsumers.SdaConsumer, configAppFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }
    }
}
