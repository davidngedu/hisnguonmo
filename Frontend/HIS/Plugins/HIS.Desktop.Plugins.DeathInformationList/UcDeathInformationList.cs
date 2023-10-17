﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.Filter;
using VSK.Filter;
using VSK.EFMODEL;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors;
using MOS.SDO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using MOS.KskSignData;
using HIS.Desktop.LocalStorage.BackendData;
using System.Resources;
using HIS.Desktop.Plugins.DeathInformationList.Resources;
using HIS.Desktop.LocalStorage.HisConfig;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Inventec.Common.SignLibrary.ServiceSign;
using EMR.WCF.DCO;
using Newtonsoft.Json;

namespace HIS.Desktop.Plugins.DeathInformationList
{
    public partial class UcDeathInformationList : UserControlBase
    {

        #region Derlare
        string datetime;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;
        List<HIS_DEPARTMENT> departmentSelecteds;
        Inventec.Desktop.Common.Modules.Module currentModule { get; set; }
        int lastRowHandle = -1;
        private bool isNotLoadWhileChangeControlStateInFirst;
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private X509Certificate2 certificate;
        private string SerialNumber;
        private List<HIS_BRANCH> lstBranch { get; set; }
        #endregion
        #region ConstructorLoad
        public UcDeathInformationList(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            this.currentModule = module;
        }
        private void UcDeathInformationList_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetDefaultControl();
                InitComboDepartmentCheck();
                InitComboDepartment();
                InitControlState();
                FillDataToGrid();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UcDeathInformationList
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.DeathInformationList.Resources.Lang", typeof(UcDeathInformationList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UcDeathInformationList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UcDeathInformationList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.ToolTip = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn15.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("UcDeathInformationList.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRefresh.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.bbtnRefresh.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UcDeathInformationList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSignFileCertUtil.Properties.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.chkSignFileCertUtil.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDongBo.Text = Inventec.Common.Resource.Get.Value("UcDeathInformationList.btnDongBo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDongBo.ToolTip = Inventec.Common.Resource.Get.Value("UcDeathInformationList.btnDongBo.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UcDeathInformationList.txtPatientCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UcDeathInformationList.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UcDeathInformationList.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarControl1.Text = Inventec.Common.Resource.Get.Value("UcDeathInformationList.navBarControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroup1.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.navBarGroup1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UcDeathInformationList.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UcDeathInformationList.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("UcDeathInformationList.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UcDeathInformationList.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("UcDeathInformationList.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("UcDeathInformationList.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroup2.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.navBarGroup2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroup3.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.navBarGroup3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatientName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UcDeathInformationList.txtPatientName.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UcDeathInformationList.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSYNC_RESULT_TYPE.Properties.Items.AddRange(new object[] {
            Inventec.Common.Resource.Get.Value("UcDeathInformationList.cboSYNC_RESULT_TYPE.Properties.1", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()),
            Inventec.Common.Resource.Get.Value("UcDeathInformationList.cboSYNC_RESULT_TYPE.Properties.2", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()),
            Inventec.Common.Resource.Get.Value("UcDeathInformationList.cboSYNC_RESULT_TYPE.Properties.3", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()),
            Inventec.Common.Resource.Get.Value("UcDeathInformationList.cboSYNC_RESULT_TYPE.Properties.4", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())});
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
        #region Private Method
        public void Search()
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Refesh()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadGridData(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadGridData, param, pageSize, gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        private void LoadGridData(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<V_HIS_TREATMENT>> apiResult = null;
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                SetFilter(ref filter);
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControl1.DataSource = data;
                    }
                    else
                    {
                        gridControl1.DataSource = null;

                    }
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControl1.DataSource = null;
                }
                gridView1.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else return false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }
        private void SetFilter(ref HisTreatmentViewFilter filter)
        {
            try
            {
                
                    string TreatmentCode = txtTreatmentCode.Text.Trim();
                    if ( !string.IsNullOrEmpty(TreatmentCode) && TreatmentCode.Length < 10 && checkDigit(TreatmentCode))
                    {
                        TreatmentCode = string.Format("{0:000000000000}", Convert.ToInt64(TreatmentCode));
                        txtTreatmentCode.Text = TreatmentCode;
                    }
                    filter.TREATMENT_CODE__EXACT = txtTreatmentCode.Text;

                    string Patientcode = txtPatientCode.Text.Trim();
                    if (!string.IsNullOrEmpty(Patientcode) && Patientcode.Length < 10 && checkDigit(Patientcode))
                    {
                        Patientcode = string.Format("{0:0000000000}", Convert.ToInt64(Patientcode));
                        txtPatientCode.Text = Patientcode;
                    }
                    filter.PATIENT_CODE__EXACT = txtPatientCode.Text;           
                    filter.PATIENT_NAME = txtPatientName.Text.Trim();      
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                        filter.DEATH_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                        filter.DEATH_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                if (dtDeathIssuedDateFrom.EditValue != null && dtDeathIssuedDateFrom.DateTime != DateTime.MinValue)
                    filter.DEATH_ISSUED_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtDeathIssuedDateFrom.EditValue).ToString("yyyyMMdd") + "000000");
                if (dtDeathIssuedDateTo.EditValue != null && dtDeathIssuedDateTo.DateTime != DateTime.MinValue)
                    filter.DEATH_ISSUED_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtDeathIssuedDateTo.EditValue).ToString("yyyyMMdd") + "235959");

                if (cboSYNC_RESULT_TYPE.EditValue != null)
                    {
                        switch (cboSYNC_RESULT_TYPE.SelectedIndex)
                        {
                            case 0:
                                filter.DEATH_SYNC_RESULT_TYPE = null;
                                break;
                            case 1:
                                filter.DEATH_SYNC_RESULT_TYPE = 1;
                                break;
                            case 2:
                                filter.DEATH_SYNC_RESULT_TYPE = 2;
                                break;
                            case 3:
                                filter.DEATH_SYNC_RESULT_TYPE = 3;
                                break;
                            default:
                                filter.DEATH_SYNC_RESULT_TYPE = null;
                                break;
                        }
                    }
                    if (this.departmentSelecteds != null && this.departmentSelecteds.Count() > 0)
                    {
                        filter.END_DEPARTMENT_IDs = this.departmentSelecteds.Select(o => o.ID).Distinct().ToList();
                    }
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDefaultControl()
        {
            try
            {
                lstBranch = BackendDataWorker.Get<HIS_BRANCH>().ToList();
                dtCreateTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.StartMonth() ?? 0));
                dtCreateTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.EndDay() ?? 0));
                dtDeathIssuedDateFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.StartMonth() ?? 0));
                dtDeathIssuedDateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.EndDay() ?? 0));
                txtPatientName.Text = "";
                txtPatientCode.Text = "";
                txtTreatmentCode.Text = "";
                txtPatientName.Text = "";
                cboSYNC_RESULT_TYPE.SelectedIndex = 0;
                btnDongBo.Enabled = false;
                GridCheckMarksSelection gridCheckMark = cboDepartment.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboDepartment.Properties.View);
                }
                cboDepartment.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
       
        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.currentModule.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkSignFileCertUtil.Name)
                        {
                            SerialNumber = item.VALUE;

                            chkSignFileCertUtil.Checked = !String.IsNullOrWhiteSpace(SerialNumber);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }
        #endregion
        #region EvenGridView
        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (V_HIS_TREATMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                    {
                        try
                        {

                            string patientDob = (view.GetRowCellValue(e.ListSourceRowIndex, "TDL_PATIENT_DOB") ?? "").ToString();

                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = patientDob.Substring(0, 4);
                            }
                            else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.TypeConvert.Parse.ToInt64(patientDob));
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao TDL_PATIENT_DOB", ex);
                        }
                    }
                    else if (e.Column.FieldName == "DEATH_SYNC_RESULT_TYPE_STR")
                    {
                        try
                        {
                            string sysResult = (view.GetRowCellValue(e.ListSourceRowIndex, "DEATH_SYNC_RESULT_TYPE") ?? "").ToString();
                            if (!string.IsNullOrEmpty(sysResult))
                            {
                                if (sysResult == "1")
                                {
                                    e.Value = Inventec.Common.Resource.Get.Value("UcDeathInformationList.cboSYNC_RESULT_TYPE.Properties.2", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                                }
                                else if (sysResult == "2")
                                {
                                    e.Value = Inventec.Common.Resource.Get.Value("UcDeathInformationList.cboSYNC_RESULT_TYPE.Properties.3", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                                }
                                else if (sysResult == "3")
                                {
                                    e.Value = Inventec.Common.Resource.Get.Value("UcDeathInformationList.cboSYNC_RESULT_TYPE.Properties.4", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                                }
                                else
                                {
                                    e.Value = "";
                                }

                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao DEATH_SYNC_RESULT_TYPE_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "CERT_STR")
                    {
                        try
                        {
                            string value = null;
                            if (data.DEATH_CERT_NUM != null)
                            {
                                string certStr = data.DEATH_CERT_NUM.ToString();
                                if (certStr.Length < 5)
                                {
                                    certStr = string.Format("{0:00000}", data.DEATH_CERT_NUM);
                                }
                                value = certStr;
                            }
                            value += ".GBT";
                            if (data.BRANCH_ID > 0)
                                value += "." + lstBranch.FirstOrDefault(o => o.ID == data.BRANCH_ID).HEIN_MEDI_ORG_CODE;
                            if (data.DEATH_ISSUED_DATE != null)
                                value += "." + data.DEATH_ISSUED_DATE.ToString().Substring(2, 2);
                            e.Value = value;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CERT_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "DEATH_TIME_STR")
                    {
                        try
                        {
                            if (data.DEATH_TIME != null)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.DEATH_TIME ?? 0);
                            }
                            else
                            {
                                e.Value = "";
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao DEATH_TIME_STR", ex);

                        }
                    }
                    else if (e.Column.FieldName == "DEATH_ISSUED_DATE_STR")
                    {
                        try
                        {
                            if (data.DEATH_ISSUED_DATE != null)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DEATH_ISSUED_DATE ?? 0);
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay cap giay bao tu DEATH_ISSUED_DATE_STR", ex);
                        }                
                    }
                    else if (e.Column.FieldName == "DEATH_DOCUMENT_DATE_STR")
                    {
                        try
                        {
                            if (data.DEATH_DOCUMENT_DATE != null)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DEATH_DOCUMENT_DATE ?? 0);
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay cap giay bao tu DEATH_DOCUMENT_DATE_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IN_TIME_STR")
                    {
                        try
                        {
                            if (data.IN_TIME > 0)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                            }
                            else
                            {
                                e.Value = "";
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao IN_TIME_STR", ex);

                        }
                    }
                    else if (e.Column.FieldName == "OUT_TIME_STR")
                    {
                        try
                        {
                            if (data.OUT_TIME != null)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                            }
                            else
                            {
                                e.Value = "";
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao OUT_TIME_STR", ex);

                        }
                    }
                    else if (e.Column.FieldName == "DEATH_DOCUMENT_DATE_STR")
                    {
                        try
                        {
                            if (data.DEATH_DOCUMENT_DATE != null)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DEATH_DOCUMENT_DATE ?? 0);
                            }
                            else
                            {
                                e.Value = "";
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao DEATH_DOCUMENT_DATE_STR", ex);

                        }
                    }
                    else if (e.Column.FieldName == "DEATH_SYNC_TIME_STR")
                    {
                        try
                        {
                            if (data.DEATH_SYNC_TIME != null)
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.DEATH_SYNC_TIME ?? 0);
                            }
                            else
                            {
                                e.Value = "";
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao DEATH_SYNC_TIME_STR", ex);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }
        #endregion
        #region EventClickPress
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void btnRefresh_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnRefresh.Focus();
                    btnRefresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtKskDriverCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
        #region EvenBarManager
        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
     
        #region InitComboExecuteRoom
        private void cboDepartment_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string roomName = "";
                if (this.departmentSelecteds != null && this.departmentSelecteds.Count > 0)
                {
                    foreach (var item in this.departmentSelecteds)
                    {
                        roomName += item.DEPARTMENT_NAME + ", ";

                    }
                }
                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void InitComboDepartment()
        {
            try
            {
                cboDepartment.Properties.DataSource = BackendDataWorker.Get<HIS_DEPARTMENT>();
                cboDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboDepartment.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn column = cboDepartment.Properties.View.Columns.AddField("DEPARTMENT_NAME");
                column.VisibleIndex = 1;
                column.Width = 200;
                column.Caption = Inventec.Common.Resource.Get.Value("UcDeathInformationList.cboSYNC_RESULT_TYPE.Properties.1", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                cboDepartment.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboDepartment.Properties.View.OptionsSelection.MultiSelect = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboDepartmentCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboDepartment.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check);
                cboDepartment.Properties.Tag = gridCheck;
                cboDepartment.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboDepartment.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboDepartment.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Event_Check(object sender, EventArgs e)
        {

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                departmentSelecteds = new List<HIS_DEPARTMENT>();
                if (gridCheckMark != null)
                {
                    List<HIS_DEPARTMENT> erSelectedNews = new List<HIS_DEPARTMENT>();
                    foreach (HIS_DEPARTMENT er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.DEPARTMENT_NAME);
                            erSelectedNews.Add(er);
                        }
                    }
                    this.departmentSelecteds = new List<HIS_DEPARTMENT>();
                    this.departmentSelecteds.AddRange(erSelectedNews);
                }
                this.cboDepartment.Text = sb.ToString();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void gridView1_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                if (gridView1.GetSelectedRows().Count() > 0)
                {
                    btnDongBo.Enabled = true;
                }
                else
                {
                    btnDongBo.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public string AppFilePathSignService()
        {
            try
            {
                string pathFolderTemp = Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "Integrate"), "EMR.SignProcessor"), "EMR.SignProcessor.exe");
                return pathFolderTemp;
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message);
                return "";
            }
        }
        private bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName == name || clsProcess.ProcessName == String.Format("{0}.exe", name) || clsProcess.ProcessName == String.Format("{0} (32 bit)", name) || clsProcess.ProcessName == String.Format("{0}.exe (32 bit)", name))
                {
                    return true;
                }
            }

            return false;
        }
        internal bool VerifyServiceSignProcessorIsRunning()
        {
            bool valid = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.1");
                string exeSignPath = AppFilePathSignService();
                if (File.Exists(exeSignPath))
                {
                    if (IsProcessOpen("EMR.SignProcessor"))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.2");
                        valid = true;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.3");
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => exeSignPath), exeSignPath));
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = exeSignPath;
                        try
                        {
                            Process.Start(startInfo);
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.4");
                            Thread.Sleep(500);
                            valid = true;
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.5");
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        private void chkSignFileCertUtil_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                timer1.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDongBo_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool rs = false;
                List<DeathSyncSDO> listDeathSyncSDO = new List<DeathSyncSDO>();
                WaitingManager.Show();
                var rowHandles = gridView1.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    if (chkSignFileCertUtil.Checked)
                    {
                        var ConnectionInfo = HisConfigs.Get<string>("MOS.HIS_DEATH.CONNECTION_INFO");
                        if (string.IsNullOrEmpty(ConnectionInfo))
                        {
                            WaitingManager.Hide();
                            XtraMessageBox.Show(ResourceMessage.SaiDiaChi, "Thông báo", MessageBoxButtons.OK);
                            return;
                        }
                        if (certificate == null && !String.IsNullOrWhiteSpace(SerialNumber))
                        {
                            certificate = Inventec.Common.SignFile.CertUtil.GetBySerial(SerialNumber, requirePrivateKey: true, validOnly: false);
                            if (certificate == null)
                            {
                                chkSignFileCertUtil.Checked = false;
                                WaitingManager.Hide();
                                if (XtraMessageBox.Show(ResourceMessage.KhongLayDuocChungThu, "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                {
                                    return;
                                }
                                WaitingManager.Show();
                            }
                        }
                        var arrBr = ConnectionInfo.Split('|');
                        if(arrBr != null && arrBr.Count() > 0)
                        {
                            foreach (var item in arrBr)
                            {
                                WaitingManager.Show();
                                var arrInfo = item.Split(';');
                                if(arrInfo != null && arrInfo.Count() > 3)
                                {
                                    var brCode = arrInfo[0];
                                    var user = arrInfo[1];
                                    var pass = arrInfo[2];
                                    var url = arrInfo[3];
                                    HIS.Bhyt.Hssk.SyncDataProcess data = new Bhyt.Hssk.SyncDataProcess(HisConfigs.Get<string>("HIS.CHECK_HEIN_CARD.BHXH__ADDRESS"), user, pass);
                                    foreach (var i in rowHandles)
                                    {
                                        var row = (V_HIS_TREATMENT)gridView1.GetRow(i);
                                        if (row != null)
                                        {
                                            DeathSyncSDO sdo = new DeathSyncSDO();
                                            sdo.PatientData = GetPatient(row.PATIENT_ID);
                                            sdo.DeathData = GetIllnessInfo(row.ID);
                                            sdo.TreatmentData = row;
                                            if (certificate != null)
                                            {
                                                string MessageError = null;
                                                sdo.FileBase64Str = data.ProcessDeathInfo(lstBranch.FirstOrDefault(o => o.ID == row.BRANCH_ID), sdo.PatientData, row, sdo.DeathData, certificate, ref MessageError);
                                                if (!String.IsNullOrEmpty(MessageError))
                                                {
                                                    WaitingManager.Hide();
                                                    XtraMessageBox.Show(row.TREATMENT_CODE + ": " + MessageError, "Thông báo");
                                                }
                                                WaitingManager.Show();
                                            }
                                            listDeathSyncSDO.Add(sdo);
                                        }
                                    }
                                }
                                else
                                {
                                    WaitingManager.Hide();
                                    XtraMessageBox.Show(ResourceMessage.SaiDiaChi, "Thông báo", MessageBoxButtons.OK);
                                    continue;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var i in rowHandles)
                        {
                            var row = (V_HIS_TREATMENT)gridView1.GetRow(i);
                            if (row != null)
                            {
                                DeathSyncSDO sdo = new DeathSyncSDO();
                                sdo.PatientData = GetPatient(row.PATIENT_ID);
                                sdo.DeathData = GetIllnessInfo(row.ID);
                                sdo.TreatmentData = row;
                                listDeathSyncSDO.Add(sdo);
                            }
                        }
                    }
                    if (listDeathSyncSDO != null && listDeathSyncSDO.Count > 0)
                    {
                        rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisTreatment/SyncDeath", ApiConsumer.ApiConsumers.MosConsumer, listDeathSyncSDO, SessionManager.ActionLostToken, param);
                    }
                    if (rs)
                    {
                        FillDataToGrid();
                    }
                }
                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this.ParentForm, param, rs);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private HIS_PATIENT GetPatient(long patientId)
        {
            HIS_PATIENT obj = null;
            try
            {
                CommonParam param = new CommonParam();
                HisPatientFilter filter = new HisPatientFilter();
                filter.ID = patientId;
                obj = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }
        private HIS_SEVERE_ILLNESS_INFO GetIllnessInfo(long treatmentId)
        {
            HIS_SEVERE_ILLNESS_INFO obj = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSevereIllnessInfoFilter filter = new HisSevereIllnessInfoFilter();
                filter.TREATMENT_ID = treatmentId;
                obj = new BackendAdapter(param).Get<List<HIS_SEVERE_ILLNESS_INFO>>("api/HisSevereIllnessInfo/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return obj;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();
                SerialNumber = null;
                if (chkSignFileCertUtil.Checked)
                {
                    if (VerifyServiceSignProcessorIsRunning())
                    {
                        WcfSignDCO wcfSignDCO = new WcfSignDCO();
                        wcfSignDCO.HwndParent = this.ParentForm.Handle;
                        string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                        SignProcessorClient signProcessorClient = new SignProcessorClient();
                        var wcfSignResultDCO = signProcessorClient.GetSerialNumber(jsonData);
                        if (wcfSignResultDCO != null)
                        {
                            SerialNumber = wcfSignResultDCO.OutputFile;
                        }
                    }
                    if (string.IsNullOrEmpty(SerialNumber))
                    {
                        chkSignFileCertUtil.Checked = false;
                        XtraMessageBox.Show(ResourceMessage.KhongLayDuocChungThu2, "Thông báo");
                    }
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSignFileCertUtil.Name && o.MODULE_LINK == this.currentModule.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = SerialNumber;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSignFileCertUtil.Name;
                    csAddOrUpdate.VALUE = SerialNumber;
                    csAddOrUpdate.MODULE_LINK = this.currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
