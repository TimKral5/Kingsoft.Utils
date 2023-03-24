using Kingsoft.Utils.TypeExtensions.ConsoleExt;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Kingsoft.Utils.Graphics.Ascii
{
    public class AsciiEngine2D
    {
        private AsciiMap2D Map { get; set; }
        private BigInteger CameraX { get; set; }
        private BigInteger CameraY { get; set; }
        private bool SquaredMode { get; set; }

        private AsciiScreenBuffer Buffer { get; set; }
        private bool RenderOverlay { get; set; }
        private AsciiScreenBuffer Overlay { get; set; }

        public AsciiEngine2D(bool squaredMode = false)
        {
            Map = new AsciiMap2D();
            Buffer = new AsciiScreenBuffer();
            SquaredMode = squaredMode;
            RenderOverlay = false;
        }

        public void ToggleOverlay(bool? show = null) => RenderOverlay = (show == null ? !RenderOverlay : (bool)show);

        public AsciiEngine2D SetMap(AsciiMap2D map)
        {
            Map = map;
            return this;
        }

        public AsciiEngine2D SetOverlay(AsciiScreenBuffer overlay)
        {
            Overlay = overlay;
            return this;
        }

        public AsciiEngine2D SetCamera(BigInteger x, BigInteger y)
        {
            CameraX = x;
            CameraY = y;
            return this;
        }

        public AsciiMap2D GetMap() => Map;

        public void Reset() => Buffer.Clear();

        public void Render(int sw, int sh)
        {
            int diff = sw % (SquaredMode ? 4 : 2);

            if (diff != 0)
                sw -= diff;
            if (sh % 2 != 0)
                sh--;

            if (SquaredMode)
                sw /= 2;
            Buffer.SetSize((uint)sw, (uint)sh);

            ConsoleColor fColor = Console.ForegroundColor;
            ConsoleColor bColor = Console.BackgroundColor;

            for (uint j = 0; j < sh; j++)
            {
                for (uint i = 0; i < sw; i++)
                {
                    var x = CameraX + i;
                    var y = CameraY + j;

                    AsciiTile2D tile;

                    if (x < 0 || y < 0)
                        tile = new AsciiTile2D();
                    else
                    {
                        var coords = AsciiMap2D.FromGlobalCoords(x, y, Map.ChunkWidth, Map.ChunkHeight);
                        tile = Map.GetTile(coords);
                    }

                    if (!Buffer.EqualToPixel(i, j, tile))
                    {
                        bool proceed = true;
                        try { Console.SetCursorPosition((int)(SquaredMode ? i * 2 : i), (int)j); } catch { proceed = false; }
                        if (proceed)
                        {
                            Console.ForegroundColor = tile.GetCCForeground();
                            Console.BackgroundColor = tile.GetCCBackground();
                            Console.Write(tile.GetChar());
                            if (SquaredMode)
                                Console.Write(tile.GetSChar());
                        }
                    }
                    Buffer.SetPixel(i, j, tile.Clone());
                }
            }

            Console.ForegroundColor = fColor;
            Console.BackgroundColor = bColor;
        }
    }
}
