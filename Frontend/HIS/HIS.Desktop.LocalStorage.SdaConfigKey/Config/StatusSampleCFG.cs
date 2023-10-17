﻿using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.LocalStorage.SdaConfig;
using Inventec.Common.Logging;
using Inventec.Core;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class StatusSampleCFG
    {

        private static long sampleSttIdReturnResult;
        public static long SAMPLE_STT_ID__RETURN_RESULT
        {
            get
            {
                if (sampleSttIdReturnResult == 0)
                {
                    sampleSttIdReturnResult = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.SAMPLE_STT_CODE__RETURN_RESULT));
                }
                return sampleSttIdReturnResult;
            }
            set
            {
                sampleSttIdReturnResult = value;
            }
        }

        private static long sampleSttIdResult;
        public static long SAMPLE_STT_ID__RESULT
        {
            get
            {
                if (sampleSttIdResult == 0)
                {
                    sampleSttIdResult = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.SAMPLE_STT_CODE__RESULT));
                }
                return sampleSttIdResult;
            }
            set
            {
                sampleSttIdResult = value;
            }
        }

        private static long sampleSttIdSpecimen;
        public static long SAMPLE_STT_ID__SPECIMEN
        {
            get
            {
                if (sampleSttIdSpecimen == 0)
                {
                    sampleSttIdSpecimen = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.SAMPLE_STT_CODE__SPECIMEN));
                }
                return sampleSttIdSpecimen;
            }
            set
            {
                sampleSttIdSpecimen = value;
            }
        }

        private static long sampleSttIdUnSpecimen;
        public static long SAMPLE_STT_ID__UNSPECIMEN
        {
            get
            {
                if (sampleSttIdUnSpecimen == 0)
                {
                    sampleSttIdUnSpecimen = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.SAMPLE_STT_CODE__UNSPECIMEN));
                }
                return sampleSttIdUnSpecimen;
            }
            set
            {
                sampleSttIdUnSpecimen = value;
            }
        }

        private static long GetId(string code)
        {
            CommonParam param = new CommonParam();
            long result = 0;
            try
            {
                result = Inventec.Common.TypeConvert.Parse.ToInt64(code);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }
    }
}
