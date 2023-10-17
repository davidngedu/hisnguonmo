﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ApproveKskTreatment
{
    class CallModule
    {
        internal const string AggrExpMestDetail = "HIS.Desktop.Plugins.AggrExpMestDetail";
        internal const string ImpMestChmsCreate = "HIS.Desktop.Plugins.ImpMestChmsCreate";
        internal const string EventLog = "Inventec.Desktop.Plugins.EventLog";
        internal const string ExpMestViewDetail = "HIS.Desktop.Plugins.ExpMestViewDetail";
        internal const string ApproveAggrExpMest = "HIS.Desktop.Plugins.ApproveAggrExpMest";
        internal const string ExpMestOtherExport = "HIS.Desktop.Plugins.ExpMestOtherExport";
        internal const string ExpMestSaleCreate = "HIS.Desktop.Plugins.ExpMestSaleCreate";
        internal const string ExpMestChmsUpdate = "HIS.Desktop.Plugins.ExpMestChmsUpdate";
        internal const string ExpMestDepaUpdate = "HIS.Desktop.Plugins.ExpMestDepaUpdate";
        internal const string ManuExpMestCreate = "HIS.Desktop.Plugins.ManuExpMestCreate";
        internal const string BrowseExportTicket = "HIS.Desktop.Plugins.BrowseExportTicket";
        internal const string MobaBloodCreate = "HIS.Desktop.Plugins.MobaBloodCreate";
        internal const string AssignServiceTest = "HIS.Desktop.Plugins.AssignServiceTest";

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
