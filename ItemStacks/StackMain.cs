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
		version = "0.12",
		SmodMajor = 3,
		SmodMinor = 1,
		SmodRevision = 21
		)]
	class StackMain : Plugin
	{
		internal static StackMain plugin;

		public static Dictionary<string, StackCheckSteamIDsforItemInts > checkSteamIDItemNum = new Dictionary<string, StackCheckSteamIDsforItemInts>();
		public static Dictionary<int, int> checkItemForItemStack = new Dictionary<int, int>();

		public static int Stack_KeycardOverride;
		public static int Stack_WeaponOverride;

		public static bool fixUseMedKit = true;
		public static bool fixthrowGrenade = true;

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
			foreach (Smod2.API.ItemType item in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
			{
				checkItemForItemStack[(int)item] = StackMain.plugin.GetConfigInt("stack_" + item.ToString().ToLower() + "_limit");
			}
		}

		public class StackCheckSteamIDsforItemInts
		{
			Dictionary<int, int> checkItemForNumOfItems = new Dictionary<int, int>();

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
			this.AddCommand("stack_version", new StackVersion(this));
			this.AddCommand("stack_disable", new StackDisable(this));
			this.AddEventHandlers(new StackEventHandler(this));
			
			this.AddConfig(new Smod2.Config.ConfigSetting("stack_override_keycard", -1, Smod2.Config.SettingType.NUMERIC, true, "Override all keycards to stack to this."));
			this.AddConfig(new Smod2.Config.ConfigSetting("stack_override_weapons", -1, Smod2.Config.SettingType.NUMERIC, true, "Override all weapons to stack to this."));
			this.AddConfig(new Smod2.Config.ConfigSetting("stack_disable", false, Smod2.Config.SettingType.BOOL, true, "Enable or disable this plugin."));

			foreach (Smod2.API.ItemType item in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
			{
				this.AddConfig(new Smod2.Config.ConfigSetting("stack_" + item.ToString().ToLower() + "_limit", 3, Smod2.Config.SettingType.NUMERIC, true, "How much " + item.ToString().ToLower() + " stacks to."));
			}
		}
	}
}