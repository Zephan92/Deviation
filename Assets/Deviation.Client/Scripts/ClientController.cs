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

namespace Assets.Deviation.Exchange.Scripts.Client
{
	public class ClientController : MonoBehaviour
	{
		public Button JoinQueueButton;
		public Button LeaveQueueButton;
		public Button ChangeQueueButton;
		public Button JoinExchangeMatchButton;
		public Button DeclineExchangeMatchButton;

		public Text Timer;

		private TimerManager tm;

		public void Awake()
		{
			tm = GetComponent<TimerManager>();
			tm.AddTimer("JoinMatch", 10.5f);

			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondMatchFound, HandleMatchFound);
			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondChangeQueuePool, HandleChangeQueue);
			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondMatchReady, HandleMatchReady);
			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondMatchDisbanded, HandleMatchDisbanded);
		}

		public void Start()
		{
			if (ClientDataController.Instance.PlayerAccount != null)
			{
				JoinQueueButton.interactable = true;
			}
			else
			{
				ClientDataController.Instance.PlayerAccountRecieved += () => 
				{
					JoinQueueButton.interactable = true;
				};
			}

			ClientDataController.Instance.State = ClientState.Client;

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

		public void Update()
		{
			if (ClientDataController.Instance.Exchange != null)
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
			if (ClientDataController.Instance.Exchange != null)
			{
				Timer.text = "Timer: " + (int) tm.GetRemainingCooldown("JoinMatch");
			}
			else
			{
				Timer.text = "Timer: 10";
			}
		}

		public void HandleMatchFound(IIncommingMessage message)
		{
			UnityEngine.Debug.Log("HandleMatchFound: Setting Player Account");
			MatchFoundPacket exchange = message.Deserialize(new MatchFoundPacket());
			exchange.Player1Id = ClientDataController.Instance.PlayerAccount.Id;
			exchange.Player2Id = -1;
			ClientDataController.Instance.Exchange = exchange;

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
			ClientDataController.Instance.Exchange = null;
		}

		public void SearchForExchangeMatch()
		{
			UnityEngine.Debug.Log("SearchForExchangeMatch");

			var packet = new ExchangeMatchMakingPacket(ClientDataController.Instance.PlayerAccount.Id, QueueTypes.Exchange1v1, PlayerClass.Default);

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

			var packet = new ExchangeMatchMakingPacket(ClientDataController.Instance.PlayerAccount.Id, QueueTypes.Exchange1v1, PlayerClass.Default);

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

			var packet = new ExchangeMatchMakingPacket(ClientDataController.Instance.PlayerAccount.Id, QueueTypes.Exchange1v1, PlayerClass.Default);

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

			Msf.Connection.SendMessage((short)Exchange1v1MatchMakingOpCodes.RequestJoinMatch, ClientDataController.Instance.Exchange, (status, data) => 
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

			Msf.Connection.SendMessage((short)Exchange1v1MatchMakingOpCodes.RequestDeclineMatch, ClientDataController.Instance.Exchange, (status, data) => {
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
					ClientDataController.Instance.Exchange = null;
				}
			});
		}
	}
}
