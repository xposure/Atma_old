
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
	public const int maxT = 60;

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
			} else {
				Blend();
			}
		}
		Expect(6);
	}

	void Sampler() {
		switch (la.kind) {
		case 46: {
			SamplerState();
			break;
		}
		case 52: {
			AddressU();
			break;
		}
		case 53: {
			AddressW();
			break;
		}
		case 54: {
			AddressV();
			break;
		}
		case 55: {
			Address();
			break;
		}
		case 57: {
			MaxAnisotropy();
			break;
		}
		case 58: {
			MaxMipLevel();
			break;
		}
		case 59: {
			MipMapLevelOfDetailBias();
			break;
		}
		default: SynErr(61); break;
		}
	}

	void Blend() {
		switch (la.kind) {
		case 8: {
			AlphaBlend();
			break;
		}
		case 9: {
			ColorBlend();
			break;
		}
		case 10: {
			AlphaSourceBlend();
			break;
		}
		case 11: {
			AlphaDestinationBlend();
			break;
		}
		case 12: {
			ColorSourceBlend();
			break;
		}
		case 13: {
			ColorDestinationBlend();
			break;
		}
		case 14: {
			BlendFactor();
			break;
		}
		case 15: {
			ColorWriteChannels();
			break;
		}
		case 7: {
			MultiSampleMask();
			break;
		}
		default: SynErr(62); break;
		}
	}

	void AlphaBlend() {
		Expect(8);
		BlendFunction(ref _material.AlphaBlendFunction);
	}

	void ColorBlend() {
		Expect(9);
		BlendFunction(ref _material.ColorBlendFunction);
	}

	void AlphaSourceBlend() {
		Expect(10);
		BlendMethod(ref _material.AlphaSourceBlend);
	}

	void AlphaDestinationBlend() {
		Expect(11);
		BlendMethod(ref _material.AlphaDestinationBlend);
	}

	void ColorSourceBlend() {
		Expect(12);
		BlendMethod(ref _material.ColorSourceBlend);
	}

	void ColorDestinationBlend() {
		Expect(13);
		BlendMethod(ref _material.ColorDestinationBlend);
	}

	void BlendFactor() {
		Expect(14);
		Color4(ref _material.BlendFactor);
	}

	void ColorWriteChannels() {
		Expect(15);
		if (la.kind == 16) {
			Get();
			ColorWriteChannel(ref _material.ColorWriteChannels);
		} else if (la.kind == 17) {
			Get();
			ColorWriteChannel(ref _material.ColorWriteChannels1);
		} else if (la.kind == 18) {
			Get();
			ColorWriteChannel(ref _material.ColorWriteChannels2);
		} else if (la.kind == 19) {
			Get();
			ColorWriteChannel(ref _material.ColorWriteChannels3);
		} else SynErr(63);
	}

	void MultiSampleMask() {
		Expect(7);
		Int(ref _material.MultiSampleMask);
	}

	void Int(ref int i) {
		Expect(3);
		i = Convert.ToInt32(t.val); 
	}

	void BlendFunction(ref XnaGraphics.BlendFunction r) {
		if (la.kind == 26) {
			Get();
			r = XnaGraphics.BlendFunction.Add; 
		} else if (la.kind == 27) {
			Get();
			r = XnaGraphics.BlendFunction.Subtract; 
		} else if (la.kind == 28) {
			Get();
			r = XnaGraphics.BlendFunction.ReverseSubtract; 
		} else if (la.kind == 29) {
			Get();
			r = XnaGraphics.BlendFunction.Max; 
		} else if (la.kind == 30) {
			Get();
			r = XnaGraphics.BlendFunction.Min; 
		} else SynErr(64);
	}

	void BlendMethod(ref XnaGraphics.Blend r) {
		switch (la.kind) {
		case 17: case 31: {
			if (la.kind == 31) {
				Get();
			} else {
				Get();
			}
			r = XnaGraphics.Blend.One; 
			break;
		}
		case 16: case 32: {
			if (la.kind == 32) {
				Get();
			} else {
				Get();
			}
			r = XnaGraphics.Blend.Zero; 
			break;
		}
		case 33: {
			Get();
			r = XnaGraphics.Blend.SourceColor; 
			break;
		}
		case 34: {
			Get();
			r = XnaGraphics.Blend.InverseSourceColor; 
			break;
		}
		case 35: {
			Get();
			r = XnaGraphics.Blend.SourceAlpha; 
			break;
		}
		case 36: {
			Get();
			r = XnaGraphics.Blend.InverseSourceAlpha; 
			break;
		}
		case 37: {
			Get();
			r = XnaGraphics.Blend.DestinationColor; 
			break;
		}
		case 38: {
			Get();
			r = XnaGraphics.Blend.InverseDestinationColor; 
			break;
		}
		case 39: {
			Get();
			r = XnaGraphics.Blend.DestinationAlpha; 
			break;
		}
		case 40: {
			Get();
			r = XnaGraphics.Blend.InverseDestinationAlpha; 
			break;
		}
		case 14: case 41: {
			if (la.kind == 41) {
				Get();
			} else {
				Get();
			}
			r = XnaGraphics.Blend.BlendFactor; 
			break;
		}
		case 42: case 43: {
			if (la.kind == 42) {
				Get();
			} else {
				Get();
			}
			r = XnaGraphics.Blend.InverseBlendFactor; 
			break;
		}
		case 44: case 45: {
			if (la.kind == 44) {
				Get();
			} else {
				Get();
			}
			r = XnaGraphics.Blend.SourceAlphaSaturation; 
			break;
		}
		default: SynErr(65); break;
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
		while (StartOf(3)) {
			ColorWriteChannelValue(ref c);
		}
	}

	void ColorWriteChannelValue(ref XnaGraphics.ColorWriteChannels c) {
		switch (la.kind) {
		case 20: {
			Get();
			break;
		}
		case 21: {
			Get();
			c |= XnaGraphics.ColorWriteChannels.Red; 
			break;
		}
		case 22: {
			Get();
			c |= XnaGraphics.ColorWriteChannels.Green; 
			break;
		}
		case 23: {
			Get();
			c |= XnaGraphics.ColorWriteChannels.Blue; 
			break;
		}
		case 24: {
			Get();
			c |= XnaGraphics.ColorWriteChannels.Alpha; 
			break;
		}
		case 25: {
			Get();
			c |= XnaGraphics.ColorWriteChannels.All; 
			break;
		}
		default: SynErr(66); break;
		}
	}

	void SamplerState() {
		Expect(46);
		SamplerStates();
	}

	void AddressU() {
		Expect(52);
		TextureAddressMode(ref _material.AddressU);
	}

	void AddressW() {
		Expect(53);
		TextureAddressMode(ref _material.AddressW);
	}

	void AddressV() {
		Expect(54);
		TextureAddressMode(ref _material.AddressV);
	}

	void Address() {
		Expect(55);
		var t = XnaGraphics.TextureAddressMode.Wrap; 
		TextureAddressMode(ref t);
		_material.SetTextureAddressMode(t); 
	}

	void MaxAnisotropy() {
		Expect(57);
		Int(ref _material.MaxAnisotropy);
	}

	void MaxMipLevel() {
		Expect(58);
		Int(ref _material.MaxMipLevel);
	}

	void MipMapLevelOfDetailBias() {
		Expect(59);
		Float(ref _material.MipMapLevelOfDetailBias);
	}

	void SamplerStates() {
		if (la.kind == 47) {
			Get();
			if (la.kind == 48) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.AnisotropicWrap); 
			} else if (la.kind == 49) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.AnisotropicClamp); 
			} else SynErr(67);
		} else if (la.kind == 50) {
			Get();
			if (la.kind == 48) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.LinearWrap); 
			} else if (la.kind == 49) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.LinearClamp); 
			} else SynErr(68);
		} else if (la.kind == 51) {
			Get();
			if (la.kind == 48) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.PointWrap); 
			} else if (la.kind == 49) {
				Get();
				_material.SetSamplerState(XnaGraphics.SamplerState.PointClamp); 
			} else SynErr(69);
		} else SynErr(70);
	}

	void TextureAddressMode(ref XnaGraphics.TextureAddressMode t) {
		if (la.kind == 48) {
			Get();
			t = XnaGraphics.TextureAddressMode.Wrap; 
		} else if (la.kind == 49) {
			Get();
			t = XnaGraphics.TextureAddressMode.Clamp; 
		} else if (la.kind == 56) {
			Get();
			t = XnaGraphics.TextureAddressMode.Mirror; 
		} else SynErr(71);
	}

	void Float(ref float f) {
		Expect(2);
		f = Convert.ToSingle(t.val); 
	}



	protected void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Transform();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,T, T,T,T,T, T,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,T,T,T, x,T,T,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,T,T,T, x,T,T,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}

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
			case 7: s = "\"multi_sample_mask\" expected"; break;
			case 8: s = "\"alpha_blend\" expected"; break;
			case 9: s = "\"color_blend\" expected"; break;
			case 10: s = "\"alpha_src\" expected"; break;
			case 11: s = "\"alpha_dst\" expected"; break;
			case 12: s = "\"color_src\" expected"; break;
			case 13: s = "\"color_dst\" expected"; break;
			case 14: s = "\"blend_factor\" expected"; break;
			case 15: s = "\"write_channel\" expected"; break;
			case 16: s = "\"0\" expected"; break;
			case 17: s = "\"1\" expected"; break;
			case 18: s = "\"2\" expected"; break;
			case 19: s = "\"3\" expected"; break;
			case 20: s = "\"none\" expected"; break;
			case 21: s = "\"red\" expected"; break;
			case 22: s = "\"green\" expected"; break;
			case 23: s = "\"blue\" expected"; break;
			case 24: s = "\"Alpha\" expected"; break;
			case 25: s = "\"all\" expected"; break;
			case 26: s = "\"add\" expected"; break;
			case 27: s = "\"subtract\" expected"; break;
			case 28: s = "\"reversesubtract\" expected"; break;
			case 29: s = "\"max\" expected"; break;
			case 30: s = "\"min\" expected"; break;
			case 31: s = "\"one\" expected"; break;
			case 32: s = "\"zero\" expected"; break;
			case 33: s = "\"src_color\" expected"; break;
			case 34: s = "\"inv_src_color\" expected"; break;
			case 35: s = "\"src_alpha\" expected"; break;
			case 36: s = "\"inv_src_alpha\" expected"; break;
			case 37: s = "\"dst_color\" expected"; break;
			case 38: s = "\"inv_dst_color\" expected"; break;
			case 39: s = "\"dst_alpha\" expected"; break;
			case 40: s = "\"inv_dst_alpha\" expected"; break;
			case 41: s = "\"blend\" expected"; break;
			case 42: s = "\"inv_blend\" expected"; break;
			case 43: s = "\"inv_blend_factor\" expected"; break;
			case 44: s = "\"src_alpha_sat\" expected"; break;
			case 45: s = "\"src_alpha_saturation\" expected"; break;
			case 46: s = "\"sampler\" expected"; break;
			case 47: s = "\"anisotropic\" expected"; break;
			case 48: s = "\"wrap\" expected"; break;
			case 49: s = "\"clamp\" expected"; break;
			case 50: s = "\"linear\" expected"; break;
			case 51: s = "\"point\" expected"; break;
			case 52: s = "\"addressu\" expected"; break;
			case 53: s = "\"addressw\" expected"; break;
			case 54: s = "\"addressv\" expected"; break;
			case 55: s = "\"address\" expected"; break;
			case 56: s = "\"mirror\" expected"; break;
			case 57: s = "\"MaxAnisotropy\" expected"; break;
			case 58: s = "\"MaxMipLevel\" expected"; break;
			case 59: s = "\"MipMapLevelOfDetailBias\" expected"; break;
			case 60: s = "??? expected"; break;
			case 61: s = "invalid Sampler"; break;
			case 62: s = "invalid Blend"; break;
			case 63: s = "invalid ColorWriteChannels"; break;
			case 64: s = "invalid BlendFunction"; break;
			case 65: s = "invalid BlendMethod"; break;
			case 66: s = "invalid ColorWriteChannelValue"; break;
			case 67: s = "invalid SamplerStates"; break;
			case 68: s = "invalid SamplerStates"; break;
			case 69: s = "invalid SamplerStates"; break;
			case 70: s = "invalid SamplerStates"; break;
			case 71: s = "invalid TextureAddressMode"; break;

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