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
        private BlendFunction _AlphaBlendFunction;
        private Blend _AlphaDestinationBlend;
        private Blend _AlphaSourceBlend;
        private Color _BlendFactor = Color.White;
        private BlendFunction _ColorBlendFunction;
        private Blend _ColorDestinationBlend;
        private Blend _ColorSourceBlend;
        private ColorWriteChannels _ColorWriteChannels;
        private ColorWriteChannels _ColorWriteChannels1;
        private ColorWriteChannels _ColorWriteChannels2;
        private ColorWriteChannels _ColorWriteChannels3;
        private int _MultiSampleMask = Int32.MaxValue;

        public BlendFunction AlphaBlendFunction { get { return _AlphaBlendFunction; } set { _AlphaBlendFunction = value; } }
        public Blend AlphaDestinationBlend { get { return _AlphaDestinationBlend; } set { _AlphaDestinationBlend = value; } }
        public Blend AlphaSourceBlend { get { return _AlphaSourceBlend; } set { _AlphaSourceBlend = value; } }
        public Color BlendFactor { get { return _BlendFactor; } set { _BlendFactor = value; } }
        public BlendFunction ColorBlendFunction { get { return _ColorBlendFunction; } set { _ColorBlendFunction = value; } }
        public Blend ColorDestinationBlend { get { return _ColorDestinationBlend; } set { _ColorDestinationBlend = value; } }
        public Blend ColorSourceBlend { get { return _ColorSourceBlend; } set { _ColorSourceBlend = value; } }
        public ColorWriteChannels ColorWriteChannels { get { return _ColorWriteChannels; } set { _ColorWriteChannels = value; } }
        public ColorWriteChannels ColorWriteChannels1 { get { return _ColorWriteChannels1; } set { _ColorWriteChannels1 = value; } }
        public ColorWriteChannels ColorWriteChannels2 { get { return _ColorWriteChannels2; } set { _ColorWriteChannels2 = value; } }
        public ColorWriteChannels ColorWriteChannels3 { get { return _ColorWriteChannels3; } set { _ColorWriteChannels3 = value; } }
        public int MultiSampleMask { get { return _MultiSampleMask; } set { _MultiSampleMask = value; } }

        public void SetBlendState(BlendState state)
        {
            _AlphaBlendFunction = state.AlphaBlendFunction;
            _AlphaDestinationBlend = state.AlphaDestinationBlend;
            _AlphaSourceBlend = state.AlphaSourceBlend;
            _BlendFactor = state.BlendFactor;
            _ColorBlendFunction = state.ColorBlendFunction;
            _ColorDestinationBlend = state.ColorDestinationBlend;
            _ColorSourceBlend = state.ColorSourceBlend;
            _ColorWriteChannels = state.ColorWriteChannels;
            _ColorWriteChannels1 = state.ColorWriteChannels;
            _ColorWriteChannels2 = state.ColorWriteChannels;
            _ColorWriteChannels3 = state.ColorWriteChannels;
            _MultiSampleMask = state.MultiSampleMask;

        }

        #endregion

        #region SamplerState
        private TextureAddressMode _AddressU = TextureAddressMode.Wrap;
        private TextureAddressMode _AddressV = TextureAddressMode.Wrap;
        private TextureAddressMode _AddressW = TextureAddressMode.Wrap;
        private TextureFilter _Filter = TextureFilter.Linear;
        private int _MaxAnisotropy = 4;
        private int _MaxMipLevel = 0;
        private float _MipMapLevelOfDetailBias = 0;

        public TextureAddressMode AddressU { get { return _AddressU; } set { _AddressU = value; } }
        public TextureAddressMode AddressV { get { return _AddressV; } set { _AddressV = value; } }
        public TextureAddressMode AddressW { get { return _AddressW; } set { _AddressW = value; } }
        public TextureFilter Filter { get { return _Filter; } set { _Filter = value; } }
        public int MaxAnisotropy { get { return _MaxAnisotropy; } set { _MaxAnisotropy = value; } }
        public int MaxMipLevel { get { return _MaxMipLevel; } set { _MaxMipLevel = value; } }
        public float MipMapLevelOfDetailBias { get { return _MipMapLevelOfDetailBias; } set { _MipMapLevelOfDetailBias = value; } }

        public void SetSamplerState(SamplerState state)
        {
            _AddressU = state.AddressU;
            _AddressV = state.AddressV;
            _AddressW = state.AddressW;
            _Filter = state.Filter;
            _MaxAnisotropy = state.MaxAnisotropy;
            _MaxMipLevel = state.MaxMipLevel;
            _MipMapLevelOfDetailBias = state.MipMapLevelOfDetailBias;

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
}
