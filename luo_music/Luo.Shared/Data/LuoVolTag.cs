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

        private string keysrc;
        public string KeySrc
        {
            get
            {
                return keysrc;
            }
            set
            {
                if (keysrc != value)
                {
                    keysrc = value;
                    RaisePropertyChanged(() => KeySrc);
                }
            }
        }

        private string imgsrc;
        public string ImgSrc
        {
            get
            {
                return imgsrc;
            }
            set
            {
                if (imgsrc != value)
                {
                    imgsrc = value;
                    RaisePropertyChanged(() => ImgSrc);
                }
            }
        }
    }
}
