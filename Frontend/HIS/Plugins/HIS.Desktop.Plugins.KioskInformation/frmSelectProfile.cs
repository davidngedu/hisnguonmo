﻿using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Modules;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.KioskInformation
{
    public partial class frmSelectProfile : FormBase
    {
        private List<KioskInformationSDO> _lstKioskInform = new List<KioskInformationSDO>();

        private Module _currentModule = new Module();

        private List<Image> _lstImage;

        private Image _currentImage;

        private KioskInformationSDO _currentData = new KioskInformationSDO();

        private int _coutWallpaper = 0;

        private int countOfGrid = 0;

        public frmSelectProfile(Module currentModule, List<KioskInformationSDO> lstKioskInform, List<Image> lstImage, int coutWallpaper)
            : base(currentModule)
        {
            this.InitializeComponent();
            this._currentModule = currentModule;
            this._lstImage = lstImage;
            this._lstKioskInform = lstKioskInform;
            this._coutWallpaper = coutWallpaper;
        }

        private void frmSelectProfile_Load(object sender, EventArgs e)
        {
            try
            {
                this.gridControlSelectProfile.DataSource = this._lstKioskInform;
                this.timerWallpaperSelectForm.Interval = 1000;
                this.timerWallpaperSelectForm.Start();
                this._currentImage = this._lstImage[this._coutWallpaper / 10];
                this.timerOffGrid.Interval = 1000;
                this.timerOffGrid.Start();
            }
            catch (Exception ex)
            {
                this.timerWallpaperSelectForm.Stop();
                this.timerOffGrid.Stop();
                LogSystem.Warn(ex);
            }
        }

        private void gridViewSelectProfile_Click(object sender, EventArgs e)
        {
            try
            {
                KioskInformationSDO kioskInformationSDO = (KioskInformationSDO)this.gridViewSelectProfile.GetFocusedRow();
                if (kioskInformationSDO != null)
                {
                    this._currentData = kioskInformationSDO;
                    base.Hide();
                    frmGetInforationScreen frmGetInforationScreen = new frmGetInforationScreen(this._currentModule, this._currentData, this._currentImage);
                    frmGetInforationScreen.ShowDialog();
                    base.Show();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewSelectProfile_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    KioskInformationSDO kioskInformationSDO = (KioskInformationSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (kioskInformationSDO != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        if (e.Column.FieldName == "DobStr")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(kioskInformationSDO.Dob);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void timerWallpaperSelectForm_Tick(object sender, EventArgs e)
        {
            if (this.timerWallpaperSelectForm.Enabled)
            {
                this._coutWallpaper++;
                if (this._coutWallpaper % 10 == 0)
                {
                    if (this._lstImage != null && this._lstImage.Count > 0)
                    {
                        if (this._coutWallpaper / 10 == this._lstImage.Count)
                        {
                            this._coutWallpaper = 0;
                        }
                        this._currentImage = this._lstImage[this._coutWallpaper / 10];
                    }
                }
            }
        }

        private void timerOffGrid_Tick(object sender, EventArgs e)
        {
            if (this.timerOffGrid.Enabled)
            {
                this.countOfGrid++;
                if (this.countOfGrid == 60)
                {
                    base.Hide();
                }
            }
        }
    }
}
