using System;

namespace CCStudio.Core.Display
{
    /// <summary>
    /// Used to represent one pixel
    /// </summary>
    public class Pixel
    {
        public byte Background;
        public byte Foreground;

        public char Character;

        public readonly int X;
        public readonly int Y;

        public Pixel(int X, int Y, byte Back = 15, byte Fore = 1, char Text = ' ')
        {
            this.X = X;
            this.Y = Y;

            Background = Back;
            Foreground = Fore;

            Character = Text;
        }
    }
}
