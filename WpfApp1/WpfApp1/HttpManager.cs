using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class HttpManager
    {
        private HttpManager() { }

        private static HttpManager instance;

        public static HttpManager GetSingleon()
        {
            if (instance == null) instance = new HttpManager();
            return instance;
        }

        private static string ip_Address = "";
        private static readonly int port = 80;

        public async void Client_Connection()
        {
            try
            {
                HttpClient client = new HttpClient();
                //HttpResponseMessage response = await client.GetAsync(ip_Address);
                //response.EnsureSuccessStatusCode(); // 如果html响应不成功, 这个方法会引发异常
                //string responseBody = await response.Content.ReadAsStringAsync();
                string responseBody = await client.GetStringAsync(ip_Address);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        /// <summary>
        /// 从Server加载nodes
        /// </summary>
        public async void Client_GetNodesByServer()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage task = await client.GetAsync(ip_Address);
                task.EnsureSuccessStatusCode();

                // 接受服务器返回
                byte[] data = await task.Content.ReadAsByteArrayAsync();

                LocalInfo.GetSingle().LoadXML(data);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        /// <summary>
        /// 保存nodes到Server
        /// </summary>
        public async void Client_SentNodesToServer()
        {
            try
            {
                HttpClient client = new HttpClient();

                byte[] tempData;
                if (LocalInfo.GetSingle().GetXmlBytes(out tempData))
                {
                    ByteArrayContent content = new ByteArrayContent(tempData);
                    HttpResponseMessage task = await client.PostAsync(ip_Address, content);
                    task.EnsureSuccessStatusCode(); // 用来抛异常

                    byte[] data = await task.Content.ReadAsByteArrayAsync();
                    // todo: 判断保存是否成功
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
    }
}
