using UnityEngine;
using System.Collections;
using Barebones.MasterServer;
using System.Collections.Generic;
using System;
using Assets.Deviation.Exchange.Scripts;

public class StandaloneController : MonoBehaviour
{
	protected SpawnRequestController Request;

	public void Awake()
	{
		Msf.Client.Connection.Connected += Login;
	}

	public void Login()
	{
		Msf.Client.Auth.LogInAsGuest((successful, error) => { });
		Msf.Client.Auth.LoggedIn += JoinServer;
	}

	public void JoinServer()
	{
		var settings = new Dictionary<string, string>
			{
				{MsfDictKeys.MaxPlayers, "2"},
				{MsfDictKeys.RoomName, "Test 1"},
				{MsfDictKeys.MapName, "1v1Exchange"},
				{MsfDictKeys.SceneName, "1v1Exchange"}
			};

		Msf.Client.Matchmaker.FindGames(games =>
		{
			var loadingPromise = Msf.Events.FireWithPromise(Msf.EventNames.ShowLoading, "Retrieving Rooms list...");

			loadingPromise.Finish();

			foreach (var game in games)
			{
				if (game.OnlinePlayers < game.MaxPlayers)
				{
					Logs.Info("Joining Existing Game");
					Msf.Client.Rooms.GetAccess(game.Id, OnPassReceived);
					return;
				}
			}

			Logs.Info("Requesting new Game Server");

			Msf.Client.Spawners.RequestSpawn(settings, "", (requestController, errorMsg) =>
			{
				if (requestController == null)
				{
					Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError("Failed to create a game: " + errorMsg));

					Logs.Error("Failed to create a game: " + errorMsg);
				}

				Display(requestController);

			});
		});
	}

	public void Display(SpawnRequestController request)
	{
		if (Request != null)
			Request.StatusChanged -= OnStatusChange;

		if (request == null)
			return;

		request.StatusChanged += OnStatusChange;

		Request = request;
	}

	protected void OnStatusChange(SpawnStatus status)
	{
		if (status < SpawnStatus.None)
		{
			// If game was aborted
			Msf.Events.Fire(Msf.EventNames.ShowDialogBox,
				DialogBoxData.CreateInfo("Game creation aborted"));

			Logs.Error("Game creation aborted");

			// Hide the window
			gameObject.SetActive(false);
		}

		if (status == SpawnStatus.Finalized)
		{
			Request.GetFinalizationData((data, error) =>
			{
				if (data == null)
				{
					Msf.Events.Fire(Msf.EventNames.ShowDialogBox,
						DialogBoxData.CreateInfo("Failed to retrieve completion data: " + error));

					Logs.Error("Failed to retrieve completion data: " + error);

					Request.Abort();
					return;
				}

				// Completion data received
				var roomId = int.Parse(data[MsfDictKeys.RoomId]);
				Msf.Client.Rooms.GetAccess(roomId, OnPassReceived);
			});
		}
	}

	public void OnFinalizationDataRetrieved(Dictionary<string, string> data)
	{
		if (!data.ContainsKey(MsfDictKeys.RoomId))
		{
			throw new Exception("Game server finalized, but didn't include room id");
		}

		var roomId = int.Parse(data[MsfDictKeys.RoomId]);
		Msf.Client.Rooms.GetAccess(roomId, OnPassReceived);
	}

	protected virtual void OnPassReceived(RoomAccessPacket packet, string errorMessage)
	{

		var loadingPromise = Msf.Events.FireWithPromise(Msf.EventNames.ShowLoading, "Joining lobby");

		if (packet == null)
		{
			Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError(errorMessage));
			Logs.Error(errorMessage);
			return;
		}

		Msf.Client.Lobbies.JoinLobby(packet.RoomId, (lobby, error) =>
		{
			loadingPromise.Finish();

			if (lobby == null)
			{
				Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError(error));
				return;
			}
		});

		Logs.Info("Connecting to Game");

		FindObjectOfType<ExchangeRoomConnector>().ConnectToGame(packet);

	}
}
