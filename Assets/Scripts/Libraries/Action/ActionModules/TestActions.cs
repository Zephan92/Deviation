using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Interface.DTO;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ActionModules
{
	public class TestActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions { get; set; }
		string IActionLibraryModule.ModuleName { get { return "Test"; } }

		public TestActions()
		{
			Actions = new Dictionary<string, IExchangeAction>
			{
				{"OneHitKO",new ExchangeAction//move this to an appropriate class named TestActionLibrary
					(
					name: "BOOOOOOM",
						attack: new Attack(baseDamage: 0, energyRecoilModifier: 0f),
						actionTexture: Resources.Load("ActionTextures/Red") as Texture2D,
						primaryActionName: "OneHitKO",
						cooldown: 0f
					)
				},
			};
		}
	}
}
