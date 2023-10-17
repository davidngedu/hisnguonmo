﻿using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.CallPatientNumOrder.Config;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientNumOrder
{
    public partial class frmWaitingScreenNumOrder : HIS.Desktop.Utility.FormBase
    {
        int countTimer = 0;
        V_HIS_ROOM currentRoom;
        public frmWaitingScreenNumOrder(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_ROOM room)
            : base(moduleData)
        {
            InitializeComponent();
            this.currentRoom = room;
            this.Name = "WAITING_NUM_ORDER_" + this.currentRoom.ROOM_CODE;
            SetIcon();
            HisConfigCFG.LoadConfigs();
            this.SetBackground();
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

        private void frmWaitingScreenNumOrder_Load(object sender, EventArgs e)
        {
            try
            {
                LoadValueToControl();
                //RegisterTimer(ModuleLink, "timer", 1000, ChangeColor);
                timer.Interval = 1000;
                timer.Enabled = true;
                timer.Start();
                this.WindowState = FormWindowState.Maximized;
                this.BringToFront();
                this.TopMost = true;
                this.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBackground()
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(HisConfigCFG.BACKGROUND_IMAGE))
                {
                    this.layoutControlGroup1.BackgroundImage = Image.FromFile(ApplicationStoreLocation.ApplicationDirectory + HisConfigCFG.BACKGROUND_IMAGE);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadValueToControl()
        {
            try
            {
                lblOrganizationName.Text = BranchDataWorker.Branch.BRANCH_NAME.ToUpper();
                lblRegisterRoom.Text = ("VÀO " + this.currentRoom.ROOM_NAME).ToUpper();
                var organ_colors = HisConfigCFG.ORGANIZATION_NAME_COLOR_CODES;
                if (organ_colors != null && organ_colors.Count == 3)
                {
                    lblOrganizationName.ForeColor = System.Drawing.Color.FromArgb(organ_colors[0], organ_colors[1], organ_colors[2]);
                }
                var room_colors = HisConfigCFG.ROOM_NAM_COLOR_CODES;
                if (room_colors != null && room_colors.Count == 3)
                {
                    lblRegisterRoom.ForeColor = System.Drawing.Color.FromArgb(room_colors[0], room_colors[1], room_colors[2]);
                }
                if (HisConfigCFG.TITLE_SIZE > 0)
                {
                    lblOrganizationName.Appearance.Font = new System.Drawing.Font(new FontFamily("Arial"), HisConfigCFG.TITLE_SIZE, FontStyle.Bold);
                    lblInvitingTitle.Appearance.Font = new System.Drawing.Font(new FontFamily("Arial"), HisConfigCFG.TITLE_SIZE, FontStyle.Bold);
                    lblRegisterRoom.Appearance.Font = new System.Drawing.Font(new FontFamily("Arial"), HisConfigCFG.TITLE_SIZE, FontStyle.Bold);
                    int size = (int)(HisConfigCFG.TITLE_SIZE * 0.6);
                    lblProcessExam.Appearance.Font = new System.Drawing.Font(new FontFamily("Arial"), size, FontStyle.Bold);
                }

                if (HisConfigCFG.NUM_ORDER_SIZE > 0)
                {
                    lblNumOrder.Appearance.Font = new Font(new FontFamily("Arial"), HisConfigCFG.NUM_ORDER_SIZE, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetNumOrder(string num)
        {
            try
            {
                if (!string.IsNullOrEmpty(num))
                {
                    lblNumOrder.Invoke(new MethodInvoker(delegate
                    {
                        lblNumOrder.Text = num + "";
                        timer_Tick(null, null);
                    }));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                Task ts = Task.Factory.StartNew(timerForChangeColor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerForChangeColor()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { ChangeColor(); }));
                }
                else
                {
                    ChangeColor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangeColor()
        {
            try
            {
                countTimer++;
                if (countTimer == 1 || countTimer == 3 || countTimer == 5)
                {
                    lblNumOrder.ForeColor = System.Drawing.Color.FromArgb(40, 255, 40);
                    timer.Start();
                }
                if (countTimer == 0 || countTimer == 2 || countTimer == 4 || countTimer == 6)
                {
                    lblNumOrder.ForeColor = Color.Red;
                    timer.Start();
                }
                if (countTimer > 6)
                {
                    timer.Stop();
                    countTimer = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



    }
}
