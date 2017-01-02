using Assets.Scripts.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Interface.DTO
{
	public interface IModule
	{
		string Name { get; set; }
		ModuleType Type { get; set; }
		Color ModuleTexture { get; set; }
		string[] ActionNames { get; set; }
		int ActionCount { get; set; }
		int MaxActions { get; set; }
		IKit ParentKit { get; set; }
		
		IModule GetRightModule();
		IModule GetLeftModule();

		IAction GetCurrentAction();
		IAction GetLeftAction();
		IAction GetRightAction();

		void CycleActionLeft();
		void CycleActionRight();
	}
}
