﻿using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExecuteRoom
{
    [KeyboardAction("FindShortcut", "HIS.Desktop.Plugins.ExecuteRoom.UCExecuteRoom", "FindShortcut", KeyStroke = XKeys.Control | XKeys.F)]
    [KeyboardAction("ExecuteShortcut", "HIS.Desktop.Plugins.ExecuteRoom.UCExecuteRoom", "ExecuteShortcut", KeyStroke = XKeys.Control | XKeys.X)]
    [KeyboardAction("AssignServiceShortcut", "HIS.Desktop.Plugins.ExecuteRoom.UCExecuteRoom", "AssignServiceShortcut", KeyStroke = XKeys.Control | XKeys.D)]
    [KeyboardAction("AssignPrescriptionShortcut", "HIS.Desktop.Plugins.ExecuteRoom.UCExecuteRoom", "AssignPrescriptionShortcut", KeyStroke = XKeys.Control | XKeys.K)]
    [KeyboardAction("BordereauShortcut", "HIS.Desktop.Plugins.ExecuteRoom.UCExecuteRoom", "BordereauShortcut", KeyStroke = XKeys.F5)]
    [KeyboardAction("CallShorcut", "HIS.Desktop.Plugins.ExecuteRoom.UCExecuteRoom", "CallShorcut", KeyStroke = XKeys.F6)]
    [KeyboardAction("ReCallShorcut", "HIS.Desktop.Plugins.ExecuteRoom.UCExecuteRoom", "ReCallShorcut", KeyStroke = XKeys.F7)]
    [KeyboardAction("123321", "HIS.Desktop.Plugins.ExecuteRoom.UCExecuteRoom", "123321", KeyStroke = XKeys.F11)]
    [KeyboardAction("FindFocus", "HIS.Desktop.Plugins.ExecuteRoom.UCExecuteRoom", "FindFocus", KeyStroke = XKeys.F2)]
    [KeyboardAction("FocusControl", "HIS.Desktop.Plugins.ExecuteRoom.UCExecuteRoom", "FocusControl", KeyStroke = XKeys.F3)]
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
