﻿using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.MedicineIsUsedPatient.ADO;
using HIS.Desktop.Plugins.MedicineIsUsedPatient.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.MedicineIsUsedPatient.MedicineIsUsedPatient
{
    public partial class frmMedicineIsUsedPatient : FormBase
    {
        #region Declare
        private Inventec.Desktop.Common.Modules.Module ModuleData;
        #endregion

        public frmMedicineIsUsedPatient(Inventec.Desktop.Common.Modules.Module moduleData, string _treatmentCode)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                this.ModuleData = moduleData;
                if (moduleData != null)
                {
                    this.Text = moduleData.text;
                }
                this.txtTreatmentCode.Text = _treatmentCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmMedicineIsUsedPatient_Load(object sender, EventArgs e)
        {
            try
            {
                HisConfigCFG.LoadConfig();
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BindTreePlus(List<ExpMestMediMateADO> lstAdo)
        {
            try
            {
                List<ExpMestMediMateADO> SereServADOs = new List<ExpMestMediMateADO>();
                var listRootSety = lstAdo.GroupBy(g => g.SERVICE_REQ_CODE).ToList();
                foreach (var itemGr in listRootSety)
                {
                    var listBySety = itemGr.ToList<ExpMestMediMateADO>();
                    ExpMestMediMateADO ssRootSety = new ExpMestMediMateADO();
                    ssRootSety.MEDIMATE_TYPE_CODE = listBySety.FirstOrDefault().SERVICE_REQ_CODE;
                    ssRootSety.MEDIMATE_TYPE_NAME = listBySety.FirstOrDefault().REQUEST_LOGINNAME + " - " + listBySety.FirstOrDefault().REQUEST_USERNAME + " " + '(' + listBySety.FirstOrDefault().INTRUCTION_TIME + ')';
                    ssRootSety.IS_USED = false;
                    ssRootSety.CONCRETE_ID__IN_SETY = listBySety.FirstOrDefault().SERVICE_REQ_CODE;
                    SereServADOs.Add(ssRootSety);

                    int d = 0;
                    foreach (var item in listBySety)
                    {
                        d++;
                        item.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + d;
                        item.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                        SereServADOs.Add(item);
                    }
                }

                SereServADOs = SereServADOs.OrderBy(o => o.PARENT_ID__IN_SETY).ThenBy(p => p.MEDIMATE_TYPE_NAME).ThenBy(o => o.MEDIMATE_TYPE_CODE).ToList();
                BindingList<ExpMestMediMateADO> records = new BindingList<ExpMestMediMateADO>(SereServADOs);
                treeMedicineIsUsePt.DataSource = records;
                treeMedicineIsUsePt.ExpandAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetDefaultValueControl();
                this.treeMedicineIsUsePt.ClearNodes();
                HIS_TREATMENT listTreatment = LoadSearch();
                FillInfoPatient(listTreatment);
                LoadDataToTreeByTreatment(listTreatment);
                if (listTreatment == null)
                {
                    WaitingManager.Hide();
                    MessageBox.Show(Resources.ResourceLanguageManager.KhongTimThayMaDieuTri);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToTreeByTreatment(HIS_TREATMENT data)
        {
            List<ExpMestMediMateADO> lstAdo = new List<ExpMestMediMateADO>();
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.EXP_MEST_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DDT, IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT };
                expMestFilter.TDL_TREATMENT_ID = data != null ? data.ID : -1;
                var lstExpMest = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, null);
                if (lstExpMest != null && lstExpMest.Count > 0)
                {
                    HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                    serviceReqFilter.IS_ACTIVE = 1;
                    serviceReqFilter.IDs = lstExpMest.Select(s => s.SERVICE_REQ_ID ?? 0).Distinct().ToList();
                    List<HIS_SERVICE_REQ> lstserviceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, null);

                    HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                    expMestMedicineFilter.IS_ACTIVE = 1;
                    expMestMedicineFilter.IS_EXPORT = true;
                    expMestMedicineFilter.EXP_MEST_IDs = lstExpMest.Select(p => p.ID).ToList();
                    var lstExpMestMedicine = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMedicineFilter, null);
                    if (lstExpMestMedicine != null && lstExpMestMedicine.Count > 0)
                    {
                        foreach (var item in lstExpMestMedicine)
                        {
                            ExpMestMediMateADO ado = new ExpMestMediMateADO();
                            ado.IS_MATERIAL = false;
                            ado.IS_MEDICINE = true;

                            ado.MEDIMATE_ID = item.MEDICINE_ID;
                            ado.MEDIMATE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                            ado.MEDIMATE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                            ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            ado.EXP_MEST_MEDI_MATE_ID = item.ID;
                            ado.AMOUNT = item.AMOUNT;
                            ado.IS_USED = item.IS_USED == 1 ? true : false;

                            HIS_SERVICE_REQ servicereq = (lstserviceReq != null && lstserviceReq.Count > 0) ? lstserviceReq.FirstOrDefault(o => o.ID == item.TDL_SERVICE_REQ_ID) : null;
                            if (servicereq != null)
                            {
                                ado.INTRUCTION_TIME = Inventec.Common.DateTime.Convert.TimeNumberToDateString(servicereq.INTRUCTION_TIME);
                                ado.SERVICE_REQ_CODE = servicereq.SERVICE_REQ_CODE;
                                ado.REQUEST_LOGINNAME = servicereq.REQUEST_LOGINNAME;
                                ado.REQUEST_USERNAME = servicereq.REQUEST_USERNAME;
                            }

                            lstAdo.Add(ado);
                        }
                    }

                    HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                    expMestMaterialFilter.IS_ACTIVE = 1;
                    expMestMaterialFilter.IS_EXPORT = true;
                    expMestMaterialFilter.EXP_MEST_IDs = lstExpMest.Select(p => p.ID).ToList();
                    var lstExpMestMaterial = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMaterialFilter, null);
                    if (lstExpMestMaterial != null && lstExpMestMaterial.Count > 0)
                    {
                        foreach (var item in lstExpMestMaterial)
                        {
                            ExpMestMediMateADO ado = new ExpMestMediMateADO();
                            ado.IS_MATERIAL = true;
                            ado.IS_MEDICINE = false;

                            ado.MEDIMATE_ID = item.MATERIAL_ID;
                            ado.MEDIMATE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                            ado.MEDIMATE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                            ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            ado.EXP_MEST_MEDI_MATE_ID = item.ID;
                            ado.AMOUNT = item.AMOUNT;
                            ado.IS_USED = item.IS_USED == 1 ? true : false;

                            HIS_SERVICE_REQ servicereq = (lstserviceReq != null && lstserviceReq.Count > 0) ? lstserviceReq.FirstOrDefault(o => o.ID == item.TDL_SERVICE_REQ_ID) : null;
                            if (servicereq != null)
                            {
                                ado.INTRUCTION_TIME = Inventec.Common.DateTime.Convert.TimeNumberToDateString(servicereq.INTRUCTION_TIME);
                                ado.SERVICE_REQ_CODE = servicereq.SERVICE_REQ_CODE;
                                ado.REQUEST_LOGINNAME = servicereq.REQUEST_LOGINNAME;
                                ado.REQUEST_USERNAME = servicereq.REQUEST_USERNAME;
                            }

                            lstAdo.Add(ado);
                        }
                    }
                }

                BindTreePlus(lstAdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillInfoPatient(HIS_TREATMENT data)
        {
            try
            {
                if (data != null)
                {
                    lbPatientCode.Text = data.TDL_PATIENT_CODE;
                    lbPatientName.Text = data.TDL_PATIENT_NAME;
                    lbDateOfBirth.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    lbGender.Text = data.TDL_PATIENT_GENDER_NAME;
                    lbAddress.Text = data.TDL_PATIENT_ADDRESS;

                    var LastPatientType = new BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, data.ID, null);
                    if (LastPatientType != null)
                    {
                        lbHeinCard.Text = HeinCardHelper.TrimHeinCardNumber(LastPatientType.HEIN_CARD_NUMBER);
                        lbDateFrom.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(LastPatientType.HEIN_CARD_FROM_TIME ?? 0);
                        lbDateTo.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(LastPatientType.HEIN_CARD_TO_TIME ?? 0);
                        lbPlaceToTreat.Text = LastPatientType.HEIN_MEDI_ORG_NAME;
                        lbPatientType.Text = LastPatientType.PATIENT_TYPE_NAME ?? "";
                        string rightRoute = "";
                        if (LastPatientType.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                        {
                            rightRoute = "Đúng tuyến";
                        }
                        else
                        {
                            rightRoute = "Trái tuyến";
                        }

                        lblRightRoute.Text = rightRoute ?? "";
                        string ratio = "";
                        if (LastPatientType.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                        {
                            decimal? heinRatio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(LastPatientType.HEIN_TREATMENT_TYPE_CODE, LastPatientType.HEIN_CARD_NUMBER, LastPatientType.LEVEL_CODE, LastPatientType.RIGHT_ROUTE_CODE);
                            if (heinRatio.HasValue)
                            {
                                ratio = ((long)(heinRatio.Value * 100)).ToString() + "%";
                            }
                        }

                        lblHeinRatio.Text = ratio ?? "";
                    }
                }
                else
                {
                    SetDefaultValueControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_TREATMENT LoadSearch()
        {
            HIS_TREATMENT result = new HIS_TREATMENT();
            try
            {
                CommonParam param = new CommonParam();
                if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;

                    var listTreatment = new BackendAdapter(param)
                            .Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                    if (listTreatment != null && listTreatment.Count > 0)
                    {
                        result = listTreatment.FirstOrDefault();
                        Inventec.Common.Logging.LogSystem.Debug("LoadSearch: " + Inventec.Common.Logging.LogUtil.TraceData("____Result Treatment____", result));
                    }
                }
            }
            catch (Exception ex)
            {
                result = new HIS_TREATMENT();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                txtTreatmentCode.Text = "";
                txtTreatmentCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                lbPatientCode.Text = "";
                lbPatientName.Text = "";
                lbHeinCard.Text = "";
                lblHeinRatio.Text = "";
                lbGender.Text = "";
                lbDateFrom.Text = "";
                lbDateOfBirth.Text = "";
                lbDateTo.Text = "";
                lbAddress.Text = "";
                lblRightRoute.Text = "";
                lbPlaceToTreat.Text = "";
                lbPatientType.Text = "";
                treeMedicineIsUsePt.ClearNodes();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWords_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeMedicineIsUsePt_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = treeMedicineIsUsePt.GetDataRecordByNode(e.Node);
                if (data != null && data is ExpMestMediMateADO)
                {
                    if (!e.Node.HasChildren)
                    {
                        if (e.Column.FieldName == "IS_USED")
                        {
                            e.RepositoryItem = repositoryItemCheckEdit1;
                        }
                    }
                    else
                    {
                        if (e.Column.FieldName == "IS_USED")
                        {
                            e.RepositoryItem = new DevExpress.XtraEditors.Repository.RepositoryItem();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWords_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                ExpMestMediMateADO data = new ExpMestMediMateADO();
                var dataSelect = treeMedicineIsUsePt.GetDataRecordByNode(treeMedicineIsUsePt.FocusedNode);
                if (dataSelect != null) data = (ExpMestMediMateADO)dataSelect;
                DevExpress.XtraTreeList.TreeList tree = sender as DevExpress.XtraTreeList.TreeList;
                if (tree != null)
                {
                    TreeListHitInfo hi = tree.CalcHitInfo(tree.PointToClient(Control.MousePosition));
                    data = (ExpMestMediMateADO)treeMedicineIsUsePt.GetDataRecordByNode(hi.Node);
                }

                bool success = false;
                if (data != null)
                {
                    if (data.IS_MEDICINE)
                    {
                        long id = data.EXP_MEST_MEDI_MATE_ID;
                        if (data.IS_USED == true)
                        {
                            var lstexpmestmedicine = new BackendAdapter(param).Post<HIS_EXP_MEST_MEDICINE>("api/HisExpMestMedicine/Unused", ApiConsumers.MosConsumer, id, null);
                            if (lstexpmestmedicine != null)
                                success = true;
                        }
                        else
                        {
                            var lstexpmestmedicine = new BackendAdapter(param).Post<HIS_EXP_MEST_MEDICINE>("api/HisExpMestMedicine/Used", ApiConsumers.MosConsumer, id, null);
                            if (lstexpmestmedicine != null)
                                success = true;
                        }
                    }
                    else
                    {
                        long id = data.EXP_MEST_MEDI_MATE_ID;
                        if (data.IS_USED == true)
                        {
                            var lstexpmestmaterial = new BackendAdapter(param).Post<HIS_EXP_MEST_MATERIAL>("api/HisExpMestMaterial/Unused", ApiConsumers.MosConsumer, id, null);
                            if (lstexpmestmaterial != null)
                                success = true;
                        }
                        else
                        {
                            var lstexpmestmaterial = new BackendAdapter(param).Post<HIS_EXP_MEST_MATERIAL>("api/HisExpMestMaterial/Used", ApiConsumers.MosConsumer, id, null);
                            if (lstexpmestmaterial != null)
                                success = true;
                        }
                    }

                    if (success)
                    {
                        data.IS_USED = !data.IS_USED;
                    }

                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeMedicineIsUsePt_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void treeMedicineIsUsePt_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = treeMedicineIsUsePt.GetDataRecordByNode(e.Node);
                if (data != null && data is ExpMestMediMateADO)
                {
                    if (e.Node.HasChildren)
                    {
                        e.Appearance.ForeColor = Color.Black;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                        e.Appearance.BackColor = Color.Yellow;
                        e.Appearance.BackColor2 = Color.Yellow;
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Black;
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
