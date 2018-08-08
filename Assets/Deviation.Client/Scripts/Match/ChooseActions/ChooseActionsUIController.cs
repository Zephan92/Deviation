using Assets.Deviation.Client.Scripts.Client;
using Assets.Deviation.Client.Scripts.UserInterface;
using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Deviation.Materials;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Library;
using Assets.Scripts.ModuleEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
		public List<IExchangeAction> _actions;
		public VerticalScrollPanel ActionList;
		public UnityAction<List<IExchangeAction>> OnConfirmActions;
		public Button ConfirmActionsButton;

		private ChooseActionsUIState _uiState;
		public ChooseActionsUIState UIState
		{
			get
			{
				return _uiState;
			}
			set
			{
				_uiState = value;
				OnUIStateChange?.Invoke(value);
			}
		}
		private UnityAction<ChooseActionsUIState> OnUIStateChange;
		private GameObject _chosenActionsPanel;

		public override void Awake()
		{
			base.Awake();
			OnUIStateChange += OnUIStateChangeMethod;

			ActionList = GetComponentInChildren<VerticalScrollPanel>();
			var footer = transform.Find("Footer");
			ConfirmActionsButton = footer.GetComponentInChildren<Button>();
			ConfirmActionsButton.onClick.AddListener(ConfirmActions);
			_chosenActionsPanel = transform.Find("ChosenActions").gameObject;
		}

		public override void Start()
		{
			UIState = ChooseActionsUIState.Start;
		}

		public override void Update()
		{
			//ChooseTraderTimer.text = ((int)tm.GetRemainingCooldown(ClientMatchState.ChooseActions.ToString())).ToString();
		}

		private void OnUIStateChangeMethod(ChooseActionsUIState state)
		{
			switch (state)
			{
				case ChooseActionsUIState.Start:
					var trader = cmc.GetTrader();
					var actionDict = ActionLibrary.GetActionLibrary_ByName(trader.Type);
					actionDict.Remove("Default");
					_actions = actionDict.Values.OrderBy(t => t.Name).ToList();
					InitializeActionPanels(_actions);
					//var panel = TraderPanelFactory.CreateTraderPanel(trader, ActionList.List, onTraderListPanelClick);
					break;
				case ChooseActionsUIState.ActionSelected:
					break;
				case ChooseActionsUIState.ActionsConfirmed:
					cmc.State = ClientMatchState.Summary;
					break;
			}
		}

		public void InitializeActionPanels(List<IExchangeAction> actions)
		{
			GameObject actionRow = new GameObject("ActionRow");
			LayoutGroupFactory.CreateHorizontalLayout(actionRow);
			actionRow.GetComponent<RectTransform>().sizeDelta = new Vector2(1240, 100);
			actionRow.transform.SetParent(ActionList.List.transform);

			for (int i = 0; i < actions.Count; i++)
			{
				var action = actions[i];
				if (i % 6 == 0 && i > 0)
				{
					actionRow = new GameObject("ActionRow");
					LayoutGroupFactory.CreateHorizontalLayout(actionRow);
					actionRow.GetComponent<RectTransform>().sizeDelta = new Vector2(1240, 100);
					actionRow.transform.SetParent(ActionList.List.transform);
				}

				CreateActionPanel(action, actionRow);
			}
		}

		public GameObject CreateActionPanel(IExchangeAction action, GameObject parent)
		{
			var actionPanel = Instantiate(Resources.Load("ActionPanel"), parent.transform) as GameObject;
			var actionDetailsPanel = actionPanel.GetComponent<ActionDetailsPanel>();
			actionDetailsPanel.UpdateActionDetails(action);
			DragableUIFactory.CreateDraggableUI(actionPanel, ValidSnapCheck, action.Type);
			return actionPanel;
		}
		
		public void ConfirmActions()
		{
			List<IExchangeAction> actions = new List<IExchangeAction>();
			ActionDetailsPanel[] chosenActionPanels = _chosenActionsPanel.GetComponentsInChildren<ActionDetailsPanel>();
			foreach (var panel in chosenActionPanels)
			{
				actions.Add(panel.Action);
			}

			OnConfirmActions(actions);
		}
		
		private bool ValidSnapCheck(SnapPoint snap, TraderType type)
		{
			return true;
		}
	}
}
