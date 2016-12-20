using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Exchange
{
	public struct Attack
	{
		//this is the attack initiator of the attack, recoil applies to them
		public Player Attacker;

		//this is the defender of the attack, drain applies to them
		public Player Defender;

		//this is the base damage of the attack
		public int BaseDamage;

		//these modifiers apply to the basedamage and only affect the attacker
		public float HealthRecoilModifier;
		public float EnergyRecoilModifier;

		//these modifiers apply to the basedamage and only affect the defender
		public float HealthDrainModifier;
		public float EnergyDrainModifier;
		
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
		public void SetAttacker(Player attacker)
		{
			Attacker = attacker;
		}

		//sets the defender
		public void SetDefender(Player defender)
		{
			Defender = defender;
		}

		//this is an attack that hits applies the attack to both the attacker and defender
		public void InitiateAttack(Player attacker = null, Player defender = null)
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
				UpdateEnergy(Attacker, EnergyRecoilModifier);
				UpdateHealth(Attacker, HealthRecoilModifier);
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
				UpdateEnergy(Defender, EnergyDrainModifier);
				UpdateHealth(Defender, HealthDrainModifier);
			}
			else
			{
				throw new Exception("Defender not set");
			}
		}

		//this helper function adds energy to the target
		private void UpdateEnergy(Player target, float modifier)
		{
			target.AddEnergy((int) (modifier * BaseDamage));
		}

		//this helper function adds health to the target
		private void UpdateHealth(Player target, float modifier)
		{
			target.AddHealth((int) (modifier * BaseDamage));
		}
	}
}
