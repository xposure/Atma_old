using Atma.Entities;
using Atma.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma;
using Atma.Managers;
using Atma.Rendering;
using Atma.Input;
using Atma.Samples.BulletHell.World;

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
            var input = CoreRegistry.require<InputSystem>(InputSystem.Uri);
            var world = this.world();

            foreach (var id in em.getWithComponents("transform", "trackmouse"))
            {
                var transform = em.getComponent<Transform>(id, "transform");
                var marker = em.getComponent<MarkerComponent>(id, "trackmouse");

                var wp = world.currentCamera.screenToWorld(input.MousePosition);
                //var wp = CameraComponent.mainCamera.screenToWorld(input.MousePosition);

                if (marker.value)
                    transform.Position = wp;
                else
                    transform.LookAt(wp);
            }
        }

        public void shutdown()
        {
        }
    }
}
