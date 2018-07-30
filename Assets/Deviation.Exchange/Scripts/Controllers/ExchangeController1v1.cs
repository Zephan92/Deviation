using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;
using Barebones.MasterServer;
using System;
using UnityEngine.SceneManagement;
using Assets.Deviation.Exchange.Scripts.Client;
using UnityEngine.Events;
using Assets.Deviation.MasterServer.Scripts;
using Assets.Deviation.Exchange.Scripts;

public interface IGameController
{
}

public interface IExchangeController1v1 : IGameController
{
	UnityAction<ExchangeState> OnExchangeStateChange { get; set; }
	ExchangeState ExchangeState { get; set; }
	void ServerResponse(int peerId);
	IExchangePlayer GetPlayerByPeerId(int peerId);
	void ResetExchange();
	IExchangePlayer GetWinner();
}

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(TimerManager))]
[RequireComponent(typeof(CoroutineManager))]
public class ExchangeController1v1 : NetworkBehaviour, IExchangeController1v1
{
	private const int Round_Time_Seconds = 90;
	private const int Round_End_Time_Seconds = 10;

	[SyncVar]
	private ExchangeState _exchangeState;
	public UnityAction<ExchangeState> OnExchangeStateChange { get; set; }

	public ExchangeState ExchangeState
	{
		get
		{
			return _exchangeState;
		}
		set
		{
			if (isServer)
			{
				_exchangeState = value;
				RpcOnExchangeStateChange(value);
				OnExchangeStateChange?.Invoke(value);
				Debug.Log(_exchangeState);
			}
		}
	}

	[ClientRpc]
	private void RpcOnExchangeStateChange(ExchangeState value)
	{
		OnExchangeStateChange?.Invoke(value);
	}

	public static bool AllPlayersConnected;
	private IExchangePlayer [] _exchangePlayers;
	private ExchangeDataEntry[] _playerInitData;
	private PlayerController[] Players { get; set; }
	private TimerManager tm;
	private ExchangeBattlefieldController bc;
	private ICoroutineManager cm;

	private Dictionary<ExchangeState,bool> _stateStatus;

	private ConcurrentDictionary<int, bool> clientReady;
	private bool _waitingForClients;
	private System.Collections.IEnumerator _coroutine;
	private int ExchangeDataId;
	private ExchangeNetworkManager etm;

	private void Awake()
	{
		etm = FindObjectOfType<ExchangeNetworkManager>();
		_playerInitData = new ExchangeDataEntry[2];
		clientReady = new ConcurrentDictionary<int, bool>();
		_stateStatus = new Dictionary<ExchangeState, bool>();
		if (gameObject.tag != "GameController")
		{
			gameObject.tag = "GameController";
		}
		
		ExchangeState = ExchangeState.Setup;

		ExchangeDataId = Msf.Args.ExtractValueInt("-exchangeId");

		OnExchangeStateChange += ExchangeStateChange;
	}

	private void ExchangeStateChange(ExchangeState value)
	{
		switch (ExchangeState)
		{
			case ExchangeState.Setup:
				break;
			case ExchangeState.PreBattle:
				break;
			case ExchangeState.Start:
				break;
			case ExchangeState.Battle:
				break;
			case ExchangeState.End:
				if (isServer)
				{
					IExchangePlayer winner = GetWinner();

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
				}
				break;
			case ExchangeState.PostBattle:
				break;
			case ExchangeState.Teardown:
				break;
			default:
				break;
		}
	}

	private void Start()
	{
		tm = GetComponent<TimerManager>();
		cm = GetComponent<CoroutineManager>();
		bc = FindObjectOfType<ExchangeBattlefieldController>();
		tm.AddTimer("ExchangeTimer", Round_Time_Seconds);
		tm.AddTimer("RoundEndTimer", Round_End_Time_Seconds);
	}

	private void FixedUpdate()
	{
		if (CheckStateCompleteStatus())
		{
			return;
		}

		switch (ExchangeState)
		{
			case ExchangeState.Setup:
				ExchangeSetup();
				break;
			case ExchangeState.PreBattle:
				ExchangePreBattle();
				break;
			case ExchangeState.Start:
				ExchangeStart();
				break;
			case ExchangeState.Battle:
				ExchangeBattle();
				break;
			case ExchangeState.End:
				ExchangeEnd();
				break;
			case ExchangeState.PostBattle:
				ExchangePostBattle();
				break;
			case ExchangeState.Teardown:
				ExchangeTeardown();
				break;
			default:
				break;
		}
	}

	public void ResetExchange()
	{
		if (!isServer)
		{
			return;
		}

		Debug.LogError("Reset Exchange");
		tm.RestartTimers();
		bc.ResetBattlefield();

		foreach (IExchangePlayer player in _exchangePlayers)
		{
			player.Init(0, 100, 0.001f, 0, 100, player.Zone, player.PlayerId, player.Kit.ActionsGuids);
		}

		_stateStatus = new Dictionary<ExchangeState, bool>();
		ExchangeState = ExchangeState.Start;
		RpcResetExchange();
	}

	[ClientRpc]
	private void RpcResetExchange()
	{
		tm.RestartTimers();
	}

	private void ExchangeSetup()
	{
		if (isServer && AllPlayersConnected)
		{
			ExchangeState = ExchangeState.PreBattle;
			if (Players == null)
			{
				Players = FindObjectsOfType<PlayerController>();
			}
		}

		if (isClient)
		{
			_stateStatus[ExchangeState] = true;
		}
	}

	private void ExchangePreBattle()
	{
		_exchangePlayers = FindObjectsOfType<ExchangePlayer>();

		if (isServer)
		{
			bc.Init();

			foreach (var player in _exchangePlayers)
			{
				int playerIndex = System.Array.IndexOf(_exchangePlayers, player);
				if (!clientReady.ContainsKey(player.PeerId))
				{
					clientReady.Add(player.PeerId, false);
				}

				Msf.Server.Auth.GetPeerAccountInfo(player.PeerId, (info, error) => {
					if (info == null)
					{
						Debug.LogErrorFormat("GetPeerAccountInfo, failed to get username. Peerid: {0}. Error {1}", error, player.PeerId);
						return;
					}

					Msf.Connection.SendMessage(
						(short)ExchangePlayerOpCodes.GetExchangePlayerInfo, 
						new ExchangePlayerPacket(ExchangeDataId, info.Username), 
						(status, response) =>
					{
						ExchangeDataEntry playerInitData = response.Deserialize(new ExchangeDataEntry());
						_playerInitData[playerIndex] = playerInitData;
						BattlefieldZone zone = (BattlefieldZone)playerIndex;
						player.Init(0, 100, 0.001f, 0, 100, zone, playerInitData.Player.Id, playerInitData.ActionGuids.GetActionGuids());
					});
				});
			}

			_stateStatus[ExchangeState] = true;
			WaitForClients(() => { ExchangeState = ExchangeState.Start; });
		}

		if (isClient)
		{
			_stateStatus[ExchangeState] = true;
		}
	}

	private void ExchangeStart()
	{
		tm.RestartTimer("ExchangeTimer");

		if (isServer)
		{
			_stateStatus[ExchangeState] = true;
			WaitForClients(() => { ExchangeState = ExchangeState.Battle; });
		}

		if (isClient)
		{
			_stateStatus[ExchangeState] = true;
		}
	}

	private void ExchangeBattle()
	{
		tm.UpdateCountdowns();

		if (isServer)
		{
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
				_stateStatus[ExchangeState] = true;
				WaitForClients(() => { tm.RestartTimer("RoundEndTimer"); ExchangeState = ExchangeState.End;	});
			}
		}
	}

	private void ExchangeEnd()
	{
		tm.UpdateCountdowns();

		if (isServer)
		{
			if (tm.TimerUp("RoundEndTimer"))
			{
				_stateStatus[ExchangeState] = true;
				WaitForClients(() => { ExchangeState = ExchangeState.PostBattle; });
			}
		}

		if (isClient)
		{
			_stateStatus[ExchangeState] = true;
		}
	}

	private void ExchangePostBattle()
	{
		if (isServer)
		{
			_stateStatus[ExchangeState] = true;

			DateTime timestamp = DateTime.Now;
			foreach (var player in _exchangePlayers)
			{
				int playerIndex = System.Array.IndexOf(_exchangePlayers, player);
				var initPacket = _playerInitData[playerIndex];
				PlayerStatsPacket playerStats = player.PlayerStats.Packet;
				ExchangeResult result = new ExchangeResult(initPacket.ExchangeId, timestamp, initPacket.Player, playerStats, initPacket.ActionGuids, initPacket.CharacterGuid);
				Msf.Connection.SendMessage((short)ExchangePlayerOpCodes.CreateExchangeResultData, result);
			}
			WaitForClients(() => { ExchangeState = ExchangeState.Teardown; });
		}

		if (isClient)
		{
			_stateStatus[ExchangeState] = true;
		}
	}

	private void ExchangeTeardown()
	{
		if (isClient)
		{
			etm.StopClient();
			SceneManager.LoadScene("DeviationClient - Results");
		}
	}

	public IExchangePlayer GetWinner()
	{
		IExchangePlayer winner = null;
		int maxHealth = 0;

		foreach (IExchangePlayer player in _exchangePlayers)
		{
			if (player.Health.Current > maxHealth)
			{
				winner = player;
				maxHealth = player.Health.Current;
			}
			else if(player.Health.Current == maxHealth)
			{
				winner = null;
			}
		}

		return winner;
	}

	public IExchangePlayer GetPlayerByPeerId(int peerId)
	{
		if (_exchangePlayers == null || _exchangePlayers.Length != ExchangeConstants.PLAYER_COUNT)
		{
			_exchangePlayers = FindObjectsOfType<ExchangePlayer>();
			Debug.LogError("Exchange Players Count: " + _exchangePlayers.Length);
			return null;
		}

		foreach (IExchangePlayer player in _exchangePlayers)
		{
			if (player.PeerId == peerId)
			{
				Debug.LogError("Exchange Players Peer Id: " + player.PeerId);
				return player;
			}
		}

		return null;
	}

	private bool CheckStateCompleteStatus()
	{
		if (_stateStatus.ContainsKey(ExchangeState))
		{
			return _stateStatus[ExchangeState];
		}
		else
		{
			_stateStatus.Add(ExchangeState, false);
			return false;
		}
	}


	private void WaitForClients(Action callback)
	{
		if (_waitingForClients)
		{
			return;
		}
		//Debug.LogErrorFormat("Starting to Wait For Clients");

		_waitingForClients = true;
		foreach (int peerId in clientReady.GetKeysArray())
		{
			clientReady[peerId] = false;
			foreach (PlayerController playerController in Players)
			{
				if (playerController.Player.PeerId == peerId)
				{
					//Debug.LogErrorFormat("Sending Request to {0}", peerId);
					playerController.RpcClientRequest();
				}
			}
		}

		cm.StartCoroutineThread_WhileLoop(WaitForClientsMethod, new object[] { callback }, 0f, ref _coroutine);
	}

	public void ServerResponse(int peerId)
	{
		clientReady[peerId] = true;
	}

	private void WaitForClientsMethod(object[] callbackParameters)
	{
		if (_coroutine == null)
		{
			return;
		}

		foreach (int peerId in clientReady.GetKeysArray())
		{
			if (!clientReady[peerId])
			{
				//Debug.LogErrorFormat("Peer: {0}. Was not ready", peerId);
				return;
			}
		}

		cm.StopCoroutineThread(ref _coroutine);
		_coroutine = null;
		//Debug.LogError("All Clients Are Ready");
		((Action)callbackParameters[0]).Invoke();
		_waitingForClients = false;
	}

}
