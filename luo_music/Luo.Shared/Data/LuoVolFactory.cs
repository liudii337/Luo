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
        public ObservableCollection<LuoVol> GetVols(string html)
        {
            // 暂用新的API
            return GetVolListFromHtml_w(html);
        }

        public static ObservableCollection<LuoVol>  GetVolListFromHtml(string html)
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

                //if (vol.VolUrl != null)
                //{
                //    var http_detail = new HttpClient();
                //    http_detail.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");
                //    var http_response = await http_detail.GetAsync(vol.VolUrl);

                //    string result = http_response.Content.ReadAsStringAsync().Result;
                //    vol.GetDetailVol(result);
                //}

                vollist.Add(vol);

            }

            return vollist;
        }

        public static ObservableCollection<LuoVolTag> GetVolTagListFromHtml(string html)
        {
            var voltaglist = new ObservableCollection<LuoVolTag>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            //查找节点
            var titleNode = doc.DocumentNode.SelectSingleNode("//div[@class='pagenav-wrapper']");
            var list = titleNode.SelectNodes("./a[@class='item']");

            foreach (var i in list)
            {
                var _src = i.GetAttributeValue("href", "");
                var _name = i.InnerText;
                voltaglist.Add(new LuoVolTag()
                {
                    Name=_name,
                    KeySrc=_src
                });
            }

            return voltaglist;
        }

        public static ObservableCollection<LuoVolTag> GetVolTagList()
        {
            var voltaglist = new ObservableCollection<LuoVolTag>();

            voltaglist.Add(new LuoVolTag() { Name = "摇滚", KeySrc = "rock", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5c75756c47c2f.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "另类", KeySrc = "alternative", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5c66bc566af6f.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "民谣", KeySrc = "folk", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5b462da311903.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "流行", KeySrc = "pop", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5a6631d52b3c2.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "电子", KeySrc = "electronic", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5a539ddc66e86.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "英伦", KeySrc = "britpop", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5a3560ee1c750.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "古典", KeySrc = "classical", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/58dd1a7d983b7.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "暗潮", KeySrc = "neo-wave", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/59ceb6e4d65e9.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "爵士", KeySrc = "jazz", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/599d8afcb0c3a.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "金属", KeySrc = "metal", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5ac68b6c67a01.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "后摇", KeySrc = "post-rock", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5c45de0c05832.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "迷幻摇滚", KeySrc = "psychedelic-rock", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/59b6c378a8ecc.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "朋克", KeySrc = "punk", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5a2be90092c79.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "说唱", KeySrc = "hip-hop", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5aaf89276cd7b.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "世界音乐", KeySrc = "world", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5981d54ad0002.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "硬核", KeySrc = "hardcore", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5919dd0de8923.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "氛围", KeySrc = "ambient", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5afc11b9a2f22.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "原声", KeySrc = "ost", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5b1401328623d.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "雷鬼", KeySrc = "reggae", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5895ce5348731.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "乡村", KeySrc = "country", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5818b04051236.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "蓝调", KeySrc = "blues", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5736055312e75.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "实验", KeySrc = "experimental", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5a810230daa52.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "华语流行", KeySrc = "mandarin-pop", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/586ebc4d7d8d8.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "后朋克", KeySrc = "post-punk", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5908c47b4707c.jpg!/fwfh/640x452" });
            voltaglist.Add(new LuoVolTag() { Name = "史诗", KeySrc = "epic", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5908c47b4707c.jpg!/fwfh/640x452" });

            //voltaglist.Add(new LuoVolTag() { Name = "人声", KeySrc = "vocal", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/57862dc457a86.jpg!/fwfh/640x452" });
            //voltaglist.Add(new LuoVolTag() { Name = "品牌", KeySrc = "brand", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5a02c66d3722b.jpg!/fwfh/640x452" });

            return voltaglist;
        }

        public static ObservableCollection<LuoVolTag> GetVolNumList()
        {
            var volnumlist = new ObservableCollection<LuoVolTag>();

            volnumlist.Add(new LuoVolTag() { Name = "999-901", KeySrc = "901_1000.html", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5c75756c47c2f.jpg!/fwfh/640x452" });
            volnumlist.Add(new LuoVolTag() { Name = "900-801", KeySrc = "801_900.html", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5c66bc566af6f.jpg!/fwfh/640x452" });
            volnumlist.Add(new LuoVolTag() { Name = "800-701", KeySrc = "701_800.html", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5b462da311903.jpg!/fwfh/640x452" });
            volnumlist.Add(new LuoVolTag() { Name = "700-601", KeySrc = "601_700.html", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5a6631d52b3c2.jpg!/fwfh/640x452" });
            volnumlist.Add(new LuoVolTag() { Name = "600-501", KeySrc = "501_600.html", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5a539ddc66e86.jpg!/fwfh/640x452" });
            volnumlist.Add(new LuoVolTag() { Name = "500-401", KeySrc = "401_500.html", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5a3560ee1c750.jpg!/fwfh/640x452" });
            volnumlist.Add(new LuoVolTag() { Name = "400-301", KeySrc = "301_400.html", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/58dd1a7d983b7.jpg!/fwfh/640x452" });
            volnumlist.Add(new LuoVolTag() { Name = "300-201", KeySrc = "201_300.html", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/59ceb6e4d65e9.jpg!/fwfh/640x452" });
            volnumlist.Add(new LuoVolTag() { Name = "200-101", KeySrc = "101_200.html", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/599d8afcb0c3a.jpg!/fwfh/640x452" });
            volnumlist.Add(new LuoVolTag() { Name = "100-1", KeySrc = "1_100.html", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5ac68b6c67a01.jpg!/fwfh/640x452" });
            //volnumlist.Add(new LuoVolTag() { Name = "音乐电台", KeySrc = "r/", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5ac68b6c67a01.jpg!/fwfh/640x452" });
            //volnumlist.Add(new LuoVolTag() { Name = "其他", KeySrc = "e/", ImgSrc = "http://img-cdn2.luoo.net/pics/vol/5ac68b6c67a01.jpg!/fwfh/640x452" });

            return volnumlist;
        }


        public async static Task<ObservableCollection<LuoVol>> getlist()
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36");
            var response = await http.GetAsync("http://www.luoo.net/tag/?p=90");
            http.Dispose();
            string result = response.Content.ReadAsStringAsync().Result;

            GetVolTagListFromHtml(result);
            return GetVolListFromHtml(result);
        }

        public static ObservableCollection<LuoVol> GetVolListFromHtml_w(string html)
        {
            var vollist = new ObservableCollection<LuoVol>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            //查找节点
            var list = doc.DocumentNode.SelectNodes("//div[@class='thumbnail theborder']");

            foreach (var i in list)
            {
                //http://luoow.wxwenku.com/100/cover.jpg
                //http://luoow.wxwenku.com/99/cover_min.jpg
                var cover = i.SelectSingleNode("./a/img").GetAttributeValue("src", "").Replace("_min", "");

                var Node1 = i.SelectSingleNode("./div/p/a");

                //var volnum = Node1.GetAttributeValue("href", "").Replace("/", "");
                var volnum = StringHelper.SpliteForm2String(Node1.InnerText, ".", " ");
                var volurl = "https://www.luoow.com" + Node1.GetAttributeValue("href", "");
                var title = Node1.GetAttributeValue("title", "");

                //Special for /e and /r


                var vol = new LuoVol(cover, volnum, volurl, title);

                vollist.Add(vol);
            }

            return vollist;
        }

    }
}
