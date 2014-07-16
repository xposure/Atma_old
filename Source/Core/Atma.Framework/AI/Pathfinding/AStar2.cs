using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Collections;

namespace Atma.AI.Pathfinding
{
    //public enum CellType : int
    //{
    //    EMPTY = 0,
    //    ROOM = 1,
    //    CORRIDOR = 2,
    //    PERIMITER = 3,
    //    ENTRANCE = 4,
    //    WALL = 5,
    //}


    public abstract class AStar2
    {
        //public Func<int, int, int> getScore;
        //public Action<int, List<int>> getNeighbors;

        private struct cell
        {
            public int parent;
            public int internalid;
            public int id;
            public int g, f, h;
            public bool visited;
            public bool closed;
        }

        //private ObjectPool<cell> cellPool = new ObjectPool<cell>(1024);
        private List<cell> cells = new List<cell>(1024);
        private Dictionary<int, int> cellIdMap = new Dictionary<int, int>(1024);
        private PriorityQueue<int, int> openList = new PriorityQueue<int, int>(1024);
        private List<int> getNeighborsCache = new List<int>(8);
        public bool foundPath { get; private set; }

        private int start, end;

        public void init(int start, int end)
        {
            this.start = start;
            this.end = end;
            this.foundPath = false;

            cells.Clear();
            cellIdMap.Clear();
            openList.Clear();

            if (start != end)
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
                src.h = score.Key;// (Math.Abs(Row - dst.Row) + Math.Abs(Col - dst.Col)) * 10;
                src.g = visitor.g + score.Value;
                src.f = src.h + src.g;
                src.parent = visitor.internalid;
            }

            return src;
        }

        private int create(int val)
        {
            var c = new cell();
            c.internalid = cells.Count;
            c.parent = -1;
            c.id = val;

            cells.Add(c);
            cellIdMap.Add(val, c.internalid);

            return c.internalid;
        }

        public List<int> getPath()
        {
            if (!foundPath)
                return null;

            var path = new List<int>();
            var current = cells[1];

            path.Add(current.id);
            while (current.parent > -1)
            {
                current = cells[current.parent];
                path.Add(current.id);
            }

            return path;
        }

        protected abstract KeyValuePair<int, int> getScore(int src, int dst);

        protected abstract void getNeighbors(int src, List<int> moves);
    }
}
