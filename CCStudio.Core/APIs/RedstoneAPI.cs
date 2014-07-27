using CCStudio.Core.Computers;
using LuaInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCStudio.Core.APIs
{
    public class RedstoneAPI : ILuaAPI
    {
        public static readonly String[] _Names = new String[] { "redstone", "rs" };
        public static readonly String[] _MethodNames = new String[] { 
            "getSides", "setOutput", "getOutput", "getInput",  
            "setAnalogOutput", "getAnalogOutput", "getAnalogInput",  
            "setBundledOutput", "getBundledOutput", "getBundledInput",  
        };

        protected static string[] Sides = new string[] { "top", "bottom", "left", "right", "front", "back" };

        protected Redstone RedstoneSides
        {
            get { return Owner.Config.Redstone; }
        }

        public LuaTable getSides()
        {
            return new LuaTable(Sides);
        }

        #region Normal
        public void setOutput(string Side, bool Value)
        {
            SetSide<byte>(Side, (byte)(Value ? 15 : 0), RedstoneSides.RedstoneOutput);
        }
        public bool getOutput(string Side)
        {
            return GetSide<byte>(Side, RedstoneSides.RedstoneOutput) > 0;
        }
        public bool getInput(string Side)
        {
            return GetSide<byte>(Side, RedstoneSides.RedstoneInput) > 0;
        }
        #endregion
        #region Analog
        public void setAnalogOutput(string Side, byte Value)
        {
            if (Value < 0 || Value > 15) throw new Exception("Expected number in range 0-15");

            SetSide<byte>(Side, Value, RedstoneSides.RedstoneOutput);
        }
        public byte getAnalogOutput(string Side)
        {
            return GetSide<byte>(Side, RedstoneSides.RedstoneOutput);
        }
        public byte getAnalogInput(string Side)
        {
            return GetSide<byte>(Side, RedstoneSides.RedstoneInput);
        }
        #endregion
        #region Bundled
        public void setBundledOutput(string Side, int Value)
        {
            SetSide<int>(Side, Value, RedstoneSides.BundledOutput);
        }
        public int getBundledOutput(string Side)
        {
            return GetSide<int>(Side, RedstoneSides.BundledOutput);
        }
        public int getBundledInput(string Side)
        {
            return GetSide<int>(Side, RedstoneSides.BundledInput);
        }
        #endregion

        #region Sides
        public void SetSide<T>(string Side, T Value, SideStatus<T> States)
        {
            switch (Side)
            {
                case "top":
                    States.Top = Value;
                    return;
                case "bottom":
                    States.Bottom = Value;
                    return;
                case "left":
                    States.Left = Value;
                    return;
                case "right":
                    States.Right = Value;
                    return;
                case "front":
                    States.Front = Value;
                    return;
                case "back":
                    States.Back = Value;
                    return;
                default:
                    throw new Exception("Invalid side");
            }
        }
        public T GetSide<T>(string Side, SideStatus<T> States)
        {
            switch (Side)
            {
                case "top":
                    return States.Top;
                case "bottom":
                    return States.Bottom;
                case "left":
                    return States.Left;
                case "right":
                    return States.Right;
                case "front":
                    return States.Front;
                case "back":
                    return States.Back;
                default:
                    throw new Exception("Invalid side");
            }
        }
        #endregion

        #region ILuaAPI Members
        public override string[] GetNames()
        {
            return _Names;
        }

        public override string[] GetMethodNames()
        {
            return _MethodNames;
        }
        #endregion
    }
}
