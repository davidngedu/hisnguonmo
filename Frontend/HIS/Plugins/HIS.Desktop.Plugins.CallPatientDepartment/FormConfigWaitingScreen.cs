﻿using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.CallPatientDepartment
{
    public partial class FormConfigWaitingScreen : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private System.Globalization.CultureInfo cultureLang;
        private const string frmWaitingScreenStr = "FormWaitingScreen99";
        private List<ADO.RoomADO> ListRoom = new List<ADO.RoomADO>();
        bool IsLoad = false;

        public FormConfigWaitingScreen()
        {
            InitializeComponent();
        }

        public FormConfigWaitingScreen(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            FormCollection fc = Application.OpenForms;
            foreach (Form frm in fc)
            {
                if (frm.Name == frmWaitingScreenStr)
                {
                    this.Close();
                    return;
                }
            }
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                this.Text = moduleData.text;
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormConfigWaitingScreen_Load(object sender, EventArgs e)
        {
            try
            {
                IsLoad = true;
                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                LoadDataToGrid();

                if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                {
                    for (int i = 0; i < Application.OpenForms.Count; i++)
                    {
                        Form f = Application.OpenForms[i];
                        if (f.Name == frmWaitingScreenStr)
                        {
                            tgExtendMonitor.IsOn = true;
                        }
                    }
                }
                IsLoad = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGrid()
        {
            try
            {
                if (moduleData != null && moduleData.RoomId > 0)
                {
                    var currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == moduleData.RoomId);
                    if (currentRoom != null)
                    {
                        LblRoom.Text = (currentRoom.ROOM_NAME + " (" + currentRoom.DEPARTMENT_NAME + ")").ToUpper();
                        ListRoom = new List<ADO.RoomADO>();
                        var lstRoomDepartment = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o => o.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID && o.IS_ACTIVE == (short) 1).ToList();
                        var lstBedRoom = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.DEPARTMENT_ID == currentRoom.DEPARTMENT_ID && o.IS_ACTIVE == (short)1).ToList();
                        if (lstRoomDepartment != null)
                        {
                            int index=0;
                            if (lstBedRoom != null)
                            {
                                foreach (var item in lstBedRoom)
                                {
                                    ADO.RoomADO ado = new ADO.RoomADO();

                                    ado.ROOM_CODE = item.BED_ROOM_CODE;
                                    ado.ROOM_NAME = item.BED_ROOM_NAME;
                                    ado.ID = item.ROOM_ID;

                                    if (item.ROOM_ID == currentRoom.ID)
                                    {
                                        ado.IsCheck = true;
                                        ado.CategoryChoose = 1;
                                        ListRoom.Insert(index, ado);
                                        index++;
                                    }
                                    else
                                    {
                                        ListRoom.Add(ado);
                                    }
                                }
                            }
                            foreach (var item in lstRoomDepartment)
                            {
                                ADO.RoomADO ado = new ADO.RoomADO();

                                ado.ROOM_CODE = item.EXECUTE_ROOM_CODE;
                                ado.ROOM_NAME = item.EXECUTE_ROOM_NAME;
                                ado.ID = item.ROOM_ID;
                                if (item.ROOM_ID == currentRoom.ID)
                                {
                                    ado.IsCheck = true;
                                    ado.CategoryChoose = 2;
                                    ListRoom.Insert(index, ado);
                                    index++;
                                }
                                else
                                {
                                    ListRoom.Add(ado);
                                }
                            }
                        }

                    }
                    else
                    {
                        LblRoom.Text = "";
                    }

                    gridControl1.BeginUpdate();
                    gridControl1.DataSource = ListRoom.Distinct();
                    gridControl1.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.tgExtendMonitor.Properties.OnText = GetLanguageControl("HIS_DESKTOP_PLUGINS__CALL_PATIENT_DEPARTMENT__TOG_EXTEND_MONITOR__ON_TEXT");
                this.tgExtendMonitor.Properties.OffText = GetLanguageControl("HIS_DESKTOP_PLUGINS__CALL_PATIENT_DEPARTMENT__TOG_EXTEND_MONITOR__OFF_TEXT");
                this.Gc_RoomCode.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS__CALL_PATIENT_DEPARTMENT__GC_ROOM_CODE");
                this.Gc_RoomName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS__CALL_PATIENT_DEPARTMENT__GC_ROOM_NAME");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageControl(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
        }

        private void repositoryItemCheckRoom_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var check = (DevExpress.XtraEditors.CheckEdit)sender;
                if (check != null && check.Checked)
                {
                    var countCheck = ListRoom.Count(c => c.IsCheck);
                    if (countCheck > 5)
                    {
                        check.Checked = false;
                        DevExpress.XtraEditors.XtraMessageBox.Show("Số phòng được chọn không vượt quá 6");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void tgExtendMonitor_Toggled(object sender, EventArgs e)
        {
            try
            {
                if (IsLoad) return;

                if (tgExtendMonitor.IsOn)
                {
                    if (ListRoom.Exists(s => s.IsCheck))
                    {
                        var lstRoom = ListRoom.Where(o => o.IsCheck).ToList();                       
                        FormWaitingScreen99 screen = new FormWaitingScreen99(lstRoom, moduleData,lstRoom.FirstOrDefault().CategoryChoose);
                        ShowFormInExtendMonitor(screen);
                        this.Close();
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn phòng/buồng");
                        tgExtendMonitor.IsOn = !tgExtendMonitor.IsOn;
                    }
                }
                else
                {
                    if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                    {
                        for (int i = 0; i < Application.OpenForms.Count; i++)
                        {
                            Form f = Application.OpenForms[i];
                            if (f.Name == frmWaitingScreenStr)
                            {
                                f.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static void ShowFormInExtendMonitor(Form control)
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
                    var sc2 = sc.FirstOrDefault(o => !o.Primary);
                    if (sc2 == null) sc2 = sc[1];

                    control.FormBorderStyle = FormBorderStyle.None;
                    control.Left = sc2.Bounds.Width;
                    control.Top = sc2.Bounds.Height;
                    control.StartPosition = FormStartPosition.Manual;
                    control.Location = sc2.Bounds.Location;
                    Point p = new Point(sc2.Bounds.Location.X, sc2.Bounds.Location.Y);
                    control.Location = p;
                    control.WindowState = FormWindowState.Maximized;
                    control.Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
