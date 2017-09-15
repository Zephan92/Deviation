using System;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using UnityEngine.Networking;
using Assets.Deviation.Exchange.Scripts;
using UnityEngine;
using System.Collections.Generic;

public interface IGameController
{
}

public interface IExchangeController1v1 : IGameController
{
	ExchangeState ExchangeState { get; set; }
	IExchangePlayer [] ExchangePlayers { get; set; }
}

public class ExchangeController1v1 : NetworkBehaviour, IExchangeController1v1
{

	[SyncVar]
	private ExchangeState _exchangeState;

	public ExchangeState ExchangeState { get { return _exchangeState; } set { _exchangeState = value; } }
	public static bool AllPlayersConnected;
	public IExchangePlayer [] ExchangePlayers  { get; set; }

	private TimerManager tm;
	private ExchangeBattlefieldController bc;

	private Dictionary<ExchangeState,bool> _stateStatus;

	private void Awake()
	{
		_stateStatus = new Dictionary<ExchangeState, bool>();
		if (gameObject.tag != "GameController")
		{
			gameObject.tag = "GameController";
		}
		_exchangeState = ExchangeState.Setup;
	}

	// Use this for initialization
	void Start()
	{
		tm = GetComponent<TimerManager>();
		bc = FindObjectOfType<ExchangeBattlefieldController>();
		tm.AddAttackTimer("ExchangeTimer", 60);
	}

	// Update is called once per frame
	void FixedUpdate()
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

	private void ExchangeSetup()
	{
		tm.RestartTimer("ExchangeTimer");

		if (isServer)
		{
			if (AllPlayersConnected)
			{
				ExchangeState = ExchangeState.PreBattle;
			}
		}

		if (isClient)
		{
			_stateStatus[ExchangeState] = true;
		}
	}

	private void ExchangePreBattle()
	{
		ExchangePlayers = FindObjectsOfType<ExchangePlayer>();

		if (isServer)
		{
			foreach (var player in ExchangePlayers)
			{
				player.Init(0, 100, 0.001f, 0, 100, (BattlefieldZone) System.Array.IndexOf(ExchangePlayers, player), "InitialKit");
			}
			bc.Init();

			ExchangeState = ExchangeState.Start;
		}

		if (isClient)
		{
			_stateStatus[ExchangeState] = true;
		}
	}

	private void ExchangeStart()
	{
		if (isServer)
		{
			ExchangeState = ExchangeState.Battle;
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
				ExchangeState = ExchangeState.End;
			}
		}
	}

	private void ExchangeEnd()
	{
		if (isServer)
		{
			ExchangeState = ExchangeState.PostBattle;
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
			ExchangeState = ExchangeState.Teardown;
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
}
