using System.Collections.Generic;
using System;
using UnityEngine;

namespace Assets.Scripts.Library
{
	public class Kit
	{
		//Kit Name
		public string Name;

		//Current Module Count
		public int ModuleCount;

		//Max Number of Modules this Kit can have
		private int _maxModules;

		//List of Modules in this kit
		private LinkedList<Module> _modules;

		//Current Module
		private LinkedListNode<Module> _currentModule;

		public Kit(string name, string[] moduleNames,  int maxModules)
		{
			Name = name;
			_maxModules = maxModules;
			_modules = new LinkedList<Module>();
			ModuleCount = 0;

			//for each module named, find corresponding module in the Module Library Table
			foreach (string moduleName in moduleNames)
			{
				ModuleCount++;
				if (ModuleLibrary.ModuleLibraryTable.ContainsKey(moduleName))
				{
					SetModule(ModuleLibrary.ModuleLibraryTable[moduleName]);
				}
				else
				{
					//otherwise module hasn't been implemented yet, use generic
					SetModule(ModuleLibrary.ModuleLibraryTable["default"]);
					Debug.LogError(name + " - Kit: The \"" + moduleName + "\" Module was not in the Module Dictionary");
				}
			}
		}

		//adds a module to the linked list
		private void SetModule(Module module)
		{
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
		public Module GetCurrentModule()
		{
			return _currentModule.Value;
		}

		//returns the previous module
		public Module GetLeftModule()
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
		public Module GetRightModule()
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
