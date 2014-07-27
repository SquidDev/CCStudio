using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCStudio.Core.Display
{
    public interface ITerminal
    {
        void SetCursorPos(int X, int Y);
        int[] GetCursorPos();
        void SetCursorBlink(bool Blink);

        void Clear();
        void ClearLine();

        void SetBackgroundColour(byte Colour);
        void SetForegroundColour(byte Colour);

        int[] GetSize();

        void Write(string Text);

        void Scroll(int Ammount);
    }
}
