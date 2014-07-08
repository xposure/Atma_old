using System.Collections.Generic;

namespace Atma.Entity
{
    public struct EntityRef : IEnumerable<Component>
    {
        private EntityManager _entityManager;

        public EntityRef(int _id, EntityManager _em)
            : this()
        {
            id = _id;
            _entityManager = _em;
        }

        public int id { get; private set; }

        public bool exists { get { return _entityManager.exists(id); } }

        public T addComponent<T>(string component, T t)
            where T : Component
        {
            return _entityManager.addComponent(id, component, t);
        }

        public void removeComponent(string component)
            //where T : IComponent
        {
            _entityManager.removeComponent(id, component);
        }

        public bool hasComponent(string component)
            //where T : IComponent
        {
            return _entityManager.hasComponent(id, component);
        }

        public T getComponent<T>(string component)
            where T : Component
        {
            return _entityManager.getComponent<T>(id, component);
        }

        public IEnumerator<Component> GetEnumerator()
        {
            foreach (var c in _entityManager.getComponents(id))
                yield return c;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (var c in _entityManager.getComponents(id))
                yield return c;
        }
    }
}
