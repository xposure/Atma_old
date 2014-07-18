
namespace Atma
{
    public interface IUpdateSubscriber 
    {
        void update(float delta);
    }

    public interface IRenderSubscriber 
    {
        void render();
    }

    public interface IInputSubscriber 
    {
        bool input();
    }

    public interface IComponentSystem 
    {
        void init();
        void shutdown();
    }
}
