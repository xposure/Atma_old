using Atma.Engine;

namespace Atma.Events
{
    public delegate bool OnStateChangeEvent(StateChangeEvent e);

    public class StateChangeEvent
    {
        public IGameState state { get; private set; }

        public StateChangeEvent(IGameState state)
        {
            this.state = state;
        }
    }
}
