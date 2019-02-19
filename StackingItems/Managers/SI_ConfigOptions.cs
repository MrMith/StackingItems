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
		public int[] si_914itemstosave;

		public void SetConfigOptions()
		{
			Stack_KeycardOverride = StackMain.plugin.GetConfigInt("si_override_keycard");
			keepItemsOnExtract = StackMain.plugin.GetConfigBool("si_extract");
			globalstacksize = StackMain.plugin.GetConfigInt("si_globalstacksize");
			si_914enable = StackMain.plugin.GetConfigBool("si_914enable");
			si_914handorinv = StackMain.plugin.GetConfigInt("si_914handorinv");
			si_914itemstosave = StackMain.plugin.GetConfigIntList("si_914itemstosave");
		}
	}
}
