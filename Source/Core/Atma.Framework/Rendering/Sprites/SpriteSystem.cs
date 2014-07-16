using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Assets;
using Atma.Engine;
using Atma.Entities;
using Atma.Graphics;
using Microsoft.Xna.Framework;

namespace Atma.Rendering.Sprites
{
    public class SpriteSystem : IComponentSystem, IRenderSystem
    {
        public static readonly GameUri Uri = "componentsystem:sprite";

        private AssetManager _assets;
        private DisplayDevice _display;
        private Texture2D _defaultTexture;
        private SpriteBatch2 batch;

        public void init()
        {
            //var world = this.world();
            _assets = this.assets();
            _display = this.display();
            _defaultTexture = _assets.getTexture("engine:default");

            batch = new SpriteBatch2(_display.device);
        }

        public void shutdown()
        {
        }

        public void renderOpaque()
        {
            var world = this.world();
            var graphics = this.graphics();

            var em = this.entities();

            batch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Texture);
            foreach (var id in em.getWithComponents("transform", "sprite"))
            {
                var transform = em.getComponent<Transform>(id, "transform");
                var sprite = em.getComponent<Sprite>(id, "sprite");

                //var size = sprite.size;
                //var p = transform.DerivedPosition + sprite.offset;

                //var offset = sprite.size * sprite.origin;
                //var texSize = sprite.material.textureSize;

                draw(sprite.material.texture,
                    position: transform.DerivedPosition + sprite.offset, size: sprite.size, depth: transform.DerivedDepth,
                    rotation: transform.DerivedOrientation + sprite.rotation, color: sprite.color, origin: sprite.origin);

            }
            batch.End();
        }

        public void renderAlphaBlend()
        {

        }

        public void renderOverlay()
        {

        }


        public void draw(Texture2D texture,
                    Vector2 position,
                    Vector2 size,
                    AxisAlignedBox? sourceRectangle = null,
                    Color? color = null,
                    float rotation = 0f,
                    Vector2? origin = null,
                    float depth = 0,
                    Microsoft.Xna.Framework.Graphics.SpriteEffects effect = Microsoft.Xna.Framework.Graphics.SpriteEffects.None
            )
        {
            texture = texture ?? _defaultTexture;

            if (texture != null)
            {
                var adjustedSize = size;
                if (!sourceRectangle.HasValue || sourceRectangle.Value.IsNull)
                {
                    adjustedSize /= texture.size;
                    batch.Draw(texture,
                        position: position, scale: adjustedSize, depth: depth,
                        rotation: rotation, color: color, origin: size * origin);
                }
                else
                {
                    adjustedSize /= sourceRectangle.Value.Size;
                    batch.Draw(texture,
                        position: position, scale: adjustedSize, depth: depth, sourceRectangle: sourceRectangle.Value.ToRect(),
                        rotation: rotation, color: color, origin: size * origin);
                }
            }
        }
    }

}
