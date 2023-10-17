﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.LisBacAntibiotic
{
    class CallModule
    {
        internal const string HisImportServiceRoom = "HIS.Desktop.Plugins.HisImportServiceRoom";

        public CallModule(string _moduleLink, long _roomId, long _roomTypeId, List<object> _listObj)
        {
            CallModuleProcess(_moduleLink, _roomId, _roomTypeId, _listObj);
        }

        private void CallModuleProcess(string _moduleLink, long _roomId, long _roomTypeId, List<object> _listObj)
        {
            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(_moduleLink, _roomId, _roomTypeId, _listObj);
        }
    }
}
