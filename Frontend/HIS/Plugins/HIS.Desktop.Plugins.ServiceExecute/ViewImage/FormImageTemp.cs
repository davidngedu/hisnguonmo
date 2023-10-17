﻿using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute.ViewImage
{
    public partial class FormImageTemp : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module Module;
        private Action<List<HIS_TEXT_LIB>> SelectImageId;
        private int rowCount = 0;
        private int dataTotal = 0;
        private int startPage = 0;
        private long? DeparmentId = null;

        public FormImageTemp(Module _module, Action<List<HIS_TEXT_LIB>> selectImageId)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this.Module = _module;
                this.SelectImageId = selectImageId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormImageTemp_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                this.DeparmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.Module.RoomId).DepartmentId;

                FillDataToGrid();

                txtKeyWord.Focus();
                txtKeyWord.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(GridPaging, param, pagingSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_TEXT_LIB>> apiResult = null;
                MOS.Filter.HisTextLibFilter filter = new MOS.Filter.HisTextLibFilter();
                SetFilter(ref filter);
                gridViewImage.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_TEXT_LIB>>
                    ("api/HisTextLib/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlImage.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlImage.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewImage.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridViewImage.EndUpdate();
            }
        }

        private void SetFilter(ref MOS.Filter.HisTextLibFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                filter.CAN_VIEW = true;
                filter.PUBLIC_DEPARTMENT_ID = this.DeparmentId;
                filter.LIB_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_LIB_TYPE.ID__IMAGE;
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<HIS_TEXT_LIB> imageLib = new List<HIS_TEXT_LIB>();
                var rowHandles = gridViewImage.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var row = (HIS_TEXT_LIB)gridViewImage.GetRow(i);
                        if (row != null)
                        {
                            imageLib.Add(row);
                        }
                    }
                }

                if (imageLib.Count <= 0 && MessageBox.Show("Bạn chưa chọn lược đồ. Bạn có muốn tiếp tục không?", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                if (this.SelectImageId != null)
                {
                    this.SelectImageId(imageLib);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSearch_Click(null, null);
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void gridViewImage_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                var row = (HIS_TEXT_LIB)gridViewImage.GetFocusedRow();
                if (row != null)
                {
                    string base64Data = Encoding.UTF8.GetString(row.CONTENT);

                    byte[] imageBytes = Convert.FromBase64String(base64Data);
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        this.pictureData.Image = System.Drawing.Image.FromStream(ms, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImage_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    var data = (HIS_TEXT_LIB)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện FormImageTemp
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourceFormImageTemp = new ResourceManager("HIS.Desktop.Plugins.ServiceExecute.Resources.Lang", typeof(FormImageTemp).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormImageTemp.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResourceFormImageTemp, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("FormImageTemp.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResourceFormImageTemp, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormImageTemp.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResourceFormImageTemp, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormImageTemp.btnSave.Text", Resources.ResourceLanguageManager.LanguageResourceFormImageTemp, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("FormImageTemp.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResourceFormImageTemp, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("FormImageTemp.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResourceFormImageTemp, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormImageTemp.bar1.Text", Resources.ResourceLanguageManager.LanguageResourceFormImageTemp, LanguageManager.GetCulture());
                this.barBtnSearch.Caption = Inventec.Common.Resource.Get.Value("FormImageTemp.barBtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResourceFormImageTemp, LanguageManager.GetCulture());
                this.barBtnSave.Caption = Inventec.Common.Resource.Get.Value("FormImageTemp.barBtnSave.Caption", Resources.ResourceLanguageManager.LanguageResourceFormImageTemp, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormImageTemp.Text", Resources.ResourceLanguageManager.LanguageResourceFormImageTemp, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                DeparmentId = null;
                startPage = 0;
                dataTotal = 0;
                rowCount = 0;
                SelectImageId = null;
                Module = null;
                this.btnSearch.Click -= new System.EventHandler(this.btnSearch_Click);
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.gridViewImage.RowClick -= new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gridViewImage_RowClick);
                this.gridViewImage.CustomUnboundColumnData -= new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewImage_CustomUnboundColumnData);
                this.barBtnSearch.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSearch_ItemClick);
                this.barBtnSave.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSave_ItemClick);
                this.Load -= new System.EventHandler(this.FormImageTemp_Load);
                gridViewImage.GridControl.DataSource = null;
                gridControlImage.DataSource = null;
                layoutControlItem6 = null;
                ucPaging1 = null;
                barBtnSave = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barBtnSearch = null;
                bar1 = null;
                barManager1 = null;
                gridColumn3 = null;
                gridColumn1 = null;
                layoutControlItem5 = null;
                layoutControlItem4 = null;
                layoutControlItem3 = null;
                pictureData = null;
                txtKeyWord = null;
                btnSearch = null;
                emptySpaceItem1 = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                gridViewImage = null;
                gridControlImage = null;
                btnSave = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
