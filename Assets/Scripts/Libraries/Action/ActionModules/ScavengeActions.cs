using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ActionModules
{
	public class ScavengeActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions { get; set; }
		string IActionLibraryModule.ModuleName { get { return "Scavenge"; } }

		public ScavengeActions()
		{
			Actions = new Dictionary<string, IExchangeAction>
			{
				{"StunField",new ExchangeAction
					(
					name: "Stun Field",
						attack: new Attack(baseDamage: 20, healthDrainModifier: -0.1f,  energyRecoilModifier: -1.8f),
						actionTexture: Resources.Load("ActionTextures/Purple") as Texture2D,
						primaryActionName: "StunField",
						cooldown: 1f
					)
				},
			};
		}
	}
}
