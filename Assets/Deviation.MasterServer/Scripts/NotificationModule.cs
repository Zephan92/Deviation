using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Deviation.MasterServer.Scripts
{
	public class NotificationModule : ServerModuleBehaviour
	{
		private Notification _notification;

		public override void Initialize(IServer server)
		{
			base.Initialize(server);

			_notification = new Notification();

			server.SetHandler((short)ExchangePlayerOpCodes.Login, HandleLogin);
			server.SetHandler((short)ExchangePlayerOpCodes.Logout, HandleLogin);

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
				Debug.LogError($"Notifying Player {playerID}. Packet: {packet}");
				IPeer peer = GetPlayerPeer(playerID);
				peer.SendMessage(opCode, packet);
			}
			else
			{
				//notify later
			}
		}

		private void HandleLogin(IIncommingMessage message)
		{
			PlayerAccount player = message.Deserialize(new PlayerAccount());
			_notification.AddPlayer(player.Id, message.Peer);
		}

		private void HandleLogout(IIncommingMessage message)
		{
			PlayerAccount player = message.Deserialize(new PlayerAccount());
			_notification.RemovePlayer(player.Id);
		}
	}
}
