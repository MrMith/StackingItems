﻿using Smod2.Commands;
using Smod2;

namespace itemStacks
{
	class StackVersion : ICommandHandler
	{
		private Plugin plugin;

		public StackVersion(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Gets version for debugging";
		}

		public string GetUsage()
		{
			return "stack_version";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			return new string[] { "mith.itemstacks is version " + plugin.Details.version };
		}
	}

	class StackDisable : ICommandHandler
	{
		private Plugin plugin;

		public StackDisable(Plugin plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Enables or disables ItemStacks. Call stack_disable false to enable or stack_disable true to disable.";
		}

		public string GetUsage()
		{
			return "stack_disable";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			Smod2.PluginManager.Manager.DisablePlugin(plugin.Details.id);
			return new string[] {"Disabled " + plugin.Details.id};
		}
	}
}
