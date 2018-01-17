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
	public GameObject BattlefieldGO;
	private Dictionary<BattlefieldZone, List<IExchangePlayer>> _playerDict;

	//make multiple of these
	private IEnumerator _coroutine;
	private ICoroutineManager cm;
	public IGridManager gm { get; set; }

	public void Start()
	{
		if (BattlefieldGO == null)
		{
			BattlefieldGO = gameObject;
		}
		_playerDict = new Dictionary<BattlefieldZone, List<IExchangePlayer>>();

		cm = FindObjectOfType<CoroutineManager>();
		gm = FindObjectOfType<GridManager>();
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

	//delete object after timeout
	public void DeleteAfterTimeout(float timeout, GameObject gameObjects)
	{
		DeleteAfterTimeout(timeout, new GameObject[] { gameObjects });
	}

	//delete objects after timeout
	public void DeleteAfterTimeout(float timeout, GameObject[] battlefieldObjects)
	{
		foreach (GameObject go in battlefieldObjects)
		{
			Destroy(go, timeout);
		}
	}

	public void ActionWarning(float delay, Action warningActionStart, Action warningActionEnd)
	{
		if (warningActionStart != null)
		{
			warningActionStart();
		}

		if (warningActionEnd != null)
		{
			cm.StartCoroutineThread_AfterTimout(warningActionEnd, delay, ref _coroutine);
		}
	}

	public void SpawnActionObject(float delay, float deletionTimeout, string resourceName, Vector3 zone, IAttack attack, Quaternion rotation = new Quaternion(),
		Action<Collider, GameObject, IAttack> onTriggerAction = null, 
		Action<GameObject> onStartAction = null, 
		Action<GameObject> updateAction = null, 
		Action<GameObject> fixedUpdateAction = null,
		Action onDelayStartAction = null,
		Action onDelayEndAction = null,
		Action<GameObject> onTileEnterAction = null,
		Action<GameObject> onDestroyAction = null)
	{
		ActionWarning(delay, onDelayStartAction, onDelayEndAction);

		object[] parameters = 
		{
			deletionTimeout,	//0
			resourceName,		//1
			zone,				//2
			rotation,			//3
			attack,				//4
			onTriggerAction,	//5
			onStartAction,		//6
			updateAction,		//7
			fixedUpdateAction,	//8
			onTileEnterAction,	//9
			onDestroyAction,	//10
		};

		cm.StartCoroutineThread_AfterTimout(() => 
		{
			SpawnActionObjectHelper(parameters);
		}, delay, ref _coroutine);
	}

	public void SpawnActionObjectAfterTimeout(float delay, float timeout, float deletionTimeout, string resourceName, Vector3 zone, IAttack attack, Quaternion rotation = new Quaternion(),
		Action<Collider, GameObject, IAttack> onTriggerAction = null,
		Action<GameObject> onStartAction = null,
		Action<GameObject> updateAction = null,
		Action<GameObject> fixedUpdateAction = null,
		Action onDelayStartAction = null,
		Action onDelayEndAction = null,
		Action<GameObject> onTileEnter = null,
		Action<GameObject> onDestroyAction = null)
	{
		cm.StartCoroutineThread_AfterTimout(() =>
		{
			SpawnActionObject(delay, deletionTimeout, resourceName, zone, attack, rotation, onTriggerAction, onStartAction, updateAction, fixedUpdateAction, onDelayStartAction, onDelayEndAction, onTileEnter, onDestroyAction);
		}, timeout, ref _coroutine);
	}

	private void SpawnActionObjectHelper(object[] parameters)
	{
		var go = Spawn(
			deletionTimeout: (float)parameters[0],
			resourceName: (string)parameters[1],
			zone: (Vector3)parameters[2],
			rotation: (Quaternion)parameters[3]);

		IActionObject actionObject = go.GetComponent<ActionObject>();

		actionObject.SetAttack((IAttack)parameters[4]);

		if (parameters[5] != null)
		{
			actionObject.SetOnTriggerEnter((Action<Collider, GameObject, IAttack>) parameters[5]);
		}
		else
		{
			actionObject.SetOnTriggerEnter(delegate (Collider other, GameObject actionGO, IAttack atk) { });
		}

		if (parameters[6] != null)
		{
			actionObject.SetStart((Action<GameObject>) parameters[6]);
		}
		else
		{
			actionObject.SetStart(delegate (GameObject actionGO) { });
		}

		if (parameters[7] != null)
		{
			actionObject.SetUpdate((Action<GameObject>) parameters[7]);
		}
		else
		{
			actionObject.SetUpdate(delegate (GameObject actionGO) { });
		}

		if (parameters[8] != null)
		{
			actionObject.SetFixedUpdate((Action<GameObject>)parameters[8]);
		}
		else
		{
			actionObject.SetFixedUpdate(delegate (GameObject actionGO) { });
		}

		if (parameters[9] != null)
		{
			actionObject.SetOnTileEnter((Action<GameObject>)parameters[9]);
		}
		else
		{
			actionObject.SetOnTileEnter(delegate (GameObject actionGO) { });
		}

		if (parameters[10] != null)
		{
			actionObject.SetOnDestroy((Action<GameObject>)parameters[10]);
		}
		else
		{
			actionObject.SetOnDestroy(delegate (GameObject actionGO) { });
		}
	}

	public void ResetBattlefield()
	{
		gm.ResetGrid();

		foreach (ActionObject actionObject  in FindObjectsOfType<ActionObject>())
		{
			Destroy(actionObject.gameObject);
		}
	}

	private GameObject Spawn(float deletionTimeout, string resourceName, Vector3 zone, Quaternion rotation = new Quaternion())
	{
		var gameObject = (GameObject)Instantiate(Resources.Load(resourceName), zone, rotation);
		NetworkServer.Spawn(gameObject);
		Destroy(gameObject, deletionTimeout);
		return gameObject;
	}

	//coroutine to delete specified objects
	private void DeleteAfterTimeoutMethod(object[] parameters)
	{
		foreach (GameObject battlefieldObject in (GameObject[])parameters[0])
		{
			Destroy(battlefieldObject);
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

		for (int columnNum = 0; columnNum < ExchangeConstants.BATTLEFIELD_COLUMN_COUNT; columnNum++)
		{
			for (int rowNum = 0; rowNum < ExchangeConstants.BATTLEFIELD_ROW_COUNT; rowNum++)
			{
				var gridSpaceGameObject = Resources.Load("GridSpace") as GameObject;
				var gridSpaceObject = Instantiate(gridSpaceGameObject, BattlefieldGO.transform);

				gridSpaceObject.transform.localPosition = new Vector3(columnNum, 0, rowNum);
				NetworkServer.Spawn(gridSpaceObject);
				var gridspace = gridSpaceObject.GetComponent<GridSpace>();
				
				if (columnNum < ExchangeConstants.BATTLEFIELD_LOCAL_COLUMN_COUNT)
				{
					gridspace.Zone = BattlefieldZone.Left;
				}
				else
				{
					gridspace.Zone = BattlefieldZone.Right;
					
				}

				gridspace.ResetTexture();
				gm.InitGridspace(gridSpaceObject, BattlefieldGO, rowNum, columnNum);
			}
		}
	}
}
