using System.Collections.Generic;
using System.Linq;

namespace Atma.Entities
{
    public class ComponentTable
    {
        private Dictionary<string, ComponentMap> _componentMap = new Dictionary<string, ComponentMap>();
        private Dictionary<int, List<string>> _idLookup = new Dictionary<int, List<string>>();

        public void add<T>(int id, string component, T t)
            where T : Component
        {
            var type = typeof(T);
            ComponentMap components;
            if (!_componentMap.TryGetValue(component, out components))
            {
                components = new ComponentMap(type);
                _componentMap.Add(component, components);
            }

            components.add(id, t);

            List<string> idLookup;
            if (!_idLookup.TryGetValue(id, out idLookup))
            {
                idLookup = new List<string>();
                _idLookup.Add(id, idLookup);
            }

            if (!idLookup.Contains(component))
                idLookup.Add(component);
        }

        public void remove(int id, string component)
        //where T : Component
        {
            ComponentMap components;
            if (_componentMap.TryGetValue(component, out components))
                components.remove(id);

            List<string> idLookup;
            if (_idLookup.TryGetValue(id, out idLookup))
            {
                if (idLookup.Contains(component))
                    idLookup.Remove(component);
            }
        }

        public bool has(int id, string component)
        {
            ComponentMap components;
            if (_componentMap.TryGetValue(component, out components))
            {
                return components.has(id);
            }

            return false;
        }

        public T get<T>(int id, string component)
            where T : Component
        {
            ComponentMap components;
            if (_componentMap.TryGetValue(component, out components))
                return (T)components.get(id);

            return default(T);
        }

        public IEnumerable<Component> getAll(int id)
        {
            List<string> idLookup;
            if (_idLookup.TryGetValue(id, out idLookup))
            {
                foreach (var type in idLookup)
                {
                    ComponentMap components;
                    if (_componentMap.TryGetValue(type, out components))
                        yield return components.get(id);

                }
            }
        }

        public void clear(int id)
        {
            List<string> idLookup;
            if (_idLookup.TryGetValue(id, out idLookup))
            {
                foreach (var type in idLookup)
                {
                    ComponentMap components;
                    if (_componentMap.TryGetValue(type, out components))
                        components.remove(id);

                }
                idLookup.Clear();

                _idLookup.Remove(id);
            }
        }

        public void clear()
        {
            var maps = _componentMap.Values.ToArray();
            foreach (var m in maps)
                m.clear();

            _componentMap.Clear();
            _idLookup.Clear();
        }
    }
}
