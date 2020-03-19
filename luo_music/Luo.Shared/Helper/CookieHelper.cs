using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace Luo.Shared.Helper
{
    public class CookieHelper
    {
        private static char[] constant =
        {
            '0','1','2','3','4','5','6','7','8','9',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
        };

        public static string randomCoding()
        {
            var idvalue = "";
            var n = 5;
            Random rd = new Random();
            for (int i = 0; i < n; i++)
            {
                idvalue += constant[rd.Next(62)];
            }
            return idvalue;
        }

        public static string ToBase64String(string value)
        {
            if (value == null || value == "")
            { return ""; }
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }


        public static string GetCookiestring()
        {
            var f = "老铁你要的数据都在这里 https://luoow.wxwenku.com/static/down.txt; 我觉得我们可以做个朋友，添加网站中的微信，记得备注: just do it";

            DateTime timeStamp = new DateTime(1970, 1, 1); //得到1970年的时间戳 
            var a = (DateTime.UtcNow.Ticks - timeStamp.Ticks) / 10000000; //注意这里有时区问题，用now就要减掉8个小时
            string timestamp = a.ToString();

            var name = "st";
            var cookie1 = name + "=" + timestamp + "; ";

            var rstr = randomCoding();
            var sts = "sts";
            var sts_value = rstr + ToBase64String((a % 3600 + 1234).ToString());
            var cookie2 = sts + "=" + sts_value;
            return cookie1 + cookie2;

        }
    }
}
