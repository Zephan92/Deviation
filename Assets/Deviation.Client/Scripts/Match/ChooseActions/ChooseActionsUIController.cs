using Assets.Deviation.Exchange.Scripts.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Match
{
	public enum ChooseActionsUIState
	{
		Start = 0,
		ActionSelected = 1,
		ActionsConfirmed = 2,
	}

	public class ChooseActionsUIController : UIController
	{
		public Text ChooseTraderTimer;//Find this dynamically

		public ChooseActionsUIState UIState
		{
			get
			{
				return _uiState;
			}
			set
			{
				_uiState = value;

				if (OnUIStateChange != null)
				{
					OnUIStateChange(value);
				}
			}
		}
		private UnityAction<ChooseActionsUIState> OnUIStateChange;
		private ChooseActionsUIState _uiState;


		public override void Awake()
		{
			base.Awake();
			OnUIStateChange += OnUIStateChangeMethod;
		}

		public override void Update()
		{
			ChooseTraderTimer.text = ((int)tm.GetRemainingCooldown(ClientMatchState.ChooseActions.ToString())).ToString();
		}

		private void OnUIStateChangeMethod(ChooseActionsUIState state)
		{
			//if (_activeMiddlePanel != null)
			//{
			//	_activeMiddlePanel.SetActive(false);
			//}

			//Transform currentPanel = MiddlePanel.transform.GetChild((int)state);
			//if (currentPanel != null)
			//{
			//	_activeMiddlePanel = currentPanel.gameObject;
			//	_activeMiddlePanel.SetActive(true);
			//}

			switch (state)
			{
				case ChooseActionsUIState.Start:
					//RebuildTraderList();
					break;
				case ChooseActionsUIState.ActionSelected:
					break;
				case ChooseActionsUIState.ActionsConfirmed:
					cmc.State = ClientMatchState.Summary;
					break;
			}
		}
	}
}
