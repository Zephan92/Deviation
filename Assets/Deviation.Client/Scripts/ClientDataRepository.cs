using Assets.Deviation.Client.Scripts;
using Assets.Deviation.Client.Scripts.Client.Market;
using Assets.Deviation.MasterServer.Scripts;
using Assets.Deviation.MasterServer.Scripts.Exchange;
using Assets.Deviation.MasterServer.Scripts.MatchMaking;
using Assets.Deviation.MasterServer.Scripts.Notification;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

	public class ClientDataRepository : MonoBehaviour
	{
		public static ClientDataRepository Instance = null;
		private static UnityAction _onInstanceCreated;

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
				OnClientDataStateChange?.Invoke(value);
			}
		}
		public UnityAction<ClientState> OnClientDataStateChange;

		public PlayerAccount PlayerAccount
		{
			get
			{
				return _playerAccount;
			}
			set
			{
				_playerAccount = value;
				PlayerAccountRecieved?.Invoke(value);
			}
		}
		public PlayerAccount _playerAccount;
		public UnityAction<PlayerAccount> PlayerAccountRecieved;

		public MatchFoundPacket Exchange;

		public int RoomId = -1;

		public UnityAction<AccountInfoPacket, string> OnLoginServer;
		public bool LoggedIn
		{
			get
			{
				return _loggedIn;
			}
			set
			{
				_loggedIn = value;
				OnLoggedIn?.Invoke(value);
			}
		}
		public UnityAction<bool> OnLoggedIn;
		public bool _loggedIn;
		public bool HasPlayerAccount;
		public bool HasExchange;

		private Dictionary<NotificationType, List<ISerializablePacket>> notifications = new Dictionary<NotificationType, List<ISerializablePacket>>();
		public List<ITradeItem> MarketOrders { get; set; }

		public void Awake()
		{
			InstanceExists();
			MarketOrders = new List<ITradeItem>();
			OnClientDataStateChange += ClientDataStateChange;
			Msf.Client.SetHandler((short)Exchange1v1MatchMakingOpCodes.RespondRoomId, HandleReceiveRoomId);
			Msf.Client.SetHandler((short)MarketOpCodes.PlayerOrders, HandlePlayerOrders);
			Msf.Client.SetHandler((short)MarketOpCodes.MarketUpdate, HandleMarketUpdate);
		}

		private void ClientDataStateChange(ClientState state)
		{
			switch (state)
			{
				case ClientState.Default:
					break;
				case ClientState.Login:
					break;
				case ClientState.Client:
					if (LoggedIn)
					{
						SendMessage((short)MarketOpCodes.GetPlayerOrders, PlayerAccount);
					}
					else
					{
						OnLoggedIn += (loggedIn) => { SendMessage((short)MarketOpCodes.GetPlayerOrders, PlayerAccount); };
					}
					break;
				case ClientState.Match:
					break;
				case ClientState.Results:
					RoomId = -1;
					Exchange = null;
					HasExchange = false;
					break;
			}
		}

		private void SendMessage(short opCode, ISerializablePacket packet)
		{
			if (Msf.Connection.IsConnected)
			{
				Msf.Connection.SendMessage(opCode, packet);
			}
			else
			{
				Msf.Connection.Connected += () => { Msf.Connection.SendMessage(opCode, packet); };
			}
		}

		public void InstanceExists()
		{
			if (Instance == null)
			{
				Instance = this;
				_onInstanceCreated?.Invoke();
				DontDestroyOnLoad(Instance);
			}
			else if (Instance != this)
			{
				Destroy(gameObject);
			}
		}

		public static void OnInstanceCreated(UnityAction onInstanceCreated)
		{
			if (Instance == null)
			{
				_onInstanceCreated = onInstanceCreated;
			}
			else
			{
				onInstanceCreated();
			}
		}

		public void HandleReceiveRoomId(IIncommingMessage message)
		{
			Instance.RoomId = message.AsInt();
		}

		public void LoginAsGuest()
		{
			if (Msf.Client.Auth.IsLoggedIn)
			{
				return;
			}
			
			if (Msf.Client.Connection.IsConnected)
			{
				Msf.Client.Auth.LogInAsGuest((successful, error) =>
				{
					OnLoginServer?.Invoke(successful, error);
					UnityEngine.Debug.Log("Logged in successfully");
				});
			}
			else
			{
				Debug.LogError("Not Connected To Server. Failed to login as guest.");
			}
		}

		public void GetPlayerAccount()
		{
			if (HasPlayerAccount)
			{
				return;
			}

			if (Msf.Client.Connection.IsConnected)
			{
				if (Msf.Client.Auth.IsLoggedIn)
				{
					Msf.Client.Connection.SendMessage((short)ExchangePlayerOpCodes.GetPlayerAccount, Msf.Client.Auth.AccountInfo.Username, (status, response) =>
					{
						PlayerAccount = response.Deserialize(new PlayerAccount());
						Msf.Client.Connection.SendMessage((short)ExchangePlayerOpCodes.Login, PlayerAccount, (loginStatus, loginReponse) => { LoggedIn = true; });
						HasPlayerAccount = true;
						Debug.Log("Player Account Successfully Received");
					});
				}
				else
				{
					Debug.LogError("Not Logged in. Failed to get player account.");
				}
			}
			else
			{
				Debug.LogError("Not Connected To Server. Failed to get player account.");
			}
		}

		private void HandlePlayerOrders(IIncommingMessage message)
		{
			ITradeItem trade = message.Deserialize(new TradeItem());
			Debug.Log($"Player Order Notification: {trade}");
			MarketOrders.Add(trade);
		}

		private void HandleMarketUpdate(IIncommingMessage message)
		{
			ISerializablePacket trade = message.Deserialize(new TradeItem());
			Debug.Log($"Player Order Notification: {trade}");
			SaveNotification(NotificationType.MarketUpdate, trade);
		}

		public List<ISerializablePacket> GetNotifications(NotificationType notificationType)
		{
			if (notifications.ContainsKey(notificationType))
			{
				return notifications[notificationType];
			}
			else
			{
				return new List<ISerializablePacket>();
			}
		}

		private void SaveNotification(NotificationType notificationType, ISerializablePacket packet)
		{
			if (notifications.ContainsKey(notificationType))
			{
				notifications[notificationType].Add(packet);
			}
			else
			{
				notifications.Add(notificationType, new List<ISerializablePacket>{ packet });
			}
		}

		void OnApplicationQuit()
		{
			try
			{
				Msf.Client.Connection.SendMessage((short)ExchangePlayerOpCodes.Logout, PlayerAccount);

				if (Msf.Client.Connection != null)
				{
					Msf.Client.Connection.Disconnect();
				}
			}
			catch(Exception ex)
			{
				//eat the exception
			}
		}
	}
}
