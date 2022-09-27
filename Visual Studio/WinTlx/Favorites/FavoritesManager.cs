using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace WinTlx.Favorites
{
	class FavoritesManager
	{
		private const string TAG = nameof(FavoritesManager);

		/// <summary>
		/// singleton pattern
		/// </summary>
		private static FavoritesManager instance;

		public static FavoritesManager Instance => instance ?? (instance = new FavoritesManager());

		private FavoritesManager()
		{
			CallHistoryLoad();
		}

		#region FavList

		private const string FAVORITES_NAME = "favorites.xml";

		public delegate void DialEventHandler(FavoriteItem favItem);
		public event DialEventHandler DialFavorite;

		public List<FavoriteItem> Favorites { get; set; }

		public bool FavListLoad()
		{
			Favorites = new List<FavoriteItem>();
			try
			{
				if (!File.Exists(FAVORITES_NAME))
				{
					Logging.Instance.Info(TAG, nameof(FavListLoad), "No favorites file found");
					return false;
				}

				string favoritesXml = File.ReadAllText(FAVORITES_NAME);
				SaveFavorites saveFavorites = Helper.Deserialize<SaveFavorites>(favoritesXml);
				Favorites = saveFavorites.Favorites;
				return true;
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(FavListLoad), "Error reading favorites file", ex);
				return false;
			}
		}

		public bool FavListSave()
		{
			try
			{
				string favoritesXml =  Helper.SerializeObject<SaveFavorites>(new SaveFavorites(Favorites));
				File.WriteAllText(FAVORITES_NAME, favoritesXml);
				Logging.Instance.Info(TAG, nameof(FavListSave), $"Favorites saved to {FAVORITES_NAME}");
				return true;
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(FavListSave), "Error writing favorites file", ex);
				return false;
			}
		}

		public List<FavoriteItem> FavListGetSortedFavorites(FavoritesItemSorter.SortIndex? index, bool ascending)
		{
			List<FavoriteItem> favList = new List<FavoriteItem>();
			foreach(FavoriteItem favItem in Favorites)
			{
				favList.Add(favItem);
			}
			if (index != null)
			{
				favList.Sort(new FavoritesItemSorter(index.Value, ascending));
			}
			return favList;
		}

		public FavoriteItem FavListGetFavorite(string number)
		{
			return (from f in Favorites where f.Number == number select f).FirstOrDefault();
		}

		/*
		public void AddFavorite(int number, string name)
		{
			Favorites.Add(new FavoriteItem(number, name)); 
		}
		*/

		public void DelFavorite(string number)
		{
			for (int i = 0; i < Favorites.Count; i++)
			{
				if (Favorites[i].Number == number)
				{
					Favorites.RemoveAt(i);
					return;
				}
			}
		}

		public void FavListDial(FavoriteItem favItem)
		{
			DialFavorite?.Invoke(favItem);
		}

		public bool FavListCheckChanged(List<FavoriteItem> favList)
		{
			if (favList.Count != Favorites.Count) return true;

			for (int i=0; i<favList.Count; i++)
			{
				if (favList[i].IsEqual(Favorites[i])) return true;
			}
			// all items are equal
			return false;
		}

		#endregion

		#region Call history

		private const string CALLHISTORY_NAME = "callhistory.xml";

		public delegate void UpdateCallHistoryEventHandler();
		public event UpdateCallHistoryEventHandler UpdateCallHistory;

		public List<CallHistoryItem> History { get; set; }

		public void CallHistoryAddCall(string number, string name, string result)
		{
			CallHistoryItem callHistItem = new CallHistoryItem()
			{
				Number = number,
				Name = name,
				Result = result,
				TimeStamp = DateTime.Now,
			};
			History.Add(callHistItem);
			CallHistorySave();
			UpdateCallHistory?.Invoke();
		}

		public void CallHistoryClear()
		{
			History.Clear();
			CallHistorySave();
			UpdateCallHistory?.Invoke();
		}

		public bool CallHistoryLoad()
		{
			History = new List<CallHistoryItem>();
			try
			{
				if (!File.Exists(CALLHISTORY_NAME))
				{
					Logging.Instance.Info(TAG, nameof(CallHistoryLoad), "call history file found");
					return false;
				}

				string callHistXml = File.ReadAllText(CALLHISTORY_NAME);
				SaveCallHistory saveCallHist = Helper.Deserialize<SaveCallHistory>(callHistXml);
				History = saveCallHist.CallHistory;
				return true;
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(CallHistoryLoad), "Error reading call history file", ex);
				return false;
			}
		}

		public bool CallHistorySave()
		{
			try
			{
				string callHistXml = Helper.SerializeObject<SaveCallHistory>(new SaveCallHistory(History));
				File.WriteAllText(CALLHISTORY_NAME, callHistXml);
				Logging.Instance.Info(TAG, nameof(CallHistorySave), $"Call history saved to {CALLHISTORY_NAME}");
				return true;
			}
			catch (Exception ex)
			{
				Logging.Instance.Error(TAG, nameof(CallHistorySave), "Error writing call history file", ex);
				return false;
			}
		}

		#endregion
	}

	[DataContract(Namespace = "")]
	class SaveFavorites
	{
		[DataMember]
		public string Version = "1.0";

		[DataMember]
		public List<FavoriteItem> Favorites { get; set; }

		public SaveFavorites(List<FavoriteItem> favorites)
		{
			Favorites = favorites;
		}
	}

	[DataContract(Namespace = "")]
	class SaveCallHistory
	{
		[DataMember]
		public string Version = "1.0";

		[DataMember]
		public List<CallHistoryItem> CallHistory { get; set; }

		public SaveCallHistory(List<CallHistoryItem> callHistory)
		{
			CallHistory = callHistory;
		}
	}
}
