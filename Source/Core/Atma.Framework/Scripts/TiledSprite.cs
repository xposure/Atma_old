//using Atma.Engine;
//using Atma.Graphics;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace Atma
//{
//    public class TiledSprite : Script
//    {
//        public int renderQuere = 0;
//        public Material material;
//        //public string textureName;

//        public Color color = Color.White;
//        public Vector2 origin = Vector2.One / 2;
//        public Vector2 tiling = Vector2.One;
//        public Vector2 offset = Vector2.Zero;
//        //public Vector2 size = Vector2.One;

//        private Transform _transform;
//        //private Vector2 texSize = Vector2.One;

//        private int? _width = 0;
//        private int? _height = 0;

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


//        private void init()
//        {
//            //var texture = Root.instance.resources.findTexture(textureName);
//            //if (texture != null)
//            //{
//            //    texSize.X = texture.width;
//            //    texSize.Y = texture.height;
//            //}

//            _transform = this.gameObject.transform2();
//        }

//        private void update()
//        {
//            //offset.Y -= Root.instance.time.deltaTime;
//        }

//        private void render()
//        {
//            if (_transform != null && material.texture != null)
//            {
//                var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
//                var texSize = new Vector2(material.textureWidth, material.textureHeight);
//                var srcRect = (offset * texSize).ToAABB(size);

//                graphics.Draw(renderQuere, 
//                                material, 
//                                _transform.DerivedPosition, 
//                                srcRect, 
//                                color, 
//                                _transform.DerivedOrientation, 
//                                origin, 
//                                _transform.DerivedScale * size,
//                                SpriteEffects.None, 
//                                _transform.DerivedDepth);

//                //debugrender();
//            }
//        }

//        private void debugrender()
//        {
//            var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
//            var points = new Vector2[4];
//            points[0] = _transform.DerivedPosition;
//            points[2] = _transform.DerivedScale * size;
//            points[1] = new Vector2(points[0].X + points[2].X, points[0].Y);
//            points[3] = new Vector2(points[0].X , points[0].Y + points[2].Y);
//            points[2] += points[0];

//            for (var i = 0; i < points.Length; i++)
//                graphics.DrawLine(0,  null, points[i], points[(i + 1) % points.Length], Color.White);
//        }
//    }
//}
