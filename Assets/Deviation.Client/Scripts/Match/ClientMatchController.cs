using Assets.Deviation.Client.Scripts;
using Assets.Deviation.Client.Scripts.Match;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Library;
using Assets.Scripts.Utilities;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Deviation.Exchange.Scripts.Client
{
	public enum ClientMatchState
	{
		Start = 0,
		ChooseTrader = 1,
		ChooseActions = 2,
		Summary = 3,
		End = 4
	}

	public class ClientMatchController : MonoBehaviour
	{
		//Public
		public GameObject MatchUis;
		public ClientMatchState State
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;

				if (OnClientMatchStateChange != null)
				{
					OnClientMatchStateChange(value);
				}
			}
		}
		public UnityAction<ClientMatchState> OnClientMatchStateChange;
		public Text HeaderTitle;
		public Text Timer;

		//Private
		private ClientMatchState _state;
		private GameObject _activeUI;
		private ITrader _chosenTrader;
		private List<IExchangeAction> _actions;

		//Controllers
		private ClientDataController cdc;
		private ITimerManager tm;

		private StartUIController StartUI;
		private ChooseTraderUIController ChooseTraderUI;
		private ChooseActionsUIController ChooseActionsUI;
		private SummaryUIController SummaryUI;
		private EndUIController EndUI;

		public void Awake()
		{
			tm = FindObjectOfType<TimerManager>();
			cdc = FindObjectOfType<ClientDataController>();
			
			tm.AddTimer(ClientMatchState.ChooseTrader.ToString(), 30);
			tm.AddTimer(ClientMatchState.ChooseActions.ToString(), 60);

			OnClientMatchStateChange += OnClientMatchStateChangeMethod;
			State = ClientMatchState.Start;
		}

		public void Start()
		{
			//TODO this needs some work
			if (cdc != null)
			{
				cdc.State = ClientState.Match;

				if (cdc.PlayerAccount != null)
				{
					//ReadyButton.interactable = true;
				}
				else
				{
					cdc.PlayerAccountRecieved += () =>
					{
						//ReadyButton.interactable = true;
					};
				}
			}

			if (Debug.isDebugBuild && !Application.isEditor)
			{
				var testArgs = Msf.Args.ExtractValue("-test");
				if (testArgs != null && testArgs.Equals("GuestLogin"))
				{
					Debug.LogError("Test: ClientMatchController");

					StartCoroutine(Test());
				}
			}

			State = ClientMatchState.ChooseTrader;
		}

		private void OnClientMatchStateChangeMethod(ClientMatchState state)
		{
			//turn off old UI
			if (_activeUI != null)
			{
				_activeUI.SetActive(false);
			}

			//turn on current UI
			Transform currentUI = MatchUis.transform.GetChild((int)state);
			if (currentUI != null)
			{
				_activeUI = currentUI.gameObject;
				_activeUI.SetActive(true);
			}

			switch (state)
			{
				case ClientMatchState.Start:
					break;
				case ClientMatchState.ChooseTrader:
					tm.RestartTimer(state.ToString());
					HeaderTitle.text = "CHOOSE YOUR TRADER!";
					break;
				case ClientMatchState.ChooseActions:
					tm.RestartTimer(state.ToString());
					HeaderTitle.text = "CHOOSE YOUR ACTIONS!";
					break;
				case ClientMatchState.Summary:
					HeaderTitle.text = "GET READY!";
					State = ClientMatchState.End;
					break;
				case ClientMatchState.End:
					break;
			}
		}

		public void FixedUpdate()
		{
			if (StartUI == null)
			{
				StartUI = MatchUis.GetComponentInChildren<StartUIController>();
			}

			if (ChooseTraderUI == null)
			{
				ChooseTraderUI = MatchUis.GetComponentInChildren<ChooseTraderUIController>();
			}

			if (ChooseActionsUI == null)
			{
				ChooseActionsUI = MatchUis.GetComponentInChildren<ChooseActionsUIController>();
			}

			if (SummaryUI == null)
			{
				SummaryUI = MatchUis.GetComponentInChildren<SummaryUIController>();
			}

			if (EndUI == null)
			{
				EndUI = MatchUis.GetComponentInChildren<EndUIController>();
			}

			if (ChooseTraderUI != null)
			{
				ChooseTraderUI.OnConfirmTrader += ConfirmTrader;
			}

			if (ChooseActionsUI != null)
			{
				ChooseActionsUI.OnConfirmActions += ConfirmActions;
			}

			CheckClientMatch();
		}

		private void CheckClientMatch()
		{
			switch (State)
			{
				case ClientMatchState.Start:
					break;
				case ClientMatchState.ChooseTrader:
					tm.UpdateCountdowns();
					Timer.text = ((int)tm.GetRemainingCooldown(State.ToString())).ToString();
					if (tm.TimerUp(State.ToString()))
					{
						Debug.Log("This should probably punt you from this match");
						//State = ClientMatchState.ChooseActions;
					}
					break;
				case ClientMatchState.ChooseActions:
					tm.UpdateCountdowns();
					Timer.text = ((int)tm.GetRemainingCooldown(State.ToString())).ToString();
					if (tm.TimerUp(State.ToString()))
					{
					//	State = ClientMatchState.End;
					}
					break;
				case ClientMatchState.End:
					break;
			}
		}

		private void ConfirmTrader(ITrader trader)
		{
			State = ClientMatchState.ChooseActions;
			_chosenTrader = trader;
		}

		private void ConfirmActions(List<IExchangeAction> actions)
		{
			State = ClientMatchState.End;
			_actions = actions;
		}

		public ITrader GetTrader()
		{
			return _chosenTrader;
		}

		//TODO
		public void Ready()
		{
			if (cdc.State == ClientState.Match && cdc.Exchange != null)
			{
				Guid characterGuid = _chosenTrader.Guid;
				ActionModulePacket module = GetPlayerActionModule();
				InitExchangePlayerPacket packet = new InitExchangePlayerPacket(cdc.Exchange.ExchangeId, cdc.PlayerAccount, characterGuid, module);

				Msf.Client.Connection.SendMessage((short)ExchangePlayerOpCodes.CreateExchangeData, packet, (response, error) => {
					if (response == ResponseStatus.Error)
					{
						UnityEngine.Debug.LogErrorFormat("CreateExchangeData Error: {0}", error);
					}
					else if(response == ResponseStatus.Success)
					{
						//ReadyButton.interactable = false;
						StartCoroutine(StartExchange());
					}
				});
			}
		}

		//TODO
		private IEnumerator StartExchange()
		{
			yield return new WaitUntil(() => cdc.RoomId != -1);
			SceneManager.LoadScene("DeviationStandalone");
		}

		private ActionModulePacket GetPlayerActionModule()
		{
			var q = _actions[0].Id;
			var w = _actions[1].Id;
			var e = _actions[2].Id;
			var r = _actions[3].Id;

			return new ActionModulePacket(q, w, e, r);
		}

		public void TestDeleteMe()
		{
			StartCoroutine(Test());
		}

		//Rename rework
		private IEnumerator Test()
		{
			_chosenTrader = new Trader("TestTrader","The ultimate avenger", Assets.Scripts.Enum.TraderType.Test, "Testing Description", Guid.NewGuid());
			_actions = new List<IExchangeAction>();
			_actions.Add(ActionLibrary.GetActionInstance("Ambush"));
			_actions.Add(ActionLibrary.GetActionInstance("ShockWave"));
			_actions.Add(ActionLibrary.GetActionInstance("Wall Push"));
			_actions.Add(ActionLibrary.GetActionInstance("Medium Projectile"));

			yield return new WaitForSeconds(1f);

			

			if (UnityEngine.Debug.isDebugBuild)
				Ready();
		}
	}
}
