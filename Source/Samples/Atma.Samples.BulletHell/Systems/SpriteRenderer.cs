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

        public event Action<GraphicSubsystem> onBeforeRender;
        public event Action<GraphicSubsystem> onAfterRender;

        public void render()
        {
            var graphics = CoreRegistry.require<GraphicSubsystem>(GraphicSubsystem.Uri);
            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            camera.begin();
            if (onBeforeRender != null)
                onBeforeRender(graphics);

            foreach (var id in em.getWithComponents("transform", "sprite"))
            {
                var transform = em.getComponent<Transform>(id, "transform");
                var sprite = em.getComponent<SpriteComponent>(id, "sprite");

                var size = sprite.size * transform.DerivedScale;
                var p = transform.DerivedPosition + sprite.offset;
                //var len = p.Length();
                //size *= (1f - len / 2048f);
                //p *= (1f - len / 2048f);
                

                graphics.Draw(0,
                              sprite.material,
                              p,
                              AxisAlignedBox.Null,
                              sprite.color,
                              transform.DerivedOrientation + sprite.rotation,
                              sprite.origin,
                              size,
                              sprite.spriteEffect,
                              transform.DerivedDepth);

            }

            if (onAfterRender != null)
                onAfterRender(graphics);

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
