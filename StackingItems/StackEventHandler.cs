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
	internal class StackEventHandler : IEventHandlerMedkitUse, IEventHandlerPlayerPickupItemLate, IEventHandlerThrowGrenade, IEventHandlerPlayerDropItem, IEventHandlerPlayerDie, IEventHandlerSetRole, IEventHandlerCheckEscape, IEventHandlerUpdate, IEventHandlerSCP914Activate, IEventHandlerWaitingForPlayers
	{
		private readonly Plugin plugin;

		public SI_ConfigOptions SI_Config = new SI_ConfigOptions();
		public StackManager stackManager = new StackManager();
		public _914Manager _914Manager = new _914Manager();

		public static Dictionary<string, UserStackData> CheckSteamIDItemNum = new Dictionary<string, UserStackData>();

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
		/// <summary>
		/// Checks stacksize and then depending on the amount it either adds one to inventory or deletes it and adds one to item count.
		/// </summary>
		public void OnPlayerPickupItemLate(PlayerPickupItemLateEvent ev)
		{
			if (ev.Item.ToString().ToLower().Contains("dropped")) return;

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
		/// <summary>
		/// If a player has more grenades in their item count it gives them one and if not it just allows it to pass normally.
		/// </summary>
		public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
		{
			if (!CheckSteamIDItemNum.ContainsKey(ev.Player.SteamId))
			{
				CheckSteamIDItemNum[ev.Player.SteamId] = new UserStackData();
			}

			if (CheckSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.GrenadeType) >= 1)
			{
				CheckSteamIDItemNum[ev.Player.SteamId].AddItemAmount((int)ev.GrenadeType, -1);
				if (CheckSteamIDItemNum[ev.Player.SteamId].GetItemAmount((int)ev.GrenadeType) % stackManager.GetStackSize((int)ev.GrenadeType) != 0)
				{
					CheckSteamIDItemNum[ev.Player.SteamId].fixUse = true;
					ev.Player.GiveItem(ev.GrenadeType);
					CheckSteamIDItemNum[ev.Player.SteamId].fixUse = false; //Looks ugly but is needed so it doesn't add one to my stacking system.
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
		/// <summary>
		/// When someone dies it drops all of their items in their itemcount.
		/// </summary>
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
		/// <summary>
		/// Sets all configs.
		/// </summary>
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
		/// <summary>
		/// When someone is a different role it clears their stacks unless they have escaped.
		/// </summary>
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
						if (CheckSteamIDItemNum.ContainsKey(playa.SteamId) && playa.IsHandcuffed())
						{
							foreach (Smod2.API.ItemType item in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
							{
								if (CheckSteamIDItemNum[playa.SteamId].GetItemAmount((int)item) != -1)
								{
									for (int i = 0; i < (CheckSteamIDItemNum[playa.SteamId].GetItemAmount((int)item) - Math.Ceiling(CheckSteamIDItemNum[playa.SteamId].GetItemAmount((int)item) / (float)stackManager.GetStackSize((int)item))); i++)
									{
										Smod2.PluginManager.Manager.Server.Map.SpawnItem(item, playa.GetPosition(), new Vector(0, 0, 0));
									}
								}
							}
							CheckSteamIDItemNum[playa.SteamId].ResetToZero();
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
	}
}
