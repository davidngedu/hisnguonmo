﻿using Inventec.Desktop.Common.LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate.Base
{
    class DateUtil
    {
        internal class DateValidObject
        {
            public DateValidObject() { }
            public int Age { get; set; }
            public string OutDate { get; set; }
            public string Message { get; set; }
            public bool HasNotDayDob { get; set; }

        }

        internal static DateValidObject ValidPatientDob(string inputDate)
        {
            DateValidObject result = new DateValidObject();
            try
            {
                if (String.IsNullOrEmpty(inputDate))
                {
                    return result;
                }
                int patientDob = Inventec.Common.TypeConvert.Parse.ToInt32(inputDate);

                if (inputDate.Length == 1 || inputDate.Length == 2)
                {
                    result.Age = (DateTime.Now.Year - patientDob);
                    result.OutDate = "01/01/" + result.Age;
                    result.HasNotDayDob = true;
                }
                else if (inputDate.Length == 4)
                {
                    if (patientDob <= DateTime.Now.Year)
                    {
                        result.OutDate = "01/01/" + inputDate;
                        result.HasNotDayDob = true;
                    }
                }
                else if (inputDate.Length == 8)
                {
                    result.OutDate = inputDate.Substring(0, 2) + "/" + inputDate.Substring(2, 2) + "/" + inputDate.Substring(4, 4);
                }
                else if (inputDate.Length == 10)
                {
                    result.OutDate = inputDate;
                }
                else
                {
                    result.Message = "Ngày sinh không đúng định dạng";
                }

                if (!String.IsNullOrEmpty(result.OutDate) && String.IsNullOrEmpty(result.Message))
                {
                    DateTime? dtPatientDob = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(result.OutDate);
                    if (dtPatientDob == null
                         || dtPatientDob.Value == DateTime.MinValue)
                    {
                        result.Message = "Ngày sinh không đúng định dạng";
                        result.OutDate = "";
                    }
                    else if (dtPatientDob != null
                         && dtPatientDob.Value != null
                         && dtPatientDob.Value.Date > DateTime.Now.Date)
                    {
                        result.Message = "Ngày sinh phải nhỏ hơn ngày hiện tại";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        //internal static DateValidObject ValidPatientDob(string inputDate)
        //{
        //    DateValidObject result = new DateValidObject();
        //    try
        //    {
        //        int patientDob = Inventec.Common.TypeConvert.Parse.ToInt32(inputDate);
        //        if (String.IsNullOrEmpty(inputDate))
        //        {
        //            result.Message = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
        //        }
        //        else if (inputDate.Length == 1 || inputDate.Length == 2)
        //        {
        //            if (patientDob < 7)
        //            {
        //                result.Message = "Ngày sinh không được nhỏ hơn 7";
        //            }
        //            else
        //            {
        //                result.Age = (DateTime.Now.Year - patientDob);
        //                result.OutDate = "01/01/" + result.Age;
        //            }
        //        }
        //        else if (inputDate.Length == 4)
        //        {
        //            if (patientDob <= DateTime.Now.Year)
        //            {
        //                result.OutDate = "01/01/" + inputDate;
        //                result.HasNotDayDob = true;
        //            }
        //        }
        //        else if (inputDate.Length == 8)
        //        {
        //            result.OutDate = inputDate.Substring(0, 2) + "/" + inputDate.Substring(2, 2) + "/" + inputDate.Substring(4, 4);
        //        }
        //        else if (inputDate.Length == 10)
        //        {
        //            result.OutDate = inputDate;
        //        }
        //        else
        //        {
        //            result.Message = "Ngày sinh không đúng định dạng";
        //        }

        //        if (!String.IsNullOrEmpty(result.OutDate) && String.IsNullOrEmpty(result.Message))
        //        {
        //            DateTime? dtPatientDob = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(result.OutDate);
        //            if (dtPatientDob == null
        //                 || dtPatientDob.Value == DateTime.MinValue)
        //            {
        //                result.Message = "Ngày sinh không đúng định dạng";
        //                result.OutDate = "";
        //            }
        //            else if (dtPatientDob != null
        //                 && dtPatientDob.Value != null
        //                 && dtPatientDob.Value.Date > DateTime.Now.Date)
        //            {
        //                result.Message = "Ngày sinh không đúng định dạng 1";
        //            }
        //            else if (CheckIsChild(dtPatientDob.Value) && inputDate.Length < 8)
        //            {
        //                result.Message = "Ngày sinh không đúng định dạng 2";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return result;
        //}

        static bool CheckIsChild(DateTime dtDob)
        {
            bool success = false;
            try
            {
                if (dtDob != null && dtDob != DateTime.MinValue)
                {
                    success = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtDob);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }
    }
}
