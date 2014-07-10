using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma;
using Atma.Engine;
using Atma.Entity;
using Atma.Systems;
using Microsoft.Xna.Framework;

namespace Atma.Samples.BulletHell.Systems.Phsyics
{
    public class PhysicsSystem : IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:physics";
        private float tick = 1f / 60f;
        private float accumulator = 0f;

        public void update(float delta)
        {
            accumulator += delta;

            while (accumulator > tick)
            {
                accumulator -= tick;

                var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
                foreach (var id in em.getWithComponents("transform", "physics"))
                {
                    var transform = em.getComponent<Transform>(id, "transform");
                    var physics = em.getComponent<PhysicsComponent>(id, "physics");


                    transform.Position += physics.velocity;
                    //transform.LookAt(wp);
                }
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
