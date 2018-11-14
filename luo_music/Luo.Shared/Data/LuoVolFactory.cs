using HtmlAgilityPack;
using Luo.Shared.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Luo.Shared.Data
{
    public class LuoVolFactory
    {
        public async static Task<ObservableCollection<LuoVol>>  GetVolListFromHtml(string html)
        {
            var vollist = new ObservableCollection<LuoVol>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            //查找节点
            var titleNode = doc.DocumentNode.SelectSingleNode("//div[@class='vol-list']");
            var list = titleNode.SelectNodes("./div[@class='item']");

            foreach (var i in list)
            {
                var cover = i.SelectSingleNode("./a/img").GetAttributeValue("src", "");
                var Node1 = i.SelectSingleNode("./div/a");
                // /html/body/div[3]/div/div[1]/div[2]/div[2]/a/img
                var volurl = Node1.GetAttributeValue("href", "");
                var title = Node1.GetAttributeValue("title", "");

                var volnum = StringHelper.SpliteForm2String(Node1.InnerText, "."," ");

                var commentcount = i.SelectSingleNode("./div/span[1]").InnerText.Replace("\n", "").Replace("\t", "");
                var favdcount = i.SelectSingleNode("./div/span[2]").InnerText.Replace("\n", "").Replace("\t", "");

                var vol = new LuoVol(cover,volnum,volurl,title,commentcount,favdcount);

                if (vol.VolUrl != null)
                {
                    var http_detail = new HttpClient();
                    http_detail.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");
                    var http_response = await http_detail.GetAsync(vol.VolUrl);

                    string result = http_response.Content.ReadAsStringAsync().Result;
                    vol.GetDetailVol(result);

                }

                vollist.Add(vol);

            }

            return vollist;
        }

        public static async void getlist()
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");
            var response = await http.GetAsync("http://www.luoo.net/music");
            http.Dispose();
            string result = response.Content.ReadAsStringAsync().Result;

            await GetVolListFromHtml(result);
        }
    }
}
