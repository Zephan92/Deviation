using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using System;

namespace Assets.Scripts.Exchange
{
	public struct Attack : IAttack
	{
		//this is the attack initiator of the attack, recoil applies to them
		public IPlayer Attacker { get; set; }

		//this is the defender of the attack, drain applies to them
		public IPlayer Defender { get; set; }

		//this is the base damage of the attack
		public int BaseDamage { get; set; }

		//these modifiers apply to the basedamage and only affect the attacker
		public float HealthRecoilModifier { get; set; }
		public float EnergyRecoilModifier { get; set; }

		//these modifiers apply to the basedamage and only affect the defender
		public float HealthDrainModifier { get; set; }
		public float EnergyDrainModifier { get; set; }

		public Attack( int baseDamage = 0, float healthDrainModifier = -1.0f, float energyDrainModifier = 0.0f, float healthRecoilModifier = 0.0f, float energyRecoilModifier = -1.0f)
		{
			Attacker = null;
			Defender = null;
			BaseDamage = baseDamage;
			HealthDrainModifier = healthDrainModifier;
			EnergyDrainModifier = energyDrainModifier;
			HealthRecoilModifier = healthRecoilModifier;
			EnergyRecoilModifier = energyRecoilModifier;
		}

		//sets the attacker
		public void SetAttacker(IPlayer attacker)
		{
			Attacker = attacker;
		}

		//sets the defender
		public void SetDefender(IPlayer defender)
		{
			Defender = defender;
		}

		//this is an attack that hits applies the attack to both the attacker and defender
		public void InitiateAttack(IPlayer attacker = null, IPlayer defender = null)
		{
			if (attacker != null)
			{
				SetAttacker(attacker);
			}

			if (defender != null)
			{
				SetDefender(defender);
			}
			
			InitiateRecoil();
			InitiateDrain();
		}

		//this is an attack that just affects the attacker
		public void InitiateRecoil()
		{
			if (Attacker != null)
			{
				UpdateEnergy(Attacker, GetDamage(EnergyRecoilModifier));
				UpdateHealth(Attacker, GetDamage(HealthRecoilModifier));
			}
			else
			{
				throw new Exception("Defender not set");
			}
		}

		//this is an attack that just affects the defender
		public void InitiateDrain()
		{
			if (Defender != null)
			{
				UpdateEnergy(Defender, GetDamage(EnergyDrainModifier));
				UpdateHealth(Defender, GetDamage(HealthDrainModifier));
			}
			else
			{
				throw new Exception("Defender not set");
			}
		}

		//this helper function adds energy to the target
		private void UpdateEnergy(IPlayer target, int damage)
		{
			target.AddEnergy(damage);
		}

		//this helper function adds health to the target
		private void UpdateHealth(IPlayer target, int damage)
		{
			target.AddHealth(damage);
		}

		public int GetDamage(float modifier)
		{
			return (int) (modifier * BaseDamage);
		}
	}
}
