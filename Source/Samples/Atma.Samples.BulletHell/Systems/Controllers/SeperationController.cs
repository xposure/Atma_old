using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Entities;
using Atma.Samples.BulletHell.Systems.Phsyics;
using Microsoft.Xna.Framework;
using Atma.Managers;
using Atma.Rendering.Sprites;

namespace Atma.Samples.BulletHell.Systems.Controllers
{

    public class SeperationComponent : Component
    {
        public float radius = 5f;
        public float force = 5f;
    }

    public class SeperationController : IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:seperation";
        private struct Seperator : IRadixKey
        {
            public Transform transform;
            public PhysicsComponent physics;
            public SeperationComponent seperate;
            public int Key { get { return MinX; } }

            public int MinX { get { return (int)(transform.Position.X - seperate.radius); } }
            public int MaxX { get { return (int)(transform.Position.X + seperate.radius); } }
        }

        private int overlaps = 0;
        private int overlaptests = 0;

        public void update(float delta)
        {
            var objs = new List<Seperator>();
            var em = CoreRegistry.require<EntityManager>();

            //var player = em.createRef(em.getEntityByTag("player"));
            
            

            foreach (var id in em.getWithComponents("transform", "physics", "seperate"))
            {
                var transform = em.getComponent<Transform>(id, "transform");
                var physics = em.getComponent<PhysicsComponent>(id, "physics");
                var seperate = em.getComponent<SeperationComponent>(id, "seperate");
                var sprite = em.getComponent<Sprite>(id, "sprite");

                

                objs.Add(new Seperator() { physics = physics, transform = transform, seperate = seperate });
            }

            overlaps = 0;
            overlaptests = 0;
            var objArr = objs.ToArray();
            Utility.RadixSort(objArr);

            var hits = new int[objArr.Length];
            var force = new Vector2[objArr.Length];
            for (var i = 0; i < objArr.Length; i++)
            {
                var push = objArr[i].seperate.force;
                var p = objArr[i].transform.Position;
                var circle = new Circle(p, objArr[i].seperate.radius);
                for (var k = i + 1; k < objArr.Length; k++)
                {
                    if (objArr[k].MinX > objArr[i].MaxX)
                        break;
                    
                    overlaptests++;

                    var otherp = objArr[k].transform.Position;
                    var other = new Circle(otherp, objArr[k].seperate.radius);
                    if (circle.Intersects(other))
                    {
                        var d = p - otherp;
                        var amt = d / (d.LengthSquared() + 1);
                        force[i] += amt * push; //apply opposite force?
                        force[k] += -amt * push;
                        hits[i]++;
                        hits[k]++;
                        overlaps++;
                    }
                }
            }

            for (var i = 0; i < objArr.Length; i++)
            {
                if (hits[i] > 0)
                {
                    objArr[i].physics.velocity += force[i] / hits[i];
                }
            }
        }



        public void init()
        {
            CoreRegistry.require<GUIManager>().onRender += PhysicsSystem_onRender;
            //CoreRegistry<GUIManager>.require(
        }

        void PhysicsSystem_onRender(GUIManager obj)
        {

            //obj.label(new Vector2(0, 250), overlaps.ToString() + "/" + overlaptests.ToString());
        }

        public void shutdown()
        {
        }
    }
}
