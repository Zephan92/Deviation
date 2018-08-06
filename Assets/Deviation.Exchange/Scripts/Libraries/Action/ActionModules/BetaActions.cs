using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ActionModules
{
	public class EnergyActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions_ByName { get; set; }
		public Dictionary<Guid, IExchangeAction> Actions_ByGuid { get; set; }

		public TraderType Type { get { return TraderType.Beta; } }

		public EnergyActions()
		{
			Actions_ByName = new Dictionary<string, IExchangeAction>();
			Actions_ByGuid = new Dictionary<Guid, IExchangeAction>();

			List<IExchangeAction> actions = new List<IExchangeAction>()
			{
				new ExchangeAction
				(
					id: new Guid("33a4911e-73cb-4138-be21-f6728dd2756e"),
					name: "Drain",
					attack: new Attack(baseDamage: 35, healthRecoilModifier: 0.2f),
					actionTexture: Resources.Load("AbilityIcons/Default") as Texture2D,
					primaryActionName: "Drain",
					cooldown: 3f,
					type: Type
				),
				new ExchangeAction
				(
					id: new Guid("36a1cf13-8b79-4800-8574-7cec0c405594"),
					name: "Small Projectile",
					attack: new Attack(baseDamage: 1, healthDrainModifier: -1f),
					actionTexture: Resources.Load("AbilityIcons/Default") as Texture2D,
					primaryActionName: "SmallProjectile",
					cooldown: 0.1f,
					type: Type
				),
				new ExchangeAction
				(
					id: new Guid("ba75f986-1bf3-40e4-a528-34b802ea0608"),
					name: "Medium Projectile",
					attack: new Attack(baseDamage: 5, healthDrainModifier: -1f),
					actionTexture: Resources.Load("AbilityIcons/Default") as Texture2D,
					primaryActionName: "MediumProjectile",
					cooldown: 0.5f,
					type: Type
				),
				new ExchangeAction
				(
					id: new Guid("071d4f85-728e-41d6-95a3-31fa013a7289"),
					name: "Large Projectile",
					attack: new Attack(baseDamage: 10, healthDrainModifier: -1f),
					actionTexture: Resources.Load("AbilityIcons/Default") as Texture2D,
					primaryActionName: "LargeProjectile",
					cooldown: 1.0f,
					type: Type
				),
			};

			actions.ForEach(x => Actions_ByName.Add(x.Name, x));
			actions.ForEach(x => Actions_ByGuid.Add(x.Id, x));
		}
	}
}
