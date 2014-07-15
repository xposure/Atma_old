#region Using Statements
using Atma.Core;
using Atma.Subsystem.BindableInput;
using System;
using System.Collections.Generic;
using System.Linq;
using Atma.Managers;
#endregion

namespace Atma.Samples.Sandbox
{

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
            using (var game = new Game(new RenderingTests.InGameState(),
                new StopwatchTime(),
                new BindableInputManager(),
                new Graphics.DisplayDevice(),
                new Graphics.GraphicSubsystem()))
                game.Run();

            //Console.Read();
        }
    }
#endif
}
