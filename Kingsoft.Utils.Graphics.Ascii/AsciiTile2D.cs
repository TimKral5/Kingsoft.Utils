using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Kingsoft.Utils.TypeExtensions.ColorExt;

namespace Kingsoft.Utils.Graphics.Ascii
{
    public class AsciiTile2D : IEquatable<AsciiTile2D>
    {
        private Color FColor { get; set; }
        private Color BColor { get; set; }
        private char Character { get; set; }
        private bool HasSecondChar { get; set; }
        private char SecondaryCharacter { get; set; }

        public AsciiTile2D()
        {
            FColor = Color.FromArgb(255, 255, 255);
            BColor = Color.FromArgb(0, 0, 0);
            Character = ' ';
            HasSecondChar = false;
        }

        public AsciiTile2D SetChars(string str)
        {
            Character = str.Length > 0 ? str[0] : ' ';
            SecondaryCharacter = str.Length > 1 ? str[1] : Character;
            HasSecondChar = str.Length > 1;
            return this;
        }

        public AsciiTile2D SetChar(char c) { Character = c; return this; }

        public AsciiTile2D SetSChar(char? c) 
        { 
            HasSecondChar = c != null;
            if (HasSecondChar) SecondaryCharacter = (char)c;
            return this; 
        }

        public AsciiTile2D SetChar(int utf32) { Character = char.ConvertFromUtf32(utf32)[0]; return this; }

        public AsciiTile2D SetForeground(Color color) { FColor = color; return this; }
        public AsciiTile2D SetForeground(byte r, byte g, byte b) { FColor = Color.FromArgb(r, g, b); return this; }
        public AsciiTile2D SetForeground(byte a, byte r, byte g, byte b) { FColor = Color.FromArgb(a, r, g, b); return this; }

        public AsciiTile2D SetBackground(Color color) { BColor = color; return this; }
        public AsciiTile2D SetBackground(byte r, byte g, byte b) { BColor = Color.FromArgb(r, g, b); return this; }
        public AsciiTile2D SetBackground(byte a, byte r, byte g, byte b) { BColor = Color.FromArgb(a, r, g, b); return this; }

        public Color GetForeground() => FColor;
        public Color GetBackground() => BColor;
        public ConsoleColor GetCCForeground() => FColor.GetClosestCColor();
        public ConsoleColor GetCCBackground() => BColor.GetClosestCColor();
        public char GetChar() => Character;
        public char GetSChar() => HasSecondChar ? SecondaryCharacter : Character;

        public AsciiTile2D Clone() => new AsciiTile2D() { Character = Character, SecondaryCharacter = SecondaryCharacter, FColor = FColor, BColor = BColor };
        public bool Equals(AsciiTile2D tile) => (
            GetCCBackground() == tile.GetCCBackground() &&
            GetCCForeground() == tile.GetCCForeground() &&
            GetChar() == tile.GetChar() &&
            GetSChar() == tile.GetSChar()
        );

        public static implicit operator char(AsciiTile2D tile) => tile.GetChar();
        public static implicit operator Color[](AsciiTile2D tile) => new Color[] { tile.GetForeground(), tile.GetBackground() };
    }
}
