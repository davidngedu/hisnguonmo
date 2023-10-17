﻿using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class AllowPrintFinishCFG
    {
        private static long AlowPrintFinish;
        public static long ALOW_PRINT_FINISH
        {
            get
            {
                if (AlowPrintFinish == 0)
                {
                    AlowPrintFinish = GetValue(SdaConfigs.Get<string>(ExtensionConfigKey.HIS_DESKTOP_ALLOW_PRINT_FINISH));
                }
                return AlowPrintFinish;
            }
            set
            {
                AlowPrintFinish = value;
            }
        }

        private static long GetValue(string code)
        {
            long result = 0;
            try
            {
                result = Inventec.Common.TypeConvert.Parse.ToInt64(code);
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
                result = 0;
            }
            return result;
        }

        //public static string HIS_DESKTOP_ALLOW_PRINT_FINISH { get; set; }
    }
}
