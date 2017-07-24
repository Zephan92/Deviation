using Assets.Deviation.Exchange.Scripts;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This script represents the changes that you will need to do in your custom
/// network manager
/// </summary>
public class ExchangeNetworkManager : NetworkManager
{
	// Set this in the inspector
	public ExchangeGameRoom GameRoom;

	void Awake()
	{
		GameRoom = FindObjectOfType<ExchangeGameRoom>();

		if (GameRoom == null)
		{
			Debug.LogError("Game Room property is not set on NetworkManager");
			return;
		}

		// Subscribe to events
		GameRoom.PlayerJoined += OnPlayerJoined;
		GameRoom.PlayerLeft += OnPlayerLeft;

	}

	private void OnPlayerJoined(UnetMsfPlayer player)
	{
		// Spawn the player object (https://docs.unity3d.com/Manual/UNetPlayers.html)
		// This is just a dummy example, you'll need to create your own object (or not)
		var prefabPlayerGameObject = Resources.Load("1v1Player") as GameObject;
		var playerGameObject = Instantiate(prefabPlayerGameObject);
		NetworkServer.AddPlayerForConnection(player.Connection, playerGameObject, 0);
		Debug.Log("Player Joined");
	}

	private void OnPlayerLeft(UnetMsfPlayer player)
	{
	}
}