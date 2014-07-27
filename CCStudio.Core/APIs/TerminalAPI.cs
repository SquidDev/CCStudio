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
            "isColour", "isColor"
        };

        protected ITerminal Terminal;

        #region Public API methods
        public void write(object Text)
        {
            if(Text != null) Terminal.Write(Text.ToString());
        }
        public void scroll(int Number)
        {
            Terminal.Scroll(Number);
        }
        public void clear()
        {
            Terminal.Clear();
        }
        public void clearLine()
        {
            Terminal.ClearLine();
        }
        #region Cursor
        public void setCursorPos(int X, int Y)
        {
            Terminal.SetCursorPos(X, Y);
        }
        public int[] getCursorPos()
        {
            return Terminal.GetCursorPos();
        }
        public void setCursorBlink(bool Blink)
        {
            Terminal.SetCursorBlink(Blink);
        }
        #endregion
        #region Colours
        public bool isColour()
        {
            return Owner.Config.IsColour;
        }
        public void setTextColour(int Colour)
        {
            Terminal.SetForegroundColour(ParseColour(Colour));
        }
        public void setBackgroundColour(int Colour)
        {
            Terminal.SetBackgroundColour(ParseColour(Colour));
        }
        
        #endregion
        #region Colors
        public bool isColor() { return isColour(); }
        public void setTextColor(int Color) { setTextColour(Color); }
        public void setBackgroundColor(int Color) { setBackgroundColour(Color); }
        #endregion

        public int[] getSize()
        {
            return Terminal.GetSize();
        }
        #endregion
        #region Colour Utils
        protected byte ParseColour(int CurrentColour)
        {
            if (CurrentColour <= 0) throw new ArgumentOutOfRangeException("Colour out of range", "colour");

            CurrentColour = GetHighestBit(CurrentColour);
            if (CurrentColour < 0 || CurrentColour > 15) throw new ArgumentOutOfRangeException("Colour out of range", "colour");

            //If not black or white, throw error;
            if (!Owner.Config.IsColour && CurrentColour!=0 && CurrentColour!=15) throw new ArgumentException("Colour not supported", "colour");

            return (byte)CurrentColour;
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
            Terminal = OwnerComputer.Terminal;
        }
        #endregion    
    }
}
