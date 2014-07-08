
namespace Atma.Systems
{
    public interface IUpdateSystem : ISystem
    {
    }

    public class UpdateSystem : IUpdateSystem
    {
        public void init()
        {
            //require<IUpdate>
            //require<IFixedUpdate>
        }

        public void update()
        {
        }

        public void render()
        {
        }

        public void destroy()
        {

        }
    }


    //public class EntitySystem : ISystem
    //{
    //    public event Action<Entity> add;
    //    public event Action<Entity> remove;

    //    private List<Entity> _entities = new List<Entity>(1024);
    //    private List<Entity> _addEntities = new List<Entity>(64);
    //    private List<Entity> _removeEntities = new List<Entity>(64);

    //    public void update()
    //    {
    //        while (_removeEntities.Count > 0)
    //        {
    //            var next = _removeEntities[_removeEntities.Count - 1];
    //            var index = _entities.IndexOf(next);

    //            if (index == -1)
    //                throw new ArgumentOutOfRangeException("index");

    //            if (_addEntities.Count > 0)
    //            {
    //                _entities[index] = _addEntities[_addEntities.Count - 1];
    //                _addEntities.RemoveAt(_addEntities.Count - 1);

    //                if (add != null)
    //                    add(_entities[index]);
    //            }

    //            if (remove != null)
    //                remove(_removeEntities[_removeEntities.Count - 1]);

    //            _removeEntities.RemoveAt(_removeEntities.Count - 1);
    //        }

    //        while (_addEntities.Count > 0)
    //        {
    //            _entities.Add(_addEntities[_addEntities.Count - 1]);
    //            _addEntities.RemoveAt(_addEntities.Count - 1);

    //            if (add != null)
    //                add(_entities[_entities.Count - 1]);
    //        }
    //    }

    //    public void render()
    //    {
    //    }
    //}
}
