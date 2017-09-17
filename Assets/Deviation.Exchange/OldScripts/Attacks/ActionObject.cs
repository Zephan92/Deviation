using System;
using UnityEngine;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;

namespace Assets.Scripts.Exchange.Attacks
{
	public class ActionObject : MonoBehaviour, IActionObject
	{
		private Action<Collider, GameObject, IAttack> _onTriggerEnterAction;
		private Action<GameObject> _startAction;
		private Action<GameObject> _updateAction;
		private Action<GameObject> _fixedUpdateAction;

		private IAttack _attack;

		public void Start()
		{
			if (_startAction != null)
			{
				_startAction(gameObject);
			}
		}

		public void Update()
		{
			if (_startAction != null)
			{
				_updateAction(gameObject);
			}
		}

		public void FixedUpdate()
		{
			if (_startAction != null)
			{
				_fixedUpdateAction(gameObject);
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			if (_startAction != null)
			{
				_onTriggerEnterAction(other, gameObject, _attack);
			}
		}

		public void SetStart(Action<GameObject> action)
		{
			_startAction = action;
		}

		public void SetOnTriggerEnter(Action<Collider, GameObject, IAttack> action)
		{
			_onTriggerEnterAction = action;
		}

		public void SetAttack(IAttack attack)
		{
			_attack = attack;
		}

		public void SetUpdate(Action<GameObject> action)
		{
			_updateAction = action;
		}

		public void SetFixedUpdate(Action<GameObject> action)
		{
			_fixedUpdateAction = action;
		}
	}
}
