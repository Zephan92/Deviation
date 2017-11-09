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
		private static bool _initialized = false;

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

		private static readonly Dictionary<string, IExchangeAction> _actionLibraryTable_ByName = new Dictionary<string, IExchangeAction> { };
		private static readonly Dictionary<Guid, IExchangeAction> _actionLibraryTable_ByGuid = new Dictionary<Guid, IExchangeAction> { };

		public static Dictionary<string, IExchangeAction> GetActionLibraryTable_ByName()
		{
			AddActionModulesToLibrary();

			return _actionLibraryTable_ByName;
		}

		public static Dictionary<Guid, IExchangeAction> GetActionLibraryTable_ByGuid()
		{
			AddActionModulesToLibrary();

			return _actionLibraryTable_ByGuid;
		}

		public static Guid GetGuidFromName(string actionName)
		{
			AddActionModulesToLibrary();

			IExchangeAction action = _actionLibraryTable_ByName[actionName];
			return action.Id;
		}

		public static string GetNameFromGuid(Guid actionName)
		{
			AddActionModulesToLibrary();

			IExchangeAction action = _actionLibraryTable_ByGuid[actionName];
			return action.Name;
		}

		public static IExchangeAction GetActionInstance(string actionName)
		{
			AddActionModulesToLibrary();
			IExchangeAction action = _actionLibraryTable_ByName[actionName];
			IExchangeAction actionInstance = new ExchangeAction(action.Id, action.Name,action.Attack,action.ActionTexture,action.PrimaryActionName,action.Cooldown,action.Type);
			return actionInstance;
		}

		public static IExchangeAction GetActionInstance(Guid actionGuid)
		{
			AddActionModulesToLibrary();
			IExchangeAction action = _actionLibraryTable_ByGuid[actionGuid];
			IExchangeAction actionInstance = new ExchangeAction(action.Id, action.Name, action.Attack, action.ActionTexture, action.PrimaryActionName, action.Cooldown, action.Type);
			return actionInstance;
		}

		private static void AddActionModulesToLibrary()
		{
			if (_initialized)
			{
				return;
			}

			var defaultAction = new ExchangeAction
			(
				id: Guid.Empty,
				name: "Default",
				attack: new Attack(),
				actionTexture: Resources.Load("ActionTextures/White") as Texture2D,
				primaryActionName: "default",
				cooldown: 0f,
				type: ModuleType.Default
			);

			_actionLibraryTable_ByName.Add(defaultAction.Name, defaultAction);
			_actionLibraryTable_ByGuid.Add(defaultAction.Id, defaultAction);

			foreach (IActionLibraryModule actionLibraryModule in ActionLibraryModules)
			{
				actionLibraryModule.Actions_ByName.ToList().ForEach(x => _actionLibraryTable_ByName.Add(x.Key, x.Value));
				actionLibraryModule.Actions_ByGuid.ToList().ForEach(x => _actionLibraryTable_ByGuid.Add(x.Key, x.Value));
			}

			_initialized = true;
		}
	}
}
