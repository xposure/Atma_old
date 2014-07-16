using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Entities;
using Atma.Graphics;
using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

namespace Atma.Rendering.Sprites
{
    public class SpriteComponent : Component
    {
        public Material material;

        private Transform _transform;
        public Vector2 origin = Vector2.One / 2;
        //public Vector2 size = Vector2.Zero;
        public Color color = Color.White;
        public Vector2 offset = Vector2.Zero;
        public float rotation = MathHelper.PiOver2;
        //public SpriteEffects spriteEffect = SpriteEffects.None;
        //public string textureName;

        private int? _width;
        private int? _height;

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


    }
}
