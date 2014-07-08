using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Collections;

namespace Atma.AI.Pathfinding
{
    public struct GenericAStarScore
    {
        public int g, h;
    }

    public abstract class GenericAStar<T>
        where T : IEquatable<T>
    {
        //public Func<int, int, int> getScore;
        //public Action<int, List<int>> getNeighbors;

        public struct cell
        {
            public int parent;
            public int internalid;
            public T id;
            public int g, f, h;
            public bool visited;
            public bool closed;
        }

        //private ObjectPool<cell> cellPool = new ObjectPool<cell>(1024);
        private List<cell> cells = new List<cell>(1024);
        private Dictionary<T, int> cellIdMap = new Dictionary<T, int>(1024);
        private PriorityQueue<int, int> openList = new PriorityQueue<int, int>(1024);
        private List<T> getNeighborsCache = new List<T>(8);
        public bool foundPath { get; private set; }

        protected T start, end;

        public void init(T start, T end)
        {
            this.start = start;
            this.end = end;
            this.foundPath = false;

            cells.Clear();
            cellIdMap.Clear();
            openList.Clear();

            if (!start.Equals(end))
            {
                create(start);
                create(end);

                var cell = cells[0];
                cell.visited = true;
                cells[0] = cell;

                openList.Add(0, 0);
            }
        }

        public bool step(int numOfSteps)
        {
            while (numOfSteps-- > 0 && openList.Count > 0 && !foundPath)
            {
                var nextid = openList.DequeueValue();
                visitCell(nextid);
            }

            return foundPath || openList.Count == 0;
        }

        private void visitCell(int cellid)
        {
            var visitor = cells[cellid];
            //Console.WriteLine("Visited: {{x: {0}, y: {1}, h: {2}, g: {3}, f: {4}}}", visitor.id >> 16, visitor.id & 0xffff, visitor.h, visitor.h, visitor.f);

            getNeighborsCache.Clear();
            getNeighbors(visitor.id, getNeighborsCache);
            for (var i = 0; i < getNeighborsCache.Count; i++)
            {
                var id = getNeighborsCache[i];
                var internalid = 0;
                cell _cell;

                if (!cellIdMap.TryGetValue(id, out internalid))
                    internalid = create(id);

                _cell = cells[internalid];
                if (!_cell.closed)
                {
                    _cell = computeScore(_cell, cells[1], visitor);
                    if (_cell.h == 0)
                    {
                        foundPath = true;
                        cells[internalid] = _cell;
                        break;
                    }

                    if (!_cell.visited)
                    {
                        openList.Add(_cell.f, _cell.internalid);
                        _cell.visited = true;
                    }
                    cells[internalid] = _cell;
                    //Console.WriteLine("--Visited: {{x: {0}, y: {1}, h: {2}, g: {3}, f: {4}}}", _cell.id >> 16, _cell.id & 0xffff, _cell.h, _cell.h, _cell.f);
                }
            }
            visitor.closed = true;
            cells[cellid] = visitor;
        }

        private cell computeScore(cell src, cell dst, cell visitor)
        {
            if (src.parent == -1 || visitor.g < cells[src.parent].g)
            {
                var score = getScore(src.id, dst.id);
                src.h = score.h;// (Math.Abs(Row - dst.Row) + Math.Abs(Col - dst.Col)) * 10;
                src.g = visitor.g + score.g;
                src.f = src.h + src.g;
                src.parent = visitor.internalid;
            }

            return src;
        }

        private int create(T val)
        {
            var c = new cell();
            c.internalid = cells.Count;
            c.parent = -1;
            c.id = val;

            cells.Add(c);
            cellIdMap.Add(val, c.internalid);

            return c.internalid;
        }

        public List<T> getPath()
        {
            if (!foundPath)
                return null;

            var path = new List<T>();
            var current = cells[1];

            path.Add(current.id);
            while (current.parent > -1)
            {
                current = cells[current.parent];
                path.Add(current.id);
            }

            return path;
        }

        public cell? getCell(T t)
        {
            var id = 0;
            if (cellIdMap.TryGetValue(t, out id))
            {
                return cells[id];
            }
            return null;
        }

        protected abstract GenericAStarScore getScore(T src, T dst);

        protected abstract void getNeighbors(T src, List<T> moves);
    }
}
