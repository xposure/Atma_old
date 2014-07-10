
using System;
using Xna = Microsoft.Xna.Framework;
using XnaGraphics = Microsoft.Xna.Framework.Graphics;

namespace Atma.Parsing.Material
{
public partial class Parser {
	public const int _EOF = 0;
	public const int _IDENT = 1;
	public const int _URI = 2;
	public const int _FLOAT = 3;
	public const int _INT = 4;
	public const int maxT = 126;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;



	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void Transform() {
		Expect(5);
		Expect(6);
		while (StartOf(1)) {
			if (StartOf(2)) {
				Sampler();
			} else if (StartOf(3)) {
				Blend();
			} else if (StartOf(4)) {
				Rasterizer();
			} else if (StartOf(5)) {
				Stencil();
			} else {
				Texture();
			}
		}
		Expect(7);
	}

	void Sampler() {
		switch (la.kind) {
		case 110: {
			SamplerState();
			break;
		}
		case 116: {
			AddressU();
			break;
		}
		case 117: {
			AddressW();
			break;
		}
		case 118: {
			AddressV();
			break;
		}
		case 119: {
			Address();
			break;
		}
		case 121: {
			MaxAnisotropy();
			break;
		}
		case 122: {
			MaxMipLevel();
			break;
		}
		case 123: {
			MipMapLevelOfDetailBias();
			break;
		}
		default: SynErr(127); break;
		}
	}

	void Blend() {
		switch (la.kind) {
		case 71: {
			AlphaBlend();
			break;
		}
		case 77: {
			ColorBlend();
			break;
		}
		case 78: {
			AlphaSourceBlend();
			break;
		}
		case 79: {
			AlphaDestinationBlend();
			break;
		}
		case 80: {
			ColorSourceBlend();
			break;
		}
		case 81: {
			ColorDestinationBlend();
			break;
		}
		case 82: {
			BlendFactor();
			break;
		}
		case 83: {
			ColorWriteChannels();
			break;
		}
		case 76: {
			MultiSampleMask();
			break;
		}
		case 67: {
			BlendState();
			break;
		}
		default: SynErr(128); break;
		}
	}

	void Rasterizer() {
		switch (la.kind) {
		case 46: {
			RasterizerState();
			break;
		}
		case 50: {
			DepthBias();
			break;
		}
		case 51: {
			SlopeScaleDepthBias();
			break;
		}
		case 52: case 53: case 54: {
			MultiSampleAntiAlias();
			break;
		}
		case 55: case 56: {
			CullMode();
			break;
		}
		case 62: case 63: {
			FillMode();
			break;
		}
		default: SynErr(129); break;
		}
	}

	void Stencil() {
		switch (la.kind) {
		case 10: {
			StencilState();
			break;
		}
		case 14: {
			DepthBufferEnable();
			break;
		}
		case 15: {
			DepthBufferWriteEnable();
			break;
		}
		case 20: {
			TwoSidedStencilMode();
			break;
		}
		case 16: {
			StencilEnable();
			break;
		}
		case 17: {
			StencilMask();
			break;
		}
		case 18: {
			ReferenceStencil();
			break;
		}
		case 19: {
			StencilWriteMask();
			break;
		}
		case 21: {
			CounterClockwiseStencilDepthBufferFail();
			break;
		}
		case 22: {
			CounterClockwiseStencilFail();
			break;
		}
		case 23: {
			CounterClockwiseStencilPass();
			break;
		}
		case 24: {
			StencilDepthBufferFail();
			break;
		}
		case 25: {
			StencilPass();
			break;
		}
		case 26: {
			StencilFail();
			break;
		}
		case 35: {
			StencilFunction();
			break;
		}
		case 36: {
			CounterClockwiseStencilFunction();
			break;
		}
		case 37: {
			DepthBufferFunction();
			break;
		}
		default: SynErr(130); break;
		}
	}

	void Texture() {
		if (la.kind == 8) {
			Get();
		} else if (la.kind == 9) {
			Get();
		} else SynErr(131);
		Uri(ref _material.texture);
	}

	void Uri(ref Atma.Engine.GameUri uri) {
		Expect(2);
		uri = t.val; 
	}

	void StencilState() {
		Expect(10);
		StencilStates();
	}

	void DepthBufferEnable() {
		Expect(14);
		Bool(ref _material.DepthBufferEnable);
	}

	void DepthBufferWriteEnable() {
		Expect(15);
		Bool(ref _material.DepthBufferWriteEnable);
	}

	void TwoSidedStencilMode() {
		Expect(20);
		Bool(ref _material.TwoSidedStencilMode);
	}

	void StencilEnable() {
		Expect(16);
		Bool(ref _material.StencilEnable);
	}

	void StencilMask() {
		Expect(17);
		Int(ref _material.StencilMask);
	}

	void ReferenceStencil() {
		Expect(18);
		Int(ref _material.ReferenceStencil);
	}

	void StencilWriteMask() {
		Expect(19);
		Int(ref _material.StencilWriteMask);
	}

	void CounterClockwiseStencilDepthBufferFail() {
		Expect(21);
		StencilOperation(ref _material.CounterClockwiseStencilDepthBufferFail);
	}

	void CounterClockwiseStencilFail() {
		Expect(22);
		StencilOperation(ref _material.CounterClockwiseStencilFail);
	}

	void CounterClockwiseStencilPass() {
		Expect(23);
		StencilOperation(ref _material.CounterClockwiseStencilPass);
	}

	void StencilDepthBufferFail() {
		Expect(24);
		StencilOperation(ref _material.StencilDepthBufferFail);
	}

	void StencilPass() {
		Expect(25);
		StencilOperation(ref _material.StencilPass);
	}

	void StencilFail() {
		Expect(26);
		StencilOperation(ref _material.StencilFail);
	}

	void StencilFunction() {
		Expect(35);
		CompareFunction(ref _material.StencilFunction);
	}

	void CounterClockwiseStencilFunction() {
		Expect(36);
		CompareFunction(ref _material.CounterClockwiseStencilFunction);
	}

	void DepthBufferFunction() {
		Expect(37);
		CompareFunction(ref _material.DepthBufferFunction);
	}

	void StencilStates() {
		if (la.kind == 11) {
			Get();
			_material.SetDepthStencilState(XnaGraphics.DepthStencilState.Default); 
		} else if (la.kind == 12) {
			Get();
			_material.SetDepthStencilState(XnaGraphics.DepthStencilState.DepthRead); 
		} else if (la.kind == 13) {
			Get();
			_material.SetDepthStencilState(XnaGraphics.DepthStencilState.None); 
		} else SynErr(132);
	}

	void Bool(ref bool b) {
		if (la.kind == 84 || la.kind == 124) {
			if (la.kind == 84) {
				Get();
			} else {
				Get();
			}
			b = false; 
		} else if (la.kind == 85 || la.kind == 125) {
			if (la.kind == 85) {
				Get();
			} else {
				Get();
			}
			b = true; 
		} else SynErr(133);
	}

	void Int(ref int i) {
		Expect(4);
		i = Convert.ToInt32(t.val); 
	}

	void StencilOperation(ref XnaGraphics.StencilOperation s) {
		switch (la.kind) {
		case 27: {
			Get();
			s = XnaGraphics.StencilOperation.Keep; 
			break;
		}
		case 28: {
			Get();
			s = XnaGraphics.StencilOperation.Zero; 
			break;
		}
		case 29: {
			Get();
			s = XnaGraphics.StencilOperation.Replace; 
			break;
		}
		case 30: {
			Get();
			s = XnaGraphics.StencilOperation.Increment; 
			break;
		}
		case 31: {
			Get();
			s = XnaGraphics.StencilOperation.Decrement; 
			break;
		}
		case 32: {
			Get();
			s = XnaGraphics.StencilOperation.IncrementSaturation; 
			break;
		}
		case 33: {
			Get();
			s = XnaGraphics.StencilOperation.DecrementSaturation; 
			break;
		}
		case 34: {
			Get();
			s = XnaGraphics.StencilOperation.Invert; 
			break;
		}
		default: SynErr(134); break;
		}
	}

	void CompareFunction(ref XnaGraphics.CompareFunction c) {
		switch (la.kind) {
		case 38: {
			Get();
			c = XnaGraphics.CompareFunction.Always; 
			break;
		}
		case 39: {
			Get();
			c = XnaGraphics.CompareFunction.Never; 
			break;
		}
		case 40: {
			Get();
			c = XnaGraphics.CompareFunction.Less; 
			break;
		}
		case 41: {
			Get();
			c = XnaGraphics.CompareFunction.LessEqual; 
			break;
		}
		case 42: {
			Get();
			c = XnaGraphics.CompareFunction.Equal; 
			break;
		}
		case 43: {
			Get();
			c = XnaGraphics.CompareFunction.GreaterEqual; 
			break;
		}
		case 44: {
			Get();
			c = XnaGraphics.CompareFunction.Greater; 
			break;
		}
		case 45: {
			Get();
			c = XnaGraphics.CompareFunction.NotEqual; 
			break;
		}
		default: SynErr(135); break;
		}
	}

	void RasterizerState() {
		Expect(46);
		RasterizerStates();
	}

	void DepthBias() {
		Expect(50);
		Float(ref _material.DepthBias);
	}

	void SlopeScaleDepthBias() {
		Expect(51);
		Float(ref _material.SlopeScaleDepthBias);
	}

	void MultiSampleAntiAlias() {
		if (la.kind == 52) {
			Get();
		} else if (la.kind == 53) {
			Get();
		} else if (la.kind == 54) {
			Get();
		} else SynErr(136);
		Bool(ref _material.MultiSampleAntiAlias);
	}

	void CullMode() {
		if (la.kind == 55) {
			Get();
		} else if (la.kind == 56) {
			Get();
		} else SynErr(137);
		CullModes();
	}

	void FillMode() {
		if (la.kind == 62) {
			Get();
		} else if (la.kind == 63) {
			Get();
		} else SynErr(138);
		FillModes();
	}

	void RasterizerStates() {
		if (la.kind == 47) {
			Get();
			_material.SetRasterizerState(XnaGraphics.RasterizerState.CullClockwise); 
		} else if (la.kind == 48) {
			Get();
			_material.SetRasterizerState(XnaGraphics.RasterizerState.CullCounterClockwise); 
		} else if (la.kind == 49) {
			Get();
			_material.SetRasterizerState(XnaGraphics.RasterizerState.CullNone); 
		} else SynErr(139);
	}

	void Float(ref float f) {
		Expect(3);
		f = Convert.ToSingle(t.val); 
	}

	void CullModes() {
		if (la.kind == 13) {
			Get();
			_material.CullMode = XnaGraphics.CullMode.None; 
		} else if (la.kind == 57 || la.kind == 58) {
			if (la.kind == 57) {
				Get();
			} else {
				Get();
			}
			_material.CullMode = XnaGraphics.CullMode.CullClockwiseFace; 
		} else if (la.kind == 59 || la.kind == 60 || la.kind == 61) {
			if (la.kind == 59) {
				Get();
			} else if (la.kind == 60) {
				Get();
			} else {
				Get();
			}
			_material.CullMode = XnaGraphics.CullMode.CullCounterClockwiseFace; 
		} else SynErr(140);
	}

	void FillModes() {
		if (la.kind == 64) {
			Get();
			_material.FillMode = XnaGraphics.FillMode.Solid; 
		} else if (la.kind == 65 || la.kind == 66) {
			if (la.kind == 65) {
				Get();
			} else {
				Get();
			}
			_material.FillMode = XnaGraphics.FillMode.WireFrame; 
		} else SynErr(141);
	}

	void AlphaBlend() {
		Expect(71);
		BlendFunction(ref _material.AlphaBlendFunction);
	}

	void ColorBlend() {
		Expect(77);
		BlendFunction(ref _material.ColorBlendFunction);
	}

	void AlphaSourceBlend() {
		Expect(78);
		BlendMethod(ref _material.AlphaSourceBlend);
	}

	void AlphaDestinationBlend() {
		Expect(79);
		BlendMethod(ref _material.AlphaDestinationBlend);
	}

	void ColorSourceBlend() {
		Expect(80);
		BlendMethod(ref _material.ColorSourceBlend);
	}

	void ColorDestinationBlend() {
		Expect(81);
		BlendMethod(ref _material.ColorDestinationBlend);
	}

	void BlendFactor() {
		Expect(82);
		Color4(ref _material.BlendFactor);
	}

	void ColorWriteChannels() {
		Expect(83);
		if (la.kind == 84) {
			Get();
			ColorWriteChannel(ref _material.ColorWriteChannels);
		} else if (la.kind == 85) {
			Get();
			ColorWriteChannel(ref _material.ColorWriteChannels1);
		} else if (la.kind == 86) {
			Get();
			ColorWriteChannel(ref _material.ColorWriteChannels2);
		} else if (la.kind == 87) {
			Get();
			ColorWriteChannel(ref _material.ColorWriteChannels3);
		} else SynErr(142);
	}

	void MultiSampleMask() {
		Expect(76);
		Int(ref _material.MultiSampleMask);
	}

	void BlendState() {
		Expect(67);
		BlendStates();
	}

	void BlendStates() {
		if (la.kind == 68 || la.kind == 69) {
			if (la.kind == 68) {
				Get();
			} else {
				Get();
			}
			_material.SetBlendState(XnaGraphics.BlendState.Additive); 
		} else if (la.kind == 70 || la.kind == 71) {
			if (la.kind == 70) {
				Get();
			} else {
				Get();
			}
			_material.SetBlendState(XnaGraphics.BlendState.AlphaBlend); 
		} else if (la.kind == 72 || la.kind == 73 || la.kind == 74) {
			if (la.kind == 72) {
				Get();
			} else if (la.kind == 73) {
				Get();
			} else {
				Get();
			}
			_material.SetBlendState(XnaGraphics.BlendState.NonPremultiplied); 
		} else if (la.kind == 75) {
			Get();
			_material.SetBlendState(XnaGraphics.BlendState.Opaque); 
		} else SynErr(143);
	}

	void BlendFunction(ref XnaGraphics.BlendFunction r) {
		if (la.kind == 68) {
			Get();
			r = XnaGraphics.BlendFunction.Add; 
		} else if (la.kind == 93) {
			Get();
			r = XnaGraphics.BlendFunction.Subtract; 
		} else if (la.kind == 94) {
			Get();
			r = XnaGraphics.BlendFunction.ReverseSubtract; 
		} else if (la.kind == 95) {
			Get();
			r = XnaGraphics.BlendFunction.Max; 
		} else if (la.kind == 96) {
			Get();
			r = XnaGraphics.BlendFunction.Min; 
		} else SynErr(144);
	}

	void BlendMethod(ref XnaGraphics.Blend r) {
		switch (la.kind) {
		case 85: case 97: {
			if (la.kind == 97) {
				Get();
			} else {
				Get();
			}
			r = XnaGraphics.Blend.One; 
			break;
		}
		case 28: case 84: {
			if (la.kind == 28) {
				Get();
			} else {
				Get();
			}
			r = XnaGraphics.Blend.Zero; 
			break;
		}
		case 98: {
			Get();
			r = XnaGraphics.Blend.SourceColor; 
			break;
		}
		case 99: {
			Get();
			r = XnaGraphics.Blend.InverseSourceColor; 
			break;
		}
		case 100: {
			Get();
			r = XnaGraphics.Blend.SourceAlpha; 
			break;
		}
		case 101: {
			Get();
			r = XnaGraphics.Blend.InverseSourceAlpha; 
			break;
		}
		case 102: {
			Get();
			r = XnaGraphics.Blend.DestinationColor; 
			break;
		}
		case 103: {
			Get();
			r = XnaGraphics.Blend.InverseDestinationColor; 
			break;
		}
		case 104: {
			Get();
			r = XnaGraphics.Blend.DestinationAlpha; 
			break;
		}
		case 105: {
			Get();
			r = XnaGraphics.Blend.InverseDestinationAlpha; 
			break;
		}
		case 82: {
			Get();
			r = XnaGraphics.Blend.BlendFactor; 
			break;
		}
		case 106: case 107: {
			if (la.kind == 106) {
				Get();
			} else {
				Get();
			}
			r = XnaGraphics.Blend.InverseBlendFactor; 
			break;
		}
		case 108: case 109: {
			if (la.kind == 108) {
				Get();
			} else {
				Get();
			}
			r = XnaGraphics.Blend.SourceAlphaSaturation; 
			break;
		}
		default: SynErr(145); break;
		}
	}

	void Color4(ref Xna.Color c) {
		float a=1f, r=1f, g=1f, b=1f; 
		Float(ref r);
		Float(ref b);
		Float(ref g);
		if (la.kind == 3) {
			Float(ref a);
		}
		c = new Xna.Color(r, g, b, a); 
	}

	void ColorWriteChannel(ref XnaGraphics.ColorWriteChannels c) {
		c = XnaGraphics.ColorWriteChannels.None; 
		ColorWriteChannelValue(ref c);
		while (StartOf(6)) {
			ColorWriteChannelValue(ref c);
		}
	}

	void ColorWriteChannelValue(ref XnaGraphics.ColorWriteChannels c) {
		switch (la.kind) {
		case 13: {
			Get();
			break;
		}
		case 88: {
			Get();
			c |= XnaGraphics.ColorWriteChannels.Red; 
			break;
		}
		case 89: {
			Get();
			c |= XnaGraphics.ColorWriteChannels.Green; 
			break;
		}
		case 90: {
			Get();
			c |= XnaGraphics.ColorWriteChannels.Blue; 
			break;
		}
		case 91: {
			Get();
			c |= XnaGraphics.ColorWriteChannels.Alpha; 
			break;
		}
		case 92: {
			Get();
			c |= XnaGraphics.ColorWriteChannels.All; 
			break;
		}
		default: SynErr(146); break;
		}
	}

	void SamplerState() {
		Expect(110);
		SamplerStates();
	}

	void AddressU() {
		Expect(116);
		TextureAddressMode(ref _material.AddressU);
	}

	void AddressW() {
		Expect(117);
		TextureAddressMode(ref _material.AddressW);
	}

	void AddressV() {
		Expect(118);
		TextureAddressMode(ref _material.AddressV);
	}

	void Address() {
		Expect(119);
		var t = XnaGraphics.TextureAddressMode.Wrap; 
		TextureAddressMode(ref t);
		_material.SetTextureAddressMode(t); 
	}

	void MaxAnisotropy() {
		Expect(121);
		Int(ref _material.MaxAnisotropy);
	}

	void MaxMipLevel() {
		Expect(122);
		Int(ref _material.MaxMipLevel);
	}

	void MipMapLevelOfDetailBias() {
		Expect(123);
		Float(ref _material.MipMapLevelOfDetailBias);
	}

	void SamplerStates() {
		if (la.kind == 111) {
			Get();
			if (la.kind == 112) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.AnisotropicWrap); 
			} else if (la.kind == 113) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.AnisotropicClamp); 
			} else SynErr(147);
		} else if (la.kind == 114) {
			Get();
			if (la.kind == 112) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.LinearWrap); 
			} else if (la.kind == 113) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.LinearClamp); 
			} else SynErr(148);
		} else if (la.kind == 115) {
			Get();
			if (la.kind == 112) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.PointWrap); 
			} else if (la.kind == 113) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.PointClamp); 
			} else SynErr(149);
		} else SynErr(150);
	}

	void TextureAddressMode(ref XnaGraphics.TextureAddressMode t) {
		if (la.kind == 112) {
			Get();
			t = XnaGraphics.TextureAddressMode.Wrap; 
		} else if (la.kind == 113) {
			Get();
			t = XnaGraphics.TextureAddressMode.Clamp; 
		} else if (la.kind == 120) {
			Get();
			t = XnaGraphics.TextureAddressMode.Mirror; 
		} else SynErr(151);
	}



	protected void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Transform();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, T,T,T,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,T, T,T,x,x, x,x,x,x, x,x,T,x, x,x,T,T, T,T,T,T, T,x,x,x, x,x,T,T, x,x,x,T, x,x,x,T, x,x,x,x, T,T,T,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,T,T,T, x,T,T,T, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,T,T,T, x,T,T,T, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, T,T,T,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,T, T,T,T,T, T,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	//public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "IDENT expected"; break;
			case 2: s = "URI expected"; break;
			case 3: s = "FLOAT expected"; break;
			case 4: s = "INT expected"; break;
			case 5: s = "\"material\" expected"; break;
			case 6: s = "\"{\" expected"; break;
			case 7: s = "\"}\" expected"; break;
			case 8: s = "\"tex\" expected"; break;
			case 9: s = "\"texture\" expected"; break;
			case 10: s = "\"depth\" expected"; break;
			case 11: s = "\"default\" expected"; break;
			case 12: s = "\"read\" expected"; break;
			case 13: s = "\"none\" expected"; break;
			case 14: s = "\"depth_check\" expected"; break;
			case 15: s = "\"depth_write\" expected"; break;
			case 16: s = "\"depth_stencil\" expected"; break;
			case 17: s = "\"stencil_mask\" expected"; break;
			case 18: s = "\"stencil_ref\" expected"; break;
			case 19: s = "\"stencil_write\" expected"; break;
			case 20: s = "\"stencil_two_sided\" expected"; break;
			case 21: s = "\"depth_counter_fail\" expected"; break;
			case 22: s = "\"stencil_counter_fail\" expected"; break;
			case 23: s = "\"stencil_counter_pass\" expected"; break;
			case 24: s = "\"depth_fail\" expected"; break;
			case 25: s = "\"stencil_pass\" expected"; break;
			case 26: s = "\"stencil_fail\" expected"; break;
			case 27: s = "\"keep\" expected"; break;
			case 28: s = "\"zero\" expected"; break;
			case 29: s = "\"replace\" expected"; break;
			case 30: s = "\"inc\" expected"; break;
			case 31: s = "\"dec\" expected"; break;
			case 32: s = "\"inc_sat\" expected"; break;
			case 33: s = "\"dec_sat\" expected"; break;
			case 34: s = "\"invert\" expected"; break;
			case 35: s = "\"stencil_func\" expected"; break;
			case 36: s = "\"counter_stencil_func\" expected"; break;
			case 37: s = "\"depth_func\" expected"; break;
			case 38: s = "\"always\" expected"; break;
			case 39: s = "\"never\" expected"; break;
			case 40: s = "\"less\" expected"; break;
			case 41: s = "\"less_eq\" expected"; break;
			case 42: s = "\"eq\" expected"; break;
			case 43: s = "\"greater_eq\" expected"; break;
			case 44: s = "\"greater\" expected"; break;
			case 45: s = "\"not_eq\" expected"; break;
			case 46: s = "\"rasterizer\" expected"; break;
			case 47: s = "\"cullclockwise\" expected"; break;
			case 48: s = "\"cullcounterclockwise\" expected"; break;
			case 49: s = "\"cullnone\" expected"; break;
			case 50: s = "\"depth_bias\" expected"; break;
			case 51: s = "\"slope_Depth_bias\" expected"; break;
			case 52: s = "\"multi_alias\" expected"; break;
			case 53: s = "\"multi_sample\" expected"; break;
			case 54: s = "\"anti_alias\" expected"; break;
			case 55: s = "\"cull\" expected"; break;
			case 56: s = "\"cull_mode\" expected"; break;
			case 57: s = "\"clock\" expected"; break;
			case 58: s = "\"clockwise\" expected"; break;
			case 59: s = "\"counter\" expected"; break;
			case 60: s = "\"counter_clock\" expected"; break;
			case 61: s = "\"counter_clockwise\" expected"; break;
			case 62: s = "\"fill\" expected"; break;
			case 63: s = "\"fill_mode\" expected"; break;
			case 64: s = "\"solid\" expected"; break;
			case 65: s = "\"wire\" expected"; break;
			case 66: s = "\"wire_frame\" expected"; break;
			case 67: s = "\"blend\" expected"; break;
			case 68: s = "\"add\" expected"; break;
			case 69: s = "\"additive\" expected"; break;
			case 70: s = "\"alpha\" expected"; break;
			case 71: s = "\"alpha_blend\" expected"; break;
			case 72: s = "\"non\" expected"; break;
			case 73: s = "\"pre\" expected"; break;
			case 74: s = "\"nonpremultiplied\" expected"; break;
			case 75: s = "\"opaque\" expected"; break;
			case 76: s = "\"multi_sample_mask\" expected"; break;
			case 77: s = "\"color_blend\" expected"; break;
			case 78: s = "\"alpha_src\" expected"; break;
			case 79: s = "\"alpha_dst\" expected"; break;
			case 80: s = "\"color_src\" expected"; break;
			case 81: s = "\"color_dst\" expected"; break;
			case 82: s = "\"blend_factor\" expected"; break;
			case 83: s = "\"write_channel\" expected"; break;
			case 84: s = "\"0\" expected"; break;
			case 85: s = "\"1\" expected"; break;
			case 86: s = "\"2\" expected"; break;
			case 87: s = "\"3\" expected"; break;
			case 88: s = "\"red\" expected"; break;
			case 89: s = "\"green\" expected"; break;
			case 90: s = "\"blue\" expected"; break;
			case 91: s = "\"Alpha\" expected"; break;
			case 92: s = "\"all\" expected"; break;
			case 93: s = "\"subtract\" expected"; break;
			case 94: s = "\"reversesubtract\" expected"; break;
			case 95: s = "\"max\" expected"; break;
			case 96: s = "\"min\" expected"; break;
			case 97: s = "\"one\" expected"; break;
			case 98: s = "\"src_color\" expected"; break;
			case 99: s = "\"inv_src_color\" expected"; break;
			case 100: s = "\"src_alpha\" expected"; break;
			case 101: s = "\"inv_src_alpha\" expected"; break;
			case 102: s = "\"dst_color\" expected"; break;
			case 103: s = "\"inv_dst_color\" expected"; break;
			case 104: s = "\"dst_alpha\" expected"; break;
			case 105: s = "\"inv_dst_alpha\" expected"; break;
			case 106: s = "\"inv_blend\" expected"; break;
			case 107: s = "\"inv_blend_factor\" expected"; break;
			case 108: s = "\"src_alpha_sat\" expected"; break;
			case 109: s = "\"src_alpha_saturation\" expected"; break;
			case 110: s = "\"sampler\" expected"; break;
			case 111: s = "\"anisotropic\" expected"; break;
			case 112: s = "\"wrap\" expected"; break;
			case 113: s = "\"clamp\" expected"; break;
			case 114: s = "\"linear\" expected"; break;
			case 115: s = "\"point\" expected"; break;
			case 116: s = "\"addressu\" expected"; break;
			case 117: s = "\"addressw\" expected"; break;
			case 118: s = "\"addressv\" expected"; break;
			case 119: s = "\"address\" expected"; break;
			case 120: s = "\"mirror\" expected"; break;
			case 121: s = "\"MaxAnisotropy\" expected"; break;
			case 122: s = "\"MaxMipLevel\" expected"; break;
			case 123: s = "\"MipMapLevelOfDetailBias\" expected"; break;
			case 124: s = "\"false\" expected"; break;
			case 125: s = "\"true\" expected"; break;
			case 126: s = "??? expected"; break;
			case 127: s = "invalid Sampler"; break;
			case 128: s = "invalid Blend"; break;
			case 129: s = "invalid Rasterizer"; break;
			case 130: s = "invalid Stencil"; break;
			case 131: s = "invalid Texture"; break;
			case 132: s = "invalid StencilStates"; break;
			case 133: s = "invalid Bool"; break;
			case 134: s = "invalid StencilOperation"; break;
			case 135: s = "invalid CompareFunction"; break;
			case 136: s = "invalid MultiSampleAntiAlias"; break;
			case 137: s = "invalid CullMode"; break;
			case 138: s = "invalid FillMode"; break;
			case 139: s = "invalid RasterizerStates"; break;
			case 140: s = "invalid CullModes"; break;
			case 141: s = "invalid FillModes"; break;
			case 142: s = "invalid ColorWriteChannels"; break;
			case 143: s = "invalid BlendStates"; break;
			case 144: s = "invalid BlendFunction"; break;
			case 145: s = "invalid BlendMethod"; break;
			case 146: s = "invalid ColorWriteChannelValue"; break;
			case 147: s = "invalid SamplerStates"; break;
			case 148: s = "invalid SamplerStates"; break;
			case 149: s = "invalid SamplerStates"; break;
			case 150: s = "invalid SamplerStates"; break;
			case 151: s = "invalid TextureAddressMode"; break;

			default: s = "error " + n; break;
		}
		Parser.logger.error(errMsgFormat, line, col, s);
		//errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		Parser.logger.error(errMsgFormat, line, col, s);
		//errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		Parser.logger.error(s);
		//errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		Parser.logger.warn(errMsgFormat, line, col, s);
		//errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		Parser.logger.warn(s);
		//errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}