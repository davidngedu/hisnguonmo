﻿using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionBill.TransactionBill
{
    class TransactionBillFactory
    {
        internal static ITransactionBill MakeITransactionBill(CommonParam param, object[] data)
        {
            ITransactionBill result = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            V_HIS_TREATMENT_FEE hisTreatment = null;
            V_HIS_PATIENT_TYPE_ALTER resultPatientType = null;
            List<V_HIS_SERE_SERV_5> ListSereServ5 = null;
            bool? IsDirectlyBilling = null;
            V_HIS_TRANSACTION tran = null;
            try
            {
                if (data.GetType() == typeof(object[]))
                {
                    if (data != null && data.Count() > 0)
                    {
                        for (int i = 0; i < data.Count(); i++)
                        {
                            if (data[i] is V_HIS_TREATMENT_FEE)
                            {
                                hisTreatment = (V_HIS_TREATMENT_FEE)data[i];
                            }
                            else if (data[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
                            }
                            else if (data[i] is List<V_HIS_SERE_SERV_5>)
                            {
                                ListSereServ5 = (List<V_HIS_SERE_SERV_5>)data[i];
                            }
                            else if (data[i] is V_HIS_PATIENT_TYPE_ALTER)
                            {
                                resultPatientType = (V_HIS_PATIENT_TYPE_ALTER)data[i];
                            }
                            else if (data[i] is bool)
                            {
                                IsDirectlyBilling = (bool)data[i];
                            }
                            else if (data[i] is V_HIS_TRANSACTION)
                            {
                                tran = (V_HIS_TRANSACTION)data[i];
                            }
                        }

                        if (moduleData != null && hisTreatment != null && ListSereServ5 != null && ListSereServ5.Count > 0)
                        {
                            result = new TransactionBillBehavior(moduleData, param, hisTreatment, ListSereServ5, resultPatientType, IsDirectlyBilling,tran);
                        }
                        else if (moduleData != null && hisTreatment != null)
                        {
                            result = new TransactionBillBehavior(moduleData, param, hisTreatment, resultPatientType, IsDirectlyBilling,tran);
                        }
                        else
                        {
                            result = new TransactionBillBehavior(moduleData, param, IsDirectlyBilling,tran);
                        }
                    }
                }

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
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
