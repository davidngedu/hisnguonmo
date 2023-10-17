﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.SDO;
using HIS.Desktop.Plugins.HisNumOrderBlockChooser.ADO;
using DevExpress.XtraLayout;
using HIS.Desktop.ApplicationFont;

namespace HIS.Desktop.Plugins.HisNumOrderBlockChooser.Run
{
    public partial class UCTimes : UserControl
    {
        private List<HisNumOrderBlockSDO> ListNumOrder;
        private Action<TimeADO> SelectNumOrder;
        //private DevExpress.XtraEditors.SimpleButton LastButton;
        Base.Block LastButton;

        private int Dai = 30;
        private int Rong = 50;
        long? timeSelected;

        public UCTimes(List<MOS.SDO.HisNumOrderBlockSDO> list, Action<TimeADO> selectNumOrder)
        {
            // TODO: Complete member initialization
            InitializeComponent();
            this.ListNumOrder = list;
            this.SelectNumOrder = selectNumOrder;
        }
        public UCTimes(List<MOS.SDO.HisNumOrderBlockSDO> list, Action<TimeADO> selectNumOrder, long? timeSelected)
        {
            // TODO: Complete member initialization
            InitializeComponent();
            this.ListNumOrder = list;
            this.SelectNumOrder = selectNumOrder;
            this.timeSelected = timeSelected;
        }

        private void UCTimes_Load(object sender, EventArgs e)
        {
            try
            {

                if (ListNumOrder != null && ListNumOrder.Count > 0)
                {
                    float fontSize = HIS.Desktop.ApplicationFont.ApplicationFontWorker.GetFontSize();
                    if (fontSize != ApplicationFontConfig.FontSize825)
                    {
                        Dai += (int)((fontSize - ApplicationFontConfig.FontSize825) / 10);
                        Rong += (int)((fontSize - ApplicationFontConfig.FontSize825) / 10);
                    }

                    List<TimeADO> lstData = (from n in ListNumOrder select new TimeADO(n)).ToList();
                    var groupHour = lstData.GroupBy(o => o.HOUR).ToList();

                    groupHour = groupHour.OrderBy(o => o.Key).ToList();

                    for (int i = 0; i < groupHour.Count; i++)
                    {
                        List<TimeADO> times = groupHour[i].OrderBy(o => o.FROM_TIME).ToList();
                        for (int j = 0; j < times.Count; j++)
                        {
                            var blockTime = new Base.Block();
                            blockTime.Text = times[j].HOUR_STR;
                            var time = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
                            if (Convert.ToInt64(times[j].HOUR_STR.Replace(":", "")) < Convert.ToInt64(time))
                                blockTime.Enabled = false;

                            if (timeSelected != null && timeSelected != 0 && timeSelected == Convert.ToInt64(times[j].HOUR_STR.Replace(":", "")))
                            {
                                blockTime.Enabled = false;
                            }
                            blockTime.ForeColor = Color.Blue;
                            if (times[j].IS_ISSUED == 1)
                            {
                                blockTime.Enabled = false;
                                blockTime.ForeColor = new Color();
                            }

                            blockTime.Tag = times[j];
                            blockTime.TextAlign = ContentAlignment.MiddleCenter;
                            blockTime.GridX = i;
                            blockTime.GridY = j;
                            blockTime.Location = new System.Drawing.Point(blockTime.GridX * Rong + (blockTime.GridX > 0 ? (blockTime.GridX) * 5 : 0), blockTime.GridY * Dai + (blockTime.GridY > 0 ? (blockTime.GridY) * 5 : 0));
                            blockTime.Size = new System.Drawing.Size(Rong, Dai);
                            blockTime.TabIndex = 0;
                            blockTime.TabStop = false;
                            blockTime.Click += new EventHandler(button_Click);

                            this.xtraScrollableControl1.Controls.Add(blockTime);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            try
            {
                if (LastButton != null)
                {
                    LastButton.Font = new Font(LastButton.Font, FontStyle.Regular);
                }

                Base.Block button = sender as Base.Block;
                if (button != null)
                {
                    LastButton = button;

                    button.Font = new Font(button.Font, FontStyle.Bold);
                    if (button.Tag != null && this.SelectNumOrder != null)
                    {
                        TimeADO dataNum = button.Tag as TimeADO;
                        this.SelectNumOrder(dataNum);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
