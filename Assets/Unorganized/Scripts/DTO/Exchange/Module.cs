using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Library;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.DTO.Exchange
{

	public class Module : IModule
	{
		//Module Name
		public string Name { get; set; }

		//This is the Module Type
		public ModuleType Type { get; set; }

		//this is the module ui texture use in the player controls
		public Texture2D ModuleTexture { get; set; }

		//this is the current number of actions on this module
		public int ActionCount { get; set; }

		//this is the parent kit
		public IKit ParentKit { get; set; }

		//this is the max number of actions allowed on this module
		public int MaxActions { get; set; }

		public string[] ActionNames { get; set; }

		public IExchangeAction[] Actions { get; set; }

		public Module(string name, string[] actionNames, ModuleType type, Texture2D moduleTexture)
		{
			Name = name;
			Type = type;
			ModuleTexture = moduleTexture;
			ActionNames = actionNames;
			Actions = new IExchangeAction [4];
			ActionCount = 0;

			//for each action name, find corresponding action in the Action Library Table
			foreach (string actionName in actionNames)
			{
				if (ActionLibrary.GetActionLibraryTable().ContainsKey(actionName))
				{
					Actions[ActionCount] = ActionLibrary.GetActionInstance(actionName);
				}
				else
				{
					Actions[ActionCount] = ActionLibrary.GetActionInstance("default");
					Debug.LogError(name + " - Module: The \"" + actionName + "\" Action was not in the Action Dictionary");
				}

				Actions[ActionCount].ParentModule = this;
				ActionCount++;
			}
		}
	}
	
}