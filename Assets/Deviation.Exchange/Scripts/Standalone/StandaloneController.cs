using UnityEngine;
using System.Collections;
using Barebones.MasterServer;
using System.Collections.Generic;
using System;
using Assets.Deviation.Exchange.Scripts;
using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Deviation.Client.Scripts;

//[RequireComponent(typeof(ExchangeRoomConnector))]
public class StandaloneController : ControllerBase
{
	protected SpawnRequestController Request;

	public override void Awake()
	{
		base.Awake();
		JoinServer();
	}

	public void JoinServer()
	{
		Msf.Client.Rooms.GetAccess(ClientDataRepository.Instance.RoomId, OnPassReceived);
	}

	protected void OnPassReceived(RoomAccessPacket packet, string errorMessage)
	{
		Msf.Events.FireWithPromise(Msf.EventNames.ShowLoading, "Joining lobby");

		if (packet == null)
		{
			Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError(errorMessage));
			Logs.Error(errorMessage);
			return;
		}

		Logs.Info("Connecting to Game");

		FindObjectOfType<ExchangeRoomConnector>().ConnectToGame(packet);
	}
}
