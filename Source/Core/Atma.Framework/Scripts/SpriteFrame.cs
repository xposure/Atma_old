using Atma.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Atma
{
    public class SpriteFrame : Script
    {
        private struct SpriteFrameData
        {
            public Vector2 point;
            public Vector2 size;
        }

        public int renderQuere = 0;
        public Material material;

        //private Transform _transform;
        public Vector2 origin = Vector2.One / 2;
        //public Vector2 size = Vector2.Zero;
        public Color color = Color.White;
        public Vector2 offset = Vector2.Zero;
        public float rotation = 0f;
        public SpriteEffects spriteEffect = SpriteEffects.None;
        //public string textureName;
        public int frame = 0;

        private List<SpriteFrameData> frames = new List<SpriteFrameData>();

        private int? _width = 0;
        private int? _height = 0;

        public Vector2 size
        {
            get { return new Vector2(width, height); }
            set
            {
                width = (int)value.X;
                height = (int)value.Y;
            }
        }

        public int width
        {
            get
            {
                if (_width.HasValue)
                    return _width.Value;
                else
                    return material.textureWidth;
            }
            set
            {
                _width = value;
            }
        }

        public int height
        {
            get
            {
                if (_height.HasValue)
                    return _height.Value;
                else
                    return material.textureHeight;
            }
            set
            {
                _height = value;
            }
        }


        public int addFrame(Vector2 normalizedPoint, Vector2 size )
        {
            frames.Add(new SpriteFrameData() { point = normalizedPoint, size = size });
            return frames.Count - 1;
        }

        public void addGrid(int numWide, int numTall)
        {
            var w = 1f / numWide;
            var h = 1f / numTall;
            var size = new Vector2(w, h);
            for (var y = 0; y < numTall; y++)
            {
                var ny0 = y * h;
                for (var x = 0; x < numWide; x++)
                {
                    var nx0 = x * w;
                    addFrame(new Vector2(nx0, ny0), size);
                }
            }
        }

        //private void init()
        //{
        //    _transform = this.gameObject.transform2();
        //}

        private void render()
        {
            if (frame >= frames.Count || frame < 0)
                return;

            var src = AxisAlignedBox.FromRect(frames[frame].point * size, frames[frame].size);

            Root.instance.graphics.Draw(renderQuere,                            
                            material,
                            gameObject.transform.DerivedPosition + offset,
                            src,
                            color,
                            gameObject.transform.DerivedOrientation + rotation,
                            Vector2.Zero,//origin,
                            size * gameObject.transform.DerivedScale,
                            spriteEffect,
                            gameObject.transform.DerivedDepth);
            //var end = ProjectPointFromCenterRotation(100);
            //Root.instance.graphics.DrawLine(renderQuere, material, _transform.DerivedPosition, end, color);
        }

    }
}
