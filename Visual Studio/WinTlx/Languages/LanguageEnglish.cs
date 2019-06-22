using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Languages
{
	class LanguageEnglish
	{
		public static Language GetLng()
		{
			Language lng = new Language("en", "English");
			lng.Items = new Dictionary<LngKeys, string>
			{
				{ LngKeys.Start_Text, "Please note that this is a test and diagnostic tool for the i-Telex network. " +
					"The members of the network have real telex machines connected to there i-Telex ports. " +
					"Please do not send longer text files or spam to i-Telex numbers!" },

				{ LngKeys.MainForm_SearchText, "Search text" },
				{ LngKeys.MainForm_SearchResult, "Search result (select member)" },
				{ LngKeys.MainForm_SearchButton, "Search" },
				{ LngKeys.MainForm_Answerback, "Answerback" },
				{ LngKeys.MainForm_Address, "Address" },
				{ LngKeys.MainForm_Port, "Port" },
				{ LngKeys.MainForm_Extension, "Extension" },
				{ LngKeys.MainForm_Itelex, "i-Telex" },
				{ LngKeys.MainForm_ASCII, "ASCII" },
				{ LngKeys.MainForm_ConnectButton, "Connect" },
				{ LngKeys.MainForm_DisconnectButton, "Disconnect" },
				{ LngKeys.MainForm_LocalButton, "Local" },
				{ LngKeys.MainForm_SendWruButton, "WRU" },
				{ LngKeys.MainForm_SendHereisButton, "Here is" },
				{ LngKeys.MainForm_SendLettersButton, "A..." },
				{ LngKeys.MainForm_SendFiguresButton, "1..." },
				{ LngKeys.MainForm_SendReturnButton, "<" },
				{ LngKeys.MainForm_SendLinefeedButton, "\u2261" },
				{ LngKeys.MainForm_SendBellButton, "BEL" },
				{ LngKeys.MainForm_SendNullButton, "....." },
				{ LngKeys.MainForm_SendTimeButton, "Time" },
				{ LngKeys.MainForm_SendRyButton, "RY" },
				{ LngKeys.MainForm_SendFoxButton, "Fox" },
				{ LngKeys.MainForm_ClearButton, "Clear" },
				{ LngKeys.MainForm_SendfileButton, "Send file" },
				{ LngKeys.MainForm_RecvOnButton, "Recv On" },
				{ LngKeys.MainForm_UpdateIpAddressButton, "Update IP" },
				{ LngKeys.MainForm_TapePunchButton, "Tape punch" },
				{ LngKeys.MainForm_EyeBallCharsButton, "Eyeball char" },
				{ LngKeys.MainForm_ConfigButton, "Setup" },
				{ LngKeys.MainForm_AboutButton, "About" },
				{ LngKeys.MainForm_ExitButton, "Exit" },

				{ LngKeys.Setup_Setup, "Setup" },
				{ LngKeys.Setup_General, "General" },
				{ LngKeys.Setup_Language, "Language / Sprache" },
				{ LngKeys.Setup_Answerback, "Answerback" },
				{ LngKeys.Setup_InactivityTimeout, "Inactivity timeout" },
				{ LngKeys.Setup_OutputSpeed, "Output speed (Baud)" },
				{ LngKeys.Setup_CodeStandard, "Code standard" },
				{ LngKeys.Setup_SubscribeServer, "Subscribe server" },
				{ LngKeys.Setup_SubscribeServerAddress, "Subscribe server address" },
				{ LngKeys.Setup_SubscribeServerPort, "Subscribe server port" },
				{ LngKeys.Setup_IncomingConnection, "Incoming connection" },
				{ LngKeys.Setup_SubscribeServerPin, "Subscribe server pin" },
				{ LngKeys.Setup_OwnTelexNumber, "Telex number" },
				{ LngKeys.Setup_ExtensionNumber, "Extension number" },
				{ LngKeys.Setup_IncomingLocalPort, "Incoming local port" },
				{ LngKeys.Setup_IncomingPublicPort, "Incoming public port" },
				{ LngKeys.Setup_CancelButton, "Cancel" },
				{ LngKeys.Setup_SaveButton, "Save" },

				{ LngKeys.SendFile_SendFile, "Send text file" },
				{ LngKeys.SendFile_LoadFile, "Load file" },
				{ LngKeys.SendFile_LineLength, "Line length" },
				{ LngKeys.SendFile_Cropping, "Cropping" },
				{ LngKeys.SendFile_CroppingRight, "Right" },
				{ LngKeys.SendFile_CroppingCenter, "Center" },
				{ LngKeys.SendFile_CroppingLeft, "Left" },
				{ LngKeys.SendFile_Convert, "Convert" },
				{ LngKeys.SendFile_SendButton, "Send" },
				{ LngKeys.SendFile_CancelButton, "Cancel" },

				{ LngKeys.TapePunch_TapePunch, "Tape punch" },
				{ LngKeys.TapePunch_OnButton, "On" },
				{ LngKeys.TapePunch_OffButton, "Off" },
				{ LngKeys.TapePunch_ClearButton, "Clear" },
				{ LngKeys.TapePunch_CloseButton, "Close" },

				{ LngKeys.Message_Connected, "connected" },
				{ LngKeys.Message_Disconnected, "disconnected" },
				{ LngKeys.Message_Reject, "reject" },
				{ LngKeys.Message_InactivityTimeout, "activity timeout" },
				{ LngKeys.Message_IncomingConnection, "incoming connection from" },
				{ LngKeys.Message_SubscribeServerError, "subscribe server error" },
				{ LngKeys.Message_InvalidSubscribeServerData, "invalid subscribe server address or port" },
				{ LngKeys.Message_QueryResult, "member(s) found" },
				{ LngKeys.Message_ConnectNoAddress, "no address" },
				{ LngKeys.Message_ConnectInvalidPort, "invalid port" },
				{ LngKeys.Message_ConnectInvalidExtension, "invalid extension" },
				{ LngKeys.Message_ConnectionError, "connection error" },
				{ LngKeys.Message_EyeballCharActive, "eyeball char mode active - start tape punch" },
			};

			return lng;
		}
	}
}
