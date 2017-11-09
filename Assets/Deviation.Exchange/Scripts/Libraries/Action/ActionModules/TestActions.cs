using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ActionModules
{
	public class TestActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions_ByName { get; set; }
		public Dictionary<Guid, IExchangeAction> Actions_ByGuid { get; set; }

		public ModuleType Type { get { return ModuleType.Test; } }

		public TestActions()
		{
			Actions_ByName = new Dictionary<string, IExchangeAction>();
			Actions_ByGuid = new Dictionary<Guid, IExchangeAction>();

			List<IExchangeAction> actions = new List<IExchangeAction>()
			{
				new ExchangeAction
				(
					id: new Guid("4563ef95-0a47-4bbd-9942-bccb3bb240fe"),
					name: "BOOOOOOM",
					attack: new Attack(baseDamage: 0, energyRecoilModifier: 0f),
					actionTexture: Resources.Load("ActionTextures/Red") as Texture2D,
					primaryActionName: "OneHitKO",
					cooldown: 0f,
					type: Type
				),
			};

			actions.ForEach(x => Actions_ByName.Add(x.Name, x));
			actions.ForEach(x => Actions_ByGuid.Add(x.Id, x));
		}
	}
}
