using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DTO.Exchange
{
	public struct Attack : IAttack
	{
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
			BaseDamage = baseDamage;
			HealthDrainModifier = healthDrainModifier;
			EnergyDrainModifier = energyDrainModifier;
			HealthRecoilModifier = healthRecoilModifier;
			EnergyRecoilModifier = energyRecoilModifier;
		}

		public void InitiateAttack(IExchangePlayer provoker, List<IExchangePlayer> allies, List<IExchangePlayer> enemies)
		{
			InitiateAttack(provoker, allies, AttackAlignment.Allies);
			InitiateAttack(provoker, enemies, AttackAlignment.Enemies);
		}

		public void ApplyEffect(List<IExchangePlayer> targets, StatusEffect effect, float timeout, float rate = 0f)
		{
			foreach (IExchangePlayer player in targets)
			{
				player.Status.ApplyEffect(effect, timeout, rate);
			}
		}

		public void InitiateAttack(IExchangePlayer provoker, List<IExchangePlayer> targets, AttackAlignment alignment)
		{
			switch (alignment)
			{
				case AttackAlignment.Allies:
					DeliverDamage(provoker, targets, EnergyRecoilModifier, HealthRecoilModifier);
					break;
				case AttackAlignment.Enemies:
					DeliverDamage(provoker, targets, EnergyDrainModifier, HealthDrainModifier);
					break;
			}
		}

		public int GetHealthCost(AttackAlignment alignment)
		{
			switch (alignment)
			{
				case AttackAlignment.Allies:
					return GetDamage(HealthRecoilModifier);
				case AttackAlignment.Enemies:
					return GetDamage(HealthDrainModifier);
				default:
					return 0;
			}
		}

		public int GetEnergyCost(AttackAlignment alignment)
		{
			switch (alignment)
			{
				case AttackAlignment.Allies:
					return GetDamage(EnergyRecoilModifier);
				case AttackAlignment.Enemies:
					return GetDamage(EnergyDrainModifier);
				default:
					return 0;
			}
		}

		private void DeliverDamage(IExchangePlayer provoker, List<IExchangePlayer> targets, float energyModifier, float healthModifier)
		{
			int energy = GetDamage(energyModifier);
			int health = GetDamage(healthModifier);

			foreach (IExchangePlayer player in targets)
			{
				if (energy != 0)
				{
					player.Energy.Add(energy);
				}

				if (health != 0)
				{
					int damage = player.Health.Add(health);
					if (player.Health.Current == 0)
					{
						provoker.PlayerStats.KnockoutsDealt++;
					}
					provoker.PlayerStats.DamageDealt -= damage;
				}
			}
		}

		private int GetDamage(float modifier)
		{
			return (int)(modifier * BaseDamage);
		}
	}
}
