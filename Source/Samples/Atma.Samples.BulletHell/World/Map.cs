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
    public class Map : GameSystem, IComponentSystem, IRenderSystem
    {
        public static readonly GameUri Uri = "bullethell:map";

        private Generators.BaseLevel _baselevel;
        private int scale = 100;
        private SpriteBatch2 batch;
        private Texture2D texture;

        public void renderOpaque()
        {
            batch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Deferred);

            for (var w = 0; w < _baselevel.Width; w++)
            {
                for (var h = 0; h < _baselevel.Height; h++)
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
                        case CellType.EMPTY:
                            batch.draw(texture, p, new Vector2(scale, scale), color: Color.Black);
                            break;
                        case CellType.PERIMITER:
                            batch.draw(texture, p, new Vector2(scale, scale), color: Color.Gray);
                            break;
                        case CellType.WALL:
                            batch.draw(texture, p, new Vector2(scale, scale), color: Color.Red);
                            break;
                    }
                }
            }

            batch.End();
        }

        public void renderAlphaBlend()
        {

        }

        public void renderOverlay()
        {

        }

        public void renderShadows()
        {
        }

        public void init()
        {
            _baselevel = new Generators.Dungeon(100, 100, 6, 6, 12, 12);
            _baselevel.Generate(123);

            batch = new SpriteBatch2(display.device);
            texture = new Texture2D("floor:texture", TextureData.create(1, 1, Microsoft.Xna.Framework.Color.White));
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
    }

 
}
