using Smod2.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StackingItems.Managers
{
	class _914Manager
	{
		public SI_ConfigOptions ConfigOptions = new SI_ConfigOptions();
		public StackManager stackManager = new StackManager();

		public void SetUp(StackingItems.Managers.SI_ConfigOptions sI_ConfigOptions, StackManager stackmanager)
		{
			ConfigOptions = sI_ConfigOptions;
			stackManager = stackmanager;
		}

		/// <summary>
		/// Does a full 914 pass on the user's held item.
		/// </summary>
		/// <param name="collider">Player</param>
		/// <param name="OutputPos">Smod2 Vector on the output of 914</param>
		/// <param name="knobSetting">Current 914 knob setting</param>
		/// <param name="scp914">914</param>
		public void DoHand914(UnityEngine.Collider collider, Vector OutputPos, KnobSetting knobSetting, Scp914 scp914)
		{
			Smod2.API.Player play = new ServerMod2.API.SmodPlayer(collider.gameObject);

			if (play.GetCurrentItem() == null || play.GetCurrentItemIndex() == -1 || play.GetCurrentItem().ItemType == ItemType.USP || stackManager.ContainsWeapon((int)play.GetCurrentItem().ItemType) && knobSetting == KnobSetting.ONE_TO_ONE)
			{
				return;
			}
			Smod2.API.ItemType currentItem = play.GetCurrentItem().ItemType;
			int itemIndex = -1;

			Dictionary<int, List<Inventory.SyncItemInfo>> TempGetItemsByItemType = new Dictionary<int, List<Inventory.SyncItemInfo>>(); //This is for seeing a item's index in inventory
			
			foreach (var item in collider.gameObject.GetComponent<Inventory>().items)
			{
				if (TempGetItemsByItemType.ContainsKey(item.id))
				{
					TempGetItemsByItemType[item.id].Add(item);
					if (item.uniq == collider.gameObject.GetComponent<Inventory>().GetItemInHand().uniq)
					{
						itemIndex = TempGetItemsByItemType[item.id].IndexOf(item) + 1;
					}
				}
				else
				{
					TempGetItemsByItemType.Add(item.id, new List<Inventory.SyncItemInfo> { item });
					if (item.uniq == collider.gameObject.GetComponent<Inventory>().GetItemInHand().uniq)
					{
						itemIndex = TempGetItemsByItemType[item.id].IndexOf(item) + 1;
					}
				}
			}

			play.GetCurrentItem().Remove();

			int AmountOfItems = -1;
			if (TempGetItemsByItemType[(int)currentItem].Count == itemIndex)
			{
				AmountOfItems = (int)Math.Ceiling(StackEventHandler.CheckSteamIDItemNum[play.SteamId].GetItemAmount((int)currentItem) % (float)stackManager.GetStackSize((int)currentItem));
			}
			else
			{
				AmountOfItems = stackManager.GetStackSize((int)currentItem);
			}

			if (stackManager.GetStackSize((int)currentItem) == 1) AmountOfItems = 1;

			StackEventHandler.CheckSteamIDItemNum[play.SteamId].AddItemAmount((int)currentItem, AmountOfItems * -1);
			for (int i = 0; i < AmountOfItems; i++)
			{
				int newitem = GetRandom914ItemByKnob(scp914, (int)currentItem, (int)knobSetting);
				if (newitem != -1 || ConfigOptions.si_914itemstosave.Contains(newitem))
				{
					if (play.GetInventory().Count == 8)
					{
						Smod2.PluginManager.Manager.Server.Map.SpawnItem((ItemType)newitem, OutputPos, new Vector(0, 0, 0));
					}
					else
					{
						StackEventHandler.CheckSteamIDItemNum[play.SteamId].AddItemAmount(newitem, 1);
					}
				}
			}

			StackEventHandler.CheckSteamIDItemNum[play.SteamId].TransferItems(collider.gameObject);
			StackEventHandler.CheckSteamIDItemNum[play.SteamId]._914Transfer = true;
		}

		/// <summary>
		/// Does a full 914 pass on each item in the user's inventory.
		/// </summary>
		/// <param name="collider">Player</param>
		/// <param name="OutputPos">Smod2 Vector on the output of 914</param>
		/// <param name="knobSetting">Current 914 knob setting</param>
		/// <param name="scp914">914</param>
		public void DoInventory914(UnityEngine.Collider collider, Vector OutputPos, KnobSetting knobSetting, Scp914 scp914)
		{
			Smod2.API.Player play = new ServerMod2.API.SmodPlayer(collider.gameObject);
			List<Inventory.SyncItemInfo> TempItems = new List<Inventory.SyncItemInfo>();
			TempItems.AddRange(collider.GetComponent<Inventory>().items);

			Dictionary<int, int> TempListOfItems = new Dictionary<int, int>();

			foreach (KeyValuePair<int, int> TypeAndAmount in StackEventHandler.CheckSteamIDItemNum[play.SteamId].checkItemTypeForNumOfItems)
			{
				TempListOfItems[TypeAndAmount.Key] = TypeAndAmount.Value;
			}
			StackEventHandler.CheckSteamIDItemNum[play.SteamId].ResetToZero();

			foreach (Inventory.SyncItemInfo item in TempItems)
			{
				if (!ConfigOptions.si_914itemstosave.Contains(item.id) && item.id != 30)
				{
					double ItemAmount;

					if (TempListOfItems.ContainsKey(item.id) && !stackManager.ContainsWeapon(item.id))
					{
						int TempItemAmount = (int)(TempListOfItems[item.id] - Math.Ceiling(TempListOfItems[item.id] / (float)stackManager.GetStackSize(item.id)));
						if (TempItemAmount >= stackManager.GetStackSize(item.id))
						{
							ItemAmount = stackManager.GetStackSize(item.id);
							TempListOfItems[item.id] -= stackManager.GetStackSize(item.id);
						}
						else
						{
							ItemAmount = TempListOfItems[item.id];
							TempListOfItems[item.id] = 0;
						}
					}
					else
					{
						ItemAmount = 1;
					}
					collider.GetComponent<Inventory>().items.Remove(item);
					for (int i = 0; i < (int)ItemAmount; i++)
					{
						int newitem = GetRandom914ItemByKnob(scp914, item.id, (int)knobSetting);
						if (newitem != -1)
						{
							if (play.GetInventory().Count == 8)
							{
								Smod2.PluginManager.Manager.Server.Map.SpawnItem((ItemType)newitem, OutputPos, new Vector(0, 0, 0));
							}
							else
							{
								StackEventHandler.CheckSteamIDItemNum[play.SteamId].AddItemAmount(newitem,1);
							}
						}
					}
				}
			}
			StackEventHandler.CheckSteamIDItemNum[play.SteamId].TransferItems(collider.gameObject);
			StackEventHandler.CheckSteamIDItemNum[play.SteamId]._914Transfer = true;
		}
		
		/// <summary>
		/// Gets randomized 914 outout recipe
		/// </summary>
		/// <param name="scp914">914</param>
		/// <param name="item">item to get recipe for</param>
		/// <param name="KnobSetting">914 knob setting</param>
		/// <returns></returns>
		public int GetRandom914ItemByKnob(Scp914 scp914, int item, int KnobSetting)
		{
			return scp914.recipes[(byte)((ItemType)item)].outputs[(byte)KnobSetting].outputs[UnityEngine.Random.Range(0, scp914.recipes[(byte)((ItemType)item)].outputs[(byte)KnobSetting].outputs.Count)];
			//shoutout to https://github.com/storm37000/SCPSL_Smod_914_held_items/blob/99cfbc282524412018d5fd53bb6fa92838a5ebc1/EventHandler.cs#L85
		}
	}
}
