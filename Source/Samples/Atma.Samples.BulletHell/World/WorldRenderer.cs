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
    public class WorldRenderer : GameSystemInstance<WorldRenderer>
    {
        public static readonly GameUri Uri = "common:worldrenderer";

        private bool recreateBuffers = false;

        private OrthoCamera _currentCamera;
        private BlendState blendShadows;

        public OrthoCamera currentCamera { get { return _currentCamera; } }
        private OrthoCamera quadCamera = new OrthoCamera();

        public virtual void init()
        {
            setInstance(this);

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

        public void shutdown()
        {
            this.setInstance(null);
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

            display.device.DepthStencilState = DepthStencilState.None;
            display.device.BlendState = BlendState.NonPremultiplied;
            graphics.drawFbo("opaque");

            display.device.BlendState = BlendState.AlphaBlend;
            graphics.drawFbo("shadows");
            graphics.drawFbo("overlay");
        }

        private void debugRender()
        {
            quadCamera.normalizedOrigin = Vector2.Zero;
            quadCamera.lookThrough();

            display.device.DepthStencilState = DepthStencilState.None;
            display.device.BlendState = BlendState.Opaque;
            graphics.drawFbo("opaque", 0, 0, 1024 / 2, 768 / 2);
            graphics.drawFbo("opaque", 0, 768 / 2, 1024 / 2, 768 / 2);

            display.device.BlendState = BlendState.AlphaBlend;
            graphics.drawFbo("shadows", 1024 / 2, 0, 1024 / 2, 768 / 2);
            graphics.drawFbo("shadows", 0, 768 / 2, 1024 / 2, 768 / 2);
            graphics.drawFbo("overlay", 0, 768 / 2, 1024 / 2, 768 / 2);
        }

        protected virtual void renderOpaque(IRenderSystem[] listeners)
        {
            display.device.BlendState = BlendState.AlphaBlend;
            display.device.DepthStencilState = DepthStencilState.Default;
            display.device.RasterizerState = RasterizerState.CullCounterClockwise;
            display.device.SamplerStates[0] = SamplerState.PointClamp;

            //_graphics.begin(SpriteSortMode.Texture, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, viewMatrix);
            graphics.bindFbo("opaque");

            display.device.Clear(Color.LightGray);
            foreach (var l in listeners)
                l.renderOpaque();

            //_graphics.end();
        }

        protected virtual void renderAlpha(IRenderSystem[] listeners)
        {
            display.device.BlendState = BlendState.AlphaBlend;
            display.device.DepthStencilState = DepthStencilState.DepthRead;
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
            display.device.BlendState = BlendState.AlphaBlend;
            display.device.DepthStencilState = DepthStencilState.DepthRead;
            graphics.bindFbo("overlay");
            display.device.Clear(Color.Transparent);
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

            graphics.bindFbo("shadows");
            //_graphics.
            //_graphics.graphicsDevice.Clear(Color.FromNonPremultiplied(255, 255, 255, 0));
            display.device.BlendState = blendShadows;
            display.device.DepthStencilState = DepthStencilState.None;
            display.device.SamplerStates[0] = SamplerState.AnisotropicClamp;

            display.device.Clear(Color.Black);

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
                var listeners = components.getSystemsByInterface<IRenderSystem>().ToArray();
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

                        //PerformanceMonitor.start("render shadows");
                        //renderShadows(listeners);
                        //PerformanceMonitor.end("render shadows");

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
                var width = display.width;
                var height = display.height;

                graphics.createFbo("opaque", width, height, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);
                graphics.createFbo("shadows", width, height, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);
                //_graphics.createFbo("alpha", width, height, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.DiscardContents);
                graphics.createFbo("overlay", width, height, false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);
            }
        }

    }



}
