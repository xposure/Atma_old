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
        private static Dictionary<Type, List<object>> _storeTypeMap = new Dictionary<Type, List<object>>();
        private static List< object> _store = new List<object>();
        //private static Dictionary<string, object> _store = new Dictionary<string, object>();
        private static List<object> _permStore = new List<object>();
        #endregion Members

        #region Methods
        /// <summary>
        /// Registers an object. These objects will be removed when CoreRegistry.clear() is called (typically when game state changes)
        /// </summary>
        /// <typeparam name="U">The system itself</typeparam>
        /// <param name="uri">Uri path</param>
        /// <param name="obj">The system itself</param>
        /// <returns>The system itself</returns>
        public static U put<U>(U obj)
        {
            var type = obj.GetType();
            //var type = typeof(U);

            //if (_store.ContainsKey(type))
            //    logger.warn(string.Format("replacing {0}", type));

            _store.Add( obj);

            //look up all the maps and see if its assignable
            foreach (var kvp in _storeTypeMap)
            {
                if (kvp.Key.IsAssignableFrom(type))
                    kvp.Value.Add(obj);
            }

            return obj;
        }

        /// <summary>
        /// Registers an object. These objects are not removed when CoreRegistry.clear() is called.
        /// </summary>
        /// <typeparam name="U">The system itself</typeparam>
        /// <param name="uri">Uri path</param>
        /// <param name="obj">The system itself</param>
        /// <returns>The system itself</returns>
        public static U putPermanently<U>(U obj)
        {
            var type = typeof(U);
            put<U>(obj);
            _permStore.Add(type);

            return obj;
        }

        /// <summary>
        /// Looks up a system and throws an exception if it doesn't exist
        /// </summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// 
        /// <returns>The system fulfilling the given interface</returns>
        public static T require<T>()
            where T : class
        {
            var t = getFirst<T>();
            if (t == null)
                throw new Exception(string.Format("{0} is required", typeof(T)));

            return t;
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
            return getFirst<T>();
        }

        public static T getFirst<T>()
        {
            var typemap = buildStoreMap<T>();
            if (typemap.Count == 0)
                return default(T);

            return (T)typemap[0];
        }

        public static IEnumerable<T> getBy<T>()
        {
            var typemap = buildStoreMap<T>();
            foreach (var t in typemap)
                yield return (T)t;
        }

        private static List<object> buildStoreMap<T>()
        {
            var type = typeof(T);
            var typemap = _storeTypeMap.get(type);
            if (typemap == null)
            {
                typemap = new List<object>();
                foreach (var t in items)
                    if (t is T)
                        typemap.Add(t);

                _storeTypeMap.Add(type, typemap);
            }

            return typemap;
        }

        public static IEnumerable<object> items { get { return _store; } }

        /// <summary>
        /// Clears all non-permanent objects from the registry.
        /// </summary>
        public static void clear()
        {
            foreach (var list in _storeTypeMap.Values)
                list.Clear();

            _storeTypeMap.Clear();

            _store.Clear();
            foreach (var obj in _permStore)
                put(obj);
        }

        public static void shutdown()
        {
            logger.info("shutdown");
            
            _storeTypeMap.Clear();
            _store.Clear();
            _permStore.Clear();
        }

        /// <summary>
        /// Removes the system fulfilling the given interface
        /// </summary>
        /// <param name="uri">Uri path</param>
        public static void remove<T>(T t)
            where T : class
        {
            var indexOf = _store.IndexOf(t);
            //var type = typeof(T);
            //if (_store.ContainsKey(type))
            {
                _store.RemoveAt(indexOf);
                foreach (var list in _storeTypeMap.Values)
                    list.Remove(t);
            }
        }

        //public static void remove(object t)
        //{
        //    if (uri.isValid())
        //    {
        //        var item = _store.get(uri);
        //        if (item != null)
        //        {
        //            foreach (var list in _storeTypeMap.Values)
        //                list.Remove(item);
        //        }

        //        _store.Remove(uri);
        //    }
        //    else
        //        logger.error("invalid uri");
        //}


        #endregion Methods
    }
}
