using System;
using System.Collections.Generic;

namespace WinTlx.Languages
{
	public enum LngKeys
	{
		Invalid,

		Start_Text,
		NoFunction_ToolTip,

		MainForm_SearchText,
		MainForm_SearchText_ToolTip,
		MainForm_SearchResult,
		MainForm_SearchButton,
		MainForm_SearchButton_ToolTip,
		MainForm_Answerback,
		MainForm_Answerback_ToolTip,
		MainForm_Address,
		MainForm_Port,
		MainForm_Extension,
		MainForm_PeerType,
		MainForm_PeerTypeHelp,
		MainForm_PeerType0,
		MainForm_PeerType1,
		MainForm_PeerType2,
		MainForm_PeerType3,
		MainForm_PeerType4,
		MainForm_PeerType5,
		MainForm_PeerType6,
		MainForm_ConnectButton,
		MainForm_DisconnectButton,
		MainForm_SendWruButton,
		MainForm_SendHereisButton,
		MainForm_SendLettersButton,
		MainForm_SendFiguresButton,
		MainForm_SendReturnButton,
		MainForm_SendLinefeedButton,
		MainForm_SendBellButton,
		MainForm_SendNullButton,
		MainForm_SendNullButton_ToolTip,
		MainForm_SendThirdLevelButton,
		MainForm_SendThirdLevelButton_ToolTip,
		MainForm_LineLabel,
		MainForm_ColumnLabel,
		MainForm_ConnTimeLabel,
		MainForm_IdleTimeLabel,
		MainForm_ReceiveStatusOn,
		MainForm_ReceiveStatusOff,
		MainForm_SendBufferStatus,
		MainForm_RemoteBufferStatus,
		MainForm_LocalBufferStatus,

		MainMenu_FileMenu,
		MainMenu_SaveBufferAsText,
		MainMenu_SaveBufferAsImage,
		MainMenu_ClearBuffer,
		MainMenu_Config,
		MainMenu_Exit,
		MainMenu_FavoritesMenu,
		MainMenu_OpenFavorites,
		MainMenu_ExtrasMenu,
		MainMenu_OpenTextEditor,
		MainMenu_OpenTapePunchEditor,
		MainMenu_EyeballCharOnOff,
		MainMenu_TestPattern,
		MainMenu_OpenScheduler,
		MainMenu_ReceiveMenu,
		MainMenu_ReceiveOnOff,
		MainMenu_UpdateSubscribeServer,
		MainMenu_DebugMenu,
		MainMenu_OpenDebug,
		MainMenu_AboutMenu,
		MainMenu_About,

		Setup_Setup,
		Setup_General,
		Setup_LogfilePath,
		Setup_Language,
		Setup_Answerback,
		Setup_Answerback_Tooltip,
		Setup_IdleTimeout,
		Setup_OutputSpeed,
		Setup_RemoteBufferSize,
		Setup_CodeSet,
		Setup_DefaultProtocolOut,
		Setup_SubscribeServer,
		Setup_SubscribeServerAddress,
		Setup_SubscribeServerPort,
		Setup_IncomingConnection,
		Setup_SubscribeServerPin,
		Setup_OwnTelexNumber,
		Setup_ExtensionNumber,
		Setup_IncomingLocalPort,
		Setup_IncomingPublicPort,
		Setup_LimitedClient,
		Setup_LimitedClientActive,
		Setup_ServerDataHint,
		Setup_CancelButton,
		Setup_SaveButton,

		SendFile_SendFile,
		SendFile_LoadFile,
		SendFile_LineLength,
		SendFile_Cropping,
		SendFile_CroppingRight,
		SendFile_CroppingCenter,
		SendFile_CroppingLeft,
		SendFile_Convert,
		SendFile_SendButton,
		SendFile_CancelButton,

		TapePunch_TapePunchLong,
		TapePunch_TapePunchShort,
		TapePunch_RecvButton,
		TapePunch_RecvButton_ToolTip,
		TapePunch_SendButton,
		TapePunch_SendButton_ToolTip,
		TapePunch_StopButton,
		TapePunch_StopButton_ToolTip,
		TapePunch_StepButton,
		TapePunch_StepButton_ToolTip,
		TapePunch_ClearButton,
		TapePunch_ClearButton_ToolTip,
		TapePunch_LoadButton,
		TapePunch_LoadButton_ToolTip,
		TapePunch_SaveButton,
		TapePunch_SaveButton_ToolTip,
		TapePunch_EditButton,
		TapePunch_EditButton_ToolTip,
		TapePunch_InsertButton,
		TapePunch_InsertButton_ToolTip,
		TapePunch_DeleteButton,
		TapePunch_DeleteButton_ToolTip,
		TapePunch_CropStartButton_ToolTip,
		TapePunch_CropEndButton_ToolTip,
		TapePunch_CloseButton,
		TapePunch_CodeLetters,
		TapePunch_CodeFigures,
		TapePunch_CodeCarriageReturn,
		TapePunch_CodeLinefeed,
		TapePunch_MirrorBufferButton,
		TapePunch_MirrorBufferButton_ToolTip,
		TapePunch_MirrorCodeButton,
		TapePunch_MirrorCodeButton_ToolTip,

		Scheduler_Scheduler,
		Scheduler_AddEntry,
		Scheduler_CopyEntry,
		Scheduler_DeleteEntry,
		Scheduler_CloseButton,
		Scheduler_ActiveRow,
		Scheduler_SuccessRow,
		Scheduler_ErrorRow,
		Scheduler_DateRow,
		Scheduler_TimeRow,
		Scheduler_DestRow,
		Scheduler_FileRow,

		Message_Connected,
		Message_Disconnected,
		Message_Reject,
		Message_IdleTimeout,
		Message_IncomingConnection,
		Message_SubscribeServerError,
		Message_InvalidSubscribeServerData,
		Message_QueryResult,
		Message_ConnectNoAddress,
		Message_ConnectInvalidPort,
		Message_ConnectInvalidExtension,
		Message_ConnectionError,
		Message_Pangram,
		Message_EyeballCharActive,

		Editor_Header,
		Editor_Clear,
		Editor_Load,
		Editor_Save,
		Editor_Send,
		Editor_Close,
		Editor_ConvBaudot,
		Editor_ConvRtty,
		Editor_AlignBlock,
		Editor_AlignLeft,
		Editor_ShiftLeft,
		Editor_ShiftRight,
		Editor_LineNr,
		Editor_ColumnNr,
		Editor_CharWidth,
		Editor_NotSavedHeader,
		Editor_NotSavedMessage,
		Editor_LoadError,
		Editor_SaveError,
		Editor_Error,

		Favorites_Header,
		Favorites_FavAddButton,
		Favorites_FavDeleteButton,
		Favorites_FavDialButton,
		Favorites_HistClearButton,
		Favorites_HistDialButton,
		Favorites_FavoritesTab,
		Favorites_EntryNumber,
		Favorites_EntryName,
		Favorites_EntryAddress,
		Favorites_EntryPort,
		Favorites_EntryDirectDial,
		Favorites_CallHistoryTab,
		Favorites_EntryDate,
		Favorites_EntryResult,
		Favorites_ClearHistHeader,
		Favorites_ClearHistMessage,

		TestPattern_Header,
		TestPattern_Selection,
		TestPattern_Count,
		TestPattern_Send,
		TestPattern_Ryry,
		TestPattern_Fox,
		TestPattern_Pelze,
		TestPattern_Quax,
		TestPattern_Line,
		TestPattern_DateTime,
		TestPattern_DamperTest,
	}

	class Language
	{
		public string Key { get; set; }

		public string Version { get; set; }

		public string DisplayName { get; set; }

		public Dictionary<LngKeys, string> Items { get; set; }

		public Language()
		{
			Items = new Dictionary<LngKeys, string>();
		}

		public Language(string key, string displayName)
		{
			Key = key;
			DisplayName = displayName;
			Items = new Dictionary<LngKeys, string>();
		}

		public static LngKeys StringToLngKey(string keyStr)
		{
			if (Enum.TryParse(keyStr, true, out LngKeys lngKey))
			{
				return lngKey;
			}
			else
			{
				return LngKeys.Invalid;
			}
		}

		public override string ToString()
		{
			return $"{Key} {DisplayName} {Items?.Count}";
		}
	}
}
