using LuaInterface;
using System.Reflection;

namespace CCStudio.Core.APIs
{
    public abstract class ILuaObject : ILuaPushable
    {
        public virtual string[] GetMethodNames() { return new string[] { }; }

        public LuaTable ToTable(Lua LuaInstance)
        {
            LuaTable Table = new LuaTable(LuaInstance);
            foreach (string Method in GetMethodNames())
            {
                MethodBase Function = GetType().GetMethod(Method);
                LuaMethodWrapper Wrapper = new LuaMethodWrapper(LuaInstance.Translator, this, Function.DeclaringType, Function);
                Table[Method] = new KopiLua.LuaNativeFunction(Wrapper.Call);
            }
            return Table;
        }

        #region ILuaPusable
        public void Push(KopiLua.LuaState State, ObjectTranslator Translator)
        {
            Push(new LuaVirtualInstance(State, Translator));
        }

        public void Push(Lua LuaInstance)
        {
            ToTable(LuaInstance).Push(LuaInstance);
        }
        #endregion
    }
}
