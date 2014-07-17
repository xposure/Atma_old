using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Assets;
using Atma.Engine;
using Atma.Entities;
using Atma.Graphics;

using Microsoft.Xna.Framework;

namespace Atma.Rendering
{
    public class ShapeRenderer : IComponentSystem, IRenderSystem
    {
        public static readonly GameUri Uri = "componentsystem:ShapeRenderer";

        private AssetManager _assets;
        private DisplayDevice _display;
        private Texture2D _defaultTexture;
        private SpriteBatch2 batch;

        public Vector2 target;

        public void init()
        {
            //var world = this.world();
            _assets = this.assets();
            _display = this.display();
            _defaultTexture = new Texture2D("TEXTURE:engine:default", TextureData.create(1, 1, Color.White));

            batch = new SpriteBatch2(_display.device);
        }

        public void shutdown()
        {
        }

        //private struct Temp
        //{
        //    public ShapeComponent shape;
        //    public Transform transform;
        //}

        public void renderOpaque()
        {
            //var items = new List<Temp>();
            var items = new List<Shape>();

            var graphics = this.graphics();
            var em = this.entities();

            //target.Normalize();


            var targetshape = new Shape(new Vector2[2] { Vector2.Zero, Vector2.Zero });

            foreach (var id in em.getWithComponents("transform", "shape"))
            {
                //var temp = new Temp();
                //temp.shape = em.getComponent<ShapeComponent>(id, "shape");
                //temp.transform = em.getComponent<Transform>(id, "transform");

                items.Add(em.getComponent<ShapeComponent>(id, "shape").shape);
            }

            items.Add(new Shape(AxisAlignedBox.FromDimensions(Vector2.Zero, new Vector2(1024, 768))));
            //var ray = new Ray(Vector2.Zero, target);
            //var result = new IntersectResult();
            var segs = 32;
            var ang = 0f;
            var rays = new Ray[segs];
            for (var i = 0; i < segs; i++)
            {
                rays[i] = new Ray(target, new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang)));
                ang += MathHelper.TwoPi / segs;
            }

            var results = new IntersectResult[segs];

            batch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Texture);
            foreach (var shape in items)
            {
                batch.drawShape(_defaultTexture, shape);

                for (var i = 0; i < segs; i++)
                {
                    var r = shape.intersects(rays[i]);
                    if (r.Hit)
                    {
                        if (!results[i].Hit || r.Distance < results[i].Distance)
                            results[i] = r;
                    }
                }
            }

            //ray = new Ray(Vector2.Zero, new Vector2(1, 0));
            //var line = new LineSegment(new Vector2(200, -100), new Vector2(100, 100));
            //result = line.intersects2(ray);

            //batch.drawLine(_defaultTexture, line.p0, line.p1);

            for (var i = 0; i < segs; i++)
            {
                if (results[i].Hit)
                {
                    batch.drawLine(_defaultTexture, rays[i].origin, rays[i].origin + rays[i].direction * results[i].Distance);
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

    }

}
