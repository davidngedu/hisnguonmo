﻿using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.TransactionDebtCollect.Config;
using HIS.UC.SereServTree;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionDebtCollect
{
    public partial class frmTransactionDebtCollect : HIS.Desktop.Utility.FormBase
    {

        private void treeSereServ_CustomDrawNodeCell(SereServADO data, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (data != null && !e.Node.HasChildren)
                {
                    if (data.VIR_TOTAL_PATIENT_PRICE.HasValue && data.VIR_TOTAL_PATIENT_PRICE.Value > 0 && (!data.IS_NO_EXECUTE.HasValue || data.IS_NO_EXECUTE.Value != 1))
                    {
                        if (e.Node.Checked)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
                    }
                    else
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
                    }
                }
                else if (data != null && e.Node.HasChildren)
                {
                    e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
                    if (e.Node.ParentNode != null)
                    {
                        e.Appearance.BackColor = Color.Khaki;
                    }
                    else
                    {
                        e.Appearance.BackColor = Color.Pink;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_CheckAllNode(TreeListNodes nodes)
        {
            try
            {
                if (nodes != null)
                {
                    foreach (TreeListNode node in nodes)
                    {
                        var nodeData = (SereServADO)node.TreeList.GetDataRecordByNode(node);
                        if (nodeData != null)
                        {
                            if (Config.HisConfigCFG.MustFinishTreatmentForBill == "1" && this.currentTreatment.IS_PAUSE != 1 && nodeData.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                            {
                                node.UncheckAll();
                                CheckNode(node);
                            }
                            else if (Config.HisConfigCFG.MustFinishTreatmentForBill == "2" && this.currentTreatment.IS_PAUSE != 1)
                            {
                                node.UncheckAll();
                                CheckNode(node);
                            }
                            else if (nodeData.IS_NO_EXECUTE.HasValue && nodeData.IS_NO_EXECUTE.Value == 1)
                            {
                                node.UncheckAll();
                                CheckNode(node);
                            }
                            else
                            {
                                node.UncheckAll();
                                CheckNode(node);
                                if (node.HasChildren)
                                {
                                    this.ProcessChildNode(node);
                                }
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

        private void CheckNode(TreeListNode node)
        {
            try
            {
                if (node != null)
                {
                    foreach (TreeListNode childNode in node.Nodes)
                    {
                        var nodeData = (SereServADO)node.TreeList.GetDataRecordByNode(childNode);
                        if (nodeData != null)
                        {
                            if (Config.HisConfigCFG.MustFinishTreatmentForBill == "1" && this.currentTreatment.IS_PAUSE != 1 && nodeData.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                            {
                                childNode.UncheckAll();
                                CheckNode(childNode);
                            }
                            else if (Config.HisConfigCFG.MustFinishTreatmentForBill == "2" && this.currentTreatment.IS_PAUSE != 1)
                            {
                                childNode.UncheckAll();
                                CheckNode(childNode);
                            }
                            else if (nodeData.IS_NO_EXECUTE.HasValue && nodeData.IS_NO_EXECUTE.Value == 1)
                            {
                                childNode.UncheckAll();
                                CheckNode(childNode);
                            }
                            else
                            {
                                childNode.UncheckAll();
                                CheckNode(childNode);
                                if (childNode.HasChildren)
                                {
                                    this.ProcessChildNode(childNode);
                                }
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

        private void ProcessChildNode(TreeListNode parentNode)
        {
            try
            {
                if (parentNode.Nodes != null)
                {
                    if (parentNode.Nodes.Any(o => o.CheckState == CheckState.Indeterminate))
                    {
                        parentNode.CheckState = CheckState.Indeterminate;
                    }
                    else if (parentNode.Nodes.Any(o => !o.Checked))
                    {
                        if (parentNode.Nodes.Any(o => o.Checked))
                        {
                            parentNode.CheckState = CheckState.Indeterminate;
                        }
                        else
                        {
                            parentNode.CheckState = CheckState.Unchecked;
                        }
                    }
                    else
                    {
                        parentNode.CheckState = CheckState.Checked;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_BeforeCheckNode(TreeListNode node, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                if (node != null)
                {
                    var nodeData = (SereServADO)node.TreeList.GetDataRecordByNode(node);
                    if (nodeData != null && Config.HisConfigCFG.MustFinishTreatmentForBill == "1" && this.currentTreatment.IS_PAUSE != 1 && nodeData.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                    {
                        e.CanCheck = false;
                        node.UncheckAll();
                        return;
                    }
                    if (nodeData != null && Config.HisConfigCFG.MustFinishTreatmentForBill == "2" && this.currentTreatment.IS_PAUSE != 1)
                    {
                        e.CanCheck = false;
                        node.UncheckAll();
                        return;
                    }
                    if (nodeData != null && nodeData.IS_NO_EXECUTE.HasValue && nodeData.IS_NO_EXECUTE.Value == 1)
                    {
                        e.CanCheck = false;
                        node.UncheckAll();
                        return;
                    }
                    e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
                    if (node.Checked)
                    {
                        node.UncheckAll();
                    }
                    else
                    {
                        node.CheckAll();
                    }
                    while (node.ParentNode != null)
                    {
                        node = node.ParentNode;
                        bool valid = false;
                        foreach (DevExpress.XtraTreeList.Nodes.TreeListNode item in node.Nodes)
                        {
                            if (item.CheckState == CheckState.Checked || item.CheckState == CheckState.Indeterminate)
                            {
                                valid = true;
                                break;
                            }
                        }
                        if (valid)
                        {
                            node.CheckState = CheckState.Checked;
                        }
                        else
                        {
                            node.CheckState = CheckState.Unchecked;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_AfterCheckNode(TreeListNode node, SereServADO data)
        {
            try
            {
                CalcuTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_CustomDrawNodeCheckBox(SereServADO data, DevExpress.XtraTreeList.CustomDrawNodeCheckBoxEventArgs e)
        {
            try
            {
                if (data != null && Config.HisConfigCFG.MustFinishTreatmentForBill == "1" && this.currentTreatment.IS_PAUSE != 1 && data.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                {
                    e.Handled = true;
                }
                else if (data != null && Config.HisConfigCFG.MustFinishTreatmentForBill == "2" && this.currentTreatment.IS_PAUSE != 1)
                {
                    e.Handled = true;
                }
                else if (data != null && data.IS_NO_EXECUTE.HasValue && data.IS_NO_EXECUTE.Value == 1)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_CustomUnboundColumnData(SereServADO data, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (!e.Node.HasChildren)
                    {
                        if (e.Column.FieldName == "AMOUNT_DISPLAY")
                        {
                            e.Value = ConvertNumberToString(data.AMOUNT);
                        }
                        else if (e.Column.FieldName == "VIR_PRICE_DISPLAY")
                        {
                            e.Value = ConvertNumberToString(data.VIR_PRICE ?? 0);
                        }
                        else if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                        {
                            e.Value = ConvertNumberToString(data.VIR_TOTAL_PRICE ?? 0);
                        }
                        else if (e.Column.FieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                        {
                            e.Value = ConvertNumberToString(data.VIR_TOTAL_HEIN_PRICE ?? 0);
                        }
                        else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                        {
                            e.Value = ConvertNumberToString(data.VIR_TOTAL_PATIENT_PRICE ?? 0);
                        }
                        else if (e.Column.FieldName == "DISCOUNT_DISPLAY")
                        {
                            e.Value = ConvertNumberToString(data.DISCOUNT ?? 0);
                        }
                        else if (e.Column.FieldName == "VAT_DISPLAY")
                        {
                            e.Value = ConvertNumberToString(data.VAT);
                        }
                    }
                    else
                    {
                        if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                        {
                            this.GetTotalPriceOfChildChoice(data, e.Node.Nodes, "VIR_TOTAL_PRICE_DISPLAY");
                            e.Value = ConvertNumberToString(data.VIR_TOTAL_PRICE ?? 0);
                        }
                        else if (e.Column.FieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                        {
                            this.GetTotalPriceOfChildChoice(data, e.Node.Nodes, "VIR_TOTAL_HEIN_PRICE_DISPLAY");
                            e.Value = ConvertNumberToString(data.VIR_TOTAL_HEIN_PRICE ?? 0);
                        }
                        else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                        {
                            this.GetTotalPriceOfChildChoice(data, e.Node.Nodes, "VIR_TOTAL_PATIENT_PRICE_DISPLAY");
                            e.Value = ConvertNumberToString(data.VIR_TOTAL_PATIENT_PRICE ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetTotalPriceOfChildChoice(SereServADO data, TreeListNodes childs, string fieldName)
        {
            try
            {
                decimal totalChoicePrice = 0;
                if (childs != null && childs.Count > 0)
                {
                    foreach (TreeListNode item in childs)
                    {
                        var nodeData = (SereServADO)item.TreeList.GetDataRecordByNode(item);
                        if (nodeData == null) continue;
                        if (!item.HasChildren && item.Checked)
                        {
                            if (fieldName == "VIR_TOTAL_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PRICE ?? 0);
                            }
                            else if (fieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_HEIN_PRICE ?? 0);
                            }
                            else if (fieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PATIENT_PRICE ?? 0);
                            }
                        }
                        else if (item.HasChildren)
                        {
                            if (fieldName == "VIR_TOTAL_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PRICE ?? 0);
                            }
                            else if (fieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_HEIN_PRICE ?? 0);
                            }
                            else if (fieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                            {
                                totalChoicePrice += (nodeData.VIR_TOTAL_PATIENT_PRICE ?? 0);
                            }
                        }
                    }
                }
                if (fieldName == "VIR_TOTAL_PRICE_DISPLAY")
                {
                    data.VIR_TOTAL_PRICE = totalChoicePrice;
                }
                else if (fieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                {
                    data.VIR_TOTAL_HEIN_PRICE = totalChoicePrice;
                }
                else if (fieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                {
                    data.VIR_TOTAL_PATIENT_PRICE = totalChoicePrice;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string ConvertNumberToString(decimal number)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Number.Convert.NumberToString(number, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }
    }
}
