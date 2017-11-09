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
	public class ManipulateActions : IActionLibraryModule
	{
		public Dictionary<string, IExchangeAction> Actions_ByName { get; set; }
		public Dictionary<Guid, IExchangeAction> Actions_ByGuid { get; set; }

		public ModuleType Type { get{return ModuleType.Manipulate;}}

		public ManipulateActions()
		{
			Actions_ByName = new Dictionary<string, IExchangeAction>();
			Actions_ByGuid = new Dictionary<Guid, IExchangeAction>();

			List<IExchangeAction> actions = new List<IExchangeAction>()
			{
				new ExchangeAction
				(
					id: new Guid("10e665a7-876e-4531-8991-987b42d3d2ff"),
					name: "Steal",
					attack: new Attack(baseDamage: 20, healthDrainModifier: -0f),
					actionTexture: Resources.Load("ActionTextures/Blue") as Texture2D,
					primaryActionName: "Steal",
					cooldown: 1f,
					type: Type
				),
				new ExchangeAction
				(
					id: new Guid("258175b1-89e0-4f16-91e2-b65cb1e11c58"),
					name: "Disappear",
					attack: new Attack(baseDamage: 15, healthDrainModifier: -0f),
					actionTexture: Resources.Load("ActionTextures/Blue") as Texture2D,
					primaryActionName: "Disappear",
					cooldown: 1f,
					type: Type
				),
				new ExchangeAction
				(
					id: new Guid("9f2614a5-8f72-4b27-991a-d1ad36d010b9"),
					name: "Short Attack",
					attack: new DTO.Exchange.Attack(baseDamage: 10, energyRecoilModifier: -0.5f),
					actionTexture: Resources.Load("ActionTextures/Green") as Texture2D,
					primaryActionName: "ShortAttack",
					cooldown: 1f,
					type: Type
				),
				new ExchangeAction
				(
					id: new Guid("93657ced-1f32-4408-9d6d-12b4096e2cae"),
					name: "Middle Attack",
					attack: new Attack(baseDamage: 20, energyRecoilModifier: -0.3f),
					actionTexture: Resources.Load("ActionTextures/Yellow") as Texture2D,
					primaryActionName: "MiddleAttack",
					cooldown: 1f,
					type: Type
				),
				new ExchangeAction
				(
					id: new Guid("1e14d696-7a90-4271-97e2-fbc8a8c740f8"),
					name: "Wall Push",
					attack: new Attack(baseDamage: 40, healthDrainModifier: -0.8f),
					actionTexture: Resources.Load("ActionTextures/Green") as Texture2D,
					primaryActionName: "WallPush",
					cooldown: 3f,
					type: Type
				),
			};

			actions.ForEach(x => Actions_ByName.Add(x.Name, x));
			actions.ForEach(x => Actions_ByGuid.Add(x.Id, x));
		}
	}
}
