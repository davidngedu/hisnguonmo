﻿using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.PrintOtherForm.Base;
using HIS.Desktop.Plugins.Library.PrintOtherForm.RunPrintTemplate;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ProcessorBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000402
{
    public partial class Mps000402Behavior : MpsDataBase, ILoad
    {
        private void SetDicParamCommon(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (!String.IsNullOrEmpty(PrintConfig.OrganizationName))
                    dicParamPlus.Add("ORGANIZATION_NAME", PrintConfig.OrganizationName);
                else if (department != null)
                    dicParamPlus.Add("ORGANIZATION_NAME", department.BRANCH_NAME);
                else
                    dicParamPlus.Add("ORGANIZATION_NAME", "");

                AddKeyIntoDictionaryPrint<V_HIS_DEPARTMENT>(department, dicParamPlus);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void SetDicParamPatient(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (this.patient != null)
                {
                    AddKeyIntoDictionaryPrint<V_HIS_PATIENT>(patient, dicParamPlus);
                    dicParamPlus.Add("AGE", this.CalculateFullAge(patient.DOB));
                    DateTime dob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patient.DOB) ?? DateTime.Now;
                    dicParamPlus.Add("AGE_BY_YEAR", DateTime.Now.Year - dob.Year);
                }
                else
                {
                    V_HIS_PATIENT temp = new V_HIS_PATIENT();
                    AddKeyIntoDictionaryPrint<V_HIS_PATIENT>(temp, dicParamPlus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal string CalculateFullAge(long ageNumber)
        {
            string tuoi;
            string cboAge;
            try
            {
                DateTime dtNgSinh = Inventec.Common.TypeConvert.Parse.ToDateTime(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ageNumber));
                TimeSpan diff = DateTime.Now - dtNgSinh;
                long tongsogiay = diff.Ticks;
                if (tongsogiay < 0)
                {
                    tuoi = "";
                    cboAge = "Tuổi";
                    return "";
                }
                DateTime newDate = new DateTime(tongsogiay);

                int nam = newDate.Year - 1;
                int thang = newDate.Month - 1;
                int ngay = newDate.Day - 1;
                int gio = newDate.Hour;
                int phut = newDate.Minute;
                int giay = newDate.Second;

                if (nam > 0)
                {
                    tuoi = nam.ToString();
                    cboAge = "Tuổi";
                }
                else
                {
                    if (thang > 0)
                    {
                        tuoi = thang.ToString();
                        cboAge = "Tháng";
                    }
                    else
                    {
                        if (ngay > 0)
                        {
                            tuoi = ngay.ToString();
                            cboAge = "ngày";
                        }
                        else
                        {
                            tuoi = "";
                            cboAge = "Giờ";
                        }
                    }
                }
                return tuoi + " " + cboAge;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        private void AddKeyIntoDictionaryPrint<T>(T data, Dictionary<string, object> dicParamPlus)
        {
            try
            {
                PropertyInfo[] pis = typeof(T).GetProperties();
                if (pis != null && pis.Length > 0)
                {
                    foreach (var pi in pis)
                    {
                        var searchKey = dicParamPlus.SingleOrDefault(o => o.Key == pi.Name);
                        if (String.IsNullOrEmpty(searchKey.Key))
                        {
                            dicParamPlus.Add(pi.Name, pi.GetValue(data) != null ? pi.GetValue(data) : "");
                        }
                        else
                        {
                            dicParamPlus[pi.Name] = pi.GetValue(data) != null ? pi.GetValue(data) : "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
