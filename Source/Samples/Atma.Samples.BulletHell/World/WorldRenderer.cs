using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Entities;
using Atma.Graphics;
using Atma.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atma.Samples.BulletHell
{
    public class WorldRenderer : ICore
    {
        public static readonly GameUri Uri = "common:worldrenderer";

        private bool recreateBuffers = false;

        private EntityManager _entities;
        private DisplayDevice _display;
        private GraphicSubsystem _graphics;
        private ComponentSystemManager _components;
        private OrthoCamera _currentCamera;

        public OrthoCamera currentCamera { get { return _currentCamera; } }

        public virtual void init()
        {
            _display = this.display();
            _graphics = this.graphics();
            _components = this.components();
            _entities = this.entities();

            _currentCamera = new OrthoCamera();

            recreateBuffers = true;
            setup();

        }

        protected virtual void preRender()
        {
            _currentCamera.lookThrough();

        }

        protected virtual void postRender() { }

        protected virtual void renderOpaque(IRenderSystem[] listeners)
        {
            _display.device.BlendState = BlendState.Opaque;
            _display.device.DepthStencilState = DepthStencilState.Default;
            _display.device.RasterizerState = RasterizerState.CullCounterClockwise;
            _display.device.SamplerStates[0] = SamplerState.PointClamp;

            //_graphics.begin(SpriteSortMode.Texture, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, viewMatrix);
            //_graphics.bindFbo("opaque");
            foreach (var l in listeners)
                l.renderOpaque();
            //_graphics.end();
        }

        protected virtual void renderAlpha(IRenderSystem[] listeners)
        {
            _display.device.BlendState = BlendState.AlphaBlend;
            _display.device.DepthStencilState = DepthStencilState.DepthRead;
            //_display.device.RasterizerState = RasterizerState.CullCounterClockwise;
            //_display.device.SamplerStates[0] = SamplerState.PointClamp;

            //_graphics.begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwise, null, viewMatrix);
            //_graphics.bindFbo("alpha");
            foreach (var l in listeners)
                l.renderAlphaBlend();
            //_graphics.end();
        }

        protected virtual void renderOverlay(IRenderSystem[] listeners)
        {
            //_graphics.begin(SpriteSortMode.Texture, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, viewMatrix);
            //_graphics.bindFbo("overlay");
            foreach (var l in listeners)
                l.renderOverlay();
            //_graphics.end();
        }

        public void render()
        {
            PerformanceMonitor.start("world render");
            {
                var listeners = _components.getSystemsByInterface<IRenderSystem>().ToArray();
                //foreach (var id in _entities.getWithComponents("camera"))
                {
                    PerformanceMonitor.start("render camera");
                    {
                        preRender();

                        PerformanceMonitor.start("render opaque");
                        renderOpaque(listeners);
                        PerformanceMonitor.end("render opaque");

                        PerformanceMonitor.start("render alpha");
                        renderAlpha(listeners);
                        PerformanceMonitor.end("render alpha");

                        PerformanceMonitor.start("render overlay");
                        renderOverlay(listeners);
                        PerformanceMonitor.end("render overlay");

                        postRender();
                    }
                    PerformanceMonitor.end("render camera");
                }
            }
            PerformanceMonitor.end("world render");
        }

        private void setup()
        {
            if (recreateBuffers)
            {
                var width = _display.width;
                var height = _display.height;

                _graphics.createFbo("opaque", width, height, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.DiscardContents);
                //_graphics.createFbo("alpha", width, height, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.DiscardContents);
                //_graphics.createFbo("overlay", width, height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            }
        }

    }

    public static class WorldRendererExtension
    {
        public static WorldRenderer world(this ICore e)
        {
            return CoreRegistry.require<WorldRenderer>(WorldRenderer.Uri);
        }
    }

}
