﻿using HIS.Desktop.LocalStorage.BackendData;
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

namespace HIS.Desktop.Plugins.CallPatientCashierTwo
{
    public partial class frmConfigForm : HIS.Desktop.Utility.FormBase
    {
        public frmConfigForm(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        private void frmConfigForm_Load(object sender, EventArgs e)
        {
            try
            {
                V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModuleBase.RoomId);
                string fname = "WAITING_NUM_ORDER_" + room.ROOM_CODE;
                bool isOpen = false;
                if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                {
                    for (int i = 0; i < Application.OpenForms.Count; i++)
                    {
                        Form f = Application.OpenForms[i];
                        if (f.Name == fname)
                        {
                            isOpen = true;
                        }
                    }
                }
                if (!isOpen)
                {
                    frmWaitingScreenCashierTwo frm = new frmWaitingScreenCashierTwo(this.currentModuleBase, room);
                    ShowFormProcessor.ShowFormInExtendMonitor(frm);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
