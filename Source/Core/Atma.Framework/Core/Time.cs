using System.Collections.Generic;

namespace Atma.Core
{
    public interface ITime
    {

        /// <returns>The size of the time change for the current update, in seconds</returns>
        float delta{get;}

        /// <returns>The size of the time change for the current update in ms</returns>
        long deltaInMs { get; }

        /// <returns>The current framerate</returns>
        float fps { get; }

        /// <returns>Game time in milliseconds. This is synched with the server.</returns>
        long gameTimeInMs { get; }

        /// <returns>The current game time, in seconds.</returns>
        float gameTime { get; }

        /// <returns>float time in milliseconds.</returns>
        double realTimeInMs { get; }

    }

    public interface IGameTime : ITime
    {
        /// <summary>
        /// Updates time.  This returns an iterator that will return a float for each time step to be processed this
        /// frame - as the steps are iterated over gameTime will be updated.
        /// </summary>
        /// <returns>An iterator over separate time cycles this tick.</returns>
        IEnumerable<float> tick();

        /// <summary>
        /// Sets the game time.
        /// </summary>
        void setGameTime(long time);

        /// <returns>Access to the raw timer</returns>
        long rawTimeInMs { get; }

        int frameCount { get; }
        /// <summary>
        /// Updates the timer with the desired time from the server. The game time won't immediately be updated - instead
        /// the update will be applied over a number of ticks to smooth the resynchronization.
        /// </summary>
        /// <param name="targetTime"></param>
        void updateTimeFromServer(long targetTime);

        void setPaused(bool pause);

        bool paused { get; }

        bool isSlow { get; }
    }


}
