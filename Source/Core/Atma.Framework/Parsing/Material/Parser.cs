
using System;

namespace Atma.Framework.Parsing.Material
{
public class Parser {
	public const int _EOF = 0;
	public const int _IDENT = 1;
	public const int _FLOAT = 2;
	public const int _INT = 3;
	public const int maxT = 19;

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
		Expect(1);
		if (la.kind == 5) {
			Get();
			Path();
		}
		Expect(6);
		while (StartOf(1)) {
			Sampler();
		}
		Expect(7);
	}

	void Path() {
		Expect(1);
		while (la.kind == 18) {
			Get();
			Expect(1);
		}
	}

	void Sampler() {
		switch (la.kind) {
		case 8: {
			AddressU();
			break;
		}
		case 9: {
			AddressW();
			break;
		}
		case 10: {
			AddressV();
			break;
		}
		case 11: {
			Address();
			break;
		}
		case 15: {
			MaxAnisotropy();
			break;
		}
		case 16: {
			MaxMipLevel();
			break;
		}
		case 17: {
			MipMapLevelOfDetailBias();
			break;
		}
		default: SynErr(20); break;
		}
	}

	void AddressU() {
		Expect(8);
		TextureAddressMode();
	}

	void AddressW() {
		Expect(9);
		TextureAddressMode();
	}

	void AddressV() {
		Expect(10);
		TextureAddressMode();
	}

	void Address() {
		Expect(11);
		TextureAddressMode();
	}

	void MaxAnisotropy() {
		Expect(15);
		Expect(3);
	}

	void MaxMipLevel() {
		Expect(16);
		Expect(3);
	}

	void MipMapLevelOfDetailBias() {
		Expect(17);
		Expect(2);
	}

	void TextureAddressMode() {
		if (la.kind == 12) {
			Get();
		} else if (la.kind == 13) {
			Get();
		} else if (la.kind == 14) {
			Get();
		} else SynErr(21);
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Transform();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x},
		{x,x,x,x, x,x,x,x, T,T,T,T, x,x,x,T, T,T,x,x, x}

	};
} // end Parser


public class Errors {
	public int count = 0;                                    // number of errors detected
	public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
	public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

	public virtual void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "IDENT expected"; break;
			case 2: s = "FLOAT expected"; break;
			case 3: s = "INT expected"; break;
			case 4: s = "\"material\" expected"; break;
			case 5: s = "\":\" expected"; break;
			case 6: s = "\"{\" expected"; break;
			case 7: s = "\"}\" expected"; break;
			case 8: s = "\"addressu\" expected"; break;
			case 9: s = "\"addressw\" expected"; break;
			case 10: s = "\"addressv\" expected"; break;
			case 11: s = "\"address\" expected"; break;
			case 12: s = "\"wrap\" expected"; break;
			case 13: s = "\"clamp\" expected"; break;
			case 14: s = "\"mirror\" expected"; break;
			case 15: s = "\"MaxAnisotropy\" expected"; break;
			case 16: s = "\"MaxMipLevel\" expected"; break;
			case 17: s = "\"MipMapLevelOfDetailBias\" expected"; break;
			case 18: s = "\"/\" expected"; break;
			case 19: s = "??? expected"; break;
			case 20: s = "invalid Sampler"; break;
			case 21: s = "invalid TextureAddressMode"; break;

			default: s = "error " + n; break;
		}
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}

	public virtual void SemErr (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
		count++;
	}
	
	public virtual void SemErr (string s) {
		errorStream.WriteLine(s);
		count++;
	}
	
	public virtual void Warning (int line, int col, string s) {
		errorStream.WriteLine(errMsgFormat, line, col, s);
	}
	
	public virtual void Warning(string s) {
		errorStream.WriteLine(s);
	}
} // Errors


public class FatalError: Exception {
	public FatalError(string m): base(m) {}
}
}