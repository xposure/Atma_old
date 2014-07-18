using Atma.Engine;

namespace Atma.Rendering
{
    public interface ISubsystem 
    {
        GameUri uri { get; }

        void preInit();
        void postInit();

        void preUpdate(float delta);
        //void update(float delta);
        void postUpdate(float delta);

        //void preRender();
        //void render();
        //void postRender();

        void shutdown();

        //void dispose();

        //void registerSystems(ComponentSystemManager componentSystemManager);
    }
}
