namespace CCStudio.Core.Computers
{
    public class Redstone
    {
        public SideStatus<byte> RedstoneInput { get; set; }
        public SideStatus<byte> RedstoneOutput { get; set; }

        public SideStatus<int> BundledInput { get; set; }
        public SideStatus<int> BundledOutput { get; set; }

        public Redstone() { }

        /// <summary>
        /// Create a redstone state with some default values;
        /// </summary>
        public static Redstone Create()
        {
            Redstone State = new Redstone();
            State.RedstoneInput = new SideStatus<byte>();
            State.RedstoneOutput = new SideStatus<byte>();

            State.BundledInput = new SideStatus<int>();
            State.BundledOutput = new SideStatus<int>();

            return State;
        }
    }

    public class SideStatus<T>
    {
        public T Top { get; set; }
        public T Bottom { get; set; }
        public T Left { get; set; }
        public T Right { get; set; }
        public T Back { get; set; }
        public T Front { get; set; }
    }
}
