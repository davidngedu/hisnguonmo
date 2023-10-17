﻿using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MultiDate
{
    public partial class frmMultiIntructonTime : Form
    {
        DelegateSelectMultiDate delegateSelectData;
        List<DateTime?> oldDatas;
        DateTime timeSelested;
        public frmMultiIntructonTime()
            : this(null, DateTime.MinValue, null)
        {
        }
        public frmMultiIntructonTime(List<DateTime?> datas, DateTime time, DelegateSelectMultiDate selectData)
        {
            try
            {
                InitializeComponent();
                this.delegateSelectData = selectData;
                this.oldDatas = datas;
                this.timeSelested = time;
                this.SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageFormMultiIntructonTime = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignPrescriptionPK.MultiDate.frmMultiIntructonTime).Assembly);

                this.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime__CaptionText", Resources.ResourceLanguageManager.LanguageFormMultiIntructonTime, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciIntructionTime.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime__CaptionTimeInput", Resources.ResourceLanguageManager.LanguageFormMultiIntructonTime, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciIntructionDate.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime__CaptionCalendaInput", Resources.ResourceLanguageManager.LanguageFormMultiIntructonTime, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnChoose.Text = Inventec.Common.Resource.Get.Value("frmMultiIntructonTime__CaptionBtnChoose", Resources.ResourceLanguageManager.LanguageFormMultiIntructonTime, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmMultiIntructonTime_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.oldDatas != null && this.oldDatas.Count > 0)
                {
                    //Add datepcker with data
                    foreach (var item in this.oldDatas)
                    {
                        if (item != null && item.Value != DateTime.MinValue)
                        {
                            calendarIntructionTime.AddSelection(item.Value);
                        }
                    }
                }
                if (this.timeSelested != DateTime.MinValue)
                {
                    timeIntruction.EditValue = this.timeSelested.ToString("HH:mm");
                }
                else
                {
                    timeIntruction.EditValue = DateTime.Now.ToString("HH:mm");
                }
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
                WaitingManager.Show();
                bool isSelected = false;
                List<DateTime?> listSelected = new List<DateTime?>();
                foreach (DateRange item in calendarIntructionTime.SelectedRanges)
                {
                    if (item != null)
                    {
                        var dt = item.StartDate;
                        while (dt.Date < item.EndDate.Date)
                        {
                            isSelected = true;
                            listSelected.Add(dt);
                            dt = dt.AddDays(1);
                        }
                    }
                }
                WaitingManager.Hide();
                if (isSelected)
                {
                    System.DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 2);
                    System.DateTime answer = today.Add(timeIntruction.TimeSpan);
                    timeSelested = answer;
                    delegateSelectData(listSelected, timeSelested);
                    this.Close();
                }
                else
                {
                    MessageManager.Show(ResourceMessage.ChuaChonNgayChiDinh);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
