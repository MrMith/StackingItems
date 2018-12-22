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
	class StackEventHandler : IEventHandlerMedkitUse, IEventHandlerPlayerPickupItemLate, IEventHandlerThrowGrenade, IEventHandlerPlayerDropItem, IEventHandlerPlayerDie, IEventHandlerRoundStart,IEventHandlerSetRole, IEventHandlerUpdate
	{
		private readonly Plugin plugin;

		public StackEventHandler(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public void OnPlayerDropItem(PlayerDropItemEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.Item.ItemType) >= 1)
				{
					int stackSize;
					if (StackMain.Stack_KeycardOverride != -1 && ev.Item.ToString().ToLower().Contains("keycard"))
					{
						stackSize = StackMain.Stack_KeycardOverride;
					}
					else if (StackMain.Stack_WeaponOverride != -1 && ContainsWeapon((int)ev.Item.ItemType))
					{
						stackSize = StackMain.Stack_WeaponOverride;
					}
					else
					{
						stackSize = StackMain.GetStackSize((int)ev.Item.ItemType);
					}

					StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ev.Item.ItemType, -1);

					int ItemAmount = StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.Item.ItemType);

					if (ItemAmount % stackSize == 0)
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
			else
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId] = new StackMain.StackCheckSteamIDsforItemInts();
				foreach (Smod2.API.Item item in ev.Player.GetInventory())
				{
					if(ev.Item != item)
					{
						StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)item.ItemType, 1);
					}
				}
			}
		}

		public void OnPlayerPickupItemLate(PlayerPickupItemLateEvent ev)
		{
			if (!StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId] = new StackMain.StackCheckSteamIDsforItemInts();
				foreach(Smod2.API.Item item in ev.Player.GetInventory())
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)item.ItemType, 1);
				}
			}

			int stackSize;

			if(StackMain.Stack_KeycardOverride != -1 && ev.Item.ToString().ToLower().Contains("keycard"))
			{
				stackSize = StackMain.Stack_KeycardOverride;
			}
			else if(StackMain.Stack_WeaponOverride != -1 && ContainsWeapon((int)ev.Item.ItemType))
			{
				stackSize = StackMain.Stack_WeaponOverride;
			}
			else
			{
				stackSize = StackMain.GetStackSize((int)ev.Item.ItemType);
			}
			plugin.Info(ev.Item.ItemType +"");
			plugin.Info(stackSize + "");
			if (StackMain.fixUseMedKit && StackMain.fixthrowGrenade && stackSize >= 2 && !ev.Item.ToString().ToLower().Contains("dropped"))
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ev.Item.ItemType, 1);
				int ItemAmount = StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.Item.ItemType);
				if (ItemAmount % stackSize != 1)
				{
					ev.Item.Remove();
				}
			}
		}

		public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.GrenadeType) >= 1)
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ev.GrenadeType,-1);
					if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.GrenadeType) % StackMain.GetStackSize((int)ev.GrenadeType) != 0)
					{
						StackMain.fixthrowGrenade = false;
						ev.Player.GiveItem(ev.GrenadeType);
						StackMain.fixthrowGrenade = true; //Looks ugly but is needed so it doesn't add one to my stacking system.
					}
				}
			}
			else
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId] = new StackMain.StackCheckSteamIDsforItemInts();
				bool AlreadySeenThisItem = false;
				foreach (Smod2.API.Item item in ev.Player.GetInventory())
				{
					if (ev.GrenadeType != item.ItemType || AlreadySeenThisItem)
					{
						StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)item.ItemType, 1);
					}
					else
					{
						AlreadySeenThisItem = true;
					}
				}
			}
		}

		public void OnMedkitUse(PlayerMedkitUseEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ItemType.MEDKIT) >= 1)
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ItemType.MEDKIT, -1);
					if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ItemType.MEDKIT) % StackMain.GetStackSize((int)ItemType.MEDKIT) != 0)
					{
						StackMain.fixUseMedKit = false;
						ev.Player.GiveItem(ItemType.MEDKIT);
						StackMain.fixUseMedKit = true; //Looks ugly but is needed to it doesn't add one to my stacking system. 
					}
				}
			}
			else
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId] = new StackMain.StackCheckSteamIDsforItemInts();
				bool AlreadySeenThisItem = false;
				foreach (Smod2.API.Item item in ev.Player.GetInventory())
				{
					if (Smod2.API.ItemType.MEDKIT != item.ItemType || AlreadySeenThisItem)
					{
						StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)item.ItemType, 1);
					}
					else
					{
						AlreadySeenThisItem = true;
					}
				}
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				foreach (Smod2.API.ItemType item in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
				{
					if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item) != -1)
					{
						int stackSize;
						if (StackMain.Stack_KeycardOverride != -1 && item.ToString().ToLower().Contains("keycard"))
						{
							stackSize = StackMain.Stack_KeycardOverride;
						}
						else if (StackMain.Stack_WeaponOverride != -1 && ContainsWeapon((int)item))
						{
							stackSize = StackMain.Stack_WeaponOverride;
						}
						else
						{
							stackSize = StackMain.GetStackSize((int)item);
						}
						for (int i = 0; i < (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item) - Math.Ceiling((float)StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item) / (float)stackSize)); i++)
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
			if (plugin.GetConfigBool("stack_disable"))
			{
				Smod2.PluginManager.Manager.DisablePlugin(plugin.Details.id);
				return;
			}
			StackMain.SetStackSize();
			StackMain.Stack_KeycardOverride = plugin.GetConfigInt("stack_override_keycard");
			StackMain.Stack_WeaponOverride = plugin.GetConfigInt("stack_override_weapons");
			foreach (Player playa in Smod2.PluginManager.Manager.Server.GetPlayers())
			{
				if (StackMain.checkSteamIDItemNum.ContainsKey(playa.SteamId))
				{
					StackMain.checkSteamIDItemNum[playa.SteamId].ResetToZero();
				}
				else
				{
					StackMain.checkSteamIDItemNum[playa.SteamId] = new StackMain.StackCheckSteamIDsforItemInts();
					foreach (Smod2.API.Item item in playa.GetInventory())
					{
						StackMain.checkSteamIDItemNum[playa.SteamId].AddItemAmount((int)item.ItemType, 1);
					}
				}
			}
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId].ResetToZero();
			}
			else
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId] = new StackMain.StackCheckSteamIDsforItemInts();
				foreach (Smod2.API.Item item in ev.Player.GetInventory())
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)item.ItemType, 1);
				}
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
							if (StackMain.checkSteamIDItemNum.ContainsKey(playa.SteamId))
							{
								foreach (Smod2.API.ItemType item in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
								{
									if (StackMain.checkSteamIDItemNum[playa.SteamId].GetItemAmount((int)item) != -1)
									{
										int stackSize;
										if (StackMain.Stack_KeycardOverride != -1 && item.ToString().ToLower().Contains("keycard"))
										{
											stackSize = StackMain.Stack_KeycardOverride;
										}
										else if (StackMain.Stack_WeaponOverride != -1 && ContainsWeapon((int)item))
										{
											stackSize = StackMain.Stack_WeaponOverride;
										}
										else
										{
											stackSize = StackMain.GetStackSize((int)item);
										}
										for (int i = 0; i < (StackMain.checkSteamIDItemNum[playa.SteamId].GetItemAmount((int)item) - Math.Ceiling((float)StackMain.checkSteamIDItemNum[playa.SteamId].GetItemAmount((int)item) / (float)stackSize)); i++)
										{
											Smod2.PluginManager.Manager.Server.Map.SpawnItem(item, playa.GetPosition(), new Vector(0, 0, 0));
										}
									}
								}
								StackMain.checkSteamIDItemNum[playa.SteamId].ResetToZero();
							}
							else
							{
								StackMain.checkSteamIDItemNum[playa.SteamId] = new StackMain.StackCheckSteamIDsforItemInts();
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
