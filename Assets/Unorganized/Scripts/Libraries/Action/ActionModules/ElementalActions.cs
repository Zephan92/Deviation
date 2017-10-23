using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Interface.DTO;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.Enum;

namespace Assets.Scripts.Library.Action.ActionModules
{
	public class ElementalActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions { get; set; }

		public ModuleType Type { get{return ModuleType.Elemental;}}

		public ElementalActions()
		{
			Actions = new Dictionary<string, IExchangeAction>
			{
				{"Avalanche",new ExchangeAction
					(
					name: "Avalanche",
						attack: new Attack(baseDamage: 30, healthDrainModifier: -0.1f, energyRecoilModifier: -2.5f),
						actionTexture: Resources.Load("ActionTextures/Pink") as Texture2D,
						primaryActionName: "Avalanche",
						cooldown: 1f,
						type: Type
					)
				},
				{"FireBlast",new ExchangeAction
					(
					name: "Fire Blast",
						attack: new Attack(baseDamage: 80, energyRecoilModifier: 1f),
						actionTexture: Resources.Load("ActionTextures/Red") as Texture2D,
						primaryActionName: "FireBlast",
						cooldown: 0f,
						type: Type
					)
				},
				{"Tremor", new ExchangeAction
					(
					name: "Tremor",
						attack: new Attack(baseDamage: 20, energyRecoilModifier: -1.8f),
						actionTexture: Resources.Load("ActionTextures/Purple") as Texture2D,
						primaryActionName: "Tremor",
						cooldown: 1f,
						type: Type
					)
				},
				{"ShockWave", new ExchangeAction
					(
					name: "ShockWave",
						attack: new Attack(baseDamage: 20, healthDrainModifier: -0.6f,  energyRecoilModifier: -1.8f),
						actionTexture: Resources.Load("ActionTextures/Purple") as Texture2D,
						primaryActionName: "ShockWave",
						cooldown: 1f,
						type: Type
					)
				},
			};
		}
	}
}
