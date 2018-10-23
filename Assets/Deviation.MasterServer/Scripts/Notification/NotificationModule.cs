using Assets.Deviation.MasterServer.Scripts.Exchange;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Deviation.MasterServer.Scripts.Notification
{
	public class NotificationModule : ServerModuleBehaviour
	{
		private Notification _notification;

		public override void Initialize(IServer server)
		{
			base.Initialize(server);

			_notification = new Notification();

			server.SetHandler((short)ExchangePlayerOpCodes.Login, HandleLogin);
			server.SetHandler((short)ExchangePlayerOpCodes.Logout, HandleLogout);

			Debug.Log("Notification Module initialized");
		}

		public IPeer GetPlayerPeer(long playerID)
		{
			return _notification.GetPlayerPeer(playerID);
		}

		public bool IsPlayerOnline(long playerID)
		{
			return _notification.IsPlayerOnline(playerID);
		}

		public void NotifyPlayer(long playerID, short opCode, ISerializablePacket packet)
		{
			if (IsPlayerOnline(playerID))
			{
				Debug.LogError($"Notifying Player {playerID}. Packet: {packet}. Opcode: {opCode}");
				IPeer peer = GetPlayerPeer(playerID);
				peer.SendMessage(opCode, packet);
			}
			else
			{
				_notification.SaveNotification(opCode, packet);
			}
		}

		private void HandleLogin(IIncommingMessage message)
		{
			PlayerAccount player = message.Deserialize(new PlayerAccount());
			message.Peer.Disconnected += (peer) => { Logout(player.Id); };

			_notification.AddPlayer(player.Id, message.Peer);
			message.Respond(ResponseStatus.Success);
			GetNotificationsForPlayer(player.Id);
		}

		private void HandleLogout(IIncommingMessage message)
		{
			PlayerAccount player = message.Deserialize(new PlayerAccount());
			Logout(player.Id);
			message.Respond(ResponseStatus.Success);
		}

		private void Logout(long playerId)
		{
			_notification.RemovePlayer(playerId);
		}

		private void GetNotificationsForPlayer(long playerId)
		{
			Dictionary<short, List<ISerializablePacket>> notificationsDictionary = _notification.GetNotifications(playerId);
			foreach (var notifications in notificationsDictionary)
			{
				foreach (var notification in notifications.Value)
				{
					NotifyPlayer(playerId, notifications.Key, notification);
				}
			}
		}
	}
}
