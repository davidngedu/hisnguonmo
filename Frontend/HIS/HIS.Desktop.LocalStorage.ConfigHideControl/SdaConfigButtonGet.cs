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

namespace HIS.Desktop.LocalStorage.ConfigButton
{
    class SdaConfigButtonGet
    {
        internal static List<SDA.EFMODEL.DataModels.SDA_MODULE_BUTTON> Get()
        {
            try
            {
                CommonParam param = new CommonParam();
                SdaConfigAppFilter filter = new SdaConfigAppFilter();
                filter.IS_ACTIVE = 1;
                filter.APP_CODE_ACCEPT = ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"];
                return new BackendAdapter(param).Get<List<SDA.EFMODEL.DataModels.SDA_MODULE_BUTTON>>("api/SdaModuleButton/Get", ApiConsumers.SdaConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }
    }
}
