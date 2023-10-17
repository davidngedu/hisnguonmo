﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using HIS.Desktop.Common;

namespace LIS.Desktop.Plugins.LisSampleType.LisSampleType
{
    public sealed class LisSampleTypeBehavior:Tool<IDesktopToolContext>,ILisSampleType
    {
        object[] entity;

        public LisSampleTypeBehavior() 
            :base() 
        { 
        
        }

        public LisSampleTypeBehavior(CommonParam param, object[] filter) 
            :base() 
        {
            this.entity = filter;
        }

        object ILisSampleType.Run() {
            try { 
                Inventec.Desktop.Common.Modules.Module moduleData = null;

                if ( entity.GetType() == typeof(object[]) ){
                    if (entity != null && entity.Count() > 0) { 
                        for(int i = 0; i < entity.Count(); i++){
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                        }
                    }
                }

                return new frmLisSampleType(moduleData);
            }
            catch (Exception ex) {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }
    }
}
