using Assets.Scripts.Enum;
using UnityEngine;
using System;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Interface.DTO;

namespace Assets.Scripts.Interface
{
	public interface IBattlefieldController
	{
		void SetBattlefieldState(Battlefield field, int row, int column, bool state);
		void SetBattlefieldStateAfterTimout(float timeout, Battlefield field, int row, int column, bool state);
		bool GetBattlefieldState(Battlefield field, int row, int column);
		IPlayer[] GetPlayers();
		IPlayer GetPlayer(int playerNumber);
		void DeleteAfterTimeout(float timeout, GameObject gameObjects);
		void DeleteAfterTimeout(float timeout, GameObject[] battlefieldObjects);
		void SpawnAfterTimeout(float timeout, float deletionTimeout, string resourceName, IAttack attack, Type attackType, Vector3 zone, Quaternion rotation);
		void Spawn(float deletionTimeout, string resourceName, IAttack attack, Type attackType, Vector3 zone, Quaternion rotation);
	}
}
