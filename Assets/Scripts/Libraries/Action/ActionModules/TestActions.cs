using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ActionModules
{
	public class TestActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions { get; set; }
		public ModuleType Type { get { return ModuleType.Test; } }

		public TestActions()
		{
			Actions = new Dictionary<string, IExchangeAction>
			{
				{"OneHitKO",new ExchangeAction
					(
					name: "BOOOOOOM",
						attack: new Attack(baseDamage: 0, energyRecoilModifier: 0f),
						actionTexture: Resources.Load("ActionTextures/Red") as Texture2D,
						primaryActionName: "OneHitKO",
						cooldown: 0f,
						type: Type
					)
				},
			};
		}
	}
}
