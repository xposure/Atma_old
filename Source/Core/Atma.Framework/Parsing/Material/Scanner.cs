
using System;
using System.IO;
using System.Collections;


namespace Atma.Parsing.Material
{

public class Token {
	public int kind;    // token kind
	public int pos;     // token position in bytes in the source text (starting at 0)
	public int charPos;  // token position in characters in the source text (starting at 0)
	public int col;     // token column (starting at 1)
	public int line;    // token line (starting at 1)
	public string val;  // token value
	public Token next;  // ML 2005-03-11 Tokens are kept in linked list
}

//-----------------------------------------------------------------------------------
// Buffer
//-----------------------------------------------------------------------------------
public class Buffer {
	// This Buffer supports the following cases:
	// 1) seekable stream (file)
	//    a) whole stream in buffer
	//    b) part of stream in buffer
	// 2) non seekable stream (network, console)

	public const int EOF = char.MaxValue + 1;
	const int MIN_BUFFER_LENGTH = 1024; // 1KB
	const int MAX_BUFFER_LENGTH = MIN_BUFFER_LENGTH * 64; // 64KB
	byte[] buf;         // input buffer
	int bufStart;       // position of first byte in buffer relative to input stream
	int bufLen;         // length of buffer
	int fileLen;        // length of input stream (may change if the stream is no file)
	int bufPos;         // current position in buffer
	Stream stream;      // input stream (seekable)
	bool isUserStream;  // was the stream opened by the user?
	
	public Buffer (Stream s, bool isUserStream) {
		stream = s; this.isUserStream = isUserStream;
		
		if (stream.CanSeek) {
			fileLen = (int) stream.Length;
			bufLen = Math.Min(fileLen, MAX_BUFFER_LENGTH);
			bufStart = Int32.MaxValue; // nothing in the buffer so far
		} else {
			fileLen = bufLen = bufStart = 0;
		}

		buf = new byte[(bufLen>0) ? bufLen : MIN_BUFFER_LENGTH];
		if (fileLen > 0) Pos = 0; // setup buffer to position 0 (start)
		else bufPos = 0; // index 0 is already after the file, thus Pos = 0 is invalid
		if (bufLen == fileLen && stream.CanSeek) Close();
	}
	
	protected Buffer(Buffer b) { // called in UTF8Buffer constructor
		buf = b.buf;
		bufStart = b.bufStart;
		bufLen = b.bufLen;
		fileLen = b.fileLen;
		bufPos = b.bufPos;
		stream = b.stream;
		// keep destructor from closing the stream
		b.stream = null;
		isUserStream = b.isUserStream;
	}

	~Buffer() { Close(); }
	
	protected void Close() {
		if (!isUserStream && stream != null) {
			stream.Close();
			stream = null;
		}
	}
	
	public virtual int Read () {
		if (bufPos < bufLen) {
			return buf[bufPos++];
		} else if (Pos < fileLen) {
			Pos = Pos; // shift buffer start to Pos
			return buf[bufPos++];
		} else if (stream != null && !stream.CanSeek && ReadNextStreamChunk() > 0) {
			return buf[bufPos++];
		} else {
			return EOF;
		}
	}

	public int Peek () {
		int curPos = Pos;
		int ch = Read();
		Pos = curPos;
		return ch;
	}
	
	// beg .. begin, zero-based, inclusive, in byte
	// end .. end, zero-based, exclusive, in byte
	public string GetString (int beg, int end) {
		int len = 0;
		char[] buf = new char[end - beg];
		int oldPos = Pos;
		Pos = beg;
		while (Pos < end) buf[len++] = (char) Read();
		Pos = oldPos;
		return new String(buf, 0, len);
	}

	public int Pos {
		get { return bufPos + bufStart; }
		set {
			if (value >= fileLen && stream != null && !stream.CanSeek) {
				// Wanted position is after buffer and the stream
				// is not seek-able e.g. network or console,
				// thus we have to read the stream manually till
				// the wanted position is in sight.
				while (value >= fileLen && ReadNextStreamChunk() > 0);
			}

			if (value < 0 || value > fileLen) {
				throw new FatalError("buffer out of bounds access, position: " + value);
			}

			if (value >= bufStart && value < bufStart + bufLen) { // already in buffer
				bufPos = value - bufStart;
			} else if (stream != null) { // must be swapped in
				stream.Seek(value, SeekOrigin.Begin);
				bufLen = stream.Read(buf, 0, buf.Length);
				bufStart = value; bufPos = 0;
			} else {
				// set the position to the end of the file, Pos will return fileLen.
				bufPos = fileLen - bufStart;
			}
		}
	}
	
	// Read the next chunk of bytes from the stream, increases the buffer
	// if needed and updates the fields fileLen and bufLen.
	// Returns the number of bytes read.
	private int ReadNextStreamChunk() {
		int free = buf.Length - bufLen;
		if (free == 0) {
			// in the case of a growing input stream
			// we can neither seek in the stream, nor can we
			// foresee the maximum length, thus we must adapt
			// the buffer size on demand.
			byte[] newBuf = new byte[bufLen * 2];
			Array.Copy(buf, newBuf, bufLen);
			buf = newBuf;
			free = bufLen;
		}
		int read = stream.Read(buf, bufLen, free);
		if (read > 0) {
			fileLen = bufLen = (bufLen + read);
			return read;
		}
		// end of stream reached
		return 0;
	}
}

//-----------------------------------------------------------------------------------
// UTF8Buffer
//-----------------------------------------------------------------------------------
public class UTF8Buffer: Buffer {
	public UTF8Buffer(Buffer b): base(b) {}

	public override int Read() {
		int ch;
		do {
			ch = base.Read();
			// until we find a utf8 start (0xxxxxxx or 11xxxxxx)
		} while ((ch >= 128) && ((ch & 0xC0) != 0xC0) && (ch != EOF));
		if (ch < 128 || ch == EOF) {
			// nothing to do, first 127 chars are the same in ascii and utf8
			// 0xxxxxxx or end of file character
		} else if ((ch & 0xF0) == 0xF0) {
			// 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx
			int c1 = ch & 0x07; ch = base.Read();
			int c2 = ch & 0x3F; ch = base.Read();
			int c3 = ch & 0x3F; ch = base.Read();
			int c4 = ch & 0x3F;
			ch = (((((c1 << 6) | c2) << 6) | c3) << 6) | c4;
		} else if ((ch & 0xE0) == 0xE0) {
			// 1110xxxx 10xxxxxx 10xxxxxx
			int c1 = ch & 0x0F; ch = base.Read();
			int c2 = ch & 0x3F; ch = base.Read();
			int c3 = ch & 0x3F;
			ch = (((c1 << 6) | c2) << 6) | c3;
		} else if ((ch & 0xC0) == 0xC0) {
			// 110xxxxx 10xxxxxx
			int c1 = ch & 0x1F; ch = base.Read();
			int c2 = ch & 0x3F;
			ch = (c1 << 6) | c2;
		}
		return ch;
	}
}

//-----------------------------------------------------------------------------------
// Scanner
//-----------------------------------------------------------------------------------
public partial class Scanner {
	const char EOL = '\n';
	const int eofSym = 0; /* pdt */
	const int maxT = 126;
	const int noSym = 126;


	public Buffer buffer; // scanner buffer
	
	Token t;          // current token
	int ch;           // current input character
	int pos;          // byte position of current character
	int charPos;      // position by unicode characters starting with 0
	int col;          // column number of current character
	int line;         // line number of current character
	int oldEols;      // EOLs that appeared in a comment;
	static readonly Hashtable start; // maps first token character to start state

	Token tokens;     // list of tokens already peeked (first token is a dummy)
	Token pt;         // current peek token
	
	char[] tval = new char[128]; // text of current token
	int tlen;         // length of current token
	
	static Scanner() {
		start = new Hashtable(128);
		for (int i = 65; i <= 90; ++i) start[i] = 10;
		for (int i = 97; i <= 122; ++i) start[i] = 10;
		for (int i = 48; i <= 49; ++i) start[i] = 11;
		for (int i = 50; i <= 57; ++i) start[i] = 9;
		start[123] = 12; 
		start[125] = 13; 
		start[Buffer.EOF] = -1;

	}
	
	public Scanner (string fileName) {
		try {
			Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			buffer = new Buffer(stream, false);
			Init();
		} catch (IOException) {
			throw new FatalError("Cannot open file " + fileName);
		}
	}
	
	public Scanner (Stream s) {
		buffer = new Buffer(s, true);
		Init();
	}
	
	void Init() {
		pos = -1; line = 1; col = 0; charPos = -1;
		oldEols = 0;
		NextCh();
		if (ch == 0xEF) { // check optional byte order mark for UTF-8
			NextCh(); int ch1 = ch;
			NextCh(); int ch2 = ch;
			if (ch1 != 0xBB || ch2 != 0xBF) {
				throw new FatalError(String.Format("illegal byte order mark: EF {0,2:X} {1,2:X}", ch1, ch2));
			}
			buffer = new UTF8Buffer(buffer); col = 0; charPos = -1;
			NextCh();
		}
		pt = tokens = new Token();  // first token is a dummy
	}
	
	void NextCh() {
		if (oldEols > 0) { ch = EOL; oldEols--; } 
		else {
			pos = buffer.Pos;
			// buffer reads unicode chars, if UTF8 has been detected
			ch = buffer.Read(); col++; charPos++;
			// replace isolated '\r' by '\n' in order to make
			// eol handling uniform across Windows, Unix and Mac
			if (ch == '\r' && buffer.Peek() != '\n') ch = EOL;
			if (ch == EOL) { line++; col = 0; }
		}

	}

	void AddCh() {
		if (tlen >= tval.Length) {
			char[] newBuf = new char[2 * tval.Length];
			Array.Copy(tval, 0, newBuf, 0, tval.Length);
			tval = newBuf;
		}
		if (ch != Buffer.EOF) {
			tval[tlen++] = (char) ch;
			NextCh();
		}
	}




	void CheckLiteral() {
		switch (t.val) {
			case "material": t.kind = 5; break;
			case "tex": t.kind = 8; break;
			case "texture": t.kind = 9; break;
			case "depth": t.kind = 10; break;
			case "default": t.kind = 11; break;
			case "read": t.kind = 12; break;
			case "none": t.kind = 13; break;
			case "depth_check": t.kind = 14; break;
			case "depth_write": t.kind = 15; break;
			case "depth_stencil": t.kind = 16; break;
			case "stencil_mask": t.kind = 17; break;
			case "stencil_ref": t.kind = 18; break;
			case "stencil_write": t.kind = 19; break;
			case "stencil_two_sided": t.kind = 20; break;
			case "depth_counter_fail": t.kind = 21; break;
			case "stencil_counter_fail": t.kind = 22; break;
			case "stencil_counter_pass": t.kind = 23; break;
			case "depth_fail": t.kind = 24; break;
			case "stencil_pass": t.kind = 25; break;
			case "stencil_fail": t.kind = 26; break;
			case "keep": t.kind = 27; break;
			case "zero": t.kind = 28; break;
			case "replace": t.kind = 29; break;
			case "inc": t.kind = 30; break;
			case "dec": t.kind = 31; break;
			case "inc_sat": t.kind = 32; break;
			case "dec_sat": t.kind = 33; break;
			case "invert": t.kind = 34; break;
			case "stencil_func": t.kind = 35; break;
			case "counter_stencil_func": t.kind = 36; break;
			case "depth_func": t.kind = 37; break;
			case "always": t.kind = 38; break;
			case "never": t.kind = 39; break;
			case "less": t.kind = 40; break;
			case "less_eq": t.kind = 41; break;
			case "eq": t.kind = 42; break;
			case "greater_eq": t.kind = 43; break;
			case "greater": t.kind = 44; break;
			case "not_eq": t.kind = 45; break;
			case "rasterizer": t.kind = 46; break;
			case "cullclockwise": t.kind = 47; break;
			case "cullcounterclockwise": t.kind = 48; break;
			case "cullnone": t.kind = 49; break;
			case "depth_bias": t.kind = 50; break;
			case "slope_Depth_bias": t.kind = 51; break;
			case "multi_alias": t.kind = 52; break;
			case "multi_sample": t.kind = 53; break;
			case "anti_alias": t.kind = 54; break;
			case "cull": t.kind = 55; break;
			case "cull_mode": t.kind = 56; break;
			case "clock": t.kind = 57; break;
			case "clockwise": t.kind = 58; break;
			case "counter": t.kind = 59; break;
			case "counter_clock": t.kind = 60; break;
			case "counter_clockwise": t.kind = 61; break;
			case "fill": t.kind = 62; break;
			case "fill_mode": t.kind = 63; break;
			case "solid": t.kind = 64; break;
			case "wire": t.kind = 65; break;
			case "wire_frame": t.kind = 66; break;
			case "blend": t.kind = 67; break;
			case "add": t.kind = 68; break;
			case "additive": t.kind = 69; break;
			case "alpha": t.kind = 70; break;
			case "alpha_blend": t.kind = 71; break;
			case "non": t.kind = 72; break;
			case "pre": t.kind = 73; break;
			case "nonpremultiplied": t.kind = 74; break;
			case "opaque": t.kind = 75; break;
			case "multi_sample_mask": t.kind = 76; break;
			case "color_blend": t.kind = 77; break;
			case "alpha_src": t.kind = 78; break;
			case "alpha_dst": t.kind = 79; break;
			case "color_src": t.kind = 80; break;
			case "color_dst": t.kind = 81; break;
			case "blend_factor": t.kind = 82; break;
			case "write_channel": t.kind = 83; break;
			case "0": t.kind = 84; break;
			case "1": t.kind = 85; break;
			case "2": t.kind = 86; break;
			case "3": t.kind = 87; break;
			case "red": t.kind = 88; break;
			case "green": t.kind = 89; break;
			case "blue": t.kind = 90; break;
			case "Alpha": t.kind = 91; break;
			case "all": t.kind = 92; break;
			case "subtract": t.kind = 93; break;
			case "reversesubtract": t.kind = 94; break;
			case "max": t.kind = 95; break;
			case "min": t.kind = 96; break;
			case "one": t.kind = 97; break;
			case "src_color": t.kind = 98; break;
			case "inv_src_color": t.kind = 99; break;
			case "src_alpha": t.kind = 100; break;
			case "inv_src_alpha": t.kind = 101; break;
			case "dst_color": t.kind = 102; break;
			case "inv_dst_color": t.kind = 103; break;
			case "dst_alpha": t.kind = 104; break;
			case "inv_dst_alpha": t.kind = 105; break;
			case "inv_blend": t.kind = 106; break;
			case "inv_blend_factor": t.kind = 107; break;
			case "src_alpha_sat": t.kind = 108; break;
			case "src_alpha_saturation": t.kind = 109; break;
			case "sampler": t.kind = 110; break;
			case "anisotropic": t.kind = 111; break;
			case "wrap": t.kind = 112; break;
			case "clamp": t.kind = 113; break;
			case "linear": t.kind = 114; break;
			case "point": t.kind = 115; break;
			case "addressu": t.kind = 116; break;
			case "addressw": t.kind = 117; break;
			case "addressv": t.kind = 118; break;
			case "address": t.kind = 119; break;
			case "mirror": t.kind = 120; break;
			case "MaxAnisotropy": t.kind = 121; break;
			case "MaxMipLevel": t.kind = 122; break;
			case "MipMapLevelOfDetailBias": t.kind = 123; break;
			case "false": t.kind = 124; break;
			case "true": t.kind = 125; break;
			default: break;
		}
	}

	Token NextToken() {
		while (ch == ' ' ||
			ch >= 9 && ch <= 10 || ch == 13
		) NextCh();

		int recKind = noSym;
		int recEnd = pos;
		t = new Token();
		t.pos = pos; t.col = col; t.line = line; t.charPos = charPos;
		int state;
		if (start.ContainsKey(ch)) { state = (int) start[ch]; }
		else { state = 0; }
		tlen = 0; AddCh();
		
		switch (state) {
			case -1: { t.kind = eofSym; break; } // NextCh already done
			case 0: {
				if (recKind != noSym) {
					tlen = recEnd - t.pos;
					SetScannerBehindT();
				}
				t.kind = recKind; break;
			} // NextCh already done
			case 1:
				if (ch == '.' || ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == ':') {AddCh(); goto case 2;}
				else {goto case 0;}
			case 2:
				if (ch == '.' || ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 3;}
				else {goto case 0;}
			case 3:
				recEnd = pos; recKind = 2;
				if (ch == '/' || ch == 92) {AddCh(); goto case 4;}
				else if (ch == '.' || ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 3;}
				else {t.kind = 2; break;}
			case 4:
				if (ch == '.' || ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 5;}
				else {goto case 0;}
			case 5:
				recEnd = pos; recKind = 2;
				if (ch == '/' || ch == 92) {AddCh(); goto case 4;}
				else if (ch == '.' || ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 5;}
				else {t.kind = 2; break;}
			case 6:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 7;}
				else {goto case 0;}
			case 7:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 7;}
				else if (ch == 'f') {AddCh(); goto case 8;}
				else {goto case 0;}
			case 8:
				{t.kind = 3; break;}
			case 9:
				recEnd = pos; recKind = 4;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 9;}
				else {t.kind = 4; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 10:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 10;}
				else if (ch == '.' || ch >= '0' && ch <= '9') {AddCh(); goto case 1;}
				else if (ch == ':') {AddCh(); goto case 2;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 11:
				recEnd = pos; recKind = 4;
				if (ch == '.') {AddCh(); goto case 6;}
				else if (ch >= '0' && ch <= '9') {AddCh(); goto case 9;}
				else if (ch == 'f') {AddCh(); goto case 8;}
				else {t.kind = 4; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 12:
				{t.kind = 6; break;}
			case 13:
				{t.kind = 7; break;}

		}
		t.val = new String(tval, 0, tlen);
		return t;
	}
	
	private void SetScannerBehindT() {
		buffer.Pos = t.pos;
		NextCh();
		line = t.line; col = t.col; charPos = t.charPos;
		for (int i = 0; i < tlen; i++) NextCh();
	}
	
	// get the next token (possibly a token already seen during peeking)
	public Token Scan () {
		if (tokens.next == null) {
			return NextToken();
		} else {
			pt = tokens = tokens.next;
			return tokens;
		}
	}

	// peek for the next token, ignore pragmas
	public Token Peek () {
		do {
			if (pt.next == null) {
				pt.next = NextToken();
			}
			pt = pt.next;
		} while (pt.kind > maxT); // skip pragmas
	
		return pt;
	}

	// make sure that peeking starts at the current scan position
	public void ResetPeek () { pt = tokens; }

} // end Scanner
}