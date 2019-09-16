using Smod2;
using Smod2.Attributes;
using System.Collections.Generic;
using Smod2.Config;
using Smod2.API;
using Smod2.Piping;

namespace StackingItems
{
    [PluginDetails(
        author = "Mith",
        name = "StackingItems",
        description = "Items stack to save inventory space.",
        id = "mith.StackingItems",
        version = "1.2.0",
        configPrefix = "si",
        SmodMajor = 3,
        SmodMinor = 5,
        SmodRevision = 1
        )]
    class StackMain : Plugin
    {
        internal static StackMain plugin;
        StackEventHandler stackEvent;

        [ConfigOption]
        public int override_keycard = -1;

        [ConfigOption]
        public bool extract = true;

        [ConfigOption]
        public bool disable = false;

        [ConfigOption]
        public int globalstacksize = -1;

        [ConfigOption]
        public Dictionary<int, int> globaldict = new Dictionary<int, int>();

        [ConfigOption]
        public bool enable914 = true;

        [ConfigOption]
        public int handorinv914 = 0;

        [ConfigOption]
        public int[] itemstosave914 = new int[] { 12, 14, 15, 17, 18, 19 };

        [PipeMethod]
        public bool SetItemAmountForPlayer(ItemType itemType,int Amount, string PlayerSteamID)
        {
            if(stackEvent != null)
            {
                StackEventHandler.CheckSteamIDItemNum[PlayerSteamID].SetItemAmount((int)itemType, Amount);
            }
            return true;
        }

        [PipeMethod]
        public bool AddItemAmountForPlayer(ItemType itemType, int Amount, string PlayerSteamID)
        {
            StackEventHandler.CheckSteamIDItemNum[PlayerSteamID].AddItemAmount((int)itemType, Amount);
            return false;
        }

        [PipeMethod]
        public int GetItemAmountForPlayer(ItemType itemType, string PlayerSteamID)
        {
            return StackEventHandler.CheckSteamIDItemNum[PlayerSteamID].GetItemAmount((int)itemType);
        }

        [PipeMethod]
        public bool ClearAllItemStackInformation(string PlayerSteamID)
        {
            StackEventHandler.CheckSteamIDItemNum[PlayerSteamID].ResetToZero();
            return true;
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
            this.AddEventHandlers(stackEvent = new StackEventHandler(this));

            this.AddCommand("si_version", new StackVersion(this));
            this.AddCommand("si_disable", new StackDisable(this));
        }
	}
}