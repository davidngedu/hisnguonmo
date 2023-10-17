﻿using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ManuImpMestUpdate.Run
{
    public partial class frmImpMestUpdate : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long _impMestId;

        HIS.Desktop.Plugins.ImpMestCreate.UCImpMestCreate _UCImpMestUpdate;

        public frmImpMestUpdate()
        {
            InitializeComponent();
        }

        public frmImpMestUpdate(Inventec.Desktop.Common.Modules.Module currentModule, long _impMestId)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this._impMestId = _impMestId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmImpMestUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                SetIcon();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ImpMestCreate").FirstOrDefault();
                _UCImpMestUpdate = new ImpMestCreate.UCImpMestCreate(moduleData, currentModule.RoomTypeId, currentModule.RoomId, this._impMestId);
                this.Controls.Add(_UCImpMestUpdate);
                _UCImpMestUpdate.Dock = DockStyle.Fill;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                _UCImpMestUpdate.BtnSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__CtrlU_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                _UCImpMestUpdate.BtnUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_CtrlR_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                _UCImpMestUpdate.BtnCancel();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__CtrlA_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                _UCImpMestUpdate.BtnAdd();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__CtrlP_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                _UCImpMestUpdate.BtnPrint();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnFocusSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            _UCImpMestUpdate.FocusSearchPanel();
        }


    }
}
