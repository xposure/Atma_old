//using Atma.Engine;
//using Atma.Graphics;
//using Microsoft.Xna.Framework;

//namespace Atma
//{
//    public class ParallaxBackground : Script
//    {
//        public int renderQueue = 0;
//        public Material material;
//        public float speed = 1f;
//        public Color color = Color.White;
//        //public string textureName;

//        private int? _width;
//        private int? _height;

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


//        private Vector2 offset;

//        private Transform _cameraTransform;
//        private Transform _transform;

//        private void init()
//        {
//            //var texture = Root.instance.resources.findTexture(textureName);
//            //if (texture != null)
//            //{
//            //    width = texture.width;
//            //    height = texture.height;
//            //}
//            _cameraTransform = Camera.mainCamera.gameObject.transform2();
//            _transform = gameObject.transform2();
//        }

//        private void update()
//        {
//            if (_cameraTransform != null && _transform != null)
//            {
//                var p = (transform.DerivedPosition + _cameraTransform.DerivedPosition) * speed;
//                offset = new Vector2(p.X % (width / _transform.DerivedScale.X),
//                                        p.Y % (height / _transform.DerivedScale.Y));

//            }
//        }

//        private void render()
//        {
//            if (_cameraTransform != null && _transform != null)
//            {
//                var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
//                var dstRect = Camera.mainCamera.worldBounds;
//                dstRect.Inflate(5, 5);

//                var p = new Vector2(offset.X * _transform.DerivedScale.X, offset.Y * _transform.DerivedScale.Y);
//                var s = new Vector2(dstRect.Width * _transform.DerivedScale.X, dstRect.Height * _transform.DerivedScale.Y);

//                var srcRect = p.ToAABB(s);

//                graphics.Draw(renderQueue, material, dstRect, srcRect, color);
//            }
//        }
//    }
//}
