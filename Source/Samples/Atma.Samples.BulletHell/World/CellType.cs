using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Samples.BulletHell.World
{
    public enum CellType : int
    {
        EMPTY = 0,
        ROOM = 1,
        CORRIDOR = 2,
        PERIMITER = 3,
        ENTRANCE = 4,
        WALL = 5,
    }
}
