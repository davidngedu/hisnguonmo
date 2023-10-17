﻿using HIS.Desktop.LocalStorage.HisConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CheckInfoBHYT
{
    public class BHXHLoginCFG
    {
        private const string CONFIG_KEY = "HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS";
        private const string CONFIG_KEY_ADDRESS = "HIS.CHECK_HEIN_CARD.BHXH__ADDRESS";
        private const string CONFIG_KEY_ADDRESS_OPTION = "HIS.CHECK_HEIN_CARD.BHXH__ADDRESS__OPTION";

        //private static string username;
        //public static string USERNAME
        //{
        //    get
        //    {
        //        if (String.IsNullOrEmpty(username))
        //        {
        //            username = Get(HisConfigs.Get<string>(CONFIG_KEY), 0);
        //        }
        //        return username;
        //    }
        //    set { username = value; }
        //}

        //private static string password;
        //public static string PASSWORD
        //{
        //    get
        //    {
        //        if (String.IsNullOrEmpty(password))
        //        {
        //            password = Get(HisConfigs.Get<string>(CONFIG_KEY), 1);
        //        }
        //        return password;
        //    }
        //    set { password = value; }
        //}

        //private static string address;
        //public static string ADDRESS
        //{
        //    get
        //    {
        //        if (String.IsNullOrEmpty(address))
        //        {
        //            address = HisConfigs.Get<string>(CONFIG_KEY_ADDRESS).Trim();
        //        }
        //        return address;
        //    }
        //    set { address = value; }
        //}

        public static string USERNAME;
        public static string PASSWORD;
        public static string ADDRESS;
        public static long ADDRESS_OPTION;

        public static void LoadConfig()
        {
            try
            {
                USERNAME = Get(HisConfigs.Get<string>(CONFIG_KEY), 0);
                //Inventec.Common.Logging.LogSystem.Error("LoadConfig >>>>>>>>>>>>> : USERNAME" + USERNAME);
                PASSWORD = Get(HisConfigs.Get<string>(CONFIG_KEY), 1);
                //Inventec.Common.Logging.LogSystem.Error("LoadConfig >>>>>>>>>>>>> : PASSWORD" + PASSWORD);
                ADDRESS = HisConfigs.Get<string>(CONFIG_KEY_ADDRESS).Trim();
                // Inventec.Common.Logging.LogSystem.Error("LoadConfig >>>>>>>>>>>>> : ADDRESS" + ADDRESS);
                ADDRESS_OPTION = HisConfigs.Get<long>(CONFIG_KEY_ADDRESS_OPTION);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string Get(string value, int index)
        {
            string user = "";
            try
            {
                if (!String.IsNullOrEmpty(value))
                {
                    var data = value.Split(':');
                    if (data != null && data.Length >= index)
                    {
                        user = data[index].Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                user = "";
            }
            return user;
        }
    }
}
