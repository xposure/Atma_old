#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
//using Microsoft.Xna.Framework.GamerServices;
using Atma;
//using GameName1.Space;
using Microsoft.Xna.Framework.Media;
//using GameName1.Space.World;
using Atma.Engine;
#endregion

namespace GameName1.BulletHell
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : GameEngine
    {
        public static Game instance { get; private set; }


        public Game(Atma.Engine.IGameState initialState, params Atma.Rendering.ISubsystem[] subsystems)
            : base(initialState, subsystems)
        {
            instance = this;
            //IsFixedTimeStep = false;

        }

        protected override void LoadContent()
        {
            IsMouseVisible = false;
            //GameWindowExtensions.SetPosition(this.Window, new Point(200, 100));
            //Atma.Engine.CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);//.setDevice(this.GraphicsDevice);

            var assets = Atma.Engine.CoreRegistry.require<Atma.Assets.AssetManager>();
            assets.addAssetSource(new Atma.Assets.Sources.DirectorySource("bullethell", "..\\content"));


            //Root.instance.RootObject.createScript<Init>();
            GraphicsDevice.Clear(Color.Black);
            base.LoadContent();
        }

        private bool isLoaded = false;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //time.preUpdate();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);

            if (!isLoaded)
            {
                isLoaded = true;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            //Root.instance.graphics.Draw(0, material, new Rectangle(20, 20, 100, 100), Color.Red);
            base.Draw(gameTime);
            PerformanceMonitor.rollCycle();
        }
    }
}
