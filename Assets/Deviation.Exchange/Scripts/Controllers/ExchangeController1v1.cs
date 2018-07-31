using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using Assets.Scripts.Utilities;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.Events;
using Assets.Deviation.Exchange.Scripts.Controllers.ExchangeControllerHelpers;

public interface IExchangeController1v1
{
	UnityAction<ExchangeState> OnExchangeStateChange { get; set; }
	ExchangeState ExchangeState { get; set; }
	void ServerResponse(int peerId);
	void ResetExchange();
	IExchangePlayer GetRoundWinner();
}

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(TimerManager))]
[RequireComponent(typeof(CoroutineManager))]
[RequireComponent(typeof(ClientExchangeControllerHelper))]
[RequireComponent(typeof(ServerExchangeControllerHelper))]
public class ExchangeController1v1 : NetworkBehaviour, IExchangeController1v1
{
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
				Debug.Log(_exchangeState);
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

	public static bool _allPlayersConnected;
	public static bool AllPlayersConnected
	{
		get
		{
			return _allPlayersConnected;
		}
		set
		{
			_allPlayersConnected = value;
			OnAllPlayersConnectedChange?.Invoke(value);			
		}
	}
	public static UnityAction<bool> OnAllPlayersConnectedChange { get; set; }

	private IExchangePlayer [] _exchangePlayers;
	private TimerManager tm;

	private IClientExchangeControllerHelper client;
	private IServerExchangeControllerHelper server;

	private void Awake()
	{
		ExchangeState = ExchangeState.Setup;

		client = GetComponent<ClientExchangeControllerHelper>();
		server = GetComponent<ServerExchangeControllerHelper>();
		client.Init();
		server.Init();

		if (gameObject.tag != "GameController")
		{
			gameObject.tag = "GameController";
		}
		
		OnExchangeStateChange += ExchangeStateChange;
		OnAllPlayersConnectedChange += (value) => server.Setup();
	}

	public void Start()
	{
		tm = GetComponent<TimerManager>();
		tm.AddTimer("ExchangeTimer", Round_Time_Seconds);
		tm.AddTimer("RoundEndTimer", Round_End_Time_Seconds);
	}

	private void ExchangeStateChange(ExchangeState value)
	{
		switch (ExchangeState)
		{
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
				server.Battle();
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

	public void ServerResponse(int peerId)
	{
		server.ServerResponse(peerId);
	}
}
