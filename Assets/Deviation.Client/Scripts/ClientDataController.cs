using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Deviation.Exchange.Scripts.Client
{
	public enum ClientState
	{
		Default = -1,
		Login = 0,
		Client = 1,
		Match = 2,
		Results = 3,
	}

	public class ClientDataController : MonoBehaviour
	{
		public static ClientDataController Instance = null;

		public ClientState _state = ClientState.Default;
		public ClientState State
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;

				if (OnClientDataStateChange != null)
				{
					OnClientDataStateChange(value);
				}
			}
		}
		public UnityAction<ClientState> OnClientDataStateChange;

		private PlayerAccountPacket _playerAccount;
		public PlayerAccountPacket PlayerAccount
		{
			get
			{
				if (_playerAccount != null)
				{
					return _playerAccount;
				}
				else
				{
					GetPlayerAccount();
					return null;
				}
			}
		}

		public MatchFoundPacket Exchange;

		public int RoomId = -1;
		
		public Action PlayerAccountRecieved;

		public void Awake()
		{
			InstanceExists();
			PlayerAccountRecieved += () => { };
			OnClientDataStateChange += OnClienDataStateChange;
			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondRoomId, HandleReceiveRoomId);
		}

		private void OnClienDataStateChange(ClientState state)
		{
			switch (state)
			{
				case ClientState.Default:
					break;
				case ClientState.Login:
					break;
				case ClientState.Client:
					break;
				case ClientState.Match:
					break;
				case ClientState.Results:
					Destroy(gameObject);
					break;
			}
		}

		public void InstanceExists()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else if (Instance != this)
			{
				Destroy(gameObject);
			}
		}

		public void Start()
		{
			if (State > 0 && _playerAccount == null)
			{
				GetPlayerAccount();
			}
		}

		public void FixedUpdate()
		{

		}

		public void HandleReceiveRoomId(IIncommingMessage message)
		{
			Instance.RoomId = message.AsInt();
		}

		public void GetPlayerAccount()
		{
			if (Msf.Client.Connection.IsConnected)
			{
				Msf.Client.Connection.SendMessage((short)ExchangePlayerOpCodes.GetPlayerAccount, Msf.Client.Auth.AccountInfo.Username, (status, response) =>
				{
					_playerAccount = response.Deserialize(new PlayerAccountPacket());
					PlayerAccountRecieved.Invoke();
				});
			}
			else
			{
				Debug.Log("Failed to Request Player Account. Not Logged in.");
			}
			
		}
	}
}
