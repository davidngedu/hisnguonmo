﻿using HIS.Desktop.Plugins.ImportBlood.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImportBlood.Helpers
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
                int patientDob = Inventec.Common.TypeConvert.Parse.ToInt32(inputDate);
                if (String.IsNullOrEmpty(inputDate))
                {
                    result.Message = ResourceMessageLang.TruongDuLieuBatBuoc;
                }
                else if (inputDate.Length == 1 || inputDate.Length == 2)
                {
                    if (patientDob < 7)
                    {
                        result.Message = ResourceMessageLang.SoTuoiKhongDuocNhoHon7;
                    }
                    else
                    {
                        result.Age = (DateTime.Now.Year - patientDob);
                        result.OutDate = "01/01/" + result.Age;
                        result.HasNotDayDob = true;
                    }
                }
                else if (inputDate.Length == 4)
                {
                    if (patientDob <= DateTime.Now.Year)
                    {
                        result.OutDate = "01/01/" + inputDate;
                        result.HasNotDayDob = true;
                    }
                }
                else if (inputDate.Length == 6)
                {
                    result.OutDate = inputDate.Substring(0, 2) + "/" + inputDate.Substring(2, 4);
                }
                else if (inputDate.Length == 7)
                {
                    result.OutDate = inputDate;
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
                    result.Message = ResourceMessageLang.NhapNgaySinhKhongDungDinhDang;
                }

                if (!String.IsNullOrEmpty(result.OutDate) && String.IsNullOrEmpty(result.Message))
                {
                    DateTime? dtPatientDob = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(result.OutDate);
                    if (dtPatientDob == null
                         || dtPatientDob.Value == DateTime.MinValue)
                    {
                        result.Message = ResourceMessageLang.NhapNgaySinhKhongDungDinhDang;
                        result.OutDate = "";
                    }
                    else if (dtPatientDob != null
                         && dtPatientDob.Value != null
                         && dtPatientDob.Value.Date > DateTime.Now.Date)
                    {
                        result.Message = ResourceMessageLang.ThongTinNgaySinhPhaiNhoHonNgayHienTai;
                    }
                    else if (CheckIsChild(dtPatientDob.Value) && inputDate.Length < 8)
                    {
                        result.Message = ResourceMessageLang.YeuCauNhapDayDuNgayThangNamSinhVoiBNDuoi6Tuoi;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }
    }
}
