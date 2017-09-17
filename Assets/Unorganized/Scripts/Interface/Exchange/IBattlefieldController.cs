using Assets.Scripts.Enum;
using UnityEngine;
using System;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Interface.DTO;
using System.Collections.Generic;

namespace Assets.Scripts.Interface
{
	public interface IBattlefieldController
	{
		IPlayer[] Players { get; set; }
		List<IExchangePlayer> GetPlayers(BattlefieldZone zone = BattlefieldZone.All);

		Vector3 GetBattlefieldCoordinates(BattlefieldZone zone);

		void SetBattlefieldState(int row, int column, bool state, BattlefieldZone zone = BattlefieldZone.All);
		bool GetBattlefieldState(int row, int column, BattlefieldZone zone = BattlefieldZone.All);

		void DeleteAfterTimeout(float timeout, GameObject gameObjects);
		void DeleteAfterTimeout(float timeout, GameObject[] battlefieldObjects);
		GameObject Spawn(float deletionTimeout, string resourceName, Vector3 zone, Quaternion rotation = new Quaternion());
		void SpawnAfterTimeout(float timeout, float deletionTimeout, string resourceName, Vector3 zone, Quaternion rotation = new Quaternion());
		void SpawnActionObject(float deletionTimeout, string resourceName, Vector3 zone, IAttack attack, Quaternion rotation = new Quaternion(),
			Action<Collider, GameObject, IAttack> onTriggerAction = null,
			Action<GameObject> onStartAction = null, 
			Action<GameObject> updateAction = null,
			Action<GameObject> fixedUpdateAction = null);

		void SpawnActionObjectAfterTimeout(float timeout, float deletionTimeout, string resourceName, Vector3 zone, IAttack attack, Quaternion rotation = new Quaternion(),
			Action<Collider, GameObject, IAttack> onTriggerAction = null,
			Action<GameObject> onStartAction = null,
			Action<GameObject> updateAction = null,
			Action<GameObject> fixedUpdateAction = null);
	}
}
