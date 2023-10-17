﻿using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Remuneration
{
    [KeyboardAction("FindShortcut1", "HIS.Desktop.Plugins.Remuneration.UC_Remuneration", "FindShortcut1", KeyStroke = XKeys.Control | XKeys.F)]
    [KeyboardAction("FindShortcut2", "HIS.Desktop.Plugins.Remuneration.UC_Remuneration", "FindShortcut2", KeyStroke = XKeys.Control | XKeys.D)]
    [KeyboardAction("SaveShortcut", "HIS.Desktop.Plugins.Remuneration.UC_Remuneration", "SaveShortcut", KeyStroke = XKeys.Control | XKeys.S)]
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
