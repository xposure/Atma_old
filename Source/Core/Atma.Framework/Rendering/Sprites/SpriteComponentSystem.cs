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
    public class SpriteComponentSystem : GameSystem, IComponentSystem, IRenderSystem
    {
        public static readonly GameUri Uri = "componentsystem:sprite";

        private Texture2D _defaultTexture;
        private SpriteBatch2 batch;

        public void init()
        {
            //var world = this.world();
            _defaultTexture = assets.getTexture("engine:default");

            batch = new SpriteBatch2(display.device);
        }

        public void shutdown()
        {
        }

        public void renderOpaque()
        {
            batch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Texture);
            foreach (var id in entities.getWithComponents("transform", "sprite"))
            {
                var sprite = entities.getComponent<Sprite>(id, "sprite");
                if (!sprite.material.isTransparent)
                {
                    var transform = entities.getComponent<Transform>(id, "transform");

                    //var size = sprite.size;
                    //var p = transform.DerivedPosition + sprite.offset;

                    //var offset = sprite.size * sprite.origin;
                    //var texSize = sprite.material.textureSize;

                    batch.draw(sprite.material.texture,
                        position: transform.DerivedPosition + sprite.offset, size: sprite.size, depth: transform.DerivedDepth,
                        rotation: transform.DerivedOrientation + sprite.rotation, color: sprite.color, origin: sprite.origin);
                }

            }
            batch.End();
        }

        public void renderShadows()
        {
        }
        public void renderAlphaBlend()
        {
            batch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Texture);
            foreach (var id in entities.getWithComponents("transform", "sprite"))
            {
                var sprite = entities.getComponent<Sprite>(id, "sprite");
                if (sprite.material.isTransparent)
                {
                    var transform = entities.getComponent<Transform>(id, "transform");

                    //var size = sprite.size;
                    //var p = transform.DerivedPosition + sprite.offset;

                    //var offset = sprite.size * sprite.origin;
                    //var texSize = sprite.material.textureSize;

                    batch.draw(sprite.material.texture,
                        position: transform.DerivedPosition + sprite.offset, size: sprite.size, depth: transform.DerivedDepth,
                        rotation: transform.DerivedOrientation + sprite.rotation, color: sprite.color, origin: sprite.origin);
                }
            }
            batch.End();
        }

        public void renderOverlay()
        {

        }

    }

}
