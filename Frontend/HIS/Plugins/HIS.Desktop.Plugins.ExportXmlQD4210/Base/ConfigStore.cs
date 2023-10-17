﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXmlQD4210.Base
{
    class ConfigStore
    {
        private static string FolderSaveXml = System.Configuration.ConfigurationSettings.AppSettings["HIS.Desktop.Plugins.InsuranceExpertise.FolderSaveXml"];

        public static string GetFolderSaveXml
        {
            get
            {
                if (string.IsNullOrEmpty(FolderSaveXml))
                {
                    FolderSaveXml = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }
                return FolderSaveXml;
            }
        }
    }
}
