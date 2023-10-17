﻿using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisMrCheckItemType.Run
{
    partial class frmHisMrCheckItemType
    {
        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_MR_CHECK_ITEM_TYPE updateDTO = new MOS.EFMODEL.DataModels.HIS_MR_CHECK_ITEM_TYPE();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MR_CHECK_ITEM_TYPE>(HisRequestUriStore.MOS_HIS_MR_CHECK_ITEM_TYPE_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MR_CHECK_ITEM_TYPE>(HisRequestUriStore.MOS_HIS_MR_CHECK_ITEM_TYPE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_MR_CHECK_ITEM_TYPE>();
                    ResetFormData();
                    this.ActionType = GlobalVariables.ActionAdd;
                    EnableControlChanged(this.ActionType);
                    SetFocusEditor();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_MR_CHECK_ITEM_TYPE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMrCheckItemTypeFilter filter = new MOS.Filter.HisMrCheckItemTypeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MR_CHECK_ITEM_TYPE>>(HisRequestUriStore.MOS_HIS_MR_CHECK_ITEM_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_MR_CHECK_ITEM_TYPE currentDTO)
        {
            try
            {
                currentDTO.CHECK_ITEM_TYPE_NAME = txtCheckItemTypeName.Text.Trim();
                currentDTO.NUM_ORDER = spinNumOrder.EditValue != null ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(spinNumOrder.Value.ToString()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DeleteProcess()
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (MOS.EFMODEL.DataModels.HIS_MR_CHECK_ITEM_TYPE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.Filter.HisMrCheckItemTypeFilter filter = new MOS.Filter.HisMrCheckItemTypeFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MR_CHECK_ITEM_TYPE>>(HisRequestUriStore.MOS_HIS_MR_CHECK_ITEM_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        WaitingManager.Show();
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOS_HIS_MR_CHECK_ITEM_TYPE_DELETE, ApiConsumers.MosConsumer, data.ID, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_MR_CHECK_ITEM_TYPE>();
                            FillDataToGridControl();
                            currentData = ((List<MOS.EFMODEL.DataModels.HIS_MR_CHECK_ITEM_TYPE>)gridControlFormList.DataSource).FirstOrDefault();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
                    ResetFormData();
                    this.ActionType = GlobalVariables.ActionAdd;
                    EnableControlChanged(this.ActionType);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LockProcess()
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HIS_MR_CHECK_ITEM_TYPE data = (HIS_MR_CHECK_ITEM_TYPE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_MR_CHECK_ITEM_TYPE>(HisRequestUriStore.MOS_HIS_MR_CHECK_ITEM_TYPE_CHANGELOCK, ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (result != null)
                    {
                        success = true;
                        BackendDataWorker.Reset<HIS_MR_CHECK_ITEM_TYPE>();
                        FillDataToGridControl();
                    }

                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UnlockProcess()
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HIS_MR_CHECK_ITEM_TYPE data = (HIS_MR_CHECK_ITEM_TYPE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_MR_CHECK_ITEM_TYPE>(HisRequestUriStore.MOS_HIS_MR_CHECK_ITEM_TYPE_CHANGELOCK, ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (result != null)
                    {
                        success = true;
                        BackendDataWorker.Reset<HIS_MR_CHECK_ITEM_TYPE>();
                        FillDataToGridControl();
                    }

                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
