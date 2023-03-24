using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingsoft.Utils.Graphics.Ascii
{
    public class AsciiScreenBuffer
    {
        private Dictionary<ulong, AsciiTile2D> Screen { get; set; }
        private uint Width { get; set; }
        private uint Height { get; set; }

        public AsciiScreenBuffer()
        {
            Screen = new Dictionary<ulong, AsciiTile2D>();
        }

        public void Clear() => Screen = new Dictionary<ulong, AsciiTile2D>();

        public AsciiScreenBuffer SetSize(uint width, uint height)
        {
            Width = width;
            Height = height;
            return this;
        }

        public AsciiScreenBuffer SetPixel(uint x, uint y, AsciiTile2D tile)
        {
            ulong index = y * Width + x;
            if (!Screen.ContainsKey(x))
                Screen.Add(x, tile);
            else 
                Screen[index] = tile;
            return this;
        }

        public AsciiTile2D GetPixel(uint x, uint y)
        {
            ulong index = y * Width + x;
            if (Screen.ContainsKey(index))
                return Screen[index];
            else
                return null;
        }

        public bool EqualToPixel(uint x, uint y, AsciiTile2D tile)
        {
            ulong index = y * Width + x;
            AsciiTile2D pixel = null;
            if (Screen.ContainsKey(index))
                pixel = Screen[index];
            if (pixel != null)
                return pixel.Equals(tile);
            else 
                return false;
        }
    }
}
