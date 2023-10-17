﻿using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    public partial class frmMultiIntructonTime : Form
    {
        DelegateSelectMultiDate delegateSelectData;
        List<DateTime?> oldDatas;
        DateTime timeSelested;
        public frmMultiIntructonTime()
        {
            InitializeComponent();
            this.SetCaptionByLanguageKey();
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

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
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
                this.Text = Inventec.Common.Resource.Get.Value("frmAssignPrescription.lciTimeAssign.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                lblTimeInput.Text = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionTimeInput", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                lblCalendaInput.Text = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionCalendaInput", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                btnChoose.Text = Inventec.Common.Resource.Get.Value("FormMultiChooseDate__CaptionBtnChoose", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmMultiIntructonTime1_Load(object sender, EventArgs e)
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
                    this.timeSelested = answer;
                    if (delegateSelectData != null)
                        delegateSelectData(listSelected, this.timeSelested);
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
