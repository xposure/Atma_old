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
        private BlendState blendShadows;

        public OrthoCamera currentCamera { get { return _currentCamera; } }
        private OrthoCamera quadCamera = new OrthoCamera();

        public virtual void init()
        {
            _display = this.display();
            _graphics = this.graphics();
            _components = this.components();
            _entities = this.entities();

            _currentCamera = new OrthoCamera();

            blendShadows = new BlendState()
            {
                ColorSourceBlend = Blend.SourceAlpha,
                ColorDestinationBlend = Blend.InverseSourceAlpha,
                ColorBlendFunction = BlendFunction.Add,

                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.One,
                AlphaBlendFunction = BlendFunction.ReverseSubtract
                //, ColorWriteChannels = ColorWriteChannels.Alpha
            };

            recreateBuffers = true;
            setup();

        }

        protected virtual void preRender()
        {
            _currentCamera.lookThrough();

        }

        protected virtual void postRender()
        {
            doRender();
            //debugRender();
        }

        private void doRender()
        {
            quadCamera.normalizedOrigin = Vector2.Zero;
            quadCamera.lookThrough();

            _display.device.DepthStencilState = DepthStencilState.None;
            _display.device.BlendState = BlendState.NonPremultiplied;
            _graphics.drawFbo("opaque");

            _display.device.BlendState = BlendState.AlphaBlend;
            _graphics.drawFbo("shadows");
            _graphics.drawFbo("overlay");
        }

        private void debugRender()
        {
            quadCamera.normalizedOrigin = Vector2.Zero;
            quadCamera.lookThrough();

            _display.device.DepthStencilState = DepthStencilState.None;
            _display.device.BlendState = BlendState.Opaque;
            _graphics.drawFbo("opaque", 0, 0, 1024 / 2, 768 / 2);
            _graphics.drawFbo("opaque", 0, 768 / 2, 1024 / 2, 768 / 2);

            _display.device.BlendState = BlendState.AlphaBlend;
            _graphics.drawFbo("shadows", 1024 / 2, 0, 1024 / 2, 768 / 2);
            _graphics.drawFbo("shadows", 0, 768 / 2, 1024 / 2, 768 / 2);
            _graphics.drawFbo("overlay", 0, 768 / 2, 1024 / 2, 768 / 2);
        }

        protected virtual void renderOpaque(IRenderSystem[] listeners)
        {
            _display.device.BlendState = BlendState.Opaque;
            _display.device.DepthStencilState = DepthStencilState.Default;
            _display.device.RasterizerState = RasterizerState.CullCounterClockwise;
            _display.device.SamplerStates[0] = SamplerState.PointClamp;

            //_graphics.begin(SpriteSortMode.Texture, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, viewMatrix);
            _graphics.bindFbo("opaque");

            _graphics.graphicsDevice.Clear(Color.LightGray);
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
            _display.device.BlendState = BlendState.AlphaBlend;
            _display.device.DepthStencilState = DepthStencilState.DepthRead;
            _graphics.bindFbo("overlay");
            _display.device.Clear(Color.Transparent);
            //_display.device.RasterizerState = RasterizerState.CullCounterClockwise;
            //_display.device.SamplerStates[0] = SamplerState.PointClamp;

            //_graphics.begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwise, null, viewMatrix);
            //_graphics.bindFbo("alpha");
            foreach (var l in listeners)
                l.renderOverlay();
            //_graphics.end();
        }

        protected virtual void renderShadows(IRenderSystem[] listeners)
        {
            //_graphics.begin(SpriteSortMode.Texture, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, viewMatrix);

            _graphics.bindFbo("shadows");
            //_graphics.
            //_graphics.graphicsDevice.Clear(Color.FromNonPremultiplied(255, 255, 255, 0));
            _display.device.BlendState = blendShadows;
            _display.device.DepthStencilState = DepthStencilState.None;
            _display.device.SamplerStates[0] = SamplerState.AnisotropicClamp;

            _graphics.graphicsDevice.Clear(Color.Black);

            foreach (var l in listeners)
                l.renderShadows();

            //_display.device.BlendState = BlendState.Opaque;
            //_display.device.DepthStencilState = DepthStencilState.None;
            //_display.device.BlendState = blendShadows;
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

                        PerformanceMonitor.start("render shadows");
                        renderShadows(listeners);
                        PerformanceMonitor.end("render shadows");

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

                _graphics.createFbo("opaque", width, height, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);
                _graphics.createFbo("shadows", width, height, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);
                //_graphics.createFbo("alpha", width, height, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.DiscardContents);
                _graphics.createFbo("overlay", width, height, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);
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
