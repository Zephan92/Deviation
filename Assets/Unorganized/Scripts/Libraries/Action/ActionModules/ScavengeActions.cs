using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
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
		public ModuleType Type { get { return ModuleType.Scavenge; } }

		public ScavengeActions()
		{
			Actions = new Dictionary<string, IExchangeAction>
			{
				{"StunField",new ExchangeAction
					(
					name: "Stun Field",
						attack: new Attack(baseDamage: 20, healthDrainModifier: -0.6f,  energyRecoilModifier: -1.8f),
						actionTexture: Resources.Load("ActionTextures/Purple") as Texture2D,
						primaryActionName: "StunField",
						cooldown: 1f,
						type: Type
					)
				},
				{"Tremor",new ExchangeAction
					(
					name: "Tremor",
						attack: new Attack(baseDamage: 20, healthDrainModifier: -0.6f,  energyRecoilModifier: -1.8f),
						actionTexture: Resources.Load("ActionTextures/Purple") as Texture2D,
						primaryActionName: "Tremor",
						cooldown: 1f,
						type: Type
					)
				},
			};
		}
	}
}
