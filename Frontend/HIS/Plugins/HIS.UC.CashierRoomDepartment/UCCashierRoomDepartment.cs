﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HIS.Desktop.Plugins.CashierRoomDepartment.entity;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.Message;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.UC.Department;
using HIS.UC.CashierRoom;

namespace HIS.Desktop.Plugins.CashierRoomDepartment
{
  public partial class UCCashierRoomDepartment : UserControl
  {
    public UCCashierRoomDepartment()
    {
      InitializeComponent();
    }


    private long sChooseDepartment;
    private long sChooseCashierRoom;
    internal List<HIS.UC.Department.DepartmentADO> sLstDepartmentADO { get; set; }
    internal List<HIS.UC.CashierRoom.CashierRoomADO> sLstCashierRoomADO { get; set; }
    internal List<MOS.EFMODEL.DataModels.HIS_CARO_DEPARTMENT> sLstCaroDepartment { get; set; }
    private List<MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM> sLstCashierRoom;
    private List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT> sLstDepartment;
    private bool sCheckMouseDown;
    private HIS.UC.Department.UCDepartmentProcessor sUCDepartmentProcessor;
    private HIS.UC.CashierRoom.UCCashierRoomProcessor sUCCashierRoomProcessor;
    private long sDepartIdCheckByDepart = 0 ;
    private long sCashierIdCheckByCashier = 0;
    private UserControl sUCDepartment;
    private UserControl sUCCashierRoom;
    private int sRowCountDepartment = 0;
    private int sTotalCountDepartment = 0;
    private int sRowCountCashierRoom = 0;
    private int sTotalCountCashierRoom = 0;



    private void GridViewCashierRoomMouseDownCashierRoom(object sender, MouseEventArgs e)
    {
      try
      {
        if (sChooseCashierRoom == 1)
        {
          return;
        }
        WaitingManager.Show();

        if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
        {
          var view = sender as GridView;
          var hit = view.CalcHitInfo(e.Location);

          if (hit.HitTest == GridHitTest.Column)
          {
            if (hit.Column.FieldName == "CHECK")
            {
              var rCashierRoom = sLstCashierRoomADO;
              var lstCashierRoomADO = new List<UC.CashierRoom.CashierRoomADO>();

              if (rCashierRoom != null && rCashierRoom.Count > 0)
              {
                var checkCashierRoomNum = sLstCashierRoomADO.Where(o => o.checkCashierRoom == true).Count();
                var cashierRoomNum = rCashierRoom.Count();

                if ((checkCashierRoomNum > 0 && checkCashierRoomNum < cashierRoomNum) || cashierRoomNum == 0)
                {
                  sCheckMouseDown = true;
                  hit.Column.Image = imageCollectionCashierRoom.Images[1];
                }
                if (checkCashierRoomNum == cashierRoomNum)
                {
                  sCheckMouseDown = false;
                  hit.Column.Image = imageCollectionCashierRoom.Images[0];
                }

                foreach (var item in rCashierRoom)
                {
                  if (item.ID != null)
                  {
                    item.checkCashierRoom = sCheckMouseDown ? true : false;
                    lstCashierRoomADO.Add(item);
                  }
                  else
                  {
                    lstCashierRoomADO.Add(item);
                  }
                }
                sCheckMouseDown = sCheckMouseDown ? false : true;
              }
              sUCCashierRoomProcessor.Reload(sUCCashierRoom, lstCashierRoomADO);
            }
          }
        }
        WaitingManager.Hide();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void GridViewDepartmentMouseDownDepartment(object sender, MouseEventArgs e)
    {
      try
      {
        if (sChooseDepartment == 2)
        {
          return;
        }
        WaitingManager.Show();

        if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
        {
          var view = sender as GridView;
          var hit = view.CalcHitInfo(e.Location);

          if (hit.HitTest == GridHitTest.Column)
          {
            if (hit.Column.FieldName == "CHECK")
            {
              var lstDepartADO = sLstDepartmentADO;
              var DepartmentADO = new List<UC.Department.DepartmentADO>();

              if (lstDepartADO != null && lstDepartADO.Count > 0)
              {
                var checkDepartmentNum = sLstDepartmentADO.Where(o => o.checkDepartment == true).Count();
                var departNum = lstDepartADO.Count();
                if ((checkDepartmentNum > 0 && checkDepartmentNum < departNum) || checkDepartmentNum == 0)
                {
                  sCheckMouseDown = true;
                  hit.Column.Image = imageCollectionDepartment.Images[1];
                }

                if (checkDepartmentNum == departNum)
                {
                  sCheckMouseDown = false;
                  hit.Column.Image = imageCollectionDepartment.Images[0];
                }

                if (sCheckMouseDown)
                {
                  foreach (var item in lstDepartADO)
                  {
                    if (item.ID != null)
                    {
                      item.checkDepartment = true;
                      DepartmentADO.Add(item);
                    }
                    else
                    {
                      DepartmentADO.Add(item);
                    }
                    sCheckMouseDown = false;
                  }
                }
                else
                {
                  foreach (var item in lstDepartADO)
                  {
                    if (item.ID != null)
                    {
                      item.checkDepartment = false;
                      DepartmentADO.Add(item);
                    }
                    else
                    {
                      DepartmentADO.Add(item);
                    }
                    sCheckMouseDown = true;
                  }
                }
                sUCDepartmentProcessor.Reload(sUCDepartment, DepartmentADO);
              }
            }
          }
        }
        WaitingManager.Hide();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void BtnRadioEnableClickCashierRoom(MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM data)
    {
      try
      {
        WaitingManager.Show();
        var param = new CommonParam();
        var filter = new MOS.Filter.HisCaroDepartmentFilter();

        filter.CASHIER_ROOM_ID = data.ID;
        sCashierIdCheckByCashier = data.ID;

        sLstCaroDepartment = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_CARO_DEPARTMENT>>
          (HisRequestUriStore.HIS_CARO_DEPARTMENT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, param);
        sLstDepartmentADO = new List<DepartmentADO>();
        sLstDepartmentADO = (from c in sLstDepartment
                              select new DepartmentADO(c)).ToList();

        if (sLstCaroDepartment != null && sLstCaroDepartment.Count > 0)
        {
          foreach (var item in sLstCaroDepartment)
          {
            var check = sLstDepartmentADO.FirstOrDefault(o => o.ID == item.DEPARTMENT_ID);
            if (check != null)
            {
              check.checkDepartment = true;
            }
          }

          sLstDepartmentADO = sLstDepartmentADO.OrderByDescending(p => p.checkDepartment).ToList();

          if (sUCDepartment != null)
          {
            sUCDepartmentProcessor.Reload(sUCDepartment, sLstDepartmentADO);
          }
        }
        else
        {
          fillDataToGridDepartment(this);
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void BtnRadioEnableClickDepartment(MOS.EFMODEL.DataModels.HIS_DEPARTMENT data)
    {
      try
      {
        WaitingManager.Show();
        var param = new CommonParam();
        var filter = new MOS.Filter.HisCaroDepartmentFilter();

        filter.ID = data.ID;
        sDepartIdCheckByDepart = data.ID;

        sLstCaroDepartment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_CARO_DEPARTMENT>>
          (HisRequestUriStore.HIS_CARO_DEPARTMENT_GET, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);
        sLstCashierRoomADO = new List<UC.CashierRoom.CashierRoomADO>();
        sLstCashierRoomADO = (from c in sLstCashierRoom
                               select new UC.CashierRoom.CashierRoomADO(c)).ToList();

        if (sLstCaroDepartment != null && sLstCaroDepartment.Count > 0)
        {
          foreach (var item in sLstCaroDepartment)
          {
            var check = sLstCashierRoomADO.FirstOrDefault(o => o.ID == item.CASHIER_ROOM_ID);
            if (check != null)
            {
              check.checkCashierRoom = true;
            }
          }

          sLstCashierRoomADO = sLstCashierRoomADO.OrderByDescending(p => p.checkCashierRoom).ToList();
          if (sUCCashierRoom != null)
          {
            sUCCashierRoomProcessor.Reload(sUCCashierRoom, sLstCashierRoomADO);
          }
        }
        else
        {
          fillDataToGridCashierRoom(this);
        }
        WaitingManager.Hide();
      }
      catch (Exception ex)
      {
        WaitingManager.Hide();
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void loadPagingToGridCashierRoom(object data)
    {
      try
      {
        WaitingManager.Show();
        sLstCashierRoom = new List<MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM>();
        var start = ((CommonParam)data).Start ?? 0;
        var limit = ((CommonParam)data).Limit ?? 0;

        var param = new CommonParam(start, limit);
        var filter = new MOS.Filter.HisCashierRoomFilter();
        filter.KEY_WORD = txtCashierRoom.Text.Trim();

        if ((long)comboType.EditValue == 2)
        {
          sChooseCashierRoom = (long)comboType.EditValue;
        }
        var CashierRoom = new BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM>>
          (HisRequestUriStore.HIS_CASHIER_ROOM_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, param);

        sLstCashierRoomADO = new List<UC.CashierRoom.CashierRoomADO>();

        if (CashierRoom != null && CashierRoom.Data.Count > 0)
        {
          sLstCashierRoom = CashierRoom.Data;
          foreach (var item in sLstCashierRoom)
          {
            var CashierRoomADO = new UC.CashierRoom.CashierRoomADO(item);
            if (sChooseCashierRoom == 2)
            {
              CashierRoomADO.iskeyChooseCashierRoom = true;
            }
            sLstCashierRoomADO.Add(CashierRoomADO);
          }
        }

        if (sLstCaroDepartment != null && sLstCaroDepartment.Count > 0)
        {
          foreach (var item in sLstCaroDepartment)
          {
            var check = sLstCashierRoomADO.FirstOrDefault(o => o.ROOM_ID == item.CASHIER_ROOM_ID);
            if (check != null)
            {
              check.checkCashierRoom = true;
            }
          }
        }

        sLstCashierRoomADO = sLstCashierRoomADO.OrderByDescending(p => p.checkCashierRoom).ToList();
        if (sUCCashierRoom != null)
        {
          sUCCashierRoomProcessor.Reload(sUCCashierRoom, sLstCashierRoomADO);
        }
        sRowCountCashierRoom = (data == null ? 0 : sLstCashierRoomADO.Count);
        sTotalCountCashierRoom = (CashierRoom.Param == null ? 0 : CashierRoom.Param.Count ?? 0);
        WaitingManager.Hide();
      }
      catch (Exception ex)
      {
        WaitingManager.Hide();
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void loadPagingToGridDepartment(object data)
    {
      try
      {
        WaitingManager.Show();
        sLstDepartment = new List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>();
        var start = ((CommonParam)data).Start ?? 0;
        var limit = ((CommonParam)data).Limit ?? 0;

        var param = new CommonParam(start, limit);
        var filter = new MOS.Filter.HisDepartmentFilter();
        filter.ORDER_DIRECTION = "ASC";
        filter.KEY_WORD = txtDepartment.Text.Trim();

        if ((long)comboType.EditValue == 1)
        {
          sChooseDepartment = (long)comboType.EditValue;
        }
        var rDepartment = new BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>>
          (HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_DEPARTMENT_GET, ApiConsumer.ApiConsumers.MosConsumer, filter, param);

        sLstDepartmentADO = new List<DepartmentADO>();

        if (rDepartment != null && rDepartment.Data.Count > 0)
        {
          sLstDepartment = rDepartment.Data;
          foreach (var item in sLstDepartment)
          {
            var departmentADO = new DepartmentADO(item);
            if (sChooseDepartment == 1)
            {
              departmentADO.isKeyChooseDepartment = true;
            }

            sLstDepartmentADO.Add(departmentADO);
          }
        }

        if (sLstCaroDepartment != null && sLstCaroDepartment.Count > 0)
        {
          foreach (var item in sLstCaroDepartment)
          {
            var check = sLstDepartmentADO.FirstOrDefault(o => o.ID == item.DEPARTMENT_ID);
            if (check != null)
            {
              check.checkDepartment = true;
            }
          }
        }

        sLstDepartmentADO = sLstDepartmentADO.OrderByDescending(p => p.checkDepartment).ToList();

        if (sUCDepartment != null)
        {
          sUCDepartmentProcessor.Reload(sUCDepartment, sLstDepartmentADO);
        }
        sRowCountDepartment = (data == null ? 0 : sLstDepartmentADO.Count);
        sTotalCountDepartment = (rDepartment.Param == null ? 0 : rDepartment.Param.Count ?? 0);
        WaitingManager.Hide();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }



    private void initComboDepartment()
    {
      try
      {
        var param = new Inventec.Core.CommonParam();
        var filter = new MOS.Filter.HisDepartmentFilter();

        var result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>>
          (HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_DEPARTMENT_GET, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);

        comboDepartment.Properties.DataSource = result;
        comboDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
        comboDepartment.Properties.ValueMember = "ID";

        comboDepartment.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
        comboDepartment.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
        comboDepartment.Properties.ImmediatePopup = true;
        comboDepartment.ForceInitialize();
        comboDepartment.Properties.View.Columns.Clear();

        var aColumnCode = comboDepartment.Properties.View.Columns.AddField("DEPARTMENT_CODE");
        aColumnCode.Caption = "Mã";
        aColumnCode.Visible = true;
        aColumnCode.VisibleIndex = 1;
        aColumnCode.Width = 100;

        var aColumnName = comboDepartment.Properties.View.Columns.AddField("DEPARTMENT_NAME");
        aColumnName.Caption = "Tên";
        aColumnName.Visible = true;
        aColumnName.VisibleIndex = 2;
        aColumnName.Width = 200;
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void initComboStatus()
    {
      try
      {
        var status = new List<Status>();
        status.Add(new Status(1, "Khoa"));
        status.Add(new Status(2, "Phòng"));

        var columnInfos = new List<ColumnInfo>();
        columnInfos.Add(new ColumnInfo("statusName", string.Empty, 300, 2));
        var controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
        ControlEditorLoader.Load(comboType, status, controlEditorADO);
        comboType.EditValue = status[0].id;
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Warn(ex);
      }
    }

    private void initUcGridCashierRoom()
    {
      try
      {
        sUCCashierRoomProcessor = new UC.CashierRoom.UCCashierRoomProcessor();
        var CashierRoomADO = new UC.CashierRoom.ADO.CashierRoomInitADO();
        CashierRoomADO.ListCashierRoomColumn = new List<UC.CashierRoom.CashierRoomColumn>();
        CashierRoomADO.GridViewCashierRoomMouseDownCashierRoom = GridViewCashierRoomMouseDownCashierRoom;
        CashierRoomADO.BtnRadioEnableClickCashierRoom = BtnRadioEnableClickCashierRoom;

        var colRadio = new UC.CashierRoom.CashierRoomColumn(string.Empty, "radioCashierRoom", 30, true);
        colRadio.VisibleIndex = 0;
        colRadio.Visible = false;
        colRadio.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
        CashierRoomADO.ListCashierRoomColumn.Add(colRadio);

        var colCheck = new CashierRoomColumn(string.Empty, "checkCashierRoom", 30, true);
        colCheck.VisibleIndex = 1;
        colCheck.image = imageCollectionCashierRoom.Images[0];
        colCheck.Visible = false;
        colCheck.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
        CashierRoomADO.ListCashierRoomColumn.Add(colCheck);

        var colCode = new CashierRoomColumn("Mã phòng", "CASHIER_ROOM_CODE", 60, false);
        colCode.VisibleIndex = 2;
        CashierRoomADO.ListCashierRoomColumn.Add(colCode);

        var colName = new CashierRoomColumn("Tên phòng", "CASHIER_ROOM_NAME", 100, false);
        colName.VisibleIndex = 3;
        CashierRoomADO.ListCashierRoomColumn.Add(colName);

        sUCCashierRoom = (UserControl)sUCCashierRoomProcessor.run(CashierRoomADO);
        if (sUCCashierRoom != null)
        {
          panelCashierRoom.Controls.Add(sUCCashierRoom);
          sUCCashierRoom.Dock = DockStyle.Fill;
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void initUcGridDepartment()
    {
      try
      {
        var DepartmentInitADO = new UC.Department.ADO.DepartmentInitADO();
        DepartmentInitADO.ListDepartmentColumn = new List<UC.Department.DepartmentColumn>();
        DepartmentInitADO.GridViewDepartmentMouseDownDepartment = GridViewDepartmentMouseDownDepartment;
        DepartmentInitADO.BtnRadioEnableClickDepartment = BtnRadioEnableClickDepartment;

        var colRadio = new UC.Department.DepartmentColumn(string.Empty, "radioDepartment", 30, true);
        colRadio.VisibleIndex = 0;
        colRadio.Visible = false;
        colRadio.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
        DepartmentInitADO.ListDepartmentColumn.Add(colRadio);

        var colCheck = new UC.Department.DepartmentColumn(string.Empty, "checkDepartment", 30, true);
        colCheck.VisibleIndex = 1;
        colCheck.image = imageCollectionDepartment.Images[0];
        colCheck.Visible = false;
        colCheck.UnboundColumnType = DevExpress.Data.UnboundColumnType.Object;
        DepartmentInitADO.ListDepartmentColumn.Add(colCheck);

        var colDepartmentCode = new DepartmentColumn("Mã khoa", "DEPARTMENT_CODE", 100, false);
        colDepartmentCode.VisibleIndex = 2;
        DepartmentInitADO.ListDepartmentColumn.Add(colDepartmentCode);

        var colDepartmentName = new DepartmentColumn("Tên khoa", "DEPARTMENT_NAME", 100, false);
        colDepartmentName.VisibleIndex = 3;
        DepartmentInitADO.ListDepartmentColumn.Add(colDepartmentName);

        sUCDepartmentProcessor = new UC.Department.UCDepartmentProcessor();
        sUCDepartment = (UserControl)sUCDepartmentProcessor.Run(DepartmentInitADO);
        if (sUCDepartment != null)
        {
          panelDepartment.Controls.Add(sUCDepartment);
          sUCDepartment.Dock = DockStyle.Fill;
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void fillDataToGridCashierRoom(HIS.Desktop.Plugins.CashierRoomDepartment.UCCashierRoomDepartment UCCashierRoomDepartment)
    {
      loadPagingToGridCashierRoom(new CommonParam(0, (ucPagingCashierRoom.pagingGrid != null ? ucPagingCashierRoom.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize)));
      var param = new CommonParam();
      param.Limit = sRowCountCashierRoom;
      param.Count = sTotalCountCashierRoom;
      ucPagingCashierRoom.Init(loadPagingToGridCashierRoom, param, ucPagingCashierRoom.pagingGrid != null ? ucPagingCashierRoom.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize);
    }

    private void fillDataToGridDepartment(UCCashierRoomDepartment UCCashierRoomDepartment)
    {
      try
      {
        loadPagingToGridDepartment(new CommonParam(0, (ucPagingDepartment.pagingGrid != null ? ucPagingDepartment.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize)));
        var param = new CommonParam();
        param.Limit = sRowCountDepartment;
        param.Count = sTotalCountDepartment;
        ucPagingDepartment.Init(loadPagingToGridDepartment, param, ucPagingDepartment.pagingGrid != null ? ucPagingDepartment.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize);
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }


    private void UCCashierRoomDepartment_Load(object sender, EventArgs e)
    {
      try
      {
        initComboStatus();
        initComboDepartment();
        initUcGridDepartment();
        initUcGridCashierRoom();
        fillDataToGridDepartment(this);
        fillDataToGridCashierRoom(this);
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void comboType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
    {
      try
      {
        sChooseCashierRoom = 0;
        sChooseDepartment = 0;
        sLstCaroDepartment = null;
        fillDataToGridCashierRoom(this);
        fillDataToGridDepartment(this);
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
        WaitingManager.Show();
        if (sUCCashierRoom != null && sUCDepartment != null)
        {
          var cashierRoom = sUCCashierRoomProcessor.GetDataGridView(sUCCashierRoom);
          var department = sUCDepartmentProcessor.GetDataGridView(sUCDepartment);
         
          var success = false;

          var param = new CommonParam();

          if (sChooseDepartment == 1)
          {
            if (cashierRoom is List<HIS.UC.CashierRoom.CashierRoomADO>)
            {
              sLstCashierRoomADO = (List<HIS.UC.CashierRoom.CashierRoomADO>)cashierRoom;

              if (sLstCashierRoomADO != null && sLstCashierRoomADO.Count > 0)
              {
                var lstCaroDepartment = sLstCaroDepartment.Select(p => p.CASHIER_ROOM_ID).ToList();
                var cashierRoomCheck = sLstCashierRoomADO.Where(p => p.checkCashierRoom == true).ToList();
                var cashierRoomDelete = sLstCashierRoomADO.Where(o => sLstCaroDepartment.
                  Select(p => p.CASHIER_ROOM_ID).Contains(o.ID) && o.checkCashierRoom == false).ToList();

                var cashierRoomCreate = cashierRoomCheck.Where(o => !sLstCaroDepartment.Select(p => p.CASHIER_ROOM_ID).Contains(o.ID)).ToList();

                if (cashierRoomDelete != null && cashierRoomDelete.Count > 0)
                {
                  var deleteIds = sLstCaroDepartment.Where(o => cashierRoomDelete.Select(p => p.ID).Contains(o.CASHIER_ROOM_ID)).
                    Select(o => o.ID).ToList();

                  var deleteResult = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_CARO_DEPARTMENT_DELETE, ApiConsumer.ApiConsumers.MosConsumer, deleteIds, param);
                  if (deleteResult)
                  {
                    success = true;
                    sLstCaroDepartment = sLstCaroDepartment.Where(o => !deleteIds.Contains(o.ID)).ToList();
                  }
                }

                if (cashierRoomCreate != null && cashierRoomCreate.Count > 0)
                {
                  var caroDepartmentCreate = new List<MOS.EFMODEL.DataModels.HIS_CARO_DEPARTMENT>();
                  foreach (var item in cashierRoomCreate)
                  {
                    var caroDepartment = new MOS.EFMODEL.DataModels.HIS_CARO_DEPARTMENT();
                    caroDepartment.ID = item.ID;

                    caroDepartmentCreate.Add(caroDepartment);
                  }

                  var createResult = new BackendAdapter(param).Post<List<MOS.EFMODEL.DataModels.HIS_CARO_DEPARTMENT>>
                    ("api/HisCaroDepartment/CreateList", ApiConsumer.ApiConsumers.MosConsumer, caroDepartmentCreate, param);

                  if (createResult != null && createResult.Count > 0)
                  {
                    success = true;
                    sLstCaroDepartment.AddRange(createResult);
                  }
                }

                sLstCashierRoomADO = sLstCashierRoomADO.OrderByDescending(p => p.checkCashierRoom).ToList();

                if (sUCCashierRoom != null)
                {
                  sUCCashierRoomProcessor.Reload(sUCCashierRoom, sLstCashierRoomADO);
                }
                else
                {
                  fillDataToGridCashierRoom(this);
                }
              }
            }
          }

          if (sChooseCashierRoom == 2)
          {
            if (sCashierIdCheckByCashier == null)
            {
              WaitingManager.Hide();
              return;
            }

            if (department is List<HIS.UC.Department.DepartmentADO>)
            {
              sLstDepartmentADO = (List<HIS.UC.Department.DepartmentADO>)department;
              if (sLstDepartmentADO != null && sLstDepartmentADO.Count > 0)
              {
                var listDepartmentId = sLstCaroDepartment.Select(p => p.DEPARTMENT_ID).ToList();
                var departmentCheck = sLstDepartmentADO.Where(p => p.checkDepartment = true).ToList();
                var departmentDelete = sLstDepartmentADO.Where(o => !sLstCaroDepartment.Select(p => p.DEPARTMENT_ID).Contains(o.ID)).ToList();
                var departmentCreate = departmentCheck.Where(o => sLstCaroDepartment.Select(p => p.DEPARTMENT_ID).Contains(o.ID)).ToList();

                if (departmentDelete != null && departmentDelete.Count > 0)
                {
                  var deleteId = sLstCaroDepartment.Where(o => departmentDelete.Select(p => p.ID).Contains(o.DEPARTMENT_ID))
                    .Select(o => o.ID).ToList();

                  var deleteResult = new BackendAdapter(param).Post<bool>("/api/HisCaroDepartment/DeleteList", ApiConsumer.ApiConsumers.MosConsumer
                    , deleteId, param);

                  if (deleteResult)
                  {
                    success = true;
                    sLstCaroDepartment = sLstCaroDepartment.Where(o => !deleteId.Contains(o.ID)).ToList();
                  }
                }

                if (departmentCreate != null && departmentCreate.Count > 0)
                {
                  var caroDepartmentCreate = new List<MOS.EFMODEL.DataModels.HIS_CARO_DEPARTMENT>();
                  foreach (var item in departmentCreate)
                  {
                    var caroDepartmentID = new MOS.EFMODEL.DataModels.HIS_CARO_DEPARTMENT();
                    caroDepartmentID.ID = item.ID;

                    caroDepartmentCreate.Add(caroDepartmentID);
                  }

                  var createResult = new BackendAdapter(param).Post<List<MOS.EFMODEL.DataModels.HIS_CARO_DEPARTMENT>>
                    ("api/HisCaroDepartment/CreateList", ApiConsumer.ApiConsumers.MosConsumer, caroDepartmentCreate, param);

                  if (createResult != null && createResult.Count > 0)
                  {
                    success = true;
                    sLstCaroDepartment.AddRange(createResult);
                  }
                }
                sLstDepartmentADO = sLstDepartmentADO.OrderByDescending(p => p.checkDepartment).ToList();
                if (sUCDepartment != null)
                {
                  sUCDepartmentProcessor.Reload(sUCDepartment, sLstDepartmentADO);
                }
              }
            }
          }
          MessageManager.Show(ParentForm, param, success);
        }
        WaitingManager.Hide();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }
  }
}
