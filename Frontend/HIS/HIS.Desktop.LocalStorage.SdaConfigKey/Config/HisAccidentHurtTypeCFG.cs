﻿using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.LocalStorage.SdaConfig;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class HisAccidentHurtTypeCFG
    {
        private static long hisAccidentHurtTypeId__Animal;
        public static long HisAccidentHurtTypeId__Animal
        {
            get
            {
                if (hisAccidentHurtTypeId__Animal <= 0)
                {
                    hisAccidentHurtTypeId__Animal = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.HIS_ACCIDENT_HURT_TYPE_CODE__ANIMAL));
                }
                return hisAccidentHurtTypeId__Animal;
            }
        }

        private static long hisAccidentHurtTypeId__Burn;
        public static long HisAccidentHurtTypeId__Burn
        {
            get
            {
                if (hisAccidentHurtTypeId__Burn <= 0)
                {
                    hisAccidentHurtTypeId__Burn = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.HIS_ACCIDENT_HURT_TYPE_CODE__BURN));
                }
                return hisAccidentHurtTypeId__Burn;
            }
        }

        private static long hisAccidentHurtTypeId__Fall;
        public static long HisAccidentHurtTypeId__Fall
        {
            get
            {
                if (hisAccidentHurtTypeId__Fall <= 0)
                {
                    hisAccidentHurtTypeId__Fall = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.HIS_ACCIDENT_HURT_TYPE_CODE__FALL));
                }
                return hisAccidentHurtTypeId__Fall;
            }
        }

        private static long hisAccidentHurtTypeId__Fight;
        public static long HisAccidentHurtTypeId__Fight
        {
            get
            {
                if (hisAccidentHurtTypeId__Fight <= 0)
                {
                    hisAccidentHurtTypeId__Fight = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.HIS_ACCIDENT_HURT_TYPE_CODE__FIGHT));
                }
                return hisAccidentHurtTypeId__Fight;
            }
        }

        private static long hisAccidentHurtTypeId__Labor;
        public static long HisAccidentHurtTypeId__Labor
        {
            get
            {
                if (hisAccidentHurtTypeId__Labor <= 0)
                {
                    hisAccidentHurtTypeId__Labor = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.HIS_ACCIDENT_HURT_TYPE_CODE__LABOR));
                }
                return hisAccidentHurtTypeId__Labor;
            }
        }

        private static long hisAccidentHurtTypeId__Life;
        public static long HisAccidentHurtTypeId__Life
        {
            get
            {
                if (hisAccidentHurtTypeId__Life <= 0)
                {
                    hisAccidentHurtTypeId__Life = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.HIS_ACCIDENT_HURT_TYPE_CODE__LIFE));
                }
                return hisAccidentHurtTypeId__Life;
            }
        }

        private static long hisAccidentHurtTypeId__Other;
        public static long HisAccidentHurtTypeId__Other
        {
            get
            {
                if (hisAccidentHurtTypeId__Other <= 0)
                {
                    hisAccidentHurtTypeId__Other = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.HIS_ACCIDENT_HURT_TYPE_CODE__OTHER));
                }
                return hisAccidentHurtTypeId__Other;
            }
        }

        private static long hisAccidentHurtTypeId__Poisoning;
        public static long HisAccidentHurtTypeId__Poisoning
        {
            get
            {
                if (hisAccidentHurtTypeId__Poisoning <= 0)
                {
                    hisAccidentHurtTypeId__Poisoning = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.HIS_ACCIDENT_HURT_TYPE_CODE__POISONING));
                }
                return hisAccidentHurtTypeId__Poisoning;
            }
        }

        private static long hisAccidentHurtTypeId__Suicidal;
        public static long HisAccidentHurtTypeId__Suicidal
        {
            get
            {
                if (hisAccidentHurtTypeId__Suicidal <= 0)
                {
                    hisAccidentHurtTypeId__Suicidal = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.HIS_ACCIDENT_HURT_TYPE_CODE__SUICIDAL));
                }
                return hisAccidentHurtTypeId__Suicidal;
            }
        }

        private static long hisAccidentHurtTypeId__Traffic;
        public static long HisAccidentHurtTypeId__Traffic
        {
            get
            {
                if (hisAccidentHurtTypeId__Traffic <= 0)
                {
                    hisAccidentHurtTypeId__Traffic = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.HIS_ACCIDENT_HURT_TYPE_CODE__TRAFFIC));
                }
                return hisAccidentHurtTypeId__Traffic;
            }
        }

        private static long hisAccidentHurtTypeId__Underwater;
        public static long HisAccidentHurtTypeId__Underwater
        {
            get
            {
                if (hisAccidentHurtTypeId__Underwater <= 0)
                {
                    hisAccidentHurtTypeId__Underwater = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.HIS_ACCIDENT_HURT_TYPE_CODE__UNDERWATER));
                }
                return hisAccidentHurtTypeId__Underwater;
            }
        }

        private static long hisAccidentHurtTypeId__Violence;
        public static long HisAccidentHurtTypeId__Violence
        {
            get
            {
                if (hisAccidentHurtTypeId__Violence <= 0)
                {
                    hisAccidentHurtTypeId__Violence = GetId(SdaConfigs.Get<string>(ExtensionConfigKey.HIS_ACCIDENT_HURT_TYPE_CODE__VIOLENCE));
                }
                return hisAccidentHurtTypeId__Violence;
            }
        }

        private static long GetId(string accidentHurtTypeCode)
        {
            long result = 0;
            try
            {
                var heinServiceType = BackendDataWorker.Get<HIS_ACCIDENT_HURT_TYPE>().FirstOrDefault(o => o.ACCIDENT_HURT_TYPE_CODE == accidentHurtTypeCode);
                if (heinServiceType != null)
                {
                    result = heinServiceType.ID;
                }
                if (result == 0) throw new NullReferenceException(accidentHurtTypeCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }
    }
}
