﻿using Inventec.Core;
using Inventec.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Desktop.Common.Core;

namespace SDA.Desktop.Plugins.SdaExecuteSql.SdaExecuteSql
{
    class SdaExecuteSqlBehavior : BusinessBase, ISdaExecuteSql
    {
        object[] entity;
        internal SdaExecuteSqlBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ISdaExecuteSql.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                if (entity.GetType() == typeof(object[]))
                {
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                        }
                    }
                }
                return new frmSdaExecuteSql(moduleData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
