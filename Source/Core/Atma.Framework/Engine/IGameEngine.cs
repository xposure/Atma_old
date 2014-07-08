using Atma.Events;
using System;

namespace Atma.Engine
{
    public interface IGameEngine : IDisposable
    {
        //void init();

        void shutdown();

        bool isRunning { get; }

        bool isDisposed { get; }

        IGameState currentState { get; }

        void changeState(IGameState state);

        bool hasFocus { get; }

        bool hasMouseFocus { get; }

        event OnStateChangeEvent onStateChange;
    }

    //public abstract class GameEngine : IGameEngine
    //{

    //}
}
