using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using System;
using Assets.Scripts.Library;

namespace Assets.Scripts.DTO.Exchange
{
	public class Kit : IKit
	{
		public IExchangePlayer Player { get; set; }
		public IExchangeAction[] Actions { get { return _actions; } }
		public string[] ActionsNames { get { return _actionsNames; } }
		public Guid[] ActionsGuids { get { return _actionsGuids; } }

		//List of Modules in this kit
		private IExchangeAction[] _actions;
		private string[] _actionsNames;
		private Guid[] _actionsGuids;

		public Kit(string[] actionNames)
		{
			_actionsGuids = new Guid[4];

			_actionsNames = actionNames;
			_actions = new IExchangeAction[4];

			for(int i = 0; i < actionNames.Length; i++)
			{
				_actions[i] = ActionLibrary.GetActionInstance(actionNames[i]);
				_actions[i].ParentKit = this;
				_actionsGuids[i] = _actions[i].Id;
			}
		}

		public Kit(Guid[] actionGuids)
		{
			_actionsNames = new string[4];

			_actions = new IExchangeAction[4];
			_actionsGuids = actionGuids;
			for (int i = 0; i < actionGuids.Length; i++)
			{
				_actions[i] = ActionLibrary.GetActionInstance(actionGuids[i]);
				_actions[i].ParentKit = this;
				_actionsNames[i] = _actions[i].Name;
			}
		}
	}
}
