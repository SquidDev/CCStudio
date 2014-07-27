using System;
using System.Collections.Generic;

namespace CCStudio.Core.Display
{
    /// <summary>
    /// Manages terminal basics
    /// </summary>
    public class Terminal
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
        public int BackgroundColour = 15;
        public int ForegroundColour = 0;
        public bool IsColour = true;
        #endregion
        #region Events
        //public event PixelChangedEventHandler PixelChanged;
        //public event TerminalScrollEventHandler TerminalScroll;

        //public event TerminalClearEventHandler TerminalClear;
        //public event TerminalClearLineEventHandler TerminalClearLine;
        public event TerminalChangedEventHandler TerminalChanged;
        #endregion

        public Terminal()
        {
            Pixels = new PixelGrid(Width, Height);
        }

        public void Write(string Text)
        {
            if(Text.Length < 1) return;

            //List<Pixel> Changed = new List<Pixel>();

            foreach(char Character in Text)
            {
                if(CursorX > Width) break;

                Pixel P = Pixels.GetPixel(CursorX - 1, CursorY - 1);
                P.Character = Character;
                P.Background = BackgroundColour;
                P.Foreground = ForegroundColour;

                //Changed.Add(P);
                

                CursorX++;
            }
            if (TerminalChanged != null) TerminalChanged(this); //, Changed);
        }
        public void Clear()
        {
            Pixels.Clear(BackgroundColour);
            //if(TerminalClear != null) TerminalClear(this);
            if (TerminalChanged != null) TerminalChanged(this); //, Pixels.Pixels);
        }
        public void ClearLine()
        {
            //List<Pixel> Changed = new List<Pixel>();
            int Start = CursorY*Height;
            int End = Start + Width;
            for (int X = Start; X < End; X++)
            {
                Pixel P = Pixels.Pixels[X];
                P.Character = ' ';
                P.Background = BackgroundColour;

                //Changed.Add(P);
            }
            //if(TerminalClearLine != null) TerminalClearLine(this);
            if (TerminalChanged != null) TerminalChanged(this); //, Changed);
        }
        public void Scroll(int Y)
        {
            Pixels.Scroll(Y, BackgroundColour);
            //if(TerminalScroll != null) TerminalScroll(this, new TerminalScroll(Y));
            if (TerminalChanged != null) TerminalChanged(this); //, Pixels.Pixels);
        }
    }
    
    /// <summary>
    /// Event for when the terminal scrolls
    /// </summary>
    public class TerminalScroll : EventArgs
    {
        public readonly int ScrollAmmount;

        /// <summary>
        /// Create a new terminal scroll event
        /// </summary>
        /// <param name="Ammount">How much to scroll the terminal by</param>
        public TerminalScroll(int Ammount)
        {
            ScrollAmmount = Ammount;
        }
    }
    public delegate void TerminalChangedEventHandler(Terminal sender); //, IEnumerable<Pixel> Changed);
}
