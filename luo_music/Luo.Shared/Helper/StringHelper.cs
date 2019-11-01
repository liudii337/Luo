﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luo.Shared.Helper
{
    public static class StringHelper
    {
        public static string SpliteForm2String(string a,string b1,string b2)
        {
            int b11 = a.IndexOf(b1);//找a的位置
            int b21 = a.IndexOf(b2);//找b的位置
            return (a.Substring(b11 + 1)).Substring(0, b21 - b11 - 1);
        }

        public static string DescripitionParse(this string s)
        {
            if(s[s.Length-1] == '\n')
            {
                s = s.Remove(s.Length - 1);
            }
            if (s[s.Length - 1] == '\n')
            {
                s = s.Remove(s.Length - 1);
            }
            return s.Replace("&nbsp;", "").Replace("&#39;", "").Replace("&ldquo;", "").Replace("&rdquo;", "");
        }

        public static string HtmlParse(this string s)
        {
            int j = 0;
            foreach(char i in s)
            {
                if (i == ' ' || i == '\n')
                { j = j + 1; }
                else
                    break;
            }
            s = s.Remove(0, j);
            s = s.Replace("<br><br>", "\n");
            s = s.Replace("<br>", "\n");
            return s;
        }

        public static string HtmlParse_w(this string s)
        {
            s = s.Replace("\n", "").Replace("\r", "").Replace(" ", "");
            s = s.Replace("<br>", "\n");
            return s;
        }
    }
}
