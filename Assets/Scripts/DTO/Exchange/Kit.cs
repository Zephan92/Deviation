using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using System;

namespace Assets.Scripts.Library
{
	public class Kit : IKit
	{
		//Kit Name
		public string Name { get; set; }

		//Current Module Count
		public int ModuleCount { get; set; }

		//Max Number of Modules this Kit can have
		public int MaxModules { get; set; }

		public IPlayer Player { get; set; }

		public string[] ModuleNames { get; set; }

		//List of Modules in this kit
		private LinkedList<IModule> _modules;

		//Current Module
		private LinkedListNode<IModule> _currentModule;

		public Kit(string name, string[] moduleNames,  int maxModules)
		{
			Name = name;
			MaxModules = maxModules;
			_modules = new LinkedList<IModule>();
			ModuleCount = 0;
			ModuleNames = moduleNames;
			//for each module named, find corresponding module in the Module Library Table
			foreach (string moduleName in moduleNames)
			{
				ModuleCount++;
				if (ModuleLibrary.ModuleLibraryTable.ContainsKey(moduleName))
				{
					SetModule(ModuleLibrary.GetModuleInstance(moduleName));
				}
				else
				{
					//otherwise module hasn't been implemented yet, use generic
					SetModule(ModuleLibrary.GetModuleInstance("default"));
					Debug.LogError(name + " - Kit: The \"" + moduleName + "\" Module was not in the Module Dictionary");
				}
			}
		}

		//adds a module to the linked list
		private void SetModule(IModule module)
		{
			module.ParentKit = this;

			if (_modules.First != null)
			{
				_modules.AddAfter(_modules.Last, module);
			}
			else
			{
				_modules.AddFirst(module);
				_currentModule = _modules.First;
			}
		}

		//return the current module
		public IModule GetCurrentModule()
		{
			return _currentModule.Value;
		}

		//returns the previous module
		public IModule GetLeftModule()
		{
			if (_currentModule.Previous != null)
			{
				return _currentModule.Previous.Value;
			}
			else
			{
				return _modules.Last.Value;
			}
		}

		//returns the next module
		public IModule GetRightModule()
		{
			if (_currentModule.Next != null)
			{
				return _currentModule.Next.Value;
			}
			else
			{
				return _modules.First.Value;
			}
		}

		//changes the current module to the left
		public void CycleModuleLeft()
		{
			if (_currentModule.Previous != null)
			{
				_currentModule = _currentModule.Previous;
			}
			else
			{
				_currentModule = _modules.Last;
			}
		}

		//changes the current module to the right
		public void CycleModuleRight()
		{
			if (_currentModule.Next != null)
			{
				_currentModule = _currentModule.Next;
			}
			else
			{
				_currentModule = _modules.First;
			}
		}
	}
}
