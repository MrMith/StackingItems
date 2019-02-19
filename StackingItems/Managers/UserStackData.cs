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
		public bool fixthrowGrenade = false;
		public bool Escape = false;
		public bool _914Transfer = false;

		public void ResetToZero()
		{
			checkItemTypeForNumOfItems.Clear();
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
			Smod2.API.Player playa = new ServerMod2.API.SmodPlayer(gameObject);

			TempItemList.Clear();

			foreach(KeyValuePair<int,int> keyValuePair in checkItemTypeForNumOfItems)
			{
				TempItemList[keyValuePair.Key] = keyValuePair.Value;
			}

			ammo9 = playa.GetAmmo(AmmoType.DROPPED_9);
			ammo7 = playa.GetAmmo(AmmoType.DROPPED_7);
			ammo5 = playa.GetAmmo(AmmoType.DROPPED_5);

			checkItemTypeForNumOfItems.Clear();
		}

		public async void ApplyTransferedItems(UnityEngine.GameObject gameObject, bool clear = false)
		{
			await Task.Delay(250);
			Smod2.API.Player playa = new ServerMod2.API.SmodPlayer(gameObject);
			if (clear)
			{
				foreach (Smod2.API.Item item in playa.GetInventory())
				{
					item.Remove();
				}
			}

			StackEventHandler.CheckSteamIDItemNum[playa.SteamId].ResetToZero();
			playa.SetAmmo(AmmoType.DROPPED_9, StackEventHandler.CheckSteamIDItemNum[playa.SteamId].ammo9 + playa.GetAmmo(AmmoType.DROPPED_9));
			playa.SetAmmo(AmmoType.DROPPED_7, StackEventHandler.CheckSteamIDItemNum[playa.SteamId].ammo7 + playa.GetAmmo(AmmoType.DROPPED_7));
			playa.SetAmmo(AmmoType.DROPPED_5, StackEventHandler.CheckSteamIDItemNum[playa.SteamId].ammo5 + playa.GetAmmo(AmmoType.DROPPED_5));
			foreach (Smod2.API.ItemType item in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
			{
				if (StackEventHandler.CheckSteamIDItemNum[playa.SteamId].TempItemList.TryGetValue((int)item, out int value))
				{
					for (int i = 0; i < value; i++)
					{
						playa.GiveItem(item);
					}
				}
			}
			StackEventHandler.CheckSteamIDItemNum[playa.SteamId].TempItemList.Clear();
		}
	}
}
