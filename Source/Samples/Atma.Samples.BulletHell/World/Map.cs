using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Assets;
using Atma.Engine;
using Atma.Graphics;
using Atma.Rendering;
using Microsoft.Xna.Framework;
using Atma.Samples.BulletHell.Systems.Phsyics;

namespace Atma.Samples.BulletHell.World
{

    public class Map : GameSystem, IComponentSystem, IRenderSystem, IUpdateSubscriber, ICollisionSystem
    {
        public static readonly GameUri Uri = "bullethell:map";

        private Generators.ILevel _baselevel;
        public float scale = 50;
        private SpriteBatch2 batch;
        private Texture2D texture;
        private Texture2D bloodTexture;

        private const int WEST = 1;
        private const int NORTH = 2;
        private const int EAST = 4;
        private const int SOUTH = 8;

        private AxisAlignedBox[] floorGrid;
        private AxisAlignedBox[] wallGrid;

        private int neighbors(int x, int y, CellType check)
        {
            var west = _baselevel.GetCell(x - 1, y) == check;
            var north = _baselevel.GetCell(x, y - 1) == check;
            var east = _baselevel.GetCell(x + 1, y) == check;
            var south = _baselevel.GetCell(x, y + 1) == check;

            var index = 0;
            if (west) index |= 1;
            if (north) index |= 2;
            if (east) index |= 4;
            if (south) index |= 8;

            return index;
        }

        private void initFloorGrid()
        {
            floorGrid = new AxisAlignedBox[16];

            for (var i = 0; i < floorGrid.Length; i++)
            {
                var west = (i & WEST) == WEST;
                var south = (i & SOUTH) == SOUTH;

                if (south && west)
                    floorGrid[i] = getSrc(0, 5, 16);
                else if (west)
                    floorGrid[i] = getSrc(0, 4, 16);
                else if (south)
                    floorGrid[i] = getSrc(1, 5, 16);
                else
                    floorGrid[i] = getSrc(1, 4, 16);
            }
        }

        private void initWallGrid()
        {
            wallGrid = new AxisAlignedBox[16];

            wallGrid[0] = getSrc(0, 0, 16);
            wallGrid[EAST] = getSrc(1, 0, 16);
            wallGrid[WEST | EAST] = getSrc(2, 0, 16);
            wallGrid[WEST] = getSrc(3, 0, 16);

            wallGrid[SOUTH] = getSrc(0, 1, 16);
            wallGrid[SOUTH | EAST] = getSrc(1, 1, 16);
            wallGrid[SOUTH | EAST | WEST] = getSrc(2, 1, 16);
            wallGrid[SOUTH | WEST] = getSrc(3, 1, 16);

            wallGrid[SOUTH | NORTH] = getSrc(0, 2, 16);
            wallGrid[SOUTH | NORTH | EAST] = getSrc(1, 2, 16);
            wallGrid[SOUTH | NORTH | EAST | WEST] = getSrc(2, 2, 16);
            wallGrid[SOUTH | NORTH | WEST] = getSrc(3, 2, 16);

            wallGrid[NORTH] = getSrc(0, 3, 16);
            wallGrid[NORTH | EAST] = getSrc(1, 3, 16);
            wallGrid[NORTH | WEST | EAST] = getSrc(2, 3, 16);
            wallGrid[NORTH | WEST] = getSrc(3, 3, 16);
        }

        private void buildCollisionMap()
        {
            //for (var x = 0; x < _baselevel.Width; x++)
            //{
            //    var startx = -1;
            //    for (var y = 0; y < _baselevel.Height; y++)
            //    {


            //        if (startx < 0)
            //        {
            //            if (_baselevel.GetCell(x, y) == CellType.PERIMITER)
            //                startx = x;
            //        }
            //        else
            //        {
            //            if (_baselevel.GetCell(x, y) != CellType.PERIMITER)
            //            {
            //                var endx = x;


            //            }
            //        }
            //    }
            //}
        }

        private List<Shape> collisionCache = new List<Shape>();
        public List<Shape> getCollidables(AxisAlignedBox aabb)
        {
            collisionCache.Clear();
            aabb.Inflate(Vector2.One * 2 * scale);
            var min = getMapPoint(aabb.Minimum);
            var max = getMapPoint(aabb.Maximum);

            for (var w = min.X; w < max.X; w++)
            {
                for (var h = min.Y; h < max.Y; h++)
                {
                    var p = new Vector2(w, h) * scale;
                    var cell = _baselevel.GetCell(w, h);

                    switch (cell)
                    {
                        case CellType.PERIMITER:
                            var shape = new Shape(AxisAlignedBox.FromRect(p, Vector2.One * scale));
                            collisionCache.Add(shape);
                            break;
                    }
                }
            }

            return collisionCache;
        }

        private AxisAlignedBox getSrc(int x, int y, int size)
        {
            return AxisAlignedBox.FromRect(new Vector2(x, y) * size, new Vector2(size));
        }

        public void renderOpaque()
        {
            //return;
            //random = new Random(123);
            //var offset = new Vector2(scale * _baselevel.Width, scale * _baselevel.Height) * 0.5f;

            PerformanceMonitor.start("render map tiles");
            var worldBounds = GameWorld.instance.currentCamera.worldBounds;
            worldBounds.Inflate(Vector2.One * 2 * scale);
            var min = getMapPoint(worldBounds.Minimum);
            var max = getMapPoint(worldBounds.Maximum);

            var room_cspread = 15;
            var room_c1 = new Color(255 - room_cspread, 255 - room_cspread, 255 - room_cspread, 255);
            var room_c2 = new Color(255, 255, 255, 255);

            batch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Deferred);
            for (var w = min.X; w < max.X; w++)
            {
                for (var h = min.Y; h < max.Y; h++)
                {
                    var p = new Vector2(w, h) * scale;
                    var cell = _baselevel.GetCell(w, h);

                    switch (cell)
                    {
                        case CellType.ROOM:
                        //batch.draw(texture, p, new Vector2(scale, scale), color: Color.Pink);
                        //break;
                        case CellType.CORRIDOR:
                        //batch.draw(texture, p, new Vector2(scale, scale), color: Color.Blue);
                        //break;
                        case CellType.ENTRANCE:
                            {
                                var bits = neighbors(w, h, CellType.PERIMITER);
                                var src = floorGrid[bits];
                                var r = PerlinSimplexNoise.noise(w * 0.1f, h * 0.1f);

                                //batch.draw(texture, p - offset, new Vector2(scale, scale), sourceRectangle: src, color: Color.Lerp(c1, c2, r));
                                batch.draw(texture, p , new Vector2(scale, scale), sourceRectangle: src, color: Color.Lerp(room_c1, room_c2, r));
                            }
                            break;
                        case CellType.EMPTY:
                            //batch.draw(texture, p, new Vector2(scale, scale), color: Color.BlanchedAlmond, depth: 0.1f);
                            break;
                        case CellType.PERIMITER:
                            {
                                var bits = neighbors(w, h, CellType.PERIMITER);
                                var src = wallGrid[bits];

                                batch.draw(texture, p , new Vector2(scale, scale), color: Color.White, depth: 0.1f, sourceRectangle: src);
                                break;
                            }
                        case CellType.WALL:
                            batch.draw(texture, p , new Vector2(scale, scale), color: Color.Red);
                            break;
                    }
                }
            }

            batch.End();
            PerformanceMonitor.end("render map tiles");
        }

        public void renderAlphaBlend()
        {
            var offset = new Vector2(scale * _baselevel.Width, scale * _baselevel.Height) * 0.5f;

            PerformanceMonitor.start("render map tiles");
            var worldBounds = GameWorld.instance.currentCamera.worldBounds;
            worldBounds.Inflate(Vector2.One * 2 * scale);
            var min = getMapPoint(worldBounds.Minimum + offset);
            var max = getMapPoint(worldBounds.Maximum + offset);

            batch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Deferred);
            for (var w = min.X; w < max.X; w++)
            {
                for (var h = min.Y; h < max.Y; h++)
                {
                    var p = new Vector2(w, h) * scale;
                    var cell = _baselevel.GetCell(w, h);

                    switch (cell)
                    {
                        case CellType.ROOM:
                        case CellType.CORRIDOR:
                        case CellType.ENTRANCE:
                            var colorSpread = 10;
                            var c1 = new Color(211 - colorSpread, 211 - colorSpread, 211 - colorSpread, 255);
                            var c2 = new Color(211 + colorSpread, 211 + colorSpread, 211 + colorSpread, 255);


                            var blood = PerlinSimplexNoise.noise(w * 0.25f, h * 0.25f);
                            if (blood < 0.1999999f)
                            {
                                var index = (int)(blood * 75);
                                var x = index % 5;
                                var y = (index - x) / 5;

                                var srcScale = 32;
                                var src = AxisAlignedBox.FromRect(new Vector2(x, y) * srcScale, Vector2.One * srcScale);

                                batch.draw(bloodTexture, p - offset + new Vector2(scale, scale) * 0.25f, new Vector2(scale, scale) * 0.5f, sourceRectangle: src, origin: new Vector2(0.5f, 0.5f), color: Color.White * 0.5f);//, color: Color.Lerp(c1, c2, r));
                            }

                            break;
                        case CellType.EMPTY:
                        case CellType.PERIMITER:
                        case CellType.WALL:
                            break;
                    }
                }
            }

            batch.End();
            PerformanceMonitor.end("render map tiles");
        }

        public void renderOverlay()
        {

        }

        public void renderShadows()
        {
        }

        public void init()
        {
            initFloorGrid();
            initWallGrid();
            //_baselevel.Generate(123);

            _baselevel = new Generators.JWLevelGenerator(100, 100);
            //_baselevel = new Generators.Dungeon(100, 100, 4, 8, 4, 8);
            //_baselevel = new Generators.Town(100, 100);
            //_baselevel = new Generators.StringLevel();
            _baselevel.Generate(123);

            batch = new SpriteBatch2(display.device);
            texture = assets.getTexture("bullethell:bullethell/hedge_maze");// new Texture2D("floor:texture", TextureData.create(1, 1, Microsoft.Xna.Framework.Color.White));
            //texture = assets.getTexture("bullethell:bullethell/stone_bricks");// new Texture2D("floor:texture", TextureData.create(1, 1, Microsoft.Xna.Framework.Color.White));
            //texture = assets.getTexture("bullethell:tiles/plain2");// new Texture2D("floor:texture", TextureData.create(1, 1, Microsoft.Xna.Framework.Color.White));
            bloodTexture = assets.getTexture("bullethell:bullethell/blood2");// new Texture2D("floor:texture", TextureData.create(1, 1, Microsoft.Xna.Framework.Color.White));
        }

        public AxisAlignedBox getCellAABBFromWorld(Vector2 p)
        {
            p /= scale;
            var x = (int)p.X;
            var y = (int)p.Y;

            if (x < 0 || x >= _baselevel.Width || y < 0 || y >= _baselevel.Height)
                return AxisAlignedBox.Null;

            return AxisAlignedBox.FromRect(x, y, scale, scale);
        }

        public Point getMapPoint(Vector2 p)
        {
            p /= scale;
            var x = (int)p.X;
            var y = (int)p.Y;

            x = MathHelper.Clamp(x, 0, _baselevel.Width);
            y = MathHelper.Clamp(y, 0, _baselevel.Height);

            return new Point(x, y);
        }

        public CellType getCellFromWorld(Vector2 p)
        {
            p /= scale;
            var x = (int)p.X;
            var y = (int)p.Y;

            if (x < 0 || x >= _baselevel.Width || y < 0 || y >= _baselevel.Height)
                return CellType.EMPTY;

            return _baselevel.GetCell(x, y);
        }

        public void shutdown()
        {

        }

        private float stepDelay = 0.015f;
        private float stepDelayTimer = 0f;
        public void update(float delta)
        {
            if (_baselevel is Generators.JWLevelGenerator && stepDelay > 0.0001f)
            {
                var jw = (Generators.JWLevelGenerator)_baselevel;
                stepDelayTimer -= delta;
                while (stepDelayTimer < 0f)
                {
                    stepDelayTimer += stepDelay;
                    if (!jw.step())
                    {
                        if (input.WasKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
                            jw.Reset();
                        //stepDelay *= 0.8f;
                        //if (stepDelay > 0.0001f)
                        //    jw.Reset();
                        //else
                        //    break;
                    }

                }
            }
        }



        public void broadphase(CollisionQuery query, List<Collision> collisions)
        {
            var min = getMapPoint(query.aabb.Minimum);
            var max = getMapPoint(query.aabb.Maximum);

            for (var w = min.X; w <= max.X; w++)
            {
                for (var h = min.Y; h <= max.Y; h++)
                {
                    var p = new Vector2(w, h) * scale;
                    var cell = _baselevel.GetCell(w, h);

                    switch (cell)
                    {
                        case CellType.PERIMITER:
                            var shape = AxisAlignedBox.FromRect(p, Vector2.One * scale);
                            collisions.Add(new Collision(0, 0, shape));
                            break;
                    }
                }
            }
        }
    }


}
