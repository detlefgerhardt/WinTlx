using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx
{
	class ScreenLine
	{
		public ScreenChar[] Line { get; set; }

		public ScreenLine()
		{
			Line = new ScreenChar[MainForm.SCREEN_WIDTH+1];
			for (int i = 0; i < MainForm.SCREEN_WIDTH+1; i++)
			{
				Line[i] = new ScreenChar();
			}
		}

		public string LineStr
		{
			get
			{
				string str = "";
				for (int i=0; i<MainForm.SCREEN_WIDTH; i++)
				{
					str += Line[i].Char;
				}
				return str.TrimEnd();
			}
		}
	}
}
