using Atma.Entity;
using Atma.Systems;
using Atma.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma;
using Atma.Managers;

namespace Atma.Samples.BulletHell.Systems
{
    public class TrackMouseSystem : IComponentSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:trackmouse";

        public void init()
        {
        }

        public void update(float delta)
        {
            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            var input = CoreRegistry.require<InputManager>(InputManager.Uri);

            foreach (var id in em.getWithComponents("transform", "trackmouse"))
            {
                var transform = em.getComponent<Transform>(id, "transform");
                var wp = Camera.mainCamera.screenToWorld(input.MousePosition);
                transform.Position = wp;
                //transform.LookAt(wp);
            }
        }

        public void shutdown()
        {
        }
    }
}
