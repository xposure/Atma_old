using Atma.Core;
using Atma.Engine;
using System.Collections.Generic;
using System.Linq;

namespace Atma
{

    public interface IComponentSystemManager : ICore
    {
        void init();
        T register<T>(GameUri uri, T system)
                    where T : IComponentSystem;

        void unregister(GameUri uri);
        void shutdown();
    }

    public class ComponentSystemManager : IComponentSystemManager
    {
        public static readonly GameUri Uri = "subsystem:component";
        private static readonly Logger logger = Logger.getLogger(typeof(ComponentSystemManager));

        private Dictionary<GameUri, IComponentSystem> _systems = new Dictionary<GameUri, IComponentSystem>();
        private List<IUpdateSubscriber> _updateSubscribers = new List<IUpdateSubscriber>();
        //private List<IRenderSubscriber> _renderSubscribers = new List<IRenderSubscriber>();
        private List<IInputSubscriber> _inputSubscribers = new List<IInputSubscriber>();
        private bool _initialised = false;

        public GameUri uri { get { return Uri; } }

        public T register<T>(GameUri uri, T system)
            where T : IComponentSystem
        {
            if (system is IUpdateSubscriber)
                _updateSubscribers.Add((IUpdateSubscriber)system);

            //if (system is IRenderSubscriber)
            //    _renderSubscribers.Add((IRenderSubscriber)system);

            if (system is IInputSubscriber)
                _inputSubscribers.Add((IInputSubscriber)system);

            _systems.Add(uri, system);
            
            CoreRegistry.put(uri, system);

            if (_initialised)
                system.init();

            return system;
        }

        public void unregister(GameUri uri)
        {
            IComponentSystem system;
            if (_systems.TryGetValue(uri, out system))
            {
                if (system is IUpdateSubscriber)
                    _updateSubscribers.Remove((IUpdateSubscriber)system);

                //if (system is IRenderSubscriber)
                //    _renderSubscribers.Remove((IRenderSubscriber)system);

                if (system is IInputSubscriber )
                    _inputSubscribers.Remove((IInputSubscriber)system);

                CoreRegistry.remove(uri);

                _systems.Remove(uri);
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

        public IEnumerable<T> getSystemsByInterface<T>()
        {
            foreach (var s in _systems.Values)
                if (s is T)
                    yield return (T)s;
        }

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
            var uris = _systems.Keys.ToArray();
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

    public static class ComponentSystemManagerExtension
    {
        public static ComponentSystemManager components(this ICore e)
        {
            return CoreRegistry.require<ComponentSystemManager>(ComponentSystemManager.Uri);
        }
    }
}
