﻿using Inventec.Common.Logging;
using Inventec.Core;
using MOS.Filter;
using SDA.EFMODEL.DataModels;
using System;
using System.Linq;
using SDA.Filter;
using Inventec.Common.LocalStorage.SdaConfig;

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class IcdGeneraCFG
    {
        private const string ICD_GENERA_KEY = "HIS.Desktop.Plugins.AutoCheckIcd";

        private static string autoCheckIcd;
        public static string AutoCheckIcd
        {
            get
            {
                if (string.IsNullOrEmpty(autoCheckIcd))
                {
                    autoCheckIcd = GetCode(ICD_GENERA_KEY);
                }
                return autoCheckIcd;
            }
            set
            {
                autoCheckIcd = value;
            }
        }

        private static string GetCode(string code)
        {
            string result = "";
            try
            {
                result = SdaConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = "";
            }
            return result;
        }
    }
}
