using CCStudio.Core.Computers;
using CCStudio.Core.Computers.Time;
using System;
using System.Collections.Generic;

namespace CCStudio.Core.APIs
{
    public class OsAPI : ILuaAPI
    {
        public Dictionary<int, IdTimer> Timers = new Dictionary<int, IdTimer>();
        public int NextTimer = 0;

        public Dictionary<int, Alarm> Alarms = new Dictionary<int, Alarm>();
        public int NextAlarm = 0;

        public static readonly String[] _Names = new String[] { "os" };
        public static readonly String[] _MethodNames = new String[] { 
            "queueEvent", "shutdown", "reboot",
            "computerID", "getComputerID", "setComputerLabel", "computerLabel", "getComputerLabel",
            "startTimer", "setAlarm", "cancelTimer", "cancelAlarm",
            "clock", "time", "day"
        };

        #region API Methods
        public void queueEvent(params object[] args)
        {
            Owner.PushEvent(args);
        }
        public void shutdown()
        {
            Owner.Shutdown();
        }
        public void reboot()
        {
            Owner.Shutdown();
        }
        #region Label functions
        public int getComputerID()
        {
            return Owner.Config.Id;
        }
        public int computerID()
        {
            return getComputerID();
        }
        public void setComputerLabel(string label)
        {
            Owner.Config.Label = label;
        }
        public string getComputerLabel()
        {
            return Owner.Config.Label;
        }
        public string computerLabel()
        {
            return getComputerLabel();
        }
        #endregion
        #region Timers
        public int startTimer(double Delay)
        {
            if (Delay < 0) throw new ArgumentOutOfRangeException("Delay out of range");

            int Id = NextTimer;
            IdTimer NewTimer = new IdTimer(Id, Delay * 1000);
            NewTimer.Elapsed += TimerElapsed;
            NewTimer.Start();
            
            Timers.Add(Id, NewTimer);

            NextTimer++;
            return Id;
        }
        public int setAlarm(double Time)
        {
            if (Time < 0 || Time > 24) throw new ArgumentOutOfRangeException("Time out of range");

            int Id = NextAlarm;
            int Day = Owner.Config.Time.Day;

            if (Time > Owner.Config.Time.Tick) Day++;
            Alarm NewAlarm = new Alarm(Id, Day, Time);
            
            Alarms.Add(Id, NewAlarm);

            NextAlarm++;
            return Id;
        }

        public void cancelTimer(int Id)
        {
            IdTimer ThisTimer;
            if (Timers.TryGetValue(Id, out ThisTimer))
            {
                ThisTimer.Stop();
                Timers.Remove(Id);
                ThisTimer.Dispose();
            }
        }

        public void cancelAlarm(int Id)
        {
            Alarm ThisAlarm;
            if (Alarms.TryGetValue(Id, out ThisAlarm))
            {
                Alarms.Remove(Id);
            }
        }

        #endregion
        #region Clock
        public double clock()
        {
            return Owner.Config.Uptime.ElapsedMilliseconds;
        }
        public double time()
        {
            return Owner.Config.Time.Time;
        }
        public int day()
        {
            return Owner.Config.Time.Day;
        }
        #endregion
        #endregion
        #region Time Utils
        void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (sender is IdTimer)
            {
                IdTimer ThisTimer = (IdTimer)sender;
                queueEvent("timer",ThisTimer.Id);

                ThisTimer.Stop();
                Timers.Remove(ThisTimer.Id);
                ThisTimer.Dispose();
            }
        }
        void ComputerTimeChanged(object sender, ComputerTimeChangedEventArgs e)
        {
            if (e.NewTime <= e.OriginalTime) return;

            List<Alarm> Remove = new List<Alarm>();
            double Tick = e.NewTime;
            foreach (KeyValuePair<int, Alarm> Alrm in Alarms)
            {
                if (Alrm.Key.CompareTo(Tick) >= 0)
                {
                    queueEvent("alarm", Alrm.Value);
                    Alarms.Remove(Alrm.Key);
                }
            }
        }
        #endregion

        #region ILuaAPI Methods
        public override string[] GetNames()
        {
            return _Names;
        }

        public override string[] GetMethodNames()
        {
            return _MethodNames;
        }

        public override void Load(Computer OwnerComputer)
        {
            base.Load(OwnerComputer);
            OwnerComputer.Config.Time.TimeChanged += ComputerTimeChanged;
        }

        #endregion  

    }
}
