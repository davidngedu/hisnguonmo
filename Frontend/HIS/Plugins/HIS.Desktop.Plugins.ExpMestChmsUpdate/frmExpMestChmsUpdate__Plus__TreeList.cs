﻿using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.ExpMestChmsUpdate.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestChmsUpdate
{
    public partial class frmExpMestChmsUpdate : HIS.Desktop.Utility.FormBase
    {
        private void expMestMediGrid__CustomUnboundColumnData(V_HIS_EXP_MEST_MEDICINE data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void expMestMateGrid__CustomUnboundColumnData(V_HIS_EXP_MEST_MATERIAL data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (e.Column.FieldName == "EXPIRED_DATE_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestChmsDetail_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0 || e.Column.FieldName != "EXP_AMOUNT")
                    return;
                var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (data != null)
                {
                    if (data.IsMedicine)
                    {
                        data.ExpMedicine.Amount = data.EXP_AMOUNT;
                    }
                    else
                    {
                        data.ExpMaterial.Amount = data.EXP_AMOUNT;
                    }
                }
                gridControlExpMestChmsDetail.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestChmsDetail_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "EXP_AMOUNT")
                        {
                            if (data.IsSupplement)
                            {
                                e.RepositoryItem = repositoryItemSpinExpAmountDisable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemSpinExpAmount;
                            }
                        }
                        else if (e.Column.FieldName == "BLOOD_ABO_ID")
                        {
                            if (data.IsBlood)
                            {
                                e.RepositoryItem = cboABO;
                            }
                        }
                        else if (e.Column.FieldName == "BLOOD_RH_ID")
                        {
                            if (data.IsBlood)
                            {
                                e.RepositoryItem = cboRH;
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

        private void gridViewExpMestChmsDetail_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MediMateTypeADO data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "VOLUME_DISPLAY")
                        {
                            if (data.IsBlood)
                            {
                                e.Value = data.VOLUME;
                            }
                            else
                            {
                                e.Value = "";
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

        private void gridViewExpMestChmsDetail_InvalidRowException(object sender, DevExpress.XtraGrid.Views.Base.InvalidRowExceptionEventArgs e)
        {
            try
            {
                e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestChmsDetail_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView vw = (sender as DevExpress.XtraGrid.Views.Grid.GridView);
                bool IsMedicine = (bool)vw.GetRowCellValue(e.RowHandle, "IsMedicine");
                bool IsBlood = (bool)vw.GetRowCellValue(e.RowHandle, "IsBlood");
                if (IsMedicine)
                {
                    e.Appearance.ForeColor = Color.Black;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else if (IsBlood)
                {
                    e.Appearance.ForeColor = Color.Red;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else
                {
                    e.Appearance.ForeColor = Color.Blue;
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestChmsDetail_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                if (gridViewExpMestChmsDetail.FocusedRowHandle < 0 || gridViewExpMestChmsDetail.FocusedColumn.FieldName != "EXP_AMOUNT")
                    return;
                var data = (MediMateTypeADO)((IList)((BaseView)sender).DataSource)[gridViewExpMestChmsDetail.FocusedRowHandle];
                if (data != null)
                {
                    bool valid = true;
                    string message = "";
                    if (data.EXP_AMOUNT <= 0)
                    {
                        valid = false;
                        message = Base.ResourceMessageLang.SoLuongXuatPhaiLonHonKhong;
                    }
                    //else if (data.EXP_AMOUNT > data.AVAILABLE_AMOUNT)
                    //{
                    //    valid = false;
                    //    message = Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho;
                    //}
                    if (!valid)
                    {
                        gridViewExpMestChmsDetail.SetColumnError(gridViewExpMestChmsDetail.FocusedColumn, message);
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
