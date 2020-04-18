using System.Collections.Generic;

namespace WinTlx.Languages
{
	class LanguageDeutsch
	{
		public static Language GetLng()
		{
			Language lng = new Language("de", "Deutsch");
			lng.Items = new Dictionary<LngKeys, string>
			{
				{ LngKeys.Start_Text, "Bitte beachte, dass dies ein Test- und Diagnose-Tool für das i-Telex-Netzwerk ist. " +
					"Die Teilnehmer habe reale Fernschreiber angeschlossen. " +
					"Sende bitte keine längeren Texte oder Spam an i-Telex-Nummern!" },

				{ LngKeys.NoFunction_ToolTip, "Noch nicht implementiert" },

				{ LngKeys.MainForm_SearchText, "Suchtext" },
				{ LngKeys.MainForm_SearchText_ToolTip, "Suchtext oder i-Telex-Nummer" },
				{ LngKeys.MainForm_SearchResult, "Suchergebnisse (auswählen)" },
				{ LngKeys.MainForm_SearchButton, "Suchen" },
				{ LngKeys.MainForm_SearchButton_ToolTip, "Auf Teilnehmer-Server suchen" },
				{ LngKeys.MainForm_Answerback, "Kennung" },
				{ LngKeys.MainForm_Answerback_ToolTip, "WinTlx Kennung" },
				{ LngKeys.MainForm_Address, "Adresse" },
				{ LngKeys.MainForm_Port, "Port" },
				{ LngKeys.MainForm_Extension, "Extension" },
				{ LngKeys.MainForm_PeerType,
					"0 - gelöscht\r\n" + 
					"1 - texting baudot / Hostname\r\n" +
					"2 - texting baudot / feste IP\r\n" +
					"3 - ascii texting / Hostname\r\n" +
					"4 - ascii texting / feste IP\r\n" +
					"5 - texting baudot / dyn. IP\r\n" +
					"6 - Emailadresse"
				},
				{ LngKeys.MainForm_Itelex, "i-Telex" },
				{ LngKeys.MainForm_ASCII, "ASCII" },
				{ LngKeys.MainForm_ConnectButton, "Verbinden" },
				{ LngKeys.MainForm_DisconnectButton, "Trennen" },
				{ LngKeys.MainForm_LocalButton, "Lokal" },
				{ LngKeys.MainForm_SendWruButton, "WRU" },
				{ LngKeys.MainForm_SendHereisButton, "Hier ist" },
				{ LngKeys.MainForm_SendLettersButton, "Bu" },
				{ LngKeys.MainForm_SendFiguresButton, "Zi" },
				{ LngKeys.MainForm_SendReturnButton, "<" },
				{ LngKeys.MainForm_SendLinefeedButton, "\u2261" },
				{ LngKeys.MainForm_SendBellButton, "Klingel" },
				{ LngKeys.MainForm_SendNullButton, "Cod32" },
				{ LngKeys.MainForm_SendNullButton_ToolTip, "Code32/NULL Zeichen senden" },
				{ LngKeys.MainForm_SendTimeButton, "Zeit" },
				{ LngKeys.MainForm_SendRyButton, "RY" },
				{ LngKeys.MainForm_SendPanButton, "Quax" },
				{ LngKeys.MainForm_ClearButton, "Löschen" },
				{ LngKeys.MainForm_ClearButton_ToolTip, "Terminal-Fenster löschen" },
				{ LngKeys.MainForm_SendfileButton, "Datei senden" },
				{ LngKeys.MainForm_SendfileButton_ToolTip, "Textdatei senden" },
				{ LngKeys.MainForm_RecvOnButton, "Empfang" },
				{ LngKeys.MainForm_RecvOnButton_ToolTip, "Empfangs-Modus ein/aus, erfordert gültigen Tln-Server-Eintrag und korrekte TCP-Einstellungen" },
				{ LngKeys.MainForm_UpdateIpAddressButton, "Update IP" },
				{ LngKeys.MainForm_UpdateIpAddressButton_ToolTip, "IP-Addresse auf dem Teilnehmerserver aktualiseren" },
				{ LngKeys.MainForm_TapePunchButton, "Lochstreifen" },
				{ LngKeys.MainForm_TapePunchButton_ToolTip, "Öffnet Lochstreifen Sender/Empfänger/Editor" },
				{ LngKeys.MainForm_EyeBallCharsButton, "Bildlocher" },
				{ LngKeys.MainForm_EyeBallCharsButton_ToolTip, "Bildlocher ein/aus" },
				{ LngKeys.MainForm_ConfigButton, "Einstellungen" },
				{ LngKeys.MainForm_AboutButton, "Info" },
				{ LngKeys.MainForm_ExitButton, "Beenden" },

				{ LngKeys.Setup_Setup, "Einstellungen" },
				{ LngKeys.Setup_General, "Allgemein" },
				{ LngKeys.Setup_Language, "Sprache / Language" },
				{ LngKeys.Setup_LogfilePath, "Logfile-Pfad" },
				{ LngKeys.Setup_Answerback, "Kennungsgeber" },
				{ LngKeys.Setup_IdleTimeout, "Inaktivitäts-Timeout" },
				{ LngKeys.Setup_OutputSpeed, "Ausgabegeschw. (Baud)" },
				{ LngKeys.Setup_CodeSet, "Zeichensatz" },
				{ LngKeys.Setup_SubscribeServer, "Teilnehmer-Server" },
				{ LngKeys.Setup_SubscribeServerAddress, "Tln-Server-Adresse" },
				{ LngKeys.Setup_SubscribeServerPort, "Tln-Server-Port" },
				{ LngKeys.Setup_IncomingConnection, "Eingehende Verbindungen" },
				{ LngKeys.Setup_SubscribeServerPin, "Teilnehmer-Server-Pin*" },
				{ LngKeys.Setup_OwnTelexNumber, "Telex-Nummer*" },
				{ LngKeys.Setup_ExtensionNumber, "Extension-Nummer*" },
				{ LngKeys.Setup_LimitedClient, "Limited client" },
				{ LngKeys.Setup_IncomingLocalPort, "Lokaler Port" },
				{ LngKeys.Setup_IncomingPublicPort, "Öffentlicher Port" },
				{ LngKeys.Setup_ServerDataHint, "* Diese Daten müssen mit den im Teilnehmer-Server gespeicherten Daten übereinstimmen." },
				{ LngKeys.Setup_CancelButton, "Abbruch" },
				{ LngKeys.Setup_SaveButton, "Speichern" },

				{ LngKeys.SendFile_SendFile, "Textdatei senden" },
				{ LngKeys.SendFile_LoadFile, "Datei laden" },
				{ LngKeys.SendFile_LineLength, "Zeilenlänge" },
				{ LngKeys.SendFile_Cropping, "Begrenzung" },
				{ LngKeys.SendFile_CroppingRight, "rechts" },
				{ LngKeys.SendFile_CroppingCenter, "zentriert" },
				{ LngKeys.SendFile_CroppingLeft, "links" },
				{ LngKeys.SendFile_Convert, "Konvertieren" },
				{ LngKeys.SendFile_SendButton, "Senden" },
				{ LngKeys.SendFile_CancelButton, "Abbrechen" },

				{ LngKeys.TapePunch_TapePunch, "Lochstreifen Sender/Empfänger/Editor" },
				{ LngKeys.TapePunch_RecvButton, "Empf" },
				{ LngKeys.TapePunch_RecvButton_ToolTip, "LS-Empfänger ein/aus" },
				{ LngKeys.TapePunch_SendButton, "Senden" },
				{ LngKeys.TapePunch_SendButton_ToolTip, "LS senden" },
				{ LngKeys.TapePunch_ClearButton, "Löschen" },
				{ LngKeys.TapePunch_ClearButton_ToolTip, "LS löschen" },
				{ LngKeys.TapePunch_LoadButton, "Laden" },
				{ LngKeys.TapePunch_LoadButton_ToolTip, "LS aus Datei laden" },
				{ LngKeys.TapePunch_SaveButton, "Speichern" },
				{ LngKeys.TapePunch_SaveButton_ToolTip, "LS in Datei speichern" },
				{ LngKeys.TapePunch_EditButton, "Edit" },
				{ LngKeys.TapePunch_EditButton_ToolTip, "LS-Editor ein/aus" },
				{ LngKeys.TapePunch_InsertButton, "Einf" },
				{ LngKeys.TapePunch_InsertButton_ToolTip, "Einfügemodus ein/aus" },
				{ LngKeys.TapePunch_DeleteButton, "Lösch" },
				{ LngKeys.TapePunch_DeleteButton_ToolTip, "Zeichen löschen" },
				{ LngKeys.TapePunch_CloseButton, "Schließen" },
				{ LngKeys.TapePunch_CodeLetters, "BU" },
				{ LngKeys.TapePunch_CodeFigures, "ZI" },
				{ LngKeys.TapePunch_CodeCarriageReturn, "<" },
				{ LngKeys.TapePunch_CodeLinefeed, "\u2261" },

				{ LngKeys.Scheduler_Scheduler, "Zeitplaner" },
				{ LngKeys.Scheduler_AddEntry, "Neuer Eintrag" },
				{ LngKeys.Scheduler_CopyEntry, "Eintrag kopieren" },
				{ LngKeys.Scheduler_DeleteEntry, "Eintrag löschen" },
				{ LngKeys.Scheduler_CloseButton, "Schliessen" },
				{ LngKeys.Scheduler_ActiveRow, "Aktiv" },
				{ LngKeys.Scheduler_SuccessRow, "Erfolg" },
				{ LngKeys.Scheduler_ErrorRow, "Fehler" },
				{ LngKeys.Scheduler_DateRow, "Datum" },
				{ LngKeys.Scheduler_TimeRow, "Zeit" },
				{ LngKeys.Scheduler_DestRow, "Nummer oder Host;Port;Extension" },
				{ LngKeys.Scheduler_FileRow, "Textdatei" },

				{ LngKeys.Message_Connected, "verbunden" },
				{ LngKeys.Message_Disconnected, "getrennt" },
				{ LngKeys.Message_Reject, "reject" },
				{ LngKeys.Message_IdleTimeout, "inaktivitaetstimeout" },
				{ LngKeys.Message_IncomingConnection, "eingehende verbindung" },
				{ LngKeys.Message_SubscribeServerError, "tln.-server-fehler" },
				{ LngKeys.Message_InvalidSubscribeServerData, "ungueltige tln.-server adresse oder port" },
				{ LngKeys.Message_QueryResult, "eintraege gefunden" },
				{ LngKeys.Message_ConnectNoAddress, "adresse fehlt" },
				{ LngKeys.Message_ConnectInvalidPort, "ungueltiger port" },
				{ LngKeys.Message_ConnectInvalidExtension, "ungueltige extension-nummer" },
				{ LngKeys.Message_ConnectionError, "verbindungsfehler" },
				{ LngKeys.Message_Pangram, "prall vom whisky flog quax den jet zu bruch. 1234567890/(:-),=?" },
				//{ LngKeys.Message_Pangram, "kaufen sie jede woche vier gute bequeme pelze xy 1234567890/(:-),=?" },
				{ LngKeys.Message_EyeballCharActive, "bildlocher aktiv - starte lochstreifenstanzer" },
			};

			return lng;
		}
	}
}
