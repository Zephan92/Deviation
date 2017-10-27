using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;
using Barebones.MasterServer;
using System;

public interface IGameController
{
}

public interface IExchangeController1v1 : IGameController
{
	ExchangeState ExchangeState { get; set; }
	IExchangePlayer [] ExchangePlayers { get; set; }
	void ResetExchange();
	void ServerResponse(int peerId);
}

public class ExchangeController1v1 : NetworkBehaviour, IExchangeController1v1
{
	[SyncVar]
	private ExchangeState _exchangeState;

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
				Debug.LogError(_exchangeState);
				_exchangeState = value;
			}
		}
	}

	public static bool AllPlayersConnected;
	public IExchangePlayer [] ExchangePlayers  { get; set; }
	private PlayerController[] Players { get; set; }
	private TimerManager tm;
	private ExchangeBattlefieldController bc;
	private ICoroutineManager cm;

	private Dictionary<ExchangeState,bool> _stateStatus;

	private ConcurrentDictionary<int, bool> clientReady;
	private bool _waitingForClients;
	private System.Collections.IEnumerator _coroutine;

	private void Awake()
	{
		clientReady = new ConcurrentDictionary<int, bool>();
		_stateStatus = new Dictionary<ExchangeState, bool>();
		if (gameObject.tag != "GameController")
		{
			gameObject.tag = "GameController";
		}
		
		ExchangeState = ExchangeState.Setup;
	}


	private void Start()
	{
		tm = GetComponent<TimerManager>();
		cm = GetComponent<CoroutineManager>();
		bc = FindObjectOfType<ExchangeBattlefieldController>();
		tm.AddAttackTimer("ExchangeTimer", 600);
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
		if (ExchangePlayers == null)
		{
			ExchangePlayers = FindObjectsOfType<ExchangePlayer>();
		}

		if (isServer)
		{
			bc.Init();
			foreach (var player in ExchangePlayers)
			{
				if (!clientReady.ContainsKey(player.PeerId))
				{
					clientReady.Add(player.PeerId, false);
				}
				
				player.Init(0, 100, 0.001f, 0, 100, (BattlefieldZone)System.Array.IndexOf(ExchangePlayers, player), "InitialKit");
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
			if (tm.TimerUp("ExchangeTimer"))
			{
				_stateStatus[ExchangeState] = true;
				WaitForClients(() => { ExchangeState = ExchangeState.End; });
			}
		}
	}

	private void ExchangeEnd()
	{
		if (isServer)
		{
			_stateStatus[ExchangeState] = true;
			WaitForClients(() => { ExchangeState = ExchangeState.PostBattle; });
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
			Application.Quit();
		}
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
