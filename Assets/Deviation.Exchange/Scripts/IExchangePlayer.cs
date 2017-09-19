using Assets.Scripts.Enum;

public interface IExchangePlayer
{
	bool Initialized { get; }
	Energy Energy { get; }
	Health Health { get; }
	Mover Mover { get; }
	Status Status { get; }
	BattlefieldZone Zone { get; }

	void Init(int energyMin, int energyMax, float energyRate, int healthMin, int healthMax, BattlefieldZone zone, string kitName);

	bool Action(int actionNumber);

	void DisableAction(bool disabled, int actionNumber = -1);
}