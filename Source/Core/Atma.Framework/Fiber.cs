using Atma.Collections;
using Atma.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Atma
{
    public interface IFiber
    {
        object tick();
        int executions { get; }
        bool completed { get; }
    }

    public class FiberScheduler
    {
        //list of all running fibers
        private LinkedList<IFiber> running = new LinkedList<IFiber>();
        //sorted list of all running fibers by time
        private PriorityQueue<float, IFiber> delayedByTime = new PriorityQueue<float, IFiber>();
        //sorted list of all running fivers by frames
        private PriorityQueue<int, IFiber> delayedByFrames = new PriorityQueue<int, IFiber>();

        public void tick()
        {
            var time = CoreRegistry.require<Atma.Core.TimeBase>(Atma.Core.TimeBase.Uri);
            //get all items that are ready to be dequeue by time
            while (!delayedByTime.IsEmpty && delayedByTime.Peek().Key < time.gameTime)
                running.AddLast(delayedByTime.DequeueValue());

            //get all items that are ready to be dequeued by frames
            while (!delayedByFrames.IsEmpty && delayedByFrames.Peek().Key < time.frameCount)
                running.AddLast(delayedByFrames.DequeueValue());

            //process items
            var fiberNode = running.First;
            while (fiberNode != null)
            {
                var fiber = fiberNode.Value;
                var result = fiber.tick(); //process micro thread

                var nextNode = fiberNode.Next;
                var shouldRemove = fiber.completed; 

                //if the micro thread has completed, lets remove it
                if (!shouldRemove && result != null)
                {
                    //assume match
                    shouldRemove = true;

                    //check the result to see if this micro thread should be rescheduled
                    if (result is TimeSpan)
                        schedule(fiber, (TimeSpan)result);
                    else if (result is float && (float)result > 0)
                        schedule(fiber, (float)result);
                    else if (result is double && (double)result > 0)
                        schedule(fiber, (double)result);
                    else if (result is int && (int)result > 0)
                        schedule(fiber, (int)result);
                    else
                        //no matches, lets not delay it
                        shouldRemove = false;
                }

                //micro thread shouldn't be requeued, lets remove it
                if (shouldRemove)
                    running.Remove(fiberNode);

                //move on
                fiberNode = nextNode;
            }

        }

        public void schedule(IFiber fiber)
        {
            running.AddLast(fiber);
        }

        public void schedule(IFiber fiber, TimeSpan ts)
        {
            var time = CoreRegistry.require<Atma.Core.TimeBase>(Atma.Core.TimeBase.Uri);
            delayedByTime.Enqueue((float)(ts.TotalSeconds + time.gameTime), fiber);
        }

        public void schedule(IFiber fiber, float delay)
        {
            var time = CoreRegistry.require<Atma.Core.TimeBase>(Atma.Core.TimeBase.Uri);
            delayedByTime.Enqueue((float)(delay + time.gameTime), fiber);
        }

        public void schedule(IFiber fiber, double delay)
        {
            var time = CoreRegistry.require<Atma.Core.TimeBase>(Atma.Core.TimeBase.Uri);
            delayedByTime.Enqueue((float)(delay + time.gameTime), fiber);
        }

        public void schedule(IFiber fiber, int frameDelay)
        {
            var time = CoreRegistry.require<Atma.Core.TimeBase>(Atma.Core.TimeBase.Uri);
            delayedByFrames.Enqueue((int)(frameDelay + time.frameCount), fiber);
        }

    }

    /// <summary>
    /// Manages a series of micro threads (c# enumerators, one at a time)
    /// </summary>
    public class FiberChannel : IFiber
    {
        public int executions { get; private set; }
        public bool completed { get; private set; }

        private List<IFiber> fibers = new List<IFiber>();
        private IFiber current = null;

        public void append(IFiber fiber)
        {
            fibers.Add(fiber);
        }

        public object tick()
        {
            //process the next fiber in the chain
            if (current == null || current.completed)
            {
                current = fibers.FirstOrDefault(x => !x.completed);
                if (current == null)
                {
                    completed = true;
                    return null;
                }
            }

            executions++;
            return current.tick();
        }

    }

    /// <summary>
    /// Enumeration manager
    /// </summary>
    public class Fiber : IFiber
    {
        public int executions { get; private set; }
        public bool completed { get; private set; }

        private IEnumerator routine;

        public Fiber(IEnumerator e)
        {
            this.routine = e;
        }

        /// <summary>
        /// Processes the next enumeration, if false the fiber is considered completed
        /// </summary>
        /// <returns></returns>
        public object tick()
        {
            if (!routine.MoveNext())
            {
                completed = true;
                return null;
            }

            executions++;
            return routine.Current;
        }
    }

}
