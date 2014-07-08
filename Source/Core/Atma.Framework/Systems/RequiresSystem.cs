using System;
using System.Collections.Generic;

namespace Atma.Systems
{
    public abstract class RequiresSystem : ISystem
    {
        private List<Action<IEntity>> _addListener = new List<Action<IEntity>>();
        private List<Action<IEntity>> _removeListener = new List<Action<IEntity>>();
        private List<Action> _callbacks = new List<Action>();
        private Dictionary<IEntity, Action> _callbackMap = new Dictionary<IEntity, Action>();

        protected void require<T0>(Action<T0> callback)
            where T0 : IEntity
        {
            var add = new Action<IEntity>(e =>
            {
                if (e is T0)
                {
                    var _t0 = (T0)e;
                    _callbacks.Add(new Action(() => { callback(_t0); }));
                }
            });

            var remove = new Action<IEntity>(e =>
            {
                if (e is T0)
                {
                    var action = _callbackMap[e];
                    _callbacks.Remove(action);
                    _callbackMap.Remove(e);
                }
            });

            Root.instance.add += add;
            Root.instance.remove += remove;

            _addListener.Add(add);
            _removeListener.Add(remove);
        }

        protected void require<T0, T1>(Action<T0, T1> callback)
            where T0 : IEntity
            where T1 : IEntity
        {
            var add = new Action<IEntity>(e =>
            {
                if (e is T0 && e is T1)
                {
                    var _t0 = (T0)e;
                    var _t1 = (T1)e;
                    _callbacks.Add(new Action(() => { callback(_t0, _t1); }));
                }
            });

            var remove = new Action<IEntity>(e =>
            {
                if (e is T0 && e is T1)
                {
                    var action = _callbackMap[e];
                    _callbacks.Remove(action);
                    _callbackMap.Remove(e);
                }
            });

            Root.instance.add += add;
            Root.instance.remove += remove;

            _addListener.Add(add);
            _removeListener.Add(remove);
        }

        public void init() { oninit(); }

        public void update()
        {
            for (var i = 0; i < _callbacks.Count; i++)
                _callbacks[i]();

            onupdate();
        }

        public void render() { onrender(); }

        public void destroy()
        {
            ondestroy();

            foreach (var listener in _addListener)
                Root.instance.add -= listener;

            foreach (var listener in _removeListener)
                Root.instance.remove -= listener;

            _addListener.Clear();
            _removeListener.Clear();
        }

        protected virtual void oninit() { }
        protected virtual void onupdate() { }
        protected virtual void onrender() { }
        protected virtual void ondestroy() { }

    }
}
