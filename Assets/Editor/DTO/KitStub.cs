using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Editor.DTO
{
	public class KitStub : IKit
	{
		private IModule CurrentModule;

		public KitStub(IModule module, string name, string[] moduleNames, int maxModules)
		{
			CurrentModule = module;
			Name = name;
			MaxModules = maxModules;
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
