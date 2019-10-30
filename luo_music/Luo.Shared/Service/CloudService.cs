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

        private async Task<string> HttpRequestAsync_w(string url)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var http = new HttpClient(handler);
            http.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");
            var response = await http.GetAsync(url);
            http.Dispose();
            return response.Content.ReadAsStringAsync().Result;
        }

        internal async Task<string> GetVolDetailHtmlAsync_w(string url)
        {
            var result = await HttpRequestAsync_w(url);
            return result;
        }

        internal async Task<string> GetVolsAsync_w(int page, int pageCount, CancellationToken token, string url)
        {
            var result = await HttpRequestAsync_w(string.Format("{0}{1}", url, Request.RequestParse(page)));
            return result;
        }
    }
}
