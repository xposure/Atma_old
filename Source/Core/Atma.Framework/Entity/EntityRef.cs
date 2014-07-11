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

        public void tag(string tag)
        {
            _entityManager.tag(this.id, tag);
        }

        public void untag(string tag)
        {
            _entityManager.untag(this.id, tag);
        }

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

        public bool hasComponent(params string[] component)
            //where T : IComponent
        {
            for (var i = 0; i < component.Length; i++)
                if (!_entityManager.hasComponent(id, component[i]))
                    return false;

            return true;
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
