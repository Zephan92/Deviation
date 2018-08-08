using Assets.Deviation.Exchange.Scripts;
using Assets.Deviation.Exchange.Scripts.DTO.Exchange;
using Assets.Deviation.MasterServer.Scripts;
using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Library;
using Assets.Scripts.Utilities;
using Barebones.MasterServer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Mover))]
[RequireComponent(typeof(Status))]
[RequireComponent(typeof(Splat))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerController))]
public class ExchangePlayer : NetworkBehaviour, IExchangePlayer
{
	[SyncVar]
	private long _playerId;
	[SyncVar]
	private int _peerId;
	[SyncVar]
	private bool _initialized;
	[SyncVar]
	private BattlefieldZone _zone;

	private IKit _kit;
	private Health _health;
	private Mover _mover;
	private Status _status;
	private Splat _splat;
	private PlayerStats _playerStats;

	public int PeerId { get { return _peerId; } set { _peerId = value; } }
	public long PlayerId { get { return _playerId; } set { _playerId = value; } }

	public Health Health { get { return _health; } }
	public Mover Mover { get { return _mover; } }
	public Status Status { get { return _status; } }
	public Splat Splat { get { return _splat; } }
	public PlayerStats PlayerStats { get { return _playerStats; } }
	public BattlefieldZone Zone { get { return _zone; } }
	public BattlefieldZone EnemyZone { get { return _zone == BattlefieldZone.Left ? BattlefieldZone.Right : BattlefieldZone.Left; } }
	public bool Initialized { get { return _initialized;  } }
	public IKit Kit { get { return _kit; } }
	public Vector3 Position { get { return transform.position; } }
	public Quaternion Rotation { get { return transform.rotation; } }

	private ExchangeBattlefieldController bc;
	private ITimerManager tm;
	private IGridManager gm;
	private ConcurrentDictionary<int, bool> _actionsDisabled;
	private Renderer [] _renderers;

	public void Start()
	{
		bc = FindObjectOfType<ExchangeBattlefieldController>();
		tm = FindObjectOfType<TimerManager>();
		gm = FindObjectOfType<GridManager>();
		_health = GetComponent<Health>();
		_mover = GetComponent<Mover>();
		_status = GetComponent<Status>();
		_splat = GetComponent<Splat>();
		_playerStats = GetComponent<PlayerStats>();
		_renderers = GetComponentsInChildren<Renderer>();
		_actionsDisabled = new ConcurrentDictionary<int, bool>();
		_actionsDisabled.Add(0, false);
		_actionsDisabled.Add(1, false);
		_actionsDisabled.Add(2, false);
		_actionsDisabled.Add(3, false);
	}

	public void FixedUpdate()
	{
		if (_initialized)
		{
			_health.Restore();
		}
	}

	public void Init(int healthMin, int healthMax, BattlefieldZone zone, long playerId, IKit kit)
	{
		_kit = kit;
		_playerId = playerId;
		ServerInit(healthMin, healthMax, zone);
		RpcInit(zone);
	}

	public void ToggleRenderer(bool value)
	{
		foreach (var rend in _renderers)
		{
			rend.enabled = value;
		}
	}

	public void DisableAction(bool disabled, int actionNumber = -1)
	{
		if (actionNumber == -1)
		{
			foreach (int keyNumber in _actionsDisabled.GetKeysArray())
			{
				_actionsDisabled[keyNumber] = disabled;
			}
		}
		else
		{
			_actionsDisabled[actionNumber] = disabled;
		}
	}

	public void Action(int actionNumber)
	{
		if (!isLocalPlayer || _actionsDisabled[actionNumber])
		{
			return;
		}

		CmdAction(actionNumber);
	}

	public void BasicAction(int actionNumber)
	{
		if (!isLocalPlayer || _actionsDisabled[actionNumber])
		{
			return;
		}

		CmdBasicAction();
	}

	[Command]
	private void CmdAction(int actionNumber)
	{
		IClip clip = _kit.Clips[actionNumber];

		if (!clip.Ready || clip.Remaining == 0)
		{
			return;
		}

		IExchangeAction action = clip.Pop();
		PlayerStats.AbilitiesUsed++;
		action.InitiateAttack(bc, _zone);
		clip.StartCooldown();
	}

	[Command]
	private void CmdBasicAction()
	{
		IBasicAction basic = _kit.BasicAction;

		if (!basic.Ready)
		{
			return;
		}

		IExchangeAction action = basic.Action;
		PlayerStats.AbilitiesUsed++;
		action.InitiateAttack(bc, _zone);
		basic.StartCooldown();
	}

	[Server]
	private void ServerInit(int healthMin, int healthMax, BattlefieldZone zone)
	{
		_mover.Init(zone, 1f);
		_health.Init(healthMin, healthMax);
		_zone = zone;
		_kit.Player = this;
		_initialized = true;
		_kit.Reset();
		gm.SetGridspaceOccupied(new GridCoordinate(ExchangeConstants.PLAYER_INITIAL_ROW, ExchangeConstants.PLAYER_INITIAL_COLUMN, zone, true), true, zone);
	}

	[ClientRpc]
	[Client]
	private void RpcInit(BattlefieldZone zone)
	{
		_mover.Init(zone, 1f);

		if (!isLocalPlayer)
		{
			return;
		}

		transform.position = bc.GetBattlefieldCoordinates(zone);

		switch (zone)
		{
			case BattlefieldZone.Left:
				Camera.main.transform.position = new Vector3(-2, 4, -2);
				Camera.main.transform.rotation = Quaternion.Euler(new Vector3(30, 60, 0));
				transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
				break;
			case BattlefieldZone.Right:
				Camera.main.transform.position = new Vector3(12, 4, -2);
				Camera.main.transform.rotation = Quaternion.Euler(new Vector3(30, -60, 0));
				transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
				break;
		}
	}

	public void Reinit()
	{
		ServerReinit();
		RpcReinit();
	}

	public void ServerReinit()
	{
		_mover.Init(Zone, 1f);
		_health.ReInit();
		_kit.Reset();
		gm.SetGridspaceOccupied(new GridCoordinate(ExchangeConstants.PLAYER_INITIAL_ROW, ExchangeConstants.PLAYER_INITIAL_COLUMN, Zone, true), true, Zone);
	}

	[ClientRpc]
	[Client]
	public void RpcReinit()
	{
		_mover.Init(Zone, 1f);

		if (!isLocalPlayer)
		{
			return;
		}

		transform.position = bc.GetBattlefieldCoordinates(Zone);

		switch (Zone)
		{
			case BattlefieldZone.Left:
				transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
				break;
			case BattlefieldZone.Right:
				transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
				break;
		}
	}
}
