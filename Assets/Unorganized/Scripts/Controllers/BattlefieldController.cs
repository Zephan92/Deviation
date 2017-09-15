using Assets.Scripts.Enum;
using Assets.Scripts.Exchange;
using Assets.Scripts.Exchange.Attacks;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using Assets.Scripts.Library;
using Assets.Scripts.Utilities;
using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Controllers
{
	//this is a controller for the battlefield
	public class BattlefieldController : MonoBehaviour, IBattlefieldController
	{
		public IPlayer[] Players { get; set; }

		private bool[,,] _battlefields;
		private IExchangeController ec;
		public ICoroutineManager CoroutineManager { get; set; }
		private IEnumerator _coroutine;

		public void Awake()
		{
			ec = FindObjectOfType<ExchangeController>();

			InitializeBattlefields(ec.NumberOfPlayers, (int) ec.MainPlayerFieldNumber);

			Players = FindObjectsOfType<Player>();

			foreach (IPlayer player in Players)
			{
				player.Enemies = FindEnemies(player);
			}

			CoroutineManager = FindObjectOfType<CoroutineManager>();
		}

		//sets the specified battlefield state of a particular cell
		public void SetBattlefieldState(BattlefieldZone field, int row, int column, bool state)
		{
			_battlefields[(int) field, row, column] = state;
		}

		//sets the specifed battlefield state of a particular cell after a period of time
		public void SetBattlefieldStateAfterTimout(float timeout, BattlefieldZone field, int row, int column, bool state)
		{
			object[] parameters = { field, row, column, state };
			CoroutineManager.StartCoroutineThread_AfterTimout(SetBattlefieldStateMethod, parameters, timeout, ref _coroutine);
		}
		
		//returns the battlefield state from the specified battlefield cell
		public bool GetBattlefieldState(BattlefieldZone field, int row, int column)
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
			object[] parameters = { battlefieldObjects };
			CoroutineManager.StartCoroutineThread_AfterTimout(DeleteAfterTimeoutMethod, parameters, timeout, ref _coroutine);
		}

		public void Spawn(float deletionTimeout, string resourceName, Vector3 zone, Quaternion rotation = new Quaternion())
		{
			GameObject go = (GameObject) Instantiate(Resources.Load(resourceName), zone, rotation);
			DeleteAfterTimeout(deletionTimeout, go);
		}

		//spawn object after timeout
		public void SpawnAfterTimeout(float timeout, float deletionTimeout, string resourceName, Vector3 zone, Quaternion rotation = new Quaternion())
		{
			object[] parameters = { deletionTimeout, resourceName, zone, rotation };
			CoroutineManager.StartCoroutineThread_AfterTimout(SpawnAfterTimeoutMethod, parameters, timeout, ref _coroutine);
		}

		//spawn a specified object
		public void SpawnProjectile(float deletionTimeout, string resourceName, Vector3 zone, Quaternion rotation, IAttack attack, Action<Collider, GameObject, IAttack> onTriggerAction = null, Action<GameObject> onStartAction = null, Action<GameObject> updateAction = null)
		{
			GameObject go = (GameObject) Instantiate(Resources.Load(resourceName), zone, rotation);

			IProjectile projectile = go.AddComponent<Projectile>();

			projectile.SetAttack(attack);

			if (onTriggerAction != null)
			{
				projectile.SetOnTriggerEnter(onTriggerAction);
			}
			else
			{
				projectile.SetOnTriggerEnter(delegate(Collider other, GameObject projectileGO,IAttack atk) { });
			}

			if (onStartAction != null)
			{
				projectile.SetStart(onStartAction);
			}
			else
			{
				projectile.SetStart(delegate (GameObject projectileGO) { });
			}

			if (updateAction != null)
			{
				projectile.SetUpdate(updateAction);
			}
			else
			{
				projectile.SetUpdate(delegate (GameObject projectileGO) { });
			}

			DeleteAfterTimeout(deletionTimeout, go);
		}

		public void SpawnProjectileAfterTimeout(float timeout, float deletionTimeout, string resourceName, Vector3 zone, Quaternion rotation, IAttack attack, Action<Collider, GameObject, IAttack> onTriggerAction = null, Action<GameObject> onStartAction = null, Action<GameObject> updateAction = null)
		{
			object[] parameters = { deletionTimeout, resourceName, zone, rotation, attack, onTriggerAction, onStartAction, updateAction};
			CoroutineManager.StartCoroutineThread_AfterTimout(SpawnProjectileAfterTimeoutMethod, parameters, timeout, ref _coroutine);
		}

		public IPlayer GetPlayer(int playerNumber)
		{
			return Players[playerNumber];
		}

		//coroutine for battlefield state
		private void SetBattlefieldStateMethod(object[] parameters)
		{
			SetBattlefieldState(
				field: (BattlefieldZone)parameters[0],
				row: (int)parameters[1],
				column: (int)parameters[2],
				state: (bool)parameters[3]);
		}

		//coroutine to delete specified objects
		private void DeleteAfterTimeoutMethod(object[] parameters)
		{
			foreach (GameObject battlefieldObject in (GameObject []) parameters[0])
			{
				Destroy(battlefieldObject);
			}
		}

		//coroutine to spawn an object after a timeout
		private void SpawnAfterTimeoutMethod(object[] parameters)
		{
			Spawn(
				deletionTimeout: (float) parameters[0], 
				resourceName: (string) parameters[1], 
				zone: (Vector3) parameters[2], 
				rotation: (Quaternion) parameters[3]);
		}

		private void SpawnProjectileAfterTimeoutMethod(object[] parameters)
		{
			SpawnProjectile(
				deletionTimeout: (float) parameters[0], 
				resourceName: (string) parameters[1], 
				zone: (Vector3) parameters[2], 
				rotation: (Quaternion) parameters[3],
				attack: (IAttack) parameters[4],
				onTriggerAction: (Action<Collider, GameObject, IAttack>) parameters[5], 
				onStartAction: (Action<GameObject>) parameters[6],
				updateAction: (Action<GameObject>)parameters[7]);
		}


		//this method initializes the battlefields
		private void InitializeBattlefields(int numBattlefields, int mainPlayerFieldNumber)
		{
			_battlefields = new bool[numBattlefields, 5, 5];
			_battlefields.Initialize();

			for (int i = 0; i < numBattlefields; i++)
			{
				if (mainPlayerFieldNumber == i)
				{
					CreateBattlefield((BattlefieldZone)i, true);
				}
				else
				{
					CreateBattlefield((BattlefieldZone)i, false);
				}
			}
		}

		private GameObject CreateBattlefield(BattlefieldZone startField, bool mainPlayer)
		{
			GameObject battlefield = Instantiate(Resources.Load("Battlefield"), Vector3.zero, new Quaternion(0, 0, 0, 0)) as GameObject;
			Transform[] battlefields = battlefield.GetComponentsInChildren<Transform>();
			foreach (Transform child in battlefields)
			{
				UpdateBattlefield(child.gameObject, startField, mainPlayer);
			}

			float x = 0, y = 0, z = 0;

			if ((int)startField <= 1)
			{
				x = -5f;
			}
			else if ((int)startField > 1 && (int)startField < 4)
			{
				x = 0f;
			}
			else if ((int)startField >= 4)
			{
				x = 5f;
			}

			y = 0;
			z = ((int)startField) % 2 == 0 ? -2.5f : 2.5f;

			battlefield.transform.position = new Vector3(x, y, z);
			return battlefield;
		}

		private void UpdateBattlefield(GameObject go, BattlefieldZone startField, bool mainPlayer)
		{
			if (go.name.Equals("Player"))
			{
				go.AddComponent<ParticleSystem>().Stop();
				if (mainPlayer)
				{
					go.tag = "MainPlayer";
				}
				Player player = go.AddComponent<Player>();
				player.SetPlayer(mainPlayer, startField, KitLibrary.GetKitInstance("InitialKit"), 0.01f, 100, 100, 0, 0);
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
			foreach (IPlayer player in Players)
			{
				if (!player.Equals(target))
				{
					enemies[counter] = player;
					counter++;
				}
			}
			return enemies;
		}

		public List<IExchangePlayer> GetPlayers(BattlefieldZone zone = BattlefieldZone.All)
		{
			throw new NotImplementedException();
		}

		public Vector3 GetBattlefieldCoordinates(BattlefieldZone zone)
		{
			throw new NotImplementedException();
		}
	}
}
