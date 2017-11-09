using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using System.Collections.Generic;
using Assets.Scripts.Library.Action.ModuleActions;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Enum;

namespace Assets.Scripts.Library
{
	//This library holds all of the values for each Action's Method, all definitions of an action method are here.
	public class ActionMethodLibrary : MonoBehaviour
	{
		public static List<IActionMethodLibraryModule> ModuleActionTypes = new List<IActionMethodLibraryModule> {
			new ElementalActionMethods(),
		};

		private static Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>> ActionMethodLibraryTable = new Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>>();

		public static System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone> GetActionMethod(string actionName)
		{
			AddActionMethodModulesToLibrary();

			return ActionMethodLibraryTable[actionName];
		}

		public static bool ContainsActionMethod(string actionName)
		{
			AddActionMethodModulesToLibrary();

			return ActionMethodLibraryTable.ContainsKey(actionName);
		}

		public static Dictionary<string, System.Action<IBattlefieldController, IAttack, IExchangePlayer, BattlefieldZone>> GetActionLibary()
		{
			AddActionMethodModulesToLibrary();

			return ActionMethodLibraryTable;
		}

		private static void AddActionMethodModulesToLibrary()
		{
			if (ActionMethodLibraryTable.Count == 0)
			{
				ActionMethodLibraryTable.Add("default",
				delegate (IBattlefieldController bc, IAttack attack, IExchangePlayer player, BattlefieldZone zone)
				{
					Debug.Log("Default Action");
				});

				foreach (IActionMethodLibraryModule moduleAction in ModuleActionTypes)
				{
					moduleAction.ActionMethods.ToList().ForEach(x => ActionMethodLibraryTable.Add(x.Key, x.Value));
				}

				EnergyActions.ActionMethodLibraryTable.ToList().ForEach(x => ActionMethodLibraryTable.Add(x.Key, x.Value));
				LifeActions.ActionMethodLibraryTable.ToList().ForEach(x => ActionMethodLibraryTable.Add(x.Key, x.Value));
				MageActions.ActionMethodLibraryTable.ToList().ForEach(x => ActionMethodLibraryTable.Add(x.Key, x.Value));
				ManipulateActions.ActionMethodLibraryTable.ToList().ForEach(x => ActionMethodLibraryTable.Add(x.Key, x.Value));
				RangerActions.ActionMethodLibraryTable.ToList().ForEach(x => ActionMethodLibraryTable.Add(x.Key, x.Value));
				ScavengeActions.ActionMethodLibraryTable.ToList().ForEach(x => ActionMethodLibraryTable.Add(x.Key, x.Value));
				TestActions.ActionMethodLibraryTable.ToList().ForEach(x => ActionMethodLibraryTable.Add(x.Key, x.Value));
			}
		}
	}
}
