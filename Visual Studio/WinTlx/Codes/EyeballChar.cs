using System;
using System.Collections.Generic;
using System.Linq;

namespace WinTlx.Codes
{
	class EyeballChar
	{
		private List<PunchCode> _punchCodes;

		/// <summary>
		/// singleton pattern
		/// </summary>
		private static EyeballChar instance;

		public static EyeballChar Instance => instance ?? (instance = new EyeballChar());

		private EyeballChar()
		{
			Init();
		}

		private void Init()
		{
			_punchCodes = new List<PunchCode>();
			for (int shift = 0; shift < 2; shift++)
			{
				for (int code = 0; code < 32; code++)
				{
					int pos = _charPos[shift, code];
					if (pos==-1)
					{
						continue;
					}
					int width = 0;
					for (int i = 0; i < 5; i++)
					{
						if (_charLines[pos, i].Length> width)
						{
							width = _charLines[pos, i].Length;
						}
					}
					PunchCode punchCode = new PunchCode()
					{
						BaudotCode = code + shift * 32,
						Pattern = new byte[width + 1]
					};

					for (int w=0; w<width; w++)
					{ 
						int pattern = 0;
						int s = 16;
						for (int i = 0; i < 5; i++)
						{
							if (_charLines[pos, i].Length-1>=w && _charLines[pos, i][w] != ' ')
							{
								pattern += s;
							}
							s >>= 1;
						}
						punchCode.Pattern[w] = (byte)pattern;
					}
					punchCode.Pattern[width] = 0;
					_punchCodes.Add(punchCode);
				}
			}
		}

		/// <summary>
		/// Get readable punchcodes for one baudot character
		/// </summary>
		/// <param name="baudotCode">0..31</param>
		/// <param name="shift">0=letters, 1=figures</param>
		/// <returns></returns>
		public byte[] GetPunchCodes(int baudotCode, ShiftStates shift)
		{
			if (baudotCode == 0x00)
			{   // code32
				return new byte[] { 0x00 };
			}

			int code = baudotCode;
			if (shift == ShiftStates.Figs)
			{
				code += 32;
			}
			byte[] pattern = (from p in _punchCodes where p.BaudotCode == code select p.Pattern).FirstOrDefault();
			if (pattern==null)
			{
				return null;
			}

			return pattern;

			/*
			// add one space row
			byte[] pattern2 = new byte[pattern.Length + 1];
			Buffer.BlockCopy(pattern, 0, pattern2, 0, pattern.Length);
			pattern2[pattern2.Length - 1] = 0;
			return pattern2;

			//return (from p in _punchCodes where p.BaudotCode == code select p.Pattern).FirstOrDefault();
			*/
		}

		private int[,] _charPos =
		{
			// letters
			{
				-1,			// 00
				20,			// 01 t
				-1,			// 02 CR
				15,			// 03 o
				0,  		// 04 space
				8,			// 05 h
				14,			// 06 n
				13,			// 07 m
				-1,			// 08 LF
				12,			// 09 l
				18,			// 0A r
				7,			// 0B g
				9,			// 0C i
				16,			// 0D p
				3,			// 0E c
				22,			// 0F v
				5,			// 10 e
				26,			// 11 z
				4,			// 12 d
				2,			// 13 b
				19,			// 14 s
				25,			// 15 y
				6,			// 16 f
				24,			// 17 x
				1,			// 18 a
				23,			// 19 w
				10,			// 1A j
				-1,			// 1B FIG
				21,			// 1C u
				17,			// 1D q
				11,			// 1E k
				-1			// 1F LTR
			},

			// figures
			{
				-1,			// 00
				31,			// 01 5
				-1,			// 02 CR
				36,			// 03 9
				0,			// 04 SP
				-1,			// 05 $
				47,			// 06 ,
				48,			// 07 .
				-1,			// 08 LF
				42,			// 09 )
				30,			// 0A 4
				-1,			// 0B @
				35,			// 0C 8
				37,			// 0D 0
				44,			// 0E :
				45,			// 0F =
				29,			// 10 3
				38,			// 11 +
				-1,			// 12 WRU
				46,			// 13 ?
				40,			// 14 '
				33,			// 15 6
				49,			// 16 ! / %
				43,			// 17 /
				39,			// 18 -
				28,			// 19 2
				-1,			// 1A BEL
				-1,			// 1B FIG
				34,			// 1C 7
				27,			// 1D 1
				41,			// 1E (
				-1	        // 1F LTR
			}
		};

		public enum EyeballCharEnum
		{
			None,
			Space,
			A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
			D1, D2, D3, D4, D5, D6, D7, D8, D9,
			Plus, Minus, Hyphen, BracketOpen, BracketClose, Slash, Colon, Equal, QuestionMark,
			Comma, Dot, ExclamationMark
		}

		private string[,] _charLines =
		{
			// 00 SPACE
			{
				"  ",
				"  ",
				"  ",
				"  ",
				"  "
			},

			// 01 A
			{
				" **",
				"*  *",
				"****",
				"*  *",
				"*  *"
			},
			// 02 B
			{
				"***",
				"*  *",
				"***",
				"*  *",
				"***"
			},
			// 03 C
			{
				" **",
				"*  *",
				"*",
				"*  *",
				" **"
			},
			// 04 D
			{
				"***",
				"*  *",
				"*  *",
				"*  *",
				"***"
			},
			// 05 E
			{
				"****",
				"*",
				"****",
				"*",
				"****"
			},
			// 06 F
			{
				"****",
				"*",
				"***",
				"*",
				"*"
			},
			// 07 G
			{
				" ***",
				"*",
				"* **",
				"*  *",
				" **"
			},
			// 08 H
			{
				"*  *",
				"*  *",
				"****",
				"*  *",
				"*  *"
			},
			// 09 I
			{
				"***",
				" *",
				" *",
				" *",
				"***"
			},
			// 10 J
			{
				"   *",
				"   *",
				"   *",
				"*  *",
				" **"
			},
			// 11 K
			{
				"*  *",
				"* *",
				"**",
				"* *",
				"*  *"
			},
			// 12 L
			{
				"*",
				"*",
				"*",
				"*",
				"****"
			},
			// 13 M
			{
				"*   *",
				"** **",
				"* * *",
				"*   *",
				"*   *"
			},
			// 14 N
			{
				"*  *",
				"** *",
				"* **",
				"*  *",
				"*  *"
			},
			// 15 O
			{
				" *** ",
				"*   *",
				"*   *",
				"*   *",
				" ***"
			},
			// 16 P
			{
				"***",
				"*  *",
				"***",
				"*",
				"*"
			},
			// 17 Q
			{
				" *** ",
				"*   *",
				"* * *",
				"*  *",
				" ** *"
			},
			// 18 R
			{
				"***",
				"*  *",
				"***",
				"* *",
				"*  *"
			},
			// 19 S
			{
				" ***",
				"*",
				" **",
				"   *",
				"***"
			},
			// 20 T
			{
				"*****",
				"  *",
				"  *",
				"  *",
				"  *"
			},
			// 21 U
			{
				"*  *",
				"*  *",
				"*  *",
				"*  *",
				" **"
			},
			// 22 V
			{
				"*   *",
				"*   *",
				"*   *",
				" * *",
				"  *"
			},
			// 23 W
			{
				"*   *",
				"*   *",
				"*   *",
				"* * *",
				" * *"
			},
			// 24 X
			{
				"*   *",
				" * *",
				"  *",
				" * *",
				"*   *"
			},
			// 25 Y
			{
				"*   *",
				" * *",
				"  *",
				"  *",
				"  *"
			},
			// 26 Z
			{
				"****",
				"   *",
				" *",
				"*",
				"****"
			},
			// 27 1
			{
				" *",
				"**",
				" *",
				" *",
				"***"
			},
			// 28 2
			{
				" **",
				"*  *",
				"  *",
				" *",
				"****"
			},
			// 29 3
			{
				" **",
				"*  *",
				"  **",
				"*  *",
				" **"
			},
			// 30 4
			{
				"  *",
				" *",
				"*  *",
				"****",
				"   *"
			},
			// 31 5
			{
				"****",
				"*",
				"***",
				"   *",
				"***"
			},
			// 32 6
			{
				" ***",
				"*",
				"***",
				"*  *",
				" **"
			},
			// 33 6
			{
				" ***",
				"*",
				"***",
				"*  *",
				" **"
			},
			// 34 7
			{
				"****",
				"   *",
				"  *",
				" *",
				" *"
			},
			// 35 8
			{
				" **",
				"*  *",
				" **",
				"*  *",
				" **"
			},
			// 36 9
			{
				" **",
				"*  *",
				" ***",
				"   *",
				"***"
			},
			// 37 0
			{
				" **",
				"*  *",
				"*  *",
				"*  *",
				" **"
			},
			// 38 +
			{
				"",
				" *",
				"***",
				" *",
				""
			},
			// 39 -
			{
				"",
				"",
				"***",
				"",
				""
			},
			// 40 '
			{
				" *",
				"*",
				"",
				"",
				""
			},
			//{
			//	"**",
			//	" *",
			//	"*",
			//	"",
			//	""
			//},
			// 41 (
			{
				" *",
				"*",
				"*",
				"*",
				" *"
			},
			// 42 )
			{
				"*",
				" *",
				" *",
				" *",
				"*"
			},
			// 43 /
			{
				"   *",
				"  *",
				" *",
				" *",
				"*"
			},
			// 44 :
			{
				"",
				"*",
				"",
				"*",
				""
			},
			// 45 =
			{
				"",
				"***",
				"",
				"***",
				""
			},
			// 46 ?
			{
				" **",
				"*  *",
				"  *",
				"",
				"  *"
			},
			// 47 ,
			{
				"",
				"",
				"",
				" *",
				"*"
			},
			// 48 .
			{
				"",
				"",
				"",
				"**",
				"**"
			},
			// 49 !
			{
				"*",
				"*",
				"*",
				"",
				"*"
			},
		};
	}

	class PunchCode
	{
		public char AsciiCaracter { get; set; }

		public int BaudotCode { get; set; }

		public int Length { get; set; }

		public byte[] Pattern { get; set; }

		public override string ToString()
		{
			return $"{AsciiCaracter} {BaudotCode} {Length}";
		}
	}
}
