using CCStudio.Core.Peripheral;
using LuaInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCStudio.Core.APIs
{
    public class PeripheralAPI : ILuaAPI
    {
        public static readonly String[] _Names = new String[] { "peripheral" };
        public static readonly String[] _MethodNames = new String[] { 
            "isPresent", "getType", "getMethods", "call"
        };

        public bool isPresent(string Side)
        {
            return Owner.Config.Peripherals.ContainsKey(Side);
        }

        public string getType(string Side)
        {
            try
            {
                return Owner.Config.Peripherals[Side].getType();
            }
            catch (Exception) { }

            return null;
        }

        public LuaTable getMethods(string Side)
        {
            try
            {
                return new LuaTable(Owner.Config.Peripherals[Side].GetMethodNames());
            }
            catch (Exception) { }

            return null;
        }

        public object call(string Side, string MethodName, params object[] Args)
        {
            try
            {
                return Owner.Config.Peripherals[Side].CallMethod(MethodName, Args);
            }
            catch (Exception) { }

            return null;
        }

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
