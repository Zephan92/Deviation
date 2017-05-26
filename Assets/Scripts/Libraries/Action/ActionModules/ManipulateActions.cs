using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Library.Action.ActionModules
{
	public class ManipulateActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions { get; set; }
		string IActionLibraryModule.ModuleName { get { return "Manipulate"; } }

		public ManipulateActions()
		{
			Actions = new Dictionary<string, IExchangeAction>
			{
				{"Steal",new ExchangeAction
				(
					name: "Steal",
					attack: new Attack(baseDamage: 20, healthDrainModifier: -0f),
					actionTexture: Resources.Load("ActionTextures/Blue") as Texture2D,
					primaryActionName: "Steal",
					cooldown: 1f
				)
				},
				{"Disappear",new ExchangeAction
					(
						name: "Disappear",
						attack: new Attack(baseDamage: 15, healthDrainModifier: -0f),
						actionTexture: Resources.Load("ActionTextures/Blue") as Texture2D,
						primaryActionName: "Disappear",
						cooldown: 1f
					)
				},
				{ "ShortAttack",new ExchangeAction
					(
						name: "Short Attack",
						attack: new DTO.Exchange.Attack(baseDamage: 10, energyRecoilModifier: -0.5f),
						actionTexture: Resources.Load("ActionTextures/Green") as Texture2D,
						primaryActionName: "ShortAttack",
						cooldown: 1f
					)
				},
				{"MiddleAttack",new ExchangeAction
					(
						name: "Middle Attack",
						attack: new Attack(baseDamage: 20, energyRecoilModifier: -0.3f),
						actionTexture: Resources.Load("ActionTextures/Yellow") as Texture2D,
						primaryActionName: "MiddleAttack",
						cooldown: 1f
					)
				},
				{"WallPush",new ExchangeAction
					(
						name: "Wall Push",
						attack: new Attack(baseDamage: 40, healthDrainModifier: -0.8f),
						actionTexture: Resources.Load("ActionTextures/Green") as Texture2D,
						primaryActionName: "WallPush",
						cooldown: 3f
					)
				},
			};
		}
	}
}
