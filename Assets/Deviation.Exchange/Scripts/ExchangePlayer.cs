using Assets.Scripts.Enum;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Library;
using Assets.Scripts.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExchangePlayer : NetworkBehaviour, IExchangePlayer
{
	private const int INITIAL_COLUMN = 2;
	private const int INITIAL_ROW = 2;

	[SyncVar]
	private bool _initialized;
	[SyncVar]
	private BattlefieldZone _zone;

	private IKit _kit;
	private Energy _energy;
	private Health _health;
	private Mover _mover;
	private Status _status;

	public Energy Energy { get { return _energy; } }
	public Health Health { get { return _health; } }
	public Mover Mover { get { return _mover; } }
	public Status Status { get { return _status; } }
	public BattlefieldZone Zone { get { return _zone; } }
	public bool Initialized { get { return _initialized;  } }
	public IKit Kit { get { return _kit; } }
	public Vector3 Position { get { return transform.position; } }
	public Quaternion Rotation { get { return transform.rotation; } }

	private ExchangeBattlefieldController bc;
	private TimerManager tm;

	private Dictionary<int, bool> _actionsDisabled;

	public void Start()
	{
		bc = FindObjectOfType<ExchangeBattlefieldController>();
		tm = FindObjectOfType<TimerManager>();
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

	public void Init(int energyMin, int energyMax, float energyRate, int healthMin, int healthMax, BattlefieldZone zone, string kitName)
	{
		ServerInit(energyMin, energyMax, energyRate, healthMin, healthMax, zone, kitName);
		RpcInit(zone, kitName);
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
		IExchangeAction action = _kit.GetCurrentModule().Actions[actionNumber];

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
		IExchangeAction action = _kit.GetCurrentModule().Actions[actionNumber];
		action.InitiateAttack(bc, _zone);
	}

	private void ServerInit(int energyMin, int energyMax, float energyRate, int healthMin, int healthMax, BattlefieldZone zone, string kitName)
	{
		if (!isServer)
		{
			return;
		}

		_energy.Init(energyMin, energyMax, energyRate);
		_health.Init(healthMin, healthMax);
		_zone = zone;
		_kit = KitLibrary.GetKitInstance(kitName);
		_kit.Player = this;
		_initialized = true;
		bc.SetGridspaceOccupied(INITIAL_ROW, INITIAL_COLUMN, true, zone);
	}

	[ClientRpc]
	private void RpcInit(BattlefieldZone zone, string kitName)
	{
		_mover.Init(zone, INITIAL_ROW, INITIAL_COLUMN, 1f);
		_kit = KitLibrary.GetKitInstance(kitName);
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
		IModule currentModule = _kit.GetCurrentModule();

		for (int i = 0; i < kit.ModuleCount; i++)
		{
			foreach (IExchangeAction action in currentModule.Actions)
			{
				tm.AddAttackTimer(action.Name, action.Cooldown, (int) zone);
			}

			currentModule = kit.GetRightModule();
			kit.CycleModuleRight();
		}
	}
}
