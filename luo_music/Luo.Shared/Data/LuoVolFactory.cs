using HtmlAgilityPack;
using Luo.Shared.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luo.Shared.Data
{
    public class LuoVolFactory
    {
        public static ObservableCollection<LuoVol> GetVolListFromHtml(string html)
        {
            var vollist = new ObservableCollection<LuoVol>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            //查找节点
            var titleNode = doc.DocumentNode.SelectSingleNode("//div[@class='vol-list']");
            var list = titleNode.SelectNodes("./div[@class='item']");

            foreach (var i in list)
            {
                var cover = i.SelectSingleNode("//img[@class='cover rounded']").GetAttributeValue("src", "");
                var Node1 = i.SelectSingleNode("//a[@class='name']");

                var volurl = Node1.GetAttributeValue("href", "");
                var title = Node1.GetAttributeValue("title", "");

                var volnum = StringHelper.SpliteForm2String(Node1.InnerText, "."," ");

                var commentcount = i.SelectSingleNode("//span[@class='comments']").InnerText.Replace("\n", "").Replace("\t", "");
                var favdcount = i.SelectSingleNode("//span[@class='favs']").InnerText.Replace("\n", "").Replace("\t", "");

                var vol = new LuoVol()
                {
                    Cover = cover,
                    VolNum = volnum,
                    VolUrl = volurl,
                    Title = title,
                    CommentCount = commentcount,
                    FavdCount = favdcount
                };
                vollist.Add(vol);
            }

            return vollist;
        }
    }
}
