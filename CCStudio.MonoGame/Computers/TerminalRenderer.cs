using CCStudio.Core.Computers;
using CCStudio.Core.Computers.Time;
using CCStudio.Core.Display;
using CCStudio.MonoGame.Components;
using CCStudio.MonoGame.Contents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

namespace CCStudio.MonoGame.Computers
{
    public class TerminalRenderer : IGameElement
    {
        /// <summary>
        /// Computer Info
        /// </summary>
        protected Computer Owner;

        protected BasicTerminal Terminal;

        /// <summary>
        /// Parent game
        /// </summary>
        protected CoreGame Game;

        /// <summary>
        /// Render utilities
        /// </summary>
        protected Texture2D BackgroundTexture;
        protected SpriteFont Font;

        /// <summary>
        /// Size of pixels
        /// </summary>
        public static readonly Point PixelSize = new Point(12, 18);

        /// <summary>
        /// Mouse info
        /// </summary>
        public MouseButtons FirstDown = MouseButtons.Null;
        public Point LastPosition;

        /// <summary>
        /// Drawing info
        /// </summary>
        protected RenderTarget2D Target;

        protected bool TargetBUse = false;

        protected Texture2D TargetTexture;
        protected SpriteBatch IBatch;

        protected bool NeedRender = true;

        protected Dictionary<int, IdTimer> KeysDown = new Dictionary<int, IdTimer>();

        public TerminalRenderer(Computer Owner, CoreGame Game, Rectangle Size) : base(Game.Batch)
        {
            //Load assets
            Font = AssetManager.CoreFont;
            BackgroundTexture = AssetManager.PixelBackground;

            this.Game = Game;

            Terminal = new BasicTerminal();
            Owner.Terminal = Terminal;

            //IGameElement setters
            this.Size = Size;
            this.Visible = true;

            this.TracksEvents = true;
            this.Enabled = true;

            //Load computer
            this.Owner = Owner;

            //Set target
            Target = new RenderTarget2D(Game.GraphicsDevice, Size.Width, Size.Height);

            TargetTexture = (Texture2D)Target;
            IBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        #region Drawing
        public override void Draw(GameTime Time)
        {
            foreach (Pixel Pix in Terminal.Pixels.Pixels)
            {
                DrawPixel(Batch, Pix);
            }

            if (Terminal.CursorBlink && Time.TotalGameTime.Milliseconds % 600 < 300)
            {
                int Height = 2;
                Batch.Draw(BackgroundTexture, new Rectangle(Terminal.CursorX * PixelSize.X  - PixelSize.X + Size.X, Terminal.CursorY * PixelSize.Y - Height + Size.Y, PixelSize.X, Height), ColorLookups.Colours[Terminal.ForegroundColour]);
            }
        }

        protected void DrawPixel(SpriteBatch Batch, Pixel Pix)
        {
            Color Background = ColorLookups.Colours[Pix.Background];

            int X = Pix.X * PixelSize.X + Size.X;
            int Y = Pix.Y * PixelSize.Y + Size.Y;
            Batch.Draw(BackgroundTexture, new Rectangle(X, Y, PixelSize.X, PixelSize.Y), Background);

            if (Pix.Character != ' ')
            {
                Color Foreground = ColorLookups.Colours[Pix.Foreground];
                Batch.DrawString(Font, Pix.Character.ToString(), new Vector2(X, Y), Foreground);
            }
        }
        #endregion

        #region Mouse Events
        public override void MouseDown(MouseButtons Button, Point Position)
        {
            Point TermPosition = (Position - new Point(Size.X, Size.Y)) / PixelSize;
            Owner.PushEvent("mouse_click", (int)Button, TermPosition.X + 1, TermPosition.Y + 1);

            //Set first button down
            if (FirstDown == MouseButtons.Null) FirstDown = Button;
        }

        public override void MouseMove(Point Position)
        {
            if (FirstDown != MouseButtons.Null)
            {
                Point TermPosition = (Position - new Point(Size.X, Size.Y)) / PixelSize;

                if (TermPosition != LastPosition)
                {
                    Owner.PushEvent("mouse_drag", ((int)FirstDown), TermPosition.X + 1, TermPosition.Y + 1);
                    LastPosition = TermPosition;
                }
            }
        }

        public override void MouseUp(MouseButtons Button, Point Position)
        {
            if (FirstDown == Button) FirstDown = MouseButtons.Null;
        }

        public override void MouseScroll(int Ammount)
        {
            //-1 for up, 1 for down
            Owner.PushEvent("mouse_scroll", Ammount > 0 ? -1 : 1);
        }
        #endregion
        #region KeyEvents
        public override void KeyDown(Keys Key)
        {
            int KeyInt;
            if (KeyLookup.KeyToInt.TryGetValue(Key, out KeyInt))
            {
                IdTimer KeyTime = new IdTimer(KeyInt, 448);
                KeyTime.Elapsed += KeyTime_Elapsed;
                KeyTime.Start();

                KeysDown.Add(KeyInt, KeyTime);

                Owner.PushEvent("key", KeyInt);
            }
        }

        protected void KeyTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            IdTimer KeyTime = (IdTimer)sender;
            KeyTime.Interval = 32;
            int TimerId = KeyTime.Id;

            if (KeysDown.ContainsKey(TimerId))
            {
                Owner.PushEvent("key", TimerId);
            }
            else
            {
                KeyTime.Stop();
                KeyTime.Dispose();
            }
        }

        public override void KeyUp(Keys Key)
        {
            string Out = String.Format("Got Key up {0} ", Key);
            int KeyInt;
            if (KeyLookup.KeyToInt.TryGetValue(Key, out KeyInt))
            {
                IdTimer KeyTime;
                if (KeysDown.TryGetValue(KeyInt, out KeyTime))
                {
                    KeyTime.Stop();
                    KeysDown.Remove(KeyInt);
                    KeyTime.Dispose();
                }
            }
        }

        public override void KeyPress(char Character)
        {
            if(Character >= 32 && Character <= 127) Owner.PushEvent("char", Character.ToString());
        }
        #endregion
    }
}
