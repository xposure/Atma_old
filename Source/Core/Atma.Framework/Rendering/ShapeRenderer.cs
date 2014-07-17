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

        private List<Ray> rays = new List<Ray>();
        private List<IntersectResult> results = new List<IntersectResult>();
        //private List<RayIntersectResult> rays = new List<RayIntersectResult>();

        private struct RayIntersectResult
        {
            public Ray ray;
            public IntersectResult result;
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
            //var segs = 32;
            //var ang = 0f;
            //var rays = new List<Ray>();
            //var results = new List<IntersectResult>();
            rays.Clear();
            results.Clear();

            foreach (var shape in items)
            {
                foreach (var p in shape.derivedVertices)
                {
                    var dir = p - target;
                    var len = dir.Length();
                    dir.Normalize();

                    var ray = new Ray(target, dir);
                    rays.Add(ray);

                    results.Add(new IntersectResult(true, len));

                    var ang = (float)Math.Atan2(dir.Y, dir.X);
                    var left = ang - 0.00001;
                    var right = ang + 0.00001;

                    var dirleft = new Vector2((float)Math.Cos(left), (float)Math.Sin(left));
                    var dirright = new Vector2((float)Math.Cos(right), (float)Math.Sin(right));

                    rays.Add(new Ray(target, dirleft));
                    results.Add(new IntersectResult());

                    rays.Add(new Ray(target, dirright));
                    results.Add(new IntersectResult());

                }
            }

            batch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Deferred);
            foreach (var shape in items)
            {
                batch.drawShape(_defaultTexture, shape);

                for (var i = 0; i < rays.Count; i++)
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

            for (var i = 0; i < rays.Count; i++)
            {
                if (results[i].Hit)
                {
                    batch.drawLine(_defaultTexture, rays[i].origin, rays[i].origin + rays[i].direction * results[i].Distance, width: 2f, color: Color.Red);
                    //batch.draw(mat.texture, rays[i].origin + rays[i].direction * results[i].Distance, new Vector2(6, 6), origin: new Vector2(0.5f, 0.5f));
                }
            }

            batch.End();

        }

        public void renderAlphaBlend()
        {
            var mat = this.assets().getMaterial("bullethell:reddot"); //resources.createMaterialFromTexture("content/textures/bullethell/cursor.png");
            batch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Deferred);



            for (var i = 0; i < rays.Count; i++)
            {
                if (results[i].Hit)
                {
                    var ni = (i + 1) % rays.Count;
                    batch.draw(mat.texture, rays[i].origin + rays[i].direction * results[i].Distance, new Vector2(16, 16), origin: new Vector2(0.5f, 0.5f));
                    var p0 = rays[i].origin;
                    var p1 = rays[i].origin + rays[i].direction * results[i].Distance;
                    var p2 = rays[ni].origin + rays[ni].direction * results[ni].Distance;
                    var p3 = rays[ni].origin;
                    
                    //batch.drawQuad(_defaultTexture, p0, p1, p2, p3);
                }
            }


            batch.End();
        }

        public void renderOverlay()
        {

        }

    }

}
