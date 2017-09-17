public enum StatusEffect
{
	//health
	Burn,//damage over time
	Regenerate,//heal over time
	TODOEndurance,//cannot die when above 1 health (if hit again, they die)
	HealBlock,//cannot heal
	TODOShield,//absorb damage without taking it

	//energy
	EnergySap,//energy drain over time

	//player
	Silence,//all abilities
	Disable,//1 ability/module

	//mover
	Root,//cannot move

	//other client
	Invisible,//cannot be seen by enemy
}
