﻿using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.UC.Login.Base;
using SDA.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.UserInfo
{
    internal partial class SdaEventLogCreate : BusinessBase
    {
        public SdaEventLogCreate()
            : base()
        {

        }

        public SdaEventLogCreate(CommonParam paramCreate)
            : base(paramCreate)
        {

        }

        public void Create(string loginName, long? eventLogTypeId, bool? isSuccess, string message)
        {
            try
            {
                SdaEventLogSDO data = new SdaEventLogSDO();
                data.EventLogTypeId = eventLogTypeId;
                data.IsSuccess = isSuccess;
                data.Description = message;
                data.Ip = GetIpLocal();
                data.LogginName = loginName;
                data.EventTime = Inventec.Common.DateTime.Get.Now();
                data.AppCode = GlobalVariables.APPLICATION_CODE;
                Create(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true; //Khong set param o day ma chi logging do viec log event la 1 viec phu khong qua quan trong
            }
        }

        string GetIpLocal()
        {
            string ip = "";
            try
            {
                // get local IP addresses
                System.Net.IPAddress[] localIPs = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());
                if (localIPs != null && localIPs.Length > 0)
                {
                    foreach (var item in localIPs)
                    {
                        if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            ip = item.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true; //Khong set param o day ma chi logging do viec log event la 1 viec phu khong qua quan trong
            }

            return ip;
        }

        private bool Create(SdaEventLogSDO data)
        {
            bool result = false;
            try
            {
                //Inventec.Core.ApiResultObject<bool> aro = ApiConsumerStore.SdaConsumer.Post<Inventec.Core.ApiResultObject<bool>>("/api/SdaEventLog/Create", param, data);
                var aro = new BackendAdapter(param).Post<bool>("/api/SdaEventLog/Create", ApiConsumer.ApiConsumers.SdaConsumer, data, param);
                if (aro)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true; //Khong set param o day ma chi logging do viec log event la 1 viec phu khong qua quan trong
                result = false;
            }
            return result;
        }
    }
}
