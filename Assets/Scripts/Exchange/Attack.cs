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

		public float HealthDrainModifier;
		public float EnergyDrainModifier;
		public float HealthRecoilModifier;
		public float EnergyRecoilModifier;

		public Attack(float healthDrainModifier, float energyDrainModifier, float healthRecoilModifier, float energyRecoilModifier)
		{
			Attacker = null;
			Defender = null;
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

		public void InitiateAttack()
		{
			UpdateHealth();
			UpdateEnergy();
		}

		private void UpdateHealth()
		{
			int healthRecoil = (int) (HealthRecoilModifier * -100);
			int healthDrain = (int) (HealthDrainModifier * -100);

			Attacker.AddHealth(healthRecoil);
			Defender.AddHealth(healthDrain);
		}

		private void UpdateEnergy()
		{
			int energyRecoil = (int) (EnergyRecoilModifier * -100);
			int energyDrain = (int) (EnergyDrainModifier * -100);

			Attacker.AddEnergy(energyRecoil);
			Defender.AddEnergy(energyDrain);
		}
	}
}
