﻿using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        internal void RebuildTutorialWithInControlContainer(object data)
        {
            try
            {
                gridViewTutorial.OptionsView.ShowColumnHeaders = false;
                gridViewTutorial.BeginUpdate();
                gridViewTutorial.Columns.Clear();
                popupControlContainerTutorial.Width = 550;
                popupControlContainerTutorial.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "TUTORIAL";
                col1.Caption = "Hướng dẫn sử dụng";
                col1.Width = 400;
                col1.VisibleIndex = 1;
                gridViewTutorial.Columns.Add(col1);

                gridViewTutorial.GridControl.DataSource = data;
                gridViewTutorial.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Tutorial_RowClick(object data)
        {
            try
            {
                HIS_MEDICINE_TYPE_TUT medicineTypeTut = data as HIS_MEDICINE_TYPE_TUT;
                if (medicineTypeTut != null)
                {
                    //Nếu hướng dẫn sử dụng mẫu có đường dùng thì lấy ra
                    if (medicineTypeTut.MEDICINE_USE_FORM_ID > 0)
                    {
                        this.cboMedicineUseForm.EditValue = medicineTypeTut.MEDICINE_USE_FORM_ID;
                    }
                    //Nếu không có đường dùng thì lấy đường dùng từ danh mục loại thuốc
                    else
                    {
                        var medicineType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == medicineTypeTut.MEDICINE_TYPE_ID);
                        if (medicineType != null && (medicineType.MEDICINE_USE_FORM_ID ?? 0) > 0)
                        {
                            this.cboMedicineUseForm.EditValue = medicineType.MEDICINE_USE_FORM_ID;
                        }
                    }

                    Inventec.Common.Logging.LogSystem.Debug("Truong hop co HDSD thuoc theo tai khoan cua loai thuoc (HIS_MEDICINE_TYPE_TUT)--> lay truong DAY_COUNT gan vao spinSoNgay" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeTut), medicineTypeTut));
                                       
                    //Nếu có trường hướng dẫn thì sử dụng luôn
                    if (!String.IsNullOrEmpty(medicineTypeTut.TUTORIAL))
                    {
                        this.txtTutorial.Text = medicineTypeTut.TUTORIAL;
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
