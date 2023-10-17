﻿using Inventec.Core;
using Inventec.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Desktop.Core.Tools;
using Inventec.Desktop.Core;


namespace SDA.Desktop.Plugins.SdaHideControl
{
    class SdaHideControlBehavior : Tool<IDesktopToolContext>, ISdaHideControl
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        internal SdaHideControlBehavior()
            : base()
        {

        }

        internal SdaHideControlBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object ISdaHideControl.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;

                List<Inventec.Common.WebApiClient.ApiConsumer> listApiConsumer = null;
                List<string> listString = null;
                Dictionary<string, string> dicBranch = new Dictionary<string, string>();
                string BranchCode = "";
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        if (entity[i] is List<Inventec.Common.WebApiClient.ApiConsumer>)
                        {
                            listApiConsumer = (List<Inventec.Common.WebApiClient.ApiConsumer>)entity[i];
                        }
                        if (entity[i] is List<string>)
                        {
                            listString = (List<string>)entity[i];
                        }
                        if (entity[i] is string)
                        {
                            BranchCode = (string)entity[i];
                        }
                        if (entity[i] is Dictionary<string, string>)
                        {
                            dicBranch = (Dictionary<string, string>)entity[i];
                        }
                    }
                }
                result = new frmSdaHideControl(moduleData, listApiConsumer[0], listApiConsumer[1], dicBranch, listString[0]);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return result;
        }
    }
}
