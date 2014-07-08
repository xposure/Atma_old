
namespace Atma.Engine
{
    public interface IGameState
    {
        void begin();

        void end();

        void update(float dt);

        void input(float dt);

        void render();

    }
}
