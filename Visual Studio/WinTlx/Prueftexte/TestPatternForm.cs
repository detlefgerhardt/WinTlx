using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinTlx.Config;
using WinTlx.Languages;
using WinTlx.TextEditor;

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
		Cyrillic,
		Line,
		DateTime,
		Numbers,
		LoremIpsum,
		LoremIpsumBlock,
		DaempferTest,
		DoloremIpsum
	}

	public partial class TestPatternForm : Form
	{
		private const string TAG = nameof(TestPatternForm);

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

			LanguageManager.Instance.LanguageChanged += LanguageChanged;
			LanguageManager.Instance.ChangeLanguage(ConfigManager.Instance.Config.Language);

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
			ShowTestPattern();
		}

		private void SendBtn_Click(object sender, EventArgs e)
		{
			string pattern = GetTestPattern();
			SendEvent?.Invoke(pattern);
		}

		private void LanguageChanged()
		{
			Logging.Instance.Log(LogTypes.Info, TAG, nameof(LanguageChanged), $"switch language to {LanguageManager.Instance.CurrentLanguage.Key}");

			this.Text = LngText(LngKeys.TestPattern_Header);
			SelectionLbl.Text = LngText(LngKeys.TestPattern_Selection);
			CountLbl.Text = LngText(LngKeys.TestPattern_Count);
			SendBtn.Text = LngText(LngKeys.TestPattern_Send);
			SetPatternList();
		}

		private void SetPatternList()
		{
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
				new TestPatternItem(TestPattern.Cyrillic, "Cyrillic",
					"Съешь ещё этих мягких французских булок, да выпей же чаю".ToUpper()),
				new TestPatternItem(TestPattern.Line, LngText(LngKeys.TestPattern_Line), CreatePattern("-", 68)),
				new TestPatternItem(TestPattern.DateTime, LngText(LngKeys.TestPattern_DateTime), ""),
				new TestPatternItem(TestPattern.Numbers, "1234567890", CreatePattern("1234567890", 7).Substring(0,68)),
				new TestPatternItem(TestPattern.LoremIpsum, "Lorem ipsum", CreateLoremIpsum()),
				new TestPatternItem(TestPattern.LoremIpsumBlock, "Lorem ipsum Block", CreateLoremIpsumBlock()),
				new TestPatternItem(TestPattern.DaempferTest, LngText(LngKeys.TestPattern_DamperTest), CreateDaempferTest()),
			};

			PatternSelectCb.DataSource = PatternList;
			PatternSelectCb.DisplayMember = "Name";
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
		}

		private void TestPatternForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			ClosedEvent?.Invoke();
		}

		private void PatternSelectCb_SelectedIndexChanged(object sender, EventArgs e)
		{
			ShowTestPattern();
		}

		private void ShowTestPattern()
		{
			string pattern = GetTestPattern();
			PreViewTb.Text = pattern;
		}

		private string GetTestPattern()
		{
			TestPatternItem testPattern = (TestPatternItem)PatternSelectCb.SelectedItem;
			if (testPattern == null) return null;

			int count = 1;
			if (int.TryParse(LineCountTb.Text, out int cnt)) count = cnt;

			string line;
			if (testPattern.Type == TestPattern.DateTime)
			{
				line = $"{DateTime.Now:dd.MM.yyyy HH:mm}\r\n";
			}
			else
			{
				line = testPattern.Pattern + "\r\n";
			}

			string lines = "";
			for (int i = 0; i < count; i++)
			{
				lines += line;
			}

			return lines;
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

		private string CreateDaempferTest()
		{
			int[] tabs = new int[] { 10, 20, 30, 40, 50, 60, 68 };
			string pattern = "\r\n\r\ndaempfertest wr + nl:\r\n";
			for (int i = 0; i < tabs.Length; i++)
			{
				string line = "\r+" + new string(' ', tabs[i] - 2) + "+\r\n";
				pattern += line;
				pattern += "+";
				if (i < tabs.Length - 1) pattern += "\r\n";
			}

			pattern += "\r\n\r\ndaempfertest nur wr:\r\n";
			for (int i = 0; i < tabs.Length; i++)
			{
				string line = "\r+" + new string(' ', tabs[i] - 2) + "+\r+";
				pattern += line;
				if (i < tabs.Length - 1) pattern += "\r\n";
			}

			return pattern;
		}

		private string CreateLoremIpsum()
		{
			List<string> lines = TextEditorManager.Instance.WrapWords(LOREM_IPSUM, 68);
			string pattern = "";
			for (int i = 0; i < lines.Count; i++)
			{
				pattern += lines[i];
				if (i < lines.Count - 1) pattern += "\r\n";
			}
			return pattern;
		}

		private string CreateLoremIpsumBlock()
		{
			List<string> lines = TextEditorManager.Instance.WrapWords(LOREM_IPSUM, 64);
			string pattern = "";
			for (int i = 0; i < lines.Count; i++)
			{
				string block = TextEditorManager.Instance.BlockSatz(lines[i], 64, "", i != lines.Count - 1);
				block = "+ " + block.PadRight(64) + " +";
				pattern += block;
				if (i<lines.Count-1) pattern += "\r\n";
			}
			return pattern;
		}

		private const string LOREM_IPSUM = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.";
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
