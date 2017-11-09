using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ActionModules
{
	public class LifeActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions_ByName { get; set; }
		public Dictionary<Guid, IExchangeAction> Actions_ByGuid { get; set; }

		public ModuleType Type { get { return ModuleType.Life; } }

		public LifeActions()
		{
			Actions_ByName = new Dictionary<string, IExchangeAction>();
			Actions_ByGuid = new Dictionary<Guid, IExchangeAction>();

			List<IExchangeAction> actions = new List<IExchangeAction>()
			{
				new ExchangeAction
				(
					id: new Guid("33a4911e-73cb-4138-be21-f6728dd2756e"),
					name: "Drain",
					attack: new Attack(baseDamage: 35, healthRecoilModifier: 0.2f, energyRecoilModifier: -1.2f),
					actionTexture: Resources.Load("ActionTextures/Blue") as Texture2D,
					primaryActionName: "Drain",
					cooldown: 3f,
					type: Type
				)
			};

			actions.ForEach(x => Actions_ByName.Add(x.Name, x));
			actions.ForEach(x => Actions_ByGuid.Add(x.Id, x));
		}
	}
}
