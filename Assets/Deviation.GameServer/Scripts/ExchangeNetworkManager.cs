using Assets.Deviation.Exchange.Scripts;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// This script represents the changes that you will need to do in your custom
/// network manager
/// </summary>
//[RequireComponent(typeof(GameRoom))]
public class ExchangeNetworkManager : NetworkManager
{
	// Set this in the inspector
	public GameRoom GameRoom;
	public short playercount;
	
	void Awake()
	{
		GameRoom = GameRoom ?? FindObjectOfType<GameRoom>();

		if (GameRoom == null)
		{
			return;
		}
		
		// Subscribe to events
		GameRoom.PlayerJoined += OnPlayerJoined;
		GameRoom.PlayerLeft += OnPlayerLeft;
		GameRoom.ServerFull += OnServerFull;
		GameRoom.ServerEmpty += OnServerEmpty;
	}

	private void OnPlayerJoined(UnetMsfPlayer player)
	{
		Debug.Log("Player Joined");

		var prefabPlayerGameObject = Resources.Load("1v1Player") as GameObject;
		var playerGameObject = Instantiate(prefabPlayerGameObject);
		playerGameObject.GetComponent<ExchangePlayer>().PeerId = player.PeerId;
		NetworkServer.AddPlayerForConnection(player.Connection, playerGameObject, (short)player.PeerId);
		playercount++;
		Debug.Log(playercount + "/" + GameRoom.MaxPlayers + " Players");
	}

	public override void OnServerDisconnect(NetworkConnection conn)
	{
		base.OnServerDisconnect(conn);

		// Don't forget to notify the room that a player disconnected
		GameRoom.ClientDisconnected(conn);
	}

	private void OnPlayerLeft(UnetMsfPlayer player)
	{
		playercount--;
	}

	private void OnServerFull()
	{
		Logs.Error("Server is full invoking method ");

		ExchangeController1v1.AllPlayersConnected = true;
	}

	private void OnServerEmpty()
	{
		Logs.Error("Server is empty. quitting");

		Application.Quit();
	}
}