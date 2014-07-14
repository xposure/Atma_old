using Atma.Assets;
using Atma.Core;
using Atma.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atma.Graphics
{
    public class Material : AbstractAsset<MaterialData>
    {
        private static readonly Logger logger = Logger.getLogger(typeof(Material));

        //private MaterialData _data;
        private AssetManager _manager;
        private BlendState blendState;
        private SamplerState samplerState;
        private RasterizerState rasterizerState;
        private RasterizerState scissorRasterizerState;
        private DepthStencilState depthStencilState;
        private Texture2D _texture;

        public Texture2D texture { get { return _texture; } set { setupTexture(value); } }

        public int textureHeight { get { return _texture == null ? 0 : _texture.height; } }

        public int textureWidth { get { return _texture == null ? 0 : _texture.width; } }

        public Vector2 textureSize { get { return _texture == null ? Vector2.Zero : new Vector2(_texture.width, _texture.height); } }

        public bool isTransparent { get; private set; }
        
        public Material(AssetUri uri, MaterialData data, AssetManager manager)
            : base(uri)
        {
            _manager = manager;
            reload(data);
        }

        public override void reload(MaterialData data)
        {
            setup(data);
        }

        protected virtual void setup(MaterialData _data)
        {
            isTransparent = _data.AlphaDestinationBlend != Blend.One;
            setupTexture(_data.texture);
            setupBlend(_data);
            setupSampler(_data);
            setupRasterizer(_data);
            setupScissorRasterizer(_data);
            setupDepth(_data);
        }

        protected virtual void setupTexture(GameUri uri)
        {
            if (!uri.isValid())
                failTexture(uri);
            else
            {
                var tex = _manager.getTexture(uri);
                if (tex == null)
                    failTexture(uri);
                else
                    setupTexture(tex);
            }
        }

        protected void failTexture(GameUri uri)
        {
            logger.error("failed to load texture {0}", uri);
            
            var basewhite = _manager.getTexture("engine:basewhite");
            if (basewhite == null)
                logger.error("engine:basewhite could not be found");
            else
                setupTexture(basewhite);
        }

        protected virtual void setupTexture(Texture2D tex)
        {
            _texture = tex;
        }

        private void setupBlend(MaterialData _data)
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

        private void setupSampler(MaterialData _data)
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

        private void setupRasterizer(MaterialData _data)
        {
            if (rasterizerState != null)
                rasterizerState.Dispose();

            rasterizerState = new RasterizerState();
            rasterizerState.CullMode = _data.CullMode;
            rasterizerState.DepthBias = _data.DepthBias;
            rasterizerState.FillMode = _data.FillMode;
            rasterizerState.MultiSampleAntiAlias = _data.MultiSampleAntiAlias;
            rasterizerState.SlopeScaleDepthBias = _data.SlopeScaleDepthBias;
        }

        private void setupScissorRasterizer(MaterialData _data)
        {
            if (scissorRasterizerState != null)
                scissorRasterizerState.Dispose();

            scissorRasterizerState = new RasterizerState();
            scissorRasterizerState.CullMode = _data.CullMode;
            scissorRasterizerState.DepthBias = _data.DepthBias;
            scissorRasterizerState.FillMode = _data.FillMode;
            scissorRasterizerState.MultiSampleAntiAlias = _data.MultiSampleAntiAlias;
            scissorRasterizerState.SlopeScaleDepthBias = _data.SlopeScaleDepthBias;
            scissorRasterizerState.ScissorTestEnable = true;
        }

        private void setupDepth(MaterialData _data)
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

        //public virtual void bind()
        //{
        //    var mgl = Atma.MonoGame.Graphics.MonoGL.instance;
        //    mgl.blendState = blendState;
        //    mgl.samplerState = samplerState;
        //    mgl.depthState = depthStencilState;
        //    mgl.rasterizerState = rasterizerState;
        //    mgl.scissorRasterizerState = scissorRasterizerState;
        //    mgl.materialState = this;
        //    //mgl.texture(texture);


        //}
        public virtual void bind(SpriteBatch batch, Matrix m)
        {
            batch.Begin(SpriteSortMode.Deferred, blendState, samplerState, depthStencilState, rasterizerState, null, m);
        }

        public void unbind(SpriteBatch batch)
        {
            batch.End();
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
