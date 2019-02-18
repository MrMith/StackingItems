using System.Collections.Generic;
using Smod2.API;
using System;

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
	}
}
