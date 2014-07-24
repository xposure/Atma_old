using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Samples.BulletHell.World.Generators
{
    public class TestLevel : BaseLevel
    {
        private int rnw, rmw, rnh, rmh;

        public int RoomMinWidth { get { return rnw; } set { rnw = value; } }

        public int RoomMaxWidth { get { return rmw; } set { rmw = value; } }

        public int RoomMinHeight { get { return rnh; } set { rnh = value; } }

        public int RoomMaxHeight { get { return rmh; } set { rmh = value; } }

        public TestLevel(int width, int height, int roomMinWidth, int roomMaxWidth, int roomMinHeight, int roomMaxHeight)
            : base()
        {
            Width = width | 1;
            Height = height | 1;
            RoomMinWidth = roomMinWidth | 1;
            RoomMaxWidth = roomMaxWidth | 1;
            RoomMinHeight = roomMinHeight | 1;
            RoomMaxHeight = roomMaxHeight | 1;
        }

        public override void Generate(int seed)
        {
            for (var i = 0; i < 5; i++)
            {

            }
        }
    }
}
