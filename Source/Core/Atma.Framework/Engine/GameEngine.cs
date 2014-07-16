using Atma.Entities;
using Atma.Rendering;
using Atma.Core;
using Atma.Engine;
using Atma.Events;
using Atma.Managers;
using System;
using System.Collections.Generic;

namespace Atma.Engine
{
    //public class User32
    //{
    //    [DllImport("user32.dll")]
    //    public static extern void SetWindowPos(uint Hwnd, uint Level, int X, int Y, int W, int H, uint Flags);
    //}

    public class GameEngine : Microsoft.Xna.Framework.Game, IGameEngine
    {
        public static readonly GameUri Uri = "core:engine";
        private static readonly Logger logger = Logger.getLogger(typeof(GameEngine));

        public event Events.OnStateChangeEvent onStateChange;

        private Root root;
        private IGameTime time;

        private bool _initialised = false;
        private bool _isRunning = false;
        private bool _isDisposed = false;
        private bool _hasFocus = false;
        private bool _hasMouseFocus = false;

        private IGameState _state = null;
        private IGameState _pendingState = null;

        private List<ISubsystem> _subsystems = new List<ISubsystem>();

        public GameEngine(IGameState initialState, params ISubsystem[] subsystems)
            : base()
        {
            CoreRegistry.put("engine:game", this);

            _pendingState = initialState;
            if (subsystems.Length > 0)
                _subsystems.AddRange(subsystems);

            CoreRegistry.putPermanently(EntityManager.Uri, new EntityManager());
            CoreRegistry.putPermanently(Assets.AssetManager.Uri, new Assets.AssetManager());
            foreach (var s in _subsystems)
                CoreRegistry.putPermanently(s.uri, s);

            foreach (var s in _subsystems)
                s.preInit();

        }

        protected override void Initialize()
        {
            if (_initialised)
                return;

            CoreRegistry.putPermanently(Uri, this);

            root = new Root();
            base.Initialize();
            root.start(this.GraphicsDevice, this.Content);

            _initialised = true;

            logger.info("Initializing GameEngine...");
            logger.info("Version: 0.1 ALPHA");
            //logger.info("Home path: {}", PathManager.getInstance().getHomePath());
            //logger.info("Install path: {}", PathManager.getInstance().getInstallPath());
            //logger.info("Java: {} in {}", System.getProperty("java.version"), System.getProperty("java.home"));
            //logger.info("Java VM: {}, version: {}", System.getProperty("java.vm.name"), System.getProperty("java.vm.version"));
            //logger.info("OS: {}, arch: {}, version: {}", System.getProperty("os.name"), System.getProperty("os.arch"), System.getProperty("os.version"));
            //logger.info("Max. Memory: {} MB", Runtime.getRuntime().maxMemory() / (1024 * 1024));
            //logger.info("Processors: {}", Runtime.getRuntime().availableProcessors());

            foreach (var s in _subsystems)
                s.postInit();

            PerformanceMonitor.init();
        }

        protected override void BeginRun()
        {
            base.BeginRun();
        }

        protected override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            this.Window.Title = gameTime.ElapsedGameTime.TotalSeconds.ToString() + "    " + TargetElapsedTime.TotalSeconds.ToString();

            processStateChanges();

            var tick = (float)gameTime.ElapsedGameTime.TotalSeconds;
            foreach (var system in _subsystems)
                system.preUpdate(tick);

            root.update(gameTime.TotalGameTime.TotalSeconds);
            if (currentState != null)
                currentState.update(tick);

            foreach (var system in _subsystems)
                system.postUpdate(tick);

            //base.Update(gameTime);

        }

        protected override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);

            if (currentState != null)
                currentState.render();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            _isRunning = false;
            base.OnExiting(sender, args);
        }

        protected override void EndRun()
        {
            root.cleanup();
            base.EndRun();
            shutdown();
        }

        public void shutdown()
        {
            if (_initialised)
            {
                logger.info("shutting down");

                _initialised = false;

                if (_state != null)
                {
                    _state.end();
                    _state = null;
                }

                CoreRegistry.clear();
                if (_state != null)
                {
                    _state.end();
                    _state = null;
                }


                for (int i = _subsystems.Count - 1; i >= 0; i--)
                    _subsystems[i].shutdown();

                _subsystems.Clear();

                CoreRegistry.shutdown();
            }
        }

        public bool isRunning { get { return _isRunning; } }

        public bool isDisposed { get { return _isDisposed; } }

        public bool hasFocus { get { return _hasFocus; } }

        public bool hasMouseFocus { get { return _hasMouseFocus; } }

        public IGameState currentState { get { return _state; } }

        public void changeState(IGameState state)
        {
            logger.info("changing state {0}", state.ToString());

            if (_pendingState != null)
                _pendingState = state;
            else
                switchState(state);
        }

        private void switchState(IGameState state)
        {
            if (_state != null)
            {
                _state.end();
                _state = null;
                _isRunning = false;
            }

            _state = state;

            if (_state != null)
                _state.begin();

            if (onStateChange != null)
                onStateChange(new StateChangeEvent(state));
        }

        private void processStateChanges()
        {
            if (_pendingState != null)
            {
                switchState(_pendingState);
                _pendingState = null;
            }
        }

        
    }
}
