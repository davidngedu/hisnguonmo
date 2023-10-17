﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Utility
{
    public class MemoryManagement
    {
        [DllImportAttribute("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet =
        CharSet.Ansi, SetLastError = true)]

        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int
        maximumWorkingSetSize);

        private static readonly string WindowsQuotaHandleKey = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Windows";//GDIProcessHandleQuota
        private static readonly string WindowsQuotaHandleKeyExt = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Windows";//GDIProcessHandleQuota     


        [DllImport("User32")]
        extern public static int GetGuiResources(IntPtr hProcess, int uiFlags);

        public static int GetGuiResourcesGDICount()
        {
            Process process = Process.GetCurrentProcess();
            return process != null ? GetGuiResources(process.Handle, 0) : 0;
        }

        public static int GetGuiResourcesUserCount()
        {
            Process process = Process.GetCurrentProcess();
            return process != null ? GetGuiResources(process.Handle, 1) : 0;
        }
        public static int GetHandleCount()
        {
            Process process = Process.GetCurrentProcess();
            return process != null ? process.HandleCount : 0;
        }

        public static int GDIProcessHandleQuota()
        {
            int value = 0;
            try
            {
                RegistryKey keyReg = Registry.LocalMachine.OpenSubKey(WindowsQuotaHandleKey, true);
                if (keyReg == null)
                {
                    keyReg = Registry.LocalMachine.OpenSubKey(WindowsQuotaHandleKeyExt, true);
                }
                string vl = (keyReg != null ? (keyReg.GetValue("GDIProcessHandleQuota", "") ?? "") : "0").ToString();
                value = !String.IsNullOrEmpty(vl) ? int.Parse(vl) : 0;
            }
            catch (Exception ex)
            {
                //Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return value;
        }

        public static int USERProcessHandleQuota()
        {
            int value = 0;
            try
            {
                RegistryKey keyReg = Registry.LocalMachine.OpenSubKey(WindowsQuotaHandleKey, true);
                if (keyReg == null)
                {
                    keyReg = Registry.LocalMachine.OpenSubKey(WindowsQuotaHandleKeyExt, true);
                }
                string vl = (keyReg != null ? (keyReg.GetValue("USERProcessHandleQuota", "") ?? "") : "0").ToString();
                value = !String.IsNullOrEmpty(vl) ? int.Parse(vl) : 0;
            }
            catch (Exception ex)
            {
                //Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return value;
        }

        public static void FlushMemory()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
                }
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Error(exx);
            }
        }
    }
}
