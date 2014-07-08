
namespace Atma.Systems
{
    public interface ISystem
    {
        void init();
        void update();
        void render();
        void destroy();
    }
}
