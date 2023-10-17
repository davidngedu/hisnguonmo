﻿using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXml.ExportXml
{
    class ExportXmlBehavior : Tool<IDesktopToolContext>, IExportXml
    {
        object[] entity;
        internal ExportXmlBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IExportXml.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
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
                if (WorkPlace.GetBranchId() > 0)
                {
                    return new UCExportXml(moduleData);
                }
                throw new NullReferenceException("BranchId<=0");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
