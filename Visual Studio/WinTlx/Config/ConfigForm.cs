﻿using System;
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

namespace WinTlx.Config
{
	public partial class ConfigForm : Form
	{
		private const string TAG = "ConfigForm";
		//private ConfigData _config;

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

			CodeStandardCb.DataSource = new string[]
				{
					ConfigData.CodeStandardToString(CodeStandards.Ita2),
					ConfigData.CodeStandardToString(CodeStandards.UsTTy)
				};

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
			CodeStandardCb.Text = LngText(LngKeys.Setup_CodeStandard);

			SubscribeServerGb.Text = LngText(LngKeys.Setup_SubscribeServer);
			SubscribeServerAddressLbl.Text = LngText(LngKeys.Setup_SubscribeServerAddress);
			SubscribeServerPortLbl.Text = LngText(LngKeys.Setup_SubscribeServerPort);
			IncomingGb.Text = LngText(LngKeys.Setup_IncomingConnection);
			SubscribeServerUpdatePinLbl.Text = LngText(LngKeys.Setup_SubscribeServerPin);
			OwnNumberLbl.Text = LngText(LngKeys.Setup_OwnTelexNumber);
			ExtensionNumberLbl.Text = LngText(LngKeys.Setup_ExtensionNumber);
			IncomingLocalPortLbl.Text = LngText(LngKeys.Setup_IncomingLocalPort);
			IncomingPublicPortLbl.Text = LngText(LngKeys.Setup_IncomingPublicPort);
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
			CodeStandardCb.SelectedItem = ConfigData.CodeStandardToString(_config.CodeStandard);
			OutputSpeedTb.Text = IntToStr(_config.OutputSpeed);
			SubscribeServerAddressTb.Text = _config.SubscribeServerAddress;
			SubscribeServerPortTb.Text = IntToStr(_config.SubscribeServerPort);
			SubscribeServerUpdatePinTb.Text = IntToStr(_config.SubscribeServerUpdatePin);
			OwnNumberTb.Text = IntToStr(_config.OwnNumber);
			ExtensionNumberTb.Text = IntToStr(_config.IncomingExtensionNumber);
			IncommingLocalPortTb.Text = IntToStr(_config.IncomingLocalPort);
			IncomingPublicPortTb.Text = IntToStr(_config.IncomingPublicPort);
		}

		public void GetData()
		{
			_config.LogfilePath = ConfigData.FormatLogPath(LogFilePathTb.Text.Trim());
			_config.Answerback = AnswerbackTb.Text.Trim();
			_config.IdleTimeout = StrToInt(IdleTimeoutTb.Text);
			_config.CodeStandard = ConfigData.StringToCodeStandard((string)CodeStandardCb.SelectedItem);
			_config.OutputSpeed = StrToInt(OutputSpeedTb.Text);
			_config.SubscribeServerAddress = SubscribeServerAddressTb.Text.Trim();
			_config.SubscribeServerPort = StrToInt(SubscribeServerPortTb.Text);
			_config.SubscribeServerUpdatePin = StrToInt(SubscribeServerUpdatePinTb.Text);
			_config.OwnNumber = StrToInt(OwnNumberTb.Text);
			_config.IncomingExtensionNumber = StrToInt(ExtensionNumberTb.Text);
			_config.IncomingLocalPort = StrToInt(IncommingLocalPortTb.Text);
			_config.IncomingPublicPort = StrToInt(IncomingPublicPortTb.Text);
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
			int intValue;
			if (int.TryParse(valStr.Trim(), out intValue))
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
			GetData();
		}
	}
}