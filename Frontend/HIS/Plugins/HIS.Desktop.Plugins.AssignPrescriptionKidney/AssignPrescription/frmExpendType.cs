﻿using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmExpendType : Form
    {
        DelegateSelectData ReloadData;
        public frmExpendType(DelegateSelectData ReloadData)
        {
            InitializeComponent();
            this.ReloadData = ReloadData;
        }

        private void frmExpendType_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                InitComboExpendType();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguagefrmAssignPrescription = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription.frmExpendType).Assembly);

                this.Text = Inventec.Common.Resource.Get.Value("frmExpendType.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmExpendType.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmExpendType.btnSave.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        //private void InitComboExpendType()
        //{
        //    try
        //    {
        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("EXPEND_TYPE_CODE", "", 100, 1));
        //        columnInfos.Add(new ColumnInfo("EXPEND_TYPE_NAME", "", 250, 2));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("EXPEND_TYPE_NAME", "ID", columnInfos, false, 350);
        //        ControlEditorLoader.Load(cboExpendType, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(), controlEditorADO);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ReloadData != null && cboExpendType.EditValue != null)
                {
                    this.ReloadData(cboExpendType.EditValue);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
