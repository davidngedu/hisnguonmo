﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.XmlCacheMonitor
{
    public class ApplicationStoreLocation
    {
        static string strApplicationDirectory;
        public static string ApplicationDirectory
        {
            get
            {
                if (System.String.IsNullOrEmpty(strApplicationDirectory))
                {
                    string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                    System.UriBuilder uri = new System.UriBuilder(codeBase);
                    string path = System.Uri.UnescapeDataString(uri.Path);
                    strApplicationDirectory = System.IO.Path.GetDirectoryName(path);
                }
                return strApplicationDirectory;
            }
        }

        static string strApplicationStartupPath;
        public static string ApplicationStartupPath
        {
            get
            {
                if (System.String.IsNullOrEmpty(strApplicationStartupPath))
                {
                    strApplicationStartupPath = System.Windows.Forms.Application.StartupPath;
                }
                return strApplicationStartupPath;
            }
        }
    }
}
