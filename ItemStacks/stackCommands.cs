using Smod2.Commands;
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
}
