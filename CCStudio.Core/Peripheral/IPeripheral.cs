using CCStudio.Core.APIs;
using CCStudio.Core.Computers;

namespace CCStudio.Core.Peripheral
{
    public abstract class IPeripheral : ILuaObject
    {
        protected Computer Owner;

        public abstract string getType();

        public object CallMethod(string Method, object[] Params)
        {
            return GetType().GetMethod(Method).Invoke(this, Params);
        }
    }
}
