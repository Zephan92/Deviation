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
}
