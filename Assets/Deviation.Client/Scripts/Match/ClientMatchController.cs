using Assets.Deviation.Client.Scripts;
using Assets.Deviation.Client.Scripts.Match;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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

		//Private
		private ClientMatchState _state;
		private GameObject _activeUI;
		private ITrader _chosenTrader;

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
			StartUI = MatchUis.GetComponentInChildren<StartUIController>();//this doesn't work
			ChooseTraderUI = MatchUis.GetComponentInChildren<ChooseTraderUIController>();
			ChooseActionsUI = MatchUis.GetComponentInChildren<ChooseActionsUIController>();
			SummaryUI = MatchUis.GetComponentInChildren<SummaryUIController>();
			EndUI = MatchUis.GetComponentInChildren<EndUIController>();
			
			tm.AddTimer(ClientMatchState.ChooseTrader.ToString(), 30);
			tm.AddTimer(ClientMatchState.ChooseActions.ToString(), 60);

			if (ChooseTraderUI != null)
			{
				ChooseTraderUI.OnConfirmTrader += ConfirmTrader;
			}

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

			//hide this
			if (UnityEngine.Debug.isDebugBuild && !Application.isEditor)
			{
				var testArgs = Msf.Args.ExtractValue("-test");
				if (testArgs.Equals("GuestLogin"))
				{
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
					break;
				case ClientMatchState.ChooseActions:
					tm.RestartTimer(state.ToString());
					break;
				case ClientMatchState.Summary:
					State = ClientMatchState.End;
					break;
				case ClientMatchState.End:
					break;
			}
		}

		public void FixedUpdate()
		{
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
					if (tm.TimerUp(State.ToString()))
					{
						Debug.LogError("This should probably punt you from this match");
						//State = ClientMatchState.ChooseActions;
					}
					break;
				case ClientMatchState.ChooseActions:
					tm.UpdateCountdowns();
					if (tm.TimerUp(State.ToString()))
					{
						State = ClientMatchState.End;
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

		//TODO
		private ActionModulePacket GetPlayerActionModule()
		{
			var q = new Guid("688b267a-fde1-4250-91a0-300aa3343147");//ShockWave
			var w = new Guid("258175b1-89e0-4f16-91e2-b65cb1e11c58");//Ambush
			//var w = new Guid("dacb468b-658f-4daa-9400-cd3f005d06bd");//Stun Field
			//var e = new Guid("d504df35-dc93-4f84-829e-01e202878341");//Tremor

			var e = new Guid("1e14d696-7a90-4271-97e2-fbc8a8c740f8");//Wall Push
			var r = new Guid("36a1cf13-8b79-4800-8574-7cec0c405594");//Small Projectile
			//this would go get the actions the player chose
			return new ActionModulePacket(q, w, e, r);
		}

		//Rename rework
		private IEnumerator Test()
		{
			yield return new WaitForSeconds(1f);
			if (UnityEngine.Debug.isDebugBuild)
				Ready();
		}
	}
}
