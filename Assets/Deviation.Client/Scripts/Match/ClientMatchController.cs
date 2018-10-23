using Assets.Deviation.Client.Scripts;
using Assets.Deviation.Client.Scripts.Match;
using Assets.Deviation.Exchange.Scripts.DTO.Exchange;
using Assets.Deviation.MasterServer.Scripts;
using Assets.Deviation.MasterServer.Scripts.Exchange;
using Assets.Scripts.DTO.Exchange;
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

	public class ClientMatchController : ControllerBase
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
		private StartUIController StartUI;
		private ChooseTraderUIController ChooseTraderUI;
		private ChooseActionsUIController ChooseActionsUI;
		private SummaryUIController SummaryUI;
		private EndUIController EndUI;

		public override void Awake()
		{
			base.Awake();			
			tm.AddTimer(ClientMatchState.ChooseTrader.ToString(), 30);
			tm.AddTimer(ClientMatchState.ChooseActions.ToString(), 60);

			var parent = GameObject.Find("MatchUIs");
			var header = parent.transform.Find("Header");
			MatchUis = parent.transform.Find("UIs").gameObject;
			HeaderTitle = header.transform.Find("Title").GetComponent<Text>();
			Timer = header.transform.Find("Timer").GetComponent<Text>(); ;
			OnClientMatchStateChange += OnClientMatchStateChangeMethod;
			State = ClientMatchState.Start;
		}

		public override void Start()
		{
			base.Start();

			ClientDataRepository.Instance.State = ClientState.Match;

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

		public override void FixedUpdate()
		{
			base.FixedUpdate();

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

		public void ConfirmTrader(ITrader trader)
		{
			State = ClientMatchState.ChooseActions;
			_chosenTrader = trader;
		}

		public void ConfirmActions(List<IExchangeAction> actions)
		{
			State = ClientMatchState.End;
			_actions = actions;
		}

		public ITrader GetTrader()
		{
			return _chosenTrader;
		}

		public void Ready()
		{
			if (ClientDataRepository.Instance.State == ClientState.Match && ClientDataRepository.Instance.HasExchange)
			{
				Guid characterGuid = _chosenTrader.Guid;
				Kit kit = GetKit();
				ExchangeDataEntry packet = new ExchangeDataEntry(ClientDataRepository.Instance.Exchange.ExchangeId, ClientDataRepository.Instance.PlayerAccount, kit, characterGuid);
				Msf.Client.Connection.SendMessage((short)ExchangePlayerOpCodes.CreateExchangeData, packet, (response, error) => {
					if (response == ResponseStatus.Error)
					{
						UnityEngine.Debug.LogErrorFormat("CreateExchangeData Error: {0}", error);
					}
					else if(response == ResponseStatus.Success)
					{
						UnityEngine.Debug.Log("Successfully Created Exchange Data");
						StartCoroutine(StartExchange());
					}
				});
			}
			else
			{
				UnityEngine.Debug.LogErrorFormat("The Exchange is not ready or does not exist");
			}
		}

		//TODO
		private IEnumerator StartExchange()
		{
			UnityEngine.Debug.Log("Waiting for Exchange Room");
			yield return new WaitUntil(() => ClientDataRepository.Instance.RoomId != -1);
			UnityEngine.Debug.Log("We Recieved Access to Room: " + ClientDataRepository.Instance.RoomId);
			UnityEngine.Debug.Log("Loading Exchange Scene");
			SceneManager.LoadScene("DeviationClient - Exchange");
		}

		private Kit CreateStarterKit()
		{
			IBasicAction basicAction = new BasicAction("Small Projectile");
			var clip1Actions = new Dictionary<string, int>
			{
				{ "Small Projectile", 2 },
				{ "Boomerang", 4 },
				{ "Tremor", 2 },
				{ "ShockWave", 2 }
			};
			var clip2Actions = new Dictionary<string, int>
			{
				{ "Small Projectile", 4 },
				//{ "Small Projectile", 2 },
				{ "Drain", 2 },
				{ "Ambush", 4 }
			};
			var clip3Actions = new Dictionary<string, int>
			{
				{ "Small Projectile", 2 },
				{ "Medium Projectile", 2 },
				{ "Large Projectile", 2 },
				{ "Wall Push", 4 }
			};
			IClip[] clips = new IClip[] { new Clip(clip1Actions), new Clip(clip2Actions), new Clip(clip3Actions) };
			return new Kit("Starter Kit", clips, basicAction);
		}

		private Kit GetKit()
		{
			//TODO Get Real CLips
			IExchangeAction q = _actions[0];
			IExchangeAction w = _actions[1];
			IExchangeAction e = _actions[2];
			IExchangeAction r = _actions[3];
			IClip clip = new Clip() {};
			clip.Add(q, 4);
			clip.Add(w, 3);
			clip.Add(e, 3);
			IClip[] clips = new IClip[] { clip, clip, clip };
			IBasicAction basicAction = new BasicAction(r);
			Kit kit = new Kit("Default Kit",clips, basicAction);
			kit = CreateStarterKit();
			return kit;
		}
	}
}
