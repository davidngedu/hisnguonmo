﻿using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisServSegr
{
    [KeyboardAction("FindShortcut1", "HIS.Desktop.Plugins.HisServSegr.ucServSegr", "FindShortcut1", KeyStroke = XKeys.Control | XKeys.F)]
    [KeyboardAction("FindShortcut2", "HIS.Desktop.Plugins.HisServSegr.ucServSegr", "FindShortcut2", KeyStroke = XKeys.Control | XKeys.D)]
    [KeyboardAction("SaveShortcut", "HIS.Desktop.Plugins.HisServSegr.ucServSegr", "SaveShortcut", KeyStroke = XKeys.Control | XKeys.S)]
    [KeyboardAction("EditServiceGroup", "HIS.Desktop.Plugins.HisServSegr.ucServSegr", "EditServiceGroup", KeyStroke = XKeys.Control | XKeys.E)]
    [KeyboardAction("SaveServiceGroup", "HIS.Desktop.Plugins.HisServSegr.ucServSegr", "SaveServiceGroup", KeyStroke = XKeys.Control | XKeys.N)]
    [KeyboardAction("New", "HIS.Desktop.Plugins.HisServSegr.ucServSegr", "New", KeyStroke = XKeys.Control | XKeys.R)]
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
