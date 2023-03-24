using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingsoft.Utils.Graphics.Ascii.UI
{
    public class UIComponent
    {
        private uint Width { get; set; }
        private uint Height { get; set; }

        public UIComponent(uint width = 0, uint height = 0)
        {
            Width = width;
            Height = height;
        }
    }
}
