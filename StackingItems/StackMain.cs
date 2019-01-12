using Smod2;
using Smod2.API;
using Smod2.Attributes;
using System.Collections.Generic;
using System;

namespace StackingItems
{
	[PluginDetails(
		author = "Mith",
		name = "StackingItems",
		description = "Items stack to save inventory space.",
		id = "mith.StackingItems",
		version = "1.01",
		SmodMajor = 3,
		SmodMinor = 2,
		SmodRevision = 2
		)]
	class StackMain : Plugin
	{
		internal static StackMain plugin;

		public static Dictionary<string, StackCheckSteamIDsforItemInts > checkSteamIDItemNum = new Dictionary<string, StackCheckSteamIDsforItemInts>();
		public static Dictionary<int, int> checkItemForItemStack = new Dictionary<int, int>();

		public static int Stack_KeycardOverride;

		public static bool fixUseMedKit = true;
		public static bool fixthrowGrenade = true;
		public static bool keepItemsOnExtract;

		public static int GetStackSize(int ItemType)
		{
			if (checkItemForItemStack.TryGetValue(ItemType, out int value))
			{
				return checkItemForItemStack[ItemType];
			}
			else
			{
				return -1;
			}
		}

		public static void SetStackSize()
		{
			if(!checkItemForItemStack.Equals(plugin.GetConfigIntDict("si_globaldict")))
			{
				foreach (KeyValuePair<int, int> item in plugin.GetConfigIntDict("si_globaldict"))
				{
					checkItemForItemStack[item.Key] = item.Value;
				}
			}
		}

		public class StackCheckSteamIDsforItemInts
		{
			public Dictionary<int, int> checkItemForNumOfItems = new Dictionary<int, int>();

			public bool HasEscaped = false;

			public void ResetToZero()
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
			plugin = this;
			this.Info(this.Details.id + " has been enabled.");
		}

		public override void Register()
		{
			
			this.AddEventHandlers(new StackEventHandler(this));

			this.AddCommand("si_version", new StackVersion(this));
			this.AddCommand("si_disable", new StackDisable(this));

			this.AddConfig(new Smod2.Config.ConfigSetting("si_override_keycard", -1, Smod2.Config.SettingType.NUMERIC, true, "Override all keycards to stack to this."));
			this.AddConfig(new Smod2.Config.ConfigSetting("si_disable", false, Smod2.Config.SettingType.BOOL, true, "Enable or disable this plugin."));
			this.AddConfig(new Smod2.Config.ConfigSetting("si_extract", true, Smod2.Config.SettingType.BOOL, true, "Should players keep their items when they extract."));

			foreach(ItemType type in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
			{
				checkItemForItemStack[(int)type] = 3;
			}
			this.AddConfig(new Smod2.Config.ConfigSetting("si_globaldict", checkItemForItemStack, Smod2.Config.SettingType.NUMERIC_DICTIONARY, true, "Dictionary that keeps stacksizes of all items in the game."));
		}
	}
}