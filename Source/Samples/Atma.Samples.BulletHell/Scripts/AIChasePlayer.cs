//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Atma;

//namespace GameName1.BulletHell.Scripts
//{
//    public class AIChaseEntity : Script
//    {
//        public float speed = 4f;
//        public Vector2 velocity;
//        public float mass = 10f;
//        public float maxForce = 2.4f;
//        public float drag = 0.94f;
//        public GameObject target;

//        private Enemy _entity;
//        private List<Entity> collisionCache = new List<Entity>(10);

//        private void init()
//        {
//            _entity = gameObject.getScript<Enemy>();
//        }

//        private void fixedupdate()
//        {
//            if (!_entity.IsActive)
//                return;

//            var p = transform.Position;

//            if (target != null && target.destroyed)
//                target = null;

//            var hasTarget = target != null;
//            if (hasTarget)
//            {
//                var steering = Steering.seek(speed, p, target.transform.DerivedPosition);
//                //var sep = seperation(p);
//                //if (sep != Vector2.Zero)
//                //{
//                //    steering += Steering.seek(speed, p, seperation(p)) * 0.25f;
//                //}
//                velocity = steering.integrate(p, velocity, maxForce, speed, mass);
//            }
//            else
//            {
//                //wander instead
//                velocity *= drag;
//                if (velocity.X >= -0.01 && velocity.X <= 0.01)
//                    velocity.X = 0f;

//                if (velocity.Y >= -0.01 && velocity.Y <= 0.01)
//                    velocity.Y = 0f;
//            }



//            seperation(p);

//            //p += velocity;
//            //transform.Position = p;
//            //transform.Forward = velocity;
//        }

//        private void afterfixedupdate()
//        {
//            transform.Position += velocity;
//            transform.Forward = velocity;
//        }

//        private void seperation(Vector2 p)
//        {
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

//            //if (count == 0)
//            //    return Vector2.Zero;

//            //return (modifier / count).ToNormalized();
//        }
//    }
//}
