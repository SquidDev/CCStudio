using System;
using System.Collections.Generic;

namespace CCStudio.Core.Display
{
    /// <summary>
    /// Manages terminal basics
    /// </summary>
    public class BasicTerminal : ITerminal
    {
        #region Properties
        public PixelGrid Pixels;
        public int Width = 51;
        public int Height = 19;

        //Cursor
        public int CursorX = 1;
        public int CursorY = 1;
        public bool CursorBlink = false;

        //Colour
        public byte BackgroundColour = 15;
        public byte ForegroundColour = 0;
        public bool IsColour = true;
        #endregion

        public BasicTerminal()
        {
            Pixels = new PixelGrid(Width, Height);
        }

        public void Write(string Text)
        {
            if(Text.Length < 1) return;

            foreach(char Character in Text)
            {
                if(CursorX > Width) break;

                Pixel P = Pixels.GetPixel(CursorX - 1, CursorY - 1);
                P.Character = Character;
                P.Background = BackgroundColour;
                P.Foreground = ForegroundColour;

                CursorX++;
            }
        }
        public void Clear()
        {
            Pixels.Clear(BackgroundColour);
        }
        public void ClearLine()
        {
            int Start = CursorY*Height;
            int End = Start + Width;
            for (int X = Start; X < End; X++)
            {
                Pixel P = Pixels.Pixels[X];
                P.Character = ' ';
                P.Background = BackgroundColour;
            }
        }
        public void Scroll(int Y)
        {
            Pixels.Scroll(Y, BackgroundColour);
        }

        public void SetCursorPos(int X, int Y)
        {
            CursorX = X;
            CursorY = Y;
        }

        public int[] GetCursorPos()
        {
            return new int[] { CursorX, CursorY };
        }

        public void SetCursorBlink(bool Blink)
        {
            CursorBlink = Blink;
        }

        public void SetBackgroundColour(byte Colour)
        {
            BackgroundColour = Colour;
        }

        public void SetForegroundColour(byte Colour)
        {
            ForegroundColour = Colour;
        }

        public int[] GetSize()
        {
            return new int[] { Width, Height };
        }
    }
}
