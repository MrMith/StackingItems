using Smod2;
using Smod2.Attributes;
using System.Collections.Generic;

namespace StackingItems
{
	[PluginDetails(
		author = "Mith",
		name = "StackingItems",
		description = "Items stack to save inventory space.",
		id = "mith.StackingItems",
		version = "1.1.5",
		SmodMajor = 3,
		SmodMinor = 4,
		SmodRevision = 0
		)]
	class StackMain : Plugin
	{
		internal static StackMain plugin;

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

			this.AddConfig(new Smod2.Config.ConfigSetting("si_override_keycard", -1, true, "Override all keycards to stack to this."));
			this.AddConfig(new Smod2.Config.ConfigSetting("si_disable", false, true, "Enable or disable this plugin."));
			this.AddConfig(new Smod2.Config.ConfigSetting("si_extract", true, true, "Should players keep their items when they extract."));
			this.AddConfig(new Smod2.Config.ConfigSetting("si_globalstacksize", 1, true, "If this is set to anything over 1 this will change every stacksize for every item to this. si_globaldict values will still be set"));
			this.AddConfig(new Smod2.Config.ConfigSetting("si_globaldict", new Dictionary<int, int> { }, true, "Dictionary that keeps custom stacksizes for items."));

			this.AddConfig(new Smod2.Config.ConfigSetting("si_914enable", true, true, "Should player's inventory interact with 914. Set to false to disable any interaction with 914 (Items will be removed if disabled)"));
			this.AddConfig(new Smod2.Config.ConfigSetting("si_914handorinv", 0, true, "Integer value that decides wether to not touch the inventory and just pass items along or upgrade item in hand or the whole user's inventory. 0 = Transfer items no upgrades, 1 = Entire inventory gets upgraded, 2 = just hand gets upgraded."));
			this.AddConfig(new Smod2.Config.ConfigSetting("si_914itemstosave", new int[] { 12,14,15,17,18,19 }, true, "Any itemtype in this list will NOT be consumed by 914 while in inventory or hand."));
		}
	}
}