﻿using Inventec.Common.Logging;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;

namespace MRS.MANAGER.Config
{
    class ConfigUtil
    {
        internal static long GetLongConfig(string code)
        {
            long result = 0;
            try
            {
                result = long.Parse(GetStrConfig(code));
            }
            catch (Exception ex)
            {
                LogSystem.Error("Loi khi lay Config: " + code, ex);
                result = 0;
            }
            return result;
        }

        internal static decimal? GetDecimalConfig(string code)
        {
            decimal? result = null;
            try
            {
                result = decimal.Parse(GetStrConfig(code));
            }
            catch (Exception ex)
            {
                LogSystem.Error("Loi khi lay Config: " + code, ex);
                result = 0;
            }
            return result;
        }

        internal static int GetIntConfig(string code)
        {
            int result = 0;
            try
            {
                result = int.Parse(GetStrConfig(code));
            }
            catch (Exception ex)
            {
                LogSystem.Error("Loi khi lay Config: " + code, ex);
                result = 0;
            }
            return result;
        }

        internal static string GetStrConfig(string code)
        {
            string result = null;
            try
            {
                var config = Loader.dictionaryConfig[code];
                if (config == null) throw new ArgumentNullException(code);
                result = !String.IsNullOrEmpty(config.VALUE) ? config.VALUE : (!String.IsNullOrEmpty(config.DEFAULT_VALUE) ? config.DEFAULT_VALUE : "");
                if (String.IsNullOrEmpty(result)) throw new ArgumentNullException(code);

            }
            catch (Exception ex)
            {
                LogSystem.Error("Loi khi lay Config: " + code, ex);
            }
            return result;
        }

        internal static List<string> GetStrConfigs(string code)
        {
            List<string> result = new List<string>();
            try
            {
                string str = GetStrConfig(code);
                string[] arr = str.Split(',');
                if (arr != null)
                {
                    result.AddRange(arr);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return result;
        }
    }
}
