using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luo.Shared.Data
{
    public class LuoVolTag : ModelBase
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged(() => Name);
                }
            }
        }

        private string src;
        public string Src
        {
            get
            {
                return src;
            }
            set
            {
                if (src != value)
                {
                    src = value;
                    RaisePropertyChanged(() => Src);
                }
            }
        }
    }
}
