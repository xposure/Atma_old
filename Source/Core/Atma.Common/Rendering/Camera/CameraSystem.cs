//using Atma.Engine;
//using Atma.Entity;
//using Atma.Graphics;
//using Atma.Systems;
//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Xna = Microsoft.Xna.Framework;

//namespace Atma.Rendering.Camera
//{

//    public class CameraSystem : IComponentSystem, IRenderSubscriber
//    {
//        public static readonly GameUri Uri = "system:camera";

//        private DisplayDevice _display;
//        private Microsoft.Xna.Framework.Graphics.SpriteBatch _batch;

//        //private RenderQueue _opaqueQueue = new TextureRenderQueue();
//        //private RenderQueue _alphaRejectQueue = new TextureRenderQueue();
//        //private RenderQueue _alphaQueue = new DepthRenderQueue();
//        //private RenderQueue _additiveQueue = new TextureRenderQueue();

//        //public event Action<CameraOld> prepareToRender;
//        //public event Action<CameraOld, RenderQueue> renderOpaque;
//        //public event Action<CameraOld, RenderQueue> renderAlphaReject;
//        //public event Action<CameraOld, RenderQueue> renderAlpha;
//        //public event Action<CameraOld, RenderQueue> renderAdditive;

//        public int draws = 0;

//        public void render()
//        {
//            draws = 0;

//            var graphics = CoreRegistry.require<GraphicSubsystem>(GraphicSubsystem.Uri);
//            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            
//            foreach (var id in em.getWithComponents("camera", "transform"))
//            {
//                var entity = em.createRef(id);
//                var camera = entity.getComponent<CameraOld>("camera");
//                var transform = entity.getComponent<Transform>("transform");

//                camera.ReCreateViewMatrix();

//                var matrix = camera.ViewMatrix;
//                var viewport = camera.viewport;

//                _display.device.Viewport = new Xna.Graphics.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);
//                _display.device.Clear(camera.clear);

//                if (prepareToRender != null)
//                {
//                    prepareToRender(camera);
//                }

//                if (renderOpaque != null)
//                {
//                    _opaqueQueue.reset();
//                    renderOpaque(camera, _opaqueQueue);
                                        
//                    _batch.Begin(Xna.Graphics.SpriteSortMode.Texture,
//                        Xna.Graphics.BlendState.Opaque,
//                        Xna.Graphics.SamplerState.PointClamp,
//                        Xna.Graphics.DepthStencilState.Default,
//                        Xna.Graphics.RasterizerState.CullCounterClockwise, null, matrix);

//                    foreach (var item in _opaqueQueue.items)
//                    {
//                        item.texture.draw(_batch, item);
//                        draws++;
//                    }
                    
//                    _batch.End();
//                }

//                if (renderAlphaReject != null)
//                {
//                    //graphics.begin();
//                    //renderAlphaReject(camera, _alphaRejectQueue);
//                    //graphics.renderQueue(_alphaRejectQueue);
//                    //graphics.end(matrix, viewport);
//                }

//                if (renderAlpha != null)
//                {
//                    //graphics.begin();
//                    //renderAlpha(camera, _alphaQueue);
//                    //graphics.renderQueue(_alphaQueue);
//                    //graphics.end(matrix, viewport);
//                }

//                if (renderAdditive != null)
//                {
//                    _additiveQueue.reset();
//                    renderAdditive(camera, _additiveQueue);

//                    _batch.Begin(Xna.Graphics.SpriteSortMode.Texture,
//                        Xna.Graphics.BlendState.Additive,
//                        Xna.Graphics.SamplerState.PointClamp,
//                        Xna.Graphics.DepthStencilState.Default,
//                        Xna.Graphics.RasterizerState.CullCounterClockwise, null, matrix);

//                    foreach (var item in _additiveQueue.items)
//                    {
//                        item.texture.draw(_batch, item);
//                        draws++;
//                    }

//                    _batch.End();
//                }
//            }

//            //var gui = CoreRegistry.require<GUIManager
//        }

//        public void init()
//        {
//            _display = CoreRegistry.require<DisplayDevice>(DisplayDevice.Uri);
//            _batch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(_display.device);
//        }

//        public void shutdown()
//        {
//        }
//    }
//}
