
namespace Atma.Systems
{
    public interface ISystem : ICore
    {
        void init();
        void update();
        void render();
        void destroy();
    }
}
