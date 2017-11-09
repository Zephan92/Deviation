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
		public Dictionary<string, IExchangeAction> Actions_ByName { get; set; }
		public Dictionary<Guid, IExchangeAction> Actions_ByGuid { get; set; }

		public ModuleType Type { get { return ModuleType.Scavenge; } }

		public ScavengeActions()
		{
			Actions_ByName = new Dictionary<string, IExchangeAction>();
			Actions_ByGuid = new Dictionary<Guid, IExchangeAction>();

			List<IExchangeAction> actions = new List<IExchangeAction>()
			{
				new ExchangeAction
				(
					id: new Guid("dacb468b-658f-4daa-9400-cd3f005d06bd"),
					name: "Stun Field",
					attack: new Attack(baseDamage: 20, healthDrainModifier: -0.6f,  energyRecoilModifier: -1.8f),
					actionTexture: Resources.Load("ActionTextures/Purple") as Texture2D,
					primaryActionName: "StunField",
					cooldown: 1f,
					type: Type
				),
			};

			actions.ForEach(x => Actions_ByName.Add(x.Name, x));
			actions.ForEach(x => Actions_ByGuid.Add(x.Id, x));
		}
	}
}
