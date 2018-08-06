using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ActionModules
{
	public class GammaActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions_ByName { get; set; }
		public Dictionary<Guid, IExchangeAction> Actions_ByGuid { get; set; }

		public TraderType Type { get { return TraderType.Gamma; } }

		public GammaActions()
		{
			Actions_ByName = new Dictionary<string, IExchangeAction>();
			Actions_ByGuid = new Dictionary<Guid, IExchangeAction>();

			List<IExchangeAction> actions = new List<IExchangeAction>()
			{
				new ExchangeAction
				(
					id: new Guid("12e42930-6c89-4a80-95df-ebeb4772a4a4"),
					name: "StunField",
					attack: new Attack(baseDamage: 15),
					actionTexture: Resources.Load("AbilityIcons/Default") as Texture2D,
					primaryActionName: "StunField",
					cooldown: 1f,
					type: Type
				)
			};

			actions.ForEach(x => Actions_ByName.Add(x.Name, x));
			actions.ForEach(x => Actions_ByGuid.Add(x.Id, x));
		}
	}
}
