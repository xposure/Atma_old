using Atma.Rendering;
using Atma.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Atma.Core
{
    public class StopwatchTime : TimeBase
    {
        private Stopwatch _stopwatch = Stopwatch.StartNew();
        public override long rawTimeInMs
        {
            get { return _stopwatch.ElapsedMilliseconds; }
        }
    }

    public class TestTime : TimeBase
    {
        private long _rawTime = 0;
        public override long rawTimeInMs
        {
            get { return _rawTime; }
        }

        public void step()
        {
            _rawTime += 10;
        }
    }

    public abstract class TimeBase : IGameTime, ISubsystem
    {
        public static readonly GameUri Uri = "subsystem:time";
        private static readonly Logger logger = Logger.getLogger(typeof(TimeBase));

        private const float DECAY_RATE = 0.95f;
        private static readonly float ONE_MINUS_DECAY_RATE = 1.0f - DECAY_RATE;
        private const float RESYNC_TIME_RATE = 0.1f;

        private const int MAX_UPDATE_CYCLE_LENGTH = 100;
        private const int UPDATE_CAP = 33;  //tries to keep the ticks at 30fps

        private long _last = 0;
        private long _delta = 0;
        private float _avgDelta;
        private long _desynch;
        private bool _paused;
        private bool _isSlow = false;

        private long _gameTime = 0;

        public override string ToString()
        {
            return string.Format("Time {{ gt: {0}, avgdt: {1}, dt: {2}, fps: {3}, desynch: {4}, rt: {5} }}", gameTime, _avgDelta / 1000f, delta, fps, _desynch, realTimeInMs);
        }

        public abstract long rawTimeInMs { get; }

        public long deltaL { get { return _delta; } }

        public float delta { get { return _delta / 1000f; } }

        public long deltaInMs { get { return _delta; } }

        //public float avgDelta { get { return _avgDelta / 1000f; } }

        public float fps { get { return 1000.0f / _avgDelta; } }

        public long gameTimeInMs { get { return _gameTime; } }

        public float gameTime { get { return _gameTime / 1000f; } }

        public int frameCount { get; private set; }

        public void setGameTime(long amount)
        {
            _delta = 0;
            _gameTime = amount;
        }

        public double realTimeInMs { get { return rawTimeInMs / 1000.0; } }

        public double maximumDeltaTimeD { get { return MAX_UPDATE_CYCLE_LENGTH / 1000.0; } }

        public double fixedDeltaTimeD { get { return 1000 / 60.0; } }

        public void updateTimeFromServer(long targetTime)
        {
            _desynch = targetTime - gameTimeInMs;
        }

        public bool paused
        {
            set { this._paused = value; }
            get { return _paused; }
        }

        /// <summary>
        /// Increments time
        /// </summary>
        /// <returns> The number of update cycles to run </returns>
        public virtual IEnumerable<float> tick()
        {
            long now = rawTimeInMs;
            long totalDelta = now - _last;
            while (0 == totalDelta)
            {
                Thread.Sleep(0);
                now = rawTimeInMs;
                totalDelta = now - _last;
            }

            _isSlow = (totalDelta > UPDATE_CAP);
            _last = now;
            _avgDelta = _avgDelta * DECAY_RATE + totalDelta * ONE_MINUS_DECAY_RATE;

            if (totalDelta >= MAX_UPDATE_CYCLE_LENGTH)
            {
                logger.warn("Delta too great ({0}), capping to {1}", totalDelta, MAX_UPDATE_CYCLE_LENGTH);
                totalDelta = MAX_UPDATE_CYCLE_LENGTH;
            }

            while (totalDelta > 0)
            {
                //skip this frame and add it to the next
                if (_isSlow && totalDelta < UPDATE_CAP)
                {
                    _last -= totalDelta;
                    yield break;
                }

                var newDelta = totalDelta;
                if (newDelta > UPDATE_CAP)
                    newDelta = UPDATE_CAP;

                totalDelta -= newDelta;

                // Reduce desynch between server time
                if (_desynch != 0)
                {
                    long diff = (long)Math.Ceiling(_desynch * RESYNC_TIME_RATE);
                    if (diff == 0)
                    {
                        diff = (long)Math.Sign(_desynch);
                    }
                    _gameTime += diff;
                    _desynch -= diff;
                }

                if (paused)
                    _delta = 0;
                else
                    _delta = newDelta;

                _gameTime += _delta;

                frameCount++;
                yield return _delta / 1000f;
            }
        }

        public void updateDelta(long dt)
        {
            _delta = dt;
        }

        public void setPaused(bool pause)
        {
            _paused = pause;
        }

        public bool isSlow { get { return _isSlow; } }

        public GameUri uri { get { return Uri; } }

        public void init()
        {
            logger.info("initialise");
        }

        public void preUpdate(float delta)
        {
        }

        public void postUpdate(float delta)
        {
        }

        public void shutdown()
        {
            logger.info("shutdown");
        }

    }

}
