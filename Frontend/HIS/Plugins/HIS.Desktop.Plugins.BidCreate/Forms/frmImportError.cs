﻿using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BidCreate.Forms
{
    public partial class frmImportError : Form
    {
        List<ADO.MedicineTypeADO> errorList = new List<ADO.MedicineTypeADO>();

        public frmImportError(List<ADO.MedicineTypeADO> listData)
        {
            InitializeComponent();
            this.errorList = listData;
            try
            {
                string iconPath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmImportError_Load(object sender, EventArgs e)
        {
            try
            {
                gridControlErrors.BeginUpdate();
                gridControlErrors.DataSource = this.errorList;
                gridControlErrors.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewErrors_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                var data = (ADO.MedicineTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (data != null)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "TYPE_DISPLAY")
                    {
                        if (data.Type == Base.GlobalConfig.THUOC)
                        {
                            e.Value = "Thuốc";
                        }
                        else if (data.Type == Base.GlobalConfig.VATTU)
                        {
                            e.Value = "Vật tư";
                        }
                        else if (data.Type == Base.GlobalConfig.MAU)
                        {
                            e.Value = "Máu";
                        }
                        else
                        {
                            e.Value = "Không xác định";
                        }
                    }
                    else if (e.Column.FieldName == "ErrorDescription")
                    {
                        if (data.ErrorDescriptions != null && data.ErrorDescriptions.Count > 0)
                        {
                            e.Value = String.Join(",", data.ErrorDescriptions);
                        }
                    }
                    else if (e.Column.FieldName == "GV_HEIN_SERVICE_BHYT_NAME")
                    {
                        if (data.Type == Base.GlobalConfig.THUOC)
                        {
                            e.Value = data.HEIN_SERVICE_BHYT_NAME;
                        }
                        else
                        {
                            e.Value = "";
                        }

                    }
                    else if (e.Column.FieldName == "GV_PACKING_TYPE_NAME")
                    {
                        if (data.Type == Base.GlobalConfig.THUOC)
                        {
                            e.Value = data.PACKING_TYPE_NAME;
                        }
                        else
                        {
                            e.Value = "";
                        }
                    }
                    else if (e.Column.FieldName == "GV_REGISTER_NUMBER")
                    {
                        if (data.Type == Base.GlobalConfig.THUOC)
                        {
                            e.Value = data.REGISTER_NUMBER;
                        }
                        else
                        {
                            e.Value = "";
                        }
                    }
                    else if (e.Column.FieldName == "GV_BID_MATERIAL_TYPE_CODE")
                    {
                        if (data.Type == Base.GlobalConfig.VATTU)
                        {
                            e.Value = data.BID_MATERIAL_TYPE_CODE;
                        }
                        else
                        {
                            e.Value = "";
                        }
                    }
                    else if (e.Column.FieldName == "GV_BID_MATERIAL_TYPE_NAME")
                    {
                        if (data.Type == Base.GlobalConfig.VATTU)
                        {
                            e.Value = data.BID_MATERIAL_TYPE_NAME;
                        }
                        else
                        {
                            e.Value = "";
                        }
                    }
                    else if (e.Column.FieldName == "GV_JOIN_BID_MATERIAL_TYPE_CODE")
                    {
                        if (data.Type == Base.GlobalConfig.VATTU)
                        {
                            e.Value = data.JOIN_BID_MATERIAL_TYPE_CODE;
                        }
                        else
                        {
                            e.Value = "";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportExcel.Enabled) return;
                List<string> expCode = new List<string>();

                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "DanhSachLoiImportThau.xlsx");

                //chọn đường dẫn
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    //getdata
                    WaitingManager.Show();

                    if (String.IsNullOrEmpty(templateFile))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Không tìm thấy file", templateFile));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Biểu mẫu đang mở hoặc không tồn tại file template. Vui lòng kiểm tra lại. (" + templateFile + ")");
                        return;
                    }
                    this.errorList.ForEach(o => o.ErrorDesc = String.Join(",", o.ErrorDescriptions));
                    ProcessData(this.errorList, ref store);
                    WaitingManager.Hide();

                    if (store != null)
                    {
                        try
                        {
                            if (store.OutFile(saveFileDialog1.FileName))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Xuất file thành công");

                                if (MessageBox.Show("Bạn có muốn mở file?",
                                    "Thông báo", MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessData(List<ADO.MedicineTypeADO> data, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.SetCommonFunctions();
                objectTag.AddObjectData(store, "ExportData", data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                store = null;
            }
        }

        private void barBtnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnExportExcel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
