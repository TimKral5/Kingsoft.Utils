using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingsoft.Utils.Graphics.Ascii
{
    public class AsciiChunk2D
    {
        private Dictionary<uint, AsciiTile2D> Tileset { get; set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }

        public AsciiChunk2D(uint width, uint height)
        {
            Tileset = new Dictionary<uint, AsciiTile2D>();
            Width = width;
            Height = height;
        }

        public AsciiChunk2D() => new AsciiChunk2D(0, 0);

        public AsciiChunk2D SetSize(uint width, uint height)
        {
            Width = width;
            Height = height;
            return this;
        }

        public AsciiChunk2D SetTile(uint x, uint y, AsciiTile2D tile)
        {
            uint index = y * Width + x;

            if (!Tileset.TryGetValue(index, out var _))
                Tileset.Add(index, tile);
            else
                Tileset[y * Width + x] = tile;
            return this;
        }

        public AsciiTile2D GetTile(uint x, uint y) => (x < Width && y < Height && Tileset.ContainsKey(y * Width + x)) ? Tileset[y * Width + x] : new AsciiTile2D();
    }
}
