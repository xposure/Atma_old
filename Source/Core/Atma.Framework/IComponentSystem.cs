
namespace Atma
{
    public interface IUpdateSubscriber : ICore
    {
        void update(float delta);
    }

    public interface IRenderSubscriber : ICore
    {
        void render();
    }

    public interface IInputSubscriber : ICore
    {
        bool input();
    }

    public interface IComponentSystem : ICore
    {
        void init();
        void shutdown();
    }
}
