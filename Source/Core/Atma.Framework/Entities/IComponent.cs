
namespace Atma.Entities
{
    public interface IComponent// : ICore
    {
    }

    public interface IDisposableComponent : IComponent
    {
        void dispose();
    }
}
