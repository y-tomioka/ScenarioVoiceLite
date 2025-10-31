using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Policy;
using System.IO;
using System.Net;
using NLog;

namespace STVoice
{
    internal class WebControl
    {
        private static HttpClient httpClient = new HttpClient();
        private Logger m_logger;

        public WebControl(Logger logger)
        {
            m_logger = logger;
            //httpClient.Timeout = TimeSpan.FromSeconds(100);
        }

        public string get(string url)
        {
            Task<HttpResponseMessage> ret = httpClient.GetAsync(url);
            bool result = false;
            try
            {
                result = ret.Wait(3000);
            }
            catch (Exception /*ex*/)
            {
                return "";
            }
            if(result == false)
            {
                return "";
            }
            HttpResponseMessage response3 = ret.Result;
            string getData = response3.Content.ReadAsStringAsync().Result;
            return getData;
        }

        public string post(string url, string json)
        {
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> ret = httpClient.PostAsync(url, content);
            HttpResponseMessage response = ret.Result;
            if (response.IsSuccessStatusCode)
            {
                string getData = response.Content.ReadAsStringAsync().Result;
                return getData;
            }
            else
            {
                HttpStatusCode status = response.StatusCode;
                string getData = response.Content.ReadAsStringAsync().Result;
                m_logger.Error("HTTPStausCode:" + status + ",ErrorResponse:" + getData);
                return "";
            }
        }

        public void postAndSave(string url, string json, string path)
        {
            File.Delete(path);

            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> ret = httpClient.PostAsync(url, content);
            HttpResponseMessage response = ret.Result;
            if (!response.IsSuccessStatusCode)
            {
                HttpStatusCode status = response.StatusCode;
                string getData = response.Content.ReadAsStringAsync().Result;
                m_logger.Error("HTTPStausCode:" + status + ",ErrorResponse:" + getData);
                return;
            }

            string newPath = Path.ChangeExtension(path, ".tmp");
            using (var fileStream = System.IO.File.Create(newPath))
            {
                var httpStream = response.Content.ReadAsStreamAsync().Result;
                httpStream.CopyTo(fileStream);
                fileStream.Flush();
            }
            File.Move(newPath, path);
        }
    }
}
