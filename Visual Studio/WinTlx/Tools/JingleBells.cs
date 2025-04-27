using System.Collections.Generic;
using System.IO;

namespace WinTlx.Tools
{
	// CuriousMarc's Jingle Bells for TTY
	// https://www.youtube.com/watch?v=Mt4EDsT7Gto
	// https://drive.google.com/file/d/1DROZ-5wChx-jCNFvJOHm65JZ4pJGvPN-/view

	internal class JingleBells
	{
		byte C = 0x08; // cr
		byte L = 0x02; // lf
		byte F = 0x1B; // figs
		byte X = 0x1F; // ltrs
		byte S = 0x0B; // bell (ITA2)
		//byte S = 0x05; // bell (USTTY)
		byte Z = 0x04; // blank

		byte m = 0x1C;
		byte e = 0x01;
		byte r = 0x0A;
		byte y = 0x15;
		byte c = 0x0E;
		byte h = 0x14;
		byte i = 0x06;
		byte s = 0x05;
		byte t = 0x10;
		byte a = 0x03;
		byte n = 0x0C;
		byte d = 0x09;
		byte p = 0x16;
		byte w = 0x13;
		byte o = 0x18;
		byte l = 0x12;

		string pattern = 
			"CL" + 
			"FSFSFSXmFS FSFSXeFSFS FSFFSSXrry" +
			"ZFFSFSFSFF SSFSFSFSSS" +
			"FSFSFSFSXc FSXhFSFSFS XrFSFSFSXi" +
			"FSFSFSFFSS XstmasFSFS" +
			"FSFFSSFSFS FSSSFSFSFS FSXZanFSSS" +
			"FSFSFSFSXd ZaFSSSFSFS" +
			"FSFSXZhaFS SSFSFSFSFS FSFSFSSSFS" +
			"FSFSFSXp FFFSSS" +
			"FSFSFSFSFF FFFSSSFSFS" +
			"FSFSFFFFFS SSFSFSFSFS FSFSFSSSFS" +
			"FSFSFSFFFS XpFSFSFSXy" +
			"FSFSFSXZFS FSFSFFSSXn ewZyFSFSFS" +
			"FFSSFSFSFS SSFSFSFSFS" +
			"XeFSXaFSFS FSXrFSFSFS XZFSFSFSFF" +
			"SSXtoZalFS FSFSFFSSFS" +
			"FSFSSSFS FSFSFSXl" + 
			"CL";

		private Dictionary<char, byte> _codes;

		public JingleBells()
		{
			_codes = new Dictionary<char, byte>();
			
			_codes['C'] = C;
			_codes['L'] = L;

			_codes['F'] = F;
			_codes['X'] = X;
			_codes['S'] = S;
			_codes['Z'] = Z;
			_codes['m'] = m;
			_codes['e'] = e;
			_codes['r'] = r;
			_codes['y'] = y;
			_codes['c'] = c;
			_codes['h'] = h;
			_codes['i'] = i;
			_codes['s'] = s;
			_codes['t'] = t;
			_codes['a'] = a;
			_codes['n'] = n;
			_codes['d'] = d;
			_codes['p'] = p;
			_codes['w'] = w;
			_codes['o'] = o;
			_codes['l'] = l;
		}

		public void CreateLs()
		{
			List<byte> seq = new List<byte>();
			foreach(char c in pattern)
			{
				if (c == ' ') continue;
				byte b = _codes[c];
				seq.Add(b);
			}
			File.WriteAllBytes(@"d:\jinglebells.ls", seq.ToArray());
		}
	}
}
