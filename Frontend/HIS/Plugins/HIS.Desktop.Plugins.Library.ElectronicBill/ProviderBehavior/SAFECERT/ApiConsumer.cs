﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.SAFECERT
{
    class ApiConsumer
    {
        public static T CreateRequest<T>(string baseUri, string requestUri, object sendData)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = new TimeSpan(0, 0, 90);

                System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                HttpResponseMessage resp = null;
                string sendJsonData = JsonConvert.SerializeObject(sendData);
                Inventec.Common.Logging.LogSystem.Info("_____sendJsonData : " + sendJsonData);

                //string extension = baseUri.Substring(baseUri.IndexOf('/', baseUri.IndexOf("//") + 2));
                string fullrequestUri = requestUri;
                //if (!requestUri.Contains(extension))
                //{
                //    fullrequestUri = CombileUrl(extension, requestUri);
                //}

                resp = client.PostAsync(fullrequestUri, new StringContent(sendJsonData, Encoding.UTF8, "application/json")).Result;

                if (resp == null || !resp.IsSuccessStatusCode)
                {
                    int statusCode = resp.StatusCode.GetHashCode();
                    Inventec.Common.Logging.LogSystem.Error("fullrequestUri: " + fullrequestUri);
                    throw new Exception(string.Format("Loi khi goi API: {0}{1}. StatusCode: {2}", baseUri, requestUri, statusCode));
                }

                string responseData = resp.Content.ReadAsStringAsync().Result;
                //Inventec.Common.Logging.LogSystem.Info("__________________api responseData: " + responseData);

                T data = JsonConvert.DeserializeObject<T>(responseData);
                if (data == null)
                {
                    throw new Exception(string.Format("Loi khi goi API. Response {0}:", responseData));
                }
                return data;
            }
        }

        internal static string CombileUrl(params string[] data)
        {
            string result = "";
            List<string> pathUrl = new List<string>();
            for (int i = 0; i < data.Length; i++)
            {
                pathUrl.Add(data[i].Trim('/'));
            }

            result = string.Join("/", pathUrl);

            //Inventec.Common.Logging.LogSystem.Debug("CombileUrl:" + result);
            return result;
        }
    }
}
