using Assets.Scripts.Interface;
using System;
using UnityEngine;
using Assets.Scripts.Interface.DTO;

namespace Assets.Scripts.Exchange.Attacks
{
	public interface IProjectile
	{
		void SetOnTriggerEnter(Action<Collider, GameObject, IAttack> action);

	}

	public class Projectile : MonoBehaviour, IProjectile, IExchangeAttack
	{
		private Action<Collider, GameObject, IAttack> OnTriggerEnterAction;

		private IAttack _attack;

		public void SetOnTriggerEnter(Action<Collider, GameObject, IAttack> action)
		{
			OnTriggerEnterAction = action;
		}

		public void OnTriggerEnter(Collider other)
		{
			OnTriggerEnterAction(other, gameObject, _attack);
		}

		public void SetAttack(IAttack attack)
		{
			_attack = attack;
		}
	}
}
