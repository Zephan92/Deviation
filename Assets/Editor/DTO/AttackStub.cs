using Assets.Scripts.Interface.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Interface.Exchange;

namespace Assets.Editor.DTO
{
	public class AttackStub : IAttack
	{
		public AttackStub(int baseDamage = 0, float healthDrainModifier = -1.0f, float energyDrainModifier = 0.0f, float healthRecoilModifier = 0.0f, float energyRecoilModifier = -1.0f)
		{
			Attacker = null;
			Defender = null;
			BaseDamage = baseDamage;
			HealthDrainModifier = healthDrainModifier;
			EnergyDrainModifier = energyDrainModifier;
			HealthRecoilModifier = healthRecoilModifier;
			EnergyRecoilModifier = energyRecoilModifier;
		}

		public IPlayer Attacker
		{ get; set; }

		public int BaseDamage
		{ get; set; }

		public IPlayer Defender
		{ get; set; }

		public float EnergyDrainModifier
		{ get; set; }

		public float EnergyRecoilModifier
		{ get; set; }

		public float HealthDrainModifier
		{get;set;}

		public float HealthRecoilModifier
		{ get; set; }

		public void InitiateAttack(IPlayer attacker = null, IPlayer defender = null)
		{
			throw new NotImplementedException();
		}

		public void InitiateDrain()
		{
			throw new NotImplementedException();
		}

		public void InitiateRecoil()
		{
			throw new NotImplementedException();
		}

		public void SetAttacker(IPlayer attacker)
		{
			throw new NotImplementedException();
		}

		public void SetDefender(IPlayer defender)
		{
			throw new NotImplementedException();
		}
	}
}
