using Atma.Engine;
using Atma.Entity;
using Atma.Graphics;
using Atma.MonoGame.Graphics;
using Atma.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Samples.BulletHell.Systems
{
    public class SpriteRenderer : IComponentSystem, IRenderSubscriber
    {
        public static readonly GameUri Uri = "componentsystem:sprite";

        private IRenderTarget target = new RenderToScreen();
        private Camera camera;

        public void render()
        {
            var graphics = CoreRegistry.require<GraphicSubsystem>(GraphicSubsystem.Uri);
            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            camera.begin();
            foreach (var id in em.getWithComponents("transform", "sprite"))
            {
                var transform = em.getComponent<Transform>(id, "transform");
                var sprite = em.getComponent<SpriteComponent>(id, "sprite");

                graphics.Draw(0,
                              sprite.material,
                              transform.DerivedPosition + sprite.offset,
                              AxisAlignedBox.Null,
                              sprite.color,
                              transform.DerivedOrientation + sprite.rotation,
                              sprite.origin,
                              sprite.size * transform.DerivedScale,
                              sprite.spriteEffect,
                              transform.DerivedDepth);

            }
            camera.end();
            Camera.drawAll();
        }

        public void init()
        {
            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            var id = em.create();
            var cameraGO = em.createRef(id);
            
            camera = cameraGO.addComponent("camera", new Camera());
            var transform = cameraGO.addComponent("transform", new Transform());
            camera.clear = Color.Black;
            camera.init(transform);
        }

        public void shutdown()
        {
        }
    }
}
