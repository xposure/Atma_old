using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Entities;
using Atma.Graphics;
using Atma.Systems;

namespace Atma.Rendering.Sprites
{
    public class SpriteSystem : IComponentSystem, IRenderSystem
    {
        public static readonly GameUri Uri = "componentsystem:sprite";

        public void init()
        {
            var world = this.world();
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

                var size = sprite.size;
                var p = transform.DerivedPosition + sprite.offset;
                size /= sprite.material.textureSize;
                size *= transform.DerivedScale;

                var offset = sprite.size * sprite.origin;
                var texSize = sprite.material.textureSize;

                graphics.batch.Draw(sprite.material.texture,
                    position: p, scale: size, depth: transform.DerivedDepth,
                    rotation: transform.DerivedOrientation + sprite.rotation, color: sprite.color, origin: sprite.size * sprite.origin);

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
