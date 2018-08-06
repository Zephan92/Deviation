using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Interface.DTO;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.Enum;

namespace Assets.Scripts.Library.Action.ActionModules
{
	public class AlphaActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions_ByName { get; set; }
		public Dictionary<Guid, IExchangeAction> Actions_ByGuid { get; set; }

		public TraderType Type { get{return TraderType.Alpha;}}

		public AlphaActions()
		{
			Actions_ByName = new Dictionary<string, IExchangeAction>();
			Actions_ByGuid = new Dictionary<Guid, IExchangeAction>();

			List<IExchangeAction> actions = new List<IExchangeAction>()
			{
				new ExchangeAction
				(
					id: new Guid("de1eff34-adcb-4301-86bb-adb1e9c01f8d"),
					name: "Avalanche",
					attack: new Attack(baseDamage: 30, healthDrainModifier: -0.1f),
					actionTexture: Resources.Load("AbilityIcons/Default") as Texture2D,
					primaryActionName: "Avalanche",
					cooldown: 1f,
					type: Type
				),
				new ExchangeAction
				(
					id: new Guid("28d4642e-7fdf-40fb-9bff-eb2a60aad15e"),
					name: "Fire Blast",
					attack: new Attack(baseDamage: 80),
					actionTexture: Resources.Load("AbilityIcons/Default") as Texture2D,
					primaryActionName: "FireBlast",
					cooldown: 0f,
					type: Type
				),
				new ExchangeAction
				(
					id: new Guid("d504df35-dc93-4f84-829e-01e202878341"),
					name: "Tremor",
					attack: new Attack(baseDamage: 20),
					actionTexture: Resources.Load("AbilityIcons/Default") as Texture2D,
					primaryActionName: "Tremor",
					cooldown: 1f,
					type: Type
				),
				new ExchangeAction
				(
					id: new Guid("688b267a-fde1-4250-91a0-300aa3343147"),
					name: "ShockWave",
					attack: new Attack(baseDamage: 20, healthDrainModifier: -0.6f),
					actionTexture: Resources.Load("AbilityIcons/Default") as Texture2D,
					primaryActionName: "ShockWave",
					cooldown: 1f,
					type: Type
				)
			};

			actions.ForEach(x => Actions_ByName.Add(x.Name, x));
			actions.ForEach(x => Actions_ByGuid.Add(x.Id, x));
		}
	}
}
