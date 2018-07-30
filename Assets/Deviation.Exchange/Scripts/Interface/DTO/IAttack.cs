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

		void InitiateAttack(IExchangePlayer provoker, List<IExchangePlayer> allies, List<IExchangePlayer> enemies);
		void InitiateAttack(IExchangePlayer provoker, List<IExchangePlayer> targets, AttackAlignment alignment);
		void ApplyEffect(List<IExchangePlayer> targets, StatusEffect effect, float timeout, float rate = 0f);
		int GetHealthCost(AttackAlignment alignment);
		int GetEnergyCost(AttackAlignment alignment);
	}
}
