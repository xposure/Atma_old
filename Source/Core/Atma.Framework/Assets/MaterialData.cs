using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Atma.Assets
{
    public class MaterialData  : IAssetData
    {
        public MaterialData()
        {
            SetSamplerState(SamplerState.PointClamp);
            SetBlendState(BlendState.NonPremultiplied);
            SetRasterizerState(RasterizerState.CullCounterClockwise);
            SetDepthStencilState(DepthStencilState.Default);
        }

        #region BlendState
        public BlendFunction AlphaBlendFunction;
        public BlendFunction ColorBlendFunction;
        public Blend AlphaDestinationBlend;
        public Blend AlphaSourceBlend;
        public Blend ColorDestinationBlend;
        public Blend ColorSourceBlend;
        public Color BlendFactor = Color.White;
        public ColorWriteChannels ColorWriteChannels;
        public ColorWriteChannels ColorWriteChannels1;
        public ColorWriteChannels ColorWriteChannels2;
        public ColorWriteChannels ColorWriteChannels3;
        public int MultiSampleMask = Int32.MaxValue;

        public void SetBlendState(BlendState state)
        {
            AlphaBlendFunction = state.AlphaBlendFunction;
            AlphaDestinationBlend = state.AlphaDestinationBlend;
            AlphaSourceBlend = state.AlphaSourceBlend;
            BlendFactor = state.BlendFactor;
            ColorBlendFunction = state.ColorBlendFunction;
            ColorDestinationBlend = state.ColorDestinationBlend;
            ColorSourceBlend = state.ColorSourceBlend;
            ColorWriteChannels = state.ColorWriteChannels;
            ColorWriteChannels1 = state.ColorWriteChannels;
            ColorWriteChannels2 = state.ColorWriteChannels;
            ColorWriteChannels3 = state.ColorWriteChannels;
            MultiSampleMask = state.MultiSampleMask;

        }

        #endregion

        #region SamplerState
        public TextureAddressMode AddressU = TextureAddressMode.Wrap;
        public TextureAddressMode AddressV = TextureAddressMode.Wrap;
        public TextureAddressMode AddressW = TextureAddressMode.Wrap;
        public TextureFilter Filter = TextureFilter.Linear;
        public int MaxAnisotropy = 4;
        public int MaxMipLevel = 0;
        public float MipMapLevelOfDetailBias = 0;

        public void SetTextureAddressMode(TextureAddressMode t)
        {
            AddressU = t;
            AddressV = t;
            AddressW = t;
        }

        public void SetSamplerState(SamplerState state)
        {
            AddressU = state.AddressU;
            AddressV = state.AddressV;
            AddressW = state.AddressW;
            Filter = state.Filter;
            MaxAnisotropy = state.MaxAnisotropy;
            MaxMipLevel = state.MaxMipLevel;
            MipMapLevelOfDetailBias = state.MipMapLevelOfDetailBias;            
        }
        #endregion

        #region RasterizerState
        private CullMode _cullMode = CullMode.CullCounterClockwiseFace;
        private float _depthBias = 0f;
        private FillMode _fillMode = FillMode.Solid;
        private bool _multiSampleAntiAlias = true;
        private float _slopeScaleDepthBias = 0f;

        public CullMode cullMode { get { return _cullMode; } set { _cullMode = value; } }
        public float depthBias { get { return _depthBias; } set { _depthBias = value; } }
        public FillMode fillMode { get { return _fillMode; } set { _fillMode = value; } }
        public bool multiSampleAntiAlias { get { return _multiSampleAntiAlias; } set { _multiSampleAntiAlias = value; } }
        public float slopeScaleDepthBias { get { return _slopeScaleDepthBias; } set { _slopeScaleDepthBias = value; } }

        public void SetRasterizerState(RasterizerState state)
        {
            _cullMode = state.CullMode;
            _depthBias = state.DepthBias;
            _fillMode = state.FillMode;
            _multiSampleAntiAlias = state.MultiSampleAntiAlias;
            _slopeScaleDepthBias = state.SlopeScaleDepthBias;

        }
        #endregion

        #region DepthStencilState
        private StencilOperation _CounterClockwiseStencilDepthBufferFail;
        private StencilOperation _CounterClockwiseStencilFail;
        private CompareFunction _CounterClockwiseStencilFunction;
        private StencilOperation _CounterClockwiseStencilPass;
        private CompareFunction _DepthBufferFunction;
        private StencilOperation _StencilDepthBufferFail;
        private StencilOperation _StencilPass;
        private StencilOperation _StencilFail;
        private CompareFunction _StencilFunction;
        private bool _DepthBufferEnable;
        private bool _DepthBufferWriteEnable;
        private bool _TwoSidedStencilMode;
        private bool _StencilEnable;
        private int _ReferenceStencil;
        private int _StencilMask;
        private int _StencilWriteMask;

        public StencilOperation CounterClockwiseStencilDepthBufferFail { get { return _CounterClockwiseStencilDepthBufferFail; } set { _CounterClockwiseStencilDepthBufferFail = value; } }
        public StencilOperation CounterClockwiseStencilFail { get { return _CounterClockwiseStencilFail; } set { _CounterClockwiseStencilFail = value; } }
        public CompareFunction CounterClockwiseStencilFunction { get { return _CounterClockwiseStencilFunction; } set { _CounterClockwiseStencilFunction = value; } }
        public StencilOperation CounterClockwiseStencilPass { get { return _CounterClockwiseStencilPass; } set { _CounterClockwiseStencilPass = value; } }
        public CompareFunction DepthBufferFunction { get { return _DepthBufferFunction; } set { _DepthBufferFunction = value; } }
        public StencilOperation StencilDepthBufferFail { get { return _StencilDepthBufferFail; } set { _StencilDepthBufferFail = value; } }
        public StencilOperation StencilPass { get { return _StencilPass; } set { _StencilPass = value; } }
        public StencilOperation StencilFail { get { return _StencilFail; } set { _StencilFail = value; } }
        public CompareFunction StencilFunction { get { return _StencilFunction; } set { _StencilFunction = value; } }
        public bool DepthBufferEnable { get { return _DepthBufferEnable; } set { _DepthBufferEnable = value; } }
        public bool DepthBufferWriteEnable { get { return _DepthBufferWriteEnable; } set { _DepthBufferWriteEnable = value; } }
        public bool TwoSidedStencilMode { get { return _TwoSidedStencilMode; } set { _TwoSidedStencilMode = value; } }
        public bool StencilEnable { get { return _StencilEnable; } set { _StencilEnable = value; } }
        public int ReferenceStencil { get { return _ReferenceStencil; } set { _ReferenceStencil = value; } }
        public int StencilMask { get { return _StencilMask; } set { _StencilMask = value; } }
        public int StencilWriteMask { get { return _StencilWriteMask; } set { _StencilWriteMask = value; } }

        public void SetDepthStencilState(DepthStencilState state)
        {
            _CounterClockwiseStencilDepthBufferFail = state.CounterClockwiseStencilDepthBufferFail;
            _CounterClockwiseStencilFail = state.CounterClockwiseStencilFail;
            _CounterClockwiseStencilFunction = state.CounterClockwiseStencilFunction;
            _CounterClockwiseStencilPass = state.CounterClockwiseStencilPass;
            _DepthBufferEnable = state.DepthBufferEnable;
            _DepthBufferFunction = state.DepthBufferFunction;
            _DepthBufferWriteEnable = state.DepthBufferWriteEnable;
            _ReferenceStencil = state.ReferenceStencil;
            _StencilDepthBufferFail = state.StencilDepthBufferFail;
            _StencilEnable = state.StencilEnable;
            _StencilFail = state.StencilFail;
            _StencilFunction = state.StencilFunction;
            _StencilMask = state.StencilMask;
            _StencilPass = state.StencilPass;
            _StencilWriteMask = state.StencilWriteMask;
            _TwoSidedStencilMode = state.TwoSidedStencilMode;

        }
        #endregion
    }

    public class MaterialDataLoader : IAssetDataLoader<MaterialData>
    {
        //private Atma.Framework.Parsing.Material.Parser _parser;
        //private Atma.Framework.Parsing.Material.Scanner _scanner;

        public MaterialData load(System.IO.Stream stream)
        {
            var _scanner = new Parsing.Material.Scanner(stream);
            var _parser = new Parsing.Material.Parser(_scanner);
            return _parser.parse();            
        }

    }
}
