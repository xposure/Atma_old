#region GPLv3 License

/*
Atma
Copyright © 2014 Atma Project Team

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License V3
as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
General Public License V3 for more details.

You should have received a copy of the GNU General Public License V3
along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/

#endregion

#region Namespace Declarations
using Atma.Core;
using System;
using System.Collections.Generic;
#endregion Namespace Declarations

namespace Atma.Engine
{
    /// <summary>
    /// Registry giving access to major singleton systems, via the interface they fulfil.
    /// </summary>
    public static class CoreRegistry
    {
        private static readonly Logger logger = Logger.getLogger(typeof(CoreRegistry));

        #region Members
        private static Dictionary<string, object> _store = new Dictionary<string, object>();
        private static HashSet<string> _permStore = new HashSet<string>();
        #endregion Members

        #region Methods
        /// <summary>
        /// Registers an object. These objects will be removed when CoreRegistry.clear() is called (typically when game state changes)
        /// </summary>
        /// <typeparam name="U">The system itself</typeparam>
        /// <param name="uri">Uri path</param>
        /// <param name="obj">The system itself</param>
        /// <returns>The system itself</returns>
        public static U put<U>(GameUri uri, U obj)
        {
            if (uri.isValid())
                _store.Add(uri, obj);
            else
                logger.error("invalid uri");

            return obj;
        }

        /// <summary>
        /// Registers an object. These objects are not removed when CoreRegistry.clear() is called.
        /// </summary>
        /// <typeparam name="U">The system itself</typeparam>
        /// <param name="uri">Uri path</param>
        /// <param name="obj">The system itself</param>
        /// <returns>The system itself</returns>
        public static U putPermanently<U>(GameUri uri, U obj)
        {
            if (uri.isValid())
            {
                _store.Add(uri, obj);
                _permStore.Add(uri);
            }
            else
                logger.error("invalid uri");

            return obj;
        }

        /// <summary>
        /// Looks up a system and throws an exception if it doesn't exist
        /// </summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <param name="uri">Uri path</param>
        /// <returns>The system fulfilling the given interface</returns>
        public static T require<T>(GameUri uri)
            where T : class
        {
            if (uri.isValid())
            {
                object obj;
                if (_store.TryGetValue(uri, out obj))
                {
                    T t = obj as T;
                    if (t != null)
                        return t;

                    logger.error("invalid cast {0} to {1}", obj.GetType().Name, typeof(T).Name);
                }
            }
            else
                logger.error("invalid uri");

            throw new Exception(string.Format("{0} is required", uri));
        }

        /// <summary>
        /// Looks up the system fulfilling the given interface
        /// </summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <param name="uri">Uri path</param>
        /// <returns>The system fulfilling the given interface</returns>
        public static T get<T>(GameUri uri)
            where T : class
        {
            if (uri.isValid())
            {
                object obj;
                if (_store.TryGetValue(uri, out obj))
                {
                    T t = obj as T;
                    if (t != null)
                        return t;

                    logger.error("invalid cast {0} to {1}", obj.GetType().Name, typeof(T).Name);
                }
            }
            else
                logger.error("invalid uri");

            return null;
        }

        /// <summary>
        /// Clears all non-permanent objects from the registry.
        /// </summary>
        public static void clear()
        {
            var objsToClear = new List<string>();
            foreach (var key in _store.Keys)
                if (!_permStore.Contains(key))
                    objsToClear.Add(key);

            foreach (var key in objsToClear)
                _store.Remove(key);
        }

        public static void shutdown()
        {
            logger.info("shutdown");
            var objsToClear = new List<string>(_store.Keys);

            foreach (var key in objsToClear)
                _store.Remove(key);

            _permStore.Clear();
        }

        /// <summary>
        /// Removes the system fulfilling the given interface
        /// </summary>
        /// <param name="uri">Uri path</param>
        public static void remove(GameUri uri)
        {
            if (uri.isValid())
                _store.Remove(uri);
            else
                logger.error("invalid uri");
        }
        #endregion Methods
    }
}
