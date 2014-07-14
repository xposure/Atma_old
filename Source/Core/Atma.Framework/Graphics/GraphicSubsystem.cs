﻿using Atma.Assets;
using Atma.Engine;
using Atma.Managers;
using Atma.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Atma.Fonts;
using System.Collections.Generic;

namespace Atma.Graphics
{
    public class FBO
    {
        private string _name;
        private RenderTarget2D _target;
        private GraphicsDevice _device;

        public FBO(GraphicsDevice device, string name, RenderTarget2D target)
        {
            _name = name;
            _device = device;
            _target = target;
        }

        public void bind()
        {
            _device.Viewport = new Microsoft.Xna.Framework.Graphics.Viewport(0, 0, _target.Width, _target.Height);
            _device.SetRenderTarget(_target);
        }

        public void renderFullScreen(int x, int y, int width, int height)
        {
            //spriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            //            spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            //            spriteBatch.End();
    
            _device.Viewport = new Microsoft.Xna.Framework.Graphics.Viewport(0, 0, _target.Width, _target.Height);
            _device.SetRenderTarget(null);
            _device.DrawUserPrimitives(PrimitiveType.TriangleList, new VertexPositionColorTexture[] { 
                new VertexPositionColorTexture() { Position = new Vector3(0,0,0), Color = Color.White, TextureCoordinate = new Vector2(0,0)}, 
                new VertexPositionColorTexture() { Position = new Vector3(0,0,0), Color = Color.White, TextureCoordinate = new Vector2(0,0)}, 
                new VertexPositionColorTexture() { Position = new Vector3(0,0,0), Color = Color.White, TextureCoordinate = new Vector2(0,0)}, 
                new VertexPositionColorTexture() { Position = new Vector3(0,0,0), Color = Color.White, TextureCoordinate = new Vector2(0,0)}
            }, 0, 4);
        }
        //public void unbind()
        //{
        //    _device.SetRenderTarget(null);
        //}
    }

    public class GraphicSubsystem2 : GraphicSubsystem
    {
        private Dictionary<string, FBO> _fbos = new Dictionary<string, FBO>();

        protected FBO getFbo(string name)
        {
            return _fbos.get(name);
        }

        public SpriteBatch createBatch()
        {
            return new SpriteBatch(this.graphicsDevice);
        }
    }

    public class GraphicSubsystem : ISubsystem
    {
        private Dictionary<Material, DeferredRenderQueue> _queues = new Dictionary<Material, DeferredRenderQueue>();
        private SpriteBatch _batch;

        public static readonly GameUri Uri = "subsystem:graphics";

        private AssetManager _assets;

        public GraphicsDevice graphicsDevice { get; private set; }

        public int currentRenderQueue = 0;
        public bool scissorEnabled { get; private set; }

        public Rectangle scissorRect { get; private set; }
        
        public Engine.GameUri uri { get { return Uri; } }
       
        public void preInit()
        {

        }

        public void postInit()
        {

            this.graphicsDevice = CoreRegistry.require<DisplayDevice>(DisplayDevice.Uri).device;
            _batch = new SpriteBatch(graphicsDevice);

            _assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);

            _assets.setFactory<MaterialData, Material>(AssetType.MATERIAL, loadMaterial);
            _assets.setFactory<TextureData, Texture2D>(AssetType.TEXTURE, loadTexture);
            var tdata = TextureData.create(1, 1, Color.White);
            _assets.cacheAsset(_assets.createTexture("engine:white", tdata));

            _assets.setFactory<FontData, Font>(AssetType.FONT, loadFont);

            var mdefault = new MaterialData();
            mdefault.SetBlendState(BlendState.Opaque);
            mdefault.SetSamplerState(SamplerState.PointClamp);
            mdefault.texture = "engine:white";
            _assets.cacheAsset(_assets.createMaterial("engine:default", mdefault));

            var madditive = new MaterialData();
            madditive.SetBlendState(BlendState.Additive);
            madditive.SetSamplerState(SamplerState.PointClamp);
            madditive.texture = "engine:white";
            _assets.cacheAsset(_assets.createMaterial("engine:additive", madditive));

        }

        public void begin()
        {
            graphicsDevice.Clear(Color.Black);
            foreach (var items in _queues.Values)
                items.reset();
        }

        public void end(Matrix matrix,  Viewport vp)
        {
            graphicsDevice.Viewport = new Microsoft.Xna.Framework.Graphics.Viewport(vp.X, vp.Y, vp.Width, vp.Height);
            
            foreach (var items in _queues)
            {
                var m = items.Key;
                var q = items.Value;
                m.bind(_batch, matrix);
                foreach (var item in items.Value.items)
                {
                    item.texture.draw(_batch, item);
                }
                m.unbind(_batch);
            }
        }

        private Material loadMaterial(AssetUri uri, MaterialData data)
        {
            return new Material(uri, data, _assets);
        }

        private Texture2D loadTexture(AssetUri uri, TextureData data)
        {
            return new Texture2D(uri, data);
        }

        private Font loadFont(AssetUri uri, FontData data)
        {
            return new Font(uri, data, _assets);
        }

        public void preUpdate(float delta)
        {
        }

        public void postUpdate(float delta)
        {
        }

        public void shutdown()
        {

        }

        internal void DrawInternal(Material material,
                    Vector2 position,
                    Vector2 scale,
                    AxisAlignedBox sourceRectangle,
                    Color color,
                    float rotation,
                    Vector2 origin,
                    SpriteEffects effect,
                    float depth)
        {
            var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
            material = material ?? assets.getMaterial("engine:default");
            if (material != null)
            {
                var _queue = _queues.getOrCreate(material);
                var offset = scale * origin;

                _queue.texture(material.texture);
                _queue.source(sourceRectangle);
                _queue.color(color);
                _queue.depth(depth);
                _queue.quad(position - offset, position + scale - offset, origin, rotation);
            }
            else
            {
            }
        }

        public void Draw(Material material, Vector2 position, AxisAlignedBox sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float depth)
        {
            DrawInternal(material,
                position,
                scale,
                sourceRectangle,
                color,
                rotation,
                origin,
                effect,
                depth);
        }

        public void Draw(Material material, Vector2 position, AxisAlignedBox sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float depth)
        {
            DrawInternal(material,
                position,
                (new Vector2(scale)),
                sourceRectangle,
                color,
                rotation,
                origin,
                effect,
                depth);
        }

        public void Draw(Material material, AxisAlignedBox destinationRectangle, AxisAlignedBox sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effect, float depth)
        {
            DrawInternal(material,
                  (new Vector2(destinationRectangle.X0, destinationRectangle.Y0)),
                  (new Vector2(destinationRectangle.Width, destinationRectangle.Height)),
                  sourceRectangle,
                  color,
                  rotation,
                  origin,
                  effect,
                  depth);
        }

        public void Draw(Material material, Vector2 position, AxisAlignedBox sourceRectangle, Color color)
        {
            Draw(currentRenderQueue, material, position, sourceRectangle, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public void Draw(Material material, AxisAlignedBox destinationRectangle, AxisAlignedBox sourceRectangle, Color color)
        {
            Draw(currentRenderQueue, material, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, 0f);
        }

        public void Draw(Material material, AxisAlignedBox destinationRectangle, AxisAlignedBox sourceRectangle, Color color, float depth)
        {
            Draw(currentRenderQueue, material, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, depth);
        }

        public void Draw(Material material, Vector2 position, Color color)
        {
            Draw(currentRenderQueue, material, position, AxisAlignedBox.Null, color);
        }

        public void Draw(Material material, Vector2 position, float rotation, Vector2 scale, Color color)
        {
            Draw(currentRenderQueue, material, position, AxisAlignedBox.Null, color, rotation, Vector2.One / 2f, scale, SpriteEffects.None, 0f);
        }

        public void Draw(Material material, AxisAlignedBox rectangle, Color color)
        {
            Draw(currentRenderQueue, material, rectangle, AxisAlignedBox.Null, color);
        }

        //public void Draw(Material material, AxisAlignedBox source, AxisAlignedBox rectangle, Color color)
        //{
        //    Draw(currentRenderQueue, material, rectangle, source, color);
        //}

        public void Draw(Material material, AxisAlignedBox rectangle, Vector2 origin, Color color)
        {
            Draw(currentRenderQueue, material, rectangle, AxisAlignedBox.Null, color, 0, Vector2.Zero, SpriteEffects.None, 0f);
        }

        public void Draw(Material material, AxisAlignedBox rectangle, Color color, float depth)
        {
            Draw(currentRenderQueue, material, rectangle, AxisAlignedBox.Null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
        }

        public void Draw(Material material, AxisAlignedBox rectangle, Color color, SpriteEffects effects, float depth)
        {
            Draw(currentRenderQueue, material, rectangle, AxisAlignedBox.Null, color, 0f, Vector2.Zero, effects, depth);
        }

        public void Draw(int renderQueue, Material material, Vector2 position, AxisAlignedBox sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effect, float depth)
        {
            DrawInternal(material,
                position,
                scale,
                sourceRectangle,
                color,
                rotation,
                origin,
                effect,
                depth);
        }

        public void Draw(int renderQueue, Material material, Vector2 position, AxisAlignedBox sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effect, float depth)
        {
            DrawInternal(material,
                position,
                (new Vector2(scale)),
                sourceRectangle,
                color,
                rotation,
                origin,
                effect,
                depth);
        }

        public void Draw(int renderQueue, Material material, AxisAlignedBox destinationRectangle, AxisAlignedBox sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effect, float depth)
        {
            DrawInternal(material,
                  (new Vector2(destinationRectangle.X0, destinationRectangle.Y0)),
                  (new Vector2(destinationRectangle.Width, destinationRectangle.Height)),
                  sourceRectangle,
                  color,
                  rotation,
                  origin,
                  effect,
                  depth);
        }

        public void Draw(int renderQueue, Material material, Vector2 position, AxisAlignedBox sourceRectangle, Color color)
        {
            Draw(renderQueue, material, position, sourceRectangle, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public void Draw(int renderQueue, Material material, AxisAlignedBox destinationRectangle, AxisAlignedBox sourceRectangle, Color color)
        {
            Draw(renderQueue, material, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, 0f);
        }

        public void Draw(int renderQueue, Material material, AxisAlignedBox destinationRectangle, AxisAlignedBox sourceRectangle, Color color, float depth)
        {
            Draw(renderQueue, material, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, depth);
        }

        public void Draw(int renderQueue, Material material, Vector2 position, Color color)
        {
            Draw(renderQueue, material, position, AxisAlignedBox.Null, color);
        }

        public void Draw(int renderQueue, Material material, AxisAlignedBox rectangle, Color color)
        {
            Draw(renderQueue, material, rectangle, AxisAlignedBox.Null, color);
        }

        public void Draw(int renderQueue, Material material, AxisAlignedBox rectangle, Color color, float depth)
        {
            Draw(renderQueue, material, rectangle, AxisAlignedBox.Null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
        }

        public void Draw(int renderQueue, AxisAlignedBox rectangle, Color color, float depth)
        {
            Draw(renderQueue, null, rectangle, AxisAlignedBox.Null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
        }

        public void Draw(AxisAlignedBox rectangle, Color color)
        {
            Draw(currentRenderQueue, null, rectangle, AxisAlignedBox.Null, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        }

        public void Draw(AxisAlignedBox rectangle, Color color, float depth)
        {
            Draw(currentRenderQueue, null, rectangle, AxisAlignedBox.Null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
        }

        public void Draw(int renderQueue, Material material, AxisAlignedBox rectangle, Color color, SpriteEffects effects, float depth)
        {
            Draw(renderQueue, material, rectangle, AxisAlignedBox.Null, color, 0f, Vector2.Zero, effects, depth);
        }

        public void DrawCircle(Material material, Vector2 center, float radius, int segments, Color color)
        {
            DrawCircle(currentRenderQueue, material, center, radius, segments, color);
        }

        public void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            DrawLine(currentRenderQueue, null, start, end, color, 1f, 0);
        }

        public void DrawLine(Material material, Vector2 start, Vector2 end, Color color)
        {
            DrawLine(currentRenderQueue, material, start, end, color, 1f, 0);
        }

        public void DrawLine(Material material, Vector2 start, Vector2 end, Color color, float depth)
        {
            DrawLine(material, start, end, color, depth);
        }

        public void DrawLine(Vector2 start, Vector2 end, Color color, float depth)
        {
            DrawLine(null, start, end, color, depth);
        }

        public void DrawRect(Material material, AxisAlignedBox rect, Color color)
        {
            DrawRect(currentRenderQueue, material, rect.minVector, rect.maxVector, color, 0f);
        }

        public void DrawRect(Material material, AxisAlignedBox rect, Color color, float depth)
        {
            DrawRect(currentRenderQueue, material, rect.minVector, rect.maxVector, color, depth);
        }

        public void DrawRect(Material material, Vector2 p0, Vector2 p1, Color color)
        {
            DrawRect(currentRenderQueue, material, p0, p1, color, 0f);
        }

        public void DrawRect(Material material, Vector2 p0, Vector2 p1, Color color, float depth)
        {
            DrawRect(currentRenderQueue, material, p0, p1, color, depth);
        }

        //public void DrawShape(Material material, Atma.Shape shape, Color color)
        //{
        //    DrawShape(currentRenderQueue, material, shape, color);
        //}

        public void DrawText(Font font, float scale, Vector2 pos, string text, Color color)
        {
            DrawText(currentRenderQueue, font, scale, pos, text, color, 0f);
        }

        public void DrawText(Font font, float scale, Vector2 pos, string text, Color color, float depth)
        {
            font.DrawText(currentRenderQueue, pos, scale, text, color, depth);
        }

        public void DrawCircle(int renderQueue, Material material, Vector2 center, float radius, int segments, Color color)
        {
            var step = MathHelper.TwoPi / segments;

            var lp = new Vector2(Utility.Cos(0), Utility.Sin(0)) * radius + center;
            var p = Vector2.Zero;
            for (var i = 1; i <= segments; i++)
            {
                var current = step * i;
                p.X = Utility.Cos(current) * radius + center.X;
                p.Y = Utility.Sin(current) * radius + center.Y;

                DrawLine(renderQueue, material, lp, p, color, 1f, 1f);

                lp = p;
            }
        }

        public void DrawLine(int renderQueue, Material material, Vector2 start, Vector2 end, Color color)
        {
            DrawLine(renderQueue, material, start, end, color, 1f, 0);
        }

        public void DrawLine(int renderQueue, Material material, Vector2 start, Vector2 end, Color color, float width, float depth)
        {
            //end.Y = -end.Y;
            //start.Y = -start.Y;
            var diff = end - start;
            //var srcRect = new AxisAlignedBox(start.X, start.Y - width * 0.5f, start.X + diff.Length(), start.Y + width * 0.5f);
            var srcRect = new AxisAlignedBox(start.X, start.Y, start.X + width, start.Y + diff.Length());

            diff.Normalize();

            var rotation = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;
            Draw(renderQueue, material, srcRect, AxisAlignedBox.Null, color, rotation, new Vector2(0.5f, 0), SpriteEffects.None, depth);
        }

        public void DrawRect(int renderQueue, Material material, AxisAlignedBox rect, Color color)
        {
            DrawRect(renderQueue, material, rect.minVector, rect.maxVector, color, 0f);
        }

        public void DrawRect(int renderQueue, AxisAlignedBox rect, Color color)
        {
            DrawRect(renderQueue, null, rect.minVector, rect.maxVector, color, 0f);
        }

        public void DrawRect(AxisAlignedBox rect, Color color)
        {
            DrawRect(currentRenderQueue, null, rect.minVector, rect.maxVector, color, 0f);
        }

        public void DrawRect(int renderQueue, Material material, AxisAlignedBox rect, Color color, float depth)
        {
            DrawRect(renderQueue, material, rect.minVector, rect.maxVector, color, depth);
        }

        public void DrawRect(int renderQueue, Material material, Vector2 p0, Vector2 p1, Color color)
        {
            DrawRect(renderQueue, material, p0, p1, color, 0f);
        }

        public void DrawRect(int renderQueue, Vector2 p0, Vector2 p1, Color color)
        {
            DrawRect(renderQueue, null, p0, p1, color, 0f);
        }

        public void DrawRect(int renderQueue, Material material, Vector2 p0, Vector2 p1, Color color, float depth)
        {
            var points = new Vector2[4];
            points[0] = p0.Floor();
            points[2] = p1.Floor();
            points[1] = new Vector2(points[2].X, points[0].Y);
            points[3] = new Vector2(points[0].X, points[2].Y);

            for (var i = 0; i < points.Length; i++)
                DrawLine(renderQueue, material, points[i], points[(i + 1) % points.Length], color, 1f, depth);
        }

        //public void DrawShape(int renderQueue, Material material, Atma.Shape shape, Color color)
        //{
        //    var points = shape.derivedVertices;
        //    for (var i = 0; i < points.Length; i++)
        //        DrawLine(renderQueue, material, points[i], points[(i + 1) % points.Length], color);
        //}

        public void DrawText(int renderQueue, Font font, float scale, Vector2 pos, string text, Color color)
        {
            DrawText(renderQueue, font, scale, pos, text, color, 0f);
        }

        public void DrawText(int renderQueue, Font font, float scale, Vector2 pos, string text, Color color, float depth)
        {
            font.DrawText(renderQueue, pos, scale, text, color, depth);
        }
    }
}
