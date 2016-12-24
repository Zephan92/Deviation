using Assets.Scripts.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Enum;
using Assets.Scripts.Exchange;
using UnityEngine;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;

namespace Assets.Editor.Controllers
{
	public class BattlefieldControllerStub : IBattlefieldController
	{
		public bool BattlefieldState;
		public IPlayer[] Players;

		public BattlefieldControllerStub(bool state, IPlayer[] players)
		{
			BattlefieldState = state;
			Players = players;
		}

		public void DeleteAfterTimeout(float timeout, GameObject[] battlefieldObjects)
		{
		}

		public void DeleteAfterTimeout(float timeout, GameObject gameObjects)
		{
		}

		public bool GetBattlefieldState(Battlefield field, int row, int column)
		{
			return BattlefieldState;
		}

		public IPlayer GetPlayer(int playerNumber)
		{
			return Players[playerNumber];
		}

		public IPlayer[] GetPlayers()
		{
			return Players;
		}

		public void SetBattlefieldState(Battlefield field, int row, int column, bool state)
		{
		}

		public void SetBattlefieldStateAfterTimout(float timeout, Battlefield field, int row, int column, bool state)
		{
		}

		public void Spawn(float deletionTimeout, string resourceName, IAttack attack, Type attackType, Vector3 zone, Quaternion rotation)
		{
		}

		public void SpawnAfterTimeout(float timeout, float deletionTimeout, string resourceName, IAttack attack, Type attackType, Vector3 zone, Quaternion rotation)
		{
		}
	}
}
