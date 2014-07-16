using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Samples.BulletHell.World.Generators
{
    public class Town : BaseLevel
    {
        protected LinkedList<Block> blocks = new LinkedList<Block>();
        protected class Block
        {

            public Town Town { get; private set; }
            public Block(Town town, int x, int y, int width, int height)
            {
                Town = town;
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            public int X { get; private set; }
            public int Y { get; private set; }
            public int Width { get; private set; }
            public int Height { get; private set; }
            public int Right { get { return X + Width - 1; } }
            public int Bottom { get { return Y + Height - 1; } }

            public int Size { get { return Width * Height; } }

            public bool CanBeDivided { get { return Width > minSize || Height > minSize; } }

            private static int minSize = 12;
            public Block[] Subdivide(Random r)
            {
                if (!CanBeDivided)
                    return null;

                int x = 0;
                int y = 0;

                //if (Width > minSize)
                //    x = r.Next(5, Width - 5);

                //if (Height > minSize)
                //    y = r.Next(5, Height - 5);

                int x_displacement = (int)(Width * 0.15);
                int x_center = Width / 2;

                int y_displacement = (int)(Height * 0.15);
                int y_center = Height / 2;

                if (Width > minSize)
                    x = r.Next(x_center - x_displacement, x_center + x_displacement);

                if (Height > minSize)
                    y = r.Next(y_center - y_displacement, y_center + y_displacement);

                return Subdivide(x, y);
            }

            private Block[] Subdivide(int x, int y)
            {
                if (x == 0 && y == 0)
                    throw new Exception("Room can not be sub divided any more");

                if (x > 0 && y == 0)
                    return new Block[] { 
                        Create(0, 0, x - 1, Height), 
                        Create(x, 0, Width - x, Height)
                        //Create(0, y , x - 1, Height - y),
                        //Create(x, y , Width - x , Height - y)
                };

                else if (y > 0 && x == 0)

                    return new Block[] { 
                    Create(0, 0, Width, y - 1), 
                    //Create(x, 0, Width - x, y - 1),                    
                    Create(0, y , Width, Height - y)
                    //Create(x, y , Width - x , Height - y)
                };
                else
                    return new Block[] { 
                    Create(0, 0, x - 1, y - 1), 
                    Create(x, 0, Width - x, y - 1),                    
                    Create(0, y , x - 1, Height - y),
                    Create(x, y , Width - x , Height - y)
                };

            }

            public void BuildRooms(Random r)
            {

                int x = 0; // Width - 1;
                int y = 0; //Height - 1;

                //if (Width > 6)
                //    x = r.Next(3, Width - 4);

                //if (Height > 6)
                //    y = r.Next(3, Height - 4);

                int x_displacement = (int)(Width * 0.10);
                int x_center = Width / 2;

                if (Width > 6)
                    x = r.Next(x_center - x_displacement, x_center + x_displacement);

                int y_displacement = (int)(Height * 0.10);
                int y_center = Height / 2;

                if (Height > 6)
                    y = r.Next(y_center - y_displacement, y_center + y_displacement);

                Block b = new Block(Town, X + 1, Y + 1, Width - 2, Height - 2);

                Block[] blocks;
                if (x > 0 || y > 0)
                    blocks = b.Subdivide(x, y);
                else
                    blocks = new Block[] { b };

                //System.Diagnostics.Debug.WriteLine(blocks.Length.ToString());

                markCells(CellType.WALL);
                if (blocks.Length > 2)
                {
                    switch (r.Next(0, 4))
                    {
                        case 0: swapBlock(0, 1, blocks); break;
                        case 1: swapBlock(2, 3, blocks); break;
                        case 2: swapBlock(0, 2, blocks); break;
                        case 3: swapBlock(1, 3, blocks); break;
                    }

                    switch (r.Next(0, 8))
                    {
                        case 0: blocks[0].mergeVertBlock(blocks[2]); break;
                        case 1: blocks[1].mergeVertBlock(blocks[3]); break;
                        case 2: blocks[0].mergeHortBlock(blocks[1]); break;
                        case 3: blocks[2].mergeHortBlock(blocks[3]); break;
                        case 4: blocks[0].mergeHortBlock(blocks[1]); goto case 0;
                        case 5: blocks[0].mergeHortBlock(blocks[1]); goto case 1;
                        case 6: blocks[2].mergeHortBlock(blocks[3]); goto case 0;
                        case 7: blocks[2].mergeHortBlock(blocks[3]); goto case 1;

                    }
                    //    blocks[0].markCells(CellType.CORRIDOR);
                    //    blocks[1].markCells(CellType.ENTRANCE);
                    //    blocks[2].markCells(CellType.PERIMITER);
                    //    blocks[3].markCells(CellType.WALL);


                }
                else
                {

                }
                foreach (Block block in blocks)
                    block.markCells(CellType.ROOM);

                if (blocks.Length > 2)
                {
                    addDoor(r, blocks[0], Direction.TOP_LEFT);
                    addDoor(r, blocks[1], Direction.TOP_RIGHT);
                    addDoor(r, blocks[2], Direction.BOTTOM_LEFT);
                    addDoor(r, blocks[3], Direction.BOTTOM_RIGHT);
                }
                else if (blocks.Length == 2)
                {
                    addDoor(r, blocks[0], Direction.TOP_LEFT);
                    addDoor(r, blocks[1], Direction.BOTTOM_RIGHT);
                }
                else if (blocks.Length == 1)
                {
                    switch (r.Next(0, 4))
                    {
                        case 0: addDoor(r, blocks[0], Direction.TOP_LEFT); break;
                        case 1: addDoor(r, blocks[0], Direction.TOP_RIGHT); break;
                        case 2: addDoor(r, blocks[0], Direction.BOTTOM_LEFT); break;
                        case 3: addDoor(r, blocks[0], Direction.BOTTOM_RIGHT); break;
                    }
                }

                //}

            }

            private enum Direction : int
            {
                TOP_LEFT,
                BOTTOM_LEFT,
                TOP_RIGHT,
                BOTTOM_RIGHT
            }

            private void addDoor(Random r, Block b, Direction dir)
            {
                switch (dir)
                {
                    case Direction.TOP_LEFT:
                        if (r.Next(0, 2) == 0)
                            addDoor(r, b.X, b.Y - 1, b.Right, b.Y - 1);
                        else
                            addDoor(r, b.X - 1, b.Y, b.X - 1, b.Bottom);
                        break;
                    case Direction.TOP_RIGHT:
                        if (r.Next(0, 2) == 0)
                            addDoor(r, b.X, b.Y - 1, b.Right, b.Y - 1);
                        else
                            addDoor(r, b.Right + 1, b.Y, b.Right + 1, b.Bottom);
                        break;
                    case Direction.BOTTOM_LEFT:
                        if (r.Next(0, 2) == 0)
                            addDoor(r, b.X, b.Bottom + 1, b.Right, b.Bottom + 1);
                        else
                            addDoor(r, b.X - 1, b.Y, b.X - 1, b.Bottom);
                        break;
                    case Direction.BOTTOM_RIGHT:
                        if (r.Next(0, 2) == 0)
                            addDoor(r, b.X, b.Bottom + 1, b.Right, b.Bottom + 1);
                        else
                            addDoor(r, b.Right + 1, b.Y, b.Right + 1, b.Bottom);
                        break;
                }
            }

            private void addDoor(Random r, int x1, int y1, int x2, int y2)
            {
                int size = ((x2 - x1) + 1) * ((y2 - y1) + 1);
                size = r.Next(0, size);

                for (int y = y1; y <= y2; y++)
                    for (int x = x1; x <= x2; x++)
                        if ((--size) <= 0)
                        {
                            Town.cells[x, y] = CellType.ENTRANCE;
                            return;
                        }
            }

            private void swapBlock(int index1, int index2, Block[] blocks)
            {
                Block b = blocks[index2];

                int x = b.X, y = b.Y;

                b.X = blocks[index1].X;
                b.Y = blocks[index1].Y;

                blocks[index1].X = x - (blocks[index1].Width - b.Width);
                blocks[index1].Y = y - (blocks[index1].Height - b.Height);

                blocks[index2] = blocks[index1];
                blocks[index1] = b;

            }

            private void markCells(CellType ct)
            {
                Town.setCells(X, Y, Right, Bottom, ct);
            }


            private void mergeVertBlock(Block block)
            {
                if (Width <= block.Width)
                {
                    if (Y > block.Y)
                        Y -= 1;

                    Height += 1;
                }
                else
                {
                    if (Y < block.Y)
                        block.Y -= 1;

                    block.Height += 1;
                }
            }

            private void mergeHortBlock(Block block)
            {
                if (Height <= block.Height)
                {
                    if (X > block.X)
                        X -= 1;

                    Width += 1;
                }
                else
                {
                    if (X < block.X)
                        block.X -= 1;

                    block.Width += 1;
                }
            }


            private Block Create(int x, int y, int w, int h)
            {
                return new Block(Town, X + x, Y + y, w, h);
            }

            public override string ToString()
            {
                return string.Format("X: {0}, Y: {1}, W: {2}, H: {3}", X, Y, Width, Height);
            }
        }

        //private CellType[,] cells;

        public Town(int width, int height)
        {
            Width = width;
            Height = height;
            //cells = new CellType[height, width];
        }

        //protected override void Resize()
        //{
        //    cells = new CellType[Height, Width];
        //}

        //public int Width { get; private set; }
        //public int Height { get; private set; }
        //public override CellType GetCell(int row, int col)
        //{
        //    return cells[row, col];
        //}

        //public void Generate()
        //{
        //    Generate((new Random()).Next());
        //}

        public override void Generate(int seed)
        {
            Resize();

            Random r = new Random(seed);

            blocks.Clear();
            setCells(0, 0, Width - 1, Height - 1, CellType.CORRIDOR);
            blocks.AddFirst(new Block(this, 1, 1, Width - 2, Height - 2));

            //blocks.AddFirst(new Block(this, 1, 1, (Width / 2) - 2, (Height / 2) - 2));
            //blocks.AddFirst(new Block(this, Width / 2 + 1, 1, (Width / 2) - 2, (Height / 2) - 2));
            //blocks.AddFirst(new Block(this, 1, (Height / 2) + 1, (Width / 2) - 2, (Height / 2) - 2));
            //blocks.AddFirst(new Block(this, Width / 2 + 1, (Height / 2) + 1, (Width / 2) - 2, (Height / 2) - 2));
            //blocks.AddFirst(new Block(this, 1, Height / 2, (Width / 2) - 1, Height / 2));
            //blocks.AddFirst(new Block(this, Width / 2, Height / 2, (Width / 2) - 1, Height / 2));

            while (true)
            {

                Block largest = null;
                foreach (Block block in blocks)
                    if (largest == null || block.Size > largest.Size)
                        largest = block;

                //were done if this is true
                if (!largest.CanBeDivided)
                    break;

                Block[] rs = largest.Subdivide(r);
                blocks.Remove(largest);
                foreach (Block block in rs)
                {
                    blocks.AddLast(block);
                    System.Diagnostics.Debug.WriteLine(block.ToString());
                }

                //rooms.Remove(rs[0]);
                //rs = rs[0].Subdivide(r);
                //foreach (Room room in rs)
                //    rooms.AddLast(room);

            }

            foreach (Block block in blocks)
                block.BuildRooms(r);
        }


        protected void setCells(int x1, int y1, int x2, int y2, CellType cell)
        {
            for (int x = x1; x <= x2; x++)
                for (int y = y1; y <= y2; y++)
                    cells[x, y] = cell;
        }



    }
}
