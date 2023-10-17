﻿using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LisConfig;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ReturnMicrobiologicalResults.Config
{
    internal class LisConfigCFG
    {

        private const string CONFIG_KEY__CONNECTION_TEST__ISALOWSAVEWHENNOTFULLVALUES = "LIS.SAMPLE__IS_ALLOW_SAVE_WHEN_NOT_FULL_VALUES";
        private const string CONFIG_KEY__CONNECTION_TEST__IS_AUTO_CREATE_BARCODE = "LIS.LIS_SAMPLE.IS_AUTO_CREATE_BARCODE";
        //private const string CONFIG_KEY__MUST_APPROVE_SAMPLE = "LIS.LIS_SAMPLE.MUST_APPROVE_SAMPLE";
        private const string CONFIG_KEY__MUST_APPROVE_RESULT = "LIS.LIS_SAMPLE.MUST_APPROVE_RESULT_BEFORE_RETURN";
        private const string CONFIG_KEY__PRINT_BARCODE_BY_BARTENDER = "LIS.LIS_SAMPLE.PRINT_BARCODE.BY_BARTENDER";
        private const string CONFIG_KEY__SHOW_BUTTON_APPROVE = "LIS.LIS_SAMPLE.IS_SHOW_BUTTON_APPROVE_SAMPLE";
        private const string CONFIG_KEY__SHOW_FROM_SAMPLE_INFO = "LIS.LIS_SAMPLE.TAKE_SAMPLE.IS_SHOW_FORM_SAMPLE_INFO";
        private const string CONFIG_KEY__IS_PRINT_WHEN_APPROVE_RESULT = "LIS.LIS_SAMPLE.WHEN_APPROVE_RESULT.IS_PRINT";
        private const string CONFIG_KEY__MUST_TICK_RUN_AGAIN_WHEN_RUN_TEST = "LIS.LIS_SAMPLE_SERVICE.MUST_TICK_RUN_AGAIN_WHEN_RERUN_TEST";
        private const string CONFIG_KEY__IS_NOT_RETURN_LIS_SERVICE_WHEN_RUNNING = "LIS.LIS_SAMPLE_SERVICE.WHEN_RUNNING.IS_NOT_RETURN_TO_LIS_SERVICE";
        private const string CONFIG_KEY__ALLOW_TO_EDIT_APPROVE_RESULT_TIME = "LIS.LIS_SAMPLE.ALLOW_TO_EDIT_APPROVE_RESULT_TIME";

        //internal static string MUST_APPROVE_SAMPLE;
        internal static string IS_AUTO_CREATE_BARCODE;
        internal static string IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT;
        internal static string PRINT_BARCODE_BY_BARTENDER;
        internal static string MUST_APPROVE_RESULT;
        internal static string SHOW_BUTTON_APPROVE;
        internal static string SHOW_FORM_SAMPLE_INFO;
        internal static string IS_PRINT_WHEN_APPROVE_RESULT;
        internal static string MUST_TICK_RUN_AGAIN;        
        internal static bool ALLOW_TO_EDIT_APPROVE_RESULT_TIME;        

        internal static void LoadConfig()
        {
            try
            {
                //MUST_APPROVE_SAMPLE = GetValue(CONFIG_KEY__MUST_APPROVE_SAMPLE);
                IS_AUTO_CREATE_BARCODE = GetValue(CONFIG_KEY__CONNECTION_TEST__IS_AUTO_CREATE_BARCODE);
                IS_ALLOW_SAVE_WHEN_NOT_FULL_RESULT = GetValue(CONFIG_KEY__CONNECTION_TEST__ISALOWSAVEWHENNOTFULLVALUES);
                PRINT_BARCODE_BY_BARTENDER = GetValue(CONFIG_KEY__PRINT_BARCODE_BY_BARTENDER);
                MUST_APPROVE_RESULT = GetValue(CONFIG_KEY__MUST_APPROVE_RESULT);
                SHOW_BUTTON_APPROVE = GetValue(CONFIG_KEY__SHOW_BUTTON_APPROVE);
                SHOW_FORM_SAMPLE_INFO = GetValue(CONFIG_KEY__SHOW_FROM_SAMPLE_INFO);
                IS_PRINT_WHEN_APPROVE_RESULT = GetValue(CONFIG_KEY__IS_PRINT_WHEN_APPROVE_RESULT);
                MUST_TICK_RUN_AGAIN = GetValue(CONFIG_KEY__MUST_TICK_RUN_AGAIN_WHEN_RUN_TEST);
                ALLOW_TO_EDIT_APPROVE_RESULT_TIME = GetValue(CONFIG_KEY__ALLOW_TO_EDIT_APPROVE_RESULT_TIME) == "1" ? true : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return LisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }
    }
}
