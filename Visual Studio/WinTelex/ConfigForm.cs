using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinTelex
{
	public partial class ConfigForm : Form
	{
		private ConfigData _config;

		public bool Canceled { get; set; }

		public ConfigForm()
		{
			InitializeComponent();

			Canceled = false;
		}

		public void SetData(ConfigData configData)
		{
			_config = configData;
			KennungTb.Text = _config.Kennung;
			IncommingPortTb.Text = IntToStr(_config.IncomingPort);
			SubscribeServerAddressTb.Text = _config.SubscribeServerAddress;
			SubscribeServerPortTb.Text = IntToStr(_config.SubscribeServerPort);
		}

		public ConfigData GetData()
		{
			_config.Kennung = KennungTb.Text.Trim();
			_config.IncomingPort = StrToInt(IncommingPortTb.Text);
			_config.SubscribeServerAddress = SubscribeServerAddressTb.Text.Trim();
			_config.SubscribeServerPort = StrToInt(SubscribeServerPortTb.Text);
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
			if (int.TryParse(valStr, out intValue))
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
