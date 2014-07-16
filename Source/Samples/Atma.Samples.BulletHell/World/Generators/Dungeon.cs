using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Samples.BulletHell.World.Generators
{
    public class Dungeon : BaseLevel
    {
        //private CellType[,] cells = null;
        public Dungeon(int width, int height, int roomMinWidth, int roomMaxWidth, int roomMinHeight, int roomMaxHeight)
        {
            Width = width | 1;
            Height = height | 1;
            RoomMinWidth = roomMinWidth | 1;
            RoomMaxWidth = roomMaxWidth | 1;
            RoomMinHeight = roomMinHeight | 1;
            RoomMaxHeight = roomMaxHeight | 1;
        }

        private int rnw, rmw, rnh, rmh;

        //public int Width { get { return w; } set { w = value; } }
        //public int Height { get { return h; } set { h = value; } }

        public int RoomMinWidth { get { return rnw; } set { rnw = value; } }

        public int RoomMaxWidth { get { return rmw; } set { rmw = value; } }

        public int RoomMinHeight { get { return rnh; } set { rnh = value; } }

        public int RoomMaxHeight { get { return rmh; } set { rmh = value; } }

        //public void Generate()
        //{
        //    Generate(75, (new Random()).Next());
        //}

        public override void Generate(int seed)
        {
            Generate(75, seed);
        }

        public void Generate(int deadEndRemoval, int seed)
        {
            Generate(deadEndRemoval, seed, 5, 15);
        }

        public void Generate(int deadEndRemoval, int corridorMin, int corridorMax)
        {
            Generate(deadEndRemoval, (new Random()).Next(), corridorMin, corridorMax);
        }

        public void Generate(int deadEndRemoval, int seed, int corridorMin, int corridorMax)
        {
            Resize();
            Random r = new Random(seed);
            //              (dungeon_cols * dungeon_rows)
            //n_rooms  =  ----------------------------------  *  2
            //            (max_room_width * max_room_height)
            //PlaceRoom(3, 3, 5, 5, 10, r);
            //return;

            int n_rooms = ((Width * Height) / (RoomMaxWidth * RoomMaxHeight));
            int i = 0;
            int placeAttempts = n_rooms; //n_rooms * 20;
            while (i < n_rooms && placeAttempts > 0)
            {
                int x = r.Next(Width - 2);
                int y = r.Next(Height - 2);
                int width = r.Next(RoomMinWidth, RoomMaxWidth);
                int height = r.Next(RoomMinHeight, RoomMaxHeight);
                //flumph       =  int(sqrt(room_width * room_height))
                //n_entrances  =  flumph + int(rand(flumph))
                int flumph = (int)Utility.Sqrt(width * height);
                int entrances = (flumph + r.Next(flumph));
                //make sure they are odd
                x |= 1;
                y |= 1;
                width = width | 1;
                height = height | 1;
                if (PlaceRoom(x, y, width, height, entrances, r))
                    i++;
                else
                    placeAttempts--;
            }
            GenerateCorridors(r);
            ////RemoveDeadEnds();
            ////GenerateCorridors(r);
            ////RemoveDeadEnds();
            ////GenerateCorridors(r);
            RemoveDeadEnds(deadEndRemoval, r);
            RemoveUnsedEntrances();
            wallCorridors();
        }

        private void wallCorridors()
        {
            for (int x = 1; x < Width - 1; x++)
                for (int y = 1; y < Height - 1; y++)
                    if (cells[x, y] == CellType.CORRIDOR)
                    {
                        if (cells[x - 1, y] == CellType.EMPTY)
                            cells[x - 1, y] = CellType.PERIMITER;
                        if (cells[x + 1, y] == CellType.EMPTY)
                            cells[x + 1, y] = CellType.PERIMITER;
                        if (cells[x, y - 1] == CellType.EMPTY)
                            cells[x, y - 1] = CellType.PERIMITER;
                        if (cells[x, y + 1] == CellType.EMPTY)
                            cells[x, y + 1] = CellType.PERIMITER;

                        if (cells[x - 1, y - 1] == CellType.EMPTY)
                            cells[x - 1, y - 1] = CellType.PERIMITER;
                        if (cells[x + 1, y + 1] == CellType.EMPTY)
                            cells[x + 1, y + 1] = CellType.PERIMITER;
                        if (cells[x + 1, y - 1] == CellType.EMPTY)
                            cells[x + 1, y - 1] = CellType.PERIMITER;
                        if (cells[x - 1, y + 1] == CellType.EMPTY)
                            cells[x - 1, y + 1] = CellType.PERIMITER;

                    }
        }

        private bool PlaceRoom(int start_x, int start_y, int width, int height, int entrances, Random r)
        {
            int end_x = start_x + width - 1;
            int end_y = start_y + height - 1;
            if (end_x >= Width || end_y >= Height)
                return false;
            for (int y = start_y; y < end_y; y++)
                for (int x = start_x; x < end_x; x++)
                    if (cells[x, y] != CellType.EMPTY)
                        return false;

            for (int y = start_y - 1; y < end_y + 2; y++)
                for (int x = start_x - 1; x < end_x + 2; x++)
                {
                    if (x < start_x || x > end_x || y < start_y || y > end_y)
                        //only set it to perimiter if its a empty cell
                        //cells[x, y] = CellType.PERIMITER;
                        cells[x, y] = cells[x, y] == CellType.EMPTY ? CellType.PERIMITER : cells[x, y];
                    else
                        cells[x, y] = CellType.ROOM;
                    //continue;
                }

            //return true;
            int placeAttempts = entrances;// *2;
            while (entrances > 0 && placeAttempts > 0)
            {
                //bool coord = r.Next(1) == 1;
                int side = r.Next(0, 4);
                int x = 0;
                int y = 0;

                //north
                if (side == 0)
                {
                    x = r.Next(start_x, end_x) | 1;
                    y = start_y - 1;
                    setCellsHort(start_x, end_x, y, CellType.PERIMITER);
                }
                //east
                else if (side == 1)
                {
                    x = end_x + 1;
                    y = r.Next(start_y, end_y) | 1;
                    setCellsVert(x, start_y, end_y, CellType.PERIMITER);
                }
                //south
                else if (side == 2)
                {
                    x = r.Next(start_x, end_x) | 1;
                    y = end_y + 1;
                    setCellsHort(start_x, end_x, y, CellType.PERIMITER);
                }
                //west
                else if (side == 3)
                {
                    x = start_x - 1;
                    y = r.Next(start_y, end_y) | 1;
                    setCellsVert(x, start_y, end_y, CellType.PERIMITER);
                }
                else
                    throw new Exception();

                if (cells[x, y] == CellType.PERIMITER && !(x == 0 || y == 0 || x == Width - 1 || y == Height - 1))
                {
                    cells[x, y] = CellType.ENTRANCE;
                    entrances--;
                }
                else
                    placeAttempts--;
            }
            return true;
        }

        private void GenerateCorridors(Random r)
        {
            for (int y = 1; y < Height - 1; y += 2)
                for (int x = 1; x < Width - 1; x += 2)
                {
                    corridorLength = r.Next(15, 25);
                    //runCorridor2(x, y, r, 40, corridorLength);
                    runCorridor(x, y, r);
                }
        }

        private enum Direction : int
        {
            North = 0,
            South = 1,
            East = 2,
            West = 3
        }

        private bool runCorridor2(int x, int y, Random r, int turnrate, int length)
        {
            if ((x & 1) == 0 || (y & 1) == 0)
                throw new Exception();
            //if (cells[x, y] == CellType.ENTRANCE)
            //    return true;
            if (cells[x, y] != CellType.EMPTY)
                return false;


            Direction[] dirs = new Direction[length];// { Direction.North, Direction.South, Direction.East, Direction.West };
            dirs[0] = (Direction)r.Next(0, 3);
            for (int i = 1; i < length; i++)
            {
                if (r.Next(0, 100) <= turnrate)
                {
                    dirs[i] = (Direction)r.Next(0, 3);
                    while (dirs[i - 1] != dirs[i])
                        dirs[i] = (Direction)r.Next(0, 3);
                }
                else
                    dirs[i] = dirs[i - 1];
            }
            
            //int shuffleCount = r.Next(3, 7);
            //for (int i = 0; i < shuffleCount; i++)
            //{
            //    int shuffleSrc = r.Next(0, 4);
            //    int shuffleDst = shuffleSrc;
            //    while (shuffleSrc == shuffleDst)
            //        shuffleDst = r.Next(0, 3);
            //    Direction tmp = dirs[shuffleDst];
            //    dirs[shuffleDst] = dirs[shuffleSrc];
            //    dirs[shuffleSrc] = tmp;
            //}
            ////Console.WriteLine("{0} {1} {2} {3}", dirs[0], dirs[1], dirs[2], dirs[3]);
            for (int i = 0; i < length; i++)
            {
                cells[x, y] = CellType.CORRIDOR;
                switch (dirs[i])
                {
                    case Direction.North:
                        //if (cells[x, y - 1] == CellType.ENTRANCE && r.Next(0, 100) > 50)
                        //    return true;
                        if (y > 1 && cells[x, y - 2] == CellType.EMPTY)
                        {
                            cells[x, --y] = CellType.CORRIDOR;
                            y--;
                        }
                        break;
                    case Direction.South:
                        //if (cells[x, y + 1] == CellType.ENTRANCE && r.Next(0, 100) > 50)
                        //    return true;
                        if (y < Height - 2 && cells[x, y + 2] == CellType.EMPTY)
                        {
                            cells[x, ++y] = CellType.CORRIDOR;
                            y++;
                        }
                        break;
                    case Direction.East:
                        //if (cells[x + 1, y] == CellType.ENTRANCE && r.Next(0, 100) > 50)
                        //    return true;
                        if (x < Width - 2 && cells[x + 2, y] == CellType.EMPTY)
                        {
                            cells[++x , y] = CellType.CORRIDOR;
                            x++;
                        }
                        break;
                    case Direction.West:
                        //if (cells[x - 1, y] == CellType.ENTRANCE && r.Next(0, 100) > 50)
                        //    return true;
                        if (x > 1 && cells[x - 2, y] == CellType.EMPTY)
                        {
                            cells[--x , y] = CellType.CORRIDOR;
                            x--;
                        }
                        break;
                }
            }
            return false;
        }


        private int corridorLength = 0;
        private bool runCorridor(int x, int y, Random r)
        {
            if ((x & 1) == 0 || (y & 1) == 0)
                throw new Exception();
            //if (cells[x, y] == CellType.ENTRANCE)
            //    return true;
            if (cells[x, y] != CellType.EMPTY)
                return false;
            corridorLength--;
            if (corridorLength == 0)
                return true;
            cells[x, y] = CellType.CORRIDOR;
            Direction[] dirs = new Direction[] { Direction.North, Direction.South, Direction.East, Direction.West };
            int shuffleCount = r.Next(3, 7);
            for (int i = 0; i < shuffleCount; i++)
            {
                int shuffleSrc = r.Next(0, 4);
                int shuffleDst = shuffleSrc;
                while (shuffleSrc == shuffleDst)
                    shuffleDst = r.Next(0, 3);
                Direction tmp = dirs[shuffleDst];
                dirs[shuffleDst] = dirs[shuffleSrc];
                dirs[shuffleSrc] = tmp;
            }
            //Console.WriteLine("{0} {1} {2} {3}", dirs[0], dirs[1], dirs[2], dirs[3]);
            for (int i = 0; i < 4; i++)
            {
                switch (dirs[i])
                {
                    case Direction.North:
                        //if (cells[x, y - 1] == CellType.ENTRANCE && r.Next(0, 100) > 50)
                        //    return true;
                        if (y > 1 && cells[x, y - 2] == CellType.EMPTY)
                        {
                            cells[x, y - 1] = CellType.CORRIDOR;
                            if (runCorridor(x, y - 2, r))
                                return true;
                        }
                        break;
                    case Direction.South:
                        //if (cells[x, y + 1] == CellType.ENTRANCE && r.Next(0, 100) > 50)
                        //    return true;
                        if (y < Height - 2 && cells[x, y + 2] == CellType.EMPTY)
                        {
                            cells[x, y + 1] = CellType.CORRIDOR;
                            if (runCorridor(x, y + 2, r))
                                return true;
                        }
                        break;
                    case Direction.East:
                        //if (cells[x + 1, y] == CellType.ENTRANCE && r.Next(0, 100) > 50)
                        //    return true;
                        if (x < Width - 2 && cells[x + 2, y] == CellType.EMPTY)
                        {
                            cells[x + 1, y] = CellType.CORRIDOR;
                            if (runCorridor(x + 2, y, r))
                                return true;
                        }
                        break;
                    case Direction.West:
                        //if (cells[x - 1, y] == CellType.ENTRANCE && r.Next(0, 100) > 50)
                        //    return true;
                        if (x > 1 && cells[x - 2, y] == CellType.EMPTY)
                        {
                            cells[x - 1, y] = CellType.CORRIDOR;
                            if (runCorridor(x - 2, y, r))
                                return true;
                        }
                        break;
                }
            }
            return false;
        }

        private void RemoveUnsedEntrances()
        {
            for (int y = 1; y < Height - 1; y++)
                for (int x = 1; x < Width - 1; x++)
                    if (cells[x, y] == CellType.ENTRANCE)
                    {
                        int conns = 0;
                        if (cells[x, y + 1] == CellType.CORRIDOR || cells[x, y + 1] == CellType.ROOM)
                            conns++;
                        if (cells[x, y - 1] == CellType.CORRIDOR || cells[x, y - 1] == CellType.ROOM)
                            conns++;
                        if (cells[x + 1, y] == CellType.CORRIDOR || cells[x + 1, y] == CellType.ROOM)
                            conns++;
                        if (cells[x - 1, y] == CellType.CORRIDOR || cells[x - 1, y] == CellType.ROOM)
                            conns++;

                        if (conns < 2)
                            cells[x, y] = CellType.PERIMITER;
                    }
        }

        private void RemoveDeadEnds(int removeChance, Random r)
        {
            for (int y = 1; y < Height - 1; y++)
                for (int x = 1; x < Width - 1; x++)
                    if (!RemoveIfIsolated(x, y) && IsDead(x, y) && r.Next(0, 100) < removeChance)
                        RemoveDeadEnd(x, y);
        }

        private bool RemoveIfIsolated(int x, int y)
        {
            if (cells[x, y] != CellType.CORRIDOR)
                return false;
            int sides = 0;
            sides += cells[x, y - 1] == CellType.CORRIDOR ? 1 : 0;
            sides += cells[x, y + 1] == CellType.CORRIDOR ? 1 : 0;
            sides += cells[x - 1, y] == CellType.CORRIDOR ? 1 : 0;
            sides += cells[x + 1, y] == CellType.CORRIDOR ? 1 : 0;
            sides += cells[x, y - 1] == CellType.ROOM ? 1 : 0;
            sides += cells[x, y + 1] == CellType.ROOM ? 1 : 0;
            sides += cells[x - 1, y] == CellType.ROOM ? 1 : 0;
            sides += cells[x + 1, y] == CellType.ROOM ? 1 : 0;
            sides += cells[x, y - 1] == CellType.ENTRANCE ? 1 : 0;
            sides += cells[x, y + 1] == CellType.ENTRANCE ? 1 : 0;
            sides += cells[x - 1, y] == CellType.ENTRANCE ? 1 : 0;
            sides += cells[x + 1, y] == CellType.ENTRANCE ? 1 : 0;
            if (sides == 0)
                cells[x, y] = CellType.EMPTY;
            return sides == 0;
        }

        private bool IsDead(int x, int y)
        {
            if (cells[x, y] != CellType.CORRIDOR)
                return false;
            int sides = 0;
            sides += cells[x, y - 1] == CellType.CORRIDOR ? 1 : 0;
            sides += cells[x, y + 1] == CellType.CORRIDOR ? 1 : 0;
            sides += cells[x - 1, y] == CellType.CORRIDOR ? 1 : 0;
            sides += cells[x + 1, y] == CellType.CORRIDOR ? 1 : 0;
            sides += cells[x, y - 1] == CellType.ROOM ? 1 : 0;
            sides += cells[x, y + 1] == CellType.ROOM ? 1 : 0;
            sides += cells[x - 1, y] == CellType.ROOM ? 1 : 0;
            sides += cells[x + 1, y] == CellType.ROOM ? 1 : 0;
            sides += cells[x, y - 1] == CellType.ENTRANCE ? 1 : 0;
            sides += cells[x, y + 1] == CellType.ENTRANCE ? 1 : 0;
            sides += cells[x - 1, y] == CellType.ENTRANCE ? 1 : 0;
            sides += cells[x + 1, y] == CellType.ENTRANCE ? 1 : 0;
            return sides < 2;
        }

        private void RemoveDeadEnd(int x, int y)
        {
            if (IsDead(x, y))
            {
                cells[x, y] = CellType.EMPTY;
                RemoveDeadEnd(x, y - 1);
                RemoveDeadEnd(x, y + 1);
                RemoveDeadEnd(x - 1, y);
                RemoveDeadEnd(x + 1, y);
            }
        }

        private void setCellsHort(int start_x, int end_x, int y, CellType cell)
        {
            for (int x = start_x; x <= end_x; x++)
                if (cells[x, y] != CellType.WALL)
                    cells[x, y] = cell;
        }

        private void setCellsVert(int x, int start_y, int end_y, CellType cell)
        {
            for (int y = start_y; y <= end_y; y++)
                if (cells[x, y] != CellType.WALL)
                    cells[x, y] = cell;
        }

        public override string ToString()
        {
            return ToString(1000);
        }

        //public CellType GetCell(int row, int col)
        //{
        //    return cells[col, row];
        //}

        public string ToString(int maxSize)
        {
            StringBuilder sb = new StringBuilder();
            //float cellWidth = (float)maxSize / (float)Width; // 50
            //float cellHeight = (float)maxSize / (float)Height; // 100
            //if (cellWidth > cellHeight)
            //{
            //    float aspectRatio = cellHeight / cellWidth;
            //    sb.Append(string.Format("<table width='{0}px' height='{1}px'>", maxSize, maxSize * aspectRatio));
            //}
            //else if (cellHeight > cellWidth)
            //{
            //    float aspectRatio = cellWidth / cellHeight;
            //    sb.Append(string.Format("<table width='{0}px' height='{1}px'>", maxSize, maxSize * aspectRatio));
            //}
            //else
            //    sb.Append(string.Format("<table width='{0}px' height='{1}px'>", maxSize, maxSize));
            sb.Append(string.Format("<table cellpadding=\"0\" cellspacing=\"0\" style=\"padding: 0px; margin: 0px;\" align=\"center\">"));
            for (int y = 0; y < Height; y++)
            {
                sb.Append("<tr>");
                for (int x = 0; x < Width; x++)
                {
                    string color = "black";
                    string text = "";
                    switch (cells[x, y])
                    {
                        case CellType.WALL:
                            text = "%";
                            break;
                        case CellType.ROOM:
                        case CellType.CORRIDOR:
                            color = "white";
                            break;
                        case CellType.ENTRANCE:
                            color = "green";
                            break;
                    }
                    sb.Append(string.Format("<td style=\"background-color: {0};\">{1}</td>", color, text));
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }
    }
}
