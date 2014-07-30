using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CCStudio.MonoGame.Computers;
using CCStudio.MonoGame.Contents;
using CCStudio.Core.Computers;
using CCStudio.MonoGame.Components;
using System;

namespace CCStudio.MonoGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CoreGame : Game
    {
        public GraphicsDeviceManager Graphics;
        public SpriteBatch Batch;

        TerminalRenderer Renderer;
        Computer Comp;
        Session Ses;

        ElementManager Manager;

        public CoreGame()
            : base()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferredBackBufferWidth = 612;
            Graphics.PreferredBackBufferHeight = 342;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            if (true)
            {
                Ses = Session.Load("Computers/Dump.xml");
                Comp = new Computer(Ses.Computers[0]);
            }
            else
            {
                Ses = new Session("Computers/Dump.xml");
                Comp = new Computer(Ses);
            }

            Manager = new ElementManager(this);
            Components.Add(Manager);
            base.Initialize();
        }

        /// <summary>
        /// Load all content
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Batch = new SpriteBatch(GraphicsDevice);
            
            //Load distributed content
            AssetManager.LoadContent(Content, GraphicsDevice);

            Renderer = new TerminalRenderer(Comp, this, new Rectangle(0, 0, 612, 342));
            Manager.Add(Renderer);

            Comp.TurnOn();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload all content.
        /// </summary>
        protected override void UnloadContent()
        {
            AssetManager.PixelBackground.Dispose();
            Ses.Save();
        }

        /// <summary>
        /// Called every tick, update anything that needs to be updated.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw the game.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //We probably shouldn't call this - save power
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Draw what needs to be drawn
            Batch.Begin();
            base.Draw(gameTime);
            Batch.End();
            
        }
    }
}
