using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Samples.BulletHell.World.Generators
{

    public abstract class BaseLevel : GameSystem, ILevel
    {
        protected CellType[,] cells = null;

        public int Width { get; protected set; }
        public int Height { get; protected set; }

        protected void Resize()
        {
            cells = new CellType[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                cells[x, 0] = CellType.PERIMITER;
                cells[x, Height - 1] = CellType.PERIMITER;
            }
            for (int y = 0; y < Height; y++)
            {
                cells[0, y] = CellType.PERIMITER;
                cells[Width - 1, y] = CellType.PERIMITER;
            }
        }

        public CellType GetCell(int col, int row)
        {
            if (col < 0 || row < 0 || col >= Width || row >= Height)
                return CellType.EMPTY;

            return cells[col, row];
        }

        public void Generate()
        {
            Generate((new Random()).Next());
        }

        public virtual void Reset()
        {
            Resize();
        }

        public abstract void Generate(int seed);

        //public void Build()
        //{
        //    Region region = Region.Create("New Region", Width, Height);

        //    //Room[,] rooms = new Room[Width,Height];
        //    for (int y = 0; y < Height; y++)
        //        for (int x = 0; x < Width; x++)
        //            Room.Create(region, string.Format("Room [{0}, {1}]", x, y), "", cells[x, y], x, y);
        //    //rooms[x, y] = room;

        //}
    }
}
