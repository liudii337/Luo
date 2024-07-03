using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuoMusic.Pages
{
    public interface INavigableUserControl
    {
        bool Presented { get; set; }

        void OnPresented();

        void OnHide();

        void ToggleAnimation();
    }
}
