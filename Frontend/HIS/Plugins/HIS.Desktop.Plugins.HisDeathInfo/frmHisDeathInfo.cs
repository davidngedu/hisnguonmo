﻿using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisDeathInfo.ProcessLoadDataCombo;
using HIS.Desktop.Plugins.HisDeathInfo.Sda.SdaEventLogCreate;
using HIS.UC.Death;
using HIS.UC.Death.ADO;
using HIS.UC.UCCauseOfDeath;
using HIS.UC.UCCauseOfDeath.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.HisDeathInfo
{
    public partial class frmHisDeathInfo : HIS.Desktop.Utility.FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId;
        //internal HIS_DEATH hisDeath;
        private HIS_TREATMENT hisTreatment;
        int positionHandle = -1;
        HIS_SEVERE_ILLNESS_INFO SevereIllNessInfo;
        List<HIS_EVENTS_CAUSES_DEATH> ListEventsCausesDeath;
        private DeathProcessor deathProcessor;
        private UserControl ucDeath;
        private UCCauseOfDeathProcessor causeOfDeathProcessor;
        private UserControl ucCauseOfDeath;
        DeathInitADO deathInitADO;
        CauseOfDeathADO causeOfDeathADO;
        public frmHisDeathInfo()
        {
            InitializeComponent();
        }

        public frmHisDeathInfo(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
                SetIconFrm();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmHisDeathInfo_Load(object sender, EventArgs e)
        {
            try
            {
                LoadTreatment();

                if (hisTreatment != null)
                {
                    btnEdit.Enabled = true;                   
                    InitUCDeath();
                    InitUCCauseOfDeath();
                }
                else
                {
                    btnEdit.Enabled = false;
                    btnSave.Enabled = false;
                }
                layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem12.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem13.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                layoutControlItem14.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCCauseOfDeath()
        {
            try
            {
                causeOfDeathADO = new CauseOfDeathADO();
                causeOfDeathADO.Treatment = hisTreatment;
                causeOfDeathADO.SevereIllNessInfo = SevereIllNessInfo;
                causeOfDeathADO.ListEventsCausesDeath = ListEventsCausesDeath;
                causeOfDeathProcessor = new UCCauseOfDeathProcessor();
                ucCauseOfDeath = (UserControl)causeOfDeathProcessor.Run(causeOfDeathADO);
                causeOfDeathProcessor.SetValue(ucCauseOfDeath, causeOfDeathADO);
                ucCauseOfDeath.Dock = DockStyle.Fill;
                xtraScrollableControl2.Controls.Clear();
                xtraScrollableControl2.Controls.Add(this.ucCauseOfDeath);
                causeOfDeathProcessor.ReadOnlyAll(ucCauseOfDeath, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCDeath()
        {
            try
            {
                deathInitADO = new DeathInitADO();
                deathInitADO.DeathDataSource = new DeathDataSourcesADO();
                deathInitADO.DeathDataSource.CurrentHisTreatment = hisTreatment;
                deathInitADO.DeathDataSource.HisDeathCauses = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>();
                deathInitADO.DeathDataSource.HisDeathWithins = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_WITHIN>();

                BackendDataWorker.Reset<V_HIS_DEATH_CERT_BOOK>();
                var deathCertBooks = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_DEATH_CERT_BOOK>().Where(o => o.IS_ACTIVE == 1 && o.TOTAL > 0 && (o.FROM_NUM_ORDER + o.TOTAL - 1 > o.CURRENT_DEATH_CERT_NUM) && (o.BRANCH_ID == null || o.BRANCH_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId()));

                if (deathCertBooks != null && deathCertBooks.Count() > 0)
                    deathInitADO.DeathDataSource.HisDeathCertBooks = deathCertBooks.ToList();
                deathInitADO.BranchName = BranchDataWorker.Branch.BRANCH_NAME;
                deathProcessor = new DeathProcessor();
                this.ucDeath = (UserControl)deathProcessor.Run(deathInitADO);
                this.ucDeath.Dock = DockStyle.Fill;
                xtraScrollableControl1.Controls.Clear();
                xtraScrollableControl1.Controls.Add(this.ucDeath);
                deathProcessor.ReadOnlyAll(ucDeath, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                hisTreatment = new HIS_TREATMENT();
                MOS.Filter.HisTreatmentFilter f = new MOS.Filter.HisTreatmentFilter();
                f.ID = this.treatmentId;
                var treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, f, param);
                if (treatment != null && treatment.Count == 1)
                {
                    hisTreatment = treatment.First();
                    HisSevereIllnessInfoFilter filter = new HisSevereIllnessInfoFilter();
                    filter.TREATMENT_ID = hisTreatment.ID;
                    var dtSevere = new BackendAdapter(param).Get<List<HIS_SEVERE_ILLNESS_INFO>>("api/HisSevereIllnessInfo/Get", ApiConsumers.MosConsumer, filter, param);
                    if (dtSevere != null && dtSevere.Count > 0)
                    {
                        SevereIllNessInfo = dtSevere[0];
                        HisEventsCausesDeathFilter filterChild = new HisEventsCausesDeathFilter();
                        filterChild.SEVERE_ILLNESS_INFO_ID = dtSevere[0].ID;
                        var dtEventsCausesDeath = new BackendAdapter(param).Get<List<HIS_EVENTS_CAUSES_DEATH>>("api/HisEventsCausesDeath/Get", ApiConsumers.MosConsumer, filterChild, param);
                        ListEventsCausesDeath = dtEventsCausesDeath;
                    }
                }
                else
                {
                    MessageBox.Show(
                        Inventec.Common.Resource.Get.Value("LoiThongTinHoSo",
                        Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()));
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }    

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                btnEdit.Enabled = false;
                btnSave.Enabled = true;
                btnCancel.Enabled = true;

                //1. Readonly tất cả control
                deathProcessor.ReadOnlyAll(ucDeath, false);
                causeOfDeathProcessor.ReadOnlyAll(ucCauseOfDeath, false);
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

                CommonParam param = new CommonParam();
                bool success = false;
                HIS_TREATMENT deathUpdate = deathProcessor.GetValueHisTreatment(ucDeath) as HIS_TREATMENT;

                if (deathUpdate == null)
                    return;
                CauseOfDeathADO cause = causeOfDeathProcessor.GetValue(this.ucCauseOfDeath) as CauseOfDeathADO;

                if (cause == null || (cause.SevereIllNessInfo == null && (cause.ListEventsCausesDeath == null || cause.ListEventsCausesDeath.Count == 0)))
                    return;

                WaitingManager.Show();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => deathUpdate), deathUpdate));
                var rs = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/UpdateDeathInfo", ApiConsumers.MosConsumer, deathUpdate, param);
                CallApiSevereIllnessInfo(cause.SevereIllNessInfo, cause.ListEventsCausesDeath);
                WaitingManager.Hide();
                if (rs != null)
                {
                    success = true;
                    hisTreatment = rs;
                    //Nếu thành công
                    btnEdit.Enabled = true;
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                    //Disable
                    deathProcessor.ReadOnlyAll(ucDeath, true);
                }

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                btnCancel.Enabled = false;
                btnSave.Enabled = false;
                if (hisTreatment != null)
                {
                    btnEdit.Enabled = true;
                    deathProcessor.SetValue(ucDeath, deathInitADO.DeathDataSource);
                    deathProcessor.ReadOnlyAll(ucDeath, true);
                    causeOfDeathProcessor.SetValue(ucCauseOfDeath, causeOfDeathADO);
                    causeOfDeathProcessor.ReadOnlyAll(ucCauseOfDeath, true);
                }
                else
                {

                    btnEdit.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Edit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnEdit.Enabled && xtraTabControl1.SelectedTabPageIndex != 0)
                    return;
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                    if (!btnSave.Enabled)
                        return;
                    btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Cancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnCancel.Enabled && xtraTabControl1.SelectedTabPageIndex != 0)
                    return;
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    
        private void OnClickInGiayBaoTu()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate(MPS.Processor.Mps000268.PDO.Mps000268PDO.printTypeCode, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case MPS.Processor.Mps000268.PDO.Mps000268PDO.printTypeCode:
                        InGiayBaoTu(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void InGiayBaoTu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                MPS.Processor.Mps000268.PDO.Mps000268ADO ado = new MPS.Processor.Mps000268.PDO.Mps000268ADO();
                if (this.hisTreatment.DEATH_CAUSE_ID != null)
                {
                    var deathCause = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == this.hisTreatment.DEATH_CAUSE_ID.Value);
                    if (deathCause != null)
                    {
                        ado.DEATH_CAUSE_CODE = deathCause.DEATH_CAUSE_CODE;
                        ado.DEATH_CAUSE_NAME = deathCause.DEATH_CAUSE_NAME;
                    }
                }
                if (this.hisTreatment.DEATH_WITHIN_ID != null)
                {
                    var deathWithin = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_WITHIN>().FirstOrDefault(o => o.ID == this.hisTreatment.DEATH_WITHIN_ID.Value);
                    if (deathWithin != null)
                    {
                        ado.DEATH_WITHIN_CODE = deathWithin.DEATH_WITHIN_CODE;
                        ado.DEATH_WITHIN_NAME = deathWithin.DEATH_WITHIN_NAME;
                    }
                }

                if (this.hisTreatment.END_ROOM_ID.HasValue)
                {
                    var endRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.hisTreatment.END_ROOM_ID.Value);
                    if (endRoom != null)
                    {
                        ado.END_DEPARTMENT_CODE = endRoom.DEPARTMENT_CODE;
                        ado.END_DEPARTMENT_NAME = endRoom.DEPARTMENT_NAME;
                        ado.END_ROOM_CODE = endRoom.ROOM_CODE;
                        ado.END_ROOM_NAME = endRoom.ROOM_NAME;
                    }
                }
                if (this.hisTreatment.FEE_LOCK_ROOM_ID.HasValue)
                {
                    var feelockRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.hisTreatment.FEE_LOCK_ROOM_ID.Value);
                    if (feelockRoom != null)
                    {
                        ado.FEE_LOCK_DEPARTMENT_CODE = feelockRoom.DEPARTMENT_CODE;
                        ado.FEE_LOCK_DEPARTMENT_NAME = feelockRoom.DEPARTMENT_NAME;
                        ado.FEE_LOCK_ROOM_CODE = feelockRoom.ROOM_CODE;
                        ado.FEE_LOCK_ROOM_NAME = feelockRoom.ROOM_NAME;
                    }
                }
                if (this.hisTreatment.IN_ROOM_ID.HasValue)
                {
                    var inRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.hisTreatment.IN_ROOM_ID.Value);
                    if (inRoom != null)
                    {
                        ado.IN_DEPARTMENT_CODE = inRoom.DEPARTMENT_CODE;
                        ado.IN_DEPARTMENT_NAME = inRoom.DEPARTMENT_NAME;
                        ado.IN_ROOM_CODE = inRoom.ROOM_CODE;
                        ado.IN_ROOM_NAME = inRoom.ROOM_NAME;
                    }
                }

                HIS_BRANCH currentBranch = new HIS_BRANCH();
                if (this.currentModule != null)
                {
                    var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId && o.ROOM_TYPE_ID == this.currentModule.RoomTypeId);
                    currentBranch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == room.BRANCH_ID);
                }

                if (currentBranch != null)
                {
                    ado.BRANCH_ADDRESS = currentBranch.ADDRESS;
                }

                V_HIS_PATIENT patient = GetViewPatientByID(this.hisTreatment.PATIENT_ID);

                MPS.Processor.Mps000268.PDO.Mps000268PDO mps000268RDO = new MPS.Processor.Mps000268.PDO.Mps000268PDO(
                patient,
                this.hisTreatment,
                ado);
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000268RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000268RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private V_HIS_PATIENT GetViewPatientByID(long patientId)
        {
            V_HIS_PATIENT result = new V_HIS_PATIENT();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
                filter.ID = patientId;
                var patients = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (patients != null && patients.Count() > 0)
                {
                    result = patients.First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                OnClickInGiayBaoTu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {

        }
        private async Task CallApiSevereIllnessInfo(HIS_SEVERE_ILLNESS_INFO SevereIllnessInfo, List<HIS_EVENTS_CAUSES_DEATH> EventsCausesDeaths)
        {
            try
            {
                CommonParam param = new CommonParam();
                SevereIllnessInfoSDO sdo = new SevereIllnessInfoSDO();
                sdo.SevereIllnessInfo = SevereIllnessInfo;
                sdo.EventsCausesDeaths = EventsCausesDeaths;
                var dt = new BackendAdapter(param)
                   .Post<HisServiceReqExamUpdateResultSDO>("api/HisSevereIllnessInfo/CreateOrUpdate", ApiConsumers.MosConsumer, sdo, param);
                string message = string.Format("Lưu thông tin tử vong. TREATMENT_CODE: {0}", hisTreatment.TREATMENT_CODE);
                string login = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                SdaEventLogCreate eventlog = new SdaEventLogCreate();
                eventlog.Create(login, null, true, message);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
