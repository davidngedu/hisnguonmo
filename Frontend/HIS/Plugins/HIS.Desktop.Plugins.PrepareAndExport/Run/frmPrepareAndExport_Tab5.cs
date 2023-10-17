﻿using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using System.Collections;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using MOS.SDO;

namespace HIS.Desktop.Plugins.PrepareAndExport.Run
{
	public partial class frmPrepareAndExport
	{
		private async Task LoadTab5()
		{
			try
			{
				Action myaction = () =>
				{
					lstTab5 = lstAll.Where(o => o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE).OrderBy(o => o.FINISH_TIME).ToList();
				};
				Task task = new Task(myaction);
				task.Start();
				await task;
				gcPassMedicine.DataSource = null;
				if (lstTab5 != null && lstTab5.Count > 0)
				{
					gcPassMedicine.DataSource = lstTab5;
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gvPassMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
		{
			try
			{

				if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
				{
					HIS_EXP_MEST pData = (HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
					if (e.Column.FieldName == "STT")
					{
						e.Value = e.ListSourceRowIndex + 1;
					}
					else if (e.Column.FieldName == "DOB_str")
					{
						if (pData.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
						{
							e.Value = pData.TDL_PATIENT_DOB.ToString().Substring(0, 4);
						}
						else
						{
							e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.TDL_PATIENT_DOB ?? 0);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void repMinusN_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
		{
			CommonParam param = new CommonParam();
			MOS.EFMODEL.DataModels.HIS_EXP_MEST rs;
			bool success = false;
			try
			{

				HIS_EXP_MEST data = (HIS_EXP_MEST)gvPassMedicine.GetFocusedRow();
				if (data != null)
				{
					HisExpMestSDO sdo = new HisExpMestSDO();
					string api = null;
					if (data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__THPK)
						api = "api/HisExpMest/AggrExamUnexport";
					else if(data.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__PL)
						api = "api/HisExpMest/AggrUnexport";
					sdo.ExpMestId = data.ID;
					sdo.ReqRoomId = this.currentModule.RoomId;
					WaitingManager.Show();
					rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST>(api, ApiConsumers.MosConsumer, sdo, param);
					WaitingManager.Hide();
					if (rs!=null)
					{
						foreach (var item in lstAll)
						{
							if (item.ID == rs.ID)
							{
								item.EXP_MEST_STT_ID = rs.EXP_MEST_STT_ID;
								item.IS_ABSENT = rs.IS_ABSENT;
								if (item.IS_ABSENT != 1)
								{
									if (lstTab3 == null || lstTab3.Count == 0)
										lstTab3 = new List<HIS_EXP_MEST>();
									lstTab3.Add(item);
									gcPrepareMedicine.DataSource = null;
									gcPrepareMedicine.DataSource = lstTab3;
								}else if (item.IS_ABSENT == 1)
								{
									if (lstTab4 == null || lstTab4.Count == 0)
										lstTab4 = new List<HIS_EXP_MEST>();
									lstTab4.Add(item);
									gcAbssentN.DataSource = null;
									gcAbssentN.DataSource = lstTab4;
								}
								break;
							}
						}
						if (!string.IsNullOrEmpty(txtGateCodeString) && dteStt.DateTime.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
						{
							CreateThreadCallPatientRefresh();
						}
						success = true;
						LoadTab5();
					}
					MessageManager.Show(this.ParentForm, param, success);
				}

			}
			catch (Exception ex)
			{
				WaitingManager.Hide();
				Inventec.Common.Logging.LogSystem.Error(ex);
			}
		}

		private void gvPassMedicine_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
		{
			try
			{
				DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
				if (e.RowHandle >= 0)
				{
					long? priority = (long?)view.GetRowCellValue(e.RowHandle, "PRIORITY");
					if (priority != null & priority == 1)
						e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
				}
			}
			catch (Exception ex)
			{
				Inventec.Common.Logging.LogSystem.Warn(ex);
			}
		}

	}
}
