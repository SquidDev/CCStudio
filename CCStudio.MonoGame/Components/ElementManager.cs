using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CCStudio.MonoGame.Components
{
    public class ElementManager : IElement
    {
        protected List<IGameElement> Elements = new List<IGameElement>();

        protected List<IGameElement> Updates = new List<IGameElement>();
        protected List<IGameElement> Draws = new List<IGameElement>();

        protected MouseState PreviousMouseState;
        protected KeyboardState PreviousKeyboardState;

        protected List<char> Press = new List<char>();

        protected OpenTK.GameWindow Window;

        public ElementManager(Game BaseGame)
        {
            Enabled = true;
            Visible = true;

            //Add some hooks to the base window.
            OpenTK.GameWindow OTKWindow = null;
            Type type = typeof(OpenTKGameWindow);

            FieldInfo field = type.GetField("window", BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                OTKWindow = field.GetValue(BaseGame.Window) as OpenTK.GameWindow;
            }

            if (OTKWindow != null)
            {
                OTKWindow.KeyPress += OTKWindow_KeyPress;
                //I could use these and it would probably be better but I then get conversion issues. *Sigh*
                //OTKWindow.Keyboard.KeyUp += OTKWindow_KeyUp;
                //OTKWindow.Keyboard.KeyDown += OTKWindow_KeyDown;
            }
        }
        #region OpenTK Events
        //protected void OTKWindow_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e) { }
        //protected void OTKWindow_KeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e) { }

        /// <summary>
        /// Handles key presses.
        /// </summary>
        protected void OTKWindow_KeyPress(object sender, OpenTK.KeyPressEventArgs e)
        {
            Press.Add(e.KeyChar);
        }
        #endregion
        #region Item Management
        /// <summary>
        /// Add an element to the ElementManager
        /// </summary>
        /// <param name="Element"></param>
        public void Add(IGameElement Element)
        {
            Elements.Add(Element);

            if (Element.Enabled) Updates.Add(Element);
            Element.EnabledChanged += Element_EnabledChanged;

            if (Element.Visible) Draws.Add(Element);
            Element.VisibleChanged += Element_VisibleChanged;
        }

        protected void Element_EnabledChanged(object sender, EventArgs e)
        {
            IGameElement Element = (IGameElement)sender;
            if (Element.Enabled)
            {
                Updates.Add(Element);
            }
            else
            {
                Updates.Remove(Element);
            }
        }

        protected void Element_VisibleChanged(object sender, EventArgs e)
        {
            IGameElement Element = (IGameElement)sender;
            if (Element.Visible)
            {
                Draws.Add(Element);
            }
            else
            {
                Draws.Remove(Element);
            }
        }
        #endregion

        #region IUpdateable Members
        public override void Update(GameTime Time)
        {
            if (Updates.Count == 0)
            {
                PreviousMouseState = Mouse.GetState();
                PreviousKeyboardState = Keyboard.GetState();
                return;
            }


            //Handle mouse events
            MouseState M = Mouse.GetState();

            bool LeftDown = (PreviousMouseState.LeftButton == ButtonState.Released && M.LeftButton == ButtonState.Pressed);
            bool RightDown = (PreviousMouseState.RightButton == ButtonState.Released && M.RightButton == ButtonState.Pressed);
            bool MiddleDown = (PreviousMouseState.MiddleButton == ButtonState.Released && M.MiddleButton == ButtonState.Pressed);

            bool LeftUp = (PreviousMouseState.LeftButton == ButtonState.Pressed && M.LeftButton == ButtonState.Released);
            bool RightUp = (PreviousMouseState.RightButton == ButtonState.Pressed && M.RightButton == ButtonState.Released);
            bool MiddleUp = (PreviousMouseState.MiddleButton == ButtonState.Pressed && M.MiddleButton == ButtonState.Released);

            Point Position = M.Position;
            bool Moved = Position != PreviousMouseState.Position;

            int ScrollAmmount = M.ScrollWheelValue - PreviousMouseState.ScrollWheelValue;

            //Handle keyboard events
            KeyboardState K = Keyboard.GetState();

            List<Keys> Down = new List<Keys>();
            List<Keys> Up = new List<Keys>();

            foreach (Keys Key in Enum.GetValues(typeof(Keys)).Cast<Keys>())
            {
                if (K.IsKeyDown(Key) && PreviousKeyboardState.IsKeyUp(Key)) Down.Add(Key);
                if (K.IsKeyUp(Key) && PreviousKeyboardState.IsKeyDown(Key)) Up.Add(Key);
            }


            //Update all items
            foreach (IGameElement Comp in Updates)
            {
                //Is this enabled?
                if (Comp.Enabled)
                {
                    //Dont do lots of stuff if we don't need to
                    if (Comp.TracksEvents)
                    {
                        if (Comp.Size.Contains(Position))
                        {
                            if (LeftDown) Comp.MouseDown(MouseButtons.LeftButton, Position);
                            if (RightDown) Comp.MouseDown(MouseButtons.RightButton, Position);
                            if (MiddleDown) Comp.MouseDown(MouseButtons.MiddleButton, Position);

                            if (LeftUp) Comp.MouseUp(MouseButtons.LeftButton, Position);
                            if (RightUp) Comp.MouseUp(MouseButtons.RightButton, Position);
                            if (MiddleUp) Comp.MouseUp(MouseButtons.MiddleButton, Position);

                            if (Moved) Comp.MouseMove(Position);

                            if (ScrollAmmount != 0) Comp.MouseScroll(ScrollAmmount);
                        }

                        foreach (Keys Key in Down)
                        {
                            Comp.KeyDown(Key);
                        }

                        foreach (Keys Key in Up)
                        {
                            Comp.KeyUp(Key);
                        }

                        foreach (char Chr in Press)
                        {
                            Comp.KeyPress(Chr);
                        }
                    }

                    //Update it
                    Comp.Update(Time);
                }
            }

            //Set local variables
            PreviousMouseState = M;
            PreviousKeyboardState = K;

            Press.Clear();
        }
        #endregion
        #region IDrawable Members
        public override void Draw(GameTime Time)
        {
            foreach (IGameElement Element in Draws)
            {
                if (Element.Visible)
                {
                    Element.Draw(Time);
                }
            }
        }
        #endregion
        #region IGameComponent Members
        public override void Initialize()
        {
            foreach (IGameElement Element in Elements)
            {
                Element.Initialize();
            }
        }
        #endregion
    }
}
