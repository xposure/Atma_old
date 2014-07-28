using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Entities;
using Atma.Samples.BulletHell.Systems.Phsyics;

namespace Atma.Samples.BulletHell._2_0.Weapons
{
    public class Bullet : Component
    {
        public int damage = 100;
        public float speed = 500.0f;
        public float range = 9999.0f;
        public float knockbackForce = 0.0f;
        //public GameObject boom;
        //public DamageEvent data;

        public float life = 9999.0f;
        //public GameObject owner;
        public int team = 0;
        public bool environmental = false;
        public bool shakesCamera = false;
        public bool penetrates = false;
        //public string[] effects;
        public float distanceTravelled = 0.0f;
    }

    public class BulletSystem : GameSystem, IComponentSystem, IUpdateSubscriber
    {
        public void init()
        {
        }

        public void shutdown()
        {
        }

        public void update(float delta)
        {
            foreach (var id in this.entities.getEntitiesByTag("bullet").ToArray())
            {
                var rid = this.entities.createRef(id);

                var transform = rid.getComponent<Transform>("transform");
                var physics = rid.getComponent<PhysicsComponent>("physics");
                var bullet = rid.getComponent<Bullet>("bullet");

                if (bullet.life == bullet.range)
                {
                    //start
                    bullet.life = (bullet.range / bullet.life);

                    if (bullet.shakesCamera)
                    {
                        //send camera shake event
                    }
                }

                if (!bullet.environmental)
                {
                    bullet.life -= delta;
                    if (bullet.life <= 0)
                        rid.destroy();
                }
            }
        }
    }
}
