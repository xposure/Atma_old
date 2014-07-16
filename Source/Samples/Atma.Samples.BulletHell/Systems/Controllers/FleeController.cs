using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Entities;
using Atma.Samples.BulletHell.Systems.Phsyics;
using Atma.Systems;
using Microsoft.Xna.Framework;
using Atma.Managers;

namespace Atma.Samples.BulletHell.Systems.Controllers
{

    public class FleeComponent : Component
    {
        public float radius = 5f;
        public float force = 1f;
        public float strength = 1f;
    }

    public class FleeController : IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:flee";
        private struct Flee
        {
            public Transform transform;
            public FleeComponent flee;
            public Circle circle;
        }

        private int overlaps = 0;
        private int overlaptests = 0;

        public void update(float delta)
        {
            var objs = new List<Flee>();

            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            foreach (var id in em.getWithComponents("transform", "flee"))
            {
                var transform = em.getComponent<Transform>(id, "transform");
                var flee = em.getComponent<FleeComponent>(id, "flee");
                flee.force -= delta;
                if (flee.force > 0)
                {
                    var holder = new Flee() { transform = transform, flee = flee, circle = new Circle(transform.DerivedPosition, flee.radius) };
                    objs.Add(holder);

                }
            }

            foreach (var id in em.getEntitiesByTag("enemy"))
            {
                var transform = em.getComponent<Transform>(id, "transform");
                var physics = em.getComponent<PhysicsComponent>(id, "physics");

                var circle = new Circle(transform.DerivedPosition, physics.radius);
                foreach (var flee in objs)
                {
                    if (circle.Intersects(flee.circle))
                    {
                        var steering = Steering.flee(flee.flee.force, physics.velocity, transform.DerivedPosition, flee.transform.DerivedPosition);
                        physics.velocity += steering.steering;//steering.integrate(transform.Position, physics.velocity, physics.maxForce, physics.speed, physics.mass);
                    }
                }
            }

        }



        public void init()
        {
            CoreRegistry.require<GUIManager>(GUIManager.Uri).onRender += PhysicsSystem_onRender;
            //CoreRegistry<GUIManager>.require(
        }

        void PhysicsSystem_onRender(GUIManager obj)
        {

            obj.label(new Vector2(0, 150), overlaps.ToString() + "/" + overlaptests.ToString());
        }

        public void shutdown()
        {
        }
    }
}
