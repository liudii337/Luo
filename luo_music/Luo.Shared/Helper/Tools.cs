using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luo.Shared.Helper
{
    public static class Tools
    {
        public static bool AlmostEqualTo(this double value1, double value2)
        {
            return Math.Abs(value1 - value2) < 0.0000001;
        }
    }
}
