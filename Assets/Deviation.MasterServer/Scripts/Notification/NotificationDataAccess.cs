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
		string ordersName = "Orders";
		string sellsName = "Sells";

		LiteDatabase db = new LiteDatabase(@"Notification.db");
		LiteCollection<TradeItem> _orders;
		LiteCollection<TradeItem> _sells;

		public NotificationDataAccess()
		{
			_orders = db.GetCollection<TradeItem>(ordersName);
			_orders.EnsureIndex(x => x.PlayerID);
		}

		public void SaveMarketUpdate(TradeItem trade)
		{
			_orders.Insert(trade);
		}

		public List<TradeItem> GetMarketOrders(long playerId)
		{
			return _orders.Find(Query.EQ("PlayerID", new BsonValue(playerId))).ToList();
		}
	}
}
