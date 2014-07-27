using System;

namespace CCStudio.Core.Computers.Time
{
    /// <summary>
    /// Used for setting alarms for computers.
    /// </summary>
    public class Alarm : IComparable<Alarm>, IComparable<double>
    {
        public readonly ComputerTime Time;
        public readonly int Id;

        public Alarm(int Id, int Day, double Time) : this(Id, new ComputerTime(Day, Time)) { }

        /// <summary>
        /// Create an alarm
        /// </summary>
        /// <param name="Id">The Id of the Alarm</param>
        /// <param name="Time">The time the alarm should go off at</param>
        public Alarm(int Id, ComputerTime Time)
        {
            this.Id = Id;
            this.Time = Time;
        }

        #region Compare
        public int CompareTo(Alarm Other)
        {
            return Time.CompareTo(Other.Time);
        }
        public int CompareTo(double Other)
        {
            return Time.CompareTo(Other);
        }
        #endregion
    }

    

    
}
