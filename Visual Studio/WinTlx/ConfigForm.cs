using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinTlx
{
	public partial class ConfigForm : Form
	{
		private ConfigData _config;

		private Rectangle _parentWindowsPosition;

		public bool Canceled { get; set; }

		public ConfigForm(Rectangle position)
		{
			InitializeComponent();

			_parentWindowsPosition = position;
			Canceled = false;
		}

		private void ConfigForm_Load(object sender, EventArgs e)
		{
			Point pos = Helper.CenterForm(this, _parentWindowsPosition);
			SetBounds(pos.X, pos.Y, Bounds.Width, Bounds.Height);
		}

		public void SetData(ConfigData configData)
		{
			_config = configData;
			KennungTb.Text = _config.Kennung;
			InactivityTimeoutTb.Text = IntToStr(_config.InactivityTimeout);
			OutputSpeedTb.Text = IntToStr(_config.OutputSpeed);
			SubscribeServerAddressTb.Text = _config.SubscribeServerAddress;
			SubscribeServerPortTb.Text = IntToStr(_config.SubscribeServerPort);
			SubscribeServerUpdatePinTb.Text = IntToStr(_config.SubscribeServerUpdatePin);
			OwnNumberTb.Text = IntToStr(_config.OwnNumber);
			IncommingLocalPortTb.Text = IntToStr(_config.IncomingLocalPort);
			IncomingPublicPortTb.Text = IntToStr(_config.IncomingPublicPort);
		}

		public ConfigData GetData()
		{
			_config.Kennung = KennungTb.Text.Trim();
			_config.InactivityTimeout = StrToInt(InactivityTimeoutTb.Text);
			_config.OutputSpeed = StrToInt(OutputSpeedTb.Text);
			_config.SubscribeServerAddress = SubscribeServerAddressTb.Text.Trim();
			_config.SubscribeServerPort = StrToInt(SubscribeServerPortTb.Text);
			_config.SubscribeServerUpdatePin = StrToInt(SubscribeServerUpdatePinTb.Text);
			_config.OwnNumber = StrToInt(OwnNumberTb.Text);
			_config.IncomingLocalPort = StrToInt(IncommingLocalPortTb.Text);
			_config.IncomingPublicPort = StrToInt(IncomingPublicPortTb.Text);
			_config.SetDefaults();
			return _config;
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

	}
}
