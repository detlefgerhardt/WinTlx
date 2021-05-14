using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinTlx.Languages;

namespace WinTlx.Prueftexte
{
	public enum TestPattern
	{
		Ryry,
		R4y6,
		QuickBrownFox,
		BequemePelze,
		PrallVomWhisky,
		VogelQuax,
		Sylvia,
		WackereBayer,
		FranzJagt,
		Sokrates,
		Line,
		DateTime,
		Numbers
	}

	public partial class TestPatternForm : Form
	{
		public delegate void ClosedEventHandler();
		public event ClosedEventHandler ClosedEvent;

		public delegate void SendEventHandler(string asciiText);
		public event SendEventHandler SendEvent;

		public List<TestPatternItem> PatternList;

		private Rectangle _parentWindowsPosition;

		public TestPatternForm(Rectangle position)
		{
			_parentWindowsPosition = position;

			InitializeComponent();

			string figures = "1234567890,.:-+=/()";
			PatternList = new List<TestPatternItem>()
			{
				new TestPatternItem(TestPattern.Ryry, "ryry", CreatePattern("ryry", 68/4)),
				new TestPatternItem(TestPattern.R4y6, "r4y6", CreatePattern("r4y6", 68/4)),
				new TestPatternItem(TestPattern.QuickBrownFox, "quick brown fox...123...",
					"the quick brown fox jumps over the lazy dog " + figures),
				new TestPatternItem(TestPattern.BequemePelze, "bequeme pelze...123...",
					"kaufen sie jede woche vier gute bequeme pelze xy " + figures),
				new TestPatternItem(TestPattern.PrallVomWhisky, "prall vom whisky...123...",
					"prall vom whisky flog quax den jet zu bruch " + figures),
				new TestPatternItem(TestPattern.VogelQuax, "vogel quax...123...",
					"vogel quax zwickt johnys pferd bim " + figures),
				new TestPatternItem(TestPattern.Sylvia, "sylvia wagt...123...",
					"sylvia wagt quick den jux bei pforzheim " + figures),
				new TestPatternItem(TestPattern.WackereBayer, "jeder wackere bayer",
					"jeder wackere bayer vertilgt bequem zwo pfund kalbshaxen"),
				new TestPatternItem(TestPattern.FranzJagt, "franz jagt",
					"franz jagt im komplett verwahrlosten taxi quer durch bayern"),
				new TestPatternItem(TestPattern.Sokrates, "bei jdem klugen wort",
					"bei jedem klugen wort von sokrates rief xanthippe zynisch: quatsch"),
				new TestPatternItem(TestPattern.Line, LngText(LngKeys.Pattern_Line), CreatePattern("-", 68)),
				new TestPatternItem(TestPattern.DateTime, LngText(LngKeys.Pattern_DateTime), ""),
				new TestPatternItem(TestPattern.Numbers, "1234567890", CreatePattern("1234567890", 7).Substring(0,68)),
			};

			PatternSelectCb.DataSource = PatternList;
			PatternSelectCb.DisplayMember = "Name";

			LineCountTb.Text = "1";
		}

		private void TestPatternForm_Load(object sender, EventArgs e)
		{
			Point pos = Helper.CenterForm(this, _parentWindowsPosition);
			SetBounds(pos.X, pos.Y, Bounds.Width, Bounds.Height);
		}

		private void LineCountTb_Layout(object sender, LayoutEventArgs e)
		{
		}

		private void LineCountTb_Leave(object sender, EventArgs e)
		{
			int count = 1;
			if (int.TryParse(LineCountTb.Text, out int cnt)) count = cnt;
			if (count < 1) count = 1;
			if (count > 20) count = 20;
			LineCountTb.Text = count.ToString();
		}

		private void SendBtn_Click(object sender, EventArgs e)
		{
			TestPatternItem testPattern = (TestPatternItem)PatternSelectCb.SelectedItem;
			if (testPattern == null) return;

			int count = 0;
			if (int.TryParse(LineCountTb.Text, out int cnt)) count = cnt;

			string sendStr;
			if (testPattern.Type == TestPattern.DateTime)
			{
				sendStr = $"{DateTime.Now:dd.MM.yyyy HH:mm}\r\n";
			}
			else
			{
				sendStr = testPattern.Pattern + "\r\n";
			}
			for (int i = 0; i < count; i++)
			{
				SendEvent?.Invoke(sendStr);
			}
		}

		private string CreatePattern(string pat, int repeat)
		{
			string pattern = "";
			for (int i = 0; i < repeat; i++)
			{
				pattern += pat;
			}
			return pattern;
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
		}

		private void TestPatternForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			ClosedEvent?.Invoke();
		}
	}

	public class TestPatternItem
	{
		public TestPattern Type { get; set; }

		public string Name { get; set; }

		public string Pattern { get; set; }

		public TestPatternItem(TestPattern type, string name, string pattern)
		{
			Type = type;
			Name = name;
			Pattern = pattern;
		}
	}
}
