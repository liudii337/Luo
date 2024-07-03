using Luo.Shared.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luo.Shared.Service
{
    public class CloudService : IService
    {
        //private List<KeyValuePair<string, string>> GetDefaultParam()
        //{
        //    var param = new List<KeyValuePair<string, string>>();
        //    param.Add(new KeyValuePair<string, string>("client_id", Request.AppKey));
        //    return param;
        //}
        private async Task<string> HttpRequestAsync(string url)
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");
            var response = await http.GetAsync(url);
            http.Dispose();
            return response.Content.ReadAsStringAsync().Result;
        }

        internal async Task<string> GetVolDetailHtmlAsync(string url)
        {
            return await HttpRequestAsync(url);
        }

        internal async Task<string> GetVolsAsync(int page, int pageCount, CancellationToken token, string url)
        {
            //var param = GetDefaultParam();
            //param.Add(new KeyValuePair<string, string>("page", page.ToString()));
            //param.Add(new KeyValuePair<string, string>("per_page", pageCount.ToString()));

            var result = await HttpRequestAsync(string.Format("{0}{1}",url,page));
            return result;
        }

        //internal async Task<string> GetImageDetailAsync(string id, CancellationToken token)
        //{
        //    //var param = GetDefaultParam();
        //    //var url = Request.AppendParamsToUrl(Request.GetImageDetail + id, param);

        //    var result = await HttpRequestSender.SendGetRequestAsync(url, token);
        //    return result;
        //}

        private async Task<string> HttpRequestAsync_q(string url)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = true // 允许自动重定向
            };

            using (HttpClient http = new HttpClient(handler))
            {
                // 设置请求头，模拟浏览器
                http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36 Edg/124.0.0.0");
                http.DefaultRequestHeaders.Add("Connection", "keep-alive");
                http.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                http.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en-GB;q=0.8,en;q=0.7,en-US;q=0.6");
                // 发送GET请求
                HttpResponseMessage response = await http.GetAsync(url);
                response.EnsureSuccessStatusCode();

                http.Dispose();
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        internal async Task<string> GetVolDetailHtmlAsync_w(string url)
        {
            var result = await HttpRequestAsync_q(url);
            return result;
        }

        internal async Task<string> GetLrcStringAsync(string url)
        {
            var result = await HttpRequestAsync_q(url);
            return result;
        }

        internal async Task<string> GetVolsAsync_w(int page, int pageCount, CancellationToken token, string url)
        {
            var result = await HttpRequestAsync_q(string.Format("{0}{1}", url, Request.RequestParse(page)));
            return result;
        }

        internal async Task<string> GetNumVolsAsync_w(int page, int pageCount, CancellationToken token, string url)
        {
            if (page == 1)
            {
                var result = await HttpRequestAsync_q(url);
                return result;
            }
            else
            {
                var result = await HttpRequestAsync_q(url + page);
                return result;
            }
        }

        internal async Task<string> GetTagVolsAsync_w(int page, int pageCount, CancellationToken token, string url)
        {
            if(page==1)
            {
                var result = await HttpRequestAsync_q(url);
                return result;
            }
            else
            {
                var result = await HttpRequestAsync_q(url + page);
                return result;
            }
        }
    }
}
