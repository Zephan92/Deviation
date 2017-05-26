using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ActionModules
{
	public class RangerActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions { get; set; }
		string IActionLibraryModule.ModuleName { get { return "Ranger"; } }

		public RangerActions()
		{
			Actions = new Dictionary<string, IExchangeAction>
			{
				{"FireRocket",new ExchangeAction
					(
						name: "Fire Rocket",
						attack: new Attack(baseDamage: 20, healthDrainModifier: -0.3f, energyRecoilModifier: -0.4f),
						actionTexture: Resources.Load("ActionTextures/Red") as Texture2D,
						primaryActionName: "FireRocket",
						cooldown: 1f
					)
				},
			};
		}
	}
}
