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
				{ LngKeys.MainForm_PeerType,
					"0 - deleted\r\n" +
					"1 - texting baudot / hostname\r\n" +
					"2 - texting baudot / fixed ip\r\n" +
					"3 - ascii texting / hostname\r\n" +
					"4 - ascii texting / fixed ip\r\n" +
					"5 - texting baudot / dyn. ip\r\n" +
					"6 - email address\r\n"
				},
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
				{ LngKeys.MainForm_SendNullButton, "Cod32" },
				{ LngKeys.MainForm_SendTimeButton, "Time" },
				{ LngKeys.MainForm_SendRyButton, "RY" },
				{ LngKeys.MainForm_SendPanButton, "Fox" },
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
				{ LngKeys.Setup_LogfilePath, "Logfile path" },
				{ LngKeys.Setup_Language, "Language / Sprache" },
				{ LngKeys.Setup_Answerback, "Answerback" },
				{ LngKeys.Setup_IdleTimeout, "Idle timeout" },
				{ LngKeys.Setup_OutputSpeed, "Output speed (Baud)" },
				{ LngKeys.Setup_CodeSet, "Code set" },
				{ LngKeys.Setup_SubscribeServer, "Subscribe server" },
				{ LngKeys.Setup_SubscribeServerAddress, "Subscribe server address" },
				{ LngKeys.Setup_SubscribeServerPort, "Subscribe server port" },
				{ LngKeys.Setup_IncomingConnection, "Incoming connection" },
				{ LngKeys.Setup_SubscribeServerPin, "Subscribe server pin*" },
				{ LngKeys.Setup_OwnTelexNumber, "Telex number*" },
				{ LngKeys.Setup_ExtensionNumber, "Extension number*" },
				{ LngKeys.Setup_LimitedClient, "Limited client" },
				{ LngKeys.Setup_IncomingLocalPort, "Incoming local port" },
				{ LngKeys.Setup_IncomingPublicPort, "Incoming public port" },
				{ LngKeys.Setup_ServerDataHint, "* This data must match the data stored on the subscribe server." },
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
				{ LngKeys.TapePunch_RecvButton, "Recv" },
				{ LngKeys.TapePunch_SendButton, "Send" },
				{ LngKeys.TapePunch_ClearButton, "Clear" },
				{ LngKeys.TapePunch_LoadButton, "Load" },
				{ LngKeys.TapePunch_SaveButton, "Save" },
				{ LngKeys.TapePunch_CloseButton, "Close" },
				{ LngKeys.TapePunch_CodeLetters, "LTR" },
				{ LngKeys.TapePunch_CodeFigures, "FIG" },
				{ LngKeys.TapePunch_CodeCarriageReturn, "<" },
				{ LngKeys.TapePunch_CodeLinefeed, "\u2261" },

				{ LngKeys.Scheduler_Scheduler, "Scheduler" },
				{ LngKeys.Scheduler_AddEntry, "Add entry" },
				{ LngKeys.Scheduler_CopyEntry, "Copy entry" },
				{ LngKeys.Scheduler_DeleteEntry, "Delete entry" },
				{ LngKeys.Scheduler_CloseButton, "Close" },
				{ LngKeys.Scheduler_ActiveRow, "Active" },
				{ LngKeys.Scheduler_SuccessRow, "Success" },
				{ LngKeys.Scheduler_ErrorRow, "Error" },
				{ LngKeys.Scheduler_DateRow, "Date" },
				{ LngKeys.Scheduler_TimeRow, "Time" },
				{ LngKeys.Scheduler_DestRow, "Number or Host;Port;Extension" },
				{ LngKeys.Scheduler_FileRow, "Text file" },

				{ LngKeys.Message_Connected, "connected" },
				{ LngKeys.Message_Disconnected, "disconnected" },
				{ LngKeys.Message_Reject, "reject" },
				{ LngKeys.Message_IdleTimeout, "idle timeout" },
				{ LngKeys.Message_IncomingConnection, "incoming connection from" },
				{ LngKeys.Message_SubscribeServerError, "subscribe server error" },
				{ LngKeys.Message_InvalidSubscribeServerData, "invalid subscribe server address or port" },
				{ LngKeys.Message_QueryResult, "entries found" },
				{ LngKeys.Message_ConnectNoAddress, "no address" },
				{ LngKeys.Message_ConnectInvalidPort, "invalid port" },
				{ LngKeys.Message_ConnectInvalidExtension, "invalid extension" },
				{ LngKeys.Message_ConnectionError, "connection error" },
				{ LngKeys.Message_Pangram, "the quick brown fox jumps over the lazy dog. 1234567890/:,-=()?" },
				{ LngKeys.Message_EyeballCharActive, "eyeball char mode active - start tape punch" },
			};

			return lng;
		}
	}
}
