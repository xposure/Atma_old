using Atma.Assets;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Atma.Graphics
{
    public class DeferredQueue : IEnumerable<Renderable>
    {
        protected int _renderableIndex = 0;
        protected Renderable[] _renderableItems = new Renderable[1024];

        public void draw(Renderable item)
        {
            if (_renderableIndex == _renderableItems.Length)
                Array.Resize(ref _renderableItems, _renderableItems.Length * 3 / 2);

            _renderableItems[_renderableIndex++] = item;
        }

        public IEnumerator<Renderable> GetEnumerator()
        {
            for (var i = 0; i < _renderableIndex; i++)
                yield return _renderableItems[i];
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            for (var i = 0; i < _renderableIndex; i++)
                yield return _renderableItems[i];
        }

        public void reset()
        {
            _renderableIndex = 0;
        }

        public void sort(Func<Renderable, int> sorter)
        {
            Utility.RadixSort(_renderableItems, _renderableIndex, sorter);
        }
    }

    public class DeferredRenderQueue : AbstractRenderQueue
    {
        protected DeferredQueue _queue = new DeferredQueue();

        protected override void draw(Renderable item)
        {
            _queue.draw(item);
        }

        public override IEnumerable<Renderable> items
        {
            get
            {
                return _queue;
            }
        }

        public override void reset()
        {
            base.reset();
            _queue.reset();
        }
    }

    public class TextureRenderQueue : DeferredRenderQueue
    {
        private Dictionary<AssetUri, DeferredQueue> _renderableItems = new Dictionary<AssetUri, DeferredQueue>();

        public override void texture(Texture2D texture)
        {
            _queue = _renderableItems.getOrCreate(texture.uri);
            base.texture(texture);
        }

        public override IEnumerable<Renderable> items
        {
            get
            {
                foreach (var ritems in _renderableItems.Values)
                {
                    foreach (var item in ritems)
                        yield return item;
                }
            }
        }

        public override void reset()
        {
            base.reset();
            foreach (var ritems in _renderableItems.Values)
                _renderableItems.Clear();
        }
    }

    public class SortingRenderQueue : DeferredRenderQueue
    {
        private Func<Renderable, int> _sorter;

        public SortingRenderQueue(Func<Renderable, int> sorter)
        {
            _sorter = sorter;
        }

        public override IEnumerable<Renderable> items
        {
            get
            {
                _queue.sort(_sorter);
                return base.items;
            }
        }
    }

    public class DepthRenderQueue : SortingRenderQueue
    {
        public DepthRenderQueue()
            : base(new Func<Renderable, int>(x => { return (int)x.depth; }))
        {

        }
    }

    public abstract class AbstractRenderQueue
    {



        protected Stack<GraphicsState> _stateStack = new Stack<GraphicsState>();

        protected GraphicsState _state;

        public AbstractRenderQueue()
        {
            reset();
        }

        public float currentRotation { get { return _state.rotation; } }
        public Vector2 currentPosition { get { return _state.position; } }

        protected abstract void draw(Renderable item);

        public void quad(Vector2 size) { quad(-size / 2f, size / 2f, new Vector2(0.5f, 0.5f), _state.rotation); }
        public void quad(AxisAlignedBox a) { quad(a.minVector, a.maxVector, new Vector2(0.5f, 0.5f), _state.rotation); }
        public void quad(Vector2 size, float rotation) { quad(-size / 2f, size / 2f, new Vector2(0.5f, 0.5f), rotation); }
        public void quad(Vector2 min, Vector2 max, float rotation) { quad(min, max, new Vector2(0.5f, 0.5f), rotation); }
        public void quad(AxisAlignedBox a, float rotation) { quad(a.minVector, a.maxVector, new Vector2(0.5f, 0.5f), rotation); }
        public void quad(AxisAlignedBox a, Vector2 pivot, float rotation) { quad(a.minVector, a.maxVector, pivot, rotation); }
        public void quad(Vector2 min, Vector2 max) { quad(min, max, new Vector2(0.5f, 0.5f), _state.rotation); }

        public void quad(Vector2 min, Vector2 max, Vector2 pivot, float rotation)
        {
            var item = new Renderable();
            item.color = _state.color;
            item.depth = _state.depth;
            item.texture = _state.texture;
            item.pivot = pivot;
            item.scale = (max - min) * _state.scale;
            item.position = (_state.position + min) * _state.scale;
            item.rotation = rotation;
            item.sourceRectangle = _state.source;

            draw(item);
        }

        public void line(Vector2 p0) { line(_state.position, p0, 1f, _state.rotation); }
        public void line(Vector2 p0, float width) { line(_state.position, p0, width, _state.rotation); }
        public void line(Vector2 p0, Vector2 p1) { line(p0, p1, 1f, _state.rotation); }
        public void line(Vector2 p0, Vector2 p1, float width) { line(p0, p1, width, _state.rotation); }
        public void line(Vector2 start, Vector2 end, float width, float rotation)
        {
            if (rotation != 0)
            {
                start = start.Rotate(Vector2.Zero, rotation);
                end = end.Rotate(Vector2.Zero, rotation);
            }

            var diff = end - start;
            //var srcRect = new AxisAlignedBox(start.X, start.Y - width * 0.5f, start.X + diff.Length(), start.Y + width * 0.5f);

            var srcRect = new AxisAlignedBox(start.X, start.Y, start.X + width, start.Y + diff.Length());

            diff.Normalize();

            var _rotation = Utility.ATan2(diff.Y, diff.X) - Utility.PIOverTwo;
            //Draw(renderQueue, material, srcRect, AxisAlignedBox.Null, color, rotation, new Vector2(0.5f, 0), SpriteEffects.None, depth);

            var item = new Renderable();
            item.color = _state.color;
            item.depth = _state.depth;
            item.pivot = new Vector2(0.5f, 0);
            item.position = (_state.position + start) * _state.scale;
            item.rotation = _rotation;
            item.scale = srcRect.Size * _state.scale;
            item.sourceRectangle = _state.source;

            draw(item);
        }

        //public void rect(Vector2 size) { rect(Vector2.Zero, size, 1f, _state.rotation); }
        public void rect(AxisAlignedBox a) { rect(a.minVector, a.maxVector, 1f, _state.rotation); }
        //public void rect(Vector2 size, float width) { rect(Vector2.Zero, size, width, _state.rotation); }
        public void rect(AxisAlignedBox a, float width) { rect(a.minVector, a.maxVector, width, _state.rotation); }
        public void rect(Vector2 min, Vector2 max) { rect(min, max, 1f, _state.rotation); }
        public void rect(Vector2 min, Vector2 max, float width) { rect(min, max, width, _state.rotation); }
        public virtual void rect(Vector2 min, Vector2 max, float width, float rotation)
        {
            min = new Vector2((float)System.Math.Floor(min.X), (float)System.Math.Floor(min.Y));
            max = new Vector2((float)System.Math.Ceiling(max.X), (float)System.Math.Ceiling(max.Y));

            var points = new Vector2[] { min, new Vector2(max.X, min.Y), max, new Vector2(min.X, max.Y) };
            if (rotation != 0)
            {
                for (var i = 0; i < 4; i++)
                    points[i] = points[i].Rotate(Vector2.Zero, rotation);
            }

            for (var i = 0; i < 4; i++)
                line(points[i], points[(i + 1) % 4], width, 0f);
        }

        //transforms
        public void position(float x, float y) { position(new Vector2(x, y)); }
        public virtual void position(Vector2 point)
        {
            _state.position = point;
        }

        public virtual void rotation(float amount)
        {
            _state.rotation = Utility.WrapAngle(amount);
        }

        public virtual void rotate(float amount)
        {
            _state.rotation = Utility.WrapAngle(_state.rotation + amount);
            //_state.rotation = _state.rotation + amount;
        }

        public void translate(float x, float y) { translate(new Vector2(x, y)); }
        public virtual void translate(Vector2 p)
        {
            p = p.Rotate(Vector2.Zero, _state.rotation);
            _state.position += p;
        }

        public void scale(float amount) { scale(new Vector2(amount, amount)); }
        public virtual void scale(Vector2 amount)
        {
            _state.scale = amount;
        }

        public virtual void depth(float amount)
        {
            _state.depth = amount;
        }

        public virtual void push()
        {
            _stateStack.Push(_state);
        }
        //public void push(Matrix4 matrix) { }
        public virtual void pop()
        {

            if (_stateStack.Count == 0)
                throw new InvalidOperationException("State stack was empty.");

            _state = _stateStack.Pop();
        }

        //coloring
        public void color(float r, float g, float b) { color(new Color(r, g, b, 1f)); }
        public void color(float r, float g, float b, float a) { color(new Color(r, g, b, a)); }
        public void color(Vector3 _color) { color(new Color(_color.X, _color.Y, _color.Z, 1f)); }
        public void color(Vector4 _color) { color(new Color(_color.X, _color.Y, _color.Z, _color.W)); }
        public virtual void color(Color _color)
        {
            _state.color = _color;
        }

        public virtual void texture(Texture2D texture)
        {
            _state.texture = texture;
        }

        public void source(Vector2 min, Vector2 max) { source(new AxisAlignedBox(min, max)); }
        public virtual void source(AxisAlignedBox src)
        {
            _state.source = src;
        }

        public abstract IEnumerable<Renderable> items { get; }

        public virtual void reset()
        {
            _state = GraphicsState.empty;
        }
    }
}
