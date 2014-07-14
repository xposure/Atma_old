using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
}
