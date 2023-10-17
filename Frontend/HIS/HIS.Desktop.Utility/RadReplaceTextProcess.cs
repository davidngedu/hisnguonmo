﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Utility
{
    public class RadReplaceTextProcess
    {
        public static void RadReplaceText(Telerik.WinControls.UI.RadRichTextEditor richTextEditor, string toSearch, string toReplaceWith, string keyReplaceRaw)
        {
            var document = richTextEditor.Document;
            var search = new Telerik.WinForms.Documents.TextSearch.DocumentTextSearch(document);
            var rangesTrackingDocumentChanges = new List<Telerik.WinForms.Documents.TextSearch.TextRange>();

            foreach (var textRange in search.FindAll(toSearch))
            {
                var newRange = new Telerik.WinForms.Documents.TextSearch.TextRange(new Telerik.WinForms.Documents.DocumentPosition(textRange.StartPosition, true), new Telerik.WinForms.Documents.DocumentPosition(textRange.EndPosition, true));
                rangesTrackingDocumentChanges.Add(newRange);
            }

            foreach (var textRange in rangesTrackingDocumentChanges)
            {
                int idxPlus = 0;
                Telerik.WinForms.Documents.DocumentPosition documentPosition = textRange.StartPosition;
                if (documentPosition.GetPreviousSpanBox() != null)
                {
                    idxPlus = 1;
                }
                document.CaretPosition.MoveToPosition(textRange.StartPosition);
                document.DeleteRange(textRange.StartPosition, textRange.EndPosition);


                Telerik.WinForms.Documents.Model.RadDocument docImport = ImportHtml(toReplaceWith);
                docImport.Style = document.Style;

                Telerik.WinForms.Documents.Model.DocumentFragment fragmentPar1 = new Telerik.WinForms.Documents.Model.DocumentFragment(docImport);
                Telerik.WinForms.Documents.Model.RadDocumentEditor documentEditor = new Telerik.WinForms.Documents.Model.RadDocumentEditor(document);
                documentEditor.InsertFragment(fragmentPar1);

                richTextEditor.Focus();
                document.CaretPosition.MoveToPosition(documentPosition);

                int keyLength = keyReplaceRaw.Length + 1 + idxPlus;
                for (int i = 0; i < keyLength; i++)
                {
                    documentPosition.MoveToNext();
                }

                document.CaretPosition.MoveToPosition(documentPosition);
               
                richTextEditor.Delete(true);
                textRange.StartPosition.Dispose();
                textRange.EndPosition.Dispose();
            }
        }
        
        public static Telerik.WinForms.Documents.Model.RadDocument ImportHtml(string content)
        {
            Telerik.WinForms.Documents.FormatProviders.Html.HtmlFormatProvider provider = new Telerik.WinForms.Documents.FormatProviders.Html.HtmlFormatProvider();
            return provider.Import(content);
        }
    }
}
