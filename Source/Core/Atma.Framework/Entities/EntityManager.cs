using Atma.Engine;
using Atma.Events;
using System.Collections.Generic;

namespace Atma.Entities
{
    public class EntityManager : IEntityManager
    {
        public static readonly GameUri Uri = "subsystem:entity";

        public event OnEntity onEntityChange;
        public event OnEntity onEntityAdd;
        public event OnEntity onEntityRemove;

        private ComponentTable _componentTable = new ComponentTable();

        private int _nextId = 0;
        private Dictionary<int, List<string>> _tagsReverse = new Dictionary<int, List<string>>();
        private Dictionary<string, List<int>> _tags = new Dictionary<string, List<int>>();
        private List<int> _entities = new List<int>(1024);
        private HashSet<int> _entityMap = new HashSet<int>();
        //private List<int> _freeIds = new List<int>();

        public int create()
        {
            _nextId++;
            var id = _nextId;

            _entityMap.Add(id);
            _entities.Add(id);

            if (onEntityAdd != null)
                onEntityAdd(id);

            return id;
        }

        //public int create(params Component[] components)
        //{
        //    _nextId++;
        //    var id = _nextId;

        //    _entityMap.Add(id);
        //    _entities.Add(id);

        //    foreach (var c in components)
        //        _componentTable.add(id, c);

        //    if (onEntityAdd != null)
        //        onEntityAdd(id);

        //    return id;
        //}

        public T addComponent<T>(int id, string component, T t)
            where T : Component
        {
            _componentTable.add(id, component, t);

            if (onEntityChange != null)
                onEntityChange(id);

            return t;
        }

        public void removeComponent(int id, string component)
        //where T: Component
        {
            _componentTable.remove(id, component);

            if (onEntityChange != null)
                onEntityChange(id);
        }

        public bool hasComponent(int id, string component)
        //where T : Component
        {
            return _componentTable.has(id, component);
        }

        public T getComponent<T>(int id, string component)
            where T : Component
        {
            return _componentTable.get<T>(id, component);
        }

        public void tag(int id, string name)
        {
            var tags = _tags.getOrCreate(name);
            tags.Add(id);

            var tagsReverse = _tagsReverse.getOrCreate(id);
            tagsReverse.Add(name);
        }

        public void untag(int id, string name)
        {
            var tags = _tags.get(name);
            if (tags != null)
                tags.Remove(id);

            var tagsReverse = _tagsReverse.get(id);
            if(tagsReverse != null)
                tagsReverse.Remove(name);
        }

        public IEnumerable<Component> getComponents(int id)
        {
            return _componentTable.getAll(id);
        }

        public IEnumerable<int> getWithComponents(params string[] components)
        {
            foreach (var id in _entities)
            {
                var found = true;
                for (var i = 0; i < components.Length; i++)
                {
                    if (!_componentTable.has(id, components[i]))
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                    yield return id;
            }
        }

        public IEnumerable<int> getEntitiesByTag(string tag)
        {
            var tags = _tags.get(tag);
            if (tags != null)
                foreach (var id in tags)
                    yield return id;
        }

        public int countEntitiesByTag(string tag)
        {
            var tags = _tags.get(tag);
            if (tags != null)
                return tags.Count;

            return 0;
        }

        public int getEntityByTag(string tag)
        {
            var tags = _tags.get(tag);
            if (tags != null)
                if (tags.Count > 0)
                    return tags[0];

            return 0;
        }

        public bool exists(int id)
        {
            return _entityMap.Contains(id);
        }

        public void destroy(int id)
        {
            if (exists(id))
            {
                var entityTags = _tagsReverse.get(id);
                if (entityTags != null)
                {
                    foreach (var tag in entityTags)
                    {
                        var tagLookup = _tags.get(tag);
                        tagLookup.Remove(id);
                    }

                    entityTags.Clear();
                    _tagsReverse.Remove(id);
                }

                var index = _entities.IndexOf(id);
                _entities[index] = _entities[_entities.Count - 1];
                _entities.RemoveAt(_entities.Count - 1);

                _entityMap.Remove(id);
                _componentTable.clear(id);

                if (onEntityRemove != null)
                    onEntityRemove(id);
            }
        }

        public void clear()
        {
            _tags.Clear();
            _tagsReverse.Clear();
            _componentTable.clear();
            _entities.Clear();
            _entityMap.Clear();
        }

        public EntityRef createRef(int id)
        {
            return new EntityRef(id, this);
        }

        public int count
        {
            get { return _entities.Count; }
        }

        public IEnumerator<int> GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _entities.GetEnumerator();
        }

    }

    public static class EntityManagerExtensions
    {
        public static EntityManager entities(this ICore e)
        {
            return CoreRegistry.require<EntityManager>(EntityManager.Uri);
        }
    }
}
