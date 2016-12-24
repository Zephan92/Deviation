using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Enum;
using UnityEngine;

namespace Assets.Editor.DTO
{
	public class ModuleStub : IModule
	{
		public IAction CurrentAction { get; set; }

		public ModuleStub(IAction action, string name, string[] actionNames, ModuleType type, Color moduleTexture, int maxActions)
		{
			Name = name;
			Type = type;
			ModuleTexture = moduleTexture;
			MaxActions = maxActions;
			CurrentAction = action;
		}

		//Module Name
		public string Name { get; set; }

		//This is the Module Type
		public ModuleType Type { get; set; }

		//this is the module ui texture use in the player controls
		public Color ModuleTexture { get; set; }

		//this is the current number of actions on this module
		public int ActionCount { get { return 1; } set { ActionCount = value; } }

		//this is the parent kit
		public IKit ParentKit { get; set; }

		//this is the max number of actions allowed on this module
		public int MaxActions { get; set; }

		public void CycleActionLeft()
		{
		}

		public void CycleActionRight()
		{
		}

		public IAction GetCurrentAction()
		{
			return CurrentAction; 
		}

		public IAction GetLeftAction()
		{
			return CurrentAction;
		}

		public IModule GetLeftModule()
		{
			return this;
		}

		public IAction GetRightAction()
		{
			return CurrentAction;
		}

		public IModule GetRightModule()
		{
			return this;
		}
	}
}
