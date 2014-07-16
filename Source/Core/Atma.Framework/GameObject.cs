using Atma.Engine;
using Atma.Entities;
using Atma.Json;
using System;
using System.Collections.Generic;

namespace Atma
{
    public static class GameObjectExt
    {

        public static IEnumerable<GameObject> allEnabledChildren(this GameObject target)
        {
            if (target.enabled)
            {
                foreach (var root in target.children)
                {
                    foreach (var child in root.allEnabledChildren())
                        if (child.enabled)
                            yield return child;

                    if (root.enabled)
                        yield return root;
                }
            }
        }

        public static IEnumerable<GameObject> allChildren(this GameObject target)
        {
            foreach (var root in target.children)
            {
                foreach (var child in root.allChildren())
                    yield return child;
                yield return root;
            }
        }

        public static IEnumerable<Script> allScripts(this GameObject target)
        {
            foreach (var c in target.scripts)
                yield return c;

            foreach (var child in target.allChildren())
                foreach (var c in child.scripts)
                    yield return c;
        }

        public static void broadcast(this GameObject target, string name)
        {
            if (target.enabled)
            {
                Event.Invoke(target.id, name);
                foreach (var c in target.allEnabledChildren())
                    Event.Invoke(c.id, name);
            }
        }

        //public static Collider2 collider(this GameObject target)
        //{
        //    return target.getScript<Collider2>();
        //}

        public static GameObject createChild(this GameObject target)
        {
            var go = new GameObject();
            var t = new Transform();
            go.add("transform", t);
            //go.add(t);
            return target.add(go);
        }

        public static GameObject createChild(this GameObject target, string name)
        {
            var go = new GameObject(name);
            var t = new Transform();
            go.add("transform", t);
            //go.add(new Transform());
            return target.add(go);
        }

        public static T createScript<T>(this GameObject target)
            where T : Script, new()
        {
            return target.add<T>(new T());
        }

        public static void forceSendMessage(this GameObject target, string name)
        {
            //foreach (var c in target.components)
            //MessageProxy.invoke(c, name, args);
            Event.Invoke(target.id, name);
        }

        public static void forceSendMessage<T>(this GameObject target, string name, T t)
        {
            //foreach (var c in target.components)
            //MessageProxy.invoke(c, name, args);
            Event<T>.Invoke(target.id, name, t);
        }

        public static GameObject getGameObjectWithScript<T>(this GameObject target)
            where T : Script
        {
            foreach (var c in target.scripts)
                if (c is T)
                    return target;

            foreach (var child in target.children)
            {
                var go = child.getGameObjectWithScript<T>();
                if (go != null)
                    return go;
            }

            return null;
        }

        public static T getScript<T>(this GameObject target)
            where T : Script
        {
            foreach (var c in target.scripts)
                if (c is T)
                    return (T)c;

            return default(T);
        }

        public static IEnumerable<T> getScripts<T>(this GameObject target)
            where T : Script
        {
            foreach (var c in target.scripts)
                if (c is T)
                    yield return (T)c;
        }

        public static T getScriptWithChildren<T>(this GameObject target)
            where T : Script
        {
            var c = target.getScript<T>();
            if (c != null)
                return c;

            foreach (var child in target.children)
            {
                c = child.getScriptWithChildren<T>();
                if (c != null)
                    return c;
            }

            return default(T);
        }

        public static IEnumerable<T> getScriptsWithChildren<T>(this GameObject target)
            where T : Script
        {
            var c = target.getScript<T>();
            if (c != null)
                yield return c;

            foreach (var child in target.children)
            {
                c = child.getScriptWithChildren<T>();
                if (c != null)
                    yield return c;
            }
        }


        public static bool hasScript<T>(this GameObject target)
           where T : Script
        {
            foreach (var c in target.scripts)
                if (c is T)
                    return true;

            return false;
        }

        //public static Rigidbody rigidbody(this GameObject target)
        //{
        //    return target.getScript<Rigidbody>();
        //}

        public static void sendMessage(this GameObject target, string name)
        {
            if (target.enabled)
            {
                //foreach (var c in target.components)
                //    if (c.enabled)
                //MessageProxy.invoke(c, name, args);
                Event.Invoke(target.id, name);
            }
        }



        public static void sendMessageDown(this GameObject target, string name)
        {
            if (target.enabled)
            {
                foreach (var c in target.allChildren())
                    Event.Invoke(c.id, name);
            }
        }

        public static void sendMessage<T0>(this GameObject target, string name, T0 t0)
        {
            if (target.enabled)
                Event<T0>.Invoke(target.id, name, t0);
        }
        public static void sendMessageDown<T0>(this GameObject target, string name, T0 t0)
        {
            if (target.enabled)
                foreach (var c in target.allEnabledChildren())
                    Event<T0>.Invoke(c.id, name, t0);
        }
        public static void broadcast<T0>(this GameObject target, string name, T0 t0)
        {
            target.sendMessage(name, t0);
            target.sendMessageDown(name, t0);
        }
        public static void sendMessage<T0, T1>(this GameObject target, string name, T0 t0, T1 t1)
        {
            if (target.enabled)
                Event<T0, T1>.Invoke(target.id, name, t0, t1);
        }
        public static void sendMessageDown<T0, T1>(this GameObject target, string name, T0 t0, T1 t1)
        {
            if (target.enabled)
                foreach (var c in target.allEnabledChildren())
                    Event<T0, T1>.Invoke(c.id, name, t0, t1);
        }
        public static void broadcast<T0, T1>(this GameObject target, string name, T0 t0, T1 t1)
        {
            target.sendMessage(name, t0, t1);
            target.sendMessageDown(name, t0, t1);
        }
        public static void sendMessage<T0, T1, T2>(this GameObject target, string name, T0 t0, T1 t1, T2 t2)
        {
            if (target.enabled)
                Event<T0, T1, T2>.Invoke(target.id, name, t0, t1, t2);
        }
        public static void sendMessageDown<T0, T1, T2>(this GameObject target, string name, T0 t0, T1 t1, T2 t2)
        {
            if (target.enabled)
                foreach (var c in target.allEnabledChildren())
                    Event<T0, T1, T2>.Invoke(c.id, name, t0, t1, t2);
        }
        public static void broadcast<T0, T1, T2>(this GameObject target, string name, T0 t0, T1 t1, T2 t2)
        {
            target.sendMessage(name, t0, t1, t2);
            target.sendMessageDown(name, t0, t1, t2);
        }
        public static void sendMessage<T0, T1, T2, T3>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3)
        {
            if (target.enabled)
                Event<T0, T1, T2, T3>.Invoke(target.id, name, t0, t1, t2, t3);
        }
        public static void sendMessageDown<T0, T1, T2, T3>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3)
        {
            if (target.enabled)
                foreach (var c in target.allEnabledChildren())
                    Event<T0, T1, T2, T3>.Invoke(c.id, name, t0, t1, t2, t3);
        }
        public static void broadcast<T0, T1, T2, T3>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3)
        {
            target.sendMessage(name, t0, t1, t2, t3);
            target.sendMessageDown(name, t0, t1, t2, t3);
        }
        public static void sendMessage<T0, T1, T2, T3, T4>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (target.enabled)
                Event<T0, T1, T2, T3, T4>.Invoke(target.id, name, t0, t1, t2, t3, t4);
        }
        public static void sendMessageDown<T0, T1, T2, T3, T4>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (target.enabled)
                foreach (var c in target.allEnabledChildren())
                    Event<T0, T1, T2, T3, T4>.Invoke(c.id, name, t0, t1, t2, t3, t4);
        }
        public static void broadcast<T0, T1, T2, T3, T4>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            target.sendMessage(name, t0, t1, t2, t3, t4);
            target.sendMessageDown(name, t0, t1, t2, t3, t4);
        }
        public static void sendMessage<T0, T1, T2, T3, T4, T5>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            if (target.enabled)
                Event<T0, T1, T2, T3, T4, T5>.Invoke(target.id, name, t0, t1, t2, t3, t4, t5);
        }
        public static void sendMessageDown<T0, T1, T2, T3, T4, T5>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            if (target.enabled)
                foreach (var c in target.allEnabledChildren())
                    Event<T0, T1, T2, T3, T4, T5>.Invoke(c.id, name, t0, t1, t2, t3, t4, t5);
        }
        public static void broadcast<T0, T1, T2, T3, T4, T5>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            target.sendMessage(name, t0, t1, t2, t3, t4, t5);
            target.sendMessageDown(name, t0, t1, t2, t3, t4, t5);
        }
        public static void sendMessage<T0, T1, T2, T3, T4, T5, T6>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            if (target.enabled)
                Event<T0, T1, T2, T3, T4, T5, T6>.Invoke(target.id, name, t0, t1, t2, t3, t4, t5, t6);
        }
        public static void sendMessageDown<T0, T1, T2, T3, T4, T5, T6>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            if (target.enabled)
                foreach (var c in target.allEnabledChildren())
                    Event<T0, T1, T2, T3, T4, T5, T6>.Invoke(c.id, name, t0, t1, t2, t3, t4, t5, t6);
        }
        public static void broadcast<T0, T1, T2, T3, T4, T5, T6>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            target.sendMessage(name, t0, t1, t2, t3, t4, t5, t6);
            target.sendMessageDown(name, t0, t1, t2, t3, t4, t5, t6);
        }
        public static void sendMessage<T0, T1, T2, T3, T4, T5, T6, T7>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            if (target.enabled)
                Event<T0, T1, T2, T3, T4, T5, T6, T7>.Invoke(target.id, name, t0, t1, t2, t3, t4, t5, t6, t7);
        }
        public static void sendMessageDown<T0, T1, T2, T3, T4, T5, T6, T7>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            if (target.enabled)
                foreach (var c in target.allEnabledChildren())
                    Event<T0, T1, T2, T3, T4, T5, T6, T7>.Invoke(c.id, name, t0, t1, t2, t3, t4, t5, t6, t7);
        }
        public static void broadcast<T0, T1, T2, T3, T4, T5, T6, T7>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        {
            target.sendMessage(name, t0, t1, t2, t3, t4, t5, t6, t7);
            target.sendMessageDown(name, t0, t1, t2, t3, t4, t5, t6, t7);
        }
        public static void sendMessage<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            if (target.enabled)
                Event<T0, T1, T2, T3, T4, T5, T6, T7, T8>.Invoke(target.id, name, t0, t1, t2, t3, t4, t5, t6, t7, t8);
        }
        public static void sendMessageDown<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            if (target.enabled)
                foreach (var c in target.allEnabledChildren())
                    Event<T0, T1, T2, T3, T4, T5, T6, T7, T8>.Invoke(c.id, name, t0, t1, t2, t3, t4, t5, t6, t7, t8);
        }
        public static void broadcast<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        {
            target.sendMessage(name, t0, t1, t2, t3, t4, t5, t6, t7, t8);
            target.sendMessageDown(name, t0, t1, t2, t3, t4, t5, t6, t7, t8);
        }
        public static void sendMessage<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            if (target.enabled)
                Event<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.Invoke(target.id, name, t0, t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }
        public static void sendMessageDown<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            if (target.enabled)
                foreach (var c in target.allEnabledChildren())
                    Event<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.Invoke(c.id, name, t0, t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }
        public static void broadcast<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this GameObject target, string name, T0 t0, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        {
            target.sendMessage(name, t0, t1, t2, t3, t4, t5, t6, t7, t8, t9);
            target.sendMessageDown(name, t0, t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }



        //public static IEnumerable<GameObject> CullCamera(this GameObject target, Camera camera)
        //{
        //    var transform = target.transform();

        //    if (transform == null || camera.IsObjectVisible(transform)
        //    {
        //    }

        //}

        public static Transform transform2(this GameObject target)
        {
            return target.getScript<Transform>();
        }
    }

    public sealed class GameObject
    {
        public readonly int id;
        internal bool _destroy = false;
        //private static int _index = 0;
        private List<GameObject> _children = null;
        private bool _destroyed = false;
        private bool _enabled = true;
        private GameObject _parent = null;
        private List<Script> _scripts = null;
        private List<Component> _components = new List<Component>();
        private Dictionary<string, JsonValue> properties = null;

        internal GameObject()
        {
            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            id = em.create();

            name = string.Format("Game Object {0}", id);
            //add(new Transform());
        }

        internal GameObject(string name)
        {
            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            id = em.create();
            
            this.name = name;
            // add(new Transform());
        }

        public IEnumerable<GameObject> children
        {
            get
            {
                if (_children != null)
                {
                    var count = _children.Count;
                    for (var i = 0; i < count; i++)
                        yield return _children[i];
                }
            }
        }

        public bool destroyed { get { return _destroyed; } }

        public bool enabled
        {
            get
            {
                if (!_enabled)
                    return false;

                var p = _parent;
                while (p != null)
                {
                    if (!p.enabled)
                        return false;
                    p = p._parent;
                }

                return true;
            }
            set
            {
                if (_enabled != value && (this.parent == null || this.parent.enabled))
                {
                    _enabled = value;

                    foreach (var c in this.allScripts())
                    {
                        if (value && c._enabled)
                            c.addToEvents();
                        //Game.messageCache.add(c);
                        else if (c._enabled)
                            c.removeFromEvents();
                        //Game.messageCache.remove(c);
                    }
                }
            }
        }

        public string name { get; set; }

        public GameObject parent { get { return _parent; } }

        public IEnumerable<Script> scripts
        {
            get
            {
                if (_scripts != null)
                    for (var i = 0; i < _scripts.Count; i++)
                        yield return _scripts[i];
            }
        }

        public int totalChildren { get { if (_children == null) return 0; return _children.Count; } }

        public int totalScripts { get { if (_scripts == null) return 0; return _scripts.Count; } }

        //public T add<T>(string name, T value)
        //    where T : JsonValue
        //{
        //    if (properties == null)
        //        properties = new Dictionary<string, JsonValue>();

        //    Root.instance.logging.assert(properties.ContainsKey(name), "duplicate key");
        //    properties.Add(name, value);
        //    return value;
        //}

        public T add<T>(string name, T t)
            where T : Component
        {
            add(t);
            return CoreRegistry.require<EntityManager>(EntityManager.Uri).addComponent(id, name, t);
        }

        public void remove(string name)
        {
            CoreRegistry.require<EntityManager>(EntityManager.Uri).removeComponent(id, name);
        }


        public GameObject add(GameObject go)
        {
            if (_children == null)
                _children = new List<GameObject>();

            if (go._parent != null)
                go._parent.remove(go);

            _children.Add(go);
            go._parent = this;
            Root.instance.indexedGO.Add(go.id, go);
            Root.instance.newGameObjects.Add(go);

            return go;
        }

        public void destroy()
        {
            if (!_destroy)
            {
                _destroy = true;
                this.forceSendMessage("destroy");
                foreach (var go in children)
                    go.destroy();

                Root.instance.destroyList.Add(this);
            }
        }

        public GameObject find(string name)
        {
            if (children != null)
                foreach (var c in children)
                    if (c.name == name)
                        return c;

            return null;
        }

        public GameObject findall(string name)
        {
            if (children != null)
                foreach (var c in this.allChildren())
                    if (c.name == name)
                        return c;

            return null;
        }


        public JsonValue getProp(string name, JsonTypes type)
        {
            var value = getProp(name);
            //Root.instance.logging.assert(value.Type == type, "property type mismatch");
            return value;
        }

        public void remove()
        {
            if (parent != null)
                parent.remove(this);
        }

        public void remove(GameObject go)
        {
            if (_children != null)
            {
                for (int i = _children.Count - 1; i >= 0; i--)
                {
                    if (_children[i] == go)
                    {
                        this.forceSendMessage("childremoved", _children[i]);
                        _children[i]._parent = null;
                        go._parent = null;
                        Root.instance.indexedGO.Remove(go.id);
                        _children.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void removeProp(string name)
        {
            //Root.instance.logging.assert(getProp(name) == null, "missing property");
            properties.Remove(name);
        }

        public void setParent(GameObject newParent)
        {
            if (_parent != null)
                _parent.remove(this);

            _parent = newParent;

            if (_parent != null)
                _parent.add(this);
        }

        internal T add<T>(T c)
            where T : Script
        {
            if (c.gameObject != null)
                throw new Exception("script was already added to a GO");

            if (_scripts == null)
                _scripts = new List<Script>();

            _scripts.Add(c);
            c.gameObject = this;
            Root.instance.newScriptList.Add(c);

            return c;
        }

        internal void internalDestroy()
        {
            if (!_destroyed)
            {
                _destroyed = true;
                var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
                em.destroy(id);

                foreach (var go in children)
                    go.internalDestroy();

                //Game.messageCache.remove(this);
                foreach (var c in scripts)
                    c.removeFromEvents();
                //Game.messageCache.remove(c);
                if (_scripts != null)
                    _scripts.Clear();

                if (_parent != null)
                    _parent.remove(this);
            }
        }

        private JsonValue getProp(string name)
        {
            //Root.instance.logging.assert(properties == null || !properties.ContainsKey(name), "missing property");
            return properties[name];
        }

        #region convience

        private Transform _transform;

        public Transform transform
        {
            get
            {
                if (_transform == null)
                    _transform = this.getScript<Transform>();
                return _transform;
            }
        }

        #endregion convience
    }
}