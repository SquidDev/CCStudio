using System;
using System.Diagnostics;

namespace CCStudio.MonoGame
{
    public class DebugWriter
    {
        public static void WriteLine(object Message)
        {
            if (Message == null)
            {
                Write("null");
            }
            else
            {
                Write(Message.ToString());
            }
        }

        public static void WriteLine(object Message, params object[] Args)
        {
            if (Message == null)
            {
                Write("null");
            }
            else
            {
                Write(String.Format(Message.ToString(), Args));
            }
        }

        protected static void Write(string Message)
        {
            Debug.WriteLine("[{0}] {1}", DateTime.Now.ToString(), Message);
        }
    }
}
