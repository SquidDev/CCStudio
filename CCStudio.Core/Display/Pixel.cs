using System;

namespace CCStudio.Core.Display
{
    /// <summary>
    /// Used to represent one pixel
    /// </summary>
    public class Pixel
    {
        public int Background;
        public int Foreground;

        public char Character;

        public readonly int X;
        public readonly int Y;

        public Pixel(int X, int Y, int Back = 15, int Fore = 1, char Text = ' ')
        {
            this.X = X;
            this.Y = Y;

            Background = Back;
            Foreground = Fore;

            Character = Text;
        }
    }
}
