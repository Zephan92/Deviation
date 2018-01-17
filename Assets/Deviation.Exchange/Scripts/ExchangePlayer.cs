using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Enum;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Library;
using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
	private Energy _energy;
	private Health _health;
	private Mover _mover;
	private Status _status;

	public int PeerId { get { return _peerId; } set { _peerId = value; } }
	public long PlayerId { get { return _playerId; } set { _playerId = value; } }

	public Energy Energy { get { return _energy; } }
	public Health Health { get { return _health; } }
	public Mover Mover { get { return _mover; } }
	public Status Status { get { return _status; } }
	public BattlefieldZone Zone { get { return _zone; } }
	public BattlefieldZone EnemyZone { get { return _zone == BattlefieldZone.Left ? BattlefieldZone.Right : BattlefieldZone.Left; } }
	public bool Initialized { get { return _initialized;  } }
	public IKit Kit { get { return _kit; } }
	public Vector3 Position { get { return transform.position; } }
	public Quaternion Rotation { get { return transform.rotation; } }

	private ExchangeBattlefieldController bc;
	private ITimerManager tm;
	private IGridManager gm;

	private Dictionary<int, bool> _actionsDisabled;

	public void Start()
	{
		bc = FindObjectOfType<ExchangeBattlefieldController>();
		tm = FindObjectOfType<TimerManager>();
		gm = FindObjectOfType<GridManager>();
		_energy = GetComponent<Energy>();
		_health = GetComponent<Health>();
		_mover = GetComponent<Mover>();
		_status = GetComponent<Status>();
		_actionsDisabled = new Dictionary<int, bool>();
		_actionsDisabled.Add(0, false);
		_actionsDisabled.Add(1, false);
		_actionsDisabled.Add(2, false);
		_actionsDisabled.Add(3, false);
	}

	public void FixedUpdate()
	{
		if (_initialized)
		{
			_energy.Restore();
			_health.Restore();
		}
	}

	public void Init(int energyMin, int energyMax, float energyRate, int healthMin, int healthMax, BattlefieldZone zone, long playerId, Guid[] actionGuids)
	{
		_kit = new Kit(actionGuids);
		_playerId = playerId;
		ServerInit(energyMin, energyMax, energyRate, healthMin, healthMax, zone);
		RpcInit(zone, _kit.ActionsNames);
	}

	public void DisableAction(bool disabled, int actionNumber = -1)
	{
		if (actionNumber == -1)
		{
			foreach (int keyNumber in _actionsDisabled.Keys)
			{
				_actionsDisabled[keyNumber] = disabled;
			}
		}
		else
		{
			_actionsDisabled[actionNumber] = disabled;
		}
	}

	public bool Action(int actionNumber)
	{
		if (!isLocalPlayer || _actionsDisabled[actionNumber])
		{
			return false;
		}

		bool success = false;
		IExchangeAction action = _kit.Actions[actionNumber];

		int attackCost = (int)(action.Attack.EnergyRecoilModifier * action.Attack.BaseDamage);
		int potentialEnergy = _energy.Current + attackCost;
		if (tm.TimerUp(action.Name, (int)_zone) && potentialEnergy >= _energy.Min)
		{			
			CmdAction(actionNumber);
			tm.StartTimer(action.Name, (int)_zone);
			success = true;
		}

		return success;
	}

	[Command]
	private void CmdAction(int actionNumber)
	{
		IExchangeAction action = _kit.Actions[actionNumber];
		action.InitiateAttack(bc, _zone);
	}

	private void ServerInit(int energyMin, int energyMax, float energyRate, int healthMin, int healthMax, BattlefieldZone zone)
	{
		if (!isServer)
		{
			return;
		}
		_mover.Init(zone, 1f);
		_energy.Init(energyMin, energyMax, energyRate);
		_health.Init(healthMin, healthMax);
		_zone = zone;
		_kit.Player = this;
		_initialized = true;
		gm.SetGridspaceOccupied(new GridCoordinate(ExchangeConstants.PLAYER_INITIAL_ROW, ExchangeConstants.PLAYER_INITIAL_COLUMN, zone, true), true, zone);
	}

	[ClientRpc]
	private void RpcInit(BattlefieldZone zone, string[] actionNames)
	{
		_mover.Init(zone, 1f);
		_kit = new Kit(actionNames);
		_kit.Player = this;

		CreateTimersForKitActions(zone);

		if (!isLocalPlayer)
		{
			return;
		}
		transform.position = bc.GetBattlefieldCoordinates(zone);

		switch (zone)
		{
			case BattlefieldZone.Left:
				Camera.main.transform.position = new Vector3(-2, 4, -2);
				Camera.main.transform.rotation = Quaternion.Euler(new Vector3(30,60,0));
				transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
				break;
			case BattlefieldZone.Right:
				Camera.main.transform.position = new Vector3(12, 4, -2);
				Camera.main.transform.rotation = Quaternion.Euler(new Vector3(30,-60,0));
				transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
				break;
		}
	}

	//create a timer for each action in each module
	private void CreateTimersForKitActions(BattlefieldZone zone)
	{
		IKit kit = _kit;

		for (int i = 0; i < kit.Actions.Length; i++)
		{
			foreach (IExchangeAction action in kit.Actions)
			{
				tm.AddAttackTimer(action.Name, action.Cooldown, (int) zone);
			}
		}
	}
}
