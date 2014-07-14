//using Atma.Collections;
//using Atma.Engine;
//using Microsoft.Xna.Framework;

//namespace Atma.Scripts
//{
//    #region Collider
//    public abstract class Collider : Script, IQuadObject
//    {
//        protected AxisAlignedBox _bounds = AxisAlignedBox.Null;
//        public AxisAlignedBox Bounds
//        {
//            get
//            {
//                if (_needsUpdate)
//                    updateBounds();
//                return _bounds;
//            }
//        }

//        protected Transform _transform;
//        protected internal Shape _shape;

//        public bool hasRigidbody { get; internal set; }

//        private bool _needsUpdate = false;

//        private void init()
//        {
//            _transform = gameObject.transform2();
//            _needsUpdate = true;
//            Root.instance.physicsQuadTree.Insert(this);
//        }

//        protected internal void updateBounds()
//        {
//            var p = _transform.DerivedPosition;
//            var hs = getHalfSize();
//            _bounds.SetExtents(p - hs, p + hs);
//            _needsUpdate = false;
//        }

//        protected internal void needsUpdate()
//        {
//            if (_needsUpdate)
//                return;

//            _needsUpdate = true;
//            resumeEvent("fixedupdate");
//        }

//        protected internal void forceUpdate()
//        {
//            _needsUpdate = true;
//            Root.instance.physicsQuadTree.UpdateBounds(this);
//        }

//        private void fixedupdate()
//        {
//            _needsUpdate = true;
//            Root.instance.physicsQuadTree.UpdateBounds(this);
//            suspendEvent("fixedupdate");
//        }

//        private void destroy()
//        {
//            Root.instance.physicsQuadTree.Remove(this);
//        }

//        protected abstract Vector2 getHalfSize();

//        protected internal bool collide(Collider other, Vector2 normalResponse)
//        {


//            return false;
//        }
//    }

//    public abstract class Collider2 : Script, IQuadObject
//    {
//        protected AxisAlignedBox _bounds = AxisAlignedBox.Null;
//        protected Transform _transform;
//        protected internal abstract ICollidable shape { get; }
//        public bool isTrigger = false;
//        public AxisAlignedBox Bounds { get { return shape.bounds; } }

//        private void transformupdate()
//        {
//            var matrix = _transform.DerivedMatrix;
//            shape.update(matrix);
//            Root.instance.physicsQuadTree2.UpdateBounds(this);
//        }

//        protected void forceUpdate()
//        {
//            transformupdate();
//        }

//        public bool hasRigidbody { get; internal set; }

//        private void init()
//        {
//            _transform = gameObject.transform2();
//            onInit();
//            var matrix = _transform.DerivedMatrix;
//            shape.update(matrix);
//            Root.instance.physicsQuadTree2.Insert(this);
//        }

//        protected virtual void onInit()
//        {

//        }

//        private void destroy()
//        {
//            Root.instance.physicsQuadTree2.Remove(this);
//        }

//        protected internal MinimumTranslationVector intersects(Collider2 other)
//        {
//            var shape1 = this.shape;
//            var shape2 = other.shape;

//            return shape1.intersects(shape2);
//        }

//        private void render()
//        {
//            var color = hasRigidbody ? Color.Purple : Color.Wheat;
//            shape.render(color);
//        }
//    }

//    #endregion

//    #region BoxCollider
//    public class BoxCollider : Collider
//    {
//        private float _width = 1f;
//        private float _height = 1f;

//        public float width
//        {
//            get { return _width; }
//            set
//            {
//                _width = value;
//                needsUpdate();
//            }
//        }

//        public float height
//        {
//            get { return _height; }
//            set
//            {
//                _height = value;
//                needsUpdate();
//            }
//        }

//        protected override Vector2 getHalfSize()
//        {
//            return new Vector2(_width, _height) / 2f;
//        }

//        private void render()
//        {
//            var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
//            graphics.DrawRect(15, null, Bounds, Color.Purple);
//        }
//    }

//    public class BoxCollider2 : Collider2
//    {
//        private bool inited = false;
//        private Vector2 _size = Vector2.One;
//        private Vector2 _offset = Vector2.Zero;
//        private Shape _shape;
//        public Vector2 size { get { return _size; } }

//        protected internal override ICollidable shape
//        {
//            get { return _shape; }
//        }

//        public float width
//        {
//            get { return _size.X; }
//            set
//            {
//                _size.X = value;
//                updateShape();
//            }
//        }

//        public float height
//        {
//            get { return _size.Y; }
//            set
//            {
//                _size.Y = value;
//                updateShape();
//            }
//        }

//        public Vector2 offset { get { return _offset; } }

//        public float offsetX
//        {
//            get { return _offset.X; }
//            set
//            {
//                _offset.X = value;
//                updateShape();
//            }
//        }

//        public float offsetY
//        {
//            get { return _offset.Y; }
//            set
//            {
//                _offset.Y = value;
//                updateShape();
//            }
//        }

//        public void setSize(float width, float height)
//        {
//            setSize(new Vector2(width, height));
//        }

//        public void setSize(Vector2 size)
//        {
//            setDeminensions(_offset, size);
//        }

//        public void setOffset(float offsetx, float offsety)
//        {
//            setOffset(new Vector2(offsetx, offsety));
//        }

//        public void setOffset(Vector2 offset)
//        {
//            setDeminensions(offset, _size);
//        }

//        public void setDeminensions(Vector2 offset, Vector2 size)
//        {
//            _offset = offset;
//            _size = size;
//            updateShape();
//        }

//        protected void updateShape()
//        {
//            if (!inited)
//                return;

//            var hs = _size / 2f;

//            _shape.vertices[0].X = offset.X - hs.X;
//            _shape.vertices[0].Y = offset.Y - hs.Y;
//            _shape.vertices[1].X = offset.X - hs.X;
//            _shape.vertices[1].Y = offset.Y + hs.Y;
//            _shape.vertices[2].X = offset.X + hs.X;
//            _shape.vertices[2].Y = offset.Y + hs.Y;
//            _shape.vertices[3].X = offset.X + hs.X;
//            _shape.vertices[3].Y = offset.Y - hs.Y;
//        }

//        protected override void onInit()
//        {
//            inited = true;
//            _shape = new Shape(AxisAlignedBox.FromDimensions(_offset, _size));
//        }
//    }
//    #endregion

//    #region CompoundCollider

//    #endregion
//}
