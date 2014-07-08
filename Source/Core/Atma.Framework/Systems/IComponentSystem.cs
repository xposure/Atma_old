
namespace Atma.Systems
{
    public interface IUpdateSubscriber
    {
        void update(float delta);
    }

    public interface IRenderSubscriber
    {
        void render();
    }

    public interface IComponentSystem
    {
        void init();
        void shutdown();
    }
}
