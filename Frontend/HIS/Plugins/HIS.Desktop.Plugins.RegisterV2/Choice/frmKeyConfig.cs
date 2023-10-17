﻿using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.RegisterV2.ADO;
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

namespace HIS.Desktop.Plugins.RegisterV2.Choice
{
    public partial class frmKeyConfig : Form
    {
        Action<string> textKey;
        private Dictionary<string, string> dic = new Dictionary<string, string>() {
            { "<#NUM_ORDER_STR;>", "Số thứ tự tiếp đón(NUM_ORDER) chuyển từ dạng số sang dạng chữ và đọc từng chữ" },
            { "<#NUM_ORDER;>", "Số thứ tự tiếp đón(NUM_ORDER) đọc từng số." },
            { "<#GATE_NAME;>", "Tên cổng" },
            { "<#REGISTER_GATE_CODE;>", "Mã dãy" },
            { "<#REGISTER_GATE_NAME;>", "Tên dãy" }
        };
        public frmKeyConfig(Action<string> textKey)
        {
            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            try
            {
                this.textKey = textKey;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmKeyConfig_Load(object sender, EventArgs e)
        {
            try
            {
                KeyADO ado = new KeyADO();
                ado.lstKeyADO = new List<KeyADO>();
                foreach (var item in dic)
                {
                    ado.lstKeyADO.Add(new KeyADO() { Key = item.Key, Details = item.Value });

                }
                gridControl1.DataSource = null;
                gridControl1.DataSource = ado.lstKeyADO;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var ado = (KeyADO)this.gridView1.GetFocusedRow();
                if (ado != null)
                {
                    textKey(ado.Key);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
