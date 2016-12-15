namespace Assets.Scripts.Enum
{
	public enum ModuleType
	{
		//heals you, heals team, use health instead of energy for attack, etc.
		Life,

		//regain energy, low cost energy attack, energy related attack (50% energy == damage modifier), etc.
		Energy,

		//steal resources, steal rare resources, find rare abilitiess, copy abilities, etc.
		Scavenge,

		//speed up, slow enemy, barriers, blockers, shields, copy self
		Manipulate,

		//fire, water, air, earth, wide area, focused damage, buffs, enemy debuffs
		Elemental,

		//Default type
		Default,
	}
}
