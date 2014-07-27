using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCStudio.MonoGame.Components
{
    public abstract class IElement : IUpdateable, IDrawable, IGameComponent
    {
        #region IUpdateable Members
        protected bool _Enabled;
        public bool Enabled
        {
            get
            {
                return _Enabled;
            }
            set
            {
                if (_Enabled != value)
                {
                    _Enabled = value;
                    if (EnabledChanged != null) EnabledChanged(this, new EventArgs());
                }
            }
        }

        public event EventHandler<EventArgs> EnabledChanged;

        //Not implimented
        public int UpdateOrder { get; protected set; }
        public event EventHandler<EventArgs> UpdateOrderChanged;

        public virtual void Update(GameTime gameTime) { }
        #endregion
        #region IDrawable Members
        public virtual void Draw(GameTime Time) { }

        public int DrawOrder { get; set; }

        public event EventHandler<EventArgs> DrawOrderChanged;

        protected bool _Visible;
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                if (_Visible != value)
                {
                    _Visible = value;
                    if (VisibleChanged != null) VisibleChanged(this, new EventArgs());
                }
            }
        }

        public event EventHandler<EventArgs> VisibleChanged;
        #endregion
        #region IGameComponent Members
        public virtual void Initialize() { }
        #endregion
    }
}
