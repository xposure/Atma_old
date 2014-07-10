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
                ////if (target != null && target.destroyed)
                ////    target = null;

                //var hasTarget = target != null;
                //if (hasTarget)
                //{
                //    var steering = Steering.seek(physics.speed, p, target.transform.DerivedPosition);
                //    //var sep = seperation(p);
                //    //if (sep != Vector2.Zero)
                //    //{
                //    //    steering += Steering.seek(speed, p, seperation(p)) * 0.25f;
                //    //}
                //    physics.velocity = steering.integrate(p, physics.velocity, physics.maxForce, physics.speed, physics.mass);
                //}
                //else
                //{
                //    //wander instead
                //    physics.velocity *= physics.drag;
                //    if (physics.velocity.X >= -0.01 && physics.velocity.X <= 0.01)
                //        physics.velocity.X = 0f;

                //    if (physics.velocity.Y >= -0.01 && physics.velocity.Y <= 0.01)
                //        physics.velocity.Y = 0f;
                //}

                //transform.Position += physics.velocity;
                ////transform.LookAt(wp);
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
