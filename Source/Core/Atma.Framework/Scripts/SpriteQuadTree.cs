//using Atma.Collections;
//using Atma.Engine;
//using Atma.Graphics;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace Atma
//{
//    public class SpriteQuadTreeItem : IQuadObject
//    {
//        public Material material;

//        public int renderQueue = 0;
//        public AxisAlignedBox src = AxisAlignedBox.Null;
//        public Vector2 origin = Vector2.One / 2;
//        public Vector2 position = Vector2.Zero;
//        public Vector2 size = Vector2.Zero;
//        public Vector2 scale = Vector2.One;
//        public Color color = Color.White;
//        public float rotation = MathHelper.PiOver2;
//        public SpriteEffects spriteEffect = SpriteEffects.None;        

//        public AxisAlignedBox Bounds
//        {
//            get
//            {
//                var hsOrigin = new Vector2(origin.X - 0.5f, origin.Y - 0.5f);
//                var min = size * scale * (hsOrigin - Vector2.One);
//                var max = size * scale * (hsOrigin + Vector2.One);

//                return new AxisAlignedBox(min + position, max + position);
//            }
//        }
//    }

//    public class SpriteQuadTree : Script
//    {
//        protected QuadTree<SpriteQuadTreeItem> _items = new QuadTree<SpriteQuadTreeItem>(new Vector2(128, 128), 32);

//        public void addSprite(SpriteQuadTreeItem item)
//        {
//            _items.Insert(item);
//        }

//        public void clear()
//        {
//            _items.Clear();
//        }

//        //private void render()
//        //{
//        //    onRender();
//        //}

//        //protected virtual void onRender()
//        //{
//        //    var bounds = getRenderBounds();
//        //    var result = _items.Query(bounds);
//        //    foreach (var r in result)
//        //        renderItem(r.material, r.renderQueue, r.position, r.src, r.color, r.rotation, r.origin, r.size, r.scale, r.spriteEffect);
//        //}

//        public virtual void renderItem(Material material, int renderQueue, Vector2 position, AxisAlignedBox src,
//            Color color, float rotation, Vector2 origin, Vector2 size, Vector2 scale, SpriteEffects spriteEffect)
//        {
//            var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
//            if (material != null)
//            {
//                graphics.Draw(renderQueue,
//                                material,
//                                position,
//                                src,
//                                color,
//                                rotation,
//                                origin,
//                                size * scale,
//                                spriteEffect, 0f);
//            }
//        }

//        //protected virtual AxisAlignedBox getRenderBounds()
//        //{
//        //    //return mainCamera.worldBounds;
//        //}
//    }
//}
