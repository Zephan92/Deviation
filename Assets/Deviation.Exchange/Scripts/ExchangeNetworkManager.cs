﻿using Assets.Deviation.Exchange.Scripts;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This script represents the changes that you will need to do in your custom
/// network manager
/// </summary>
public class ExchangeNetworkManager : NetworkManager
{
	// Set this in the inspector
	public GameRoom GameRoom;
	public int playercount;
	
	void Awake()
	{
		GameRoom = GameRoom ?? FindObjectOfType<GameRoom>();

		if (GameRoom == null)
		{
			Debug.LogError("Game Room property is not set on NetworkManager");
			return;
		}
		
		// Subscribe to events
		GameRoom.PlayerJoined += OnPlayerJoined;
		GameRoom.PlayerLeft += OnPlayerLeft;
		GameRoom.ServerFull += OnServerFull;
	}

	private void OnPlayerJoined(UnetMsfPlayer player)
	{
		Debug.LogError("Player Joined");

		var prefabPlayerGameObject = Resources.Load("1v1Player") as GameObject;
		var playerGameObject = Instantiate(prefabPlayerGameObject);
		NetworkServer.AddPlayerForConnection(player.Connection, playerGameObject, (short)player.PeerId);

		playercount++;
		Debug.LogError(playercount + "/" + GameRoom.MaxPlayers + " Players");
	}

	private void OnPlayerLeft(UnetMsfPlayer player)
	{
		playercount--;
	}

	private void OnServerFull()
	{
		ExchangeController1v1.AllPlayersConnected = true;

		Logs.Error("Server is full invoking method ");

	}

	private void OnServerEmpty()
	{
		Logs.Error("Server is empty. quitting");

		Application.Quit();
	}
}