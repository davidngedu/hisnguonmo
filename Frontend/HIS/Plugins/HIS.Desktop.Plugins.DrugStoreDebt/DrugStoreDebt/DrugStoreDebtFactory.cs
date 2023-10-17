﻿using HIS.Desktop.Common;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.DrugStoreDebt.DrugStoreDebt
{
    class DrugStoreDebtFactory
    {
        internal static IDrugStoreDebt MakeIDrugStoreDebt(CommonParam param, object[] data)
        {
            IDrugStoreDebt result = null;
            List<long> expMestIds = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            DelegateSelectData refreshData = null;
            string expMestCode = null;
            try
            {
                if (data.GetType() == typeof(object[]))
                {
                    if (data != null && data.Count() > 0)
                    {
                        for (int i = 0; i < data.Count(); i++)
                        {
                            if (data[i] is List<long>)
                            {
                                expMestIds = (List<long>)data[i];
                            }
                            else if (data[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
                            }
                            else if (data[i] is string)
                            {
                                expMestCode = (string)data[i];
                            }
                        }

                        if (moduleData != null && expMestIds != null && expMestIds.Count > 0)
                        {
                            result = new DrugStoreDebtBehavior(moduleData, param, expMestIds, refreshData);
                        }
                        else if (moduleData != null && !String.IsNullOrWhiteSpace(expMestCode))
                        {
                            result = new DrugStoreDebtBehavior(moduleData, param, expMestCode, refreshData);
                        }
                        else
                        {
                            result = new DrugStoreDebtBehavior(moduleData, param);
                        }
                    }
                }

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                LogSystem.Error("Factory khong khoi tao duoc doi tuong.\n" + LogUtil.TraceData("data", data), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
