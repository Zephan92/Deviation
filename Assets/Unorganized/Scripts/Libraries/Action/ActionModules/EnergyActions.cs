using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ActionModules
{
	public class EnergyActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions { get; set; }
		public ModuleType Type { get { return ModuleType.Energy; } }

		public EnergyActions()
		{
			Actions = new Dictionary<string, IExchangeAction>
			{
				
			};
		}
	}
}
