﻿using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExecuteRoleUser
{
    [KeyboardAction("FindShortcut1", "HIS.Desktop.Plugins.ExecuteRoleUser.UC_ExecuteRoleUser", "FindShortcut1", KeyStroke = XKeys.Control | XKeys.F)]
    [KeyboardAction("FindShortcut2", "HIS.Desktop.Plugins.ExecuteRoleUser.UC_ExecuteRoleUser", "FindShortcut2", KeyStroke = XKeys.Control | XKeys.D)]
    [KeyboardAction("SaveShortcut", "HIS.Desktop.Plugins.ExecuteRoleUser.UC_ExecuteRoleUser", "SaveShortcut", KeyStroke = XKeys.Control | XKeys.S)]
    [KeyboardAction("Import", "HIS.Desktop.Plugins.ExecuteRoleUser.UC_ExecuteRoleUser", "Import", KeyStroke = XKeys.Control | XKeys.I)]
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
