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

namespace Assets.Deviation.Exchange.Scripts.Client
{
	public class ClientController : MonoBehaviour
	{
		public PlayerAccountPacket playerAccount;
		public Button JoinQueueButton;
		public Button LeaveQueueButton;
		public Button ChangeQueueButton;
		public Button JoinExchangeMatchButton;
		public Button DeclineExchangeMatchButton;

		public Text Timer;

		public bool lookingForMatch = false;
		private int exchangeId = -1;
		private TimerManager tm;

		public void Awake()
		{
			tm = GetComponent<TimerManager>();
			tm.AddAttackTimer("JoinMatch", 10f);

			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondMatchFound, HandleMatchFound);
			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondChangeQueuePool, HandleChangeQueue);
		}

		public void Start()
		{
			GetPlayerAccount();
		}

		public void Update()
		{
			if (exchangeId > 0)
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
			if (exchangeId > 0)
			{
				Timer.text = "Timer: " + (int)tm.GetRemainingCooldown("JoinMatch");
			}
			else
			{
				Timer.text = "Timer: " + 10;
			}
		}

		private void GetPlayerAccount()
		{
			UnityEngine.Debug.Log("Get Player Account");

			Msf.Client.Connection.SendMessage((short)ExchangePlayerOpCodes.GetPlayerAccount, Msf.Client.Auth.AccountInfo.Username, (status, response) =>
			{
				playerAccount = response.Deserialize(new PlayerAccountPacket());
				JoinQueueButton.interactable = true;
			});
		}

		public void HandleMatchFound(IIncommingMessage message)
		{
			MatchFoundPacket packet = message.Deserialize(new MatchFoundPacket());
			LeaveQueueButton.interactable = false;
			ChangeQueueButton.interactable = false;
			JoinExchangeMatchButton.interactable = true;
			DeclineExchangeMatchButton.interactable = true;
			exchangeId = packet.ExchangeId;
			tm.RestartTimer("JoinMatch");
		}

		public void HandleChangeQueue(IIncommingMessage message)
		{
			ExchangeMatchMakingPacket packet = message.Deserialize(new ExchangeMatchMakingPacket());
			ChangeQueueButton.interactable = true;
		}

		public void SearchForExchangeMatch()
		{
			var packet = new ExchangeMatchMakingPacket(playerAccount.Id, QueueTypes.Exchange1v1, PlayerClass.Default);

			Msf.Connection.SendMessage((short)Exchange1v1MatchMakingOpCodes.RequestJoinQueue, packet, (status, data) =>
			{
				if (status == ResponseStatus.Error)
				{
					UnityEngine.Debug.LogErrorFormat("RequestJoinQueue failed to join exchange queue. Error {1}", data);
				}
				else if (status == ResponseStatus.Success)
				{
					UnityEngine.Debug.Log("Looking for Match");
					lookingForMatch = true;
					LeaveQueueButton.interactable = true;
					JoinQueueButton.interactable = false;
				}
			});
		}

		public void ChangeQueue()
		{
			var packet = new ExchangeMatchMakingPacket(playerAccount.Id, QueueTypes.Exchange1v1, PlayerClass.Default);

			Msf.Connection.SendMessage((short)Exchange1v1MatchMakingOpCodes.RequestChangeQueuePool, packet, (status, data) =>
			{
				if (status == ResponseStatus.Error)
				{
					UnityEngine.Debug.LogErrorFormat("RequestJoin1v1Queue failed to leave exchange queue. Error {1}", data);
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
			var packet = new ExchangeMatchMakingPacket(playerAccount.Id, QueueTypes.Exchange1v1, PlayerClass.Default);

			Msf.Connection.SendMessage((short)Exchange1v1MatchMakingOpCodes.RequestLeaveQueue, packet, (status, data) =>
			{
				if (status == ResponseStatus.Error)
				{
					UnityEngine.Debug.LogErrorFormat("RequestJoin1v1Queue failed to leave exchange queue. Error {1}", data);
				}
				else if (status == ResponseStatus.Success)
				{
					UnityEngine.Debug.Log("Leaving Queue");
					lookingForMatch = false;
					LeaveQueueButton.interactable = false;
					JoinQueueButton.interactable = true;
				}
			});
		}

		public void JoinExchange()
		{//todo open standalone with correct params
			JoinExchangeMatchButton.interactable = false;
			DeclineExchangeMatchButton.interactable = false;

			//wait for other player
			StartExchange(exchangeId);
			//should communicate differently but this works
			var commandLineArgs = "-show-screen-selector false -screen-height 900 -screen-width 1600 -exchangeId " + exchangeId;
			var exePath = Environment.GetEnvironmentVariable("DeviationStandalone", EnvironmentVariableTarget.User) + "/DeviationStandalone.exe";
			Process.Start(exePath, commandLineArgs);
		}

		private void StartExchange(int exchangeId)
		{
			Guid characterGuid = GetPlayerCharacter();
			ActionModulePacket module = GetPlayerActionModule();
			InitExchangePlayerPacket packet = new InitExchangePlayerPacket(exchangeId, playerAccount, characterGuid, module);
		}

		public void DeclineExchange()
		{
			JoinExchangeMatchButton.interactable = false;
			DeclineExchangeMatchButton.interactable = false;
			JoinQueueButton.interactable = true;
			exchangeId = -1;
			tm.RestartTimer("JoinMatch");

			//let server/other player know
		}

		private Guid GetPlayerCharacter()
		{
			return Guid.NewGuid();
		}

		private ActionModulePacket GetPlayerActionModule()
		{
			var q = new Guid("688b267a-fde1-4250-91a0-300aa3343147");
			var w = new Guid("dacb468b-658f-4daa-9400-cd3f005d06bd");
			var e = new Guid("d504df35-dc93-4f84-829e-01e202878341");
			var r = new Guid("36a1cf13-8b79-4800-8574-7cec0c405594");
			//this would go get the actions the player chose
			return new ActionModulePacket(q, w, e, r);
		}
	}
}
