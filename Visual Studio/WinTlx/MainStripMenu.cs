using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinTlx.Languages;
using static System.Windows.Forms.Control;

namespace WinTlx
{
	class MainStripMenu
	{
		public enum MenuTypes
		{
			// file
			SaveBufferAsText,
			SaveBufferAsImage,
			ClearBuffer,
			Config,
			Exit,
			// favorites
			OpenFavorites,
			// extra
			OpenTextEditor,
			OpenTapePunchEditor,
			EyeballCharOnOff,
			TestPattern,
			OpenScheduler,
			// receiver
			ReceiveOnOff,
			UpdateSubscribeServer,
			// debug
			OpenDebugForm,
			// help
			About,
			// none
			None,
		}

		private List<ToolStripMenuItem> _menuItemList;

		private Action<MenuTypes> _menuHandler;

		public MenuStrip GetMenu(Action<MenuTypes> menuHandler)
		{
			_menuHandler = menuHandler;

			_menuItemList = new List<ToolStripMenuItem>();

			MenuStrip topMenuStrip = new MenuStrip();
			topMenuStrip.Name = "menuStrip";

			ToolStripMenuItem fileMenuItem = new ToolStripMenuItem(LngText(LngKeys.MainMenu_FileMenu));
			fileMenuItem.DropDownItems.Add(CreateMenuItem(MenuTypes.SaveBufferAsText, LngKeys.MainMenu_SaveBufferAsText, true));
			fileMenuItem.DropDownItems.Add(CreateMenuItem(MenuTypes.SaveBufferAsImage, LngKeys.MainMenu_SaveBufferAsImage, true));
			fileMenuItem.DropDownItems.Add(CreateMenuItem(MenuTypes.ClearBuffer, LngKeys.MainMenu_ClearBuffer, true));
			fileMenuItem.DropDownItems.Add(new ToolStripSeparator());
			fileMenuItem.DropDownItems.Add(CreateMenuItem(MenuTypes.Config, LngKeys.MainMenu_Config, true));
			fileMenuItem.DropDownItems.Add(new ToolStripSeparator());
			fileMenuItem.DropDownItems.Add(CreateMenuItem(MenuTypes.Exit, LngKeys.MainMenu_Exit, true));
			topMenuStrip.Items.Add(fileMenuItem);

			ToolStripMenuItem favoritesMenuItem = new ToolStripMenuItem(LngText(LngKeys.MainMenu_FavoritesMenu));
			favoritesMenuItem.DropDownItems.Add(
				CreateMenuItem(MenuTypes.OpenFavorites, LngKeys.MainMenu_OpenFavorites, true));
			topMenuStrip.Items.Add(favoritesMenuItem);

			ToolStripMenuItem textEditorMenuItem = new ToolStripMenuItem(LngText(LngKeys.MainMenu_ExtrasMenu));
			textEditorMenuItem.DropDownItems.Add(CreateMenuItem(MenuTypes.OpenTextEditor, LngKeys.MainMenu_OpenTextEditor, true));
			textEditorMenuItem.DropDownItems.Add(new ToolStripSeparator());
			textEditorMenuItem.DropDownItems.Add(CreateMenuItem(MenuTypes.OpenTapePunchEditor, LngKeys.MainMenu_OpenTapePunchEditor, true));
			textEditorMenuItem.DropDownItems.Add(CreateMenuItem(MenuTypes.EyeballCharOnOff, LngKeys.MainMenu_EyeballCharOnOff, true));
			textEditorMenuItem.DropDownItems.Add(new ToolStripSeparator());
			textEditorMenuItem.DropDownItems.Add(CreateMenuItem(MenuTypes.TestPattern, LngKeys.MainMenu_TestPattern, true));
			textEditorMenuItem.DropDownItems.Add(new ToolStripSeparator());
			textEditorMenuItem.DropDownItems.Add(CreateMenuItem(MenuTypes.OpenScheduler, LngKeys.MainMenu_OpenScheduler, false));
			topMenuStrip.Items.Add(textEditorMenuItem);

			ToolStripMenuItem receiveMenuItem = new ToolStripMenuItem(LngText(LngKeys.MainMenu_ReceiveMenu));
			receiveMenuItem.Enabled = true;
			receiveMenuItem.DropDownItems.Add(CreateMenuItem(MenuTypes.ReceiveOnOff, LngKeys.MainMenu_ReceiveOnOff, true));
			receiveMenuItem.DropDownItems.Add(CreateMenuItem(MenuTypes.UpdateSubscribeServer, LngKeys.MainMenu_UpdateSubscribeServer, true));
			topMenuStrip.Items.Add(receiveMenuItem);

			ToolStripMenuItem debugMenuItem = new ToolStripMenuItem(LngText(LngKeys.MainMenu_DebugMenu));
			debugMenuItem.DropDownItems.Add(CreateMenuItem(MenuTypes.OpenDebugForm, LngKeys.MainMenu_OpenDebug, true));
			topMenuStrip.Items.Add(debugMenuItem);

			ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem(LngText(LngKeys.MainMenu_AboutMenu));
			aboutMenuItem.DropDownItems.Add(CreateMenuItem(MenuTypes.About, LngKeys.MainMenu_About, true));
			topMenuStrip.Items.Add(aboutMenuItem);

			return topMenuStrip;
		}

		private ToolStripMenuItem CreateMenuItem(MenuTypes menuType, LngKeys lngKey, bool enabled)
		{
			ToolStripMenuItem menuItem = new ToolStripMenuItem(LngText(lngKey))
			{
				Enabled = enabled,
				Tag = menuType,
				Checked = false,
			};
			menuItem.Click += Menu_Click;
			_menuItemList.Add(menuItem);
			return menuItem;
		}

		private void ChangedLanguageMenu()
		{
			foreach(ToolStripMenuItem item in _menuItemList)
			{
				item.Name = LngText(((MenuItemTag)item.Tag).LngKey);
			}
		}

		public void SetChecked(MenuTypes type, bool status)
		{
			ToolStripMenuItem item = (from t in _menuItemList where (MenuTypes)t.Tag == type select t).FirstOrDefault();
			if (item != null) item.Checked = status;
		}

		private void Menu_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
			MenuTypes menuType = (MenuTypes)menuItem.Tag;

			_menuHandler(menuType);
		}

		private string LngText(LngKeys key)
		{
			return LanguageManager.Instance.GetText(key);
		}
	}

	class MenuItemTag
	{
		public MainStripMenu.MenuTypes MenuType { get; set; }

		public LngKeys LngKey { get; set; }

		public MenuItemTag(MainStripMenu.MenuTypes menuType, LngKeys lngKey)
		{
			MenuType = menuType;
			LngKey = lngKey;
		}
	}
}
