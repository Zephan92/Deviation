using Assets.Scripts.Interface.DTO;
using Assets.Scripts.DTO.Exchange;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Library.Action.ActionModules;
using Assets.Scripts.Library.Action;
using System.Linq;
using Assets.Scripts.Enum;
using System;

namespace Assets.Scripts.Library
{
	//This library holds all of the values for each Action, all definitions of an action are here.
	public class ActionLibrary : MonoBehaviour
	{
		public static List<IActionLibraryModule> ActionLibraryModules = new List<IActionLibraryModule> {
			new AlphaActions(),
			new EnergyActions(),
			new GammaActions(),
			new DeltaActions(),
			new EpsilonActions(),
			new TestActions(),
		};

		public static Guid GetGuidFromName(string actionName)
		{
			IExchangeAction action = GetActionLibrary_ByName()[actionName];
			return action.Id;
		}

		public static string GetNameFromGuid(Guid actionGuid)
		{

			IExchangeAction action = GetActionLibrary_ByGuid()[actionGuid];
			return action.Name;
		}

		public static IExchangeAction GetActionInstance(string actionName)
		{
			IExchangeAction action = GetActionLibrary_ByName()[actionName];
			IExchangeAction actionInstance = new ExchangeAction(action.Id, action.Name,action.Attack,action.ActionTexture,action.PrimaryActionName,action.Cooldown,action.Type);
			return actionInstance;
		}

		public static IExchangeAction GetActionInstance(Guid actionGuid)
		{
			IExchangeAction action = GetActionLibrary_ByGuid()[actionGuid];
			IExchangeAction actionInstance = new ExchangeAction(action.Id, action.Name, action.Attack, action.ActionTexture, action.PrimaryActionName, action.Cooldown, action.Type);
			return actionInstance;
		}

		public static Dictionary<Guid, IExchangeAction> GetActionLibrary_ByGuid(TraderType moduleType = TraderType.Default)
		{
			var retVal = new Dictionary<Guid, IExchangeAction>();
			var defaultAction = GetDefaultAction();
			retVal.Add(defaultAction.Id, defaultAction);

			if (TraderType.Default == moduleType)
			{
				foreach (IActionLibraryModule actionLibraryModule in ActionLibraryModules)
				{
					actionLibraryModule.Actions_ByGuid.ToList().ForEach(x => retVal.Add(x.Key, x.Value));
				}
			}
			else
			{
				foreach (IActionLibraryModule actionLibraryModule in ActionLibraryModules)
				{

					actionLibraryModule.Actions_ByGuid.ToList().ForEach(x => { if (x.Value.Type == moduleType) retVal.Add(x.Key, x.Value); });
				}
			}

			return retVal;
		}

		public static Dictionary<string, IExchangeAction> GetActionLibrary_ByName(TraderType moduleType = TraderType.Default)
		{
			var retVal = new Dictionary<string, IExchangeAction>();
			var defaultAction = GetDefaultAction();
			retVal.Add(defaultAction.Name, defaultAction);

			if (TraderType.Default == moduleType)
			{
				foreach (IActionLibraryModule actionLibraryModule in ActionLibraryModules)
				{
					actionLibraryModule.Actions_ByName.ToList().ForEach(x => retVal.Add(x.Key, x.Value));
				}
			}
			else
			{
				foreach (IActionLibraryModule actionLibraryModule in ActionLibraryModules)
				{
					actionLibraryModule.Actions_ByName.ToList().ForEach(x => { if (x.Value.Type == moduleType) retVal.Add(x.Key, x.Value); });
				}
			}

			return retVal;
		}

		private static IExchangeAction GetDefaultAction()
		{
			return new ExchangeAction
			(
				id: Guid.Empty,
				name: "Default",
				attack: new Attack(),
				actionTexture: Resources.Load("ActionTextures/White") as Texture2D,
				primaryActionName: "default",
				cooldown: 0f,
				type: TraderType.Default
			);
		}
	}
}
