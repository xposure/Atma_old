using Atma.Core;
using Atma.Engine;
using System.Collections.Generic;
using System.Linq;

namespace Atma.Systems
{

    public interface IComponentSystemManager 
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
        private List<IRenderSubscriber> _renderSubscribers = new List<IRenderSubscriber>();
        private bool _initialised = false;

        public GameUri uri { get { return Uri; } }

        public T register<T>(GameUri uri, T system)
            where T : IComponentSystem
        {
            if (system is IUpdateSubscriber)
                _updateSubscribers.Add((IUpdateSubscriber)system);

            if (system is IRenderSubscriber)
                _renderSubscribers.Add((IRenderSubscriber)system);

            _systems.Add(uri, system);

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

                if (system is IRenderSubscriber)
                    _renderSubscribers.Remove((IRenderSubscriber)system);

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

        public void render()
        {
            foreach (var system in _renderSubscribers)
                system.render();
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
}
