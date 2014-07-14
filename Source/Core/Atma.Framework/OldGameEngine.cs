#region Using Statements

using Atma.Engine;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

#endregion Using Statements

public class User32
{
    [DllImport("user32.dll")]
    public static extern void SetWindowPos(uint Hwnd, uint Level, int X, int Y, int W, int H, uint Flags);
}

namespace Atma
{
    public partial class OldGameEngine : Microsoft.Xna.Framework.Game
    {
        //private StopwatchTime time = new StopwatchTime();
        //private GraphicsDeviceManager graphics;

        //SpriteBatch spriteBatch;
        private Root root;

        private bool skipDraw = true;

        public OldGameEngine()
            : base()
        {
            IsFixedTimeStep = true;
            //graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            //content = this.Content;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (skipDraw)
                skipDraw = false;
            else
            {
                root.draw();
                base.Draw(gameTime);
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsFixedTimeStep = false;
            //graphics.SynchronizeWithVerticalRetrace = false;
            //graphics.PreferredBackBufferWidth = 1024;
            //graphics.PreferredBackBufferHeight = 768;
            //graphics.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;
            //graphics.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
            //graphics.ApplyChanges();
            var gfx = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
           // gfx.setDevice(graphics.GraphicsDevice);
            
            var display = CoreRegistry.require<Atma.Graphics.DisplayDevice>(Atma.Graphics.DisplayDevice.Uri);
            //display.setGraphicsDeviceManager(graphics);

            
            //time.init();
            //spriteBatch.Begin(
            root = new Root();
            //root.reload();
            base.Initialize();

            //display.SetResolution(this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight, this.graphics.IsFullScreen);
            root.start(this.GraphicsDevice, this.Content);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //spriteBatch = new SpriteBatch(GraphicsDevice);
            base.LoadContent();
        }


        protected override void BeginRun()
        {
            base.BeginRun();
            //root.reload();
        }

        protected override void EndRun()
        {
            root.cleanup();
            base.EndRun();
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //foreach (var tick in time.tick())
            {
                this.Window.Title = gameTime.ElapsedGameTime.TotalSeconds.ToString() + "    " + TargetElapsedTime.TotalSeconds.ToString();
                //TargetElapsedTime = TimeSpan.FromSeconds(0.5);
                root.update(gameTime.TotalGameTime.TotalSeconds);
                //base.Update(gameTime);
                //User32.SetWindowPos((uint)this.Window.Handle, 0, 0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 0);
            }
        }


    }
}