using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Entities;

namespace Atma.Samples.BulletHell.Systems
{
    public class ExpireComponent : Component
    {
        public float timer = 1f;
    }

    public class ExpirationSystem : IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:expire";

        private Random random = new Random();

        public void update(float delta)
        {
            var em = CoreRegistry.require<EntityManager>();
            var ids = em.getWithComponents("expire").ToArray();
            foreach (var id in ids)
            {
                var expire = em.getComponent<ExpireComponent>(id, "expire");
                expire.timer -= delta;

                if (expire.timer <= 0f)
                    em.destroy(id);
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
