using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Deviation.Exchange.Scripts.Client
{
	public enum ClientState
	{
		Login = 0,
		Client = 1,
		Match = 2,
	}

	public class ClientDataController : MonoBehaviour
	{
		public static ClientDataController instance = null;

		public ClientState State = ClientState.Login;

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

			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondRoomId, HandleReceiveRoomId);
		}

		public void InstanceExists()
		{
			if (instance == null)
			{
				instance = this;
			}
			else if (instance != this)
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
			instance.RoomId = message.AsInt();
		}

		public void GetPlayerAccount()
		{
			Msf.Client.Connection.SendMessage((short)ExchangePlayerOpCodes.GetPlayerAccount, Msf.Client.Auth.AccountInfo.Username, (status, response) =>
			{
				_playerAccount = response.Deserialize(new PlayerAccountPacket());
				PlayerAccountRecieved.Invoke();
			});
		}
	}
}
