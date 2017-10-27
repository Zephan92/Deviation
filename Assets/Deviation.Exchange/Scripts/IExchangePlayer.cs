using Assets.Scripts.Enum;
using UnityEngine;

public interface IExchangePlayer
{
	bool Initialized { get; }
	int PeerId { get; set; }
	Energy Energy { get; }
	Health Health { get; }
	Mover Mover { get; }
	Status Status { get; }
	BattlefieldZone Zone { get; }
	Vector3 Position { get; }
	Quaternion Rotation { get; }
	void Init(int energyMin, int energyMax, float energyRate, int healthMin, int healthMax, BattlefieldZone zone, string kitName);

	bool Action(int actionNumber);

	void DisableAction(bool disabled, int actionNumber = -1);
}