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
        //Computer Info
        protected Computer Comp;
        protected Terminal Term;

        //Parent game
        protected CoreGame Game;

        //Render utilities
        protected Texture2D BackgroundTexture;
        protected SpriteFont Font;

        //Size of pixels
        public static readonly Point PixelSize = new Point(12, 18);

        //Mouse info
        public MouseButtons FirstDown = MouseButtons.Null;
        public Point LastPosition;

        // Drawing info
        protected RenderTarget2D Target;

        protected bool TargetBUse = false;

        protected Texture2D TargetTexture;
        protected SpriteBatch IBatch;

        protected bool NeedRender = true;

        protected Dictionary<int, IdTimer> KeysDown = new Dictionary<int, IdTimer>();

        public TerminalRenderer(Computer Comp, CoreGame Gme, Rectangle Size) : base(Gme.Batch)
        {
            //Load assets
            Font = AssetManager.CoreFont;
            BackgroundTexture = AssetManager.PixelBackground;

            this.Game = Gme;

            //IGameElement setters
            this.Size = Size;
            this.Visible = true;

            this.TracksEvents = true;
            this.Enabled = true;

            //Load computer
            this.Comp = Comp;
            Term = Comp.Term;

            //Listen to events
            Term.TerminalChanged += Term_TerminalChanged;

            //Set target
            Target = new RenderTarget2D(Game.GraphicsDevice, Size.Width, Size.Height);

            TargetTexture = (Texture2D)Target;
            IBatch = new SpriteBatch(Gme.GraphicsDevice);
        }

        #region Terminal Events
        protected void Term_TerminalChanged(Terminal sender)
        {
            NeedRender = true;
        }
        #endregion

        #region Drawing
        public override void Draw(GameTime Time)
        {
            /*
            if (NeedRender == true)
            {
                NeedRender = false;
                //DrawToRender();
            }*/

            //Batch.Draw(TargetTexture, Size, Color.White);
            foreach (Pixel Pix in Term.Pixels.Pixels)
            {
                DrawPixel(Batch, Pix);
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
        protected void DrawToRender()
        {
            Game.Graphics.GraphicsDevice.SetRenderTarget(Target);
            
            //Game.Graphics.GraphicsDevice.Clear(Color.Black);

            IBatch.Begin();

            foreach (Pixel Pix in Term.Pixels.Pixels)
            {
                DrawPixel(IBatch, Pix);
            }
            IBatch.End();

            Game.Graphics.GraphicsDevice.SetRenderTarget(null);

            TargetTexture = (Texture2D)Target;
        }

        #endregion

        #region Mouse Events
        public override void MouseDown(MouseButtons Button, Point Position)
        {
            Point TermPosition = (Position - new Point(Size.X, Size.Y)) / PixelSize;
            Comp.PushEvent("mouse_click", (int)Button, TermPosition.X + 1, TermPosition.Y + 1);

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
                    Comp.PushEvent("mouse_drag", ((int)FirstDown), TermPosition.X + 1, TermPosition.Y + 1);
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
            Comp.PushEvent("mouse_scroll", Ammount > 0 ? -1 : 1);
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

                Comp.PushEvent("key", KeyInt);
            }
        }

        protected void KeyTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            IdTimer KeyTime = (IdTimer)sender;
            KeyTime.Interval = 32;
            int TimerId = KeyTime.Id;

            if (KeysDown.ContainsKey(TimerId))
            {
                Comp.PushEvent("key", TimerId);
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
            if(Character >= 32 && Character <= 127) Comp.PushEvent("char", Character.ToString());
        }
        #endregion
    }
}
