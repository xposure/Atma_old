
namespace Atma.Entities
{
    public class Component : Script, IComponent
    {
    }

    public abstract class DisposableComponent : Component, IDisposableComponent
    {
        public abstract void dispose();
    }
}
