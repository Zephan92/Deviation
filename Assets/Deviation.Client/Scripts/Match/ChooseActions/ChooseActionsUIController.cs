﻿using Assets.Deviation.Client.Scripts.Client;
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
			CreateHorizontalLayout(actionRow);
			actionRow.GetComponent<RectTransform>().sizeDelta = new Vector2(1240, 100);
			actionRow.transform.SetParent(ActionList.List.transform);

			for (int i = 0; i < actions.Count; i++)
			{
				var action = actions[i];
				if (i % 6 == 0 && i > 0)
				{
					actionRow = new GameObject("ActionRow");
					CreateHorizontalLayout(actionRow);
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
			CreateEventTriggers(actionPanel, ValidSnapCheck, action.Type);
			return actionPanel;
		}

		public HorizontalLayoutGroup CreateHorizontalLayout(GameObject go)
		{
			var layout = go.AddComponent<HorizontalLayoutGroup>();
			layout.childControlWidth = false;
			layout.childControlHeight = true;
			layout.childForceExpandWidth = false;
			layout.childForceExpandHeight = true;
			return layout;
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

		private void CreateEventTriggers<T>(GameObject actionPanel, DragableUI.onEndDrag<T> onEndDrag, T type)
		{
			DragableUI dragable = actionPanel.AddComponent<DragableUI>();
			EventTrigger et = actionPanel.AddComponent<EventTrigger>();
			et.triggers.Add(CreateEventTriggerEntry(EventTriggerType.BeginDrag, dragable, onEndDrag, type));
			et.triggers.Add(CreateEventTriggerEntry(EventTriggerType.Drag, dragable, onEndDrag, type));
			et.triggers.Add(CreateEventTriggerEntry(EventTriggerType.EndDrag, dragable, onEndDrag, type));
		}

		private bool ValidSnapCheck(SnapPoint snap, TraderType type)
		{
			return true;
		}

		private EventTrigger.Entry CreateEventTriggerEntry<T>(EventTriggerType eventTriggerType, DragableUI dragable, DragableUI.onEndDrag<T> onEndDrag, T type)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = eventTriggerType;

			switch (eventTriggerType)
			{
				case EventTriggerType.BeginDrag:
					entry.callback.AddListener((eventData) => { dragable.BeginDrag(); });
					break;

				case EventTriggerType.Drag:
					entry.callback.AddListener((eventData) => { dragable.OnDrag(); });
					break;

				case EventTriggerType.EndDrag:
					entry.callback.AddListener((eventData) => { dragable.EndDrag(onEndDrag, type); });
					break;
			}

			return entry;
		}
	}
}
