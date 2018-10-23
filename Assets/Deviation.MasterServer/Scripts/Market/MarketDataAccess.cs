using Assets.Deviation.Client.Scripts.Client.Market;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Deviation.MasterServer.Scripts.Market
{
	public class MarketDataAccess
	{
		string buysName = "Buys";
		string sellsName = "Sells";
		LiteDatabase db = new LiteDatabase(@"Market.db");
		LiteCollection<TradeItem> _buys;
		LiteCollection<TradeItem> _sells;

		public MarketDataAccess()
		{
			BsonMapper.Global.RegisterType
			(
				serialize: (packet) => packet.ToBsonDocument(),
				deserialize: (bson) => new TradeItem(bson.AsDocument)
			);

			_buys = db.GetCollection<TradeItem>(buysName);
			_buys.EnsureIndex(x => x.ID);
			_sells = db.GetCollection<TradeItem>(sellsName);
			_sells.EnsureIndex(x => x.ID);
		}

		public List<ITradeItem> GetBuyOrders(long playerId)
		{
			return _buys.Find(Query.EQ("PlayerID", new BsonValue(playerId))).ToList<ITradeItem>();
		}

		public List<ITradeItem> GetBuyOrders()
		{
			return _buys.FindAll().ToList<ITradeItem>();
		}

		public ITradeItem GetBuyOrder(long tradeId)
		{
			return _buys.FindOne(Query.EQ("TradeID", new BsonValue(tradeId)));
		}

		public void SaveBuyOrder(TradeItem trade)
		{
			_buys.Insert(trade);
		}

		public void UpdateBuyOrder(TradeItem trade)
		{
			RemoveBuyOrder(trade.ID);
			SaveBuyOrder(trade);
		}

		public void RemoveBuyOrder(long tradeId)
		{
			_buys.Delete(Query.EQ("TradeID", new BsonValue(tradeId)));
		}

		public List<ITradeItem> GetSellOrders(long playerId)
		{
			return _sells.Find(Query.EQ("PlayerID", new BsonValue(playerId))).ToList<ITradeItem>();
		}

		public List<ITradeItem> GetSellOrders()
		{
			return _sells.FindAll().ToList<ITradeItem>();
		}

		public ITradeItem GetSellOrder(long tradeId)
		{
			return _sells.FindOne(Query.EQ("TradeID", new BsonValue(tradeId)));
		}

		public void SaveSellOrder(TradeItem trade)
		{
			_sells.Insert(trade);
		}

		public void UpdateSellOrder(TradeItem trade)
		{
			RemoveSellOrder(trade.ID);
			SaveSellOrder(trade);
		}

		public void RemoveSellOrder(long tradeId)
		{
			_sells.Delete(Query.EQ("TradeID", new BsonValue(tradeId)));
		}
	}
}
