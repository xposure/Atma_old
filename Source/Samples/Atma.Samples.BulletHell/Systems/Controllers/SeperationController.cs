﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Entity;
using Atma.Samples.BulletHell.Systems.Phsyics;
using Atma.Systems;
using Microsoft.Xna.Framework;
using Atma.Managers;

namespace Atma.Samples.BulletHell.Systems.Controllers
{

    public class SeperationComponent : Component
    {
        public float radius = 10f;
        public float force = 10f;
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

            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            foreach (var id in em.getWithComponents("transform", "physics", "seperate"))
            {
                var transform = em.getComponent<Transform>(id, "transform");
                var physics = em.getComponent<PhysicsComponent>(id, "physics");
                var seperate = em.getComponent<SeperationComponent>(id, "seperate");
                objs.Add(new Seperator() { physics = physics, transform = transform, seperate = seperate });

                //            collisionCache.Clear();
                //            rootObject.broadcast("checkCollision", (Entity)_entity, collisionCache);

                //            var count = 0;
                //            var modifier = Vector2.Zero;
                //            if (collisionCache.Count > 0)
                //            {
                //                var radiusSq = _entity.radius * _entity.radius;

                //                foreach (var e in collisionCache)
                //                {
                //                    var d = p - e.transform.DerivedPosition;
                //                    modifier += d / (d.LengthSquared() + 1);
                //                    count++;
                //                    //var d = p - e.transform.DerivedPosition;
                //                }

                //            }

                //            if (count > 0)
                //                velocity += modifier / count;


                //transform.Position += physics.velocity;
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
