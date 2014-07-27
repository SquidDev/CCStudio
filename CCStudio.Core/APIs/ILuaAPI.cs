using CCStudio.Core.Computers;

namespace CCStudio.Core.APIs
{
    public abstract class ILuaAPI : ILuaObject
    {
        public virtual string[] GetNames() { return new string[] { }; }

        protected Computer Owner;

        public virtual void Load(Computer OwnerComputer)
        {
            Owner = OwnerComputer;
        }
    }
}
