﻿using System.Collections.Generic;

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

				{ LngKeys.NoFunction_ToolTip, "Not implemented yet" },

				{ LngKeys.MainForm_SearchText, "Search text" },
				{ LngKeys.MainForm_SearchText_ToolTip, "Search text or i-Telex number" },
				{ LngKeys.MainForm_SearchResult, "Search result (select member)" },
				{ LngKeys.MainForm_SearchButton, "Search" },
				{ LngKeys.MainForm_SearchButton_ToolTip, "Search on subscribe server" },
				{ LngKeys.MainForm_Answerback, "Answerback" },
				{ LngKeys.MainForm_Answerback_ToolTip, "WinTlx answerback" },
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
				{ LngKeys.MainForm_SendNullButton_ToolTip, "Send Code32/NUL character" },
				{ LngKeys.MainForm_SendTimeButton, "Time" },
				{ LngKeys.MainForm_SendRyButton, "RY" },
				{ LngKeys.MainForm_SendPanButton, "Fox" },
				{ LngKeys.MainForm_ClearButton, "Clear" },
				{ LngKeys.MainForm_ClearButton_ToolTip, "Clear terminal window" },
				{ LngKeys.MainForm_TextEditorButton, "Edit text file" },
				{ LngKeys.MainForm_TextEditorButton_ToolTip, "Edit and send text file" },
				{ LngKeys.MainForm_SaveImageButton, "Save graphic" },
				{ LngKeys.MainForm_SaveImageButton_ToolTip, "Save buffer contents as graphic file (PNG)" },
				{ LngKeys.MainForm_SaveBufferButton, "Save buffer" },
				{ LngKeys.MainForm_SaveBufferButton_ToolTip, "Save buffer contents as text file" },
				{ LngKeys.MainForm_RecvOnButton, "Recv On" },
				{ LngKeys.MainForm_RecvOnButton_ToolTip, "Recveive mode on/off, needs valid subscriber server and valid local TCP setup" },
				{ LngKeys.MainForm_UpdateIpAddressButton, "Update IP" },
				{ LngKeys.MainForm_UpdateIpAddressButton_ToolTip, "Update ip address on the subscribe server" },
				{ LngKeys.MainForm_TapePunchButton, "Tape punch" },
				{ LngKeys.MainForm_TapePunchButton_ToolTip, "Opens punch tape sender/receiver/editor" },
				{ LngKeys.MainForm_EyeBallCharsButton, "Eyeball char" },
				{ LngKeys.MainForm_EyeBallCharsButton_ToolTip, "Eyevall characters on/off" },
				{ LngKeys.MainForm_ConfigButton, "Setup" },
				{ LngKeys.MainForm_AboutButton, "About" },
				{ LngKeys.MainForm_ExitButton, "Exit" },

				{ LngKeys.Setup_Setup, "Setup" },
				{ LngKeys.Setup_General, "General" },
				{ LngKeys.Setup_LogfilePath, "Logfile path" },
				{ LngKeys.Setup_Language, "Language / Sprache" },
				{ LngKeys.Setup_Answerback, "Answerback" },
				{ LngKeys.Setup_IdleTimeout, "Idle timeout (s)" },
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
				{ LngKeys.Setup_LimitedClientActive, "Limited client active" },
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

				{ LngKeys.TapePunch_TapePunchLong, "Tape punch sender/receiver/editor" },
				{ LngKeys.TapePunch_TapePunchShort, "Tape punch editor" },
				{ LngKeys.TapePunch_RecvButton, "Recv" },
				{ LngKeys.TapePunch_RecvButton_ToolTip, "Tape punch receiver on/off" },
				{ LngKeys.TapePunch_SendButton, "Send" },
				{ LngKeys.TapePunch_SendButton_ToolTip, "Send tape" },

				{ LngKeys.TapePunch_StopButton, "Stop" },
				{ LngKeys.TapePunch_StopButton_ToolTip, "Stop sending" },

				{ LngKeys.TapePunch_StepButton, "Step" },
				{ LngKeys.TapePunch_StepButton_ToolTip, "Send single step" },

				{ LngKeys.TapePunch_ClearButton, "Clear" },
				{ LngKeys.TapePunch_ClearButton_ToolTip, "Clear tape" },
				{ LngKeys.TapePunch_LoadButton, "Load" },
				{ LngKeys.TapePunch_LoadButton_ToolTip, "Load tape from file" },
				{ LngKeys.TapePunch_SaveButton, "Save" },
				{ LngKeys.TapePunch_SaveButton_ToolTip, "Save tape to file" },
				{ LngKeys.TapePunch_EditButton, "Edit" },
				{ LngKeys.TapePunch_EditButton_ToolTip, "Tape editor on/off" },
				{ LngKeys.TapePunch_InsertButton, "Ins" },
				{ LngKeys.TapePunch_InsertButton_ToolTip, "Insert mode on/off" },
				{ LngKeys.TapePunch_DeleteButton, "Del" },
				{ LngKeys.TapePunch_DeleteButton_ToolTip, "Delete character" },
				{ LngKeys.TapePunch_CropStartButton_ToolTip, "Delete all character up to the current position" },
				{ LngKeys.TapePunch_CropEndButton_ToolTip, "Delete all characters from the current position" },
				{ LngKeys.TapePunch_CloseButton, "Close" },
				{ LngKeys.TapePunch_CodeLetters, "LTR" },
				{ LngKeys.TapePunch_CodeFigures, "FIG" },
				{ LngKeys.TapePunch_CodeCarriageReturn, "<" },
				{ LngKeys.TapePunch_CodeLinefeed, "\u2261" },
				{ LngKeys.TapePunch_ReverseButton, "Reverse" },
				{ LngKeys.TapePunch_ReverseButton_ToolTip, "Reverse bit code" },

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

				{ LngKeys.Editor_Header, "Text Editor" },
				{ LngKeys.Editor_Clear, "Clear" },
				{ LngKeys.Editor_Load, "Load" },
				{ LngKeys.Editor_Save, "Save" },
				{ LngKeys.Editor_Send, "Send" },
				{ LngKeys.Editor_Close, "Close" },
				{ LngKeys.Editor_ConvBaudot, "Baudot" },
				{ LngKeys.Editor_ConvRtty, "RttyArt" },
				{ LngKeys.Editor_AlignBlock, "Block" },
				{ LngKeys.Editor_AlignLeft, "Left" },
				{ LngKeys.Editor_ShiftLeft, "<" },
				{ LngKeys.Editor_ShiftRight, ">" },
				{ LngKeys.Editor_LineNr, "Ln" },
				{ LngKeys.Editor_ColumnNr, "Co" },
				{ LngKeys.Editor_CharWidth, "Width:" },
				{ LngKeys.Editor_NotSavedHeader, "Save" },
				{ LngKeys.Editor_NotSavedMessage, "The editor text was not saved. Save now?" },
				{ LngKeys.Editor_LoadError, "Load error" },
				{ LngKeys.Editor_SaveError, "Save error" },
				{ LngKeys.Editor_Error, "Error" },
			};

			return lng;
		}
	}
}
