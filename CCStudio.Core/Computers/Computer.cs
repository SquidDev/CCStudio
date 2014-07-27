using CCStudio.Core.APIs;
using CCStudio.Core.Display;
using LuaInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CCStudio.Core.Computers
{
    /// <summary>
    /// State computers are in
    /// </summary>
    public enum ComputerState
    {
        Stopped = 0,
        Running = 1,
        Starting = 2
    }

    /// <summary>
    /// Core computer - manages event loop, time, terminal.
    /// </summary>
    public class Computer : IDisposable
    {
        #region Properties
        /// <summary>
        /// The core event queue
        /// </summary>
        protected Queue<object[]> EventQueue = new Queue<object[]>();

        /// <summary>
        /// Set so the computer will block when waiting for an event
        /// </summary>
        protected ManualResetEvent EventWait = new ManualResetEvent(false);

        public EventManager Events { get; protected set; }

        public ITerminal Terminal;

        public ComputerConfig Config { get; protected set; }
        public string DisplayLabel
        {
            get
            {
                return String.IsNullOrEmpty(Config.Label) ? "[Computer " + Config.Id.ToString() + "]" : Config.Label;
            }
        }

        protected Thread VMThread;
        #endregion

        #region Creation
        public Computer(Session Parent)
        {
            //Initialise config
            Config = ComputerConfig.Create(Parent.NextComputer, Parent.ComputerDirectory);

            Parent.NextComputer++;
            Parent.Computers.Add(Config);

            Initialise();
        }

        public Computer(ComputerConfig Config)
        {
            this.Config = Config;

            Initialise();
        }

        protected void Initialise()
        {
            Events = new EventManager(this);
        }
        #endregion

        #region Power
        public void TurnOn()
        {
            if (Config.State != ComputerState.Stopped) return;

            Config.State = ComputerState.Starting;

            LuaStart();
        }

        public void Shutdown()
        {
            if (Config.State == ComputerState.Stopped) return;

            LuaStop();
        }

        public void Restart()
        {
            Shutdown();
            TurnOn();
        }
        #endregion
        #region LuaManagement
        protected void LuaStart()
        {
            VMThread = new Thread(new ThreadStart(_LuaStart));
            VMThread.SetApartmentState(ApartmentState.STA);
            VMThread.IsBackground = true;   //Set if background to force it to close on exit
            VMThread.Name = String.Format("VM-{0}-Thread", Config.Id);
            VMThread.Start();
        }
        protected void LuaStop()
        {
            if (VMThread != null)
            {
                VMThread.Abort();
            }
        }
        protected void _LuaStart()
        {
            try
            {
                Config.State = ComputerState.Starting;

                using (LuaVM VM = new LuaVM(this))
                {
                    VM.AddAPI(new OsAPI());
                    VM.AddAPI(new TerminalAPI());
                    VM.AddAPI(new HttpAPI());
                    VM.AddAPI(new RedstoneAPI());
                    VM.AddAPI(new PeripheralAPI());
                    VM.AddAPI(new FileSystemAPI());

                    LuaThread VMThread = VM.LuaThreadInstance;

                    string Bios;
                    using(StreamReader BootFile = new StreamReader(Config.BootFile))
                    {
                         Bios = BootFile.ReadToEnd();
                    }

                    LuaThreadResume Resume = VMThread.DoThreadedString(Bios, "bios.lua");

                    if (Resume.Status != LuaThreadStatus.LUA_YIELD)
                    {
                        Config.State = ComputerState.Stopped;

                        throw new Exception("Initialisation failed");
                    }

                    Config.Uptime.Start();
                    Config.State = ComputerState.Running;

                    string Filter = null;
                    while (Config.State != ComputerState.Stopped)
                    {
                        try
                        {
                            //Wait for event
                            EventWait.WaitOne();

                            //Get event
                            object[] Args = EventQueue.Dequeue();

                            //Reset event if has none
                            if (EventQueue.Count < 1) EventWait.Reset();

                            //If there is no filter or filter matches then continue
                            if (Filter == null || ((string)Args[0]) == Filter)
                            {
                                //Resume with args
                                LuaThreadResume Resumer = VMThread.ResumeWithArgs(Args);
                                if (Resumer.Status == LuaThreadStatus.LUA_YIELD)
                                {
                                    //If we have values then filter, otherwise reset filter
                                    if (Resumer.Values != null && Resumer.Values.Length > 0)
                                    {
                                        Filter = (string)Resumer.Values[0];
                                    }
                                    else
                                    {
                                        Filter = null;
                                    }
                                }
                                else if (Resumer.Status == LuaThreadStatus.LUA_DEAD)
                                {
                                    Debug.WriteLine("Died");
                                    Config.State = ComputerState.Stopped;
                                }
                                else
                                {
                                    //Should handle errors
                                    Debug.WriteLine("Errored");
                                    Config.State = ComputerState.Stopped;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Config.State = ComputerState.Stopped;
                            Debug.WriteLine("Stopped: {0}\n{1}", e.Message, e.StackTrace);
                        }
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                Debug.WriteLine("Thread aborted");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error {0}:{1}\n{2}", e.GetType().ToString(), e.Message, e.StackTrace);
            }

            Config.Uptime.Stop();
            Config.Uptime.Reset();
        }
        #endregion
        #region Events
        public void PushEvent(params object[] Args)
        {
            EventQueue.Enqueue(Args);
            EventWait.Set();
        }
        #endregion

        public void Dispose()
        {
            while (VMThread != null && !VMThread.IsAlive)
            {
                VMThread.Abort();
                Thread.Yield();
            }
        }
    }
}
