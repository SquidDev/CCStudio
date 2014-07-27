using CCStudio.Core.Computers;
using CCStudio.Core.Display;
using System;

namespace CCStudio.Core.APIs
{
    public class TerminalAPI : ILuaAPI
    {
        public static readonly string[] _Names = new string[] { "term" };
        public static readonly string[] _MethodNames = new string[] { 
            "write", "scroll", "clear", "clearLine",
            "setCursorPos", "setCursorBlink", "getCursorPos", "getSize",  
            "setTextColour", "setTextColor", 
            "setBackgroundColour", "setBackgroundColor",
            "isColour", "isColor", "Debug"
        };

        protected Terminal Term;

        #region Public API methods
        public void write(object Text)
        {
            if(Text != null) Term.Write(Text.ToString());
        }
        public void scroll(int Number)
        {
            Term.Scroll(Number);
        }
        public void clear()
        {
            Term.Clear();
        }
        public void clearLine()
        {
            Term.ClearLine();
        }
        #region Cursor
        public void setCursorPos(int X, int Y)
        {
            Term.CursorX = Math.Max(1,X);
            Term.CursorY = Math.Min(Term.Height, Y);
        }
        public int[] getCursorPos()
        {
            return new int[] { Term.CursorX, Term.CursorY };
        }
        public void setCursorBlink(bool Blink)
        {
            Term.CursorBlink = Blink;
        }
        #endregion
        #region Colours
        public bool isColour()
        {
            return Term.IsColour;
        }
        public void setTextColour(int Colour)
        {
            Term.ForegroundColour = ParseColour(Colour);
        }
        public void setBackgroundColour(int Colour)
        {
            Term.BackgroundColour = ParseColour(Colour);
        }
        
        #endregion
        #region Colors
        public bool isColor() { return isColour(); }
        public void setTextColor(int Color) { setTextColour(Color); }
        public void setBackgroundColor(int Color) { setBackgroundColour(Color); }
        #endregion

        public int[] getSize()
        {
            return new int[] { Term.Width, Term.Height };
        }
        #endregion
        #region Colour Utils
        protected int ParseColour(int CurrentColour)
        {
            if (CurrentColour <= 0) throw new ArgumentOutOfRangeException("Colour out of range", "colour");

            CurrentColour = GetHighestBit(CurrentColour);
            if (CurrentColour < 0 || CurrentColour > 15) throw new ArgumentOutOfRangeException("Colour out of range", "colour");

            //If not black or white, throw error;
            if (!Term.IsColour && CurrentColour!=0 && CurrentColour!=15) throw new ArgumentException("Colour not supported", "colour");

            return CurrentColour;
        }
        protected int GetHighestBit(int Group)
        {
            int Bit = 0;
            while (Group > 0)
            {
                Group >>= 1;
                Bit++;
            }
            return Bit - 1;
        }
        #endregion

        #region ILuaAPI Methods
        public override string[] GetNames()
        {
            return _Names;
        }

        public override string[] GetMethodNames()
        {
            return _MethodNames;
        }

        public override void Load(Computer OwnerComputer)
        {
            base.Load(OwnerComputer);
            Term = OwnerComputer.Term;
        }
        #endregion    
    }
}
