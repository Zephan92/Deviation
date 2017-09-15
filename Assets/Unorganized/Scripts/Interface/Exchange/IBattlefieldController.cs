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

		void SetBattlefieldState(BattlefieldZone field, int row, int column, bool state);
		void SetBattlefieldStateAfterTimout(float timeout, BattlefieldZone field, int row, int column, bool state);
		bool GetBattlefieldState(BattlefieldZone field, int row, int column);

		void DeleteAfterTimeout(float timeout, GameObject gameObjects);
		void DeleteAfterTimeout(float timeout, GameObject[] battlefieldObjects);
		void Spawn(float deletionTimeout, string resourceName, Vector3 zone, Quaternion rotation = new Quaternion());
		void SpawnAfterTimeout(float timeout, float deletionTimeout, string resourceName, Vector3 zone, Quaternion rotation = new Quaternion());
		void SpawnProjectile(float deletionTimeout, string resourceName, Vector3 zone, Quaternion rotation, IAttack attack, Action<Collider, GameObject, IAttack> onTriggerAction = null, Action<GameObject> onStartAction = null, Action<GameObject> updateAction = null);
		void SpawnProjectileAfterTimeout(float timeout, float deletionTimeout, string resourceName, Vector3 zone, Quaternion rotation, IAttack attack, Action<Collider, GameObject, IAttack> onTriggerAction = null, Action<GameObject> onStartAction = null, Action<GameObject> updateAction = null);
	}
}
