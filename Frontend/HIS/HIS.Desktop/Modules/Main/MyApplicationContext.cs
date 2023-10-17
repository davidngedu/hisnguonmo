﻿using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop
{
    class MyApplicationContext : ApplicationContext
    {      
        public MyApplicationContext()
        {
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            InitializeComponent();
        }

        void InitializeComponent()
        {
            try
            {
                LogSystem.Info("-----------------------MyApplicationContext => Start-----------------------");
                HIS.Desktop.Modules.Login.frmLogin frmLogin = new Modules.Login.frmLogin();
                frmLogin.ShowDialog();
                LogSystem.Info("-----------------------MyApplicationContext => End-----------------------");

                GlobalVariables.IsLostToken = true;
                GlobalVariables.isLogouter = true;
                //Application.Exit();
                //System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {               
                LogSystem.Error(ex);
            }
        }

        void OnApplicationExit(object sender, EventArgs e)
        {
            //Cleanup so that the icon will be removed when the application is closed
            try
            {
                LogSystem.Info("OnApplicationExit. Time=" + DateTime.Now.ToString("yyyyMMddhhmmss"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        
    }
}
