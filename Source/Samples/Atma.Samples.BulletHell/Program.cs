#region Using Statements
using Atma.Core;
using Atma.Samples.BulletHell.States;
using Atma.Subsystem.BindableInput;
using GameName1.BulletHell;
using System;
using System.Collections.Generic;
using System.Linq;
using Atma.Managers;
#endregion

namespace Atma.Samples.BulletHell
{

    public class Fuck : Microsoft.Xna.Framework.Game
    {
        Microsoft.Xna.Framework.GraphicsDeviceManager graphics;
        public Fuck()
        {
            graphics = new Microsoft.Xna.Framework.GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            base.Initialize();

            //TargetElapsedTime = TimeSpan.FromSeconds(0);
            //IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();

        }

        private float shit = 0f;
        private float amt = 0f;
        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                this.Exit();

            var delta =  (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (shit != 0)
            {
                shit = delta;
            }
            else
            {
                shit = delta * 0.95f + delta * 0.05f;
            }

            amt += delta;

            if (amt > 1f)
            {
                this.Window.Title = shit.ToString();
                System.Diagnostics.Debug.WriteLine(shit);
                amt = 0;                
            }

            base.Update(gameTime);
        }
    }
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //using (var game = new Fuck())
            //{
            //    game.Run();
            //}
            new ConsoleLogger();
            using (var game = new Game(new InGameState(),
                new StopwatchTime(),
                new BindableInputManager(),
                new Graphics.DisplayDevice(),
                new Graphics.GraphicSubsystem2()))
                game.Run();

            //Console.Read();
        }
    }
#endif
}
