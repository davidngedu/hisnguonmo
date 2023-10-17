﻿using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using HIS.Desktop.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;

namespace HIS.Desktop.Plugins.CallPatient
{
    public class ChooseRoomForWaitingScreenProcess
    {
        const string frmWaitingScreen1 = "frmWaitingScreen1";
        const string frmWaitingScreenQy = "frmWaitingScreen_QY1";
        const string frmWaitingExam = "frmWaitingExam";
        //internal static void LoadDataToComboRoom(DevExpress.XtraEditors.GridLookUpEdit cboRoom)
        //{
        //    try
        //    {
        //        List<long> roomTypeIds = new List<long>();
        //        roomTypeIds.Add(HisRoomTypeCFG.HIS_ROOM_TYPE_ID__DV);
        //        roomTypeIds.Add(HisRoomTypeCFG.HIS_ROOM_TYPE_ID__GI);

        //        cboRoom.Properties.DataSource = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Where(o => roomTypeIds.Contains(o.ROOM_TYPE_ID) && o.ROOM_TYPE_CODE == "XL").ToList();
        //        cboRoom.Properties.DisplayMember = "ROOM_NAME";
        //        cboRoom.Properties.ValueMember = "ID";

        //        cboRoom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
        //        cboRoom.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
        //        cboRoom.Properties.ImmediatePopup = true;
        //        cboRoom.ForceInitialize();
        //        cboRoom.Properties.View.Columns.Clear();

        //        GridColumn aColumnCode = cboRoom.Properties.View.Columns.AddField("ROOM_CODE");
        //        aColumnCode.Caption = "Mã";
        //        aColumnCode.Visible = true;
        //        aColumnCode.VisibleIndex = 1;
        //        aColumnCode.Width = 100;

        //        GridColumn aColumnName = cboRoom.Properties.View.Columns.AddField("ROOM_NAME");
        //        aColumnName.Caption = "Tên";
        //        aColumnName.Visible = true;
        //        aColumnName.VisibleIndex = 2;
        //        aColumnName.Width = 300;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //internal static void LoadRoomCombo(string searchCode, bool isExpand, frmChooseRoomForWaitingScreen control)
        //{
        //    try
        //    {
        //        if (String.IsNullOrEmpty(searchCode))
        //        {
        //            control.cboRoom.Properties.Buttons[1].Visible = false;
        //            control.cboRoom.EditValue = null;
        //            control.cboRoom.Focus();
        //            control.cboRoom.ShowPopup();
        //            //PopupProcess.SelectFirstRowPopup(control.cboRoom);
        //        }
        //        else
        //        {
        //            var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Where(o => o.ROOM_TYPE_CODE.Contains(searchCode)).ToList();
        //            if (data != null)
        //            {
        //                if (data.Count == 1)
        //                {
        //                    control.cboRoom.Properties.Buttons[1].Visible = true;
        //                    control.cboRoom.EditValue = data[0].ID;
        //                    control.txtRoomCode.Text = data[0].ROOM_TYPE_CODE;
        //                    control.txtRoomCode.Focus();
        //                    control.txtRoomCode.SelectAll();
        //                }
        //                else
        //                {
        //                    var search = data.FirstOrDefault(m => m.ROOM_TYPE_CODE == searchCode);
        //                    if (search != null)
        //                    {
        //                        control.cboRoom.Properties.Buttons[1].Visible = true;
        //                        control.cboRoom.EditValue = search.ID;
        //                        control.txtRoomCode.Text = search.ROOM_TYPE_CODE;
        //                        control.txtRoomCode.Focus();
        //                        control.txtRoomCode.SelectAll();
        //                    }
        //                    else
        //                    {
        //                        control.cboRoom.Properties.Buttons[1].Visible = false;
        //                        control.cboRoom.EditValue = null;
        //                        control.cboRoom.Focus();
        //                        control.cboRoom.ShowPopup();
        //                        //PopupProcess.SelectFirstRowPopup(control.cboRoom);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        internal static void ShowFormInExtendMonitor(frmWaitingScreen1 control)
        {
            try
            {
                Screen[] sc;
                sc = Screen.AllScreens;
                if (sc.Length <= 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy màn hình mở rộng");
                    control.Show();
                }
                else
                {
                    control.FormBorderStyle = FormBorderStyle.None;
                    control.Left = sc[1].Bounds.Width;
                    control.Top = sc[1].Bounds.Height;
                    control.StartPosition = FormStartPosition.Manual;
                    control.Location = sc[1].Bounds.Location;
                    Point p = new Point(sc[1].Bounds.Location.X, sc[1].Bounds.Location.Y);
                    control.Location = p;
                    control.WindowState = FormWindowState.Maximized;
                    control.Show();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        internal static void ShowFormInExtendMonitor(frmWaitingScreen_QY1 control)
        {
            try
            {
                Screen[] sc;
                sc = Screen.AllScreens;
                if (sc.Length <= 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy màn hình mở rộng");
                    control.Show();
                }
                else
                {
                    control.FormBorderStyle = FormBorderStyle.None;
                    control.Left = sc[1].Bounds.Width;
                    control.Top = sc[1].Bounds.Height;
                    control.StartPosition = FormStartPosition.Manual;
                    control.Location = sc[1].Bounds.Location;
                    Point p = new Point(sc[1].Bounds.Location.X, sc[1].Bounds.Location.Y);
                    control.Location = p;
                    control.WindowState = FormWindowState.Maximized;
                    control.Show();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        internal static void TurnOffExtendMonitor(frmWaitingScreen1 control)
        {
            try
            {
                if (control != null)
                {
                    if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                    {
                        for (int i = 0; i < Application.OpenForms.Count; i++)
                        {
                            Form f = Application.OpenForms[i];
                            if (f.Name == frmWaitingScreen1)
                            {
                                f.Close();
                            }
                            else if (f.Name == frmWaitingExam)
                            {
                                f.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        internal static void TurnOffExtendMonitor(frmWaitingScreen_QY1 control)
        {
            try
            {
                if (control != null)
                {
                    if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                    {
                        for (int i = 0; i < Application.OpenForms.Count; i++)
                        {
                            Form f = Application.OpenForms[i];
                            if (f.Name == frmWaitingScreenQy)
                            {
                                f.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        //internal static void ShowFormInExtendMonitor(frmWaitingExam control)
        //{
        //    try
        //    {
        //        Screen[] sc;
        //        sc = Screen.AllScreens;
        //        if (sc.Length <= 1)
        //        {
        //            DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy màn hình mở rộng");
        //            control.Show();
        //        }
        //        else
        //        {
        //            control.FormBorderStyle = FormBorderStyle.None;
        //            control.Left = sc[1].Bounds.Width;
        //            control.Top = sc[1].Bounds.Height;
        //            control.StartPosition = FormStartPosition.Manual;
        //            control.Location = sc[1].Bounds.Location;
        //            Point p = new Point(sc[1].Bounds.Location.X, sc[1].Bounds.Location.Y);
        //            control.Location = p;
        //            control.WindowState = FormWindowState.Maximized;
        //            control.Show();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystem.Error(ex);
        //    }
        //}
        //internal static void TurnOffExtendMonitor(frmWaitingExam control)
        //{
        //    try
        //    {
        //        if (control != null)
        //        {
        //            if (Application.OpenForms != null && Application.OpenForms.Count > 0)
        //            {
        //                for (int i = 0; i < Application.OpenForms.Count; i++)
        //                {
        //                    Form f = Application.OpenForms[i];
        //                    if (f.Name == frmWaitingScreen1)
        //                    {
        //                        f.Close();
        //                    }
        //                    else if (f.Name == frmWaitingExam)
        //                    {
        //                        f.Close();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystem.Error(ex);
        //    }
        //}
        internal static void LoadDataToExamServiceReqSttGridControl(frmChooseRoomForWaitingScreen control)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqSttFilter filter = new MOS.Filter.HisServiceReqSttFilter();
                var HisServiceReqStts = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_STT>>(HisRequestUriStore.HIS_SERVICE_REQ_STT_GET, ApiConsumers.MosConsumer, filter, param);
                //List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> HisServiceReqStts = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT>();
                List<ServiceReqSttSDO> serviceReqSttSdos = new List<ServiceReqSttSDO>();
                foreach (var item in HisServiceReqStts)
                {
                    ServiceReqSttSDO serviceReqSttSdo = new ServiceReqSttSDO();
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT, ServiceReqSttSDO>();
                    serviceReqSttSdo = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT, ServiceReqSttSDO>(item);
                    if (item.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                        serviceReqSttSdo.checkStt = true;
                    else
                        serviceReqSttSdo.checkStt = false;
                    serviceReqSttSdos.Add(serviceReqSttSdo);
                }
                control.gridControlExecuteStatus.DataSource = serviceReqSttSdos;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
