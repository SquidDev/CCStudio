using System.Collections.Generic;

namespace CCStudio.Core.Utils
{
    /// <summary>
    /// Invert the comparer (for sorting in reverse and what not
    /// </summary>
    public class InverseComparer<T> : IComparer<T>
    {
        public IComparer<T> Comparer { get; protected set; }
        public InverseComparer(IComparer<T> Comparer)
        {
            this.Comparer = Comparer;
        }

        public int Compare(T x, T y)
        {
            return Comparer.Compare(y, x);
        }
    }
}
