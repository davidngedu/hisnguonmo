﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;

namespace HIS.Desktop.Plugins.InvoiceBook.Popup.AssignAuthorized
{
    public partial class frmAssignAuthorized
    {
        #region Get_Data
        internal List<ACS_USER> AcsUserGetData()
        {
            var result = new List<ACS_USER>();
            try
            {
                var paramCommon = new CommonParam();

                result = new BackendAdapter(paramCommon).Get<List<ACS_USER>>
                    (AcsRequestUriStore.ACS_USER_GET, ApiConsumers.AcsConsumer, new AcsUserFilter(), paramCommon);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal List<V_HIS_USER_INVOICE_BOOK> UserInvoiceBookGetData(V_HIS_INVOICE_BOOK invoiceBook)
        {
            var result = new List<V_HIS_USER_INVOICE_BOOK>();
            try
            {
                var paramCommon = new CommonParam();

                var userInvoiceBookFilter = new HisUserInvoiceBookViewFilter
                {
                    INVOICE_BOOK_ID = invoiceBook.ID
                };
                result = new BackendAdapter(paramCommon).Get<List<V_HIS_USER_INVOICE_BOOK>>
                    (ApiConsumer.HisRequestUriStore.HIS_USER_INVOICE_BOOK_GET__VIEW, ApiConsumers.MosConsumer, userInvoiceBookFilter, paramCommon);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        #endregion

        #region Insert_Data
        private void InsertUserInvoiceBook(HisUserInvoiceBookSDO userInvoiceBookSdo)
        {
            try
            {
                var common = new CommonParam();
                var result = new BackendAdapter(new CommonParam()).Post<List<MOS.EFMODEL.DataModels.HIS_USER_INVOICE_BOOK>>
                    (ApiConsumer.HisRequestUriStore.HIS_USER_INVOICE_BOOK_CREATE, ApiConsumers.MosConsumer, userInvoiceBookSdo, common);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
