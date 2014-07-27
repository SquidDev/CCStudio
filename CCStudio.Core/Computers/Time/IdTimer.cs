using System.Timers;

namespace CCStudio.Core.Computers.Time
{
    /// <summary>
    /// A timer with an associated Id
    /// </summary>
    public class IdTimer : Timer
    {
        public readonly int Id;
        public IdTimer(int Id) : base()
        {
            this.Id = Id;
        }

        public IdTimer(int Id, double Interval) : base(Interval)
        {
            this.Id = Id;
        }
    }
}
