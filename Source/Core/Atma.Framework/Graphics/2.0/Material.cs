using Atma.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atma.Graphics
{
    public class Material : AbstractAsset<MaterialData>
    {
        private MaterialData _data;
        private BlendState blendState;
        private SamplerState samplerState;
        private RasterizerState rasterizerState;
        private RasterizerState scissorRasterizerState;
        private DepthStencilState depthStencilState;
        private Texture2D _texture;

        public Texture2D texture { get { return _texture; } set { _texture = value; } }

        public int textureHeight { get { return texture == null ? 0 : texture.height; } }

        public int textureWidth { get { return texture == null ? 0 : texture.width; } }

        public Vector2 textureSize { get { return texture == null ? Vector2.Zero : new Vector2(_texture.width, _texture.height); } }

        public bool isTransparent { get { return _data.AlphaDestinationBlend != Blend.One; } }
        
        public Material(AssetUri uri, MaterialData data)
            : base(uri)
        {
            reload(data);
        }

        public override void reload(MaterialData data)
        {
            _data = data;
            setup();
        }

        protected virtual void setup()
        {
            setupBlend();
            setupSampler();
            setupRasterizer();
            setupScissorRasterizer();
            setupDepth();
        }

        private void setupBlend()
        {
            if (blendState != null)
                blendState.Dispose();

            blendState = new BlendState();
            blendState.AlphaBlendFunction = _data.AlphaBlendFunction;
            blendState.AlphaDestinationBlend = _data.AlphaDestinationBlend;
            blendState.AlphaSourceBlend = _data.AlphaSourceBlend;
            blendState.BlendFactor = _data.BlendFactor;//.ToXnaColor();
            blendState.ColorBlendFunction = _data.ColorBlendFunction;
            blendState.ColorDestinationBlend = _data.ColorDestinationBlend;
            blendState.ColorSourceBlend = _data.ColorSourceBlend;
            blendState.ColorWriteChannels = _data.ColorWriteChannels;
            blendState.ColorWriteChannels1 = _data.ColorWriteChannels1;
            blendState.ColorWriteChannels2 = _data.ColorWriteChannels2;
            blendState.ColorWriteChannels3 = _data.ColorWriteChannels3;
            blendState.MultiSampleMask = _data.MultiSampleMask;
        }

        private void setupSampler()
        {
            if (samplerState != null)
                samplerState.Dispose();

            samplerState = new SamplerState();
            samplerState.AddressU = _data.AddressU;
            samplerState.AddressV = _data.AddressV;
            samplerState.AddressW = _data.AddressW;
            samplerState.Filter = _data.Filter;
            samplerState.MaxAnisotropy = _data.MaxAnisotropy;
            samplerState.MaxMipLevel = _data.MaxMipLevel;
            samplerState.MipMapLevelOfDetailBias = _data.MipMapLevelOfDetailBias;
        }

        private void setupRasterizer()
        {
            if (rasterizerState != null)
                rasterizerState.Dispose();

            rasterizerState = new RasterizerState();
            rasterizerState.CullMode = _data.cullMode;
            rasterizerState.DepthBias = _data.depthBias;
            rasterizerState.FillMode = _data.fillMode;
            rasterizerState.MultiSampleAntiAlias = _data.multiSampleAntiAlias;
            rasterizerState.SlopeScaleDepthBias = _data.slopeScaleDepthBias;
        }

        private void setupScissorRasterizer()
        {
            if (scissorRasterizerState != null)
                scissorRasterizerState.Dispose();

            scissorRasterizerState = new RasterizerState();
            scissorRasterizerState.CullMode = _data.cullMode;
            scissorRasterizerState.DepthBias = _data.depthBias;
            scissorRasterizerState.FillMode = _data.fillMode;
            scissorRasterizerState.MultiSampleAntiAlias = _data.multiSampleAntiAlias;
            scissorRasterizerState.SlopeScaleDepthBias = _data.slopeScaleDepthBias;

        }

        private void setupDepth()
        {
            if (depthStencilState != null)
                depthStencilState.Dispose();

            depthStencilState = new DepthStencilState();
            depthStencilState.CounterClockwiseStencilDepthBufferFail = _data.CounterClockwiseStencilDepthBufferFail;
            depthStencilState.CounterClockwiseStencilFail = _data.CounterClockwiseStencilFail;
            depthStencilState.CounterClockwiseStencilFunction = _data.CounterClockwiseStencilFunction;
            depthStencilState.CounterClockwiseStencilPass = _data.CounterClockwiseStencilPass;
            depthStencilState.DepthBufferEnable = _data.DepthBufferEnable;
            depthStencilState.DepthBufferFunction = _data.DepthBufferFunction;
            depthStencilState.DepthBufferWriteEnable = _data.DepthBufferWriteEnable;
            depthStencilState.ReferenceStencil = _data.ReferenceStencil;
            depthStencilState.StencilDepthBufferFail = _data.StencilDepthBufferFail;
            depthStencilState.StencilEnable = _data.StencilEnable;
            depthStencilState.StencilFail = _data.StencilFail;
            depthStencilState.StencilFunction = _data.StencilFunction;
            depthStencilState.StencilMask = _data.StencilMask;
            depthStencilState.StencilPass = _data.StencilPass;
            depthStencilState.StencilWriteMask = _data.StencilWriteMask;
            depthStencilState.TwoSidedStencilMode = _data.TwoSidedStencilMode;
        }

        public virtual void enable()
        {
            var mgl = Atma.MonoGame.Graphics.MonoGL.instance;
            mgl.blendState = blendState;
            mgl.samplerState = samplerState;
            mgl.depthState = depthStencilState;
            mgl.rasterizerState = rasterizerState;
            mgl.scissorRasterizerState = scissorRasterizerState;
            mgl.materialState = this;
            //mgl.texture(texture);
        }

        //internal void begin(SpriteBatch batch, Matrix currentMatrix)
        //{
        //    batch.Begin(SpriteSortMode.Deferred, blendState, samplerState, depthStencilState, rasterizerState, null, currentMatrix);

        //}

        protected override void ondispose()
        {
            if (blendState != null) blendState.Dispose();
            if (samplerState != null) samplerState.Dispose();
            if (rasterizerState != null) rasterizerState.Dispose();
            if (scissorRasterizerState != null) scissorRasterizerState.Dispose();
            if (depthStencilState != null) depthStencilState.Dispose();

            blendState = null;
            samplerState = null;
            rasterizerState = null;
            scissorRasterizerState = null;
            depthStencilState = null;
        }
    }
}
