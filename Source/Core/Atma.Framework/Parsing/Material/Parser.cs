
using System;
using Xna = Microsoft.Xna.Framework;
using XnaGraphics = Microsoft.Xna.Framework.Graphics;

namespace Atma.Parsing.Material
{
public partial class Parser {
	public const int _EOF = 0;
	public const int _IDENT = 1;
	public const int _FLOAT = 2;
	public const int _INT = 3;
	public const int maxT = 123;

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
		Expect(4);
		Expect(5);
		while (StartOf(1)) {
			if (StartOf(2)) {
				Sampler();
			} else if (StartOf(3)) {
				Blend();
			} else if (StartOf(4)) {
				Rasterizer();
			} else {
				Stencil();
			}
		}
		Expect(6);
	}

	void Sampler() {
		switch (la.kind) {
		case 107: {
			SamplerState();
			break;
		}
		case 113: {
			AddressU();
			break;
		}
		case 114: {
			AddressW();
			break;
		}
		case 115: {
			AddressV();
			break;
		}
		case 116: {
			Address();
			break;
		}
		case 118: {
			MaxAnisotropy();
			break;
		}
		case 119: {
			MaxMipLevel();
			break;
		}
		case 120: {
			MipMapLevelOfDetailBias();
			break;
		}
		default: SynErr(124); break;
		}
	}

	void Blend() {
		switch (la.kind) {
		case 68: {
			AlphaBlend();
			break;
		}
		case 74: {
			ColorBlend();
			break;
		}
		case 75: {
			AlphaSourceBlend();
			break;
		}
		case 76: {
			AlphaDestinationBlend();
			break;
		}
		case 77: {
			ColorSourceBlend();
			break;
		}
		case 78: {
			ColorDestinationBlend();
			break;
		}
		case 79: {
			BlendFactor();
			break;
		}
		case 80: {
			ColorWriteChannels();
			break;
		}
		case 73: {
			MultiSampleMask();
			break;
		}
		case 64: {
			BlendState();
			break;
		}
		default: SynErr(125); break;
		}
	}

	void Rasterizer() {
		switch (la.kind) {
		case 43: {
			RasterizerState();
			break;
		}
		case 47: {
			DepthBias();
			break;
		}
		case 48: {
			SlopeScaleDepthBias();
			break;
		}
		case 49: case 50: case 51: {
			MultiSampleAntiAlias();
			break;
		}
		case 52: case 53: {
			CullMode();
			break;
		}
		case 59: case 60: {
			FillMode();
			break;
		}
		default: SynErr(126); break;
		}
	}

	void Stencil() {
		switch (la.kind) {
		case 7: {
			StencilState();
			break;
		}
		case 11: {
			DepthBufferEnable();
			break;
		}
		case 12: {
			DepthBufferWriteEnable();
			break;
		}
		case 17: {
			TwoSidedStencilMode();
			break;
		}
		case 13: {
			StencilEnable();
			break;
		}
		case 14: {
			StencilMask();
			break;
		}
		case 15: {
			ReferenceStencil();
			break;
		}
		case 16: {
			StencilWriteMask();
			break;
		}
		case 18: {
			CounterClockwiseStencilDepthBufferFail();
			break;
		}
		case 19: {
			CounterClockwiseStencilFail();
			break;
		}
		case 20: {
			CounterClockwiseStencilPass();
			break;
		}
		case 21: {
			StencilDepthBufferFail();
			break;
		}
		case 22: {
			StencilPass();
			break;
		}
		case 23: {
			StencilFail();
			break;
		}
		case 32: {
			StencilFunction();
			break;
		}
		case 33: {
			CounterClockwiseStencilFunction();
			break;
		}
		case 34: {
			DepthBufferFunction();
			break;
		}
		default: SynErr(127); break;
		}
	}

	void StencilState() {
		Expect(7);
		StencilStates();
	}

	void DepthBufferEnable() {
		Expect(11);
		Bool(ref _material.DepthBufferEnable);
	}

	void DepthBufferWriteEnable() {
		Expect(12);
		Bool(ref _material.DepthBufferWriteEnable);
	}

	void TwoSidedStencilMode() {
		Expect(17);
		Bool(ref _material.TwoSidedStencilMode);
	}

	void StencilEnable() {
		Expect(13);
		Bool(ref _material.StencilEnable);
	}

	void StencilMask() {
		Expect(14);
		Int(ref _material.StencilMask);
	}

	void ReferenceStencil() {
		Expect(15);
		Int(ref _material.ReferenceStencil);
	}

	void StencilWriteMask() {
		Expect(16);
		Int(ref _material.StencilWriteMask);
	}

	void CounterClockwiseStencilDepthBufferFail() {
		Expect(18);
		StencilOperation(ref _material.CounterClockwiseStencilDepthBufferFail);
	}

	void CounterClockwiseStencilFail() {
		Expect(19);
		StencilOperation(ref _material.CounterClockwiseStencilFail);
	}

	void CounterClockwiseStencilPass() {
		Expect(20);
		StencilOperation(ref _material.CounterClockwiseStencilPass);
	}

	void StencilDepthBufferFail() {
		Expect(21);
		StencilOperation(ref _material.StencilDepthBufferFail);
	}

	void StencilPass() {
		Expect(22);
		StencilOperation(ref _material.StencilPass);
	}

	void StencilFail() {
		Expect(23);
		StencilOperation(ref _material.StencilFail);
	}

	void StencilFunction() {
		Expect(32);
		CompareFunction(ref _material.StencilFunction);
	}

	void CounterClockwiseStencilFunction() {
		Expect(33);
		CompareFunction(ref _material.CounterClockwiseStencilFunction);
	}

	void DepthBufferFunction() {
		Expect(34);
		CompareFunction(ref _material.DepthBufferFunction);
	}

	void StencilStates() {
		if (la.kind == 8) {
			Get();
			_material.SetDepthStencilState(XnaGraphics.DepthStencilState.Default); 
		} else if (la.kind == 9) {
			Get();
			_material.SetDepthStencilState(XnaGraphics.DepthStencilState.DepthRead); 
		} else if (la.kind == 10) {
			Get();
			_material.SetDepthStencilState(XnaGraphics.DepthStencilState.None); 
		} else SynErr(128);
	}

	void Bool(ref bool b) {
		if (la.kind == 81 || la.kind == 121) {
			if (la.kind == 81) {
				Get();
			} else {
				Get();
			}
			b = false; 
		} else if (la.kind == 82 || la.kind == 122) {
			if (la.kind == 82) {
				Get();
			} else {
				Get();
			}
			b = true; 
		} else SynErr(129);
	}

	void Int(ref int i) {
		Expect(3);
		i = Convert.ToInt32(t.val); 
	}

	void StencilOperation(ref XnaGraphics.StencilOperation s) {
		switch (la.kind) {
		case 24: {
			Get();
			s = XnaGraphics.StencilOperation.Keep; 
			break;
		}
		case 25: {
			Get();
			s = XnaGraphics.StencilOperation.Zero; 
			break;
		}
		case 26: {
			Get();
			s = XnaGraphics.StencilOperation.Replace; 
			break;
		}
		case 27: {
			Get();
			s = XnaGraphics.StencilOperation.Increment; 
			break;
		}
		case 28: {
			Get();
			s = XnaGraphics.StencilOperation.Decrement; 
			break;
		}
		case 29: {
			Get();
			s = XnaGraphics.StencilOperation.IncrementSaturation; 
			break;
		}
		case 30: {
			Get();
			s = XnaGraphics.StencilOperation.DecrementSaturation; 
			break;
		}
		case 31: {
			Get();
			s = XnaGraphics.StencilOperation.Invert; 
			break;
		}
		default: SynErr(130); break;
		}
	}

	void CompareFunction(ref XnaGraphics.CompareFunction c) {
		switch (la.kind) {
		case 35: {
			Get();
			c = XnaGraphics.CompareFunction.Always; 
			break;
		}
		case 36: {
			Get();
			c = XnaGraphics.CompareFunction.Never; 
			break;
		}
		case 37: {
			Get();
			c = XnaGraphics.CompareFunction.Less; 
			break;
		}
		case 38: {
			Get();
			c = XnaGraphics.CompareFunction.LessEqual; 
			break;
		}
		case 39: {
			Get();
			c = XnaGraphics.CompareFunction.Equal; 
			break;
		}
		case 40: {
			Get();
			c = XnaGraphics.CompareFunction.GreaterEqual; 
			break;
		}
		case 41: {
			Get();
			c = XnaGraphics.CompareFunction.Greater; 
			break;
		}
		case 42: {
			Get();
			c = XnaGraphics.CompareFunction.NotEqual; 
			break;
		}
		default: SynErr(131); break;
		}
	}

	void RasterizerState() {
		Expect(43);
		RasterizerStates();
	}

	void DepthBias() {
		Expect(47);
		Float(ref _material.DepthBias);
	}

	void SlopeScaleDepthBias() {
		Expect(48);
		Float(ref _material.SlopeScaleDepthBias);
	}

	void MultiSampleAntiAlias() {
		if (la.kind == 49) {
			Get();
		} else if (la.kind == 50) {
			Get();
		} else if (la.kind == 51) {
			Get();
		} else SynErr(132);
		Bool(ref _material.MultiSampleAntiAlias);
	}

	void CullMode() {
		if (la.kind == 52) {
			Get();
		} else if (la.kind == 53) {
			Get();
		} else SynErr(133);
		CullModes();
	}

	void FillMode() {
		if (la.kind == 59) {
			Get();
		} else if (la.kind == 60) {
			Get();
		} else SynErr(134);
		FillModes();
	}

	void RasterizerStates() {
		if (la.kind == 44) {
			Get();
			_material.SetRasterizerState(XnaGraphics.RasterizerState.CullClockwise); 
		} else if (la.kind == 45) {
			Get();
			_material.SetRasterizerState(XnaGraphics.RasterizerState.CullCounterClockwise); 
		} else if (la.kind == 46) {
			Get();
			_material.SetRasterizerState(XnaGraphics.RasterizerState.CullNone); 
		} else SynErr(135);
	}

	void Float(ref float f) {
		Expect(2);
		f = Convert.ToSingle(t.val); 
	}

	void CullModes() {
		if (la.kind == 10) {
			Get();
			_material.CullMode = XnaGraphics.CullMode.None; 
		} else if (la.kind == 54 || la.kind == 55) {
			if (la.kind == 54) {
				Get();
			} else {
				Get();
			}
			_material.CullMode = XnaGraphics.CullMode.CullClockwiseFace; 
		} else if (la.kind == 56 || la.kind == 57 || la.kind == 58) {
			if (la.kind == 56) {
				Get();
			} else if (la.kind == 57) {
				Get();
			} else {
				Get();
			}
			_material.CullMode = XnaGraphics.CullMode.CullCounterClockwiseFace; 
		} else SynErr(136);
	}

	void FillModes() {
		if (la.kind == 61) {
			Get();
			_material.FillMode = XnaGraphics.FillMode.Solid; 
		} else if (la.kind == 62 || la.kind == 63) {
			if (la.kind == 62) {
				Get();
			} else {
				Get();
			}
			_material.FillMode = XnaGraphics.FillMode.WireFrame; 
		} else SynErr(137);
	}

	void AlphaBlend() {
		Expect(68);
		BlendFunction(ref _material.AlphaBlendFunction);
	}

	void ColorBlend() {
		Expect(74);
		BlendFunction(ref _material.ColorBlendFunction);
	}

	void AlphaSourceBlend() {
		Expect(75);
		BlendMethod(ref _material.AlphaSourceBlend);
	}

	void AlphaDestinationBlend() {
		Expect(76);
		BlendMethod(ref _material.AlphaDestinationBlend);
	}

	void ColorSourceBlend() {
		Expect(77);
		BlendMethod(ref _material.ColorSourceBlend);
	}

	void ColorDestinationBlend() {
		Expect(78);
		BlendMethod(ref _material.ColorDestinationBlend);
	}

	void BlendFactor() {
		Expect(79);
		Color4(ref _material.BlendFactor);
	}

	void ColorWriteChannels() {
		Expect(80);
		if (la.kind == 81) {
			Get();
			ColorWriteChannel(ref _material.ColorWriteChannels);
		} else if (la.kind == 82) {
			Get();
			ColorWriteChannel(ref _material.ColorWriteChannels1);
		} else if (la.kind == 83) {
			Get();
			ColorWriteChannel(ref _material.ColorWriteChannels2);
		} else if (la.kind == 84) {
			Get();
			ColorWriteChannel(ref _material.ColorWriteChannels3);
		} else SynErr(138);
	}

	void MultiSampleMask() {
		Expect(73);
		Int(ref _material.MultiSampleMask);
	}

	void BlendState() {
		Expect(64);
		BlendStates();
	}

	void BlendStates() {
		if (la.kind == 65 || la.kind == 66) {
			if (la.kind == 65) {
				Get();
			} else {
				Get();
			}
			_material.SetBlendState(XnaGraphics.BlendState.Additive); 
		} else if (la.kind == 67 || la.kind == 68) {
			if (la.kind == 67) {
				Get();
			} else {
				Get();
			}
			_material.SetBlendState(XnaGraphics.BlendState.AlphaBlend); 
		} else if (la.kind == 69 || la.kind == 70 || la.kind == 71) {
			if (la.kind == 69) {
				Get();
			} else if (la.kind == 70) {
				Get();
			} else {
				Get();
			}
			_material.SetBlendState(XnaGraphics.BlendState.NonPremultiplied); 
		} else if (la.kind == 72) {
			Get();
			_material.SetBlendState(XnaGraphics.BlendState.Opaque); 
		} else SynErr(139);
	}

	void BlendFunction(ref XnaGraphics.BlendFunction r) {
		if (la.kind == 65) {
			Get();
			r = XnaGraphics.BlendFunction.Add; 
		} else if (la.kind == 90) {
			Get();
			r = XnaGraphics.BlendFunction.Subtract; 
		} else if (la.kind == 91) {
			Get();
			r = XnaGraphics.BlendFunction.ReverseSubtract; 
		} else if (la.kind == 92) {
			Get();
			r = XnaGraphics.BlendFunction.Max; 
		} else if (la.kind == 93) {
			Get();
			r = XnaGraphics.BlendFunction.Min; 
		} else SynErr(140);
	}

	void BlendMethod(ref XnaGraphics.Blend r) {
		switch (la.kind) {
		case 82: case 94: {
			if (la.kind == 94) {
				Get();
			} else {
				Get();
			}
			r = XnaGraphics.Blend.One; 
			break;
		}
		case 25: case 81: {
			if (la.kind == 25) {
				Get();
			} else {
				Get();
			}
			r = XnaGraphics.Blend.Zero; 
			break;
		}
		case 95: {
			Get();
			r = XnaGraphics.Blend.SourceColor; 
			break;
		}
		case 96: {
			Get();
			r = XnaGraphics.Blend.InverseSourceColor; 
			break;
		}
		case 97: {
			Get();
			r = XnaGraphics.Blend.SourceAlpha; 
			break;
		}
		case 98: {
			Get();
			r = XnaGraphics.Blend.InverseSourceAlpha; 
			break;
		}
		case 99: {
			Get();
			r = XnaGraphics.Blend.DestinationColor; 
			break;
		}
		case 100: {
			Get();
			r = XnaGraphics.Blend.InverseDestinationColor; 
			break;
		}
		case 101: {
			Get();
			r = XnaGraphics.Blend.DestinationAlpha; 
			break;
		}
		case 102: {
			Get();
			r = XnaGraphics.Blend.InverseDestinationAlpha; 
			break;
		}
		case 79: {
			Get();
			r = XnaGraphics.Blend.BlendFactor; 
			break;
		}
		case 103: case 104: {
			if (la.kind == 103) {
				Get();
			} else {
				Get();
			}
			r = XnaGraphics.Blend.InverseBlendFactor; 
			break;
		}
		case 105: case 106: {
			if (la.kind == 105) {
				Get();
			} else {
				Get();
			}
			r = XnaGraphics.Blend.SourceAlphaSaturation; 
			break;
		}
		default: SynErr(141); break;
		}
	}

	void Color4(ref Xna.Color c) {
		float a=1f, r=1f, g=1f, b=1f; 
		Float(ref r);
		Float(ref b);
		Float(ref g);
		if (la.kind == 2) {
			Float(ref a);
		}
		c = new Xna.Color(r, g, b, a); 
	}

	void ColorWriteChannel(ref XnaGraphics.ColorWriteChannels c) {
		c = XnaGraphics.ColorWriteChannels.None; 
		ColorWriteChannelValue(ref c);
		while (StartOf(5)) {
			ColorWriteChannelValue(ref c);
		}
	}

	void ColorWriteChannelValue(ref XnaGraphics.ColorWriteChannels c) {
		switch (la.kind) {
		case 10: {
			Get();
			break;
		}
		case 85: {
			Get();
			c |= XnaGraphics.ColorWriteChannels.Red; 
			break;
		}
		case 86: {
			Get();
			c |= XnaGraphics.ColorWriteChannels.Green; 
			break;
		}
		case 87: {
			Get();
			c |= XnaGraphics.ColorWriteChannels.Blue; 
			break;
		}
		case 88: {
			Get();
			c |= XnaGraphics.ColorWriteChannels.Alpha; 
			break;
		}
		case 89: {
			Get();
			c |= XnaGraphics.ColorWriteChannels.All; 
			break;
		}
		default: SynErr(142); break;
		}
	}

	void SamplerState() {
		Expect(107);
		SamplerStates();
	}

	void AddressU() {
		Expect(113);
		TextureAddressMode(ref _material.AddressU);
	}

	void AddressW() {
		Expect(114);
		TextureAddressMode(ref _material.AddressW);
	}

	void AddressV() {
		Expect(115);
		TextureAddressMode(ref _material.AddressV);
	}

	void Address() {
		Expect(116);
		var t = XnaGraphics.TextureAddressMode.Wrap; 
		TextureAddressMode(ref t);
		_material.SetTextureAddressMode(t); 
	}

	void MaxAnisotropy() {
		Expect(118);
		Int(ref _material.MaxAnisotropy);
	}

	void MaxMipLevel() {
		Expect(119);
		Int(ref _material.MaxMipLevel);
	}

	void MipMapLevelOfDetailBias() {
		Expect(120);
		Float(ref _material.MipMapLevelOfDetailBias);
	}

	void SamplerStates() {
		if (la.kind == 108) {
			Get();
			if (la.kind == 109) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.AnisotropicWrap); 
			} else if (la.kind == 110) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.AnisotropicClamp); 
			} else SynErr(143);
		} else if (la.kind == 111) {
			Get();
			if (la.kind == 109) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.LinearWrap); 
			} else if (la.kind == 110) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.LinearClamp); 
			} else SynErr(144);
		} else if (la.kind == 112) {
			Get();
			if (la.kind == 109) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.PointWrap); 
			} else if (la.kind == 110) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.PointClamp); 
			} else SynErr(145);
		} else SynErr(146);
	}

	void TextureAddressMode(ref XnaGraphics.TextureAddressMode t) {
		if (la.kind == 109) {
			Get();
			t = XnaGraphics.TextureAddressMode.Wrap; 
		} else if (la.kind == 110) {
			Get();
			t = XnaGraphics.TextureAddressMode.Clamp; 
		} else if (la.kind == 117) {
			Get();
			t = XnaGraphics.TextureAddressMode.Mirror; 
		} else SynErr(147);
	}



	protected void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Transform();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,T, x,x,x,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,x, x,x,x,x, T,T,T,x, x,x,x,x, x,x,x,T, x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,T, T,x,x,x, T,x,x,x, T,x,x,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,T,T, T,x,T,T, T,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,T,T, T,x,T,T, T,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x}

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
			case 2: s = "FLOAT expected"; break;
			case 3: s = "INT expected"; break;
			case 4: s = "\"material\" expected"; break;
			case 5: s = "\"{\" expected"; break;
			case 6: s = "\"}\" expected"; break;
			case 7: s = "\"depth\" expected"; break;
			case 8: s = "\"default\" expected"; break;
			case 9: s = "\"read\" expected"; break;
			case 10: s = "\"none\" expected"; break;
			case 11: s = "\"depth_check\" expected"; break;
			case 12: s = "\"depth_write\" expected"; break;
			case 13: s = "\"depth_stencil\" expected"; break;
			case 14: s = "\"stencil_mask\" expected"; break;
			case 15: s = "\"stencil_ref\" expected"; break;
			case 16: s = "\"stencil_write\" expected"; break;
			case 17: s = "\"stencil_two_sided\" expected"; break;
			case 18: s = "\"depth_counter_fail\" expected"; break;
			case 19: s = "\"stencil_counter_fail\" expected"; break;
			case 20: s = "\"stencil_counter_pass\" expected"; break;
			case 21: s = "\"depth_fail\" expected"; break;
			case 22: s = "\"stencil_pass\" expected"; break;
			case 23: s = "\"stencil_fail\" expected"; break;
			case 24: s = "\"keep\" expected"; break;
			case 25: s = "\"zero\" expected"; break;
			case 26: s = "\"replace\" expected"; break;
			case 27: s = "\"inc\" expected"; break;
			case 28: s = "\"dec\" expected"; break;
			case 29: s = "\"inc_sat\" expected"; break;
			case 30: s = "\"dec_sat\" expected"; break;
			case 31: s = "\"invert\" expected"; break;
			case 32: s = "\"stencil_func\" expected"; break;
			case 33: s = "\"counter_stencil_func\" expected"; break;
			case 34: s = "\"depth_func\" expected"; break;
			case 35: s = "\"always\" expected"; break;
			case 36: s = "\"never\" expected"; break;
			case 37: s = "\"less\" expected"; break;
			case 38: s = "\"less_eq\" expected"; break;
			case 39: s = "\"eq\" expected"; break;
			case 40: s = "\"greater_eq\" expected"; break;
			case 41: s = "\"greater\" expected"; break;
			case 42: s = "\"not_eq\" expected"; break;
			case 43: s = "\"rasterizer\" expected"; break;
			case 44: s = "\"cullclockwise\" expected"; break;
			case 45: s = "\"cullcounterclockwise\" expected"; break;
			case 46: s = "\"cullnone\" expected"; break;
			case 47: s = "\"depth_bias\" expected"; break;
			case 48: s = "\"slope_Depth_bias\" expected"; break;
			case 49: s = "\"multi_alias\" expected"; break;
			case 50: s = "\"multi_sample\" expected"; break;
			case 51: s = "\"anti_alias\" expected"; break;
			case 52: s = "\"cull\" expected"; break;
			case 53: s = "\"cull_mode\" expected"; break;
			case 54: s = "\"clock\" expected"; break;
			case 55: s = "\"clockwise\" expected"; break;
			case 56: s = "\"counter\" expected"; break;
			case 57: s = "\"counter_clock\" expected"; break;
			case 58: s = "\"counter_clockwise\" expected"; break;
			case 59: s = "\"fill\" expected"; break;
			case 60: s = "\"fill_mode\" expected"; break;
			case 61: s = "\"solid\" expected"; break;
			case 62: s = "\"wire\" expected"; break;
			case 63: s = "\"wire_frame\" expected"; break;
			case 64: s = "\"blend\" expected"; break;
			case 65: s = "\"add\" expected"; break;
			case 66: s = "\"additive\" expected"; break;
			case 67: s = "\"alpha\" expected"; break;
			case 68: s = "\"alpha_blend\" expected"; break;
			case 69: s = "\"non\" expected"; break;
			case 70: s = "\"pre\" expected"; break;
			case 71: s = "\"nonpremultiplied\" expected"; break;
			case 72: s = "\"opaque\" expected"; break;
			case 73: s = "\"multi_sample_mask\" expected"; break;
			case 74: s = "\"color_blend\" expected"; break;
			case 75: s = "\"alpha_src\" expected"; break;
			case 76: s = "\"alpha_dst\" expected"; break;
			case 77: s = "\"color_src\" expected"; break;
			case 78: s = "\"color_dst\" expected"; break;
			case 79: s = "\"blend_factor\" expected"; break;
			case 80: s = "\"write_channel\" expected"; break;
			case 81: s = "\"0\" expected"; break;
			case 82: s = "\"1\" expected"; break;
			case 83: s = "\"2\" expected"; break;
			case 84: s = "\"3\" expected"; break;
			case 85: s = "\"red\" expected"; break;
			case 86: s = "\"green\" expected"; break;
			case 87: s = "\"blue\" expected"; break;
			case 88: s = "\"Alpha\" expected"; break;
			case 89: s = "\"all\" expected"; break;
			case 90: s = "\"subtract\" expected"; break;
			case 91: s = "\"reversesubtract\" expected"; break;
			case 92: s = "\"max\" expected"; break;
			case 93: s = "\"min\" expected"; break;
			case 94: s = "\"one\" expected"; break;
			case 95: s = "\"src_color\" expected"; break;
			case 96: s = "\"inv_src_color\" expected"; break;
			case 97: s = "\"src_alpha\" expected"; break;
			case 98: s = "\"inv_src_alpha\" expected"; break;
			case 99: s = "\"dst_color\" expected"; break;
			case 100: s = "\"inv_dst_color\" expected"; break;
			case 101: s = "\"dst_alpha\" expected"; break;
			case 102: s = "\"inv_dst_alpha\" expected"; break;
			case 103: s = "\"inv_blend\" expected"; break;
			case 104: s = "\"inv_blend_factor\" expected"; break;
			case 105: s = "\"src_alpha_sat\" expected"; break;
			case 106: s = "\"src_alpha_saturation\" expected"; break;
			case 107: s = "\"sampler\" expected"; break;
			case 108: s = "\"anisotropic\" expected"; break;
			case 109: s = "\"wrap\" expected"; break;
			case 110: s = "\"clamp\" expected"; break;
			case 111: s = "\"linear\" expected"; break;
			case 112: s = "\"point\" expected"; break;
			case 113: s = "\"addressu\" expected"; break;
			case 114: s = "\"addressw\" expected"; break;
			case 115: s = "\"addressv\" expected"; break;
			case 116: s = "\"address\" expected"; break;
			case 117: s = "\"mirror\" expected"; break;
			case 118: s = "\"MaxAnisotropy\" expected"; break;
			case 119: s = "\"MaxMipLevel\" expected"; break;
			case 120: s = "\"MipMapLevelOfDetailBias\" expected"; break;
			case 121: s = "\"false\" expected"; break;
			case 122: s = "\"true\" expected"; break;
			case 123: s = "??? expected"; break;
			case 124: s = "invalid Sampler"; break;
			case 125: s = "invalid Blend"; break;
			case 126: s = "invalid Rasterizer"; break;
			case 127: s = "invalid Stencil"; break;
			case 128: s = "invalid StencilStates"; break;
			case 129: s = "invalid Bool"; break;
			case 130: s = "invalid StencilOperation"; break;
			case 131: s = "invalid CompareFunction"; break;
			case 132: s = "invalid MultiSampleAntiAlias"; break;
			case 133: s = "invalid CullMode"; break;
			case 134: s = "invalid FillMode"; break;
			case 135: s = "invalid RasterizerStates"; break;
			case 136: s = "invalid CullModes"; break;
			case 137: s = "invalid FillModes"; break;
			case 138: s = "invalid ColorWriteChannels"; break;
			case 139: s = "invalid BlendStates"; break;
			case 140: s = "invalid BlendFunction"; break;
			case 141: s = "invalid BlendMethod"; break;
			case 142: s = "invalid ColorWriteChannelValue"; break;
			case 143: s = "invalid SamplerStates"; break;
			case 144: s = "invalid SamplerStates"; break;
			case 145: s = "invalid SamplerStates"; break;
			case 146: s = "invalid SamplerStates"; break;
			case 147: s = "invalid TextureAddressMode"; break;

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