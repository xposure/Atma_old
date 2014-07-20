using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Entities;
using Atma.Samples.BulletHell.Systems.Phsyics;
using GameName1.BulletHell;
using Microsoft.Xna.Framework;

namespace Atma.Samples.BulletHell.Systems.Controllers
{
    public class ChaseComponent : Component
    {
        public int target;
    }

    public class ChaseController : GameSystem, IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:chase";

        public void update(float delta)
        {
            //var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            foreach (var id in entities.getWithComponents("transform", "input", "chase"))
            {
                var transform = entities.getComponent<Transform>(id, "transform");
                var input = entities.getComponent<InputComponent>(id, "input");
                var chase = entities.getComponent<ChaseComponent>(id, "chase");

                input.thrust = Vector2.Zero;

                if (chase.target <= 0)
                    continue;

                if (!entities.exists(chase.target))
                    chase.target = 0;

                var target = entities.getComponent<Transform>(chase.target, "transform");
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
