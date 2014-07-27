using System;
using System.Timers;

namespace CCStudio.Core.Computers.Time
{
    /// <summary>
    /// The time of the computer
    /// </summary>
    [Serializable()]
    public class ComputerTime : IComparable<ComputerTime>, IComparable<double>
    {
        #region Properties
        public event ComputerTimeChanged TimeChanged;

        public int Day { get; set; }
        public double Time { get; set; }

        public double Tick
        {
            get { return Day * 24 + Time; }
        }
        #endregion
        public ComputerTime(int Day = 1, double Time = 0)
        {
            this.Day = Day;
            this.Time = Time;
        }

        public void SetTime(int NewDay, double NewTime)
        {
            double Old = Tick;

            Day = NewDay;
            Time = NewTime;

            if (TimeChanged != null)
            {
                TimeChanged(this, new ComputerTimeChangedEventArgs(Old, Tick));
            }
        }

        #region Compare methods
        public int CompareTo(ComputerTime Other)
        {
            return Tick.CompareTo(Other.Tick);
        }
        public int CompareTo(double Other)
        {
            return Tick.CompareTo(Other);
        }
        #endregion
    }

    [Serializable()]
    public class DynamicComputerTime : ComputerTime
    {
        [NonSerialized()]
        protected Timer ClockUpdater;
        public bool Running { get; set; }

        public DynamicComputerTime() { }
        public DynamicComputerTime(int Day = 1, double Time = 0) : base(Day, Time)
        {
            Running = true;
        }

        public void StartTime()
        {
            if (Running) return;

            ClockUpdater = new Timer(2);
            ClockUpdater.Elapsed += ClockUpdater_Elapsed;

            Running = true;
        }

        public void StopTimer()
        {
            if (!Running) return;

            ClockUpdater.Stop();
            ClockUpdater.Dispose();

            Running = false;
        }

        protected void ClockUpdater_Elapsed(object sender, ElapsedEventArgs e)
        {
            int NewDay = Day;
            double NewTime = Time + 0.002;

            if (NewTime >= 24)
            {
                NewTime -= 24;
                NewDay++;
            }

            SetTime(NewDay, NewTime);
        }
    }
    #region Events
    public delegate void ComputerTimeChanged(object sender, ComputerTimeChangedEventArgs e);
    public class ComputerTimeChangedEventArgs : EventArgs
    {
        public readonly double OriginalTime;
        public readonly double NewTime;

        public ComputerTimeChangedEventArgs(double Old, double New)
        {
            OriginalTime = Old;
            NewTime = New;
        }
    }
    #endregion
}
