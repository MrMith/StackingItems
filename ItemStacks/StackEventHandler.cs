using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System;

/*
 * If you're wondering why I'm casting floats on GetConfigInt its because if its set to 0 and I divide by 0 while its a int it will error but floats can handle divide by zero.
 */

namespace itemStacks
{
	class StackEventHandler : IEventHandlerMedkitUse, IEventHandlerPlayerPickupItem, IEventHandlerThrowGrenade, IEventHandlerPlayerDropItem, IEventHandlerPlayerDie, IEventHandlerRoundStart,IEventHandlerSetRole
	{
		private readonly Plugin plugin;

		StackMain.StackCheckSteamIDsforItemInts value;

		public StackEventHandler(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public void OnPlayerDropItem(PlayerDropItemEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.TryGetValue(ev.Player.SteamId, out value))
			{
				switch (ev.Item.ItemType)
				{
					case ItemType.MEDKIT:
						if (value.Medkits >= 1)
						{
							StackMain.checkSteamIDItemNum[ev.Player.SteamId].Medkits--;
							if (value.Medkits % plugin.GetConfigInt("stack_medkitlimit") == 0 && value.Medkits >= 2 || value.Medkits == 0)
							{
								ev.Allow = true;
							}
							else
							{
								Smod2.PluginManager.Manager.Server.Map.SpawnItem(ItemType.MEDKIT, ev.Player.GetPosition(), new Vector(0, 0, 0));
								ev.Allow = false;
							}
						}
						break;
					case ItemType.COIN:
						if (value.Coins >= 1)
						{
							StackMain.checkSteamIDItemNum[ev.Player.SteamId].Coins--;
							if (value.Coins % plugin.GetConfigInt("stack_coinlimit") == 0 && value.Coins >= 2 || value.Coins == 0)
							{
								ev.Allow = true;
							}
							else
							{
								Smod2.PluginManager.Manager.Server.Map.SpawnItem(ItemType.COIN, ev.Player.GetPosition(), new Vector(0, 0, 0));
								ev.Allow = false;
							}

						}
						break;
					case ItemType.FLASHBANG:
						if (value.Flashs >= 1)
						{
							StackMain.checkSteamIDItemNum[ev.Player.SteamId].Flashs--;
							if (value.Flashs % plugin.GetConfigInt("stack_flashlimit") == 0 && value.Flashs >=2 || value.Flashs == 0)
							{
								ev.Allow = true;
							}
							else
							{
								Smod2.PluginManager.Manager.Server.Map.SpawnItem(ItemType.FLASHBANG, ev.Player.GetPosition(), new Vector(0, 0, 0));
								ev.Allow = false;
							}
						}
						break;
					case ItemType.FRAG_GRENADE:
						if (value.Frags >= 1)
						{
							StackMain.checkSteamIDItemNum[ev.Player.SteamId].Frags--;
							if (value.Frags % plugin.GetConfigInt("stack_fraglimit") == 0 && value.Frags >= 2 || value.Frags == 0)
							{
								ev.Allow = true;
							}
							else
							{
								Smod2.PluginManager.Manager.Server.Map.SpawnItem(ItemType.FRAG_GRENADE, ev.Player.GetPosition(), new Vector(0, 0, 0));
								ev.Allow = false;
							}
						}
						break;
				}
			}
		}

		public void OnPlayerPickupItem(PlayerPickupItemEvent ev)
		{
			if (!StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId] = new StackMain.StackCheckSteamIDsforItemInts();
			}
			switch (ev.Item.ItemType)
			{
				case ItemType.MEDKIT:
					if (StackMain.fixUseMedKit && plugin.GetConfigInt("stack_medkitlimit") >= 2)
					{
						StackMain.checkSteamIDItemNum[ev.Player.SteamId].Medkits++;
						if ((StackMain.checkSteamIDItemNum[ev.Player.SteamId].Medkits + (plugin.GetConfigInt("stack_medkitlimit") - 1)) % plugin.GetConfigInt("stack_medkitlimit") != 0)
						{
							ev.Allow = false;
						}
					}
					break;
				case ItemType.COIN:
					if (plugin.GetConfigInt("stack_coinlimit") >= 2)
					{
						StackMain.checkSteamIDItemNum[ev.Player.SteamId].Coins++;
						if ((StackMain.checkSteamIDItemNum[ev.Player.SteamId].Coins + (plugin.GetConfigInt("stack_coinlimit") - 1)) % plugin.GetConfigInt("stack_coinlimit") != 0)
						{
							ev.Allow = false;
						}
					}
					break;
				case ItemType.FLASHBANG:
					if (StackMain.fixthrowGrenade && plugin.GetConfigInt("stack_flashlimit") >= 2)
					{
						StackMain.checkSteamIDItemNum[ev.Player.SteamId].Flashs++;
						if ((StackMain.checkSteamIDItemNum[ev.Player.SteamId].Flashs + (plugin.GetConfigInt("stack_flashlimit") - 1)) % plugin.GetConfigInt("stack_flashlimit") != 0)
						{
							ev.Allow = false;
						}
					}
					break;
				case ItemType.FRAG_GRENADE:
					if (StackMain.fixthrowGrenade && plugin.GetConfigInt("stack_fraglimit") >= 2)
					{
						StackMain.checkSteamIDItemNum[ev.Player.SteamId].Frags++;
						if ((StackMain.checkSteamIDItemNum[ev.Player.SteamId].Frags + (plugin.GetConfigInt("stack_fraglimit") - 1)) % plugin.GetConfigInt("stack_fraglimit") != 0)
						{
							ev.Allow = false;
						}
					}
					break;
			}
		}

		public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.TryGetValue(ev.Player.SteamId, out value))
			{
				if (value.Frags >= 1 && ev.GrenadeType == ItemType.FRAG_GRENADE)
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].Frags--;
					if (value.Frags % plugin.GetConfigInt("stack_fraglimit") != 0 && value.Frags >= 2)
					{
						StackMain.fixthrowGrenade = false;
						ev.Player.GiveItem(ItemType.FRAG_GRENADE);
						StackMain.fixthrowGrenade = true; //Looks ugly but is needed so it doesn't add one to my stacking system.
					}
				}
				if (value.Flashs >= 1 && ev.GrenadeType == ItemType.FLASHBANG)
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].Flashs--;
					if (value.Flashs % plugin.GetConfigInt("stack_flashlimit") != 0 && value.Flashs >= 2)
					{
						StackMain.fixthrowGrenade = false;
						ev.Player.GiveItem(ItemType.FLASHBANG);
						StackMain.fixthrowGrenade = true; //Looks ugly but is needed so it doesn't add one to my stacking system. 
					}
				}
			}
		}

		public void OnMedkitUse(PlayerMedkitUseEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.TryGetValue(ev.Player.SteamId, out value))
			{
				if (value.Medkits >= 1)
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].Medkits--;
					if (value.Medkits % plugin.GetConfigInt("stack_medkitlimit") != 0 && value.Medkits >= 2)
					{
						StackMain.fixUseMedKit = false;
						ev.Player.GiveItem(ItemType.MEDKIT);
						StackMain.fixUseMedKit = true; //Looks ugly but is needed to it doesn't add one to my stacking system. 
					}
				}
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.TryGetValue(ev.Player.SteamId, out value))
			{
				if (value.Medkits >= 1)
				{
					for (int i = 0;  i < (value.Medkits - Math.Floor((float)value.Medkits/(float)plugin.GetConfigInt("stack_medkitlimit"))); i++)
					{
						Smod2.PluginManager.Manager.Server.Map.SpawnItem(ItemType.MEDKIT, ev.Player.GetPosition(), new Vector(0, 0, 0));
					}
				}
				if (value.Frags >= 1)
				{
					for (int i = 0; i < (value.Frags - Math.Floor((float)value.Frags / (float)plugin.GetConfigInt("stack_fraglimit"))); i++)
					{
						Smod2.PluginManager.Manager.Server.Map.SpawnItem(ItemType.FRAG_GRENADE, ev.Player.GetPosition(), new Vector(0, 0, 0));
					}
				}
				if (value.Coins >= 1)
				{
					for (int i = 0; i < (value.Coins - Math.Floor((float)value.Coins / (float)plugin.GetConfigInt("stack_coinlimit"))); i++)
					{
						Smod2.PluginManager.Manager.Server.Map.SpawnItem(ItemType.COIN, ev.Player.GetPosition(), new Vector(0, 0, 0));
					}
				}
				if (value.Flashs >= 1)
				{
					for (int i = 0; i < (value.Flashs - Math.Floor((float)value.Flashs / (float)plugin.GetConfigInt("stack_flashlimit"))); i++)
					{
						Smod2.PluginManager.Manager.Server.Map.SpawnItem(ItemType.FLASHBANG, ev.Player.GetPosition(), new Vector(0, 0, 0));
					}
				}
				StackMain.checkSteamIDItemNum[ev.Player.SteamId].Clear();
			}
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			foreach (Player playa in Smod2.PluginManager.Manager.Server.GetPlayers())
			{
				if (StackMain.checkSteamIDItemNum.ContainsKey(playa.SteamId))
				{
					StackMain.checkSteamIDItemNum[playa.SteamId].Clear();
				}
			}
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId].Clear();
			}
		}
	}
}
