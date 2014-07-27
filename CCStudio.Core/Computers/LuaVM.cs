using CCStudio.Core.APIs;
using LuaInterface;
using System;

namespace CCStudio.Core.Computers
{
    /// <summary>
    /// Manages Lua and APIs for the Computer
    /// </summary>
    public class LuaVM : IDisposable
    {
        /// <summary>
        /// The code Lua Instance
        /// </summary>
        public Lua LuaInstance;
        /// <summary>
        /// The thread on which code runs on
        /// </summary>
        public LuaThread LuaThreadInstance;

        Computer Owner;

        /// <summary>
        /// Create a new LuaVM instance
        /// </summary>
        /// <param name="OwnerComputer">The computer to work with</param>
        public LuaVM(Computer OwnerComputer)
        {      
            LuaInstance = new Lua();
            Owner = OwnerComputer;

            //Clear globals
            foreach (string Global in OwnerComputer.Config.BannedGlobals)
            {
                LuaInstance[Global] = null;
            }

            LuaThreadInstance = LuaInstance.NewThread();
        }

        /// <summary>
        /// Add an API to the environment and pass the computer to the API
        /// </summary>
        /// <param name="API">The API instance to load</param>
        public void AddAPI(ILuaAPI API)
        {
            API.Load(Owner);

            LuaTable Table = API.ToTable(LuaInstance);

            foreach (string Name in API.GetNames())
            {
                LuaInstance[Name] = Table;
            }
        }

        #region IDisposable Methods
        public void Dispose()
        {
            LuaInstance.Dispose();
            LuaThreadInstance.Dispose();
        }
        #endregion
    }
}
