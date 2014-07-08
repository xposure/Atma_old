using Atma.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Atma
{
    public class SpriteSheet
    {
        private struct SpriteSheetFrame
        {
            public AxisAlignedBox rect;

        }

        public Material material;

        private List<SpriteSheetFrame> frames = new List<SpriteSheetFrame>();

        //public void Draw(int renderQueue, int sprite, Vector2 position, Color color)
        //{
        //    Draw(renderQueue, sprite, rectangle, color, SpriteEffects.None, 0f);
        //}

        public int add(AxisAlignedBox rect)
        {
            var frame = new SpriteSheetFrame();
            frame.rect = rect;
            frames.Add(frame);
            
            return frames.Count - 1;
        }

        public void draw(int sprite, AxisAlignedBox rectangle, Color color)
        {
            draw(sprite, rectangle, color, SpriteEffects.None, 0f);
        }

        public void draw(int sprite, AxisAlignedBox rectangle, Color color, float depth)
        {
            draw(sprite, rectangle, color, SpriteEffects.None, depth);
        }

        public void draw(int sprite, AxisAlignedBox rectangle, Color color, SpriteEffects effects, float depth)
        {
            if (sprite < 0 || sprite >= frames.Count)
                throw new IndexOutOfRangeException("sprite");

            Root.instance.graphics.Draw(material, rectangle, frames[sprite].rect, color, 0f, Vector2.Zero, effects, depth);
        }


        public void draw(int renderQueue, int sprite, AxisAlignedBox rectangle, Color color)
        {
            draw(renderQueue, sprite, rectangle, color, SpriteEffects.None, 0f);
        }

        public void draw(int renderQueue, int sprite, AxisAlignedBox rectangle, Color color, float depth)
        {
            draw(renderQueue, sprite, rectangle, color, SpriteEffects.None, depth);
        }

        public void draw(int renderQueue, int sprite, AxisAlignedBox rectangle, Color color, SpriteEffects effects, float depth)
        {
            if (sprite < 0 || sprite >= frames.Count)
                throw new IndexOutOfRangeException("sprite");

            Root.instance.graphics.Draw(renderQueue, material, rectangle, frames[sprite].rect, color, 0f, Vector2.Zero, effects, depth);
        }
    }
}
