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

namespace HIS.Desktop.Plugins.Library.PrintOtherForm.MpsBehavior.Mps000345
{
    public partial class Mps000345Behavior : MpsDataBase, ILoad
    {
        private void SetDicParamEkipUser(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (!String.IsNullOrEmpty(PrintConfig.OrganizationName))
                    dicParamPlus.Add("ORGANIZATION_NAME", PrintConfig.OrganizationName);
                else
                    dicParamPlus.Add("ORGANIZATION_NAME", "");

                List<HIS_EXECUTE_ROLE> executeRoles = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EXECUTE_ROLE>();
                if (executeRoles != null && executeRoles.Count > 0)
                {
                    foreach (var item in executeRoles)
                    {
                        dicParamPlus.Add("USERNAME_EXECUTE_ROLE_" + item.EXECUTE_ROLE_CODE, "");
                        dicParamPlus.Add("LOGIN_NAME_EXECUTE_ROLE_" + item.EXECUTE_ROLE_CODE, "");
                    }
                }

                //Ekip thuc hien
                if (this.ekipUsers != null)
                {
                    foreach (var item in ekipUsers)
                    {
                        if (dicParamPlus.ContainsKey("USERNAME_EXECUTE_ROLE_" + item.EXECUTE_ROLE_CODE))
                        {
                            dicParamPlus["USERNAME_EXECUTE_ROLE_" + item.EXECUTE_ROLE_CODE] = item.USERNAME;
                        }
                        if (dicParamPlus.ContainsKey("LOGIN_NAME_EXECUTE_ROLE_" + item.EXECUTE_ROLE_CODE))
                        {
                            dicParamPlus["LOGIN_NAME_EXECUTE_ROLE_" + item.EXECUTE_ROLE_CODE] = item.LOGINNAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void SetDicParamMedicine(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                // thuốc đã sử dụng
                if (this.expMestMedicines != null)
                {
                    List<string> results = new List<string>();

                    var expMestMedicines = this.expMestMedicines.GroupBy(o => o.TDL_MEDICINE_TYPE_ID).ToList();
                    foreach (var item in expMestMedicines)
                    {
                        string medi = "";
                        medi += item.FirstOrDefault().MEDICINE_TYPE_NAME + " - SL: ";
                        medi += item.Sum(o => o.AMOUNT);
                        results.Add(medi);
                    }

                    dicParamPlus.Add("EXP_MEST_MEDICINES", String.Join("; ", results));
                }
                else
                {
                    dicParamPlus.Add("EXP_MEST_MEDICINES", "");
                }
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

        private void SetDicDHST(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (this.dhst != null)
                {
                    AddKeyIntoDictionaryPrint<V_HIS_DHST>(dhst, dicParamPlus);
                }
                else
                {
                    V_HIS_DHST temp = new V_HIS_DHST();
                    AddKeyIntoDictionaryPrint<V_HIS_DHST>(temp, dicParamPlus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDicParamSereServPTTT(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (this.sereServPTTT != null && this.sereServ != null)
                {
                    AddKeyIntoDictionaryPrint<V_HIS_SERE_SERV_PTTT>(this.sereServPTTT, dicParamPlus);
                }
                else
                {
                    V_HIS_SERE_SERV_PTTT temp = new V_HIS_SERE_SERV_PTTT();
                    AddKeyIntoDictionaryPrint<V_HIS_SERE_SERV_PTTT>(temp, dicParamPlus);
                }

                if (!String.IsNullOrEmpty((string)(dicParamPlus["BEFORE_PTTT_ICD_TEXT"] == null ? "" : dicParamPlus["BEFORE_PTTT_ICD_TEXT"])))
                {
                    dicParamPlus["BEFORE_PTTT_ICD_NAME"] = dicParamPlus["BEFORE_PTTT_ICD_TEXT"];
                }

                if (!String.IsNullOrEmpty((string)(dicParamPlus["AFTER_PTTT_ICD_TEXT"] == null ? "" : dicParamPlus["AFTER_PTTT_ICD_TEXT"])))
                {
                    dicParamPlus["AFTER_PTTT_ICD_NAME"] = dicParamPlus["AFTER_PTTT_ICD_TEXT"];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDicParamSereServExt(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (this.sereServExt != null)
                {
                    AddKeyIntoDictionaryPrint<HIS_SERE_SERV_EXT>(this.sereServExt, dicParamPlus);
                }
                else
                {
                    HIS_SERE_SERV_EXT temp = new HIS_SERE_SERV_EXT();
                    AddKeyIntoDictionaryPrint<HIS_SERE_SERV_EXT>(temp, dicParamPlus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDicParamServiceReq(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (this.serviceReq != null)
                {
                    if (String.IsNullOrEmpty(this.serviceReq.REQUEST_DEPARTMENT_NAME))
                    {
                        HIS_DEPARTMENT department = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == this.serviceReq.REQUEST_DEPARTMENT_ID);
                        this.serviceReq.REQUEST_DEPARTMENT_NAME = department != null ? department.DEPARTMENT_NAME : "";
                    }
                    dicParamPlus.Add("REQUEST_DEPARTMENT_NAME", this.serviceReq.REQUEST_DEPARTMENT_NAME);

                    if (String.IsNullOrEmpty(this.serviceReq.REQUEST_ROOM_NAME))
                    {
                        V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.serviceReq.REQUEST_ROOM_ID);
                        this.serviceReq.REQUEST_ROOM_NAME = room != null ? room.ROOM_NAME : "";
                    }
                    AddKeyIntoDictionaryPrint<V_HIS_SERVICE_REQ>(this.serviceReq, dicParamPlus);
                }
                else
                {
                    V_HIS_SERVICE_REQ temp = new V_HIS_SERVICE_REQ();
                    AddKeyIntoDictionaryPrint<V_HIS_SERVICE_REQ>(temp, dicParamPlus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDicParamBedAndBedRoomFromTreatment(ref Dictionary<string, object> dicParamPlus)
        {
            try
            {
                if (treatmentBedRooms != null && treatmentBedRooms.Count > 0)
                {
                    var bedRoom = treatmentBedRooms.FirstOrDefault(o => o.REMOVE_TIME == null);
                    dicParamPlus.Add("BED_ROOM_NAME", bedRoom != null ? bedRoom.BED_ROOM_NAME : "");
                    dicParamPlus.Add("BED_NAME", bedRoom != null ? bedRoom.BED_NAME : "");
                }
                else
                {
                    dicParamPlus.Add("BED_ROOM_NAME", "");
                    dicParamPlus.Add("BED_NAME", "");
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
