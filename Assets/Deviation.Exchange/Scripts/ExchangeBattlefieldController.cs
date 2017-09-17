using UnityEngine;
using System.Collections;
using Assets.Scripts.Enum;
using System.Collections.Generic;
using UnityEngine.Networking;
using Assets.Scripts.Utilities;
using Assets.Scripts.Interface;
using Assets.Scripts.Interface.DTO;
using Assets.Scripts.Interface.Exchange;
using System;
using Assets.Scripts.Exchange.Attacks;

public class ExchangeBattlefieldController : NetworkBehaviour, IBattlefieldController
{
	private const int BATTLEFIELD_ROW_COUNT = 5;
	private const int BATTLEFIELD_LOCAL_COLUMN_COUNT = 5;
	private const int BATTLEFIELD_COLUMN_COUNT = 10;

	public GridSpace[,] Grid;
	public GameObject BattlefieldGO;
	private Dictionary<BattlefieldZone, List<IExchangePlayer>> _playerDict;

	//make multiple of these
	private IEnumerator _coroutine;

	private ICoroutineManager cm;

	[Obsolete]
	public IPlayer[] Players
	{
		get
		{
			throw new NotImplementedException();
		}

		set
		{
			throw new NotImplementedException();
		}
	}

	public void Start()
	{
		if (BattlefieldGO == null)
		{
			BattlefieldGO = gameObject;
		}
		Grid = new GridSpace[BATTLEFIELD_COLUMN_COUNT, BATTLEFIELD_ROW_COUNT];
		_playerDict = new Dictionary<BattlefieldZone, List<IExchangePlayer>>();

		cm = FindObjectOfType<CoroutineManager>();
	}

	public void Init()
	{
		InstantiateBattlefield();
	}

	public List<IExchangePlayer> GetPlayers(BattlefieldZone zone = BattlefieldZone.All)
	{
		if (_playerDict.Keys.Count == 0)
		{
			AddPlayersToDict();
		}

		if (zone == BattlefieldZone.All)
		{
			List<IExchangePlayer> allPlayers = new List<IExchangePlayer>();
			allPlayers.AddRange(_playerDict[BattlefieldZone.Left]);
			allPlayers.AddRange(_playerDict[BattlefieldZone.Right]);
			return allPlayers;
		}
		else
		{
			return _playerDict[zone];
		}
	}

	public bool GetBattlefieldState(int row, int column, BattlefieldZone zone = BattlefieldZone.All)
	{
		switch (zone)
		{
			case BattlefieldZone.All:
				if (BATTLEFIELD_ROW_COUNT <= row || BATTLEFIELD_COLUMN_COUNT <= column)
				{
					return false;
				}
				break;
			case BattlefieldZone.Left:
			case BattlefieldZone.Right:
				if (BATTLEFIELD_ROW_COUNT <= row || BATTLEFIELD_LOCAL_COLUMN_COUNT <= column)
				{
					return false;
				}
				break;
			default:
				return false;
		}
		var coordinates = GetGridCoordinates(row, column, zone);
		if (coordinates.x > 0 &&
			coordinates.x < Grid.GetLength(0) &&
			coordinates.y > 0 &&
			coordinates.y < Grid.GetLength(1))
		{
			return Grid[(int)coordinates.x, (int)coordinates.y].Occupied;
		}
		else
		{
			return false;
		}
	}

	public void SetBattlefieldState(int row, int column, bool state, BattlefieldZone zone = BattlefieldZone.All)
	{
		var coordinates = GetGridCoordinates(row, column, zone);
		if (coordinates.x >= 0 &&
			coordinates.x < Grid.GetLength(0) &&
			coordinates.y >= 0 &&
			coordinates.y < Grid.GetLength(1))
		{
			Grid[(int)coordinates.x, (int)coordinates.y].Occupied = state;
		}
		else
		{
			Debug.LogError("Coordinates were not valid. (" + coordinates.x + ", " + coordinates.y + ")");
		}
	}

	public Rect GetBattlefieldBoundaries(BattlefieldZone zone = BattlefieldZone.All)
	{
		switch (zone)
		{
			case BattlefieldZone.All:
				return new Rect(0, 0, 10, 5);
			case BattlefieldZone.Left:
				return new Rect(0, 0, 5, 5);
			case BattlefieldZone.Right:
				return new Rect(5, 0, 5, 5);
			default:
				throw new System.Exception("Zone was not Left, Right or All.");
		}
	}

	public Vector3 GetBattlefieldCoordinates(BattlefieldZone zone)
	{
		switch (zone)
		{
			case BattlefieldZone.Left:
				return new Vector3(2f, 0, 2f);
			case BattlefieldZone.Right:
				return new Vector3(7f, 0, 2f);
			default:
				throw new System.Exception("Zone was not Left, Right or All.");
		}
	}


	public void SetBattlefieldStateAfterTimout(float timeout, BattlefieldZone field, int row, int column, bool state)
	{
		throw new NotImplementedException();
	}

	public GameObject Spawn(float deletionTimeout, string resourceName, Vector3 zone, Quaternion rotation = new Quaternion())
	{
		var gameObject = (GameObject)Instantiate(Resources.Load(resourceName), zone, rotation);
		NetworkServer.Spawn(gameObject);
		DeleteAfterTimeout(deletionTimeout, gameObject);
		return gameObject;
	}

	//spawn object after timeout
	public void SpawnAfterTimeout(float timeout, float deletionTimeout, string resourceName, Vector3 zone, Quaternion rotation = new Quaternion())
	{
		object[] parameters = { deletionTimeout, resourceName, zone, rotation };
		cm.StartCoroutineThread_AfterTimout(SpawnAfterTimeoutMethod, parameters, timeout, ref _coroutine);
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
		cm.StartCoroutineThread_AfterTimout(DeleteAfterTimeoutMethod, parameters, timeout, ref _coroutine);
	}

	public void SpawnActionObject(float deletionTimeout, string resourceName, Vector3 zone, IAttack attack, Quaternion rotation = new Quaternion(),
		Action<Collider, GameObject, IAttack> onTriggerAction = null, 
		Action<GameObject> onStartAction = null, 
		Action<GameObject> updateAction = null, 
		Action<GameObject> fixedUpdateAction = null)
	{
		GameObject go = Spawn(deletionTimeout, resourceName, zone, rotation);

		IActionObject actionObject = go.GetComponent<ActionObject>();

		actionObject.SetAttack(attack);

		if (onTriggerAction != null)
		{
			actionObject.SetOnTriggerEnter(onTriggerAction);
		}
		else
		{
			actionObject.SetOnTriggerEnter(delegate (Collider other, GameObject actionGO, IAttack atk) { });
		}

		if (onStartAction != null)
		{
			actionObject.SetStart(onStartAction);
		}
		else
		{
			actionObject.SetStart(delegate (GameObject actionGO) { });
		}

		if (updateAction != null)
		{
			actionObject.SetUpdate(updateAction);
		}
		else
		{
			actionObject.SetUpdate(delegate (GameObject actionGO) { });
		}

		if (fixedUpdateAction != null)
		{
			actionObject.SetFixedUpdate(fixedUpdateAction);
		}
		else
		{
			actionObject.SetFixedUpdate(delegate (GameObject actionGO) { });
		}
	}

	public void SpawnActionObjectAfterTimeout(float timeout, float deletionTimeout, string resourceName, Vector3 zone, IAttack attack, Quaternion rotation = new Quaternion(),
		Action<Collider, GameObject, IAttack> onTriggerAction = null, 
		Action<GameObject> onStartAction = null, 
		Action<GameObject> updateAction = null, 
		Action<GameObject> fixedUpdateAction = null)
	{
		object[] parameters = { deletionTimeout, resourceName, zone, rotation, attack, onTriggerAction, onStartAction, updateAction, fixedUpdateAction };
		cm.StartCoroutineThread_AfterTimout(SpawnProjectileAfterTimeoutMethod, parameters, timeout, ref _coroutine);
	}

	public Vector2 GetGridCoordinates(int row, int column, BattlefieldZone zone = BattlefieldZone.All)
	{
		int gridColumn = 0;
		switch (zone)
		{
			case BattlefieldZone.All:
			case BattlefieldZone.Left:
				gridColumn = column;
				break;
			case BattlefieldZone.Right:
				gridColumn = column + 5;
				break;
		}

		return new Vector2(gridColumn, row);
	}

	public Vector2 GetGridCoordinates(Vector3 pos, BattlefieldZone zone = BattlefieldZone.All)
	{
		switch (zone)
		{
			case BattlefieldZone.All:
			case BattlefieldZone.Left:
				return GetGridCoordinates((int)pos.z, (int)pos.x);
			case BattlefieldZone.Right:
				return GetGridCoordinates((int)pos.z, (int)pos.x - 5);
				break;
			default:
				return GetGridCoordinates((int)pos.z, (int)pos.x);
		}

	}

	private void AddPlayersToDict()
	{
		var players = FindObjectsOfType<ExchangePlayer>();
		foreach (IExchangePlayer player in players)
		{
			if (_playerDict.ContainsKey(player.Zone))
			{
				_playerDict[player.Zone].Add(player);
			}
			else
			{
				_playerDict.Add(player.Zone, new List<IExchangePlayer> { player });
			}
		}
	}

	[ClientRpc]
	private void RpcAddPlayersToDict()
	{
		AddPlayersToDict();
	}

	private void InstantiateBattlefield()
	{
		if (!isServer)
		{
			return;
		}

		for (int columnNum = 0; columnNum < BATTLEFIELD_COLUMN_COUNT; columnNum++)
		{
			for (int rowNum = 0; rowNum < BATTLEFIELD_ROW_COUNT; rowNum++)
			{
				var gridSpaceGameObject = Resources.Load("GridSpace") as GameObject;
				var gridSpaceObject = Instantiate(gridSpaceGameObject, BattlefieldGO.transform);

				gridSpaceObject.transform.localPosition = new Vector3(columnNum, 0, rowNum);
				NetworkServer.Spawn(gridSpaceObject);
				var gridspace = gridSpaceObject.GetComponent<GridSpace>();

				if (columnNum < BATTLEFIELD_LOCAL_COLUMN_COUNT)
				{
					GridSpaceInit(gridSpaceObject, Color.blue, columnNum, rowNum);
					RpcGridSpaceInit(gridSpaceObject, Color.blue, columnNum, rowNum);
					gridspace.Zone = BattlefieldZone.Left;
				}
				else
				{
					GridSpaceInit(gridSpaceObject, Color.red, columnNum, rowNum);
					RpcGridSpaceInit(gridSpaceObject, Color.red, columnNum, rowNum);
					gridspace.Zone = BattlefieldZone.Right;
				}
			}
		}
		SetBattlefieldState(2, 2, true, BattlefieldZone.Left);
		SetBattlefieldState(2, 2, true, BattlefieldZone.Right);
	}

	[ClientRpc]
	private void RpcGridSpaceInit(GameObject go, Color color, int column, int row)
	{
		GridSpaceInit(go, color, column, row);		
	}

	private void GridSpaceInit(GameObject go, Color color, int column, int row)
	{
		go.transform.parent = BattlefieldGO.transform;
		go.GetComponentInChildren<MeshRenderer>().material.color = color;
		var gridspace = go.GetComponent<GridSpace>();
		Grid[column, row] = gridspace;
	}

	//coroutine to delete specified objects
	private void DeleteAfterTimeoutMethod(object[] parameters)
	{
		foreach (GameObject battlefieldObject in (GameObject[])parameters[0])
		{
			Destroy(battlefieldObject);
		}
	}

	//coroutine to spawn an object after a timeout
	private void SpawnAfterTimeoutMethod(object[] parameters)
	{
		Spawn(
			deletionTimeout: (float)parameters[0],
			resourceName: (string)parameters[1],
			zone: (Vector3)parameters[2],
			rotation: (Quaternion)parameters[3]);
	}

	private void SpawnProjectileAfterTimeoutMethod(object[] parameters)
	{
		SpawnActionObject(
			deletionTimeout: (float)parameters[0],
			resourceName: (string)parameters[1],
			zone: (Vector3)parameters[2],
			rotation: (Quaternion)parameters[3],
			attack: (IAttack)parameters[4],
			onTriggerAction: (Action<Collider, GameObject, IAttack>)parameters[5],
			onStartAction: (Action<GameObject>)parameters[6],
			updateAction: (Action<GameObject>)parameters[7],
			fixedUpdateAction: (Action<GameObject>)parameters[8]);
	}
}
