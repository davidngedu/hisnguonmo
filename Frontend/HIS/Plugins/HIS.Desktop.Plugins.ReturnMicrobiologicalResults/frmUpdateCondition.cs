﻿using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.ReturnMicrobiologicalResults.Validation;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using LIS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ReturnMicrobiologicalResults
{
    public partial class frmUpdateCondition : FormBase
    {
        private int positionHandle = -1;

        private V_LIS_SAMPLE sample;
        List<LIS_SAMPLE_CONDITION> sampleConditions = new List<LIS_SAMPLE_CONDITION>();
        public frmUpdateCondition(Inventec.Desktop.Common.Modules.Module module, V_LIS_SAMPLE data)
            : base(module)
        {
            InitializeComponent();
            this.sample = data;
        }

        private void frmUpdateCondition_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.ValidControl();
                this.LoadComboCondition();
                lblBarcode.Text = sample.BARCODE ?? "";
                lblPatientCode.Text = sample.PATIENT_CODE ?? "";
                lblPatientName.Text = (sample.LAST_NAME ?? "") + " " + (sample.FIRST_NAME ?? "");
                lblServiceReqCode.Text = sample.SERVICE_REQ_CODE ?? "";
                lblTreatmentCode.Text = sample.TREATMENT_CODE ?? "";
                cboSampleCondition.EditValue = sample.SAMPLE_CONDITION_ID;
                WaitingManager.Hide();
                this.KeyPreview = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboCondition()
        {
            try
            {

                LisSampleConditionFilter filter = new LisSampleConditionFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.LIS_RS.COMMON.IS_ACTIVE__TRUE;
                this.sampleConditions = new BackendAdapter(new CommonParam()).Get<List<LIS_SAMPLE_CONDITION>>("api/LisSampleCondition/Get", ApiConsumers.LisConsumer, filter, null);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SAMPLE_CONDITION_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SAMPLE_CONDITION_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SAMPLE_CONDITION_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboSampleCondition, this.sampleConditions, controlEditorADO);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ConditionValidationRule rule = new ConditionValidationRule();
                rule.txtSampleConditionCode = txtSampleConditionCode;
                rule.cboSampleCondition = cboSampleCondition;
                dxValidationProvider1.SetValidationRule(txtSampleConditionCode, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSampleConditionCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtSampleConditionCode.Text))
                    {
                        string txt = txtSampleConditionCode.Text.ToLower().Trim();
                        var listData = this.sampleConditions != null ? this.sampleConditions.Where(o => o.SAMPLE_CONDITION_CODE.ToLower().Contains(txt)).ToList() : null;
                        if (listData != null && listData.Count == 1)
                        {
                            cboSampleCondition.EditValue = listData[0].ID;
                            SendKeys.Send("{TAB}");
                            SendKeys.Send("{TAB}");
                        }
                        else
                        {
                            cboSampleCondition.Focus();
                            cboSampleCondition.ShowPopup();
                        }
                    }
                    else
                    {
                        cboSampleCondition.Focus();
                        cboSampleCondition.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSampleCondition_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSampleCondition_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtSampleConditionCode.Text = "";
                if (cboSampleCondition.EditValue != null)
                {
                    var con = this.sampleConditions.FirstOrDefault(o => o.ID == Convert.ToInt64(cboSampleCondition.EditValue));
                    if (con != null)
                    {
                        txtSampleConditionCode.Text = con.SAMPLE_CONDITION_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!btnSave.Enabled || this.sample == null || !dxValidationProvider1.Validate()) return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                LisSampleConditionSDO sdo = new LisSampleConditionSDO();
                sdo.SampleId = this.sample.ID;
                sdo.SampleConditionId = Convert.ToInt64(cboSampleCondition.EditValue); ;

                LIS_SAMPLE rowBe = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Condition", ApiConsumers.LisConsumer, sdo, null);
                if (rowBe != null)
                {
                    success = true;
                    sample.SAMPLE_CONDITION_ID = rowBe.SAMPLE_CONDITION_ID;
                }
                WaitingManager.Hide();


                #region Show message
                MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion

                if (success)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



    }
}

