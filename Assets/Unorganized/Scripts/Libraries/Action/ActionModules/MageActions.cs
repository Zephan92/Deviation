using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ActionModules
{
	public class MageActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions { get; set; }
		public ModuleType Type { get { return ModuleType.Mage; } }

		public MageActions()
		{
			Actions = new Dictionary<string, IExchangeAction>
			{
				{"PortalRocket",new ExchangeAction
					(
					name: "Portal Rocket",
						attack: new Attack(baseDamage: 15, energyRecoilModifier: -3.0f),
						actionTexture: Resources.Load("ActionTextures/Yellow") as Texture2D,
						primaryActionName: "PortalAttack",
						cooldown: 1f,
						type: Type
					)
				},
				{"Teleport",new ExchangeAction
					(
						name: "Teleport",
						attack: new Attack(baseDamage: 10, healthDrainModifier: -0.0f),
						actionTexture: Resources.Load("ActionTextures/Purple") as Texture2D,
						primaryActionName: "Teleport",
						cooldown: 1f,
						type: Type
					)
				},
			};
		}
	}
}
