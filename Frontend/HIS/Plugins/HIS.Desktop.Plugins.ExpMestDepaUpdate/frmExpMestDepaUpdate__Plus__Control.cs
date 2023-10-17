﻿using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestDepaUpdate
{
    public partial class frmExpMestDepaUpdate : HIS.Desktop.Utility.FormBase
    {
        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (xtraTabControlMain.SelectedTabPageIndex == 0)
                    {
                        this.mediInStockProcessor.FocusKeyword(this.ucMediInStock);
                    }
                    else
                    {
                        this.mediInStockProcessor.FocusKeyword(this.ucMediInStock);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinExpAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNote_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void xtraTabControlMain_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                try
                {
                    if (xtraTabControlMain.SelectedTabPageIndex == 0)
                    {
                        this.mediInStockProcessor.FocusKeyword(this.ucMediInStock);
                        this.lciRemedyCount.Enabled = !(dicMediMateAdo.Select(o => o.Value).ToList().Exists(o => !o.IsMedicine));
                    }
                    else if (xtraTabControlMain.SelectedTabPageIndex == 1)
                    {
                        this.mediInStockProcessor.FocusKeyword(this.ucMediInStock);
                        this.lciRemedyCount.Enabled = false;
                        this.spinRemedyCount.EditValue = null;
                    }
                    else if (xtraTabControlMain.SelectedTabPageIndex == 2)
                    {
                        //Review
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioImport_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDataToCboMediStock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkLinh_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                LoadDataToCboMediStock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void chkTra_CheckedChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    if (chkTra.Checked)
            //    {
            //        chkLinh.Checked = false;
            //    }
            //    else
            //    {
            //        chkLinh.Checked = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}

        }

        private void barThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnAdd();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
