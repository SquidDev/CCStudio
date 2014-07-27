using System.Timers;

namespace CCStudio.Core.Computers
{
    public class EventManager
    {
        protected Computer Comp;

        protected bool Control = false;

        protected char TriggerChar;
        protected Timer TriggerCharTimer;

        protected double TriggerDuration = 1000;

        public EventManager(Computer Comp)
        {
            this.Comp = Comp;

            TriggerCharTimer = new Timer(TriggerDuration);
            TriggerCharTimer.Elapsed += TriggerCharTimer_Elapsed;
        }

        public void Paste(string Text)
        {
            Comp.PushEvent("paste", Text);
        }

        #region Keys
        public void KeyDown(int Key)
        {
            Comp.PushEvent("key",Key);
        }

        public void ControlDown()
        {
            Control = true;
        }

        public void ControlUp()
        {
            Control = false;
            if (TriggerChar != null)
            {
                TriggerCharTimer.Stop();
                TriggerChar = default(char);
            }
        }

        public void CharacterDown(char Character)
        {
            Comp.PushEvent("char", Character.ToString());

            if (Control && (Character == 't' || Character == 'r' || Character == 's'))
            {
                TriggerChar = Character;
                TriggerCharTimer.Start();    
            }
        }

        protected void TriggerCharTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TriggerCharTimer.Stop();
            switch (TriggerChar)
            {
                case 't':
                    Comp.PushEvent("terminate");
                    break;
                case 'r':
                    Comp.Restart();
                    break;
                case 's':
                    Comp.Shutdown();
                    break;
            }

            TriggerChar = default(char);
        }
        #endregion
    }
}
