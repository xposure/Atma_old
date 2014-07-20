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
    public class MoveController : IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:move";
        private float tick = 1f / 60f;
        private float accumulator = 0f;

        public void update(float delta)
        {
            accumulator += delta;

            while (accumulator > tick)
            {
                accumulator -= tick;

                var em = CoreRegistry.require<EntityManager>();
                foreach (var id in em.getWithComponents("transform", "physics", "input"))
                {
                    var input = em.getComponent<InputComponent>(id, "input");

                    if (input.disabled)
                        continue;

                    var transform = em.getComponent<Transform>(id, "transform");
                    var physics = em.getComponent<PhysicsComponent>(id, "physics");

                    if (input.thrust != Vector2.Zero)
                    {
                        var steering = Steering.seek(physics.speed, transform.Position, transform.Position + input.thrust);
                        //var sep = seperation(p);
                        //if (sep != Vector2.Zero)
                        //{
                        //    steering += Steering.seek(speed, p, seperation(p)) * 0.25f;
                        //}
                        physics.velocity = steering.integrate(transform.Position, physics.velocity, physics.maxForce, physics.speed, physics.mass);
                        transform.LookAt(transform.Position + physics.velocity);
                    }
                    else
                    {
                        //wander instead
                        physics.velocity *= physics.drag;
                        if (physics.velocity.X >= -0.01 && physics.velocity.X <= 0.01)
                            physics.velocity.X = 0f;

                        if (physics.velocity.Y >= -0.01 && physics.velocity.Y <= 0.01)
                            physics.velocity.Y = 0f;
                    }
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
