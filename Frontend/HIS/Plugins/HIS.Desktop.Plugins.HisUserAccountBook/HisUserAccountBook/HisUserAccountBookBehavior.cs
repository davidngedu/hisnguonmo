﻿using HIS.Desktop.Common;
using HIS.Desktop.Plugins.HisUserAccountBook.HisUserAccountBook;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisUserAccountBook.HisUserAccountBook
{
    class HisUserAccountBookBehavior : BusinessBase, IHisUserAccountBook
    {
        object[] entity;
        internal HisUserAccountBookBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisUserAccountBook.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
               MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK accountBook = null;

                if (entity.GetType() == typeof(object[]))
                {
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                            else if (entity[i] is MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK)
                            {
                                accountBook = (MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK)entity[i];
                            }
                        }
                    }
                }
                if (moduleData != null && accountBook != null)
                {
                    return new frmUserAccountBook(moduleData,accountBook);
                }
                else if (moduleData != null)
                {
                    return new frmUserAccountBook(moduleData);
                }
                else
                {
                    return new frmUserAccountBook();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
