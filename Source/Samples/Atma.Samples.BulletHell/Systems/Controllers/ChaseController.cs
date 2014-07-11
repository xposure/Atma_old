using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Entity;
using Atma.Samples.BulletHell.Systems.Phsyics;
using Atma.Systems;
using GameName1.BulletHell;
using Microsoft.Xna.Framework;

namespace Atma.Samples.BulletHell.Systems.Controllers
{
    public class ChaseComponent : Component
    {
        public int target;
    }

    public class ChaseController : IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:chase";

        public void update(float delta)
        {
            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            foreach (var id in em.getWithComponents("transform", "input", "chase"))
            {
                var transform = em.getComponent<Transform>(id, "transform");
                var input = em.getComponent<InputComponent>(id, "input");
                var chase = em.getComponent<ChaseComponent>(id, "chase");

                input.thrust = Vector2.Zero;

                if (chase.target <= 0)
                    continue;

                if (!em.exists(chase.target))
                    chase.target = 0;

                var target = em.getComponent<Transform>(chase.target, "transform");
                if (target == null)
                    continue;


                input.thrust = target.Position - transform.Position;
                input.thrust.Normalize();                
            }
        }

        public void init()
        {

        }

        public void shutdown()
        {
        }
    }
}
