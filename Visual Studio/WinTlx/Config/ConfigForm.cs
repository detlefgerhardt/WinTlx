using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinTlx.Codes;
using WinTlx.Config;
using WinTlx.Languages;

namespace WinTlx.Config
{
	public partial class ConfigForm : Form
	{
		private const string TAG = "ConfigForm";

		private Rectangle _parentWindowsPosition;

		public bool Canceled { get; set; }

		private ConfigData _config => ConfigManager.Instance.Config;

		public ConfigForm(Rectangle position)
		{
			InitializeComponent();

			_parentWindowsPosition = position;
			Canceled = false;

			LanguageManager.Instance.LanguageChanged += LanguageChanged;
			LanguageChanged();

			var enums = Enum.GetValues(typeof(CodeSets));
			string[] codeSets = new string[CodeItem.CODESETS_COUNT];
			for (int i=0; i<CodeItem.CODESETS_COUNT; i++)
			{
				codeSets[i] = ConfigData.CodeSetToString((CodeSets)enums.GetValue(i));
			}
			CodeSetCb.DataSource = codeSets;

			LanguageCb.DataSource = LanguageManager.Instance.LanguageList;
			LanguageCb.DisplayMember = "Key";
			LanguageCb.SelectedItem = LanguageManager.Instance.CurrentLanguage;

			SetData();
		}

		private void LanguageChanged()
		{
			Logging.Instance.Log(LogTypes.Info, TAG, nameof(LanguageChanged), $"switch language to {LanguageManager.Instance.CurrentLanguage.Key}");

			this.Text = $"{Constants.PROGRAM_NAME} {LngText(LngKeys.Setup_Setup)}";
			GeneralGb.Text = LngText(LngKeys.Setup_General);
			LogfilePathLbl.Text = LngText(LngKeys.Setup_LogfilePath);
			LanguageCb.Text = LngText(LngKeys.Setup_Language);
			AnswerbackLbl.Text = LngText(LngKeys.Setup_Answerback);
			IdleTimeoutLbl.Text = LngText(LngKeys.Setup_IdleTimeout);
			OutputSpeedLbl.Text = LngText(LngKeys.Setup_OutputSpeed);
			CodeSetLbl.Text = LngText(LngKeys.Setup_CodeSet);

			SubscribeServerGb.Text = LngText(LngKeys.Setup_SubscribeServer);
			SubscribeServerAddress1Lbl.Text = LngText(LngKeys.Setup_SubscribeServerAddress) + " 1";
			SubscribeServerAddress2Lbl.Text = LngText(LngKeys.Setup_SubscribeServerAddress) + " 2";
			SubscribeServerAddress3Lbl.Text = LngText(LngKeys.Setup_SubscribeServerAddress) + " 3";
			SubscribeServerPortLbl.Text = LngText(LngKeys.Setup_SubscribeServerPort);
			IncomingGb.Text = LngText(LngKeys.Setup_IncomingConnection);
			SubscribeServerUpdatePinLbl.Text = LngText(LngKeys.Setup_SubscribeServerPin);
			OwnNumberLbl.Text = LngText(LngKeys.Setup_OwnTelexNumber);
			ExtensionNumberLbl.Text = LngText(LngKeys.Setup_ExtensionNumber);
			IncomingLocalPortLbl.Text = LngText(LngKeys.Setup_IncomingLocalPort);
			IncomingPublicPortLbl.Text = LngText(LngKeys.Setup_IncomingPublicPort);
			LimitedClientGb.Text = LngText(LngKeys.Setup_LimitedClient);
			LimitedClientLbl.Text = LngText(LngKeys.Setup_LimitedClientActive);
			//ServerDataHintLbl.Text = LngText(LngKeys.Setup_ServerDataHint);

			CancelBtn.Text = LngText(LngKeys.Setup_CancelButton);
			SaveBtn.Text = LngText(LngKeys.Setup_SaveButton);
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
		}

		private void ConfigForm_Load(object sender, EventArgs e)
		{
			Point pos = Helper.CenterForm(this, _parentWindowsPosition);
			SetBounds(pos.X, pos.Y, Bounds.Width, Bounds.Height);
		}

		public void SetData()
		{
			LanguageCb.SelectedItem = _config.Language;
			LogFilePathTb.Text = _config.LogfilePath;
			AnswerbackTb.Text = _config.Answerback;
			IdleTimeoutTb.Text = IntToStr(_config.IdleTimeout);
			CodeSetCb.SelectedItem = ConfigData.CodeSetToString(_config.CodeSet);
			OutputSpeedTb.Text = IntToStr(_config.OutputSpeed);
			SubscribeServerAddress1Tb.Text = _config.SubscribeServerAddress;
			SubscribeServerAddress2Tb.Text = _config.SubscribeServerAddress2;
			SubscribeServerAddress3Tb.Text = _config.SubscribeServerAddress3;
			SubscribeServerPortTb.Text = IntToStr(_config.SubscribeServerPort);
			SubscribeServerUpdatePinTb.Text = IntToStr(_config.SubscribeServerUpdatePin);
			OwnNumberTb.Text = IntToStr(_config.OwnNumber);
			ExtensionNumberTb.Text = _config.IncomingExtensionNumber.ToString();
			IncommingLocalPortTb.Text = IntToStr(_config.IncomingLocalPort);
			IncomingPublicPortTb.Text = IntToStr(_config.IncomingPublicPort);
			LimitedClientCb.Checked = _config.LimitedClient;
			RemoteServerAddressTb.Text = _config.RemoteServerAddress;
			RemoteServerPortTb.Text = IntToStr(_config.RemoteServerPort);
		}

		public void GetData()
		{
			_config.LogfilePath = ConfigData.FormatLogPath(LogFilePathTb.Text.Trim());
			_config.Answerback = AnswerbackTb.Text.Trim().ToLower();
			_config.IdleTimeout = StrToInt(IdleTimeoutTb.Text);
			_config.CodeSet = ConfigData.StringToCodeSet((string)CodeSetCb.SelectedItem);
			_config.OutputSpeed = StrToInt(OutputSpeedTb.Text);
			_config.SubscribeServerAddress = SubscribeServerAddress1Tb.Text.Trim();
			_config.SubscribeServerAddress2 = SubscribeServerAddress2Tb.Text.Trim();
			_config.SubscribeServerAddress3 = SubscribeServerAddress3Tb.Text.Trim();
			_config.SubscribeServerPort = StrToInt(SubscribeServerPortTb.Text);
			_config.SubscribeServerUpdatePin = StrToInt(SubscribeServerUpdatePinTb.Text);
			_config.OwnNumber = StrToInt(OwnNumberTb.Text);
			_config.IncomingExtensionNumber = StrToInt(ExtensionNumberTb.Text);
			_config.IncomingLocalPort = StrToInt(IncommingLocalPortTb.Text);
			_config.IncomingPublicPort = StrToInt(IncomingPublicPortTb.Text);
			_config.LimitedClient = LimitedClientCb.Checked;
			_config.RemoteServerAddress = RemoteServerAddressTb.Text.Trim();
			_config.RemoteServerPort = StrToInt(RemoteServerPortTb.Text);
			_config.SetDefaults();

			string oldLnd = _config.Language;
			Language newLng = (Language)LanguageCb.SelectedItem;
			_config.Language = newLng.Key;
			if (_config.Language!=oldLnd)
			{
				LanguageManager.Instance.ChangeLanguage(_config.Language);
			}
		}

		private string IntToStr(int intValue)
		{
			return intValue == 0 ? "" : intValue.ToString();
		}

		private int StrToInt(string valStr)
		{
			if (int.TryParse(valStr.Trim(), out int intValue))
			{
				return intValue;
			}
			else
			{
				return 0;
			}
		}

		private void SaveBtn_Click(object sender, EventArgs e)
		{
			Close();
			Canceled = false;
		}

		private void CancelBtn_Click(object sender, EventArgs e)
		{
			Close();
			Canceled = true;
		}

		private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!Canceled)
			{
				GetData();
			}
		}
	}
}
