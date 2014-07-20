using Atma.Core;
using Atma.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atma
{

    public interface IComponentSystemManager 
    {
        void init();
        T register<T>(T system)
                    where T : IComponentSystem;

        void unregister<T>(T system)
            where T : class, IComponentSystem;
        void shutdown();
    }

    public class ComponentSystemManager : IComponentSystemManager
    {
        public static readonly GameUri Uri = "subsystem:component";
        private static readonly Logger logger = Logger.getLogger(typeof(ComponentSystemManager));

        private Dictionary<Type, IComponentSystem> _systems = new Dictionary<Type, IComponentSystem>();
        private List<IUpdateSubscriber> _updateSubscribers = new List<IUpdateSubscriber>();
        //private List<IRenderSubscriber> _renderSubscribers = new List<IRenderSubscriber>();
        private List<IInputSubscriber> _inputSubscribers = new List<IInputSubscriber>();
        private bool _initialised = false;

        public GameUri uri { get { return Uri; } }

        public T register<T>(T system)
            where T : IComponentSystem
        {
            if (system is IUpdateSubscriber)
                _updateSubscribers.Add((IUpdateSubscriber)system);

            //if (system is IRenderSubscriber)
            //    _renderSubscribers.Add((IRenderSubscriber)system);

            if (system is IInputSubscriber)
                _inputSubscribers.Add((IInputSubscriber)system);

            _systems.Add(typeof(T), system);
            
            CoreRegistry.put(system);

            if (_initialised)
                system.init();

            return system;
        }

        public void unregister<T>(T t)
            where T: class,IComponentSystem
        {
            IComponentSystem system;
            if (_systems.TryGetValue(typeof(T), out system))
            {
                if (system is IUpdateSubscriber)
                    _updateSubscribers.Remove((IUpdateSubscriber)system);

                //if (system is IRenderSubscriber)
                //    _renderSubscribers.Remove((IRenderSubscriber)system);

                if (system is IInputSubscriber )
                    _inputSubscribers.Remove((IInputSubscriber)system);

                CoreRegistry.remove(t);

                _systems.Remove(typeof(T));
                system.shutdown();
            }
        }

        public void init()
        {
            if (_initialised)
                return;

            logger.info("initialise");
            _initialised = true;

            foreach (var system in _systems.Values)
                system.init();
        }

        public void update(float delta)
        {
            foreach (var system in _updateSubscribers)
                system.update(delta);
        }

        //public IEnumerable<T> getSystemsByInterface<T>()
        //{
        //    foreach (var s in _systems.Values)
        //        if (s is T)
        //            yield return (T)s;
        //}

        //public void render()
        //{
        //    foreach (var system in _renderSubscribers)
        //        system.render();
        //}

        public bool input()
        {
            foreach (var system in _inputSubscribers)
                if (system.input())
                    return true;

            return false;
        }

        public void clear()
        {
            var uris = _systems.Values.ToArray();
            foreach (var uri in uris)
                unregister(uri);

            _initialised = false;
        }

        public void shutdown()
        {
            logger.info("shutdown");
            clear();
        }

    }


}
