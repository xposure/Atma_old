using Atma.Assets;
using Atma.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Texture2D = Atma.Graphics.Texture2D;
using XnaBlendState = Microsoft.Xna.Framework.Graphics.BlendState;
using XnaDepthStencilState = Microsoft.Xna.Framework.Graphics.DepthStencilState;
using XnaMath = Microsoft.Xna.Framework;
using XnaRasterizerState = Microsoft.Xna.Framework.Graphics.RasterizerState;
using XnaSamplerState = Microsoft.Xna.Framework.Graphics.SamplerState;

namespace Atma.MonoGame.Graphics
{
    public class MonoGL : GL
    {
        public static MonoGL instance { get; private set; }

        internal Texture2D baseWhite { get; private set; }
        internal Material _defaultMaterial;

        //private XnaMath.Matrix currentMatrix = XnaMath.Matrix.Identity;

        internal GraphicsDevice _device;
        internal SpriteBatch batch;
        private float _maxDepth = 0f;
        internal float _minDepth = 0f;
        internal float depthRange = 1f;
        private int spritesSubmittedThisBatch = 0;
        public int spritesSubmittedThisFrame = 0;
        public int drawCallsThisFrame = 0;
        public int spritesSubmittedLastFrame = 0;
        public int drawCallsLastFrame = 0;
        public List<string> materialsRenderedThisFrame = new List<string>();
        public List<string> materialsRenderedLastFrame = new List<string>();

        private bool applyingScissor;
        internal Atma.Graphics.Material materialState;

        internal XnaBlendState blendState;
        internal XnaDepthStencilState depthState;
        internal XnaRasterizerState rasterizerState;
        internal XnaRasterizerState scissorRasterizerState;
        internal XnaSamplerState samplerState;
        internal Effect effect = null;

        public MonoGL(GraphicsDevice device)
        {
            _device = device;
            batch = new SpriteBatch(device);

            baseWhite = new Texture2D("asset:texture:basewhite", 1, 1);
            baseWhite.setData(new XnaMath.Color[] { XnaMath.Color.White });
            _defaultMaterial = new Material("asset:material:default", new MaterialData());
            _defaultMaterial.texture = baseWhite;
        }

        protected internal override Material defaultMaterial { get { return _defaultMaterial; } }
        protected internal override Texture2D defaultTexture { get { return baseWhite; } }

        public override void begin(IRenderTarget target, SortMode mode, Matrix viewMatrix, Atma.Graphics.Viewport viewport)
        {
            instance = this;
            //Matrix4.Compose(new Vector3(-(int)position.X, -(int)position.Y, 0), Vector3.UnitScale, Quaternion.Zero);
            _device.Viewport = new XnaMath.Graphics.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);

            base.begin(target, mode, viewMatrix, viewport);

            _maxDepth = 0f;
            _minDepth = 0f;
        }

        public override void depth(float amount)
        {
            base.depth(amount);
            _minDepth = Utility.Min(amount, _minDepth);
            _maxDepth = Utility.Max(amount, _maxDepth);
        }

        protected override void onclear(Color color)
        {
            _device.Clear(color);
        }

        protected override void onend(GLRenderable[] renderableItems, int length)
        {

            instance = this;

            var start = 0;
            var depthRange = 1f;
            if (_maxDepth != _minDepth)
                depthRange = (_maxDepth - _minDepth);

            for (var i = start; i < start + length; i++)
            {
                var item = renderableItems[i];
                {
                    if (item.material.uri.isValid())
                    {
                        spritesSubmittedThisFrame++;
                        if (updateBatchState(item)) //we changed states
                            materialsRenderedThisFrame.Add(item.material.uri);

                        var texture = item.texture ?? baseWhite;
                        texture.draw(item);

                        spritesSubmittedThisBatch++;
                    }
                }
            }

            endBatchState();
        }

        public override void resetstatistics()
        {
            base.resetstatistics();
            drawCallsLastFrame = drawCallsThisFrame;
            spritesSubmittedLastFrame = spritesSubmittedThisFrame;

            drawCallsThisFrame = 0;
            spritesSubmittedThisFrame = 0;
            spritesSubmittedThisBatch = 0;

            materialsRenderedLastFrame.Clear();
            materialsRenderedLastFrame.AddRange(materialsRenderedThisFrame);
            materialsRenderedThisFrame.Clear();

        }

        private void endBatchState()
        {
            spritesSubmittedThisBatch = 0;
            if (materialState != null)
            {
                batch.End();

                if (applyingScissor)
                {
                    batch.GraphicsDevice.ScissorRectangle = new XnaMath.Rectangle(0, 0, 800, 480);
                    applyingScissor = false;
                }

                drawCallsThisFrame++;
                materialState = null;
            }
        }

        //private HashSet<string> crap = new HashSet<string>();
        internal void setBatchState(GLRenderable item)
        {
            endBatchState();

            item.material.enable();

            var _rasterizer = rasterizerState;
            if (item.applyScissor)
            {
                _rasterizer = scissorRasterizerState;
                batch.GraphicsDevice.ScissorRectangle = new XnaMath.Rectangle((int)item.scissorRect.X0, (int)item.scissorRect.Y0, (int)item.scissorRect.Width, (int)item.scissorRect.Height);
                applyingScissor = true;
            }

            var width = _device.PresentationParameters.Bounds.Width;
            var height = _device.PresentationParameters.Bounds.Height;
            var position = Vector2.Zero;
            var orientation = 0f;
            var scale = Vector2.One;

            var viewMatrix =
               XnaMath.Matrix.CreateTranslation(new XnaMath.Vector3(-(int)position.X, -(int)position.Y, 0)) *
               XnaMath.Matrix.CreateRotationZ(orientation) *
               XnaMath.Matrix.CreateScale(scale.X, scale.Y, 1) *
               XnaMath.Matrix.CreateTranslation(new XnaMath.Vector3(width * 0.5f, height * 0.5f, 0));
            //Matrix4.CreateTranslation(new Vector3(_viewport.Width * 0.5f, _viewport.Height * 0.5f, 0));

            var viewMatrix2 =
               Matrix.CreateTranslation(new Vector3(-(int)position.X, -(int)position.Y, 0)) *
               Matrix.CreateRotationZ(orientation) *
               Matrix.CreateScale(scale.X, scale.Y, 1) *
               Matrix.CreateTranslation(new Vector3(width * 0.5f, height * 0.5f, 0));

            //item.material.begin(batch, currentMatrix);
            //Matrix4.CreateTranslation(new Vector3(_viewport.Width * 0.5f, _viewport.Height * 0.5f, 0));
            batch.Begin(SpriteSortMode.Deferred, blendState, samplerState, depthState, _rasterizer, effect, currentMatrix);
        }

        private bool updateBatchState(GLRenderable item)
        {
            if (materialState == null)
                setBatchState(item);
            else if (materialState != item.material)
                setBatchState(item);
            else if (item.applyScissor != applyingScissor)
                setBatchState(item);
            else if (item.applyScissor && applyingScissor && !checkScissorRect(item.scissorRect))
                setBatchState(item);
            else if ((spritesSubmittedThisBatch % 2048) == 0)
                setBatchState(item);
            else
                return false;
            //            else
            //                setBatchState(item);
            return true;
        }

        private bool checkScissorRect(AxisAlignedBox aabb)
        {
            var scissor = aabb.ToRect();
            return scissor == batch.GraphicsDevice.ScissorRectangle;
        }
    }
}
