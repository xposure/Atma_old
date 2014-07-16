using Atma.Engine;
using Atma.Entity;
using Atma.Graphics;
using Atma.Systems;
using Atma.TwoD.Rendering;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Samples.BulletHell.Systems
{
    public class SpriteRenderer : IComponentSystem, IRenderSubscriber, IRenderSystem
    {
        public static readonly GameUri Uri = "componentsystem:sprite";

        //private Camera camera;

        public event Action<GraphicSubsystem> onBeforeRender;
        public event Action<GraphicSubsystem> onAfterRender;

        public void render()
        {

            //var graphics = CoreRegistry.require<GraphicSubsystem>(GraphicSubsystem.Uri);
            //var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            //camera.begin();
            //if (onBeforeRender != null)
            //    onBeforeRender(graphics);

            //foreach (var id in em.getWithComponents("transform", "sprite"))
            //{
            //    var transform = em.getComponent<Transform>(id, "transform");
            //    var sprite = em.getComponent<SpriteComponent>(id, "sprite");

            //    var size = sprite.size * transform.DerivedScale;
            //    var p = transform.DerivedPosition + sprite.offset;
            //    //var len = p.Length();
            //    //size *= (1f - len / 2048f);
            //    //p *= (1f - len / 2048f);


            //    graphics.Draw(0,
            //                  sprite.material,
            //                  p,
            //                  AxisAlignedBox.Null,
            //                  sprite.color,
            //                  transform.DerivedOrientation + sprite.rotation,
            //                  sprite.origin,
            //                  size,
            //                  sprite.spriteEffect,
            //                  transform.DerivedDepth);

            //}

            //if (onAfterRender != null)
            //    onAfterRender(graphics);

            //camera.end();
            //Camera.drawAll();
        }

        public void init()
        {
            var world = this.world();


            var cs = CoreRegistry.require<CameraSystem>(CameraSystem.Uri);
            cs.prepareToRender += cs_prepareToRender;
            cs.renderOpaque += cs_renderOpaque;
            cs.renderAdditive += cs_renderAdditive;
            //var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            //var id = em.create();
            //var cameraGO = em.createRef(id);

            //camera = cameraGO.addComponent("camera", new Camera());
            //var transform = cameraGO.addComponent("transform", new Transform());
            //camera.clear = Color.Black;
            //camera.init(transform);
        }

        void cs_renderAdditive(CameraComponent arg1, RenderQueue arg2)
        {
            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            foreach (var id in em.getWithComponents("transform", "sprite"))
            {
                var transform = em.getComponent<Transform>(id, "transform");
                var sprite = em.getComponent<SpriteComponent>(id, "sprite");

                var size = sprite.size * transform.DerivedScale;
                var p = transform.DerivedPosition + sprite.offset;
                //var len = p.Length();
                //size *= (1f - len / 2048f);
                //p *= (1f - len / 2048f);
                var offset = sprite.size * sprite.origin;

                arg2.texture(sprite.material.texture);
                //arg2.position(p);
                arg2.source(AxisAlignedBox.Null);
                arg2.color(sprite.color);
                arg2.rotation(transform.DerivedOrientation + sprite.rotation);
                arg2.depth(transform.DerivedDepth);

                arg2.quad(p - offset, p + sprite.size - offset, sprite.origin, transform.DerivedOrientation + sprite.rotation);
                ////arg2.quad(p, 
                //graphics.Draw(0,
                //              sprite.material,
                //              p,
                //              AxisAlignedBox.Null,
                //              sprite.color,
                //              transform.DerivedOrientation + sprite.rotation,
                //              sprite.origin,
                //              size,
                //              sprite.spriteEffect,
                //              transform.DerivedDepth);

            }
        }

        void cs_renderOpaque(CameraComponent arg1, RenderQueue arg2)
        {
            return;
            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            foreach (var id in em.getWithComponents("transform", "sprite"))
            {
                var transform = em.getComponent<Transform>(id, "transform");
                var sprite = em.getComponent<SpriteComponent>(id, "sprite");

                var size = sprite.size * transform.DerivedScale;
                var p = transform.DerivedPosition + sprite.offset;
                //var len = p.Length();
                //size *= (1f - len / 2048f);
                //p *= (1f - len / 2048f);
                var offset = sprite.size * sprite.origin;

                arg2.texture(sprite.material.texture);
                arg2.position(p);
                arg2.source(AxisAlignedBox.Null);
                arg2.color(sprite.color);
                arg2.rotation(transform.DerivedOrientation + sprite.rotation);
                arg2.depth(transform.DerivedDepth);

                arg2.quad(p - offset, p + sprite.size - offset, sprite.origin, transform.DerivedOrientation + sprite.rotation);
                ////arg2.quad(p, 
                //graphics.Draw(0,
                //              sprite.material,
                //              p,
                //              AxisAlignedBox.Null,
                //              sprite.color,
                //              transform.DerivedOrientation + sprite.rotation,
                //              sprite.origin,
                //              size,
                //              sprite.spriteEffect,
                //              transform.DerivedDepth);

            }
        }

        void cs_prepareToRender(CameraComponent obj)
        {

        }

        public void shutdown()
        {

        }

        public void renderOpaque()
        {
            var world = this.world();
            var graphics = this.graphics();

            var em = this.entities();

            world.beginOpaque();
            foreach (var id in em.getWithComponents("transform", "sprite"))
            {
                var transform = em.getComponent<Transform>(id, "transform");
                var sprite = em.getComponent<SpriteComponent>(id, "sprite");

                var size = sprite.size;// *transform.DerivedScale;
                var p = transform.DerivedPosition + sprite.offset;
                size /= sprite.material.textureSize;
                size *= transform.DerivedScale;

                //var len = p.Length();
                //size *= (1f - len / 2048f);
                //p *= (1f - len / 2048f);
                var offset = sprite.size * sprite.origin;
                var texSize = sprite.material.textureSize;

                graphics.batch.Draw(sprite.material.texture.texture,
                    position: p, scale: size, depth: transform.DerivedDepth,
                    rotation: transform.DerivedOrientation + sprite.rotation, color: sprite.color, origin: sprite.size * sprite.origin);



                //arg2.texture(sprite.material.texture);
                ////arg2.position(p);
                //arg2.source(AxisAlignedBox.Null);
                //arg2.color(sprite.color);
                //arg2.rotation(transform.DerivedOrientation + sprite.rotation);
                //arg2.depth(transform.DerivedDepth);

                //arg2.quad(p - offset, p + sprite.size - offset, sprite.origin, transform.DerivedOrientation + sprite.rotation);
                //////arg2.quad(p, 
                //graphics.Draw(0,
                //              sprite.material,
                //              p,
                //              AxisAlignedBox.Null,
                //              sprite.color,
                //              transform.DerivedOrientation + sprite.rotation,
                //              sprite.origin,
                //              size,
                //              sprite.spriteEffect,
                //              transform.DerivedDepth);

            }
            world.endOpaque();
        }

        public void renderAlphaBlend()
        {

        }

        public void renderOverlay()
        {

        }
    }
}
