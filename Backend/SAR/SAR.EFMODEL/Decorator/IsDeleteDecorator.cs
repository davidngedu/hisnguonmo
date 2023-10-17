﻿using System;
using System.Reflection;

namespace SAR.EFMODEL.Decorator
{
    public class IsDeleteDecorator
    {
        public static void Set<RAW>(RAW raw)
        {
            try
            {
                PropertyInfo pi = typeof(RAW).GetProperty("IS_DELETE");
                pi.SetValue(raw, (short)0);
            }
            catch (Exception)
            {

            }
        }
    }
}
