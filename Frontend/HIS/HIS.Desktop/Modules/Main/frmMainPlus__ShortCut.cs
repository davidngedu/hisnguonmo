﻿using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraTab;
using HIS.Desktop.Base;
using Inventec.Common.Logging;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Common.Modules;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraBars;

namespace HIS.Desktop.Modules.Main
{
    public partial class frmMain : RibbonForm
    {
        /// <summary>
        /// Called to build menus and toolbars.  Override this method to customize menu and toolbar building.
        /// </summary>
        /// <remarks>
        /// The default implementation simply clears and re-creates the toolstrip using methods on the
        /// utility class <see cref="ToolStripBuilder"/>.
        /// </remarks>
        /// <param name="toolStrip"></param>
        /// <param name="actionModel"></param>
        public void BuildToolStrip(List<Module> actionModel)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("BuildToolStrip.Begin");
                // Toolstrip should only be visible if there are items on it
                List<DevExpress.XtraBars.BarItem> barItems = new List<BarItem>();
                barItems = new HIS.Desktop.Base.ToolStripBuilder().BuildToolStrip(actionModel, GetCurrentPage, GetCurrentModule);
                Inventec.Common.Logging.LogSystem.Debug("actionModel.Count=" + (actionModel != null ? actionModel.Count : 0));
                if (barItems != null && barItems.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("BuildToolStrip.barItems.Count=" + barItems.Count);
                    this.ribbonMain.Items.AddRange(barItems.ToArray());
                }

                Inventec.Common.Logging.LogSystem.Debug("BuildToolStrip.End");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        Control GetCurrentPage()
        {
            try
            {
                var controlInPages = tabControlMain.SelectedTabPage.Controls;
                if (controlInPages != null && controlInPages.Count > 0)
                    return controlInPages[0];
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return null;
        }

        Inventec.Desktop.Common.Modules.Module GetCurrentModule()
        {
            try
            {
                var module = (Inventec.Desktop.Common.Modules.Module)(tabControlMain.TabPages[tabControlMain.SelectedTabPageIndex].Tag);
                return module;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return null;
        }
    }
}