﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Graphics;

namespace Atma
{
    public interface IWindowEventListener
    {
        /// <summary>
        /// Window has moved position
        /// </summary>
        /// <param name="rw">The RenderWindow which created this event</param>
        void WindowMoved(RenderWindow rw);

        /// <summary>
        /// Window has resized
        /// </summary>
        /// <param name="rw">The RenderWindow which created this event</param>
        void WindowResized(RenderWindow rw);

        /// <summary>
        /// Window has closed
        /// </summary>
        /// <param name="rw">The RenderWindow which created this event</param>
        void WindowClosed(RenderWindow rw);

        /// <summary>
        /// Window lost/regained the focus
        /// </summary>
        /// <param name="rw">The RenderWindow which created this event</param>
        void WindowFocusChange(RenderWindow rw);
    }

    public abstract class Platform : IDisposable // Singleton<WindowMonitor>
    {
        private static readonly Logger logger = Logger.getLogger(typeof(Platform));
        private Dictionary<RenderWindow, List<IWindowEventListener>> _listeners = new Dictionary<RenderWindow, List<IWindowEventListener>>();
        private List<RenderWindow> _windows = new List<RenderWindow>();
        public IEnumerable<RenderWindow> Windows { get { return _windows; } }

        protected Platform() { }

        protected static Platform _instance;
        
        ///// <summary>
        ///// Singleton Instance of the class
        ///// </summary>
        //public static Platform Instance { get { return _instance; } }

        /// <summary>
        /// Add a listener to listen to renderwindow events (multiple listener's per renderwindow is fine)
        /// The same listener can listen to multiple windows, as the Window Pointer is sent along with
        /// any messages.
        /// </summary>
        /// <param name="window">The RenderWindow you are interested in monitoring</param>
        /// <param name="listener">Your callback listener</param>
        public void RegisterListener(RenderWindow window, IWindowEventListener listener)
        {
            Contract.RequiresNotNull(window, "window");
            Contract.RequiresNotNull(listener, "listener");

            if (!_listeners.ContainsKey(window))
            {
                _listeners.Add(window, new List<IWindowEventListener>());
            }
            _listeners[window].Add(listener);
        }

        /// <summary>
        /// Remove previously added listener
        /// </summary>
        /// <param name="window">The RenderWindow you registered with</param>
        /// <param name="listener">The listener registered</param>
        public void UnregisterListener(RenderWindow window, IWindowEventListener listener)
        {
            Contract.RequiresNotNull(window, "window");
            Contract.RequiresNotNull(listener, "listener");

            if (_listeners.ContainsKey(window))
            {
                _listeners[window].Remove(listener);
            }
        }

        /// <summary>
        /// Called by RenderWindows upon creation for Ogre generated windows. You are free to add your
        /// external windows here too if needed.
        /// </summary>
        /// <param name="window">The RenderWindow to monitor</param>
        public void RegisterWindow(RenderWindow window)
        {
            Contract.RequiresNotNull(window, "window");

            _windows.Add(window);
            _listeners.Add(window, new List<IWindowEventListener>());
        }

        /// <summary>
        /// Called by RenderWindows upon destruction for Ogre generated windows. You are free to remove your
        /// external windows here too if needed.
        /// </summary>
        /// <param name="window">The RenderWindow to remove from list</param>
        public void UnregisterWindow(RenderWindow window)
        {
            Contract.RequiresNotNull(window, "window");

            if (_windows.Contains(window))
            {
                _windows.Remove(window);
            }

            if (_listeners.ContainsKey(window))
            {
                _listeners[window].Clear();
                _listeners.Remove(window);
            }
        }

        /// <summary>
        /// Window has either gained or lost the focus
        /// </summary>
        /// <param name="window">RenderWindow that caused the event</param>
        /// <param name="hasFocus">True if window has focus</param>
        public void WindowFocusChange(RenderWindow window, bool hasFocus)
        {
            Contract.RequiresNotNull(window, "window");

            if (_windows.Contains(window))
            {
                logger.info("window focus changed [{0}]", hasFocus);

                // Notify Window of focus change
                window.IsActive = hasFocus;

                // Notify listeners of focus change
                foreach (IWindowEventListener listener in _listeners[window])
                {
                    listener.WindowFocusChange(window);
                }

                return;
            }
        }

        /// <summary>
        /// Window has moved position
        /// </summary>
        /// <param name="window">RenderWindow that caused the event</param>
        public void WindowMoved(RenderWindow window)
        {
            Contract.RequiresNotNull(window, "window");

            if (_windows.Contains(window))
            {
                logger.info("window moved");

                // Notify Window of Move or Resize
                window.WindowMovedOrResized();

                // Notify listeners of Resize
                foreach (IWindowEventListener listener in _listeners[window])
                {
                    listener.WindowMoved(window);
                }
                return;
            }
        }

        /// <summary>
        /// Window has changed size
        /// </summary>
        /// <param name="win">RenderWindow that caused the event</param>
        public void WindowResized(RenderWindow window)
        {
            Contract.RequiresNotNull(window, "window");

            if (_windows.Contains(window))
            {
                logger.info("window resized");

                // Notify Window of Move or Resize
                window.WindowMovedOrResized();

                // Notify listeners of Resize
                foreach (IWindowEventListener listener in _listeners[window])
                {
                    listener.WindowResized(window);
                }
                return;
            }
        }

        /// <summary>
        /// Window has closed
        /// </summary>
        /// <param name="window">RenderWindow that caused the event</param>
        public void WindowClosed(RenderWindow window)
        {
            Contract.RequiresNotNull(window, "window");

            if (_windows.Contains(window))
            {
                logger.info("window closed");

                // Notify listeners of close
                foreach (IWindowEventListener listener in _listeners[window])
                {
                    listener.WindowClosed(window);
                }

                // Notify Window of closure
                window.Dispose();

                return;
            }
        }

        #region Singleton<WindowManager> Members

        public void Dispose()
        {
            foreach (List<IWindowEventListener> list in _listeners.Values)
            {
                list.Clear();
            }
            _listeners.Clear();

            _windows.Clear();

            //base.Dispose();
        }

        #endregion Singleton<WindowManager> Members

        public static void doEvents()
        {
            if (_instance != null)
                _instance.processEvents();
        }

        protected abstract void processEvents();

        
        
    }

}
