using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Kingsoft.Utils.Graphics.Ascii
{
    public class AsciiMap2D
    {
        private Dictionary<ulong, AsciiChunk2D> Map { get; set; }
        public ulong Width { get; private set; }
        public ulong Height { get; private set; }
        public uint ChunkWidth { get; private set; }
        public uint ChunkHeight { get; private set; }

        public event Action<AsciiMap2D, ulong, ulong> GetChunkEvent;
        public event Action<AsciiMap2D, ulong, ulong, uint, uint> GetTileEvent;

        public AsciiMap2D()
        {
            Map = new Dictionary<ulong, AsciiChunk2D>();
        }


        public AsciiMap2D SetSize(ulong width, ulong height)
        {
            Width = width;
            Height = height;
            return this;
        }

        public AsciiMap2D SetChunkSize(uint width, uint height)
        {
            ChunkWidth = width;
            ChunkHeight = height;
            Map.Values.All(val => { val.SetSize(width, height); return true; });
            return this;
        }

        public AsciiMap2D SetChunk(ulong cx, ulong cy, AsciiChunk2D chunk)
        {
            ulong index = cy * Width + cx;
            if (!Map.ContainsKey(index))
                Map.Add(index, chunk);
            else
                Map[index] = chunk;
            return this;
        }

        public AsciiMap2D SetTile(ulong cx, ulong cy, uint x, uint y, AsciiTile2D tile)
        {
            ulong index = cy * Width + cx;
            if (!Map.ContainsKey(index))
                Map.Add(index, new AsciiChunk2D(ChunkWidth, ChunkHeight).SetTile(x, y, tile));
            else
                Map[index].SetTile(x, y, tile);
            return this;
        }

        public AsciiMap2D SetTile((ulong, ulong, uint, uint) pos, AsciiTile2D tile) => SetTile(pos.Item1, pos.Item2, pos.Item3, pos.Item4, tile);
        public AsciiMap2D SetTile(BigInteger x, BigInteger y, AsciiTile2D tile) => SetTile(FromGlobalCoords(x, y, ChunkWidth, ChunkHeight), tile);


        public AsciiChunk2D GetChunk(ulong cx, ulong cy)
        {
            GetChunkEvent?.Invoke(this, cx, cy);
            return Map.ContainsKey(cy * Width + cx) ? Map[Math.Min(cy, Height - 1) * Width + Math.Min(cx, Width - 1)] : new AsciiChunk2D();
        }
        public AsciiTile2D GetTile(ulong cx, ulong cy, uint x, uint y)
        {
            if (cx < Width && cy < Height && x < ChunkWidth && y < ChunkHeight)
            {
                GetTileEvent?.Invoke(this, cx, cy, x, y);
                return Map.ContainsKey(Math.Min(cy, Height - 1) * Width + Math.Min(cx, Width - 1)) ? GetChunk(cx, cy).GetTile(x, y) : new AsciiTile2D();
            }
            else
                return new AsciiTile2D();
        }
        public AsciiTile2D GetTile((ulong, ulong, uint, uint) pos) => GetTile(pos.Item1, pos.Item2, pos.Item3, pos.Item4);
        public AsciiTile2D GetTile(BigInteger x, BigInteger y) => GetTile((ulong)((x - (x % ChunkWidth)) / ChunkWidth), (ulong)((y - (y % ChunkHeight)) / ChunkHeight), (uint)(x % ChunkWidth), (uint)(y % ChunkHeight));


        public static (ulong, ulong, uint, uint) FromGlobalCoords(BigInteger x, BigInteger y, uint cw, uint ch)
        {
            uint _x = (uint)(x % cw), _y = (uint)(y % ch);
            ulong cx = (ulong)((x - _x) / cw), cy = (ulong)((y - _y) / ch);
            return (cx, cy, _x, _y);
        }

        public static (BigInteger, BigInteger) FromChunkCoords((ulong, ulong, uint, uint) cord, uint cw, uint ch)
        {
            BigInteger x = cord.Item1 * cw + cord.Item3;
            BigInteger y = cord.Item2 * ch + cord.Item4;
            return (x, y);
        }
    }
}
