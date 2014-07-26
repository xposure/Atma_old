using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Assets;
using Atma.Engine;
using Atma.Entities;
using Atma.Graphics;
using Atma.Rendering;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Texture2D = Atma.Graphics.Texture2D;
using Atma.Samples.BulletHell.World;

namespace Atma.Samples.Bullethell.Rendering
{
    public class ShadowRenderer : GameSystem, IComponentSystem, IRenderSystem
    {
        public static readonly GameUri Uri = "componentsystem:shadowrender";

        private Texture2D _defaultTexture;
        private SpriteBatch2 batch;

        public Vector2 target;
        private BlendState blendLight;

        private Texture2D testtex;

        public void init()
        {
            _defaultTexture = new Texture2D("TEXTURE:engine:default", TextureData.create(1, 1, Color.White));

            batch = new SpriteBatch2(display.device);
        }

        public void shutdown()
        {
        }

        private List<RayIntersectResult> rays = new List<RayIntersectResult>();

        private struct RayIntersectResult
        {
            public Ray ray;
            public IntersectResult result;
        }

        public void renderOpaque()
        {

        }

        private void renderShadows(Vector2 target, List<Shape> items)
        {
            rays.Clear();

            foreach (var shape in items)
            {
                foreach (var p in shape.derivedVertices)
                {
                    var dir = p - target;
                    var len = dir.Length();
                    dir.Normalize();

                    var ray = new RayIntersectResult();
                    ray.ray = new Ray(target, dir);
                    ray.result = new IntersectResult(true, len);
                    rays.Add(ray);

                    var ang = (float)Math.Atan2(dir.Y, dir.X);
                    var left = ang - 0.00001;
                    var right = ang + 0.00001;

                    var dirleft = new Vector2((float)Math.Cos(left), (float)Math.Sin(left));
                    var dirright = new Vector2((float)Math.Cos(right), (float)Math.Sin(right));

                    rays.Add(new RayIntersectResult() { ray = new Ray(target, dirleft), result = new IntersectResult() });
                    rays.Add(new RayIntersectResult() { ray = new Ray(target, dirright), result = new IntersectResult() });

                }
            }

            PerformanceMonitor.start("ray trace shadows");
            batch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Deferred);
            var counter = 0;
            foreach (var shape in items)
            {
                for (var i = 0; i < rays.Count; i++)
                {
                    var ray = rays[i];

                    var r = shape.intersects(ray.ray);
                    var result = ray.result;
                    if (r.Hit)
                    {
                        if (!result.Hit || r.Distance < result.Distance)
                            result = r;
                    }
                    ray.result = result;
                    rays[i] = ray;
                    counter++;
                }
            }
            PerformanceMonitor.end("ray trace shadows");

            //ray = new Ray(Vector2.Zero, new Vector2(1, 0));
            //var line = new LineSegment(new Vector2(200, -100), new Vector2(100, 100));
            //result = line.intersects2(ray);

            //batch.drawLine(_defaultTexture, line.p0, line.p1);
            rays.Sort(CompareAngle);
            var color1 = Color.Green;
            var color2 = Color.Blue;
            //for (var i = 0; i < rays.Count; i++)
            //{
            //    if (rays[i].result.Hit)
            //    {
            //        var ni = (i + 1) % rays.Count;
            //        var p0 = rays[i].ray.origin;
            //        var p1 = rays[i].ray.origin + rays[i].ray.direction * rays[i].result.Distance;
            //        var p2 = rays[ni].ray.origin + rays[ni].ray.direction * rays[ni].result.Distance;
            //        var p3 = rays[ni].ray.origin;
            //        var color = Color.FromNonPremultiplied(255, 128, 0, 128);

            //        batch.drawQuad(_defaultTexture, p0, p1, p2, p3, color: color);
            //    }
            //}

            PerformanceMonitor.start("render ray shadows");
            var color = Color.FromNonPremultiplied(0, 0, 0, 256);

            if (totalightpasses > 0)
                color = Color.FromNonPremultiplied(0, 0, 0, 256 / totalightpasses);

            for (var i = 0; i < rays.Count; i++)
            {
                if (rays[i].result.Hit)
                {
                    var ni = (i + 1) % rays.Count;
                    var p0 = rays[i].ray.Origin;
                    var p1 = rays[i].ray.Origin + rays[i].ray.Direction * rays[i].result.Distance;
                    var p2 = rays[ni].ray.Origin + rays[ni].ray.Direction * rays[ni].result.Distance;
                    var p3 = rays[ni].ray.Origin;

                    batch.drawQuad(_defaultTexture, p0, p1, p2, p3, color: color);
                }
            }
            PerformanceMonitor.end("render ray shadows");


            //_display.device.SetRenderTarget(null);

            //for (var i = 0; i < rays.Count; i++)
            //{

            //    if (rays[i].result.Hit)
            //    {
            //        //batch.draw(mat.texture, rays[i].ray.origin + rays[i].ray.direction * rays[i].result.Distance, new Vector2(16, 16), origin: new Vector2(0.5f, 0.5f));
            //    }
            //}


            batch.End();
        }


        private int lightoffset = 15;
        private int totalightpasses = 1;
        private int CompareAngle(RayIntersectResult a, RayIntersectResult b)
        {
            return a.ray.Angle.CompareTo(b.ray.Angle);
        }

        private struct RadixShape : IRadixKey
        {
            private Projection projection;

            public Shape shape;

            public void setup(Vector2 source)
            {
                var dir = shape.derivedCenter - source;
                dir.Normalize();
                projection = shape.project(dir);
            }

            public int Key { get { return (int)projection.min; } }
            public int Min { get { return (int)projection.min; } }
            public int Max { get { return (int)projection.max; } }
        }


        public void renderShadows()
        {
            return;
            //return;
            var mat = this.assets.getMaterial("bullethell:reddot"); //resources.createMaterialFromTexture("content/textures/bullethell/cursor.png");
            var map = CoreRegistry.require<Map>();
            var items = map.getCollidables(AxisAlignedBox.FromDimensions(target, new Vector2(1024, 768)));

            items.Add(new Shape(AxisAlignedBox.FromDimensions(Vector2.Zero, new Vector2(1024, 768))));

            var shapes = new RadixShape[items.Count];

            var ang = 0f;
            for (var i = 0; i < totalightpasses; i++)
            {
                var dir = new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang));
                var p = dir * lightoffset;
                renderShadows(target + p, items);

                ang += MathHelper.TwoPi / totalightpasses;
            }

            renderShadows(target, items);

            //batch.Begin(SpriteSortMode.Deferred);
            //batch.draw(testtex, new Vector2(-50, -50), new Vector2(300, 300), color: Color.Black);
            //batch.draw(testtex, Vector2.Zero, new Vector2(200, 200), color: Color.FromNonPremultiplied(255, 0, 0, 160));
            //batch.End();
            //_display.device.SetRenderTarget(target2d);
            //_display.device.Clear(Color.Transparent);
            //var oldBlendState = _display.device.BlendState;


            //_display.device.SetRenderTarget(null);
            //_display.device.BlendState = oldBlendState;
        }

        public void renderAlphaBlend()
        {
            var mat = assets.getMaterial("bullethell:reddot"); //resources.createMaterialFromTexture("content/textures/bullethell/cursor.png");

            //_display.device.SetRenderTarget(target2d);
            //_display.device.Clear(Color.Transparent);
            //var oldBlendState = _display.device.BlendState;

            //batch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Deferred);

            //for (var i = 0; i < rays.Count; i++)
            //{
            //    if (rays[i].result.Hit)
            //    {
            //        var ni = (i + 1) % rays.Count;
            //        var p0 = rays[i].ray.origin;
            //        var p1 = rays[i].ray.origin + rays[i].ray.direction * rays[i].result.Distance;
            //        var p2 = rays[ni].ray.origin + rays[ni].ray.direction * rays[ni].result.Distance;
            //        var p3 = rays[ni].ray.origin;
            //        var color = Color.FromNonPremultiplied(0, 0, 0, 128);

            //        batch.drawQuad(_defaultTexture, p0, p1, p2, p3, color: color);
            //    }
            //}

            //_display.device.SetRenderTarget(null);

            //for (var i = 0; i < rays.Count; i++)
            //{

            //    if (rays[i].result.Hit)
            //    {
            //        //batch.draw(mat.texture, rays[i].ray.origin + rays[i].ray.direction * rays[i].result.Distance, new Vector2(16, 16), origin: new Vector2(0.5f, 0.5f));
            //    }
            //}


            //batch.End();

            //_display.device.SetRenderTarget(null);
            //_display.device.BlendState = oldBlendState;
        }

        public void renderOverlay()
        {
            return;
            //var items = new List<Temp>();
            var items = new List<Shape>();


            var targetshape = new Shape(new Vector2[2] { Vector2.Zero, Vector2.Zero });

            foreach (var id in entities.getWithComponents("transform", "shape"))
            {
                //var temp = new Temp();
                //temp.shape = em.getComponent<ShapeComponent>(id, "shape");
                //temp.transform = em.getComponent<Transform>(id, "transform");

                items.Add(entities.getComponent<ShapeComponent>(id, "shape").shape);
            }

            items.Add(new Shape(AxisAlignedBox.FromDimensions(Vector2.Zero, new Vector2(1024, 768))));


            //for (var i = 0; i < rays.Count; i++)
            //{
            //    if (rays[i].result.Hit)
            //    {
            //        //var color = Color.Lerp(color1, color2, i / (float)rays.Count);
            //        var color = Color.Red;
            //        batch.drawLine(_defaultTexture, rays[i].ray.origin, rays[i].ray.origin + rays[i].ray.direction * rays[i].result.Distance, width: 2f, color: color);
            //        //batch.draw(mat.texture, rays[i].origin + rays[i].direction * results[i].Distance, new Vector2(6, 6), origin: new Vector2(0.5f, 0.5f));
            //    }
            //}

            batch.Begin(SpriteSortMode.Deferred);
            foreach (var shape in items)
            {
                batch.drawShape(_defaultTexture, shape, color: Color.Red, width: 2f);
            }

            var mat = assets.getMaterial("bullethell:reddot"); //resources.createMaterialFromTexture("content/textures/bullethell/cursor.png");
            var ang = 0f;
            for (var i = 0; i < totalightpasses; i++)
            {
                var dir = new Vector2((float)Math.Cos(ang), (float)Math.Sin(ang));
                var p = dir * lightoffset;
                batch.draw(mat.texture, target + p, new Vector2(8, 8), origin: new Vector2(0.5f, 0.5f));
                ang += MathHelper.TwoPi / totalightpasses;
            }
            batch.draw(mat.texture, target, new Vector2(8, 8), origin: new Vector2(0.5f, 0.5f));


            batch.End();
        }

    }

}
