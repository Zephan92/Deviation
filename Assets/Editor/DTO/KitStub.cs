using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;

namespace Assets.Editor.DTO
{
	public class KitStub : IKit
	{
		private IModule CurrentModule;
		public IPlayer Player { get; set; }
		public string[] ModuleNames { get; set; }


		public KitStub(IModule module, string name, string[] moduleNames, int maxModules)
		{
			CurrentModule = module;
			Name = name;
			MaxModules = maxModules;
			ModuleNames = moduleNames;
		}

		public string Name { get; set; }

		//Current Module Count
		public int ModuleCount { get { return 1; } set { ModuleCount = value; } }

		//Max Number of Modules this Kit can have
		public int MaxModules { get; set; }

		public void CycleModuleLeft()
		{
		}

		public void CycleModuleRight()
		{
		}

		public IModule GetCurrentModule()
		{
			return CurrentModule;
		}

		public IModule GetLeftModule()
		{
			return CurrentModule;
		}

		public IModule GetRightModule()
		{
			return CurrentModule;
		}
	}
}
