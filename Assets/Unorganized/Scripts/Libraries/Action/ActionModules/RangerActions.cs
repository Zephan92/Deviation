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
	public class RangerActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions { get; set; }
		public ModuleType Type { get { return ModuleType.Ranger; } }

		public RangerActions()
		{
			Actions = new Dictionary<string, IExchangeAction>
			{
				{"SmallProjectile",new ExchangeAction
					(
						name: "Small Projectile",
						attack: new Attack(baseDamage: 1, healthDrainModifier: -1f, energyRecoilModifier: -0.0f),
						actionTexture: Resources.Load("ActionTextures/Red") as Texture2D,
						primaryActionName: "SmallProjectile",
						cooldown: 0.1f,
						type: Type
					)
				},
				{"MediumProjectile",new ExchangeAction
					(
						name: "Medium Projectile",
						attack: new Attack(baseDamage: 5, healthDrainModifier: -1f, energyRecoilModifier: -0.0f),
						actionTexture: Resources.Load("ActionTextures/Red") as Texture2D,
						primaryActionName: "MediumProjectile",
						cooldown: 0.5f,
						type: Type
					)
				},
				{"LargeProjectile",new ExchangeAction
					(
						name: "LargeProjectile",
						attack: new Attack(baseDamage: 10, healthDrainModifier: -1f, energyRecoilModifier: -0.0f),
						actionTexture: Resources.Load("ActionTextures/Red") as Texture2D,
						primaryActionName: "LargeProjectile",
						cooldown: 1.0f,
						type: Type
					)
				},
			};
		}
	}
}
