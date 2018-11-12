using Smod2;
using Smod2.Attributes;
using System.Collections.Generic;

/*
	REWRITE logic like in 212 to add methods for dropping and handling stacks. 
 */
namespace itemStacks
{
	[PluginDetails(
		author = "Mith",
		name = "Item Stacks",
		description = "Items stack to save inventory space.",
		id = "mith.itemstacks",
		version = "0.01",
		SmodMajor = 3,
		SmodMinor = 1,
		SmodRevision = 21
		)]
	class StackMain : Plugin
	{
		public static Dictionary<string, StackCheckSteamIDsforItemInts > checkSteamIDItemNum= new Dictionary<string, StackCheckSteamIDsforItemInts>();

		public static bool fixUseMedKit = true;
		public static bool fixthrowGrenade = true;

		public class StackCheckSteamIDsforItemInts
		{
			public int Medkits = 0,
			Frags = 0,
			Flashs = 0,
			Coins = 0;

			public void Clear()
			{
				this.Medkits = 0;
				this.Frags = 0;
				this.Flashs = 0;
				this.Coins = 0;
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

			this.AddConfig(new Smod2.Config.ConfigSetting("stack_flashlimit", 3, Smod2.Config.SettingType.NUMERIC, true, "How much flashbangs stack to"));
			this.AddConfig(new Smod2.Config.ConfigSetting("stack_fraglimit", 3, Smod2.Config.SettingType.NUMERIC, true, "How much frag grenades stack to"));
			this.AddConfig(new Smod2.Config.ConfigSetting("stack_coinlimit", 4, Smod2.Config.SettingType.NUMERIC, true, "How much coins stack to"));
			this.AddConfig(new Smod2.Config.ConfigSetting("stack_medkitlimit", 5, Smod2.Config.SettingType.NUMERIC, true, "How much medkits stack to"));
		}
	}
}