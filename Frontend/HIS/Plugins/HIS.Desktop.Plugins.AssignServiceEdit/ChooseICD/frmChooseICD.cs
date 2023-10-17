﻿using HIS.Desktop.Common;
using HIS.Desktop.Plugins.AssignServiceEdit.ADO;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignServiceEdit.ChooseICD
{
    public partial class frmChooseICD : Form
    {

        private List<HIS_ICD> icds { get; set; }
        private List<ICDADO> icdADOs { get; set; }
        private DelegateSelectData refeshDataChooseICD { get; set; }

        public frmChooseICD(List<HIS_ICD> _icds, DelegateSelectData _refeshDataChooseICD)
        {
            InitializeComponent();
            try
            {
                this.icds = _icds;
                this.refeshDataChooseICD = _refeshDataChooseICD;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChooseICD_Load(object sender, EventArgs e)
        {
            try
            {
                LoadIcdToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdToGrid()
        {
            try
            {
                icdADOs = new List<ICDADO>();
                if (icds != null && icds.Count > 0)
                {
                    foreach (var item in icds)
                    {
                        ICDADO icdADO = new ICDADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<ICDADO>(icdADO,item);
                        icdADOs.Add(icdADO);
                    }
                }
                gridControlICD.DataSource = icdADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEditChooseService_Click(object sender, EventArgs e)
        {
            try
            {
                var icd  = (ICDADO)gridViewICD.GetFocusedRow();
                foreach (var item in icdADOs)
                {
                    if (icd.ID == item.ID)
                    {

                        item.Check = true;
                    }
                    else
                    {
                        item.Check = false;
                    }
                }
                gridControlICD.RefreshDataSource();
                gridViewICD.LayoutChanged();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                List<ICDADO> sereServADOs = gridViewICD.DataSource as List<ICDADO>;
                if (icdADOs == null || icdADOs.Count == 0)
                    return;
                ICDADO icdCheck = null ;
                foreach (var item in icdADOs)
                {
                    if (item.Check)
                    {
                        icdCheck = item;
                        break;
                    }
                }

                if (icdCheck == null)
                {
                    MessageBox.Show("Vui lòng chọn 1 icd!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (refeshDataChooseICD != null)
                        refeshDataChooseICD(icdCheck);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
