using Atma.Events;
using System.Collections.Generic;

namespace Atma.Entities
{
    public interface IEntityManager : IEnumerable<int>
    {
        event OnEntity onEntityChange;
        event OnEntity onEntityAdd;
        event OnEntity onEntityRemove;

        int create();
        //IEntityRef create(params IComponent[] components);
        //IEntityRef create(string prefab);
        //int get(int id);
        bool exists(int id);
        void destroy(int e);
        int count { get; }
    }
}
