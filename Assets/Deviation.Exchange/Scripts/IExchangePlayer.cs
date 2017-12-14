using Assets.Scripts.Enum;
using UnityEngine;
using System;
using Assets.Scripts.Interface.DTO;

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
	BattlefieldZone Zone { get; }
	Vector3 Position { get; }
	Quaternion Rotation { get; }
	void Init(int energyMin, int energyMax, float energyRate, int healthMin, int healthMax, BattlefieldZone zone, long playerId, Guid[] actionGuids);

	bool Action(int actionNumber);

	void DisableAction(bool disabled, int actionNumber = -1);
}