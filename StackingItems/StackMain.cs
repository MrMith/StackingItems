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
		version = "1.0.6",
		SmodMajor = 3,
		SmodMinor = 2,
		SmodRevision = 2
		)]
	class StackMain : Plugin
	{
		internal static StackMain plugin;

		public static Dictionary<string, StackCheckSteamIDsforItemInts> checkSteamIDItemNum = new Dictionary<string, StackCheckSteamIDsforItemInts>();
		public static Dictionary<int, int> checkItemForItemStack = new Dictionary<int, int>();

		public static int Stack_KeycardOverride;

		public static bool keepItemsOnExtract;
		public static int globalstacksize = 1;

		public static int GetStackSize(int ItemType)
		{
			if (checkItemForItemStack.TryGetValue(ItemType, out int value))
			{
				return value;
			}
			else
			{
				return -1;
			}
		}

		public static void SetStackSize()
		{
			if (globalstacksize != 1)
			{
				foreach (ItemType type in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
				{
					checkItemForItemStack[(int)type] = globalstacksize;
				}
				foreach (KeyValuePair<int, int> item in plugin.GetConfigIntDict("si_globaldict"))
				{
					checkItemForItemStack[item.Key] = item.Value;
				}
			}
			else
			{
				foreach (KeyValuePair<int, int> item in plugin.GetConfigIntDict("si_globaldict"))
				{
					checkItemForItemStack[item.Key] = item.Value;
				}
			}
		}

		public class StackCheckSteamIDsforItemInts
		{
			public Dictionary<int, int> checkItemTypeForNumOfItems = new Dictionary<int, int>();
			public Dictionary<int, int> EscapedItemList = new Dictionary<int, int>();

			public int ammo9 = 0,
				ammo7 = 0,
				ammo5 = 0;

			public bool fixUseMedKit = false;
			public bool fixthrowGrenade = false;

			public bool HasEscaped = false;

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
			this.AddConfig(new Smod2.Config.ConfigSetting("si_globalstacksize", 1, Smod2.Config.SettingType.NUMERIC, true, "If this is set to anything over 1 this will change every stacksize for every item to this. si_globaldict values will still be set"));
			this.AddConfig(new Smod2.Config.ConfigSetting("si_globaldict", new Dictionary<int, int> { }, Smod2.Config.SettingType.NUMERIC_DICTIONARY, true, "Dictionary that keeps custom stacksizes for items."));
		}
	}
}