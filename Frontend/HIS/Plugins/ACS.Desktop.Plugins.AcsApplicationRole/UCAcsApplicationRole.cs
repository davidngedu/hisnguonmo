﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Adapter;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;

using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.Data;
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using ACS.Desktop.Plugins.AcsApplicationRole.entity;

namespace ACS.Desktop.Plugins.AcsApplicationRole
{
    public partial class UCAcsApplicationRole : HIS.Desktop.Utility.UserControlBase
    {
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int start1 = 0;
        int limit1 = 0;
        int rowCount1 = 0;
        int dataTotal1 = 0;
        List<ApplicationADO> ListApplicationADO = new List<ApplicationADO>();
        List<RoleADO> ListRoleADOCurrent = new List<RoleADO>();
        List<RoleADO> ListRoleADO = new List<RoleADO>();
        List<ACS_APPLICATION_ROLE> ListModuleRole = new List<ACS_APPLICATION_ROLE>();
        long ApplicationID = 0;
        long RoleID = 0;
        Inventec.Desktop.Common.Modules.Module moduleData;
        Action<Type> delegateRefresh;

        public UCAcsApplicationRole(Inventec.Desktop.Common.Modules.Module moduleData, Inventec.Common.WebApiClient.ApiConsumer sdaConsumer, Inventec.Common.WebApiClient.ApiConsumer acsConsumer, Action<Type> delegateRefresh, long numPageSize, string applicationCode, string iconPath)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            this.delegateRefresh = delegateRefresh;
            ConfigApplications.NumPageSize = numPageSize;
            GlobalVariables.APPLICATION_CODE = applicationCode;
            ApiConsumers.SdaConsumer = sdaConsumer;
            ApiConsumers.AcsConsumer = acsConsumer;
        }

        public void FindShortcut1()
        {
            try
            {
                btnFind1_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FindShortcut2()
        {
            try
            {
                btnFind2_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SaveShortcut()
        {
            try
            {
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCAcsApplicationRole_Load(object sender, EventArgs e)
        {
            try
            {
                LoadAcsApplicationRole();
                WaitingManager.Show();
                grclCheckForRole.Image = imgStock.Images[1];
                grclCheckForRole.ImageAlignment = StringAlignment.Center;
                grclcheck21.Image = imgStock.Images[1];
                grclcheck21.ImageAlignment = StringAlignment.Center;
                cboChoose.EditValue = "Ứng dụng";
                LoadStatus();
                FilldataToGridModule();
                FilldataToGridRole();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadAcsApplicationRole()
        {
            CommonParam param2 = new CommonParam();
            AcsApplicationRoleFilter filter2 = new AcsApplicationRoleFilter();
            ListModuleRole = new BackendAdapter(param2).Get<List<ACS_APPLICATION_ROLE>>("api/AcsApplicationRole/Get", ApiConsumers.AcsConsumer, filter2, param2);
        }



        private void LoadStatus()
        {
            if (cboChoose.EditValue == "Ứng dụng")
            {
                grclCheckForApplication.OptionsColumn.AllowEdit = true;
                grclcheck21.OptionsColumn.AllowEdit = true;
                grclCheckForRole.OptionsColumn.AllowEdit = false;
                grclcheck22.OptionsColumn.AllowEdit = false;
                check21.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;

            }
            else
            {
                grclCheckForApplication.OptionsColumn.AllowEdit = false;
                grclcheck21.OptionsColumn.AllowEdit = false;
                grclCheckForRole.OptionsColumn.AllowEdit = true;
                grclcheck22.OptionsColumn.AllowEdit = true;
                chkForRole.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            }
        }

        private void FilldataToGridRole()
        {
            try
            {
                WaitingManager.Show();
                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                LoadPaging2(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount1;
                param.Count = dataTotal1;
                ucPaging2.Init(LoadPaging2, param, numPageSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging2(object param)
        {

            try
            {
                start1 = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start1, limit);
                Inventec.Core.ApiResultObject<List<ACS_ROLE>> apiResult = null;
                AcsRoleFilter filter = new AcsRoleFilter();
                SetFilterNavBar2(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                gridView2.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<ACS_ROLE>>("api/AcsRole/Get", ApiConsumers.AcsConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<ACS_ROLE>)apiResult.Data;

                    if (data != null)
                    {
                        ListRoleADO = new List<RoleADO>();
                        foreach (var item in data)
                        {
                            RoleADO x = new RoleADO();
                            x.ROLE_CODE = item.ROLE_CODE;
                            x.ROLE_NAME = item.ROLE_NAME;
                            x.ID = item.ID;
                            x.checkForApplication = false;
                            x.checkForRole = false;

                            ListRoleADO.Add(x);
                        }
                        if (ApplicationID != 0)
                        {
                            List<long> ListRoleID = new List<long>();
                            ListRoleID = (from item in (ListModuleRole.Where(o => o.APPLICATION_ID == ApplicationID).ToList()) select item.ROLE_ID).ToList();

                            foreach (var item in ListRoleADO)
                            {
                                item.checkForApplication = false;
                                foreach (var item1 in ListRoleID)
                                {
                                    if (item.ID == item1)
                                    {
                                        item.checkForApplication = true;
                                    }
                                }
                            }
                        }

                        gridView2.GridControl.DataSource = ListRoleADO.OrderByDescending(o => o.checkForApplication);
                        rowCount1 = (ListRoleADO == null ? 0 : ListRoleADO.Count);
                        dataTotal1 = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView2.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        public object GetDataGridViewRole()
        {
            object result = null;
            try
            {
                if (gridView2.IsEditing)
                    gridView2.CloseEditor();

                if (gridView2.FocusedRowModified)
                    gridView2.UpdateCurrentRow();

                result = (List<RoleADO>)gridControl2.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }
        public object GetDataGridViewModule()
        {
            object result = null;
            try
            {
                if (grViewModule.IsEditing)
                    grViewModule.CloseEditor();

                if (grViewModule.FocusedRowModified)
                    grViewModule.UpdateCurrentRow();

                result = (List<ApplicationADO>)gridControl1.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }


        private void SetFilterNavBar2(ref AcsRoleFilter filter)
        {
            filter.KEY_WORD = txtKeyword2.Text.Trim();
        }

        private void FilldataToGridModule()
        {
            try
            {
                WaitingManager.Show();
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                LoadPaging1(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging1, param, numPageSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging1(object param)
        {

            try
            {
                start = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<ACS_APPLICATION>> apiResult = null;
                AcsApplicationFilter filter = new AcsApplicationFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                grViewModule.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<ACS_APPLICATION>>("api/AcsApplication/Get", ApiConsumers.AcsConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<ACS_APPLICATION>)apiResult.Data;

                    if (data != null)
                    {
                        ListApplicationADO = new List<ApplicationADO>();
                        foreach (var item in data)
                        {
                            ApplicationADO x = new ApplicationADO();
                            x.APPLICATION_NAME = item.APPLICATION_NAME;
                            x.ID = item.ID;
                            x.APPLICATION_CODE = item.APPLICATION_CODE;
                            x.CheckForApplication = false;
                            x.CheckForRole = false;

                            ListApplicationADO.Add(x);
                        }
                        if (RoleID != 0)
                        {
                            List<long> ListApplicationID = new List<long>();
                            CommonParam param2 = new CommonParam();
                            AcsApplicationRoleFilter filter2 = new AcsApplicationRoleFilter();
                            ListApplicationID = (from item in ListModuleRole.Where(o => o.ROLE_ID == RoleID).ToList() select item.APPLICATION_ID).ToList();

                            foreach (var item in ListApplicationADO)
                            {
                                item.CheckForRole = false;
                                foreach (var item1 in ListApplicationID)
                                {
                                    if (item.ID == item1)
                                    {
                                        item.CheckForRole = true;
                                    }
                                }
                            }
                        }

                        grViewModule.GridControl.DataSource = ListApplicationADO.OrderByDescending(o => o.CheckForRole);
                        rowCount = (ListApplicationADO == null ? 0 : ListApplicationADO.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                grViewModule.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref AcsApplicationFilter filter)
        {
            filter.KEY_WORD = txtKeyword1.Text.Trim();
        }

        private void btnFind2_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FilldataToGridRole();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind1_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FilldataToGridModule();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChoose_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                grclCheckForRole.Image = imgStock.Images[1];
                grclcheck21.Image = imgPatient.Images[1];
                ApplicationID = 0;
                RoleID = 0;
                LoadStatus();
                FilldataToGridRole();
                FilldataToGridModule();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FilldataToGridModule();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FilldataToGridRole();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            WaitingManager.Show();
            if (ApplicationID == 0 && RoleID == 0)
            {
                if (cboChoose.EditValue == "Ứng dụng")
                {
                    MessageManager.Show("chưa chọn ứng dụng");
                    WaitingManager.Hide();
                    return;
                }
                else
                {
                    MessageManager.Show("chưa chọn đối tượng");
                    WaitingManager.Hide();
                    return;
                }
            }
            if (cboChoose.EditValue == "Ứng dụng")
            {
                try
                {
                    Boolean success = false;
                    List<ACS_APPLICATION_ROLE> ListCreate = new List<ACS_APPLICATION_ROLE>();
                    List<long> ListDelete = new List<long>();
                    List<ACS_APPLICATION_ROLE> ListModuleRoleOld = ListModuleRole.Where(o => o.APPLICATION_ID == ApplicationID).ToList();
                    
                    List<long> listRoleIDOld = new List<long>();
                    List<long> listRoleIDCurent = new List<long>();
                    List<long> listRoleIDNew = new List<long>();
                    CommonParam param = new CommonParam();

                        if (ListModuleRoleOld != null && ListModuleRoleOld.Count > 0)
                        {
                            
                            foreach (var item in ListModuleRoleOld)
                            {
                                listRoleIDOld.Add(item.ROLE_ID);
                            }
                            foreach (var item in ListRoleADO.Where(o => o.checkForApplication))
                            {
                                listRoleIDNew.Add(item.ID);
                            }
                            foreach (var item in ((ListRoleADO.Where(o => o.checkForApplication && !listRoleIDOld.Contains(o.ID))).ToList()))
                            {
                                ACS_APPLICATION_ROLE ModuleRoleNew = new ACS_APPLICATION_ROLE();
                                ModuleRoleNew.ROLE_ID = item.ID;
                                ModuleRoleNew.APPLICATION_ID = ApplicationID;
                                ListCreate.Add(ModuleRoleNew);
                            }

                            foreach (var item in ListRoleADO.Where(o => !o.checkForApplication))
                            {
                                foreach (var itemOld in listRoleIDOld)
                                {
                                    if (item.ID == itemOld)
                                    {
                                        var x = ListModuleRoleOld.Where(o => o.ROLE_ID == itemOld).ToList().FirstOrDefault();
                                        ListDelete.Add(x.ID);
                                    }
                                }
                            }
                            
                            //foreach (var item in listRoleIDOld.Where(o => !listRoleIDNew.Contains(o)))
                            //{
                            //    var x = ListModuleRoleOld.Where(o => o.ROLE_ID == item).ToList().FirstOrDefault();
                            //    ListDelete.Add(x.ID);
                            //}
                        }
                        else
                        {
                            foreach (var item in ListRoleADO)
                            {
                                if (item.checkForApplication)
                                {
                                    ACS_APPLICATION_ROLE ModuleRoleNew = new ACS_APPLICATION_ROLE();
                                    ModuleRoleNew.APPLICATION_ID = ApplicationID;
                                    ModuleRoleNew.ROLE_ID = item.ID;
                                    ListCreate.Add(ModuleRoleNew);
                                }
                            }
                        }
                        if (ListCreate.Count != null && ListCreate.Count > 0)
                        {
                            //foreach (var item in dataCreates)
                            //{
                            //    ACS_APPLICATION_ROLE ModuleRoleNew = new ACS_APPLICATION_ROLE();
                            //    ModuleRoleNew.APPLICATION_ID = ApplicationID;
                            //    ModuleRoleNew.ROLE_ID = item.ID;
                            //    ListCreate.Add(ModuleRoleNew);
                            //}
                            var resultData = new BackendAdapter(param).Post<ACS_APPLICATION_ROLE>("api/AcsApplicationRole/CreateList", ApiConsumers.AcsConsumer, ListCreate, param);
                            success = true;

                        }

                        if (ListDelete.Count > 0)
                        {
                            //ListDelete = ListModuleRoleOld.Where(o => dataDeletes.Select(p => p.ID)
                                        //.Contains(o.ROLE_ID)).Select(o => o.ID).ToList();
                            var resultData = new BackendAdapter(param).Post<bool>("api/AcsApplicationRole/DeleteList", ApiConsumers.AcsConsumer, ListDelete, param);
                            if (resultData)
                            {
                                success = true;
                            }
                        }
                        else if (ListCreate.Count == 0 && ListDelete.Count == 0)
                        {
                            WaitingManager.Hide();
                            MessageBox.Show("Chưa có thay đổi", "thông báo");

                            return;
                        }
                    
                    WaitingManager.Hide();
                    RefeshGridRole(ApplicationID);
                    #region Hien thi message thong bao
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            else
            {
                try
                {
                    Boolean success = false;
                    List<ACS_APPLICATION_ROLE> ListCreate = new List<ACS_APPLICATION_ROLE>();
                    List<long> ListDelete = new List<long>();
                    List<ACS_APPLICATION_ROLE> ListModuleRoleOld = ListModuleRole.Where(o => o.ROLE_ID == RoleID).ToList();
                    List<long> listApplicationIDOld = new List<long>();
                    List<long> listApplicationIDNew = new List<long>();
                    CommonParam param = new CommonParam();
                    if (ListModuleRoleOld != null && ListModuleRoleOld.Count > 0)
                    {
                        foreach (var item in ListModuleRoleOld)
                        {
                            listApplicationIDOld.Add(item.APPLICATION_ID);
                        }
                        foreach (var item in ListApplicationADO.Where(o => o.CheckForRole))
                        {
                            listApplicationIDNew.Add(item.ID);
                        }
                        foreach (var item in ((ListApplicationADO.Where(o => o.CheckForRole && !listApplicationIDOld.Contains(o.ID))).ToList()))
                        {
                            ACS_APPLICATION_ROLE ModuleRoleNew = new ACS_APPLICATION_ROLE();
                            ModuleRoleNew.APPLICATION_ID = item.ID;
                            ModuleRoleNew.ROLE_ID = RoleID;
                            ListCreate.Add(ModuleRoleNew);
                        }
                        foreach (var item in listApplicationIDOld.Where(o => !listApplicationIDNew.Contains(o)))
                        {
                            var x = ListModuleRoleOld.Where(o => o.APPLICATION_ID == item).ToList().FirstOrDefault();
                            ListDelete.Add(x.ID);
                        }
                    }
                    else
                    {
                        foreach (var item in ListApplicationADO)
                        {
                            if (item.CheckForRole)
                            {
                                ACS_APPLICATION_ROLE ModuleRoleNew = new ACS_APPLICATION_ROLE();
                                ModuleRoleNew.APPLICATION_ID = item.ID;
                                ModuleRoleNew.ROLE_ID = RoleID;
                                ListCreate.Add(ModuleRoleNew);
                            }
                        }
                    }
                    if (ListCreate.Count != null && ListCreate.Count > 0)
                    {
                        var resultData = new BackendAdapter(param).Post<ACS_APPLICATION_ROLE>("api/AcsApplicationRole/CreateList", ApiConsumers.AcsConsumer, ListCreate, param);

                        success = true;
                    }

                    if (ListDelete.Count > 0)
                    {
                        var resultData = new BackendAdapter(param).Post<bool>("api/AcsApplicationRole/DeleteList", ApiConsumers.AcsConsumer, ListDelete, param);
                        if (resultData)
                        {
                            success = true;
                        }
                    }
                    else if (ListCreate.Count == 0 && ListDelete.Count == 0)
                    {
                        WaitingManager.Hide();
                        MessageBox.Show("Chưa có thay đổi", "thông báo");

                        return;
                    }
                    RefeshGridApplication(RoleID);
                    WaitingManager.Hide();
                    #region Hien thi message thong bao
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            #region
            //try
            //{
            //    WaitingManager.Show();

            //    Boolean success = false;
            //    List<HIS_MEST_PATIENT_TYPE> ListCreate = new List<HIS_MEST_PATIENT_TYPE>();
            //    List<long> ListDelete = new List<long>();
            //    CommonParam param = new CommonParam();
            //    if (ApplicationID == 0 && PatientID == 0)
            //    {
            //        if (cboChoose.EditValue == "Ứng dụng")
            //        {
            //            MessageBox.Show("chưa chọn Ứng dụng");
            //            WaitingManager.Hide();
            //            return;
            //        }
            //        else
            //        {
            //            MessageBox.Show("chưa chọn đối tượng");
            //            WaitingManager.Hide();
            //            return;
            //        }
            //    }
            //    if (ApplicationID != 0)
            //    {
            //        List<HIS_MEST_PATIENT_TYPE> AcsApplicationRole = BackendDataWorker.Get<HIS_MEST_PATIENT_TYPE>();
            //        List<long> ListPatientTypeIDOld = new List<long>();
            //        foreach (var item in AcsApplicationRole)
            //        {
            //            if (item.MEDI_STOCK_ID == ApplicationID)
            //            {
            //                ListPatientTypeIDOld.Add(item.PATIENT_TYPE_ID);
            //            }
            //        }
            //        foreach (var item in ListRoleADO)
            //        {
            //            if (ListPatientTypeIDOld.Contains(item.ID) && !item.check1)
            //            {
            //                var x = from item1 in AcsApplicationRole
            //                        where item1.PATIENT_TYPE_ID == item.ID && item1.MEDI_STOCK_ID == ApplicationID
            //                        select item1;
            //                HIS_MEST_PATIENT_TYPE o = x.FirstOrDefault();
            //                long ID = o.ID;
            //                ListDelete.Add(ID);
            //            }
            //            if (!ListPatientTypeIDOld.Contains(item.ID) && item.check1)
            //            {
            //                HIS_MEST_PATIENT_TYPE patientype = new HIS_MEST_PATIENT_TYPE();
            //                patientype.MEDI_STOCK_ID = ApplicationID;
            //                patientype.PATIENT_TYPE_ID = item.ID;
            //                ListCreate.Add(patientype);
            //            }
            //        }
            //        if (ListCreate.Count != null && ListCreate.Count > 0)
            //        {
            //            var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE>("api/AcsApplicationRole/CreateList", ApiConsumers.AcsConsumer, ListCreate, param);
            //            success = true;

            //        }

            //        else if (ListDelete.Count > 0)
            //        {
            //            var resultData = new BackendAdapter(param).Post<bool>("api/AcsApplicationRole/DeleteList", ApiConsumers.AcsConsumer, ListDelete, param);
            //            if (resultData)
            //            {
            //                success = true;
            //            }
            //        }
            //        else if (ListCreate.Count == 0 && ListDelete.Count == 0)
            //        {
            //            WaitingManager.Hide();
            //            MessageBox.Show("Chưa có thay đổi", "thông báo");

            //            return;
            //        }
            //        if (success)
            //        {
            //            BackendDataWorker.Reset<HIS_MEST_PATIENT_TYPE>();
            //            RefeshGridRole(ApplicationID);
            //        }
            //    }
            //    else
            //    {
            //        List<HIS_MEST_PATIENT_TYPE> AcsApplicationRole = BackendDataWorker.Get<HIS_MEST_PATIENT_TYPE>();
            //        List<long> ListApplicationIDOld = new List<long>();
            //        foreach (var item in AcsApplicationRole)
            //        {
            //            if (item.PATIENT_TYPE_ID == PatientID)
            //            {
            //                ListApplicationIDOld.Add(item.MEDI_STOCK_ID);
            //            }
            //        }
            //        foreach (var item in ListApplicationADO)
            //        {
            //            if (ListApplicationIDOld.Contains(item.ID) && !item.check1)
            //            {
            //                var x = from item1 in AcsApplicationRole
            //                        where item1.PATIENT_TYPE_ID == PatientID && item1.MEDI_STOCK_ID == item.ID
            //                        select item1;
            //                HIS_MEST_PATIENT_TYPE o = x.FirstOrDefault();
            //                long ID = o.ID;
            //                ListDelete.Add(ID);
            //            }
            //            if (!ListApplicationIDOld.Contains(item.ID) && item.check1)
            //            {
            //                HIS_MEST_PATIENT_TYPE patientype = new HIS_MEST_PATIENT_TYPE();
            //                patientype.MEDI_STOCK_ID = item.ID;
            //                patientype.PATIENT_TYPE_ID = PatientID;
            //                ListCreate.Add(patientype);
            //            }
            //        }
            //        if (ListCreate.Count != null && ListCreate.Count > 0)
            //        {
            //            var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MEST_PATIENT_TYPE>("api/AcsApplicationRole/CreateList", ApiConsumers.AcsConsumer, ListCreate, param);
            //            success = true;

            //        }

            //        else if (ListDelete.Count > 0)
            //        {
            //            var resultData = new BackendAdapter(param).Post<bool>("api/AcsApplicationRole/DeleteList", ApiConsumers.AcsConsumer, ListDelete, param);
            //            if (resultData)
            //            {
            //                success = true;
            //            }
            //        }
            //        else if (ListCreate.Count == 0 && ListDelete.Count == 0)
            //        {
            //            WaitingManager.Hide();
            //            MessageBox.Show("Chưa có thay đổi", "thông báo");

            //            return;
            //        }
            //        if (success)
            //        {
            //            BackendDataWorker.Reset<HIS_MEST_PATIENT_TYPE>();
            //            RefeshGridApplication(PatientID);
            //        }
            //    }
            //    WaitingManager.Hide();
            //    #region Hien thi message thong bao
            //    MessageManager.Show(this.ParentForm, param, success);
            //    #endregion

            //    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
            //    SessionManager.ProcessTokenLost(param);
            //    #endregion

            //}
            //catch (Exception ex)
            //{
            //    WaitingManager.Hide();
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
            #endregion
        }

        private void gridView1_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                ApplicationADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (ApplicationADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "grclCheckForRole")
                    {
                        if (cboChoose.EditValue != "Ứng dụng")
                        {
                            e.RepositoryItem = chkForRole;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemCheckEdit1;
                        }
                    }
                    else if (e.Column.FieldName == "grclcheckForApplication")
                    {
                        e.RepositoryItem = chkForApplication;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ApplicationADO pData = (ApplicationADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "grclCheckForRole")
                    {
                        e.Value = pData.CheckForRole;
                    }
                    else if (e.Column.FieldName == "grclCheckForApplication")
                    {
                        e.Value = pData.CheckForApplication;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    RoleADO pData = (RoleADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + start1; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "check22")
                    {
                        e.Value = pData.checkForRole;
                    }
                    else if (e.Column.FieldName == "check21")
                    {
                        e.Value = pData.checkForApplication;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void check12_Click(object sender, EventArgs e)
        {
            var row = (ApplicationADO)grViewModule.GetFocusedRow();
            if (row.CheckForApplication == true)
            {
                row.CheckForApplication = false;
                ApplicationID = 0;
                FilldataToGridRole();
                return;
            }
            foreach (var item in ListApplicationADO)
            {
                item.CheckForApplication = false;
                if (item.ID == row.ID)
                {
                    ApplicationID = row.ID;
                    item.CheckForApplication = true;
                }
            }
            gridControl1.DataSource = ListApplicationADO;
            gridControl1.RefreshDataSource();
            RefeshGridRole(ApplicationID);
        }

        private void RefeshGridRole(long ApplicationID)
        {
            List<long> listRoleID = new List<long>();
            LoadAcsApplicationRole();
            foreach (var item in ListModuleRole)
            {
                if (item.APPLICATION_ID == ApplicationID)
                {
                    listRoleID.Add(item.ROLE_ID);
                }
            }
            foreach (var item in ListRoleADO)
            {
                item.checkForApplication = false;
                foreach (var item1 in listRoleID)
                {
                    if (item.ID == item1)
                    {
                        item.checkForApplication = true;
                    }
                }
            }
            var data = ListRoleADO.OrderByDescending(o => o.checkForApplication);
            gridControl2.DataSource = null;
            gridControl2.DataSource = data;
            gridControl2.RefreshDataSource();
        }

        private void check21_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (RoleADO)gridView2.GetFocusedRow();
                if (row.checkForApplication)
                {
                    row.checkForApplication = false;
                }
                else
                {
                    row.checkForApplication = true;
                }

                foreach (var item in ListRoleADO)
                {
                    if (row.ID == item.ID)
                    {
                        item.checkForApplication = row.checkForApplication;
                    }
                }
                //    gridControl2.DataSource = ListRoleADO;
                //    gridControl2.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void check22_Click(object sender, EventArgs e)
        {
            var row = (RoleADO)gridView2.GetFocusedRow();
            if (row.checkForRole == true)
            {
                row.checkForRole = false;
                RoleID = 0;
                FilldataToGridModule();
                return;
            }
            foreach (var item in ListRoleADO)
            {
                item.checkForRole = false;
                if (item.ID == row.ID)
                {
                    RoleID = row.ID;
                    item.checkForRole = true;
                }
            }
            gridControl2.DataSource = ListRoleADO;
            gridControl2.RefreshDataSource();
            RefeshGridApplication(RoleID);
        }

        private void RefeshGridApplication(long RoleID)
        {
            LoadAcsApplicationRole();
            List<long> ListApplicationID = new List<long>();
            foreach (var item in ListModuleRole)
            {
                if (item.ROLE_ID == RoleID)
                {
                    ListApplicationID.Add(item.APPLICATION_ID);
                }
            }
            foreach (var item in ListApplicationADO)
            {
                item.CheckForRole = false;
                foreach (var item1 in ListApplicationID)
                {
                    if (item.ID == item1)
                    {
                        item.CheckForRole = true;
                    }
                }
            }
            //var data = 
            // gridControl1.DataSource = null;
            gridControl1.DataSource = ListApplicationADO.OrderByDescending(o => o.CheckForRole);
            gridControl1.RefreshDataSource();
        }

        private void check11_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (ApplicationADO)grViewModule.GetFocusedRow();
                if (row.CheckForRole)
                {
                    row.CheckForRole = false;
                }
                else
                {
                    row.CheckForRole = true;
                }

                foreach (var item in ListApplicationADO)
                {
                    if (row.ID == item.ID)
                    {
                        item.CheckForRole = row.CheckForRole;
                    }
                }
                //    gridControl1.DataSource = ListApplicationADO;
                //    gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControl2_Click(object sender, EventArgs e)
        {

        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (cboChoose.EditValue == "Ứng dụng")
                {
                    return;
                }
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check11")
                        {
                            if (grclCheckForRole.Image == imgStock.Images[1])
                            {
                                grclCheckForRole.Image = imgStock.Images[0];
                                foreach (var item in ListApplicationADO)
                                {
                                    item.CheckForRole = true;
                                }
                            }
                            else
                            {
                                grclCheckForRole.Image = imgStock.Images[1];
                                foreach (var item in ListApplicationADO)
                                {
                                    item.CheckForRole = false;
                                }
                            }
                            gridControl1.RefreshDataSource();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "check21")
                    {
                        if (cboChoose.EditValue == "Ứng dụng")
                        {
                            e.RepositoryItem = check21;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemCheckEdit2;
                        }
                    }
                    else if (e.Column.FieldName == "check22")
                    {
                        e.RepositoryItem = check22;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView2_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (cboChoose.EditValue != "Ứng dụng")
                {
                    return;
                }
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "check21")
                        {
                            if (grclcheck21.Image == imgPatient.Images[1])
                            {
                                grclcheck21.Image = imgPatient.Images[0];
                                foreach (var item in ListRoleADO)
                                {
                                    item.checkForApplication = true;
                                }
                            }
                            else
                            {
                                grclcheck21.Image = imgPatient.Images[1];
                                foreach (var item in ListRoleADO)
                                {
                                    item.checkForApplication = false;
                                }
                            }
                            gridControl2.RefreshDataSource();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FilldataToGridModule();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if (e.KeyCode == Keys.Enter)
                {
                    FilldataToGridRole();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
