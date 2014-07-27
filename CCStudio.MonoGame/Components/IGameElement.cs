using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace CCStudio.MonoGame.Components
{
    public abstract class IGameElement : IElement
    {
        
        public Rectangle Size;

        public bool TracksEvents
        {
            get;
            protected set;
        }

        protected SpriteBatch Batch;
        public IGameElement(SpriteBatch Batch)
        {
            this.Batch = Batch;
        }

        #region Mouse Events
        public virtual void MouseDown(MouseButtons Button, Point Position) { }
        public virtual void MouseUp(MouseButtons Button, Point Position) { }
        public virtual void MouseMove(Point Position) { }
        public virtual void MouseScroll(int Ammount) { }
        #endregion

        #region Key Events
        public virtual void KeyDown(Keys Key) { }
        public virtual void KeyUp(Keys Key) { }
        public virtual void KeyPress(char Character) { }
        #endregion
    }
}
