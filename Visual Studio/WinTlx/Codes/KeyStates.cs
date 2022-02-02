using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Codes
{
	public class KeyStates
	{
		public ShiftStates ShiftState { get; set; }

		public CodeSets CodeSet { get; set; }

		public KeyStates()
		{
			ShiftState = ShiftStates.Unknown;
			CodeSet = CodeSets.ITA2;
		}

		public KeyStates (ShiftStates shiftState, CodeSets codeSet)
		{
			ShiftState = shiftState;
			CodeSet = codeSet;
		}

		public override string ToString()
		{
			return $"{ShiftState} {CodeSet}";
		}
	}
}
