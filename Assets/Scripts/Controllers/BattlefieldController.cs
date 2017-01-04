using Assets.Scripts.Enum;
using Assets.Scripts.Exchange;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Library;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
	//this is a controller for the battlefield
	public class BattlefieldController : MonoBehaviour, IBattlefieldController
	{
		public IPlayer[] Players { get; set; }

		private bool[,,] _battlefields;
		private IExchangeController ec;

		public void Awake()
		{
			ec = FindObjectOfType<ExchangeController>();

			InitializeBattlefields(ec.NumberOfPlayers, (int) ec.MainPlayerFieldNumber);

			Players = FindObjectsOfType<Player>();

			foreach (IPlayer player in Players)
			{
				player.Enemies = FindEnemies(player);
			}
		}

		//this method initializes the battlefields
		private void InitializeBattlefields(int numBattlefields, int mainPlayerFieldNumber)
		{
			_battlefields = new bool[numBattlefields, 5, 5];
			_battlefields.Initialize();
			
			for (int i = 0; i < numBattlefields; i++)
			{
				if(mainPlayerFieldNumber == i)
				{
					CreateBattlefield((Battlefield) i, true);
				}
				else
				{
					CreateBattlefield((Battlefield)i, false);
				}		
			}
		}

		private GameObject CreateBattlefield(Battlefield startField, bool mainPlayer)
		{
			GameObject battlefield = Instantiate(Resources.Load("Battlefield"), Vector3.zero, new Quaternion(0, 0, 0, 0)) as GameObject;
			Transform[] battlefields = battlefield.GetComponentsInChildren<Transform>();
			foreach (Transform child in battlefields)
			{
				UpdateBattlefield(child.gameObject, startField, mainPlayer);
			}
			
			float x = 0, y = 0, z = 0;

			if ((int) startField <= 1)
			{
				x = -5f;
			}
			else if ((int) startField > 1 && (int) startField < 4)
			{
				x = 0f;
			}
			else if ((int) startField >= 4)
			{
				x = 5f;
			}

			y = 0;
			z = ((int) startField) % 2 == 0 ? -2.5f : 2.5f;

			battlefield.transform.position = new Vector3(x,y,z);
			return battlefield;
		}

		private void UpdateBattlefield(GameObject go, Battlefield startField, bool mainPlayer)
		{
			if (go.name.Equals("Player"))
			{
				if (mainPlayer)
				{
					go.tag = "MainPlayer";
				}
				Player player = go.AddComponent<Player>();
				player.SetPlayer(mainPlayer, startField, KitLibrary.GetKitInstance("InitialKit"), 0.01f, 100, 100, 0 ,0);
			}
			else if (go.name.Equals("Grid"))
			{
				GridManager grid = go.AddComponent<GridManager>();
				grid.ThisBattlefield = startField;
			}
		}

		private IPlayer[] FindEnemies(IPlayer target)
		{
			IPlayer[] enemies = new IPlayer[Players.Length - 1];
			int counter = 0;
			foreach(IPlayer player in Players)
			{
				if (!player.Equals(target))
				{
					enemies[counter] = player;
					counter++;
				}
			}
			return enemies;
		}

		//sets the specified battlefield state of a particular cell
		public void SetBattlefieldState(Battlefield field, int row, int column, bool state)
		{
			_battlefields[(int) field, row, column] = state;
		}

		//sets the specifed battlefield state of a particular cell after a period of time
		public void SetBattlefieldStateAfterTimout(float timeout, Battlefield field, int row, int column, bool state)
		{
			StartCoroutine(SetBattlefieldStateAfterTimoutCoroutine(timeout, field, row,  column, state));
		}

		//coroutine for battlefield state
		private IEnumerator SetBattlefieldStateAfterTimoutCoroutine(float timeout, Battlefield field, int row, int column, bool state)
		{
			yield return new WaitForSeconds(timeout);
			SetBattlefieldState(field, row, column, state);
		}
		
		//returns the battlefield state from the specified battlefield cell
		public bool GetBattlefieldState(Battlefield field, int row, int column)
		{
			if (column < 0 || column > 4 || row < 0 || row > 4)
			{
				return false;
			}

			if (_battlefields != null)
			{
				return _battlefields[(int) field, row, column];
			}
			else
			{
				Debug.LogError("Battlefields doesn't exist yet");
			}
			
			return false;
		}

		//delete object after timeout
		public void DeleteAfterTimeout(float timeout, GameObject gameObjects)
		{
			DeleteAfterTimeout(timeout, new GameObject[] { gameObjects });
		}

		//delete objects after timeout
		public void DeleteAfterTimeout(float timeout, GameObject[] battlefieldObjects)
		{
			StartCoroutine(DeleteAfterTimeoutCoroutine(timeout, battlefieldObjects));
		}

		//spawn object after timeout
		public void SpawnAfterTimeout(float timeout, float deletionTimeout, string resourceName, IAttack attack, Type attackType, Vector3 zone, Quaternion rotation)
		{
			StartCoroutine(SpawnAfterTimeoutCoroutine(timeout, deletionTimeout, resourceName, attack, attackType, zone, rotation));
		}

		//spawn a specified object
		public void Spawn(float deletionTimeout, string resourceName, IAttack attack, Type attackType, Vector3 zone, Quaternion rotation)
		{
			GameObject go = (GameObject)Instantiate(Resources.Load(resourceName), zone, rotation);
			
			IExchangeAttack attackScript = go.GetComponent(attackType) as IExchangeAttack;
			if (attackScript == null)
				attackScript = go.GetComponentInChildren(attackType) as IExchangeAttack;
			attackScript.SetAttack(attack);
			DeleteAfterTimeout(deletionTimeout, go);
		}

		//coroutine to delete specified objects
		private IEnumerator DeleteAfterTimeoutCoroutine(float timeout, GameObject[] battlefieldObjects)
		{
			yield return new WaitForSeconds(timeout);
			foreach (GameObject battlefieldObject in battlefieldObjects)
			{
				Destroy(battlefieldObject);
			}
		}

		//coroutine to spawn an object after a timeout
		private IEnumerator SpawnAfterTimeoutCoroutine(float timeout, float deletionTimeout, string resourceName, IAttack attack, Type attackType, Vector3 zone, Quaternion rotation)
		{
			yield return new WaitForSeconds(timeout);
			Spawn(deletionTimeout, resourceName, attack, attackType, zone, rotation);
		}

		public IPlayer GetPlayer(int playerNumber)
		{
			return Players[playerNumber];
		}
	}
}
