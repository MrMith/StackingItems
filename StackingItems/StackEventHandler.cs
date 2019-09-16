using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using StackingItems.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * If you're wondering why I'm casting floats on GetConfigInt its because if its set to 0 and I divide by 0 while its a int it will error but floats can handle divide by zero and I don't want the value to be a float by default.
*/

namespace StackingItems
{
	internal class StackEventHandler : IEventHandlerMedkitUse, IEventHandlerPlayerPickupItemLate, IEventHandlerThrowGrenade, IEventHandlerPlayerDropItem, IEventHandlerPlayerDie, IEventHandlerSetRole, IEventHandlerCheckEscape, IEventHandlerSCP914Activate, IEventHandlerWaitingForPlayers, IEventHandlerPlayerDropAllItems
	{
		private readonly Plugin plugin;

		public SI_ConfigOptions SI_Config = new SI_ConfigOptions();
		public StackManager stackManager = new StackManager();
		public _914Manager _914Manager = new _914Manager();

		public static Dictionary<string, UserStackData> CheckSteamIDItemNum = new Dictionary<string, UserStackData>();

		public StackEventHandler(Plugin plugin)
		{
			this.plugin = plugin;
		}

		#region OnPlayerDropItem
		public void OnPlayerDropItem(PlayerDropItemEvent ev)
		{
			if (CheckSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				if (CheckSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.Item.ItemType) >= 1)
				{
					CheckSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ev.Item.ItemType, -1);

					int ItemAmount = CheckSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.Item.ItemType);

					if (ItemAmount % stackManager.GetStackSize((int)ev.Item.ItemType) == 0)
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
				CheckSteamIDItemNum[ev.Player.SteamId] = new UserStackData();
			}
		}
		#endregion

		#region OnPlayerPickupItemLate
		public void OnPlayerPickupItemLate(PlayerPickupItemLateEvent ev)
		{
			if (ev.Item.ItemType == ItemType.DROPPED_5 || ev.Item.ItemType == ItemType.DROPPED_7 || ev.Item.ItemType == ItemType.DROPPED_9) return;

			if (!CheckSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				CheckSteamIDItemNum[ev.Player.SteamId] = new UserStackData();
			}

			if (ev.Item.ItemType == ItemType.FRAG_GRENADE || ev.Item.ItemType == ItemType.FLASHBANG || ev.Item.ItemType == ItemType.MEDKIT)
			{
				if (CheckSteamIDItemNum[ev.Player.SteamId].fixUse)
				{
					return;
				}
			}

			CheckSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ev.Item.ItemType, 1);
			if (stackManager.ContainsWeapon((int)ev.Item.ItemType)) return;
			int ItemAmount = CheckSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.Item.ItemType);

			if (ItemAmount % stackManager.GetStackSize((int)ev.Item.ItemType) != 1 && stackManager.GetStackSize((int)ev.Item.ItemType) >= 2)
			{
				ev.Item.Remove();
			}
		}
		#endregion

		#region OnThrowGrenade
		public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
		{
			if (!CheckSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				CheckSteamIDItemNum[ev.Player.SteamId] = new UserStackData();
			}

			int GrenadeTypeToUse = -1;

			if (ev.GrenadeType == GrenadeType.FLASHBANG)
			{
				GrenadeTypeToUse = (int)ItemType.FLASHBANG;
			}
			else if (ev.GrenadeType == GrenadeType.FRAG_GRENADE)
			{
				GrenadeTypeToUse = (int)ItemType.FRAG_GRENADE;
			}

			if (GrenadeTypeToUse == -1) return;

			if (CheckSteamIDItemNum[ev.Player.SteamId].GetItemAmount(GrenadeTypeToUse) >= 1)
			{
				CheckSteamIDItemNum[ev.Player.SteamId].AddItemAmount(GrenadeTypeToUse, -1);
				if (CheckSteamIDItemNum[ev.Player.SteamId].GetItemAmount(GrenadeTypeToUse) % stackManager.GetStackSize(GrenadeTypeToUse) != 0)
				{
					CheckSteamIDItemNum[ev.Player.SteamId].fixUse = true;
					ev.Player.GiveItem((ItemType)GrenadeTypeToUse);
					CheckSteamIDItemNum[ev.Player.SteamId].fixUse = false; //Looks ugly but is needed so it doesn't add one to my stacking system.
				}
			}
		}
		#endregion

		#region OnMedkitUse
		public void OnMedkitUse(PlayerMedkitUseEvent ev)
		{
			if (CheckSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				if (CheckSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ItemType.MEDKIT) >= 1)
				{
					CheckSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ItemType.MEDKIT, -1);
					if (CheckSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ItemType.MEDKIT) % stackManager.GetStackSize((int)ItemType.MEDKIT) != 0)
					{
						CheckSteamIDItemNum[ev.Player.SteamId].fixUse = true;
						ev.Player.GiveItem(ItemType.MEDKIT);
						CheckSteamIDItemNum[ev.Player.SteamId].fixUse = false;
					}
				}
			}
			else
			{
				CheckSteamIDItemNum[ev.Player.SteamId] = new UserStackData();
			}
		}
		#endregion

		#region OnPlayerDie
		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (CheckSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				foreach (Smod2.API.ItemType item in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
				{
					if (CheckSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item) != -1)
					{
						for (int i = 0; i < (CheckSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item) - Math.Ceiling(CheckSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item) / (float)stackManager.GetStackSize((int)item))); i++)
						{
							Smod2.PluginManager.Manager.Server.Map.SpawnItem(item, ev.Player.GetPosition(), new Vector(0, 0, 0));
						}
					}
				}
				CheckSteamIDItemNum[ev.Player.SteamId].ResetToZero();
				CheckSteamIDItemNum[ev.Player.SteamId].TempItemList.Clear();
				CheckSteamIDItemNum[ev.Player.SteamId].Escape = false;
			}
		}
		#endregion

		#region OnWaitingForPlayers
		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			if (plugin.GetConfigBool("si_disable"))
			{
				Smod2.PluginManager.Manager.DisablePlugin(plugin.Details.id);
				return;
			}

			SI_Config.SetConfigOptions();
			stackManager.SetStackSize();
			_914Manager.SetUp(SI_Config, stackManager);
		}
		#endregion

		#region OnSetRole
		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (CheckSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				if (CheckSteamIDItemNum[ev.Player.SteamId].Escape)
				{
					CheckSteamIDItemNum[ev.Player.SteamId].Escape = false;
					CheckSteamIDItemNum[ev.Player.SteamId].ApplyTransferedItems((GameObject)ev.Player.GetGameObject());
				}
				else
				{
					CheckSteamIDItemNum[ev.Player.SteamId].ResetToZero();
				}
			}
			else
			{
				CheckSteamIDItemNum[ev.Player.SteamId] = new UserStackData();
			}
		}
		#endregion

		#region OnCheckEscape
		public void OnCheckEscape(PlayerCheckEscapeEvent ev)
		{
			if (CheckSteamIDItemNum.ContainsKey(ev.Player.SteamId) && SI_Config.keepItemsOnExtract)
			{
				CheckSteamIDItemNum[ev.Player.SteamId].Escape = true;
				CheckSteamIDItemNum[ev.Player.SteamId].TransferItems((GameObject)ev.Player.GetGameObject());
			}
			else
			{
				CheckSteamIDItemNum[ev.Player.SteamId] = new UserStackData();
			}
		}
		#endregion

		#region 914
		public void OnSCP914Activate(SCP914ActivateEvent ev)
		{
			if (!SI_Config.si_914enable)
			{
				return;
			}
			Scp914 scp914 = GameObject.FindObjectOfType<Scp914>();
			if (scp914 == null)
			{
				plugin.Error("914 cannot be found.");
				return;
			}
			foreach (UnityEngine.Collider collider in ev.Inputs)
			{
				if (collider.GetComponent<Inventory>() != null)
				{
					Smod2.API.Player play = new ServerMod2.API.SmodPlayer(collider.gameObject);

					if (SI_Config.si_914handorinv == 1) // Don't know if passing the collider will work the way I want it to work so I'm just gonna pray to the unity gods for a bug free experience.
					{
						_914Manager.DoHand914(collider, ev.OutputPos, ev.KnobSetting, scp914);
					}
					else if (SI_Config.si_914handorinv == 2)
					{
						_914Manager.DoInventory914(collider, ev.OutputPos, ev.KnobSetting, scp914);
					}
					else if (SI_Config.si_914handorinv == 0)
					{
						CheckSteamIDItemNum[play.SteamId].TransferItems(collider.gameObject);
						CheckSteamIDItemNum[play.SteamId].ApplyTransferedItems(collider.gameObject,true);
					}
				}
			}
		}
		#endregion
		#region  OnPlayerDropAllItems
		public void OnPlayerDropAllItems(PlayerDropAllItemsEvent ev)
		{
			if (CheckSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				foreach (Smod2.API.ItemType item in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
				{
					if (CheckSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item) != -1)
					{
						for (int i = 0; i < (CheckSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item) - Math.Ceiling(CheckSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)item) / (float)stackManager.GetStackSize((int)item))); i++)
						{
							Smod2.PluginManager.Manager.Server.Map.SpawnItem(item, ev.Player.GetPosition(), new Vector(0, 0, 0));
						}
					}
				}
				CheckSteamIDItemNum[ev.Player.SteamId].ResetToZero();
			}
		}
		#endregion
	}
}
