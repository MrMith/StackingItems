using System.Collections.Generic;
using Smod2.API;
using System;
using System.Threading.Tasks;

namespace StackingItems.Managers
{
	class UserStackData
	{
		public Dictionary<int, int> checkItemTypeForNumOfItems = new Dictionary<int, int>();
		public Dictionary<int, int> TempItemList = new Dictionary<int, int>();

		public int ammo9 = 0,
			ammo7 = 0,
			ammo5 = 0;

		public bool fixUse = false;
		public bool Escape = false;
		public bool _914Transfer = false;

		public void ResetToZero()
		{
			checkItemTypeForNumOfItems.Clear();
		}

        public void SetItemAmount(int ItemType, int Amount)
        {
            checkItemTypeForNumOfItems[ItemType] = Amount;
        }

		public int GetItemAmount(int ItemType)
		{
			if (checkItemTypeForNumOfItems.TryGetValue(ItemType, out int value))
			{
				return value;
			}
			else
			{
				return 0;
			}
		}

		public void AddItemAmount(int ItemType, int Amount)
		{
			if (checkItemTypeForNumOfItems.TryGetValue(ItemType, out int value))
			{
				checkItemTypeForNumOfItems[ItemType] += Amount;
			}
			else
			{
				checkItemTypeForNumOfItems[ItemType] = Amount;
			}
		}

		public void TransferItems(UnityEngine.GameObject gameObject)
		{
			Smod2.API.Player player = new ServerMod2.API.SmodPlayer(gameObject);

			TempItemList.Clear();

			foreach(KeyValuePair<int,int> keyValuePair in checkItemTypeForNumOfItems)
			{
				TempItemList[keyValuePair.Key] = keyValuePair.Value;
			}

			ammo9 = player.GetAmmo(AmmoType.DROPPED_9);
			ammo7 = player.GetAmmo(AmmoType.DROPPED_7);
			ammo5 = player.GetAmmo(AmmoType.DROPPED_5);

			checkItemTypeForNumOfItems.Clear();
		}

		public async void ApplyTransferedItems(UnityEngine.GameObject gameObject, bool clear = false)
		{
			await Task.Delay(250);
			Smod2.API.Player player = new ServerMod2.API.SmodPlayer(gameObject);
			if (clear)
			{
				foreach (Smod2.API.Item item in player.GetInventory())
				{
					item.Remove();
				}
			}

			StackEventHandler.CheckSteamIDItemNum[player.SteamId].ResetToZero();

			if(Escape)
			{
				player.SetAmmo(AmmoType.DROPPED_9, StackEventHandler.CheckSteamIDItemNum[player.SteamId].ammo9 + player.GetAmmo(AmmoType.DROPPED_9));
				player.SetAmmo(AmmoType.DROPPED_7, StackEventHandler.CheckSteamIDItemNum[player.SteamId].ammo7 + player.GetAmmo(AmmoType.DROPPED_7));
				player.SetAmmo(AmmoType.DROPPED_5, StackEventHandler.CheckSteamIDItemNum[player.SteamId].ammo5 + player.GetAmmo(AmmoType.DROPPED_5));
			}
			
			foreach (Smod2.API.ItemType item in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
			{
				if (StackEventHandler.CheckSteamIDItemNum[player.SteamId].TempItemList.TryGetValue((int)item, out int value))
				{
					for (int i = 0; i < value; i++)
					{
						player.GiveItem(item);
					}
				}
			}
			StackEventHandler.CheckSteamIDItemNum[player.SteamId].TempItemList.Clear();
		}
	}
}
