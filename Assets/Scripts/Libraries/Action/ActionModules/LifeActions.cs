using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Interface.DTO;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ActionModules
{
	public class LifeActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions { get; set; }
		string IActionLibraryModule.ModuleName { get { return "Life"; } }

		public LifeActions()
		{
			Actions = new Dictionary<string, IExchangeAction>
			{
				{"Drain",new ExchangeAction
					(
					name: "Drain",
						attack: new Attack(baseDamage: 35, healthRecoilModifier: 0.2f, energyRecoilModifier: -1.2f),
						actionTexture: Resources.Load("ActionTextures/Blue") as Texture2D,
						primaryActionName: "Drain",
						cooldown: 3f
					)
				},
			};
		}
	}
}
