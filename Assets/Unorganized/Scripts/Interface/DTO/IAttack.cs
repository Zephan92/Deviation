using Assets.Scripts.Exchange;
using Assets.Scripts.Interface.Exchange;

namespace Assets.Scripts.Interface.DTO
{
	public interface IAttack
	{
		IPlayer Attacker { get; set; }
		IPlayer Defender { get; set; }
		int BaseDamage { get; set; }
		float HealthRecoilModifier { get; set; }
		float EnergyRecoilModifier { get; set; }
		float HealthDrainModifier { get; set; }
		float EnergyDrainModifier { get; set; }

		void SetAttacker(IPlayer attacker);
		void SetDefender(IPlayer defender);

		void InitiateAttack(IPlayer attacker = null, IPlayer defender = null);
		void InitiateRecoil();
		void InitiateDrain();
	}
}
