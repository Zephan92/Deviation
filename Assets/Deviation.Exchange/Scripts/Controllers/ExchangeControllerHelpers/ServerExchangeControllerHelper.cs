using Assets.Deviation.MasterServer.Scripts;
using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using Barebones.MasterServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Deviation.Exchange.Scripts.Controllers.ExchangeControllerHelpers
{
	public interface IServerExchangeControllerHelper : IExchangeControllerHelper
	{
		void ServerResponse(int peerId);
		void ResetExchange();
	}

	public class ServerExchangeControllerHelper : ExchangeControllerHelper, IServerExchangeControllerHelper
	{
		//private objects
		private ExchangeBattlefieldController bc;
		private TimerManager tm;
		private ICoroutineManager cm;

		//private fields
		private PlayerController[] _players;
		private IExchangePlayer[] _exchangePlayers;
		private ConcurrentDictionary<int, bool> _clientReady;
		private ExchangeDataEntry[] _exchangeDataEntries;
		private int _exchangeId;
		private bool _waitingForClients;
		private IEnumerator _coroutine;

		public override void Init()
		{
			base.Init(ExchangeControllerHelperType.Server);
		}

		public void Awake()
		{
			_exchangeId = Msf.Args.ExtractValueInt("-exchangeId");
			_exchangeDataEntries = new ExchangeDataEntry[2];
			_clientReady = new ConcurrentDictionary<int, bool>();
		}

		public new void Start()
		{
			base.Start();
			cm = GetComponent<CoroutineManager>();
			tm = GetComponent<TimerManager>();
			bc = FindObjectOfType<ExchangeBattlefieldController>();
		}

		[Server]
		public override void Setup()
		{
			base.Setup();

			if (_players == null)
			{
				_players = FindObjectsOfType<PlayerController>();
			}

			if (_exchangePlayers == null)
			{
				_exchangePlayers = FindObjectsOfType<ExchangePlayer>();
				foreach (var player in _exchangePlayers)
				{
					if (!_clientReady.ContainsKey(player.PeerId))
					{
						_clientReady.Add(player.PeerId, false);
					}
				}
			}

			WaitForClients(() => { ec.ExchangeState = ExchangeState.PreBattle; });
		}

		[Server]
		public override void PreBattle()
		{
			base.PreBattle();
			bc.Init();

			foreach (var player in _exchangePlayers)
			{
				int playerIndex = System.Array.IndexOf(_exchangePlayers, player);

				Msf.Server.Auth.GetPeerAccountInfo(player.PeerId, (info, error) => {
					if (info == null)
					{
						Debug.LogErrorFormat("GetPeerAccountInfo, failed to get username. Peerid: {0}. Error {1}", error, player.PeerId);
						return;
					}

					Msf.Connection.SendMessage(
						(short)ExchangePlayerOpCodes.GetExchangePlayerInfo,
						new ExchangePlayerPacket(_exchangeId, info.Username),
						(status, response) =>
						{
							ExchangeDataEntry exchangeDataEntry = response.Deserialize(new ExchangeDataEntry());
							_exchangeDataEntries[playerIndex] = exchangeDataEntry;
							BattlefieldZone zone = (BattlefieldZone)playerIndex;

							//todo Get kit
							IKit kit = new Kit();

							player.Init(0, 100, zone, exchangeDataEntry.Player.Id, kit);
						});
				});
			}

			WaitForClients(() => { ec.ExchangeState = ExchangeState.Begin; });
		}

		[Server]
		public override void Begin()
		{
			base.Begin();
			WaitForClients(() => { ec.ExchangeState = ExchangeState.Battle; });
		}

		[Server]
		public override void Battle_FixedUpdate()
		{
			base.Battle_FixedUpdate();

			bool playerDefeated = false;
			foreach (IExchangePlayer player in _exchangePlayers)
			{
				if (player.Health.Current == 0)
				{
					playerDefeated = true;
					break;
				}
			}

			if (tm.TimerUp("ExchangeTimer") || playerDefeated)
			{
				WaitForClients(() => { ec.ExchangeState = ExchangeState.End; });
			}
		}

		[Server]
		public override void End()
		{
			base.End();
			IExchangePlayer winner = ec.GetRoundWinner();

			if (winner != null)
			{
				winner.PlayerStats.Wins++;

				foreach (IExchangePlayer player in _exchangePlayers)
				{
					if (player != winner)
					{
						player.PlayerStats.Losses++;
					}
				}
			}
			else
			{
				foreach (IExchangePlayer player in _exchangePlayers)
				{
					player.PlayerStats.Draws++;
				}
			}

			if (ec.Round >= 1)
			{
				winner = ec.GetWinner();
				if (!ReferenceEquals(null, winner))
				{
					winner.PlayerStats.Winner = true;
				}
				else if (ec.Round >= 2)
				{
					//Draw
				}
				else
				{
					ec.Round++;
					ResetExchange();
				}
			}
			else
			{
				ec.Round++;
				ResetExchange();
			}
		}

		[Server]
		public override void End_FixedUpdate()
		{
			base.End_FixedUpdate();

			if (tm.TimerUp("RoundEndTimer"))
			{
				WaitForClients(() => { ec.ExchangeState = ExchangeState.PostBattle; });
			}
		}

		[Server]
		public override void PostBattle()
		{
			base.PostBattle();

			DateTime timestamp = DateTime.Now;
			foreach (var player in _exchangePlayers)
			{
				int playerIndex = System.Array.IndexOf(_exchangePlayers, player);
				ExchangeDataEntry exchangeDataEntry = _exchangeDataEntries[playerIndex];
				PlayerStatsPacket playerStats = player.PlayerStats.Packet;
				ExchangeResult result = new ExchangeResult(	exchangeDataEntry.ExchangeId, 
															timestamp, 
															exchangeDataEntry.Player, 
															playerStats, 
															exchangeDataEntry.Kit, 
															exchangeDataEntry.CharacterGuid);
				Msf.Connection.SendMessage((short)ExchangePlayerOpCodes.CreateExchangeResultData, result);
			}

			WaitForClients(() => { ec.ExchangeState = ExchangeState.Teardown; });
		}

		[Server]
		public void ResetExchange()
		{
			tm.RestartTimers();
			bc.ResetBattlefield();

			foreach (IExchangePlayer player in _exchangePlayers)
			{
				//TODO Get clips

				player.Init(0, 100, player.Zone, player.PlayerId, null);
				player.Kit.Reset();
			}

			WaitForClients(() => { ec.ExchangeState = ExchangeState.Begin; });
		}

		[Server]
		public void ServerResponse(int peerId)
		{
			_clientReady[peerId] = true;
		}

		private void WaitForClients(Action callback)
		{
			if (_waitingForClients)
			{
				return;
			}

			_waitingForClients = true;
			foreach (int peerId in _clientReady.GetKeysArray())
			{
				_clientReady[peerId] = false;
				foreach (PlayerController playerController in _players)
				{
					if (playerController.Player.PeerId == peerId)
					{
						playerController.RpcClientRequest();
					}
				}
			}

			cm.StartCoroutineThread_WhileLoop(WaitForClientsMethod, new object[] { callback }, 0f, ref _coroutine);
		}

		private void WaitForClientsMethod(object[] callbackParameters)
		{
			if (_coroutine == null)
			{
				return;
			}

			foreach (int peerId in _clientReady.GetKeysArray())
			{
				if (!_clientReady[peerId])
				{
					return;
				}
			}

			cm.StopCoroutineThread(ref _coroutine);
			_coroutine = null;
			_waitingForClients = false;
			((Action)callbackParameters[0]).Invoke();
		}
	}
}
