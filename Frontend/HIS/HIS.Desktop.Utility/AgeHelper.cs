﻿using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Utility
{
    public class AgeHelper
    {
        public static string CalculateAgeFromYear(long ageYearNumber)
        {
            string tuoi = "";
            try
            {
                DateTime? dtAge = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ageYearNumber);
                long age = DateTime.UtcNow.Year - dtAge.Value.Year;
                if (age <= 0)
                {
                    age = 1;
                }
                tuoi = string.Format("{0:00}", age);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                tuoi = "";
            }
            return tuoi;
        }

        public static string CalculateAgeNew(string parameters)
        {
            string result = System.String.Empty;
            try
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to FuncCalculatePatientAge user-defined function");

                long dob = Convert.ToInt64(parameters[0]);
                string caption__Tuoi = "tuổi";
                string caption__ThangTuoi = "tháng tuổi";
                string caption__NgayTuoi = "ngày tuổi";
                string caption__GioTuoi = "giờ tuổi";
                if (parameters.Count() > 1)
                {
                    caption__Tuoi = Convert.ToString(parameters[1]);
                }
                if (parameters.Count() > 2)
                {
                    caption__ThangTuoi = Convert.ToString(parameters[2]);
                }
                if (parameters.Count() < 5)
                {
                    if (parameters.Count() > 3)
                    {
                        caption__GioTuoi = Convert.ToString(parameters[3]);
                    }
                }
                else
                {
                    if (parameters.Count() > 3)
                    {
                        caption__NgayTuoi = Convert.ToString(parameters[3]);
                    }
                    if (parameters.Count() > 4)
                    {
                        caption__GioTuoi = Convert.ToString(parameters[4]);
                    }
                }

                if (dob > 0)
                {
                    System.DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob).Value;
                    if (dtNgSinh == System.DateTime.MinValue) throw new ArgumentNullException("dtNgSinh");

                    TimeSpan diff__hour = (System.DateTime.Now - dtNgSinh);
                    TimeSpan diff__month = (System.DateTime.Now.Date - dtNgSinh.Date);

                    //- Dưới 24h: tính chính xác đến giờ.
                    double hour = diff__hour.TotalHours;
                    if (hour < 24)
                    {
                        result = ((int)hour + " " + caption__GioTuoi);
                    }
                    else
                    {
                        long tongsogiay__hour = diff__hour.Ticks;
                        System.DateTime newDate__hour = new System.DateTime(tongsogiay__hour);
                        int month__hour = ((newDate__hour.Year - 1) * 12 + newDate__hour.Month - 1);
                        if (parameters.Count() == 5 && month__hour == 0)
                        {
                            //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                            result = ((int)diff__month.TotalDays + " " + caption__NgayTuoi);
                        }
                        else
                        {
                            long tongsogiay = diff__month.Ticks;
                            System.DateTime newDate = new System.DateTime(tongsogiay);
                            int month = ((newDate.Year - 1) * 12 + newDate.Month - 1);
                            if (month == 0)
                            {
                                //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                                result = ((int)diff__month.TotalDays + " " + caption__NgayTuoi);
                            }
                            else
                            {
                                //- Dưới 72 tháng tuổi: tính chính xác đến tháng như hiện tại
                                if (month < 72)
                                {
                                    result = (month + " " + caption__ThangTuoi);
                                }
                                //- Trên 72 tháng tuổi: tính chính xác đến năm: tuổi= năm hiện tại - năm sinh
                                else
                                {
                                    int year = System.DateTime.Now.Year - dtNgSinh.Year;
                                    result = (year + " " + caption__Tuoi);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
            }

            return result;
        }
    }
}
