﻿using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
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

namespace HIS.Desktop.LocalStorage.ConfigHideControl
{
    public class ConfigHideControlWorker
    {
        static readonly string APPLICATION_CODE = ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"];//Ma ung dung khai bao tren ACS
        static List<SDA.EFMODEL.DataModels.SDA_HIDE_CONTROL> ConfigHideControls;
        static Object thisLock = new Object();

        public static void Init()
        {
            try
            {
                CommonParam param = new CommonParam();
                ConfigHideControls = SdaHideControlGet.Get();
                if (ConfigHideControls == null || ConfigHideControls.Count == 0) throw new ArgumentNullException("ConfigApps");

                ConfigHideControls = ConfigHideControls.Where(o => o.APP_CODE == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.APPLICATION_CODE || String.IsNullOrEmpty(o.APP_CODE)).ToList();

                var branch = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.BranchWorker.GetCurrentBranchId());
                ConfigHideControls = ConfigHideControls.Where(o => (branch != null && o.BRANCH_CODE == branch.BRANCH_CODE) || String.IsNullOrEmpty(o.BRANCH_CODE)).ToList();
                Inventec.Common.Logging.LogSystem.Debug("ConfigHideControlWorker.Init. ConfigHideControls.count =" + (ConfigHideControls != null && ConfigHideControls.Count > 0 ? ConfigHideControls.Count : 0));
            }
            catch (ArgumentNullException ex)
            {
                ConfigHideControls = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static List<SDA.EFMODEL.DataModels.SDA_HIDE_CONTROL> GetByModule(string moduleLink)
        {
            List<SDA.EFMODEL.DataModels.SDA_HIDE_CONTROL> data = default(List<SDA.EFMODEL.DataModels.SDA_HIDE_CONTROL>);
            try
            {
                if (ConfigHideControls != null && ConfigHideControls.Count > 0)
                {
                    var cfgButton = ConfigHideControls.AsQueryable();
                    cfgButton = cfgButton.Where(o => o.MODULE_LINK == moduleLink);
                    return data = cfgButton.ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return default(List<SDA.EFMODEL.DataModels.SDA_HIDE_CONTROL>);
        }

        public static bool ReloadAll()
        {
            bool result = false;
            try
            {
                ConfigHideControls.Clear();
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
