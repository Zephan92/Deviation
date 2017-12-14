using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ActionModules
{
	public class MageActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions_ByName { get; set; }
		public Dictionary<Guid, IExchangeAction> Actions_ByGuid { get; set; }

		public ModuleType Type { get { return ModuleType.Mage; } }

		public MageActions()
		{
			Actions_ByName = new Dictionary<string, IExchangeAction>();
			Actions_ByGuid = new Dictionary<Guid, IExchangeAction>();

			List<IExchangeAction> actions = new List<IExchangeAction>()
			{
				new ExchangeAction
				(
					id: new Guid("bc98cc04-14fc-428c-a295-4fe3fc4e7c3a"),
					name: "Portal Rocket",
					attack: new Attack(baseDamage: 15, energyRecoilModifier: -3.0f),
					actionTexture: Resources.Load("AbilityIcons/Default") as Texture2D,
					primaryActionName: "PortalAttack",
					cooldown: 1f,
					type: Type
				),
				new ExchangeAction
				(
					id: new Guid("c07fb055-9144-4be0-be45-c8e0742381c9"),
					name: "Teleport",
					attack: new Attack(baseDamage: 10, healthDrainModifier: -0.0f),
					actionTexture: Resources.Load("AbilityIcons/Default") as Texture2D,
					primaryActionName: "Teleport",
					cooldown: 1f,
					type: Type
				)
			};

			actions.ForEach(x => Actions_ByName.Add(x.Name, x));
			actions.ForEach(x => Actions_ByGuid.Add(x.Id, x));
		}
	}
}
