
namespace Atma.Entities
{
    public class Component : IComponent
    {
    }

    public abstract class DisposableComponent : Component, IDisposableComponent
    {
        public abstract void dispose();
    }
}
