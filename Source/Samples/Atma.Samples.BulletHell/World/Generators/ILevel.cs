using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Samples.BulletHell.World.Generators
{
    public interface ILevel
    {
        int Width { get; }
        int Height { get; }
        CellType GetCell(int col, int row);
        void Generate();
        void Generate(int seed);

    }
}
