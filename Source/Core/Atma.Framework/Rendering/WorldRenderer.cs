using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Engine;
using Atma.Entities;
using Atma.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atma.Rendering
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

        public void init()
        {
            _display = this.display();
            _graphics = this.graphics();
            _components = this.components();
            _entities = this.entities();

            _currentCamera = new OrthoCamera();

            recreateBuffers = true;
            setup();

        }

        public void render()
        {
            PerformanceMonitor.start("world render");
            {
                var listeners = _components.getSystemsByInterface<IRenderSystem>().ToArray();
                //foreach (var id in _entities.getWithComponents("camera"))
                {
                    _currentCamera.lookThrough();
                    PerformanceMonitor.start("render camera");
                    {
                        PerformanceMonitor.start("render opaque");
                        //_graphics.begin(SpriteSortMode.Texture, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, viewMatrix);
                        //_graphics.bindFbo("opaque");
                        foreach (var l in listeners)
                            l.renderOpaque();
                        //_graphics.end();
                        PerformanceMonitor.end("render opaque");

                        PerformanceMonitor.start("render alpha");
                        //_graphics.begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwise, null, viewMatrix);
                        //_graphics.bindFbo("alpha");
                        foreach (var l in listeners)
                            l.renderAlphaBlend();
                        //_graphics.end();
                        PerformanceMonitor.end("render alpha");

                        PerformanceMonitor.start("render overlay");
                        //_graphics.begin(SpriteSortMode.Texture, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, viewMatrix);
                        //_graphics.bindFbo("overlay");
                        foreach (var l in listeners)
                            l.renderOverlay();
                        //_graphics.end();
                        PerformanceMonitor.end("render overlay");
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

        public void beginOpaque()
        {
            _graphics.begin(SpriteSortMode.Texture, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null);
        }

        public void endOpaque()
        {
            _graphics.end();
        }

        public void beginAlphaBlend()
        {
            _graphics.begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwise, null);
        }

        public void endAlphaBlend()
        {
            _graphics.end();
        }

        public void beginAdditive()
        {
            _graphics.begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.DepthRead, RasterizerState.CullCounterClockwise, null);
        }

        public void endAdditive()
        {
            _graphics.end();
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
