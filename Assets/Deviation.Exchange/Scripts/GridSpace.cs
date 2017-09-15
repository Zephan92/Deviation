using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Assets.Scripts.Enum;

public class GridSpace : NetworkBehaviour
{
	[SyncVar]
	public BattlefieldZone Zone;
	[SyncVar]
	public bool Occupied = false;
	public GameObject GridSpaceObject;

	public GridSpace(GameObject gridSpaceObject, BattlefieldZone zone)
	{
		Occupied = false;
		GridSpaceObject = gridSpaceObject;
		Zone = zone;
	}
}
