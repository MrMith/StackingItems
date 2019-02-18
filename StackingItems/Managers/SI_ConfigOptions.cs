using System.Collections.Generic;
using System;

namespace StackingItems.Managers
{
	public class SI_ConfigOptions
	{
		public int Stack_KeycardOverride;
		public bool keepItemsOnExtract;
		public int globalstacksize;
		public bool si_914enable;
		public int si_914handorinv;
		public Dictionary<Smod2.API.KnobSetting,Dictionary<int,int>> si_classtransfer = new Dictionary<Smod2.API.KnobSetting, Dictionary<int, int>>();
		public int[] si_914itemstosave;

		readonly string[] _914ClassChangeConfigSettings = { "scp914_rough_change_class", "scp914_coarse_change_class", "scp914_1_to_1_change_class", "scp914_fine_change_class" , "scp914_very_fine_change_class" };

		public void SetConfigOptions()
		{
			Stack_KeycardOverride = StackMain.plugin.GetConfigInt("si_override_keycard");
			keepItemsOnExtract = StackMain.plugin.GetConfigBool("si_extract");
			globalstacksize = StackMain.plugin.GetConfigInt("si_globalstacksize");

			Smod2.API.KnobSetting[] values = (Smod2.API.KnobSetting[])Enum.GetValues(typeof(Smod2.API.KnobSetting));
			for (int i = 0; i <= values.Length - 1; i++)
			{
				if (ConfigFile.GetRawString(_914ClassChangeConfigSettings[i]).Length != 0)
				{
					si_classtransfer[values[i]] = new Dictionary<int, int>();
					foreach (var temp in ConfigFile.GetDict(_914ClassChangeConfigSettings[i]))
					{
						si_classtransfer[values[i]].Add(int.Parse(temp.Key), int.Parse(temp.Value));
					}
				}
				else
				{
					si_classtransfer[values[i]] = new Dictionary<int, int>();
				}
			}
			
			si_914enable = StackMain.plugin.GetConfigBool("si_914enable");
			si_914handorinv = StackMain.plugin.GetConfigInt("si_914handorinv");
			si_914itemstosave = StackMain.plugin.GetConfigIntList("si_914itemstosave");
		}
	}
}
