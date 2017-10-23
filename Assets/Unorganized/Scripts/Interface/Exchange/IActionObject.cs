using Assets.Scripts.Interface.DTO;
using System;
using UnityEngine;

namespace Assets.Scripts.Interface.Exchange
{
	public interface IActionObject
	{
		void SetOnTriggerEnter(Action<Collider, GameObject, IAttack> action);
		void SetStart(Action<GameObject> action);
		void SetUpdate(Action<GameObject> action);
		void SetFixedUpdate(Action<GameObject> action);
		void SetAttack(IAttack attack);
		void DisableRenderer();
		void SetOnTileEnter(Action<GameObject> action);
	}
}
