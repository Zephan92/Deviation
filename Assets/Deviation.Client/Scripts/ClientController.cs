using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using UnityEngine;
using Barebones.MasterServer;
using Barebones.Networking;
using UnityEngine.UI;
using Assets.Scripts.Utilities;
using System.Collections;
using UnityEngine.SceneManagement;
using Assets.Deviation.MasterServer.Scripts;
using Assets.Deviation.MasterServer.Scripts.ResourceBank;
using Assets.Scripts.Interface;
using Assets.Deviation.Client.Scripts;

namespace Assets.Deviation.Exchange.Scripts.Client
{
	public class ClientController : ControllerBase
	{
		public Button JoinQueueButton;
		public Button LeaveQueueButton;
		public Button ChangeQueueButton;
		public Button JoinExchangeMatchButton;
		public Button DeclineExchangeMatchButton;

		public Text TimerText;
		public ResourceBag Bag;

		public override void Awake()
		{
			base.Awake();

			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondMatchFound, HandleMatchFound);
			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondChangeQueuePool, HandleChangeQueue);
			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondMatchReady, HandleMatchReady);
			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondMatchDisbanded, HandleMatchDisbanded);

			var clientUI = GameObject.Find("ClientUI");

			JoinQueueButton = clientUI.transform.Find("Search For Matches").GetComponent<Button>();
			LeaveQueueButton = clientUI.transform.Find("Leave Queue").GetComponent<Button>();
			ChangeQueueButton = clientUI.transform.Find("Change Queue").GetComponent<Button>();
			JoinExchangeMatchButton = clientUI.transform.Find("Join Exchange").GetComponent<Button>();
			DeclineExchangeMatchButton = clientUI.transform.Find("Decline Exchange").GetComponent<Button>();

			JoinQueueButton.onClick.AddListener(SearchForExchangeMatch);
			LeaveQueueButton.onClick.AddListener(LeaveQueue);
			ChangeQueueButton.onClick.AddListener(ChangeQueue);
			JoinExchangeMatchButton.onClick.AddListener(JoinExchange);
			DeclineExchangeMatchButton.onClick.AddListener(DeclineExchange);

			TimerText = clientUI.transform.Find("Timer").GetComponentInChildren<Text>();

			tm.AddTimer("JoinMatch", 10.5f);
			Bag = new ResourceBag();
			EnableJoinQueueButton();
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

		public override void Start()
		{
			base.Start();

			ClientDataRepository.Instance.State = ClientState.Client;

			if (UnityEngine.Debug.isDebugBuild)
			{
				var testArgs = Msf.Args.ExtractValue("-test");
				if (testArgs != null && testArgs.Equals("GuestLogin"))
				{
					UnityEngine.Debug.LogError("Test: ClientController");

					StartCoroutine(Test());
				}
			}
		}

		private IEnumerator Test()
		{
			yield return new WaitForSeconds(1f);
			if (UnityEngine.Debug.isDebugBuild)
				SearchForExchangeMatch();
		}

		public void GetResource()
		{
			Msf.Client.Connection.SendMessage((short)ResourceBankOpCodes.GetResources, (status, response) =>
			{
				if (status == ResponseStatus.Error)
				{
					UnityEngine.Debug.LogErrorFormat("GetResource failed. Error {1}", response);
				}
				else if (status == ResponseStatus.Success)
				{
					ResourcesPacket packet = response.Deserialize(new ResourcesPacket());
					var resource = packet.Resources;
					Bag.AddResource(resource);
				}
			});
		}

		public void GetResourceBag()
		{
			UnityEngine.Debug.Log(Bag);
		}

		public void GetResourceBank()
		{
			Msf.Client.Connection.SendMessage((short)ResourceBankOpCodes.GetResourceBank, (status, response) =>
			{
				if (status == ResponseStatus.Error)
				{
					UnityEngine.Debug.LogErrorFormat("GetResourceBank failed. Error {1}", response);
				}
				else if (status == ResponseStatus.Success)
				{
					ResourceBankPacket packet = response.Deserialize(new ResourceBankPacket());
					var resourceBank = packet.ResourceBank;
					UnityEngine.Debug.Log(resourceBank.ToString());
				}
			});
		}

		public override void Update()
		{
			base.Update();

			if (ClientDataRepository.Instance.HasExchange)
			{
				tm.UpdateCountdowns();

				if (tm.TimerUp("JoinMatch"))
				{
					DeclineExchange();
				}
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			if (ClientDataRepository.Instance.HasExchange)
			{
				TimerText.text = "Timer: " + (int) tm.GetRemainingCooldown("JoinMatch");
			}
			else
			{
				TimerText.text = "Timer: 10";
			}
		}

		public void HandleMatchFound(IIncommingMessage message)
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

		public void HandleChangeQueue(IIncommingMessage message)
		{
			UnityEngine.Debug.Log("HandleChangeQueue: Enabling Change Queue Button");
			//ExchangeMatchMakingPacket packet = message.Deserialize(new ExchangeMatchMakingPacket());
			ChangeQueueButton.interactable = true;
		}

		public void HandleMatchReady(IIncommingMessage message)
		{
			UnityEngine.Debug.Log("HandleMatchReady: Starting Match Exchange");
			SceneManager.LoadScene("DeviationClient - Match");
		}

		public void HandleMatchDisbanded(IIncommingMessage message)
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
	}
}
