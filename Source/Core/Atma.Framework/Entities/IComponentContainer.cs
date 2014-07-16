using System.Collections.Generic;

namespace Atma.Entities
{
    public interface IComponentContainer : IEnumerable<IComponent>
    {
        bool hasComponent<T>() where T: IComponent;
        T getComponent<T>() where T: IComponent;
    }
}
