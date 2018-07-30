using Assets.Deviation.Exchange;
using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Deviation.MasterServer.Scripts;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Client
{
	public class MatchmakingController : MonoBehaviour
	{
		public Transform MenuBar;
		public Transform PVP;
		public Transform AI;

		public Button JoinQueueButton;
		public Button LeaveQueueButton;
		public Button ChangeQueueButton;
		public Button JoinExchangeMatchButton;
		public Button DeclineExchangeMatchButton;

		public Text TimerText;

		private ITimerManager tm;

		public void Awake()
		{
			tm = FindObjectOfType<TimerManager>();

			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondMatchFound, HandleMatchFound);
			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondChangeQueuePool, HandleChangeQueue);
			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondMatchReady, HandleMatchReady);
			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondMatchDisbanded, HandleMatchDisbanded);

			var parent = GameObject.Find("MatchmakingUI");
			MenuBar = parent.transform.Find("Menu Bar");
			PVP = parent.transform.Find("PVP");
			AI = parent.transform.Find("AI");

			JoinQueueButton = PVP.Find("Search For Matches").GetComponent<Button>();
			LeaveQueueButton = PVP.Find("Leave Queue").GetComponent<Button>();
			ChangeQueueButton = PVP.Find("Change Queue").GetComponent<Button>();
			JoinExchangeMatchButton = PVP.Find("Join Exchange").GetComponent<Button>();
			DeclineExchangeMatchButton = PVP.Find("Decline Exchange").GetComponent<Button>();
			TimerText = PVP.Find("Timer").GetComponent<Text>();

			JoinQueueButton.onClick.AddListener(SearchForExchangeMatch);
			LeaveQueueButton.onClick.AddListener(LeaveQueue);
			ChangeQueueButton.onClick.AddListener(ChangeQueue);
			JoinExchangeMatchButton.onClick.AddListener(JoinExchange);
			DeclineExchangeMatchButton.onClick.AddListener(DeclineExchange);

			tm.AddTimer("JoinMatch", 10.5f);
			EnableJoinQueueButton();
		}

		public void Update()
		{
			if (ClientDataRepository.Instance.HasExchange)
			{
				tm.UpdateCountdowns();

				if (tm.TimerUp("JoinMatch"))
				{
					DeclineExchange();
				}
			}
		}

		public void FixedUpdate()
		{
			if (ClientDataRepository.Instance.HasExchange)
			{
				TimerText.text = "Timer: " + (int)tm.GetRemainingCooldown("JoinMatch");
			}
			else
			{
				TimerText.text = "Timer: 10";
			}
		}

		public void EnableJoinQueueButton()
		{
			ClientDataRepository.OnInstanceCreated(() =>
			{
				if (ClientDataRepository.Instance.HasPlayerAccount)
				{
					JoinQueueButton.interactable = true;
				}
				else
				{
					ClientDataRepository.Instance.PlayerAccountRecieved += () =>
					{
						JoinQueueButton.interactable = true;
					};
				}
			});
		}

		public void SearchForExchangeMatch()
		{
			var packet = new ExchangeMatchMakingPacket(ClientDataRepository.Instance.PlayerAccount.Id, QueueTypes.Exchange1v1, PlayerClass.Default);

			Msf.Connection.SendMessage((short)Exchange1v1MatchMakingOpCodes.RequestJoinQueue, packet, (status, data) =>
			{
				if (status == ResponseStatus.Error)
				{
					UnityEngine.Debug.LogErrorFormat("RequestJoinQueue failed. Error {1}", data);
				}
				else if (status == ResponseStatus.Success)
				{
					UnityEngine.Debug.Log("Looking for Match");
					LeaveQueueButton.interactable = true;
					JoinQueueButton.interactable = false;
				}
			});
		}

		public void ChangeQueue()
		{
			UnityEngine.Debug.Log("ChangeQueue");

			var packet = new ExchangeMatchMakingPacket(ClientDataRepository.Instance.PlayerAccount.Id, QueueTypes.Exchange1v1, PlayerClass.Default);

			Msf.Connection.SendMessage((short)Exchange1v1MatchMakingOpCodes.RequestChangeQueuePool, packet, (status, data) =>
			{
				if (status == ResponseStatus.Error)
				{
					UnityEngine.Debug.LogErrorFormat("RequestChangeQueuePool failed. Error {1}", data);
				}
				else if (status == ResponseStatus.Success)
				{
					UnityEngine.Debug.Log("Changing Queue");
					ChangeQueueButton.interactable = false;
				}
			});
		}

		public void LeaveQueue()
		{
			UnityEngine.Debug.Log("LeaveQueue");

			var packet = new ExchangeMatchMakingPacket(ClientDataRepository.Instance.PlayerAccount.Id, QueueTypes.Exchange1v1, PlayerClass.Default);

			Msf.Connection.SendMessage((short)Exchange1v1MatchMakingOpCodes.RequestLeaveQueue, packet, (status, data) =>
			{
				if (status == ResponseStatus.Error)
				{
					UnityEngine.Debug.LogErrorFormat("RequestLeaveQueue failed. Error {1}", data);
				}
				else if (status == ResponseStatus.Success)
				{
					UnityEngine.Debug.Log("Leaving Queue");
					LeaveQueueButton.interactable = false;
					JoinQueueButton.interactable = true;
					ChangeQueueButton.interactable = false;
				}
			});
		}

		public void JoinExchange()
		{
			UnityEngine.Debug.Log("JoinExchange");

			Msf.Connection.SendMessage((short)Exchange1v1MatchMakingOpCodes.RequestJoinMatch, ClientDataRepository.Instance.Exchange, (status, data) =>
			{
				if (status == ResponseStatus.Error)
				{
					UnityEngine.Debug.LogErrorFormat("RequestJoinMatch failed. Error {1}", data);
				}
				else if (status == ResponseStatus.Success)
				{
					JoinExchangeMatchButton.interactable = false;
					DeclineExchangeMatchButton.interactable = false;
					ChangeQueueButton.interactable = false;
					tm.PauseTimer("JoinMatch");
				}
			});
		}

		public void DeclineExchange()
		{
			UnityEngine.Debug.Log("DeclineExchange");

			Msf.Connection.SendMessage((short)Exchange1v1MatchMakingOpCodes.RequestDeclineMatch, ClientDataRepository.Instance.Exchange, (status, data) => {
				if (status == ResponseStatus.Error)
				{
					UnityEngine.Debug.LogErrorFormat("RequestDeclineMatch failed. Error {1}", data);
				}
				else if (status == ResponseStatus.Success)
				{
					JoinExchangeMatchButton.interactable = false;
					DeclineExchangeMatchButton.interactable = false;
					ChangeQueueButton.interactable = false;
					JoinQueueButton.interactable = true;
					tm.PauseTimer("JoinMatch");
					ClientDataRepository.Instance.Exchange = null;
					ClientDataRepository.Instance.HasExchange = false;
				}
			});
		}

		private void HandleMatchFound(IIncommingMessage message)
		{
			UnityEngine.Debug.Log("HandleMatchFound: Setting Player Account");
			MatchFoundPacket exchange = message.Deserialize(new MatchFoundPacket());
			exchange.Player1Id = ClientDataRepository.Instance.PlayerAccount.Id;
			exchange.Player2Id = -1;
			ClientDataRepository.Instance.Exchange = exchange;
			ClientDataRepository.Instance.HasExchange = true;
			LeaveQueueButton.interactable = false;
			ChangeQueueButton.interactable = false;
			JoinExchangeMatchButton.interactable = true;
			DeclineExchangeMatchButton.interactable = true;
			tm.RestartTimer("JoinMatch");

			//should move this to test class
			if (UnityEngine.Debug.isDebugBuild)
			{
				var testArgs = Msf.Args.ExtractValue("-test");
				if (testArgs != null && testArgs.Equals("GuestLogin"))
				{
					UnityEngine.Debug.LogError("Test: ClientController.HandleMatchFound");

					JoinExchange();
				}
			}
		}

		private void HandleChangeQueue(IIncommingMessage message)
		{
			UnityEngine.Debug.Log("HandleChangeQueue: Enabling Change Queue Button");
			//ExchangeMatchMakingPacket packet = message.Deserialize(new ExchangeMatchMakingPacket());
			ChangeQueueButton.interactable = true;
		}

		private void HandleMatchReady(IIncommingMessage message)
		{
			UnityEngine.Debug.Log("HandleMatchReady: Starting Match Exchange");
			SceneManager.LoadScene("DeviationClient - Match");
		}

		private void HandleMatchDisbanded(IIncommingMessage message)
		{
			UnityEngine.Debug.Log("HandleMatchDisbanded: Match Disbanded, Looking For New Match");

			//MatchFoundPacket packet = message.Deserialize(new MatchFoundPacket());
			LeaveQueueButton.interactable = true;
			JoinQueueButton.interactable = false;
			ChangeQueueButton.interactable = false;
			JoinExchangeMatchButton.interactable = false;
			DeclineExchangeMatchButton.interactable = false;
			tm.PauseTimer("JoinMatch");
			ClientDataRepository.Instance.Exchange = null;
			ClientDataRepository.Instance.HasExchange = false;
		}
	}
}
