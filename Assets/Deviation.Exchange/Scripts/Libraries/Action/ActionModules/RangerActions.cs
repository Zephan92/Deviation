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
		public Dictionary<string, IExchangeAction> Actions_ByName { get; set; }
		public Dictionary<Guid, IExchangeAction> Actions_ByGuid { get; set; }

		public ModuleType Type { get { return ModuleType.Ranger; } }

		public RangerActions()
		{
			Actions_ByName = new Dictionary<string, IExchangeAction>();
			Actions_ByGuid = new Dictionary<Guid, IExchangeAction>();

			List<IExchangeAction> actions = new List<IExchangeAction>()
			{
				new ExchangeAction
				(
					id: new Guid("36a1cf13-8b79-4800-8574-7cec0c405594"),
					name: "Small Projectile",
					attack: new Attack(baseDamage: 1, healthDrainModifier: -1f, energyRecoilModifier: -0.0f),
					actionTexture: Resources.Load("AbilityIcons/Default") as Texture2D,
					primaryActionName: "SmallProjectile",
					cooldown: 0.1f,
					type: Type
				),
				new ExchangeAction
				(
					id: new Guid("ba75f986-1bf3-40e4-a528-34b802ea0608"),
					name: "Medium Projectile",
					attack: new Attack(baseDamage: 5, healthDrainModifier: -1f, energyRecoilModifier: -0.0f),
					actionTexture: Resources.Load("AbilityIcons/Default") as Texture2D,
					primaryActionName: "MediumProjectile",
					cooldown: 0.5f,
					type: Type
				),
				new ExchangeAction
				(
					id: new Guid("071d4f85-728e-41d6-95a3-31fa013a7289"),
					name: "LargeProjectile",
					attack: new Attack(baseDamage: 10, healthDrainModifier: -1f, energyRecoilModifier: -0.0f),
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
