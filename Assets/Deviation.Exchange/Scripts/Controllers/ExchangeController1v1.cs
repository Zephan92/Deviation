using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.Events;
using Assets.Deviation.Exchange.Scripts.Controllers.ExchangeControllerHelpers;

public interface IExchangeController1v1
{
	//properties
	UnityAction<ExchangeState> OnExchangeStateChange { get; set; }
	ExchangeState ExchangeState { get; set; }
	int Round { get; set; }

	//Methods
	void ServerResponse(int peerId);
	void ResetExchange();
	IExchangePlayer GetRoundWinner();
	IExchangePlayer GetWinner();
}

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(TimerManager))]
[RequireComponent(typeof(CoroutineManager))]
[RequireComponent(typeof(ClientExchangeControllerHelper))]
[RequireComponent(typeof(ServerExchangeControllerHelper))]
public class ExchangeController1v1 : NetworkBehaviour, IExchangeController1v1
{
	//public properties
	public int Round { get; set; }

	//private constants
	private const int Round_Time_Seconds = 90;
	private const int Round_End_Time_Seconds = 10;

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
				_exchangeState = value;
				RpcOnExchangeStateChange(value);
				OnExchangeStateChange?.Invoke(value);
			}
		}
	}
	public UnityAction<ExchangeState> OnExchangeStateChange { get; set; }

	[ClientRpc]
	private void RpcOnExchangeStateChange(ExchangeState value)
	{
		OnExchangeStateChange?.Invoke(value);
	}

	private ITimerManager tm;
	private IClientExchangeControllerHelper client;
	private IServerExchangeControllerHelper server;
	private IExchangePlayer[] _exchangePlayers;

	private void Awake()
	{
		ExchangeState = ExchangeState.Awake;

		client = GetComponent<ClientExchangeControllerHelper>();
		server = GetComponent<ServerExchangeControllerHelper>();
		client.Init();
		server.Init();

		if (gameObject.tag != "GameController")
		{
			gameObject.tag = "GameController";
		}
		
		OnExchangeStateChange += ExchangeStateChange;
	}

	public void Start()
	{
		tm = GetComponent<TimerManager>();
		tm.AddTimer("ExchangeTimer", Round_Time_Seconds);
		tm.AddTimer("RoundEndTimer", Round_End_Time_Seconds);
	}

	private void ExchangeStateChange(ExchangeState value)
	{
		switch (value)
		{
			case ExchangeState.Setup:
				server.Setup();
				break;
			case ExchangeState.PreBattle:
				_exchangePlayers = FindObjectsOfType<ExchangePlayer>();
				server.PreBattle();
				break;
			case ExchangeState.Begin:
				tm.RestartTimer("ExchangeTimer");
				server.Begin();
				break;
			case ExchangeState.End:
				server.End();
				break;
			case ExchangeState.PostBattle:
				tm.RestartTimer("RoundEndTimer");
				server.PostBattle();
				break;
			case ExchangeState.Teardown:
				client.Teardown();
				break;
		}
	}

	private void FixedUpdate()
	{
		switch (ExchangeState)
		{
			case ExchangeState.Battle:
				tm.UpdateCountdowns();
				server.Battle_FixedUpdate();
				break;
			case ExchangeState.End:
				tm.UpdateCountdowns();
				server.End_FixedUpdate();
				break;
		}
	}

	public void ResetExchange()
	{
		server.ResetExchange();
	}

	public IExchangePlayer GetRoundWinner()
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
			else if (player.Health.Current == maxHealth)
			{
				winner = null;
			}
		}

		return winner;
	}

	public IExchangePlayer GetWinner()
	{
		IExchangePlayer winner = null;
		int wins = 0;

		foreach (IExchangePlayer player in _exchangePlayers)
		{
			var stats = player.PlayerStats;

			if (stats.Wins + stats.Draws > wins)
			{
				winner = player;
				wins = stats.Wins + stats.Draws;
			}
			else if (stats.Wins + stats.Draws == wins)
			{
				winner = null;
			}
		}

		if (winner != null)
		{
			return winner;
		}
		else
		{
			int totalKnockouts = 0;

			foreach (IExchangePlayer player in _exchangePlayers)
			{
				var stats = player.PlayerStats;

				if (stats.KnockoutsDealt > totalKnockouts)
				{
					winner = player;
					totalKnockouts = stats.KnockoutsDealt;
				}
				else if (stats.KnockoutsDealt == totalKnockouts)
				{
					winner = null;
				}
			}
		}

		if (winner != null)
		{
			return winner;
		}
		else
		{
			int totalDamageDealt = 0;

			foreach (IExchangePlayer player in _exchangePlayers)
			{
				var stats = player.PlayerStats;

				if (stats.DamageDealt > totalDamageDealt)
				{
					winner = player;
					totalDamageDealt = stats.DamageDealt;
				}
				else if (stats.DamageDealt == totalDamageDealt)
				{
					winner = null;
				}
			}
		}

		if (winner != null)
		{
			return winner;
		}
		else
		{
			int totalAbilitiesUsed = 0;

			foreach (IExchangePlayer player in _exchangePlayers)
			{
				var stats = player.PlayerStats;

				if (stats.AbilitiesUsed > totalAbilitiesUsed)
				{
					winner = player;
					totalAbilitiesUsed = stats.AbilitiesUsed;
				}
				else if (stats.AbilitiesUsed == totalAbilitiesUsed)
				{
					winner = null;
				}
			}
		}

		return winner;
	}

	public void ServerResponse(int peerId)
	{
		server.ServerResponse(peerId);
	}
}
