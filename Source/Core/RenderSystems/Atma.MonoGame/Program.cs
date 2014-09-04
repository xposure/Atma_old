using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.MonoGame
{
    public class Program
    {
        public static void Main()
        {
            new ConsoleLogger();
            var renderSystem = new MGRenderSystem();
            renderSystem.Initialize(true);
            while (!Console.KeyAvailable)
            {
                Platform.doEvents();
                renderSystem.render();
            }
        }
    }
}
