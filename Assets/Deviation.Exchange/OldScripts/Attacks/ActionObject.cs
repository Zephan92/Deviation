using System;
using UnityEngine;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using UnityEngine.Networking;

namespace Assets.Scripts.Exchange.Attacks
{
	public class ActionObject : NetworkBehaviour, IActionObject
	{
		private Action<Collider, GameObject, IAttack> _onTriggerEnterAction;
		private Action<GameObject> _startAction;
		private Action<GameObject> _updateAction;
		private Action<GameObject> _fixedUpdateAction;
		private Action<GameObject> _onTileEnter;
		private IAttack _attack;

		public void Start()
		{
			gameObject.layer = 8;
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

		public void SetOnTileEnter(Action<GameObject> action)
		{
			_onTileEnter = action;
		}

		public void OnTileEnter()
		{
			if (_onTileEnter != null)
			{
				_onTileEnter(gameObject);
			}
		}

		public void DisableRenderer()
		{
			if (!isServer)
			{
				return;
			}

			GetComponentInChildren<MeshRenderer>().enabled = false;
			RpcDisableRenderer();
		}

		[ClientRpc]
		private void RpcDisableRenderer()
		{
			GetComponentInChildren<MeshRenderer>().enabled = false;
		}
	}
}
