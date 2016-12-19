using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Exchange
{
	public struct Attack
	{
		public Player Attacker;
		public Player Defender;

		public int BaseDamage;

		public float HealthDrainModifier;
		public float EnergyDrainModifier;
		public float HealthRecoilModifier;
		public float EnergyRecoilModifier;

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

		public void SetAttacker(Player attacker)
		{
			Attacker = attacker;
		}

		public void SetDefender(Player defender)
		{
			Defender = defender;
		}

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

		private void UpdateEnergy(Player target, float modifier)
		{
			target.AddEnergy((int) (modifier * BaseDamage));
		}

		private void UpdateHealth(Player target, float modifier)
		{
			target.AddHealth((int) (modifier * BaseDamage));
		}
	}
}
