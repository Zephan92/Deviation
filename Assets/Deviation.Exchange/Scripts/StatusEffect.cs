public enum StatusEffect
{
	//health
	HealthRate,//damage over time
	HealBlock,//cannot heal
	DamageBlock,//cannot take damage
	TODOShield,//absorb damage without taking it
	TODOEndurance,//cannot die when above 1 health (if hit again, they die)

	//energy
	EnergyRate,//energy drain over time
	EnergyRecoilBlock,//cannot lose energy
	EnergyRegenBlock,//cannot gain energy

	//player
	Silence,//all abilities
	Disable,//1 ability/module

	//mover
	Root,//cannot move

	//other client
	Invisible,//cannot be seen by enemy
}
