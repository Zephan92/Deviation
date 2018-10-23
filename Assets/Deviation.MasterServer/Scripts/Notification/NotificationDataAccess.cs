using Assets.Deviation.Client.Scripts.Client.Market;
using Barebones.Networking;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Deviation.MasterServer.Scripts.Notification
{
	public class NotificationDataAccess
	{
		string buysName = "Buys";
		string sellsName = "Sells";

		LiteDatabase db = new LiteDatabase(@"Notification.db");
		LiteCollection<TradeItem> _buys;
		LiteCollection<TradeItem> _sells;

		public NotificationDataAccess()
		{
			_buys = db.GetCollection<TradeItem>(buysName);
			_buys.EnsureIndex(x => x.PlayerID);
			_sells = db.GetCollection<TradeItem>(sellsName);
			_sells.EnsureIndex(x => x.PlayerID);
		}

		public void SaveBuyOrder(TradeItem trade)
		{
			_buys.Insert(trade);
		}

		public void SaveSellOrder(TradeItem trade)
		{
			_sells.Insert(trade);
		}

		public List<TradeItem> GetBuyOrder(long playerId)
		{
			return _buys.Find(Query.EQ("PlayerID", new BsonValue(playerId))).ToList();
		}

		public List<TradeItem> GetSellOrder(long playerId)
		{
			return _sells.Find(Query.EQ("PlayerID", new BsonValue(playerId))).ToList();
		}
	}
}
