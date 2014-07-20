using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Assets;
using Atma.Core;
using Atma.Engine;
using Atma.Entities;
using Atma.Graphics;
using Atma.Input;

namespace Atma
{
    public class GameSystem
    {
        private GraphicSubsystem _graphics;
        protected GraphicSubsystem graphics
        {
            get
            {
                if (_graphics == null)
                    _graphics = CoreRegistry.require<GraphicSubsystem>();

                return _graphics;
            }
        }

        private DisplayDevice _display;
        protected DisplayDevice display
        {
            get
            {
                if (_display == null)
                    _display = CoreRegistry.require<DisplayDevice>();

                return _display;
            }
        }

        private AssetManager _assets;
        protected AssetManager assets
        {
            get
            {
                if (_assets == null)
                    _assets = CoreRegistry.require<AssetManager>();

                return _assets;
            }
        }

        private TimeBase _time;
        protected TimeBase time
        {
            get
            {
                if (_time == null)
                    _time = CoreRegistry.require<TimeBase>();

                return _time;
            }
        }

        private Dictionary<Type, Logger> _loggers = new Dictionary<Type, Logger>();
        private Logger _logger;
        protected Logger logger
        {
            get
            {
                if (_logger == null)
                {
                    lock (_loggers)
                    {
                        var type = this.GetType();
                        _logger = _loggers.get(type);
                        if (_logger == null)
                        {
                            _logger = Logger.getLogger(type);
                            _loggers.Add(type, _logger);
                        }
                    }
                }

                return _logger;
            }
        }

        private GameEngine _engine;
        protected GameEngine engine
        {
            get
            {
                if (_engine == null)
                    _engine = CoreRegistry.require<GameEngine>();

                return _engine;
            }
        }

        private EntityManager _entities;
        protected EntityManager entities
        {
            get
            {
                if (_entities == null)
                    _entities = CoreRegistry.require<EntityManager>();

                return _entities;
            }
        }

        private InputSystem _input;
        protected InputSystem input
        {
            get
            {
                if (_input == null)
                    _input = CoreRegistry.require<InputSystem>();

                return _input;
            }
        }

        private ComponentSystemManager _components;
        protected ComponentSystemManager components
        {
            get
            {
                if (_components == null)
                    _components = CoreRegistry.require<ComponentSystemManager>();

                return _components;
            }
        }
    }

    public class GameSystemInstance<T> : GameSystem
    {
        private static T _instance;

        public static T instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception(string.Format("{0} was not initialized yet", typeof(T)));

                return _instance;
            }
        }


        protected void setInstance(T t)
        {
            _instance = t;
        }
    }
}
