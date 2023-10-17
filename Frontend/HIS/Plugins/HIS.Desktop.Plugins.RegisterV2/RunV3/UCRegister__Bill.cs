﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {
        private void Bill()
        {
            try
            {
                if (this.CheckCashierRoom())
                {
                    if (this.GetTreatmentIdFromResultData() > 0)
                    {
                        TransactionBillADO transactionBillADO = new TransactionBillADO(this.GetTreatmentIdFromResultData(), this.currentModule.RoomId);

                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionBill").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionBill'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.TransactionBill' is not plugins");

                        List<object> listArgs = new List<object>();
                        transactionBillADO.CashierRoomId = (long)(this.cboCashierRoom.EditValue);
                        listArgs.Add(transactionBillADO);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool CheckCashierRoom()
        {
            bool valid = true;
            try
            {
                if (Inventec.Common.TypeConvert.Parse.ToInt64((this.cboCashierRoom.EditValue ?? "0").ToString()) == 0)
                {
                    valid = false;
                    MessageManager.Show(ResourceMessage.ChonPhongThuNganTruocKhiMoTinhNangNay);
                    cboCashierRoom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private long GetTreatmentIdFromResultData()
        {
            long result = 0;
            try
            {
                if (this.resultHisPatientProfileSDO != null)
                {
                    result = this.resultHisPatientProfileSDO.HisTreatment.ID;
                }
                else if (this.currentHisExamServiceReqResultSDO != null)
                {
                    result = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.ID;
                }
            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }
    }
}
