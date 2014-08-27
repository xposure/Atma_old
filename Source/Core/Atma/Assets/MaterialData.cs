//using Atma;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;


//namespace Atma
//{
//    public class MaterialData  : IAssetData
//    {
//        public MaterialData()
//        {
//            SetSamplerState(SamplerState.PointClamp);
//            SetBlendState(BlendState.NonPremultiplied);
//            SetRasterizerState(RasterizerState.CullCounterClockwise);
//            SetDepthStencilState(DepthStencilState.Default);
//        }

//        public GameUri texture;

//        #region BlendState
//        public BlendFunction AlphaBlendFunction;
//        public BlendFunction ColorBlendFunction;
//        public Blend AlphaDestinationBlend;
//        public Blend AlphaSourceBlend;
//        public Blend ColorDestinationBlend;
//        public Blend ColorSourceBlend;
//        public Color BlendFactor = Color.White;
//        public ColorWriteChannels ColorWriteChannels;
//        public ColorWriteChannels ColorWriteChannels1;
//        public ColorWriteChannels ColorWriteChannels2;
//        public ColorWriteChannels ColorWriteChannels3;
//        public int MultiSampleMask = Int32.MaxValue;

//        public void SetBlendState(BlendState state)
//        {
//            AlphaBlendFunction = state.AlphaBlendFunction;
//            AlphaDestinationBlend = state.AlphaDestinationBlend;
//            AlphaSourceBlend = state.AlphaSourceBlend;
//            BlendFactor = state.BlendFactor;
//            ColorBlendFunction = state.ColorBlendFunction;
//            ColorDestinationBlend = state.ColorDestinationBlend;
//            ColorSourceBlend = state.ColorSourceBlend;
//            ColorWriteChannels = state.ColorWriteChannels;
//            ColorWriteChannels1 = state.ColorWriteChannels;
//            ColorWriteChannels2 = state.ColorWriteChannels;
//            ColorWriteChannels3 = state.ColorWriteChannels;
//            MultiSampleMask = state.MultiSampleMask;

//        }

//        #endregion

//        #region SamplerState
//        public TextureAddressMode AddressU = TextureAddressMode.Wrap;
//        public TextureAddressMode AddressV = TextureAddressMode.Wrap;
//        public TextureAddressMode AddressW = TextureAddressMode.Wrap;
//        public TextureFilter Filter = TextureFilter.Linear;
//        public int MaxAnisotropy = 4;
//        public int MaxMipLevel = 0;
//        public float MipMapLevelOfDetailBias = 0;

//        public void SetTextureAddressMode(TextureAddressMode t)
//        {
//            AddressU = t;
//            AddressV = t;
//            AddressW = t;
//        }

//        public void SetSamplerState(SamplerState state)
//        {
//            AddressU = state.AddressU;
//            AddressV = state.AddressV;
//            AddressW = state.AddressW;
//            Filter = state.Filter;
//            MaxAnisotropy = state.MaxAnisotropy;
//            MaxMipLevel = state.MaxMipLevel;
//            MipMapLevelOfDetailBias = state.MipMapLevelOfDetailBias;            
//        }
//        #endregion

//        #region RasterizerState
//        public CullMode CullMode = CullMode.CullCounterClockwiseFace;
//        public float DepthBias = 0f;
//        public FillMode FillMode = FillMode.Solid;
//        public bool MultiSampleAntiAlias = true;
//        public float SlopeScaleDepthBias = 0f;

//        public void SetRasterizerState(RasterizerState state)
//        {
//            CullMode = state.CullMode;
//            DepthBias = state.DepthBias;
//            FillMode = state.FillMode;
//            MultiSampleAntiAlias = state.MultiSampleAntiAlias;
//            SlopeScaleDepthBias = state.SlopeScaleDepthBias;

//        }
//        #endregion

//        #region DepthStencilState
//        public StencilOperation CounterClockwiseStencilDepthBufferFail;
//        public StencilOperation CounterClockwiseStencilFail;
//        public StencilOperation CounterClockwiseStencilPass;
//        public StencilOperation StencilDepthBufferFail;
//        public StencilOperation StencilPass;
//        public StencilOperation StencilFail;
//        public CompareFunction StencilFunction;
//        public CompareFunction CounterClockwiseStencilFunction;
//        public CompareFunction DepthBufferFunction;
//        public bool DepthBufferEnable;
//        public bool DepthBufferWriteEnable;
//        public bool TwoSidedStencilMode;
//        public bool StencilEnable;
//        public int ReferenceStencil;
//        public int StencilMask;
//        public int StencilWriteMask;

//        public void SetDepthStencilState(DepthStencilState state)
//        {
//            CounterClockwiseStencilDepthBufferFail = state.CounterClockwiseStencilDepthBufferFail;
//            CounterClockwiseStencilFail = state.CounterClockwiseStencilFail;
//            CounterClockwiseStencilFunction = state.CounterClockwiseStencilFunction;
//            CounterClockwiseStencilPass = state.CounterClockwiseStencilPass;
//            DepthBufferEnable = state.DepthBufferEnable;
//            DepthBufferFunction = state.DepthBufferFunction;
//            DepthBufferWriteEnable = state.DepthBufferWriteEnable;
//            ReferenceStencil = state.ReferenceStencil;
//            StencilDepthBufferFail = state.StencilDepthBufferFail;
//            StencilEnable = state.StencilEnable;
//            StencilFail = state.StencilFail;
//            StencilFunction = state.StencilFunction;
//            StencilMask = state.StencilMask;
//            StencilPass = state.StencilPass;
//            StencilWriteMask = state.StencilWriteMask;
//            TwoSidedStencilMode = state.TwoSidedStencilMode;

//        }
//        #endregion
//    }

//    public class MaterialDataLoader : IAssetDataLoader<MaterialData>
//    {
//        //private Atma.Framework.Parsing.Material.Parser _parser;
//        //private Atma.Framework.Parsing.Material.Scanner _scanner;

//        public MaterialData load(System.IO.Stream stream)
//        {
//            var _scanner = new Parsing.Material.Scanner(stream);
//            var _parser = new Parsing.Material.Parser(_scanner);
//            return _parser.parse();            
//        }

//    }
//}
