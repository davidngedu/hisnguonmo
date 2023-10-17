using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.Paging;
using DevExpress.XtraNavBar;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using QCS.Desktop.LibraryMessage;
using QCS.Filter;
using Inventec.Core;
using Inventec.Common.Logging;
using System.Collections;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using QCS.EFMODEL.DataModels;
using QCS.Desktop.Common;
using Inventec.Desktop.Common.LanguageManager;
using QCS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.Message;
using QCS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using QCS.Desktop.ApiConsumer;
using QCS.Desktop.Controls.Session;
using QCS.Desktop.Utilities;
using QCS.Desktop.LocalStorage.ConfigApplication;

namespace QCS.Desktop.Plugins.QcsQuery.QcsQueryList
{
    public partial class UCQcsQueryList : UserControl, IControlCallBack
    {
        #region Declare
        DevExpress.XtraNavBar.NavBarGroup navBarGroupGender;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        #endregion

        #region Construct
        public UCQcsQueryList()
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method

        void SetCaptionByLanguageKey()
        {
            try
            {
                //navTitleCreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCPATIENTLIST_NAV_TITLE_CREATE_TIME", QCS.Desktop.Resources.ResourceLanguageManager.LanguageUCPatientList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //lciFromTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCPATIENTLIST_LCI_FROM_TIME", QCS.Desktop.Resources.ResourceLanguageManager.LanguageUCPatientList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //lciToTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCPATIENTLIST_LCI_TO_TIME", QCS.Desktop.Resources.ResourceLanguageManager.LanguageUCPatientList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());                                 
                //btnSearch.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCPATIENTLIST_BTN_SEARCH", QCS.Desktop.Resources.ResourceLanguageManager.LanguageUCPatientList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //btnRefesh.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_LANGUAGEUCPATIENTLIST_BTN_REFESH", QCS.Desktop.Resources.ResourceLanguageManager.LanguageUCPatientList, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());              
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                DateTime FirstDayOfNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dtFromTime.DateTime = FirstDayOfNow;
                dtToTime.DateTime = DateTime.Now;
                txtKeyWord.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                FillDataToGridControl();

                //Gan ngon ngu
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        private void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                LoadPaging(new CommonParam(0, Convert.ToInt32(ConfigApplications.NumPageSize)));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingForQcsQuery.Init(LoadPaging, param);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Ham goi api lay du lieu phan trang
        /// </summary>
        /// <param name="param"></param>
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<QCS.EFMODEL.DataModels.V_QCS_QUERY>> apiResult = null;
                QcsQueryViewFilter filter = new QcsQueryViewFilter();
                SetFilterTimeFromToNavBar(ref filter);

                gridviewQcsQueryList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_QCS_QUERY>>(QcsRequestUriStore.QCS_QUERY_GETVIEW, ApiConsumers.QcsConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<QCS.EFMODEL.DataModels.V_QCS_QUERY>)apiResult.Data;
                    if (data != null)
                    {
                        gridviewQcsQueryList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridviewQcsQueryList.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterTimeFromToNavBar(ref QcsQueryViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                if (dtFromTime != null && dtFromTime.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtFromTime.EditValue).ToString("yyyyMMdd") + "000000");
                if (dtToTime != null && dtToTime.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtToTime.EditValue).ToString("yyyyMMdd") + "232359");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        // private void SetFilterFromGenderNavBar(NavBarControl navBarFilter, ref SdcPatientViewFilter filter)
        // {
            // try
            // {
                // if (navBarFilter.Controls.Count > 0)
                // {
                    // for (int i = 0; i < navBarFilter.Controls.Count; i++)
                    // {
                        // if (navBarFilter.Controls[i] is DevExpress.XtraNavBar.NavBarGroupControlContainer)
                        // {
                            // continue;
                        // }
                        // if (navBarFilter.Controls[i] is DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper)
                        // {
                            // var groupWrapper = navBarFilter.Controls[i] as DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper;
                            // foreach (DevExpress.XtraNavBar.NavBarGroupControlContainer group in groupWrapper.Controls)
                            // {
                                // foreach (var itemCheckEdit in group.Controls)
                                // {
                                    // if (itemCheckEdit is CheckEdit)
                                    // {
                                        // var checkEdit = itemCheckEdit as CheckEdit;
                                        // if (checkEdit.Checked)
                                        // {
                                            // if (filter.GENDER_IDs == null)
                                                // filter.GENDER_IDs = new List<long>();
                                            // filter.GENDER_IDs.Add(Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name));
                                        // }
                                    // }
                                // }
                            // }
                        // }
                    // }
                // }
            // }
            // catch (Exception ex)
            // {
                // LogSystem.Error(ex);
            // }
        // }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewQcsQueryList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    QCS.EFMODEL.DataModels.V_QCS_QUERY pData = (QCS.EFMODEL.DataModels.V_QCS_QUERY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }

                    
                    gridControlQcsQueryList.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (QCS.EFMODEL.DataModels.V_QCS_QUERY)gridviewQcsQueryList.GetFocusedRow();
                if (row != null)
                {
                    Desktop.Plugins.QcsQuery.QcsQueryUpdate.frmQcsQueryUpdate infoQcsQuery = new Desktop.Plugins.QcsQuery.QcsQueryUpdate.frmQcsQueryUpdate(row.ID, FillDataToGridControl);
                    infoQcsQuery.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (QCS.EFMODEL.DataModels.V_QCS_QUERY)gridviewQcsQueryList.GetFocusedRow();
                if (rowData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>(QcsRequestUriStore.QCS_QUERY_DELETE, ApiConsumers.QcsConsumer, rowData.ID, param);
                    if (success)
                    {
                        FillDataToGridControl();
                    }
                    MessageManager.Show(param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Public method
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
                btnRefesh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        
    }
}

