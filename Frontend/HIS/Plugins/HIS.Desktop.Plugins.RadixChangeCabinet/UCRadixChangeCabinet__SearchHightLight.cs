﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;

namespace HIS.Desktop.Plugins.RadixChangeCabinet
{
    public partial class UCRadixChangeCabinet : UserControl
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
