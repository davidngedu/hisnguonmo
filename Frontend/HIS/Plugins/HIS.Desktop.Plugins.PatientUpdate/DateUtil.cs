﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PatientUpdate
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

        internal static DateValidObject ValidPatientDob(string inputDate)
        {
            Inventec.Common.Logging.LogSystem.Debug("inputDate__" + inputDate);
            inputDate = inputDate.Replace(" ", "");
            inputDate = inputDate.Replace(":", "");
            inputDate = inputDate.Replace("/", "");
            DateValidObject result = new DateValidObject();
            try
            {
                int patientDob = Inventec.Common.TypeConvert.Parse.ToInt32(inputDate);
                if (String.IsNullOrEmpty(inputDate))
                {
                    result.Message = ResourceMessage.TruongDuLieuBatBuoc;
                }
                else if (inputDate.Length == 1 || inputDate.Length == 2)
                {
                    if (patientDob < 7)
                    {
                        result.Message = ResourceMessage.NgaySinhKhongDuocNhoHon7;
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
                else if (inputDate.Length == 8)
                {
                    result.OutDate = inputDate.Substring(0, 2) + "/" + inputDate.Substring(2, 2) + "/" + inputDate.Substring(4, 4);
                }
                else if (inputDate.Length == 10)
                {
                    result.OutDate = inputDate;
                }
                else if (inputDate.Length == 12 && Convert.ToInt64(inputDate.Substring(8, 2)) < 24 && Convert.ToInt64(inputDate.Substring(10, 2)) < 60)
                {
                    result.OutDate = inputDate.Substring(0, 2) + "/" + inputDate.Substring(2, 2) + "/" + inputDate.Substring(4, 4) + " " + inputDate.Substring(8, 2) + ":" + inputDate.Substring(10, 2);
                }
                else
                {
                    
                    result.Message = ResourceMessage.NhapNgaySinhKhongDungDinhDang;
                }

                if (!String.IsNullOrEmpty(result.OutDate) && String.IsNullOrEmpty(result.Message))
                {
                    DateTime? dtPatientDob = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(result.OutDate);
                    if (dtPatientDob == null
                         || dtPatientDob.Value == DateTime.MinValue)
                    {
                        result.Message = ResourceMessage.NhapNgaySinhKhongDungDinhDang;
                        result.OutDate = "";
                    }
                    else if (dtPatientDob != null
                         && dtPatientDob.Value != null
                         && dtPatientDob.Value.Date > DateTime.Now.Date)
                    {
                        result.Message = ResourceMessage.ThongTinNgaySinhPhaiNhoHonNgayHienTai;
                    }
                    else if (CheckIsChild(dtPatientDob.Value) && inputDate.Length < 8)
                    {
                        result.Message = ResourceMessage.YeuCauNhapDayDuNgayThangNamSinhVoiBNDuoi6Tuoi;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
