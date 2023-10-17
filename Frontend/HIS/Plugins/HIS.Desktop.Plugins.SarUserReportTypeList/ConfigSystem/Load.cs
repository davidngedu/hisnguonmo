﻿using Inventec.Common.XmlConfig;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SarUserReportTypeList
{
    public class Load
    {
        public static void Init()
        {
            try
            {
                XmlApplicationConfig ApplicationConfig = null;
                string filePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                string pathXmlFileConfig = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"ConfigSystem.xml");
                string lang = (LanguageManager.GetLanguage() == LanguageManager.GetLanguageVi() ? "Vi" : "En");
                ApplicationConfig = new Inventec.Common.XmlConfig.XmlApplicationConfig(pathXmlFileConfig, lang);
                if (ApplicationConfig != null)
                {
                   
                    try
                    {
                        ConfigSystems.URI_API_SDA = (ApplicationConfig.GetKeyValue(ConfigKeys.EXE_CONFIG_KEY__SDA_BASE_URI) ?? "").ToString();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server SDA. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ConfigKeys.EXE_CONFIG_KEY__SDA_BASE_URI), ConfigKeys.EXE_CONFIG_KEY__SDA_BASE_URI));
                    }
                    try
                    {
                        ConfigSystems.URI_API_SAR = (ApplicationConfig.GetKeyValue(ConfigKeys.EXE_CONFIG_KEY__SAR_BASE_URI) ?? "").ToString();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server SAR. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ConfigKeys.EXE_CONFIG_KEY__SAR_BASE_URI), ConfigKeys.EXE_CONFIG_KEY__SAR_BASE_URI));
                    }
                    try
                    {
                        ConfigSystems.URI_API_ACS = (ApplicationConfig.GetKeyValue(ConfigKeys.EXE_CONFIG_KEY__ACS_BASE_URI) ?? "").ToString();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server ACS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ConfigKeys.EXE_CONFIG_KEY__ACS_BASE_URI), ConfigKeys.EXE_CONFIG_KEY__ACS_BASE_URI));
                    }
                    try
                    {
                        ConfigSystems.URI_API_FSS = (ApplicationConfig.GetKeyValue(ConfigKeys.EXE_CONFIG_KEY__FSS_BASE_URI) ?? "").ToString();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server FSS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ConfigKeys.EXE_CONFIG_KEY__FSS_BASE_URI), ConfigKeys.EXE_CONFIG_KEY__FSS_BASE_URI));
                    }
                   
                    try
                    {
                        ConfigSystems.URI_API_TOS = (ApplicationConfig.GetKeyValue(ConfigKeys.EXE_CONFIG_KEY__TOS_BASE_URI) ?? "").ToString();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server TOS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ConfigKeys.EXE_CONFIG_KEY__TOS_BASE_URI), ConfigKeys.EXE_CONFIG_KEY__TOS_BASE_URI));
                    }

                    try
                    {
                        ConfigSystems.URI_API_MOS = (ApplicationConfig.GetKeyValue(ConfigKeys.EXE_CONFIG_KEY__MOS_BASE_URI) ?? "").ToString();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server TOS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ConfigKeys.EXE_CONFIG_KEY__TOS_BASE_URI), ConfigKeys.EXE_CONFIG_KEY__TOS_BASE_URI));
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
