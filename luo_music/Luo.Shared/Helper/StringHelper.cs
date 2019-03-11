using System;
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

    }
}
