﻿using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class InvoicePrintPageCFG
    {
        private const string INVOICE_PRINT_FIRST_PAGE = "HIS.Desktop.Plugins.InvoiceBook.rowCountGridFirstPage";
        private const string INVOICE_PRINT_NEXT_PAGE = "HIS.Desktop.Plugins.InvoiceBook.rowCountGridNextPage";

        private static long firstPage;
        public static long First_Page
        {
            get
            {
                firstPage = GetLong(INVOICE_PRINT_FIRST_PAGE);
                return firstPage;
            }
            set
            {
                firstPage = value;
            }
        }

        private static long nextPage;
        public static long Next_Page
        {
            get
            {
                nextPage = GetLong(INVOICE_PRINT_NEXT_PAGE);
                return nextPage;
            }
            set
            {
                nextPage = value;
            }
        }

        private static long GetLong(string code)
        {
            long result = 0;
            try
            {
                SDA.EFMODEL.DataModels.SDA_CONFIG config = Inventec.Common.LocalStorage.SdaConfig.ConfigLoader.dictionaryConfig[code];
                if (config == null) throw new ArgumentNullException(code);
                string value = String.IsNullOrEmpty(config.VALUE) ? (String.IsNullOrEmpty(config.DEFAULT_VALUE) ? "" : config.DEFAULT_VALUE) : config.VALUE;
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);
                result = Inventec.Common.TypeConvert.Parse.ToInt64(value);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }
    }
}
