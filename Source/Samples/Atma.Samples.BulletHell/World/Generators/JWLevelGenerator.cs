using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Samples.BulletHell.World.Generators
{
    public class JWLevelGenerator : BaseLevel
    {

        public class JWFloorMaker
        {
            public int x, y;
            public int direction;
        }

        private List<JWFloorMaker> floorMakers = new List<JWFloorMaker>();

        //public float floorMakerSpawnRate = 0.1f;
        public int maxFloorMakers = 4;
        public float turnRate = 0.25f;
        public float despawnRate = 0.025f;
        public int iterations = 400;

        public JWLevelGenerator(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }

        public bool step()
        {
            if (iterations-- < 0)
                return false;

            for (int i = floorMakers.Count - 1; i >= 0; i--)
            {
                var remove = iterations == 0;
                var current = floorMakers[i];
                //check for spawn floor maker spawn
                if (!remove)
                {
                    var factor = 1f - floorMakers.Count / (float)maxFloorMakers;
                    if (random.NextFloat() < factor)
                    {
                        floorMakers.Add(new JWFloorMaker() { x = current.x, y = current.y, direction = (current.direction ^ 3) });
                    }
                    else if (random.NextFloat() < despawnRate && floorMakers.Count > 1)
                        remove = true;
                }

                if (random.NextFloat() < turnRate)
                {
                    if (random.NextFloat() < turnRate)
                    {
                        current.direction ^= 3;
                    }
                    else
                    {
                        var newdir = random.Next(0, 2);
                        if (newdir == 2)
                            throw new Exception("shit");

                        switch (current.direction)
                        {

                            case 2:
                            case 1:
                                if (newdir == 0)
                                    current.direction = 0;
                                else
                                    current.direction = 3;
                                break;
                            case 0:
                            case 3:
                                if (newdir == 0)
                                    current.direction = 1;
                                else
                                    current.direction = 2;
                                break;
                        }
                    }
                }

                var width = 1;
                var height = 1;
                var offsetx = 0;
                var offsety = 0;

                var next = random.NextFloat();

                if (next < 0.1f)
                {
                    width = 3;
                    height = 3;
                }
                else if (next < 0.3f)
                {
                    width = 2;
                    height = 2;
                }

                offsetx = random.Next(0, width);
                offsety = random.Next(0, height);

                var placed = PlaceRoom(current.x - offsetx, current.y - offsety, width, height);
                if(!placed)
                    this.cells[current.x, current.y] = CellType.ROOM;
                var moved = true;

                switch (current.direction)
                {
                    case 1:
                        if (current.x + 1 == Width - 1)
                        {
                            current.direction = (current.direction ^ 3);
                            moved = false;
                        }
                        else
                            current.x++;
                        break;
                    case 2:
                        if (current.x - 1 == 0)
                        {
                            current.direction = (current.direction ^ 3);
                            moved = false;
                        }
                        else
                            current.x--;
                        break;
                    case 0:
                        if (current.y + 1 == Height - 1)
                        {
                            current.direction = (current.direction ^ 3);
                            moved = false;
                        }
                        else
                            current.y++;
                        break;
                    case 3:
                        if (current.y - 1 == 0)
                        {
                            current.direction = (current.direction ^ 3);
                            moved = false;
                        }
                        else
                            current.y--;
                        break;
                }
                //if (placed)
                //if (moved)
                if (!remove)
                    this.cells[current.x, current.y] = CellType.WALL;
                else
                    floorMakers.RemoveAt(i);
                //else
                //    this.cells[current.x, current.y] = CellType.EMPTY;

            }

            return true;
        }

        private Random random;

        public override void Generate(int seed)
        {
            Resize();

            random = new Random(seed);

            floorMakers.Add(new JWFloorMaker() { x = Width / 2, y = Height / 2 });
            //for (var i = 0; i < 100; i++)
            //    step();
        }

        public override void Reset()
        {
            base.Reset();

            random = new Random();

            floorMakers.Add(new JWFloorMaker() { x = Width / 2, y = Height / 2 });
            iterations = 400;
        }

        private bool PlaceRoom(int start_x, int start_y, int width, int height)
        {
            int end_x = start_x + width - 1;
            int end_y = start_y + height - 1;
            if (end_x >= Width - 1 || end_y >= Height - 1 || start_x < 1 || start_y < 1)
                return false;
            //for (int y = start_y; y < end_y; y++)
            //    for (int x = start_x; x < end_x; x++)
            //        if (cells[x, y] != CellType.EMPTY)
            //            return false;

            for (int y = start_y - 1; y < end_y + 2; y++)
            {
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
            }
            return true;
        }

    }
}
