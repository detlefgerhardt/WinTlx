using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Codes
{
	public enum ShiftStates
	{
		Unknown,
		Ltr,
		Figs,
		Both
	}

	//public enum CodeSets { ITA2 = 0, ITA2EXT = 1, USTTY = 2 };
	public enum CodeSets { ITA2 = 0, USTTY = 1 };

	public class CodeItem
	{

		public const int CODESETS_COUNT = 2;

		public byte Code { get; set; }

		public char?[] CharLtr { get; set; }

		public char?[] CharFig { get; set; }

		private char? CharExt { get; set; }

		public char GetCharExt(int codeSet)
		{
			if (CharFig[codeSet]!=null)
			{
				return CharFig[codeSet].Value;
			}
			else
			{
				return CharExt.Value;
			}
		}

		public string[] NameLtr { get; set; }

		public string[] NameFig { get; set; }

		public string NameExt { get; set; }
		public string GetNameExt(int codeSet)
		{
			if (CharFig[codeSet] != null)
			{
				return NameFig[codeSet];
			}
			else
			{
				return NameExt;
			}
		}

		private Exception _illegalCodeExeption = new Exception("illegal code definition");

		public CodeItem(byte code, char?[] charLtr, char?[] charFig, string[] nameLtr, string[] nameFig, char? charExt = null, string nameExt = null)
		{
			Code = code;

			if (charLtr.Length==CODESETS_COUNT)
			{
				CharLtr = charLtr;
			}
			else if (charLtr.Length==1)
			{
				SetCharLtr(charLtr[0]);
			}
			else
			{
				throw _illegalCodeExeption;
			}

			if (charFig.Length == CODESETS_COUNT)
			{
				CharFig = charFig;
			}
			else if (charFig.Length==1)
			{
				SetCharFig(charFig[0]);
			}
			else
			{
				throw _illegalCodeExeption;
			}

			if (nameLtr.Length == CODESETS_COUNT)
			{
				NameLtr = nameLtr;
			}
			else if (nameLtr.Length == 1)
			{
				SetNameLtr(nameLtr[0]);
			}
			else
			{
				throw _illegalCodeExeption;
			}

			if (nameFig.Length == CODESETS_COUNT)
			{
				NameFig = nameFig;
			}
			else if (nameFig.Length == 1)
			{
				SetNameFig(nameFig[0]);
			}
			else
			{
				throw _illegalCodeExeption;
			}

			CharExt = charExt;
			NameExt = nameExt;
		}

		/// <summary>
		/// Short initialization if both shift levels are equal
		/// </summary>
		/// <param name="code"></param>
		/// <param name="charLtrFig"></param>
		/// <param name="nameLtrFig"></param>
		public CodeItem(byte code, char charLtrFig, string[] nameLtrFig)
		{
			Code = code;
			SetCharLtr(charLtrFig);
			SetCharFig(charLtrFig);
			NameLtr = nameLtrFig;
			NameFig = nameLtrFig;
			CharExt = null;
			NameExt = null;
		}

		private void SetCharLtr(char? code)
		{
			CharLtr = new char?[CODESETS_COUNT];
			for (int i = 0; i < CODESETS_COUNT; i++)
			{
				CharLtr[i] = code;
			}
		}

		private void SetCharFig(char? code)
		{
			CharFig = new char?[CODESETS_COUNT];
			for (int i = 0; i < CODESETS_COUNT; i++)
			{
				CharFig[i] = code;
			}
		}

		private void SetNameLtr(string name)
		{
			NameLtr = new string[CODESETS_COUNT];
			for (int i = 0; i < CODESETS_COUNT; i++)
			{
				NameLtr[i] = name;
			}
		}

		private void SetNameFig(string name)
		{
			NameFig = new string[CODESETS_COUNT];
			for (int i = 0; i < CODESETS_COUNT; i++)
			{
				NameFig[i] = name;
			}
		}

		public char GetCode(ShiftStates shiftState, CodeSets codeSet)
		{
			char? chr;
			switch (shiftState)
			{
				case ShiftStates.Ltr:
				case ShiftStates.Both:
					chr = CharLtr[(int)codeSet];
					break;
				case ShiftStates.Figs:
					chr = CharFig[(int)codeSet];
					break;
				default:
				case ShiftStates.Unknown:
					chr = null;
					break;
			}
			if (chr==null)
			{
				chr = CodeManager.ASC_INV;
				/*
				if (ext && CharExt!=null)
				{
					chr = CharExt.Value;
				}
				else
				{
					chr = CodeConversion.ASC_INV;
				}
				*/
			}

			return chr.Value;
		}

		public string GetName(ShiftStates shiftState, CodeSets codeSet)
		{
			string name;
			switch (shiftState)
			{
				case ShiftStates.Ltr:
				case ShiftStates.Both:
					name = NameLtr[(int)codeSet];
					break;
				case ShiftStates.Figs:
					name = NameFig[(int)codeSet];
					break;
				default:
				case ShiftStates.Unknown:
					name = null;
					break;
			}
			if (name == null)
			{
				name = CodeManager.ASC_INV.ToString();
				/*
				if (ext && CharExt != null)
				{
					name = NameExt;
				}
				else
				{
					name = CodeConversion.ASC_INV.ToString();
				}
				*/
			}

			return name;
		}

	}
}
