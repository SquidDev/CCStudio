using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCStudio.MonoGame.Computers
{
    public class KeyLookup
    {
        /*
        public static Dictionary<Keys, char> CharWithShift = new Dictionary<Keys, char>()
        {
            {Keys.A, 'A'},
            {Keys.B, 'B'},
            {Keys.C, 'C'},
            {Keys.D, 'D'},
            {Keys.E, 'E'},
            {Keys.F, 'F'},
            {Keys.G, 'G'},
            {Keys.H, 'H'},
            {Keys.I, 'I'},
            {Keys.J, 'J'},
            {Keys.K, 'K'},
            {Keys.L, 'L'},
            {Keys.M, 'M'},
            {Keys.N, 'N'},
            {Keys.O, 'O'},
            {Keys.P, 'P'},
            {Keys.Q, 'Q'},
            {Keys.R, 'R'},
            {Keys.S, 'S'},
            {Keys.T, 'T'},
            {Keys.U, 'U'},
            {Keys.V, 'V'},
            {Keys.W, 'W'},
            {Keys.X, 'X'},
            {Keys.Y, 'Y'},
            {Keys.Z, 'Z'},

            {Keys.Space, ' '},
            {Keys.D0, ')'},
            {Keys.D1, '!'},
#if MONOMAC
            {Keys.D2, '@'},
            {Keys.D3, '#'},
#else
            {Keys.D2, '"'},
            {Keys.D3, '£'},
#endif
            {Keys.D4, '$'},
            {Keys.D5, '%'},
            {Keys.D6, '^'},
            {Keys.D7, '&'},
            {Keys.D8, '*'},
            {Keys.D9, '('},

            {Keys.OemSemicolon, ':'},
            {Keys.OemPlus, '+'},
            {Keys.Add, '+'},
            {Keys.OemPeriod, '>'},
            {Keys.OemMinus, '_'},
            {Keys.OemComma, '<'},
            {Keys.OemQuestion, '?'},
            {Keys.OemTilde, '~'}
            /*
            "190"		">"
            "191"		"?"
            "192"		"~"
            "219"		"{"
            "220"		"|"
            "221"		"}"
            "222"		"\""
            "96"		"0"
            "97"		"1"
            "98"		"2"
            "99"		"3"
            "100"		"4"
            "101"		"5"
            "102"		"6"
            "103"		"7"
            "104"		"8"
            "105"		"9"
            "111"		"/"
            "106"		"*"
            "109"		"-"
            "107"		"+"
            "110"		"."
             *//*
        };
        */
        public static Dictionary<Keys, int> KeyToInt = new Dictionary<Keys, int>()
        {
            {Keys.A, 30},
            {Keys.Add, 78}, //Numpad - : 74 (Is just OemMinus for MonoGame)
            {Keys.B, 48},
            {Keys.Back, 14},
            {Keys.C, 46},
            {Keys.CapsLock, 58},
            {Keys.D, 32},
            {Keys.D0, 11},
            {Keys.D1, 2},
            {Keys.D2, 3},
            {Keys.D3, 4},
            {Keys.D4, 5},
            {Keys.D5, 6},
            {Keys.D6, 7},
            {Keys.D7, 8},
            {Keys.D8, 9},
            {Keys.D9, 10},
            {Keys.Decimal, 83},
            {Keys.Delete, 211},
            {Keys.Divide, 181},
            {Keys.Down, 208},
            {Keys.E, 18},
            {Keys.End, 207},
            {Keys.Enter, 28},
            {Keys.Escape, 1},
            {Keys.F, 33},
            {Keys.F1, 59},
            {Keys.F10, 68},
            {Keys.F11, 87},
            {Keys.F12, 88},
            {Keys.F2, 60},
            {Keys.F3, 61},
            {Keys.F4, 62},
            {Keys.F5, 63},
            {Keys.F6, 64},
            {Keys.F7, 65},
            {Keys.F8, 66},
            {Keys.F9, 67},
            {Keys.G, 34},
            {Keys.H, 35},
            {Keys.Home, 199},
            {Keys.I, 23},
            {Keys.Insert, 210},
            {Keys.J, 36},
            {Keys.K, 37},
            {Keys.L, 38},
            {Keys.Left, 203},
            {Keys.LeftAlt, 56},
            {Keys.LeftControl, 29}, //Right control is 157??
            {Keys.LeftShift, 42},
            {Keys.LeftWindows, 219},
            {Keys.M, 50},
            {Keys.Multiply, 55},
            {Keys.N, 39},
            {Keys.NumLock, 69},
            {Keys.NumPad0, 82},
            {Keys.NumPad1, 79},
            {Keys.NumPad2, 80},
            {Keys.NumPad3, 81},
            {Keys.NumPad4, 75},
            {Keys.NumPad5, 76},
            {Keys.NumPad6, 77},
            {Keys.NumPad7, 71},
            {Keys.NumPad8, 72},
            {Keys.NumPad9, 73},
            {Keys.O, 24},
            {Keys.OemCloseBrackets, 27},    // ]    // `: 41
            {Keys.OemComma, 51},
            {Keys.OemMinus, 12},
            {Keys.OemOpenBrackets, 26},     // [
            {Keys.OemPeriod, 52},
            {Keys.OemPipe, 53},             // \
            {Keys.OemPlus, 13},             // =
            {Keys.OemQuestion, 53},         // /
            {Keys.OemSemicolon, 39},        //;
            {Keys.OemTilde, 41},            // ' (Mimic says it is 40. CC says 41)
            {Keys.P, 25},
            {Keys.PageDown, 209},
            {Keys.PageUp, 201},
            {Keys.Q, 16},
            {Keys.R, 19},
            {Keys.Right, 205},
            {Keys.RightControl, 157},
            {Keys.RightShift, 54},
            {Keys.RightWindows, 220},
            {Keys.S, 31},
            {Keys.T, 20},
            {Keys.Tab, 15},
            {Keys.U, 22},
            {Keys.Up, 200},
            {Keys.V, 47},
            {Keys.W, 17},
            {Keys.X, 45},
            {Keys.Y, 21},
            {Keys.Z, 44}
        };
    }
}
