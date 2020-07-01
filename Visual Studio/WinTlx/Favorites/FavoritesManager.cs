using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinTlx.Favorites
{
	class FavoritesManager
	{
		private const string TAG = nameof(FavoritesManager);

		private const string FAVORITES_NAME = "favorites.xml";
		private const string LASTCALLED_NAME = "lastcalled.xml";

		/// <summary>
		/// singleton pattern
		/// </summary>
		private static FavoritesManager instance;

		public static FavoritesManager Instance => instance ?? (instance = new FavoritesManager());

		public List<FavoriteItem> Favorites { get; set; }

		public List<FavoriteItem> LastCalled { get; set; }

		private FavoritesManager()
		{
		}

		public bool LoadFavorites()
		{
			try
			{
				if (!File.Exists(FAVORITES_NAME))
				{
					Logging.Instance.Info(TAG, nameof(LoadFavorites), "No favorites file found");
					return false;
				}

				string favoritesXml = File.ReadAllText(FAVORITES_NAME);
				SaveFavorites saveFavorites = Helper.Deserialize<SaveFavorites>(favoritesXml);
				Favorites = saveFavorites.Favorites;
				return true;
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(LoadFavorites), "Error reading favorites file", ex);
				Favorites = new List<FavoriteItem>();
				return false;
			}
		}

		public bool SaveFavorites()
		{
			try
			{
				string favoritesXml =  Helper.SerializeObject<SaveFavorites>(new SaveFavorites(Favorites));
				File.WriteAllText(FAVORITES_NAME, favoritesXml);
				Logging.Instance.Info(TAG, nameof(SaveFavorites), $"Favorites saved to {FAVORITES_NAME}");
				return true;
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SaveFavorites), "Error writing favorites file", ex);
				return false;
			}
		}

		public void AddFavorite(int number, string name)
		{
			Favorites.Add(new FavoriteItem(number, name)); 
		}

		public bool LoadLastCalled()
		{
			try
			{
				if (!File.Exists(LASTCALLED_NAME))
				{
					Logging.Instance.Info(TAG, nameof(LoadLastCalled), "No lastcalled file found");
					return false;
				}

				string lastCalledXml = File.ReadAllText(LASTCALLED_NAME);
				SaveLastCalled saveLastCalled = Helper.Deserialize<SaveLastCalled>(lastCalledXml);
				Favorites = saveLastCalled.LastCalled;
				return true;
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(LoadLastCalled), "Error reading lastcalled file", ex);
				LastCalled = new List<FavoriteItem>();
				return false;
			}
		}

		public bool SaveLastCalled()
		{
			try
			{
				string lastCalledXml = Helper.SerializeObject<SaveLastCalled>(new SaveLastCalled(LastCalled));
				File.WriteAllText(LASTCALLED_NAME, lastCalledXml);
				Logging.Instance.Info(TAG, nameof(SaveLastCalled), $"Favorites saved to {LASTCALLED_NAME}");
				return true;
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(SaveLastCalled), "Error writing lastcalled file", ex);
				return false;
			}
		}

	}

	class SaveFavorites
	{
		public string Version = "1.0";

		public List<FavoriteItem> Favorites { get; set; }

		public SaveFavorites(List<FavoriteItem> favorites)
		{
			Favorites = favorites;
		}
	}

	class SaveLastCalled
	{
		public string Version = "1.0";

		public List<FavoriteItem> LastCalled { get; set; }

		public SaveLastCalled(List<FavoriteItem> lastCalled)
		{
			LastCalled = lastCalled;
		}
	}
}
