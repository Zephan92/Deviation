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
		List<IExchangePlayer> GetPlayers(BattlefieldZone zone = BattlefieldZone.All);

		Vector3 GetBattlefieldCoordinates(BattlefieldZone zone);
		bool GetGridSpaceBroken(int column, int row, BattlefieldZone zone = BattlefieldZone.All);
		void SetGridSpaceColor(int column, int row, Color color, BattlefieldZone zone = BattlefieldZone.All);
		void ResetGridSpaceColor(int column, int row, BattlefieldZone zone = BattlefieldZone.All);
		void BreakTile(int column, int row, BattlefieldZone zone = BattlefieldZone.All);
		void DamageTile(int column, int row, BattlefieldZone zone = BattlefieldZone.All);

		void SetBattlefieldState(int row, int column, bool state, BattlefieldZone zone = BattlefieldZone.All);
		bool GetBattlefieldState(int row, int column, BattlefieldZone zone = BattlefieldZone.All);

		void DeleteAfterTimeout(float timeout, GameObject gameObjects);
		void DeleteAfterTimeout(float timeout, GameObject[] battlefieldObjects);

		void ActionWarning(float delay, Action warningActionStart, Action warningActionEnd);

		void SpawnActionObject(float delay, float deletionTimeout, string resourceName, Vector3 zone, IAttack attack, Quaternion rotation = new Quaternion(),
			Action<Collider, GameObject, IAttack> onTriggerAction = null,
			Action<GameObject> onStartAction = null,
			Action<GameObject> updateAction = null,
			Action<GameObject> fixedUpdateAction = null,
			Action onDelayStartAction = null,
			Action onDelayEndAction = null);

		void SpawnActionObjectAfterTimeout(float delay, float timeout, float deletionTimeout, string resourceName, Vector3 zone, IAttack attack, Quaternion rotation = new Quaternion(),
			Action<Collider, GameObject, IAttack> onTriggerAction = null,
			Action<GameObject> onStartAction = null,
			Action<GameObject> updateAction = null,
			Action<GameObject> fixedUpdateAction = null,
			Action onDelayStartAction = null,
			Action onDelayEndAction = null);
	}
}
