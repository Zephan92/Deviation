﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Assets.Scripts.Enum;
using Assets.Scripts.Utilities;

public class GridSpace : NetworkBehaviour
{
	[SyncVar]
	public BattlefieldZone Zone;

	[SyncVar]
	public bool Occupied = false;

	[SyncVar]
	public bool Damaged = false;

	[SyncVar]
	public bool Broken = false;

	private CoroutineManager cm;
	private IEnumerator _coroutine;

	public void Start()
	{
		cm = FindObjectOfType<CoroutineManager>();
	}

	public void Update()
	{
		if (Damaged && GetCurrentTexture() != Color.green && isServer)
		{
			AddBrokenTexture();
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player" && Damaged && isServer)
		{
			BreakTile(true);
		}
	}

	public void ReInit()
	{
		FixTile();
	}

	public void ResetTexture(BattlefieldZone zone = BattlefieldZone.None)
	{
		zone = zone == BattlefieldZone.None ? Zone : zone;
		Color color = Color.white;
		if (zone == BattlefieldZone.Left)
		{
			color = Color.blue;
		}
		else if (zone == BattlefieldZone.Right)
		{
			color = Color.red;
		}

		UpdateTexture(color);
	}

	public void UpdateTexture(Color color)
	{
		gameObject.GetComponentInChildren<MeshRenderer>().material.color = color;

		if (isServer)
		{
			RpcUpdateTexture(color);
		}
	}

	public void DamageTile(bool breakable = false)
	{
		if (Damaged && breakable)
		{
			BreakTile();
			return;
		}
		
		Damaged = true;	
	}

	public void BreakTile(bool force = false)
	{
		if (Occupied && !force)
		{
			Damaged = true;
			return;
		}

		Broken = true;
		Damaged = false;
		Occupied = true;
		gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
		cm.StartCoroutineThread_AfterTimout(FixTile, 5, ref _coroutine);
		RpcBreakTile();
	}

	private void AddBrokenTexture()
	{
		UpdateTexture(Color.green);
	}

	private void FixTile()
	{
		Damaged = false;
		Occupied = false;
		Broken = false;
		gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
		RpcFixTile();
		ResetTexture();
	}

	private Color GetCurrentTexture()
	{
		return gameObject.GetComponentInChildren<MeshRenderer>().material.color;
	}

	[ClientRpc]
	private void RpcBreakTile()
	{
		gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
	}

	[ClientRpc]
	private void RpcFixTile()
	{
		gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
	}

	[ClientRpc]
	private void RpcUpdateTexture(Color color)
	{
		gameObject.GetComponentInChildren<MeshRenderer>().material.color = color;
	}
}
