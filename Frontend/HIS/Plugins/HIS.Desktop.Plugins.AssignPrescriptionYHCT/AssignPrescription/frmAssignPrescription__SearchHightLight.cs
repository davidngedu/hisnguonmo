﻿using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Save;
using HIS.Desktop.Utility;
using HIS.UC.DateEditor;
using HIS.UC.PatientSelect;
using HIS.UC.PeriousExpMestList;
using HIS.UC.SecondaryIcd;
using HIS.UC.TreatmentFinish;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {

        #region HightLight result while search medi maty
        private string GetStringWithoutQuotes(string findText)
        {
            string stringWithoutQuotes = findText.ToLower().Replace("\"", string.Empty);
            return stringWithoutQuotes;
        }

        private int FindSubStringStartPosition(string dispalyText, string findText)
        {
            string stringWithoutQuotes = GetStringWithoutQuotes(findText);
            int index = dispalyText.ToLower().IndexOf(stringWithoutQuotes);
            return index;
        }

        private bool HiglightSubString(DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e, string findText)
        {
            int index = FindSubStringStartPosition(e.DisplayText, findText);
            if (index == -1)
            {
                return false;
            }

            e.Appearance.FillRectangle(e.Cache, e.Bounds);
            e.Cache.Paint.DrawMultiColorString(e.Cache, e.Bounds, e.DisplayText, GetStringWithoutQuotes(findText),
                e.Appearance, Color.Indigo, Color.Gold, true, index);
            return true;
        }

        private void OnCustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.OptionsFind.HighlightFindResults && !txtMediMatyForPrescription.Text.Equals(string.Empty))
            {
                CriteriaOperator op = ConvertFindPanelTextToCriteriaOperator(txtMediMatyForPrescription.Text, view, false);
                if (op is GroupOperator)
                {
                    string findText = txtMediMatyForPrescription.Text;
                    if (HiglightSubString(e, findText))
                        e.Handled = true;
                }
                else if (op is FunctionOperator)
                {
                    FunctionOperator func = op as FunctionOperator;
                    CriteriaOperator colNameOperator = func.Operands[0];
                    string colName = colNameOperator.LegacyToString().Replace("[", string.Empty).Replace("]", string.Empty);
                    if (!e.Column.FieldName.StartsWith(colName)) return;

                    CriteriaOperator valueOperator = func.Operands[1];
                    string findText = valueOperator.LegacyToString().ToLower().Replace("'", "");
                    if (HiglightSubString(e, findText))
                        e.Handled = true;
                }

            }
        }

        public static CriteriaOperator ConvertFindPanelTextToCriteriaOperator(string findPanelText, GridView view, bool applyPrefixes)
        {
            if (!string.IsNullOrEmpty(findPanelText))
            {
                FindSearchParserResults parseResult = new FindSearchParser().Parse(findPanelText, GetFindToColumnsCollection(view));
                if (applyPrefixes)
                    parseResult.AppendColumnFieldPrefixes();

                return DxFtsContainsHelperAlt.Create(parseResult, FilterCondition.Contains, false);
            }
            return null;
        }

        private static ICollection GetFindToColumnsCollection(GridView view)
        {
            System.Reflection.MethodInfo mi = typeof(ColumnView).GetMethod("GetFindToColumnsCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return mi.Invoke(view, null) as ICollection;
        }
        #endregion

    }
}
