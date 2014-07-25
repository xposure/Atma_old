using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Assets;
using Atma.Engine;
using Atma.Graphics;
using Atma.Rendering;
using Microsoft.Xna.Framework;

namespace Atma.Samples.BulletHell.World
{
    public class Map : GameSystem, IComponentSystem, IRenderSystem, IUpdateSubscriber
    {
        public static readonly GameUri Uri = "bullethell:map";



        //private Generators.BaseLevel _baselevel;
        private Generators.ILevel _baselevel;
        private int scale =5;
        private SpriteBatch2 batch;
        private Texture2D texture;
        private Texture2D bloodTexture;
        //private Random random = new Random();



        public void renderOpaque()
        {
            //return;
            //random = new Random(123);
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
                        //batch.draw(texture, p, new Vector2(scale, scale), color: Color.Pink);
                        //break;
                        case CellType.CORRIDOR:
                        //batch.draw(texture, p, new Vector2(scale, scale), color: Color.Blue);
                        //break;
                        case CellType.ENTRANCE:
                            //batch.draw(texture, p, new Vector2(scale, scale), color: Color.Green);
                            //break;
                            //var west = _baselevel.GetCell(w - 1, h) == CellType.ROOM;
                            //var north = _baselevel.GetCell(w, h - 1) == CellType.ROOM;
                            //var east = _baselevel.GetCell(w + 1, h) == CellType.ROOM;
                            //var south = _baselevel.GetCell(w, h + 1) == CellType.ROOM;

                            //var index = 0;
                            //if (west) index |= 1;
                            //if (north) index |= 2;
                            //if (east) index |= 4;
                            //if (south) index |= 8;

                            //var src = AxisAlignedBox.Null;

                            //switch(index)

                            var colorSpread = 10;
                            var c1 = new Color(211 - colorSpread, 211 - colorSpread, 211 - colorSpread, 255);
                            var c2 = new Color(211 + colorSpread, 211 + colorSpread, 211 + colorSpread, 255);
                            //var r = random.NextFloat();
                            var r = PerlinSimplexNoise.noise(w, h);

                            batch.draw(texture, p - offset, new Vector2(scale, scale), color: Color.Lerp(c1, c2, r));

                            break;
                        case CellType.EMPTY:
                            //batch.draw(texture, p, new Vector2(scale, scale), color: Color.BlanchedAlmond, depth: 0.1f);
                            break;
                        case CellType.PERIMITER:
                            //batch.draw(texture, p - new Vector2(scale * 0.1f, scale * -0.1f), new Vector2(scale, scale), color: Color.DarkGray, depth: 0.05f);
                            batch.draw(texture, p - offset, new Vector2(scale, scale), color: Color.Gray, depth: 0.1f);
                            break;
                        case CellType.WALL:
                            batch.draw(texture, p - offset, new Vector2(scale, scale), color: Color.Red);
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

                                batch.draw(bloodTexture, p - offset + new Vector2(scale,scale) * 0.25f, new Vector2(scale, scale)*0.5f, sourceRectangle: src, origin: new Vector2(0.5f, 0.5f), color: Color.White * 0.5f);//, color: Color.Lerp(c1, c2, r));
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
            //_baselevel.Generate(123);

            _baselevel = new Generators.JWLevelGenerator(150, 150);
            //_baselevel = new Generators.Dungeon(100, 100, 4, 8, 4, 8);
            //_baselevel = new Generators.Town(100, 100);
            //_baselevel = new Generators.StringLevel();
            _baselevel.Generate(123);

            batch = new SpriteBatch2(display.device);
            texture = assets.getTexture("bullethell:tiles/plain2");// new Texture2D("floor:texture", TextureData.create(1, 1, Microsoft.Xna.Framework.Color.White));
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
                        stepDelay *= 0.8f;
                        if (stepDelay > 0.0001f)
                            jw.Reset();
                        else
                            break;
                    }

                }
            }
        }
    }


}
