using Atma.Engine;
using Atma.Entities;
using Atma.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Samples.BulletHell.Systems
{
    public class FiberSystem : IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:fiber";

        private FiberScheduler _fiber = new FiberScheduler();

        public void update(float delta)
        {
            _fiber.tick();
        }

        public void schedule(IFiber fiber)
        {
            _fiber.schedule(fiber);
        }

        public void schedule(IFiber fiber, TimeSpan ts)
        {
            _fiber.schedule(fiber,ts);
        }

        public void schedule(IFiber fiber, float delay)
        {
            _fiber.schedule(fiber, delay);
        }

        public void schedule(IFiber fiber, double delay)
        {
            _fiber.schedule(fiber, delay);
        }

        public void schedule(IFiber fiber, int frameDelay)
        {
            _fiber.schedule(fiber, frameDelay);
        }

        public void init()
        {
        }

        public void shutdown()
        {
        }
    }
}
