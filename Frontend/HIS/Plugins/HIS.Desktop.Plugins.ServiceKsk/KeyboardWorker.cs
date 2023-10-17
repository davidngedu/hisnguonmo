﻿using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceKsk
{
    [KeyboardAction("FindShortcut1", "HIS.Desktop.Plugins.ServiceKskt.UCServiceKsk", "FindShortcut1", KeyStroke = XKeys.Control | XKeys.F)]
    [KeyboardAction("FindShortcut2", "HIS.Desktop.Plugins.ServiceKsk.UCServiceKsk", "FindShortcut2", KeyStroke = XKeys.Control | XKeys.D)]
    [KeyboardAction("SaveShortcut", "HIS.Desktop.Plugins.ServiceKsk.UCServiceKsk", "SaveShortcut", KeyStroke = XKeys.Control | XKeys.S)]
    [KeyboardAction("SaveKsk", "HIS.Desktop.Plugins.ServiceKsk.UCServiceKsk", "SaveKsk", KeyStroke = XKeys.Control | XKeys.A)]
    [KeyboardAction("Refresh", "HIS.Desktop.Plugins.ServiceKsk.UCServiceKsk", "Refresh", KeyStroke = XKeys.Control | XKeys.R)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public sealed class KeyboardWorker : Tool<IDesktopToolContext>
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
