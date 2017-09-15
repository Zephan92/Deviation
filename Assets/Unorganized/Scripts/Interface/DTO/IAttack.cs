using System.Collections.Generic;

namespace Assets.Scripts.Interface.DTO
{
	public interface IAttack
	{
		int BaseDamage { get; set; }
		float HealthRecoilModifier { get; set; }
		float EnergyRecoilModifier { get; set; }
		float HealthDrainModifier { get; set; }
		float EnergyDrainModifier { get; set; }

		void InitiateAttack(List<IExchangePlayer> allies, List<IExchangePlayer> enemies);
		void InitiateAttack(List<IExchangePlayer> targets, AttackAlignment alignment);
		int GetHealthCost(AttackAlignment alignment);
		int GetEnergyCost(AttackAlignment alignment);
	}
}
