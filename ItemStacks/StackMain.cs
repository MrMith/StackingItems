using Smod2;
using Smod2.Attributes;
using System.Collections.Generic;
using System;

namespace itemStacks
{
	[PluginDetails(
		author = "Mith",
		name = "Item Stacks",
		description = "Items stack to save inventory space.",
		id = "mith.itemstacks",
		version = "0.10",
		SmodMajor = 3,
		SmodMinor = 1,
		SmodRevision = 21
		)]
	class StackMain : Plugin
	{
		public static Dictionary<string, StackCheckSteamIDsforItemInts > checkSteamIDItemNum = new Dictionary<string, StackCheckSteamIDsforItemInts>();

		public static bool fixUseMedKit = true;
		public static bool fixthrowGrenade = true;

		public class StackCheckSteamIDsforItemInts
		{
			public static Dictionary<int, int> checkItemForNumOfItems = new Dictionary<int, int>();

			public void Clear()
			{
				checkItemForNumOfItems.Clear();
			}

			public int GetItemAmount(int ItemType)
			{
				if (checkItemForNumOfItems.TryGetValue(ItemType, out int value))
				{
					return checkItemForNumOfItems[ItemType];
				}
				else
				{
					return -1;
				}
				
			}

			public void AddItemAmount(int ItemType,int Amount)
			{
				if (checkItemForNumOfItems.TryGetValue(ItemType, out int value))
				{
					checkItemForNumOfItems[ItemType] += Amount;
				}
				else
				{
					checkItemForNumOfItems[ItemType] = Amount;
				}
			}
		}

		public override void OnDisable()
		{
			this.Info(this.Details.id + " has been disabled.");
		}

		public override void OnEnable()
		{
			this.Info(this.Details.id + " has been enabled.");
		}

		public override void Register()
		{
			this.AddCommand("stack_version", new StackVersion(this));
			this.AddEventHandlers(new StackEventHandler(this));

			foreach (Smod2.API.ItemType item in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
			{
				//this.Info(item.ToString().ToLower());
				this.AddConfig(new Smod2.Config.ConfigSetting("stack_" + item.ToString().ToLower() + "_limit", 3, Smod2.Config.SettingType.NUMERIC, true, "How much " + item.ToString().ToLower() + " stacks to."));
			}
		}
	}
}