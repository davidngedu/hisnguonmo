﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core.Tools;

namespace HIS.Desktop.Plugins.HisExportChmsList
{
    [KeyboardAction("Search", "HIS.Desktop.Plugins.HisExportChmsList.UCHisExportChmsList", KeyStroke = XKeys.Control | XKeys.F)]
    [KeyboardAction("Refreshs", "HIS.Desktop.Plugins.HisExportChmsList.UCHisExportChmsList", KeyStroke = XKeys.Control | XKeys.R)]
    [KeyboardAction("FocusExpCode", "HIS.Desktop.Plugins.HisExportChmsList.UCHisExportChmsList", KeyStroke = XKeys.F2)]
    [KeyboardAction("FocusImpCode", "HIS.Desktop.Plugins.HisExportChmsList.UCHisExportChmsList", KeyStroke = XKeys.F3)]
    [KeyboardAction("Export", "HIS.Desktop.Plugins.HisExportChmsList.UCHisExportChmsList", KeyStroke = XKeys.Control | XKeys.E)]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    class KeyboardWorker:Tool<IDesktopToolContext>
    {
        public KeyboardWorker() : base() { }

        public override IActionSet Actions
        {
            get
            {
                return base.Actions;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
