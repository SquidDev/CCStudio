using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCStudio.Core.APIs
{
    public class BitAPI : ILuaAPI
    {
        public static readonly String[] _Names = new String[] { "bit" };
        public static readonly String[] _MethodNames = new String[] { 
            "bnot", "band", "bor", "bxor", "brshift", "blshift", "blogic_rshift" 
        };

        public int bnot(int A)
        {
            return ~ A;
        }

        public int band(int A, int B)
        {
            return A & B;
        }

        public int bor(int A, int B)
        {
            return A | B;
        }

        public int bxor(int A, int B)
        {
            return A ^ B;
        }
        public int brshift(int A, int B)
        {
            return A >> B;
        }
        public int blsight(int A, int B)
        {
            return B << A;
        }
        public uint blogic_rshift(uint A, int B)
        {
            return A >> B;
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
