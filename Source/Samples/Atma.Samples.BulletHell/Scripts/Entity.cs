using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Atma;

namespace GameName1.BulletHell.Scripts
{
    public class Entity : Script, IComparable<Entity>
    {
        //protected Shape _shape;
        //protected Color _color = Color.White;
        //protected Material _material;

        public int collisionIndex = 0;
        public float radius = 20f;
        //public bool isExpired = false;
        public Vector2 size = new Vector2(24, 24);

        public int minX { get { return (int)gameObject.transform.DerivedPosition.X - ((int)size.X >> 1); } }
        public int maxX { get { return (int)gameObject.transform.DerivedPosition.X + ((int)size.X >> 1); } }

        private void init()
        {
            //_shape = new Shape(new Vector2[] {
            //    new Vector2(0, 0),
            //    new Vector2(0, 17),
            //    new Vector2(6, 23),
            //    new Vector2(8, 23),
            //    new Vector2(10, 21),
            //    new Vector2(14, 21),
            //    new Vector2(16, 23),
            //    new Vector2(18, 23),
            //    new Vector2(23, 18),
            //    new Vector2(23, 0),
            //    new Vector2(17, 11),
            //    new Vector2(16, 11),
            //    new Vector2(12, 7),
            //    new Vector2(11, 7),
            //    new Vector2(7, 11),
            //    new Vector2(6, 11)
            //});
            //_material = resources.createMaterialFromTexture("
        }


        private void render()
        {
            //graphics.Draw(_material, transform.DerivedPosition, AxisAlignedBox.Null, _color, transform.DerivedOrientation + MathHelper.PiOver2, Vector2.One / 2f, size, SpriteEffects.None, 0f);
            //graphics.DrawShape(null, _shape, _color);
        }

        public int CompareTo(Entity other)
        {
            return minX - other.minX;
        }
    }
}
