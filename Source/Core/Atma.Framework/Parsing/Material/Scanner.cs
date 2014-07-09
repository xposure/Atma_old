
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
	const int maxT = 60;
	const int noSym = 60;


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
		for (int i = 65; i <= 90; ++i) start[i] = 1;
		for (int i = 101; i <= 104; ++i) start[i] = 1;
		for (int i = 106; i <= 108; ++i) start[i] = 1;
		for (int i = 110; i <= 114; ++i) start[i] = 1;
		for (int i = 116; i <= 118; ++i) start[i] = 1;
		for (int i = 120; i <= 122; ++i) start[i] = 1;
		for (int i = 48; i <= 49; ++i) start[i] = 6;
		for (int i = 50; i <= 57; ++i) start[i] = 5;
		start[123] = 7; 
		start[125] = 8; 
		start[109] = 107; 
		start[97] = 108; 
		start[99] = 109; 
		start[98] = 110; 
		start[119] = 111; 
		start[115] = 112; 
		start[105] = 113; 
		start[100] = 114; 
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
			case "material": t.kind = 4; break;
			case "0": t.kind = 16; break;
			case "1": t.kind = 17; break;
			case "2": t.kind = 18; break;
			case "3": t.kind = 19; break;
			case "none": t.kind = 20; break;
			case "red": t.kind = 21; break;
			case "green": t.kind = 22; break;
			case "blue": t.kind = 23; break;
			case "Alpha": t.kind = 24; break;
			case "all": t.kind = 25; break;
			case "add": t.kind = 26; break;
			case "subtract": t.kind = 27; break;
			case "reversesubtract": t.kind = 28; break;
			case "max": t.kind = 29; break;
			case "min": t.kind = 30; break;
			case "one": t.kind = 31; break;
			case "zero": t.kind = 32; break;
			case "blend": t.kind = 41; break;
			case "sampler": t.kind = 46; break;
			case "anisotropic": t.kind = 47; break;
			case "wrap": t.kind = 48; break;
			case "clamp": t.kind = 49; break;
			case "linear": t.kind = 50; break;
			case "point": t.kind = 51; break;
			case "addressu": t.kind = 52; break;
			case "addressw": t.kind = 53; break;
			case "addressv": t.kind = 54; break;
			case "address": t.kind = 55; break;
			case "mirror": t.kind = 56; break;
			case "MaxAnisotropy": t.kind = 57; break;
			case "MaxMipLevel": t.kind = 58; break;
			case "MipMapLevelOfDetailBias": t.kind = 59; break;
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
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 1;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 2:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 3;}
				else {goto case 0;}
			case 3:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 3;}
				else if (ch == 'f') {AddCh(); goto case 4;}
				else {goto case 0;}
			case 4:
				{t.kind = 2; break;}
			case 5:
				recEnd = pos; recKind = 3;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 5;}
				else {t.kind = 3; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 6:
				recEnd = pos; recKind = 3;
				if (ch == '.') {AddCh(); goto case 2;}
				else if (ch >= '0' && ch <= '9') {AddCh(); goto case 5;}
				else if (ch == 'f') {AddCh(); goto case 4;}
				else {t.kind = 3; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 7:
				{t.kind = 5; break;}
			case 8:
				{t.kind = 6; break;}
			case 9:
				if (ch == 's') {AddCh(); goto case 10;}
				else {goto case 0;}
			case 10:
				if (ch == 'a') {AddCh(); goto case 11;}
				else {goto case 0;}
			case 11:
				if (ch == 'm') {AddCh(); goto case 12;}
				else {goto case 0;}
			case 12:
				if (ch == 'p') {AddCh(); goto case 13;}
				else {goto case 0;}
			case 13:
				if (ch == 'l') {AddCh(); goto case 14;}
				else {goto case 0;}
			case 14:
				if (ch == 'e') {AddCh(); goto case 15;}
				else {goto case 0;}
			case 15:
				if (ch == '_') {AddCh(); goto case 16;}
				else {goto case 0;}
			case 16:
				if (ch == 'm') {AddCh(); goto case 17;}
				else {goto case 0;}
			case 17:
				if (ch == 'a') {AddCh(); goto case 18;}
				else {goto case 0;}
			case 18:
				if (ch == 's') {AddCh(); goto case 19;}
				else {goto case 0;}
			case 19:
				if (ch == 'k') {AddCh(); goto case 20;}
				else {goto case 0;}
			case 20:
				{t.kind = 7; break;}
			case 21:
				if (ch == 'l') {AddCh(); goto case 22;}
				else {goto case 0;}
			case 22:
				if (ch == 'e') {AddCh(); goto case 23;}
				else {goto case 0;}
			case 23:
				if (ch == 'n') {AddCh(); goto case 24;}
				else {goto case 0;}
			case 24:
				if (ch == 'd') {AddCh(); goto case 25;}
				else {goto case 0;}
			case 25:
				{t.kind = 8; break;}
			case 26:
				if (ch == 'l') {AddCh(); goto case 27;}
				else {goto case 0;}
			case 27:
				if (ch == 'e') {AddCh(); goto case 28;}
				else {goto case 0;}
			case 28:
				if (ch == 'n') {AddCh(); goto case 29;}
				else {goto case 0;}
			case 29:
				if (ch == 'd') {AddCh(); goto case 30;}
				else {goto case 0;}
			case 30:
				{t.kind = 9; break;}
			case 31:
				if (ch == 'r') {AddCh(); goto case 32;}
				else {goto case 0;}
			case 32:
				if (ch == 'c') {AddCh(); goto case 33;}
				else {goto case 0;}
			case 33:
				{t.kind = 10; break;}
			case 34:
				if (ch == 's') {AddCh(); goto case 35;}
				else {goto case 0;}
			case 35:
				if (ch == 't') {AddCh(); goto case 36;}
				else {goto case 0;}
			case 36:
				{t.kind = 11; break;}
			case 37:
				if (ch == 'r') {AddCh(); goto case 38;}
				else {goto case 0;}
			case 38:
				if (ch == 'c') {AddCh(); goto case 39;}
				else {goto case 0;}
			case 39:
				{t.kind = 12; break;}
			case 40:
				if (ch == 's') {AddCh(); goto case 41;}
				else {goto case 0;}
			case 41:
				if (ch == 't') {AddCh(); goto case 42;}
				else {goto case 0;}
			case 42:
				{t.kind = 13; break;}
			case 43:
				if (ch == 'f') {AddCh(); goto case 44;}
				else {goto case 0;}
			case 44:
				if (ch == 'a') {AddCh(); goto case 45;}
				else {goto case 0;}
			case 45:
				if (ch == 'c') {AddCh(); goto case 46;}
				else {goto case 0;}
			case 46:
				if (ch == 't') {AddCh(); goto case 47;}
				else {goto case 0;}
			case 47:
				if (ch == 'o') {AddCh(); goto case 48;}
				else {goto case 0;}
			case 48:
				if (ch == 'r') {AddCh(); goto case 49;}
				else {goto case 0;}
			case 49:
				{t.kind = 14; break;}
			case 50:
				if (ch == 'c') {AddCh(); goto case 51;}
				else {goto case 0;}
			case 51:
				if (ch == 'h') {AddCh(); goto case 52;}
				else {goto case 0;}
			case 52:
				if (ch == 'a') {AddCh(); goto case 53;}
				else {goto case 0;}
			case 53:
				if (ch == 'n') {AddCh(); goto case 54;}
				else {goto case 0;}
			case 54:
				if (ch == 'n') {AddCh(); goto case 55;}
				else {goto case 0;}
			case 55:
				if (ch == 'e') {AddCh(); goto case 56;}
				else {goto case 0;}
			case 56:
				if (ch == 'l') {AddCh(); goto case 57;}
				else {goto case 0;}
			case 57:
				{t.kind = 15; break;}
			case 58:
				if (ch == 'o') {AddCh(); goto case 59;}
				else {goto case 0;}
			case 59:
				if (ch == 'l') {AddCh(); goto case 60;}
				else {goto case 0;}
			case 60:
				if (ch == 'o') {AddCh(); goto case 61;}
				else {goto case 0;}
			case 61:
				if (ch == 'r') {AddCh(); goto case 62;}
				else {goto case 0;}
			case 62:
				{t.kind = 33; break;}
			case 63:
				if (ch == 'o') {AddCh(); goto case 64;}
				else {goto case 0;}
			case 64:
				if (ch == 'l') {AddCh(); goto case 65;}
				else {goto case 0;}
			case 65:
				if (ch == 'o') {AddCh(); goto case 66;}
				else {goto case 0;}
			case 66:
				if (ch == 'r') {AddCh(); goto case 67;}
				else {goto case 0;}
			case 67:
				{t.kind = 34; break;}
			case 68:
				if (ch == 'l') {AddCh(); goto case 69;}
				else {goto case 0;}
			case 69:
				if (ch == 'p') {AddCh(); goto case 70;}
				else {goto case 0;}
			case 70:
				if (ch == 'h') {AddCh(); goto case 71;}
				else {goto case 0;}
			case 71:
				if (ch == 'a') {AddCh(); goto case 72;}
				else {goto case 0;}
			case 72:
				{t.kind = 36; break;}
			case 73:
				if (ch == 'o') {AddCh(); goto case 74;}
				else {goto case 0;}
			case 74:
				if (ch == 'l') {AddCh(); goto case 75;}
				else {goto case 0;}
			case 75:
				if (ch == 'o') {AddCh(); goto case 76;}
				else {goto case 0;}
			case 76:
				if (ch == 'r') {AddCh(); goto case 77;}
				else {goto case 0;}
			case 77:
				{t.kind = 37; break;}
			case 78:
				if (ch == 'o') {AddCh(); goto case 79;}
				else {goto case 0;}
			case 79:
				if (ch == 'l') {AddCh(); goto case 80;}
				else {goto case 0;}
			case 80:
				if (ch == 'o') {AddCh(); goto case 81;}
				else {goto case 0;}
			case 81:
				if (ch == 'r') {AddCh(); goto case 82;}
				else {goto case 0;}
			case 82:
				{t.kind = 38; break;}
			case 83:
				if (ch == 'l') {AddCh(); goto case 84;}
				else {goto case 0;}
			case 84:
				if (ch == 'p') {AddCh(); goto case 85;}
				else {goto case 0;}
			case 85:
				if (ch == 'h') {AddCh(); goto case 86;}
				else {goto case 0;}
			case 86:
				if (ch == 'a') {AddCh(); goto case 87;}
				else {goto case 0;}
			case 87:
				{t.kind = 39; break;}
			case 88:
				if (ch == 'l') {AddCh(); goto case 89;}
				else {goto case 0;}
			case 89:
				if (ch == 'p') {AddCh(); goto case 90;}
				else {goto case 0;}
			case 90:
				if (ch == 'h') {AddCh(); goto case 91;}
				else {goto case 0;}
			case 91:
				if (ch == 'a') {AddCh(); goto case 92;}
				else {goto case 0;}
			case 92:
				{t.kind = 40; break;}
			case 93:
				if (ch == 'f') {AddCh(); goto case 94;}
				else {goto case 0;}
			case 94:
				if (ch == 'a') {AddCh(); goto case 95;}
				else {goto case 0;}
			case 95:
				if (ch == 'c') {AddCh(); goto case 96;}
				else {goto case 0;}
			case 96:
				if (ch == 't') {AddCh(); goto case 97;}
				else {goto case 0;}
			case 97:
				if (ch == 'o') {AddCh(); goto case 98;}
				else {goto case 0;}
			case 98:
				if (ch == 'r') {AddCh(); goto case 99;}
				else {goto case 0;}
			case 99:
				{t.kind = 43; break;}
			case 100:
				if (ch == 'r') {AddCh(); goto case 101;}
				else {goto case 0;}
			case 101:
				if (ch == 'a') {AddCh(); goto case 102;}
				else {goto case 0;}
			case 102:
				if (ch == 't') {AddCh(); goto case 103;}
				else {goto case 0;}
			case 103:
				if (ch == 'i') {AddCh(); goto case 104;}
				else {goto case 0;}
			case 104:
				if (ch == 'o') {AddCh(); goto case 105;}
				else {goto case 0;}
			case 105:
				if (ch == 'n') {AddCh(); goto case 106;}
				else {goto case 0;}
			case 106:
				{t.kind = 45; break;}
			case 107:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 't' || ch >= 'v' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'u') {AddCh(); goto case 115;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 108:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'k' || ch >= 'm' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'l') {AddCh(); goto case 116;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 109:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'n' || ch >= 'p' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'o') {AddCh(); goto case 117;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 110:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'k' || ch >= 'm' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'l') {AddCh(); goto case 118;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 111:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'q' || ch >= 's' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'r') {AddCh(); goto case 119;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 112:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'q' || ch >= 's' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'r') {AddCh(); goto case 120;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 113:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'm' || ch >= 'o' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'n') {AddCh(); goto case 121;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 114:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'r' || ch >= 't' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 's') {AddCh(); goto case 122;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 115:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'k' || ch >= 'm' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'l') {AddCh(); goto case 123;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 116:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'o' || ch >= 'q' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'p') {AddCh(); goto case 124;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 117:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'k' || ch >= 'm' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'l') {AddCh(); goto case 125;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 118:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'd' || ch >= 'f' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'e') {AddCh(); goto case 126;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 119:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'h' || ch >= 'j' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'i') {AddCh(); goto case 127;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 120:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'b' || ch >= 'd' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'c') {AddCh(); goto case 128;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 121:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'u' || ch >= 'w' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'v') {AddCh(); goto case 129;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 122:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 's' || ch >= 'u' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 't') {AddCh(); goto case 130;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 123:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 's' || ch >= 'u' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 't') {AddCh(); goto case 131;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 124:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'g' || ch >= 'i' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'h') {AddCh(); goto case 132;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 125:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'n' || ch >= 'p' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'o') {AddCh(); goto case 133;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 126:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'm' || ch >= 'o' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'n') {AddCh(); goto case 134;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 127:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 's' || ch >= 'u' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 't') {AddCh(); goto case 135;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 128:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == '_') {AddCh(); goto case 136;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 129:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == '_') {AddCh(); goto case 137;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 130:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == '_') {AddCh(); goto case 138;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 131:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'h' || ch >= 'j' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'i') {AddCh(); goto case 139;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 132:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'b' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'a') {AddCh(); goto case 140;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 133:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'q' || ch >= 's' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'r') {AddCh(); goto case 141;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 134:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'c' || ch >= 'e' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'd') {AddCh(); goto case 142;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 135:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'd' || ch >= 'f' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == 'e') {AddCh(); goto case 143;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 136:
				if (ch == 'c') {AddCh(); goto case 58;}
				else if (ch == 'a') {AddCh(); goto case 144;}
				else {goto case 0;}
			case 137:
				if (ch == 's') {AddCh(); goto case 145;}
				else if (ch == 'd') {AddCh(); goto case 146;}
				else if (ch == 'b') {AddCh(); goto case 147;}
				else {goto case 0;}
			case 138:
				if (ch == 'c') {AddCh(); goto case 73;}
				else if (ch == 'a') {AddCh(); goto case 83;}
				else {goto case 0;}
			case 139:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == '_') {AddCh(); goto case 9;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 140:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == '_') {AddCh(); goto case 148;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 141:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == '_') {AddCh(); goto case 149;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 142:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == '_') {AddCh(); goto case 43;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 143:
				recEnd = pos; recKind = 1;
				if (ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 1;}
				else if (ch == '_') {AddCh(); goto case 50;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 144:
				if (ch == 'l') {AddCh(); goto case 150;}
				else {goto case 0;}
			case 145:
				if (ch == 'r') {AddCh(); goto case 151;}
				else {goto case 0;}
			case 146:
				if (ch == 's') {AddCh(); goto case 152;}
				else {goto case 0;}
			case 147:
				if (ch == 'l') {AddCh(); goto case 153;}
				else {goto case 0;}
			case 148:
				if (ch == 'b') {AddCh(); goto case 21;}
				else if (ch == 's') {AddCh(); goto case 31;}
				else if (ch == 'd') {AddCh(); goto case 34;}
				else {goto case 0;}
			case 149:
				if (ch == 'b') {AddCh(); goto case 26;}
				else if (ch == 's') {AddCh(); goto case 37;}
				else if (ch == 'd') {AddCh(); goto case 40;}
				else {goto case 0;}
			case 150:
				if (ch == 'p') {AddCh(); goto case 154;}
				else {goto case 0;}
			case 151:
				if (ch == 'c') {AddCh(); goto case 155;}
				else {goto case 0;}
			case 152:
				if (ch == 't') {AddCh(); goto case 156;}
				else {goto case 0;}
			case 153:
				if (ch == 'e') {AddCh(); goto case 157;}
				else {goto case 0;}
			case 154:
				if (ch == 'h') {AddCh(); goto case 158;}
				else {goto case 0;}
			case 155:
				if (ch == '_') {AddCh(); goto case 159;}
				else {goto case 0;}
			case 156:
				if (ch == '_') {AddCh(); goto case 160;}
				else {goto case 0;}
			case 157:
				if (ch == 'n') {AddCh(); goto case 161;}
				else {goto case 0;}
			case 158:
				if (ch == 'a') {AddCh(); goto case 162;}
				else {goto case 0;}
			case 159:
				if (ch == 'c') {AddCh(); goto case 63;}
				else if (ch == 'a') {AddCh(); goto case 68;}
				else {goto case 0;}
			case 160:
				if (ch == 'c') {AddCh(); goto case 78;}
				else if (ch == 'a') {AddCh(); goto case 88;}
				else {goto case 0;}
			case 161:
				if (ch == 'd') {AddCh(); goto case 163;}
				else {goto case 0;}
			case 162:
				recEnd = pos; recKind = 35;
				if (ch == '_') {AddCh(); goto case 164;}
				else {t.kind = 35; break;}
			case 163:
				recEnd = pos; recKind = 42;
				if (ch == '_') {AddCh(); goto case 93;}
				else {t.kind = 42; break;}
			case 164:
				if (ch == 's') {AddCh(); goto case 165;}
				else {goto case 0;}
			case 165:
				if (ch == 'a') {AddCh(); goto case 166;}
				else {goto case 0;}
			case 166:
				if (ch == 't') {AddCh(); goto case 167;}
				else {goto case 0;}
			case 167:
				recEnd = pos; recKind = 44;
				if (ch == 'u') {AddCh(); goto case 100;}
				else {t.kind = 44; break;}

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