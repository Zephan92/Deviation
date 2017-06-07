using System;
using UnityEngine;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;

namespace Assets.Scripts.Exchange.Attacks
{
	public class Projectile : MonoBehaviour, IProjectile
	{
		private Action<Collider, GameObject, IAttack> _onTriggerEnterAction;
		private Action<GameObject> _startAction;
		private Action<GameObject> _updateAction;
		
		private IAttack _attack;

		public void SetStart(Action<GameObject> action)
		{
			_startAction = action;
		}

		public void Start()
		{
			_startAction(gameObject);
		}

		public void Update()
		{
			_updateAction(gameObject);
		}

		public void SetOnTriggerEnter(Action<Collider, GameObject, IAttack> action)
		{
			_onTriggerEnterAction = action;
		}

		public void OnTriggerEnter(Collider other)
		{
			_onTriggerEnterAction(other, gameObject, _attack);
		}

		public void SetAttack(IAttack attack)
		{
			_attack = attack;
		}

		public void SetUpdate(Action<GameObject> action)
		{
			_updateAction = action;
		}
	}
}
