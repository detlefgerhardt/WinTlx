using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Codes
{
	public class CodeItem
	{
		public const int CODESETS_COUNT = 3;

		public byte Code { get; set; }

		public char CharLtr { get; set; }
		public string NameLtr { get; set; }

		public char CharFig { get; set; }
		public string NameFig { get; set; }

		public char Char3rd { get; set; }
		public string Name3rd { get; set; }

		//private readonly Exception _illegalCodeExeption = new Exception("illegal code definition");

		public CodeItem(byte code, char charLtr, string nameLtr)
		{
			Code = code;
			CharLtr = charLtr;
			NameLtr = nameLtr;
			CharFig = charLtr;
			NameFig = nameLtr;
		}

		public CodeItem(byte code, char charLtr, string nameLtr, char charFig, string nameFig)
		{
			Code = code;
			CharLtr = charLtr;
			NameLtr = nameLtr;
			CharFig = charFig;
			NameFig = nameFig;
		}

		public CodeItem(byte code, char charLtr, string nameLtr, char charFig, string nameFig, char char3rd, string name3rd)
		{
			Code = code;
			CharLtr = charLtr;
			NameLtr = nameLtr;
			CharFig = charFig;
			NameFig = nameFig;
			Char3rd = char3rd;
			Name3rd = name3rd;
		}

		public char GetChar(ShiftStates shiftState)
		{
			char? chr;
			switch (shiftState)
			{
				case ShiftStates.Ltr:
				case ShiftStates.Both:
				case ShiftStates.Unknown:
					chr = CharLtr;
					break;
				case ShiftStates.Figs:
					chr = CharFig;
					break;
				case ShiftStates.Third:
					chr = Char3rd;
					break;
				default:
					chr = null;
					break;
			}

			if (chr == null)
			{
				chr = CodeManager.ASC_INVALID;
			}

			return chr.Value;
		}

		public string GetName(ShiftStates shiftState)
		{
			string name;
			switch (shiftState)
			{
				case ShiftStates.Ltr:
				case ShiftStates.Both:
					name = NameLtr;
					break;
				case ShiftStates.Figs:
					name = NameFig;
					break;
				default:
				case ShiftStates.Unknown:
					name = null;
					break;
			}
			if (name == null)
			{
				name = CodeManager.ASC_INVALID.ToString();
			}

			return name;
		}

	}
}
