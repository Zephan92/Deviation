using Assets.Scripts.Enum;
using UnityEngine;
using System;
using Assets.Scripts.Interface.DTO;
using Assets.Deviation.Exchange.Scripts;

public interface IExchangePlayer
{
	bool Initialized { get; }
	int PeerId { get; set; }
	long PlayerId { get; set; }
	IKit Kit { get; }
	Energy Energy { get; }
	Health Health { get; }
	Mover Mover { get; }
	Status Status { get; }
	Splat Splat { get; }
	PlayerStats PlayerStats { get; }
	BattlefieldZone Zone { get; }
	BattlefieldZone EnemyZone { get; }
	Vector3 Position { get; }
	Quaternion Rotation { get; }

	void Init(int energyMin, int energyMax, float energyRate, int healthMin, int healthMax, BattlefieldZone zone, long playerId, Guid[] actionGuids);
	bool Action(int actionNumber);
	void DisableAction(bool disabled, int actionNumber = -1);
	void ToggleRenderer(bool value);
}