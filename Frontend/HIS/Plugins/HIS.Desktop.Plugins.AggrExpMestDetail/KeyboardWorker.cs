﻿using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AggrExpMestDetail
{
    [KeyboardAction("Search", "HIS.Desktop.Plugins.AggrExpMestDetail.AggrExpMestDetail.UCMedcineTypeList", "Search", KeyStroke = XKeys.Control | XKeys.F)]
    [KeyboardAction("Refesh", "HIS.Desktop.Plugins.AggrExpMestDetail.AggrExpMestDetail.UCMedcineTypeList", "Refesh", KeyStroke = XKeys.Control | XKeys.R)]
    [KeyboardAction("Save", "HIS.Desktop.Plugins.AggrExpMestDetail.MecicineTypeUpdate.frmMedicineTypeUpdate", "Save", KeyStroke = XKeys.Control | XKeys.S)]
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
