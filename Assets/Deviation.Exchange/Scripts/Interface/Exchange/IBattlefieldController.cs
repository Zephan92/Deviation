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
		IGridManager gm { get; set; }
		List<IExchangePlayer> GetPlayers(BattlefieldZone zone = BattlefieldZone.All);
		void ResetBattlefield();
		Vector3 GetBattlefieldCoordinates(BattlefieldZone zone);
		//Rect GetBattlefieldBoundaries(BattlefieldZone zone = BattlefieldZone.All);
		//bool IsInsideBattlefieldBoundaries(Vector3 pos, BattlefieldZone zone = BattlefieldZone.All);
		//bool IsInsideBattlefieldBoundaries(int row, int column, BattlefieldZone zone = BattlefieldZone.All);
	
		void DeleteAfterTimeout(float timeout, GameObject gameObjects);
		void DeleteAfterTimeout(float timeout, GameObject[] battlefieldObjects);

		void ActionWarning(float delay, Action warningActionStart, Action warningActionEnd);

		void SpawnActionObject(float delay, float deletionTimeout, string resourceName, Vector3 zone, IAttack attack, Quaternion rotation = new Quaternion(),
			Action<Collider, GameObject, IAttack> onTriggerAction = null,
			Action<GameObject> onStartAction = null,
			Action<GameObject> updateAction = null,
			Action<GameObject> fixedUpdateAction = null,
			Action onDelayStartAction = null,
			Action onDelayEndAction = null,
			Action<GameObject> onTileEnter = null);

		void SpawnActionObjectAfterTimeout(float delay, float timeout, float deletionTimeout, string resourceName, Vector3 zone, IAttack attack, Quaternion rotation = new Quaternion(),
			Action<Collider, GameObject, IAttack> onTriggerAction = null,
			Action<GameObject> onStartAction = null,
			Action<GameObject> updateAction = null,
			Action<GameObject> fixedUpdateAction = null,
			Action onDelayStartAction = null,
			Action onDelayEndAction = null,
			Action<GameObject> onTileEnter = null);
	}
}
