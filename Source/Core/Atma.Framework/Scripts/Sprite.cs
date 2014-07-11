//using Atma.Engine;
//using Atma.Graphics;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace Atma
//{
//    public class Sprite : Script
//    {
//        public int renderQueue = 0;
//        public Material material;

//        private Transform _transform;
//        public Vector2 origin = Vector2.One / 2;
//        //public Vector2 size = Vector2.Zero;
//        public Color color = Color.White;
//        public Vector2 offset = Vector2.Zero;
//        public float rotation = MathHelper.PiOver2;
//        public SpriteEffects spriteEffect = SpriteEffects.None;
//        //public string textureName;

//        private int? _width;
//        private int? _height;

//        public Vector2 size
//        {
//            get { return new Vector2(width, height); }
//            set
//            {
//                width = (int)value.X;
//                height = (int)value.Y;
//            }
//        }

//        public int width
//        {
//            get
//            {
//                if (_width.HasValue)
//                    return _width.Value;
//                else
//                    return material.textureWidth;
//            }
//            set
//            {
//                _width = value;
//            }
//        }

//        public int height
//        {
//            get
//            {
//                if (_height.HasValue)
//                    return _height.Value;
//                else
//                    return material.textureHeight;
//            }
//            set
//            {
//                _height = value;
//            }
//        }



//        //BmFont font = null;
//        private void init()
//        {
//            _transform = this.gameObject.transform2();
//        }

//        private void render()
//        {
//            if (material != null)
//            {
//                var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
//                graphics.Draw(renderQueue,
//                                material,
//                                _transform.DerivedPosition + offset,
//                                AxisAlignedBox.Null,
//                                color,
//                                _transform.DerivedOrientation + rotation,
//                                origin,
//                                size * _transform.DerivedScale,
//                                spriteEffect,
//                                _transform.DerivedDepth);
//            }

//            //graphics.DrawLine(
//            //var end = ProjectPointFromCenterRotation(100);
//            //graphics.DrawLine(renderQuere, material, _transform.DerivedPosition, end, color);
//        }

//        private void debugrender()
//        {
//            //graphics.DrawLine
//        }

//        //public Vector2 ProjectPointFromCenterRotation(float length)
//        //{
//        //    var r = _transform.DerivedOrientation % MathHelper.TwoPi;
//        //    //r = 0;
//        //    var x = (float)Math.Cos(MathHelper.PiOver2 - r);
//        //    var y = -(float)Math.Sin(MathHelper.PiOver2 - r);
//        //    return (new Vector2(x, y) * length) + _transform.DerivedPosition;
//        //}
//    }
//}
