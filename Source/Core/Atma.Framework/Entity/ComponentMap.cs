using System;
using System.Collections.Generic;

namespace Atma.Entity
{
    public class ComponentMap : IEnumerable<KeyValuePair<int, Component>>
    {
        public readonly Type type;
        public readonly Dictionary<int, Component> components;

        public ComponentMap(Type type)
        {
            this.type = type;
            this.components = new Dictionary<int, Component>();
        }

        public void add(int id, Component c)
        {
            if (components.ContainsKey(id))
                components.Remove(id);

            components.Add(id, c);
        }

        public bool has(int id)
        {
            return components.ContainsKey(id);
        }

        public Component get(int id)
        {
            Component c = null;
            components.TryGetValue(id, out c);
            return c;
        }

        public void remove(int id)
        {
            if (components.ContainsKey(id))
                components.Remove(id);
        }

        public void clear()
        {
            components.Clear();
        }

        public IEnumerator<KeyValuePair<int, Component>> GetEnumerator()
        {
            foreach (var kvp in components)
                yield return kvp;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
