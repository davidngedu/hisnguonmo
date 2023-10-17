﻿using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.XmlConfig;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.ConfigCustomizaButton
{
    public class ConfigCustomizaButtonWorker
    {
        static readonly string APPLICATION_CODE = ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"];//Ma ung dung khai bao tren ACS
        static List<SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_BUTTON> ConfigCustomizaButtons;
        static Object thisLock = new Object();

        public static void Init()
        {
            try
            {
                CommonParam param = new CommonParam();
                ConfigCustomizaButtons = SdaCustomizaButtonGet.Get();
                if (ConfigCustomizaButtons == null || ConfigCustomizaButtons.Count == 0) throw new ArgumentNullException("ConfigApps");

                ConfigCustomizaButtons = ConfigCustomizaButtons.Where(o => o.APP_CODE == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.APPLICATION_CODE || String.IsNullOrEmpty(o.APP_CODE)).ToList();

                Inventec.Common.Logging.LogSystem.Debug("ConfigCustomizaButtonWorker.Init. ConfigCustomizaButtons.count =" + (ConfigCustomizaButtons != null && ConfigCustomizaButtons.Count > 0 ? ConfigCustomizaButtons.Count : 0));
            }
            catch (ArgumentNullException ex)
            {
                ConfigCustomizaButtons = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static List<SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_BUTTON> GetByModule(string moduleLink)
        {
            List<SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_BUTTON> data = default(List<SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_BUTTON>);
            try
            {
                if (ConfigCustomizaButtons != null && ConfigCustomizaButtons.Count > 0)
                {
                    var cfgButton = ConfigCustomizaButtons.AsQueryable();
                    cfgButton = cfgButton.Where(o => o.MODULE_LINK == moduleLink);
                    return data = cfgButton.ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return default(List<SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_BUTTON>);
        }

        public static bool ReloadAll()
        {
            bool result = false;
            try
            {
                ConfigCustomizaButtons.Clear();
                Init();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
    }
}
