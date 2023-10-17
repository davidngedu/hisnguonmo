﻿using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisImportBed.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportBed
{
    public partial class frmImportBed : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module _Module { get; set; }
        RefeshReference delegateRefresh;
        List<BedADO> _BedAdos;
        List<BedADO> _CurrentAdos;
        List<HIS_BED> _ListBeds { get; set; }

        public frmImportBed()
        {
            InitializeComponent();
        }

        public frmImportBed(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmImportBed(Inventec.Desktop.Common.Modules.Module _module, RefeshReference _delegateRefresh)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this._Module = _module;
                this.delegateRefresh = _delegateRefresh;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmImportBed_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._Module != null)
                {
                    this.Text = this._Module.text;
                }
                SetIcon();
                LoadDataBed();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataBed()
        {
            try
            {
                _ListBeds = new List<HIS_BED>();
                MOS.Filter.HisBedFilter filter = new MOS.Filter.HisBedFilter();
                _ListBeds = new BackendAdapter(new CommonParam()).Get<List<HIS_BED>>("api/HisBed/Get", ApiConsumers.MosConsumer, filter, null);
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
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDownLoadFile_Click(object sender, EventArgs e)
        {
            try
            {

                string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Imp\\", "IMPORT_BED.xlsx");
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                param.Messages = new List<string>();
                if (File.Exists(fileName))
                {
                    saveFileDialog.Title = "Save File";
                    saveFileDialog.FileName = "IMPORT_BED";
                    saveFileDialog.DefaultExt = "xlsx";
                    saveFileDialog.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(fileName, saveFileDialog.FileName);
                        MessageManager.Show(this.ParentForm, param, true);
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            try
            {
                btnShowLineError.Text = "Dòng lỗi";

                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();

                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var hisServiceImport = import.GetWithCheck<BedADO>(0);
                        if (hisServiceImport != null && hisServiceImport.Count > 0)
                        {
                            List<BedADO> listAfterRemove = new List<BedADO>();


                            foreach (var item in hisServiceImport)
                            {
                                bool checkNull = string.IsNullOrEmpty(item.BED_CODE)
                                    && string.IsNullOrEmpty(item.BED_NAME)
                                    && string.IsNullOrEmpty(item.BED_ROOM_CODE)
                                    && string.IsNullOrEmpty(item.BED_TYPE_CODE)
                                    ;

                                if (!checkNull)
                                {
                                    listAfterRemove.Add(item);
                                }
                            }
                            WaitingManager.Hide();

                            this._CurrentAdos = listAfterRemove;

                            if (this._CurrentAdos != null && this._CurrentAdos.Count > 0)
                            {
                                btnShowLineError.Enabled = true;
                                this._BedAdos = new List<BedADO>();

                                Inventec.Common.Logging.LogSystem.Debug("+++++++++++++++ lần 1 ++++++++++" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._BedAdos), this._BedAdos));
                                addServiceToProcessList(_CurrentAdos, ref this._BedAdos);
                                Inventec.Common.Logging.LogSystem.Debug("+++++++++++++++ lần 2 ++++++++++" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this._BedAdos), this._BedAdos));
                                SetDataSource(this._BedAdos);
                            }

                            //btnSave.Enabled = true;
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Import thất bại");
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không đọc được file");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_service"></param>
        /// <param name="_bedRoomRef"></param>
        private void addServiceToProcessList(List<BedADO> _service, ref List<BedADO> _bedRoomRef)
        {
            try
            {
                _bedRoomRef = new List<BedADO>();
                long i = 0;
                foreach (var item in _service)
                {
                    i++;
                    string error = "";
                    var serAdo = new BedADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<BedADO>(serAdo, item);

                    if (!string.IsNullOrEmpty(item.BED_CODE))
                    {
                        if (item.BED_CODE.Length > 10)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã giường");
                        }
                        serAdo.BED_CODE = item.BED_CODE;
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã giường");
                    }
                    #region Đã tồn tại mã giường
                    //  Đã tồn tại mã giường
                    if (!string.IsNullOrEmpty(item.BED_CODE) && !string.IsNullOrEmpty(item.BED_ROOM_CODE))
                    {
                        var HisbedRoom = BackendDataWorker.Get<HIS_BED_ROOM>().FirstOrDefault(p => p.BED_ROOM_CODE == item.BED_ROOM_CODE);
                        if (HisbedRoom != null)
                        {
                            var bed = this._ListBeds.FirstOrDefault(p => p.BED_CODE == item.BED_CODE && p.BED_ROOM_ID == HisbedRoom.ID);
                            if (bed != null)
                            {
                                error += string.Format(Message.MessageImport.DaTonTai, "Mã giường");
                            }
                        }
                        
                    }
                    if (!string.IsNullOrEmpty(item.BED_NAME) && !string.IsNullOrEmpty(item.BED_ROOM_CODE))
                    {
                        var HisbedRoom = BackendDataWorker.Get<HIS_BED_ROOM>().FirstOrDefault(p => p.BED_ROOM_CODE == item.BED_ROOM_CODE);
                        if (HisbedRoom != null) 
                        {
                            var bed = this._ListBeds.FirstOrDefault(p => p.BED_NAME == item.BED_NAME && p.BED_ROOM_ID == HisbedRoom.ID);
                            if (bed != null)
                            {
                                error += string.Format(Message.MessageImport.DaTonTai, "Tên giường");
                            }
                        }
                       
                    }
                    if (!string.IsNullOrEmpty(item.BED_NAME) && !string.IsNullOrEmpty(item.BED_TYPE_CODE))
                    {
                        var Hisbedtype = BackendDataWorker.Get<HIS_BED_TYPE>().FirstOrDefault(p => p.BED_TYPE_CODE == item.BED_TYPE_CODE);
                        if (Hisbedtype != null) 
                        
                        {
                            var bed = this._ListBeds.FirstOrDefault(p => p.BED_NAME == item.BED_NAME && p.BED_ROOM_ID == Hisbedtype.ID);
                            if (bed != null)
                            {
                                error += string.Format(Message.MessageImport.DaTonTaiLoaiGiuong, "Tên giường");
                            }
                        }
                        
                    }
                    // Đã tồn tại mã giường

                    #endregion 
                    var checkTrung12 = _service.Where(p => p.BED_CODE == item.BED_CODE && p.BED_NAME == item.BED_NAME && p.BED_ROOM_CODE == item.BED_ROOM_CODE && p.BED_TYPE_CODE == item.BED_TYPE_CODE && p.MAX_CAPACITY_STR == item.MAX_CAPACITY_STR && p.X == item.Y).ToList();
                    if (checkTrung12 != null && checkTrung12.Count > 1)
                    {
                        error += string.Format(Message.MessageImport.FileImportDaTonTai, item.BED_CODE);
                    }



                    if (!string.IsNullOrEmpty(item.BED_NAME))
                    {
                        if (item.BED_NAME.Length > 100)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Tên giường");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Tên giường");
                    }
                    if (!string.IsNullOrEmpty(item.BED_ROOM_CODE))
                    {
                        var bedRoom = BackendDataWorker.Get<HIS_BED_ROOM>().FirstOrDefault(p => p.BED_ROOM_CODE == item.BED_ROOM_CODE.Trim());
                        if (bedRoom != null)
                        {
                            if (bedRoom.IS_ACTIVE == 1)
                            {
                                serAdo.BED_ROOM_ID = bedRoom.ID;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.MaBuongDaKhoa, "Mã buồng");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã buồng");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã buồng");
                    }

                    if (!string.IsNullOrEmpty(item.BED_TYPE_CODE))
                    {
                        var bedType = BackendDataWorker.Get<HIS_BED_TYPE>().FirstOrDefault(p => p.BED_TYPE_CODE == item.BED_TYPE_CODE.Trim());
                        if (bedType != null)
                        {
                            if (bedType.IS_ACTIVE == 1)
                            {
                                serAdo.BED_TYPE_ID = bedType.ID;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.MaLoaiGiuongDaKhoa, "Mã loại giường");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã loại giường");
                        }
                    }
                    else
                    {
                        error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã loại giường");
                    }
                    
                
                    if (!string.IsNullOrEmpty(item.MAX_CAPACITY_STR))
                    {
                        long MaxCapacity = 0;
                        if (Int64.TryParse(item.MAX_CAPACITY_STR, out MaxCapacity))
                        {
                            serAdo.MAX_CAPACITY = MaxCapacity;
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Sức chứa tối đa");
                        }
                    }
                    if (!string.IsNullOrEmpty(item.TREATMENT_ROOM_CODE))
                    {

                        if (item.TREATMENT_ROOM_CODE.Length > 20)
                        {
                            error += string.Format(Message.MessageImport.Maxlength, "Mã phòng điều trị");
                        }
                        //CommonParam paramcommon = new CommonParam();
                        //HisTreatmentRoomFilter filter = new HisTreatmentRoomFilter();
                        //filter.ORDER_DIRECTION = "DESC";
                        //filter.ORDER_FIELD = "MODIFY_TIME";
                        //var histreament = new BackendAdapter(paramcommon).Get<List<HIS_TREATMENT_ROOM>>("api/HisTreatmentRoom/Get", ApiConsumers.MosConsumer, filter, paramcommon);
                        //var histreamentID = histreament.FirstOrDefault(p => p.TREATMENT_ROOM_CODE == item.TREATMENT_ROOM_CODE.Trim());

                        var histreamentID = BackendDataWorker.Get<HIS_TREATMENT_ROOM>().FirstOrDefault(p => p.TREATMENT_ROOM_CODE == item.TREATMENT_ROOM_CODE.Trim());

                        Inventec.Common.Logging.LogSystem.Debug("+++++++++++++++ item.TREATMENT_ROOM_CODE.Trim() ++++++++++" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item.TREATMENT_ROOM_CODE.Trim()), item.TREATMENT_ROOM_CODE.Trim()));
                        Inventec.Common.Logging.LogSystem.Debug("+++++++++++++++ histreamentID ++++++++++" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => histreamentID), histreamentID));
                        if (histreamentID != null)
                        {
                            if (histreamentID.IS_ACTIVE == 1)
                            {
                                serAdo.TREATMENT_ROOM_ID = histreamentID.ID;
                            }
                            else
                            {
                                error += string.Format(Message.MessageImport.MaPhongDieuTri, "Mã phòng điều trị");
                            }
                        }
                        else
                        {
                            error += string.Format(Message.MessageImport.KhongHopLe, "Mã phòng điều trị");
                        }
                    }
                    //else
                    //{
                    //    error += string.Format(Message.MessageImport.ThieuTruongDL, "Mã phòng điều trị");
                    //}
                    serAdo.ERROR = error;
                    serAdo.ID = i;
                    _bedRoomRef.Add(serAdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDataSource(List<BedADO> dataSource)
        {
            try
            {
                gridControlData.BeginUpdate();
                gridControlData.DataSource = null;
                gridControlData.DataSource = dataSource;
                gridControlData.EndUpdate();
                CheckErrorLine(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine(List<BedADO> dataSource)
        {
            try
            {
                var checkError = this._BedAdos.Exists(o => !string.IsNullOrEmpty(o.ERROR));
                if (!checkError)
                {
                    btnImport.Enabled = true;
                    btnShowLineError.Enabled = false;
                }
                else
                {
                    btnShowLineError.Enabled = true;
                    btnImport.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnShowLineError.Text == "Dòng lỗi")
                {
                    btnShowLineError.Text = "Dòng không lỗi";
                    var errorLine = this._BedAdos.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);

                }
                else
                {
                    btnShowLineError.Text = "Dòng lỗi";
                    var errorLine = this._BedAdos.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(errorLine);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton_ER_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (BedADO)gridViewData.GetFocusedRow();
                if (row != null && !string.IsNullOrEmpty(row.ERROR))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR, "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (BedADO)gridViewData.GetFocusedRow();
                if (row != null)
                {
                    if (this._BedAdos != null && this._BedAdos.Count > 0)
                    {
                        this._BedAdos.Remove(row);
                        var dataCheck = this._BedAdos.Where(p => p.BED_CODE == row.BED_CODE && p.BED_NAME == row.BED_NAME && p.BED_ROOM_CODE == row.BED_ROOM_CODE && p.BED_TYPE_CODE == row.BED_TYPE_CODE && p.MAX_CAPACITY_STR == row.MAX_CAPACITY_STR && p.X == row.Y).ToList();
                        if (dataCheck != null && dataCheck.Count == 1)
                        {
                            if (!string.IsNullOrEmpty(dataCheck[0].ERROR))
                            {
                                string erro = string.Format(Message.MessageImport.FileImportDaTonTai, dataCheck[0].BED_CODE);
                                //string[] Codes = dataCheck[0].ERROR.Split('|');
                                dataCheck[0].ERROR = dataCheck[0].ERROR.Replace(erro, "");
                            }

                        }
                        SetDataSource(this._BedAdos);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    BedADO pData = (BedADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                WaitingManager.Show();
                List<HIS_BED> datas = new List<HIS_BED>();

                if (this._BedAdos != null && this._BedAdos.Count > 0)
                {
                    foreach (var item in this._BedAdos)
                    {
                        HIS_BED ado = new HIS_BED();
                        ado.BED_CODE = item.BED_CODE;
                        ado.BED_NAME = item.BED_NAME;
                        ado.BED_ROOM_ID = item.BED_ROOM_ID;
                        ado.BED_TYPE_ID = item.BED_TYPE_ID;
                        ado.MAX_CAPACITY = item.MAX_CAPACITY;
                        ado.TREATMENT_ROOM_ID = item.TREATMENT_ROOM_ID;
                        ado.X = item.X;
                        ado.Y = item.Y;
                        datas.Add(ado);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    return;
                }

                CommonParam param = new CommonParam();
                var dataImports = new BackendAdapter(param).Post<List<HIS_BED>>("api/HisBed/CreateList", ApiConsumers.MosConsumer, datas, param);
                WaitingManager.Hide();
                if (dataImports != null && dataImports.Count > 0)
                {
                    success = true;
                    btnImport.Enabled = false;
                    LoadDataBed();
                    if (this.delegateRefresh != null)
                    {
                        this.delegateRefresh();
                    }
                }

                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnImport.Enabled)
                    btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewData_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    //BedADO data = (BedADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    string error = (gridViewData.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString();
                    if (e.Column.FieldName == "ERROR_")
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            e.RepositoryItem = repositoryItemButton_ER;
                        }
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
