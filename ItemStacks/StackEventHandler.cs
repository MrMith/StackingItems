using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System;
using System.Linq;

/*
 * If you're wondering why I'm casting floats on GetConfigInt its because if its set to 0 and I divide by 0 while its a int it will error but floats can handle divide by zero.
 */

namespace itemStacks
{
	class StackEventHandler : IEventHandlerMedkitUse, IEventHandlerPlayerPickupItem, IEventHandlerThrowGrenade, IEventHandlerPlayerDropItem, IEventHandlerPlayerDie, IEventHandlerRoundStart,IEventHandlerSetRole, IEventHandlerUpdate
	{
		private readonly Plugin plugin;
		static string configStack;

		StackMain.StackCheckSteamIDsforItemInts value;

		public StackEventHandler(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public void OnPlayerDropItem(PlayerDropItemEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.TryGetValue(ev.Player.SteamId, out value))
			{
				if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.Item.ItemType) >= 1)
				{
					if (plugin.GetConfigInt("stack_override_keycard") != -1 && ev.Item.ToString().ToLower().Contains("keycard"))
					{
						configStack = "stack_override_keycard";
					}
					else if (plugin.GetConfigInt("stack_override_weapons") != -1 && ContainsWeapon((int)ev.Item.ItemType))
					{
						configStack = "stack_override_weapons";
					}
					else
					{
						configStack = ("stack_" + ev.Item.ToString().ToLower() + "_limit").Replace(" (inventory)", "");
					}

					StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ev.Item.ItemType, -1);
					int ItemAmount = StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.Item.ItemType);
					if (ItemAmount % plugin.GetConfigInt(configStack) == 0)
					{
						ev.Allow = true;
					}
					else
					{
						Smod2.PluginManager.Manager.Server.Map.SpawnItem(ev.Item.ItemType, ev.Player.GetPosition(), new Vector(0, 0, 0));
						ev.Allow = false;
					}
				}
			}
		}

		public void OnPlayerPickupItem(PlayerPickupItemEvent ev)
		{
			if (!StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId] = new StackMain.StackCheckSteamIDsforItemInts();
			}
			if(plugin.GetConfigInt("stack_override_keycard") != -1 && ev.Item.ToString().ToLower().Contains("keycard"))
			{
				configStack = "stack_override_keycard";
			}
			else if(plugin.GetConfigInt("stack_override_weapons") != -1 && ContainsWeapon((int)ev.Item.ItemType))
			{
				configStack = "stack_override_weapons";
			}
			else
			{
				configStack = ("stack_" + ev.Item.ToString().ToLower() + "_limit").Replace(" (inventory)", "");
			}

			if (StackMain.fixUseMedKit && StackMain.fixthrowGrenade && plugin.GetConfigInt(configStack) >= 2 && !ev.Item.ToString().ToLower().Contains("dropped"))
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ev.Item.ItemType, 1);
				int ItemAmount = StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.Item.ItemType);
				if (ItemAmount % plugin.GetConfigInt(configStack) == 1)
				{
					ev.Allow = true;
				}
				else
				{
					ev.Allow = false;
				}
			}
		}

		public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.TryGetValue(ev.Player.SteamId, out value))
			{
				if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.GrenadeType) >= 1)
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ev.GrenadeType,-1);
					if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.GrenadeType) % plugin.GetConfigInt("stack_" + ev.GrenadeType.ToString().ToLower() + "_limit") != 0)
					{
						StackMain.fixthrowGrenade = false;
						ev.Player.GiveItem(ev.GrenadeType);
						StackMain.fixthrowGrenade = true; //Looks ugly but is needed so it doesn't add one to my stacking system.
					}
				}
			}
		}

		public void OnMedkitUse(PlayerMedkitUseEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.TryGetValue(ev.Player.SteamId, out value))
			{
				if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ItemType.MEDKIT) >= 1)
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ItemType.MEDKIT, -1);
					if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ItemType.MEDKIT) % plugin.GetConfigInt("stack_" + ItemType.MEDKIT.ToString().ToLower() + "_limit") != 0)
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
				foreach (Smod2.API.ItemType item in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
				{
					if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item) != -1)
					{
						if (plugin.GetConfigInt("stack_override_keycard") != -1 && item.ToString().ToLower().Contains("keycard"))
						{
							configStack = "stack_override_keycard";
						}
						else if (plugin.GetConfigInt("stack_override_weapons") != -1 && ContainsWeapon((int)item))
						{
							configStack = "stack_override_weapons";
						}
						else
						{
							configStack = ("stack_" + item.ToString().ToLower() + "_limit").Replace(" (inventory)", "");
						}
						for (int i = 0; i < (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item) - Math.Ceiling((float)StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item) / (float)plugin.GetConfigInt(configStack))); i++)
						{
							Smod2.PluginManager.Manager.Server.Map.SpawnItem(item, ev.Player.GetPosition(), new Vector(0, 0, 0));
						}
					}
				}
				StackMain.checkSteamIDItemNum[ev.Player.SteamId].ResetToZero();
			}
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			foreach (Player playa in Smod2.PluginManager.Manager.Server.GetPlayers())
			{
				if (StackMain.checkSteamIDItemNum.ContainsKey(playa.SteamId))
				{
					StackMain.checkSteamIDItemNum[playa.SteamId].ResetToZero();
				}
			}
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId].ResetToZero();
			}
		}
		
		public void OnUpdate(UpdateEvent ev) // OnHandCuffed is broke >:(
		{
			DateTime timeOnEvent = DateTime.Now;
			if (DateTime.Now >= timeOnEvent)
			{
				timeOnEvent = DateTime.Now.AddSeconds(1.0);
				{
					foreach (Player playa in Smod2.PluginManager.Manager.Server.GetPlayers())
					{
						if (playa.IsHandcuffed())
						{
							if (StackMain.checkSteamIDItemNum.TryGetValue(playa.SteamId, out value))
							{
								foreach (Smod2.API.ItemType item in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
								{
									if (StackMain.checkSteamIDItemNum[playa.SteamId].GetItemAmount((int)item) != -1)
									{
										if (plugin.GetConfigInt("stack_override_keycard") != -1 && item.ToString().ToLower().Contains("keycard"))
										{
											configStack = "stack_override_keycard";
										}
										else if (plugin.GetConfigInt("stack_override_weapons") != -1 && ContainsWeapon((int)item))
										{
											configStack = "stack_override_weapons";
										}
										else
										{
											configStack = ("stack_" + item.ToString().ToLower() + "_limit").Replace(" (inventory)", "");
										}
										for (int i = 0; i < (StackMain.checkSteamIDItemNum[playa.SteamId].GetItemAmount((int)item) - Math.Ceiling((float)StackMain.checkSteamIDItemNum[playa.SteamId].GetItemAmount((int)item) / (float)plugin.GetConfigInt(configStack))); i++)
										{
											Smod2.PluginManager.Manager.Server.Map.SpawnItem(item, playa.GetPosition(), new Vector(0, 0, 0));
										}
									}
								}
								StackMain.checkSteamIDItemNum[playa.SteamId].ResetToZero();
							}
						}
					}
				}
			}
		}

		public bool ContainsWeapon(int weaponID)
		{
			int[] weaponList = {13,16,20,21,23,24};
			if (weaponList.Contains(weaponID))
			{
				return true;
			}
			return false;
		}
	}
}
