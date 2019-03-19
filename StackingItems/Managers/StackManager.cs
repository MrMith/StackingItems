using System;
using System.Linq;
using System.Collections.Generic;
using Smod2.API;

namespace StackingItems.Managers
{
	class StackManager
	{
		public Dictionary<int, int> checkItemForItemStack = new Dictionary<int, int>();

		public int GetStackSize(int ItemType)
		{
			if(checkItemForItemStack.TryGetValue(ItemType, out int value) && !ContainsWeapon(ItemType))
			{
				return value;
			}
			else
			{
				return 1;
			}
		}

		public void SetStackSize()
		{
			int globalstacksize = StackMain.plugin.GetConfigInt("si_globalstacksize");
			if (globalstacksize != 1)
			{
				foreach (ItemType type in (Smod2.API.ItemType[])Enum.GetValues(typeof(Smod2.API.ItemType)))
				{
					if(ContainsWeapon((int)type))
					{
						checkItemForItemStack[(int)type] = globalstacksize;
					}
					else
					{
						checkItemForItemStack[(int)type] = globalstacksize;
					}
					
				}
				foreach (KeyValuePair<int, int> item in StackMain.plugin.GetConfigIntDict("si_globaldict"))
				{
					if (ContainsWeapon(item.Key))
					{
						checkItemForItemStack[item.Key] = 1;
					}
					else
					{
						checkItemForItemStack[item.Key] = item.Value;
					}
					
				}
			}
			else
			{
				foreach (KeyValuePair<int, int> item in StackMain.plugin.GetConfigIntDict("si_globaldict")) //heck away lazerdude this only gets called on round start
				{
					if (ContainsWeapon(item.Key))
					{
						checkItemForItemStack[item.Key] = 1;
					}
					else
					{
						checkItemForItemStack[item.Key] = item.Value;
					}
				}
			}
		}

		public bool ContainsWeapon(int weaponID)
		{
			int[] weaponList = { 13, 16, 19, 20, 21, 23, 24, 30 };
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
	}
}
