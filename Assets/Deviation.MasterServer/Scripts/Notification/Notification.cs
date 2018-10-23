using Assets.Deviation.Client.Scripts.Client.Market;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Deviation.MasterServer.Scripts.Notification
{
	public class Notification
	{
		private NotificationDataAccess nda = new NotificationDataAccess();
		private ConcurrentDictionary<long, IPeer> _players;

		public Notification()
		{
			_players = new ConcurrentDictionary<long, IPeer>();
		}

		public void AddPlayer(long playerID, IPeer peer)
		{
			if (!IsPlayerOnline(playerID))
			{
				Debug.LogError($"Player {playerID} is Online");
				_players.Add(playerID, peer);
			}
			else
			{
				throw new Exception($"Player is already logged in. Player: {playerID}.");
			}
		}

		public void RemovePlayer(long playerID)
		{
			if (IsPlayerOnline(playerID))
			{
				Debug.LogError($"Removing Player {playerID}");
				_players.Remove(playerID);
			}
		}

		public IPeer GetPlayerPeer(long playerID)
		{
			if (IsPlayerOnline(playerID))
			{
				return _players[playerID];
			}

			return null;
		}

		public bool IsPlayerOnline(long playerID)
		{
			return _players.ContainsKey(playerID);
		}

		public void SaveNotification(short opCode, ISerializablePacket packet)
		{
			switch (opCode)
			{
				case (short) MarketOpCodes.Bought:
					TradeItem buy = MessageHelper.Deserialize(packet.ToBytes(), new TradeItem());
					nda.SaveBuyOrder(buy);
					break;

				case (short) MarketOpCodes.Sold:
					TradeItem sell = MessageHelper.Deserialize(packet.ToBytes(), new TradeItem());
					nda.SaveSellOrder(sell);
					break;
			}
		}

		public Dictionary<short, List<ISerializablePacket>> GetNotifications(long playerId)
		{
			Dictionary<short, List<ISerializablePacket>> retval = new Dictionary<short, List<ISerializablePacket>>();
			List<ISerializablePacket> notifications;

			//buys
			notifications = new List<ISerializablePacket>();
			List<TradeItem> buys = nda.GetBuyOrder(playerId);
			foreach (var trade in buys)
			{
				notifications.Add(trade);
			}
			retval.Add((short) MarketOpCodes.Bought, notifications);

			//sells
			notifications = new List<ISerializablePacket>();
			List<TradeItem> sells = nda.GetSellOrder(playerId);
			foreach (var trade in sells)
			{
				notifications.Add(trade);
			}
			retval.Add((short)MarketOpCodes.Sold, notifications);

			return retval;
		}
	}
}
