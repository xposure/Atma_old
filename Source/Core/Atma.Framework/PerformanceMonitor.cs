using Atma.Core;
using Atma.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Atma
{
    public struct Activity
    {
        public string name;
        public long ownTime;
        public long startTime;
        public long resumeTime;
    }

    public static class PerformanceMonitor
    {
        private static int RETAINED_CYCLES = 400;
        private static double DECAY_RATE = 0.98;

        //private List<TObjectLongMap<String>> metricData;


        private static Queue<Dictionary<String, long>> metricData = new Queue<Dictionary<string, long>>();
        private static Dictionary<string, double> spikeData = new Dictionary<string, double>();
        private static Dictionary<string, long> lastData = new Dictionary<string, long>();
        private static Dictionary<string, long> currentData = new Dictionary<string, long>();
        private static Dictionary<string, long> runningTotals = new Dictionary<string, long>();
        private static Stack<Activity> _activityStack = new Stack<Activity>();
        private static int mainThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
        private static TimeBase _timer;

        public static void init()
        {
            _timer = CoreRegistry.require<TimeBase>();
        }

        public static void rollCycle()
        {
            lastData = new Dictionary<string, long>(currentData.Count);
            foreach (var kvp in currentData)
                lastData.Add(kvp.Key, kvp.Value);
            metricData.Enqueue(lastData);

            var newSpikeData = spikeData.ToArray();
            foreach(var kvp in newSpikeData)
            //foreach (var kvp in spikeData)
                spikeData[kvp.Key] = kvp.Value * DECAY_RATE;

            foreach (var kvp in currentData)
            {
                runningTotals[kvp.Key] = runningTotals.get(kvp.Key) + kvp.Value;

                var prev = spikeData.get(kvp.Key);
                if (kvp.Value > prev)
                    spikeData[kvp.Key] = kvp.Value;
            }

            while (metricData.Count > RETAINED_CYCLES)
            {
                var data = metricData.Dequeue();
                foreach (var kvp in data)
                    runningTotals[kvp.Key] = runningTotals.get(kvp.Key) - kvp.Value;
            }

            currentData.Clear();
        }

        public static void start(String activity)
        {
            if (Thread.CurrentThread.ManagedThreadId != mainThread)
            {
                return;
            }

            var newActivity = new Activity();
            newActivity.name = activity;
            newActivity.startTime = _timer.rawTimeInMs;
            if (_activityStack.Count > 0)
            {
                Activity currentActivity = _activityStack.Peek();
                currentActivity.ownTime += newActivity.startTime - ((currentActivity.resumeTime > 0) ? currentActivity.resumeTime : currentActivity.startTime);
            }

            _activityStack.Push(newActivity);
        }

        public static void end(string name)
        {
            if (Thread.CurrentThread.ManagedThreadId != mainThread || _activityStack.Count == 0)
            {
                return;
            }

            var oldActivity = _activityStack.Pop();
            long time = _timer.rawTimeInMs;
            long total = (oldActivity.resumeTime > 0) ? oldActivity.ownTime + time - oldActivity.resumeTime : time - oldActivity.startTime;

            currentData[oldActivity.name] = currentData.get(oldActivity.name) + total;

            if (_activityStack.Count > 0)
            {
                Activity currentActivity = _activityStack.Pop();
                currentActivity.resumeTime = time;
                _activityStack.Push(currentActivity);
            }
        }

        public static Dictionary<string, long> getRunningTotals()
        {
            return runningTotals;
        }

        public static Dictionary<string, double> getRunningMean()
        {
            var result = new Dictionary<String, double>();
            var factor = 1.0 / metricData.Count;

            foreach (var kvp in runningTotals)
            {
                if (kvp.Value > 0)
                    result[kvp.Key] = kvp.Value * factor;
            }

            return result;
        }

        public static Dictionary<string, long> getLastFrame()
        {
            return lastData;
        }

        public static Queue<Dictionary<string, long>> getMetricData()
        {
            return metricData;
        }

        public static Dictionary<string, double> getDecayingSpikes()
        {
            return spikeData;
        }
    }
}
