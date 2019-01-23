using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System;
using System.Linq;
using System.Collections.Generic;
/*
 * If you're wondering why I'm casting floats on GetConfigInt its because if its set to 0 and I divide by 0 while its a int it will error but floats can handle divide by zero and I don't want the value to be a float by default.
 */

namespace StackingItems
{
	class StackEventHandler : IEventHandlerMedkitUse, IEventHandlerPlayerPickupItemLate, IEventHandlerThrowGrenade, IEventHandlerPlayerDropItem, IEventHandlerPlayerDie, IEventHandlerRoundStart, IEventHandlerSetRole, IEventHandlerCheckEscape, IEventHandlerUpdate
	{
		private readonly Plugin plugin;

		DateTime timeOnEvent = DateTime.Now;

		public StackEventHandler(Plugin plugin)
		{
			this.plugin = plugin;
		}

		#region OnPlayerDropItem
		/// <summary>
		/// Checks stacksize and then depending on the amount it either drops a item from your inventory or spawns one and removes one from the players count.
		/// </summary>
		public void OnPlayerDropItem(PlayerDropItemEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.Item.ItemType) >= 1)
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ev.Item.ItemType, -1);

					int ItemAmount = StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.Item.ItemType);

					if (ItemAmount % GetStackSizeOfItem((int)ev.Item.ItemType) == 0)
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
					if (ev.Item != item)
					{
						StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)item.ItemType, 1);
					}
				}
			}
		}
		#endregion

		#region OnPlayerPickupItemLate
		/// <summary>
		/// Checks stacksize and then depending on the amount it either adds one to inventory or deletes it and adds one to item count.
		/// </summary>
		public void OnPlayerPickupItemLate(PlayerPickupItemLateEvent ev)
		{
			if (!StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId] = new StackMain.StackCheckSteamIDsforItemInts();
				foreach (Smod2.API.Item item in ev.Player.GetInventory())
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)item.ItemType, 1);
				}
			}

			if (ev.Item.ItemType == ItemType.FRAG_GRENADE || ev.Item.ItemType == ItemType.FLASHBANG || ev.Item.ItemType == ItemType.MEDKIT)
			{
				if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].fixUseMedKit || StackMain.checkSteamIDItemNum[ev.Player.SteamId].fixthrowGrenade)
				{
					return;
				}
			}

			if (GetStackSizeOfItem((int)ev.Item.ItemType) >= 2 && !ev.Item.ToString().ToLower().Contains("dropped"))
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ev.Item.ItemType, 1);
				int ItemAmount = StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.Item.ItemType);
				if (ItemAmount % GetStackSizeOfItem((int)ev.Item.ItemType) != 1)
				{
					ev.Item.Remove();
				}
			}
		}
		#endregion

		#region OnThrowGrenade
		/// <summary>
		/// If a player has more grenades in their item count it gives them one and if not it just allows it to pass normally.
		/// </summary>
		public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.GrenadeType) >= 1)
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ev.GrenadeType, -1);
					if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.GrenadeType) % StackMain.GetStackSize((int)ev.GrenadeType) != 0)
					{
						StackMain.checkSteamIDItemNum[ev.Player.SteamId].fixthrowGrenade = true;
						ev.Player.GiveItem(ev.GrenadeType);
						StackMain.checkSteamIDItemNum[ev.Player.SteamId].fixthrowGrenade = false; //Looks ugly but is needed so it doesn't add one to my stacking system.
					}
				}
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
		#endregion

		#region OnMedkitUse
		/// <summary>
		/// If a player has more medkits in their item count it gives them one and if not it just allows it to pass normally.
		/// </summary>
		public void OnMedkitUse(PlayerMedkitUseEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ItemType.MEDKIT) >= 1)
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ItemType.MEDKIT, -1);
					if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ItemType.MEDKIT) % StackMain.GetStackSize((int)ItemType.MEDKIT) != 0)
					{
						StackMain.checkSteamIDItemNum[ev.Player.SteamId].fixUseMedKit = true;
						ev.Player.GiveItem(ItemType.MEDKIT);
						StackMain.checkSteamIDItemNum[ev.Player.SteamId].fixUseMedKit = false;
					}
				}
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
		#endregion

		#region OnPlayerDie
		/// <summary>
		/// When someone dies it drops all of their items in their itemcount.
		/// </summary>
		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				foreach (Smod2.API.ItemType item in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
				{
					if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item) != -1)
					{
						for (int i = 0; i < (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item) - Math.Ceiling((float)StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item) / (float)GetStackSizeOfItem((int)item))); i++)
						{
							Smod2.PluginManager.Manager.Server.Map.SpawnItem(item, ev.Player.GetPosition(), new Vector(0, 0, 0));
						}
					}
				}
				StackMain.checkSteamIDItemNum[ev.Player.SteamId].ResetToZero();
			}
		}
		#endregion

		#region OnRoundStart
		/// <summary>
		/// Sets all configs.
		/// </summary>
		public void OnRoundStart(RoundStartEvent ev)
		{
			if (plugin.GetConfigBool("si_disable"))
			{
				Smod2.PluginManager.Manager.DisablePlugin(plugin.Details.id);
				return;
			}

			StackMain.Stack_KeycardOverride = plugin.GetConfigInt("si_override_keycard");
			StackMain.keepItemsOnExtract = plugin.GetConfigBool("si_extract");
			StackMain.globalstacksize = plugin.GetConfigInt("si_globalstacksize");
			StackMain.SetStackSize();

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
		#endregion

		#region OnSetRole
		/// <summary>
		/// When someone is a different role it clears their stacks unless they have escaped.
		/// </summary>
		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].HasEscaped)
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].HasEscaped = false;

					StackMain.checkSteamIDItemNum[ev.Player.SteamId].ResetToZero();
					ev.Player.SetAmmo(AmmoType.DROPPED_9, StackMain.checkSteamIDItemNum[ev.Player.SteamId].ammo9 + ev.Player.GetAmmo(AmmoType.DROPPED_9));
					ev.Player.SetAmmo(AmmoType.DROPPED_7, StackMain.checkSteamIDItemNum[ev.Player.SteamId].ammo7 + ev.Player.GetAmmo(AmmoType.DROPPED_7));
					ev.Player.SetAmmo(AmmoType.DROPPED_5, StackMain.checkSteamIDItemNum[ev.Player.SteamId].ammo5 + ev.Player.GetAmmo(AmmoType.DROPPED_5));
					foreach (Smod2.API.ItemType item in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
					{
						if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].EscapedItemList.TryGetValue((int)item, out int value))
						{
							for (int i = 0; i < value; i++)
							{
								ev.Items.Add(item);
							}
						}
					}
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].EscapedItemList.Clear();
				}
				else
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].ResetToZero();
				}
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
		#endregion

		#region OnUpdate
		/// <summary>
		/// Checks every second to see if someone is handcuffed and if so they drop all of their items like if they've died.
		/// </summary>
		public void OnUpdate(UpdateEvent ev) // OnHandCuffed is broke >:(
		{
			if (DateTime.Now >= timeOnEvent)
			{
				timeOnEvent = DateTime.Now.AddSeconds(1.0);
				{
					foreach (Player playa in Smod2.PluginManager.Manager.Server.GetPlayers())
					{
						if (StackMain.checkSteamIDItemNum.ContainsKey(playa.SteamId) && playa.IsHandcuffed())
						{
							foreach (Smod2.API.ItemType item in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
							{
								if (StackMain.checkSteamIDItemNum[playa.SteamId].GetItemAmount((int)item) != -1)
								{
									for (int i = 0; i < (StackMain.checkSteamIDItemNum[playa.SteamId].GetItemAmount((int)item) - Math.Ceiling((float)StackMain.checkSteamIDItemNum[playa.SteamId].GetItemAmount((int)item) / (float)GetStackSizeOfItem((int)item))); i++)
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
		#endregion

		#region OnCheckEscape
		/// <summary>
		/// Checks if they have escaped to transfer item stacks and ammo over.
		/// </summary>
		public void OnCheckEscape(PlayerCheckEscapeEvent ev)
		{
			if (StackMain.checkSteamIDItemNum.ContainsKey(ev.Player.SteamId) && StackMain.keepItemsOnExtract)
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId].HasEscaped = true;
				foreach(KeyValuePair<int,int> kv in StackMain.checkSteamIDItemNum[ev.Player.SteamId].checkItemTypeForNumOfItems)
				{
					StackMain.checkSteamIDItemNum[ev.Player.SteamId].EscapedItemList[kv.Key] = kv.Value;
				}

				foreach(Item item in ev.Player.GetInventory())
				{
					if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item.ItemType) != 0) continue;
					if (StackMain.checkSteamIDItemNum[ev.Player.SteamId].EscapedItemList.ContainsKey((int)item.ItemType))
					{
						StackMain.checkSteamIDItemNum[ev.Player.SteamId].EscapedItemList[(int)item.ItemType]++;
					}
					else
					{
						StackMain.checkSteamIDItemNum[ev.Player.SteamId].EscapedItemList[(int)item.ItemType] = 1;
					}
					
				}

				StackMain.checkSteamIDItemNum[ev.Player.SteamId].ammo9 = ev.Player.GetAmmo(AmmoType.DROPPED_9);
				StackMain.checkSteamIDItemNum[ev.Player.SteamId].ammo7 = ev.Player.GetAmmo(AmmoType.DROPPED_7);
				StackMain.checkSteamIDItemNum[ev.Player.SteamId].ammo5 = ev.Player.GetAmmo(AmmoType.DROPPED_5);
			}
			else
			{
				StackMain.checkSteamIDItemNum[ev.Player.SteamId] = new StackMain.StackCheckSteamIDsforItemInts();
			}
		}
		#endregion

		#region ChecksIfWeaponOrKeycard
		/// <summary>
		/// Easy way to check if a itemtype is a weapon or keycard.
		/// </summary>
		public bool ContainsWeapon(int weaponID)
		{
			int[] weaponList = { 13, 16, 20, 21, 23, 24, 30 };
			if (weaponList.Contains(weaponID))
			{
				return true;
			}
			return false;
		}
		public bool ContainsKeycard(int keycardID)
		{
			int[] keycardList = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
			if (keycardList.Contains(keycardID))
			{
				return true;
			}
			return false;
		}
		#endregion

		public int GetStackSizeOfItem(int itemType)
		{
			if (StackMain.Stack_KeycardOverride != -1 && ContainsKeycard(itemType))
			{
				return StackMain.Stack_KeycardOverride;
			}

			if (ContainsWeapon(itemType))
			{
				return 1;
			}
			return StackMain.GetStackSize(itemType);
		}
	}
}
