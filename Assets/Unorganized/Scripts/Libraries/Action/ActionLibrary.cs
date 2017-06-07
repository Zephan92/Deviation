using Assets.Scripts.Interface.DTO;
using Assets.Scripts.DTO.Exchange;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Library.Action.ActionModules;
using Assets.Scripts.Library.Action;
using System.Linq;
using Assets.Scripts.Enum;

namespace Assets.Scripts.Library
{
	//This library holds all of the values for each Action, all definitions of an action are here.
	public class ActionLibrary : MonoBehaviour
	{
		public static List<IActionLibraryModule> ActionLibraryModules = new List<IActionLibraryModule> {
			new ElementalActions(),
			new EnergyActions(),
			new LifeActions(),
			new MageActions(),
			new ManipulateActions(),
			new RangerActions(),
			new ScavengeActions(),
			new TestActions(),
		};

		public static readonly Dictionary<string, IExchangeAction> ActionLibraryTable = new Dictionary<string, IExchangeAction> { };

		public static IExchangeAction GetActionInstance(string actionName)
		{
			AddActionModulesToLibrary();

			IExchangeAction action = ActionLibraryTable[actionName];
			IExchangeAction actionInstance = new ExchangeAction(action.Name,action.Attack,action.ActionTexture,action.PrimaryActionName,action.Cooldown,action.Type);
			return actionInstance;
		}

		private static void AddActionModulesToLibrary()
		{
			if (ActionLibraryTable.Count == 0)
			{
				ActionLibraryTable.Add("default", new ExchangeAction
				(
					name: "Default",
					attack: new Attack(),
					actionTexture: Resources.Load("ActionTextures/White") as Texture2D,
					primaryActionName: "default",
					cooldown: 0f,
					type: ModuleType.Default
				));

				foreach (IActionLibraryModule actionLibraryModule in ActionLibraryModules)
				{
					actionLibraryModule.Actions.ToList().ForEach(x => ActionLibraryTable.Add(x.Key, x.Value));
				}
			}
		}
	}
}
