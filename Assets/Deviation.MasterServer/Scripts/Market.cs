using Assets.Deviation.Client.Scripts.Client.Market;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Deviation.MasterServer.Scripts
{
	public class Market
	{
		private ConcurrentQueue<ITradeItem> _buyOrders;
		private ConcurrentQueue<ITradeItem> _sellOrders;

		private Dictionary<string, List<ITradeItem>> buysByItemName;
		private Dictionary<string, List<ITradeItem>> sellsByItemName;

		private DeviationServer deviation;

		public Market(DeviationServer server)
		{
			deviation = server;
			_buyOrders = new ConcurrentQueue<ITradeItem>();
			_sellOrders = new ConcurrentQueue<ITradeItem>();
			buysByItemName = new Dictionary<string, List<ITradeItem>>();
			sellsByItemName = new Dictionary<string, List<ITradeItem>>();
		}

		public void AddBuyOrder(ITradeItem trade)
		{
			_buyOrders.Enqueue(trade);
		}

		public void AddSellOrder(ITradeItem trade)
		{
			_sellOrders.Enqueue(trade);
		}

		public void AddUpdateOrder(){ }
		public void AddCancelOrder(){ }

		public void UpdateQueues()
		{
			ITradeItem item;
			while (_buyOrders.TryPeek(out item))
			{
				_buyOrders.TryDequeue(out item);
				if (buysByItemName.ContainsKey(item.Name))
				{
					List<ITradeItem> trades = buysByItemName[item.Name];
					trades.Add(item);
					trades.OrderByDescending(x => x.Price);
					buysByItemName[item.Name] = trades;
				}
				else
				{
					buysByItemName.Add(item.Name, new List<ITradeItem>() { item });
				}
			}

			while (_sellOrders.TryPeek(out item))
			{
				_sellOrders.TryDequeue(out item);
				if (sellsByItemName.ContainsKey(item.Name))
				{
					List<ITradeItem> trades = sellsByItemName[item.Name];
					trades.Add(item);
					trades.OrderBy(x => x.Price);
					sellsByItemName[item.Name] = trades;
				}
				else
				{
					sellsByItemName.Add(item.Name, new List<ITradeItem>() { item });
				}
			}
		}

		public void FindTrades()
		{
			foreach (string itemName in buysByItemName.Keys)
			{
				if (sellsByItemName.ContainsKey(itemName))
				{
					List<ITradeItem> buys = buysByItemName[itemName];
					if (buys == null)
					{
						continue;
					}

					List<ITradeItem> sells = sellsByItemName[itemName];
					List<ITradeItem> sellsToRemove = new List<ITradeItem>();
					List<ITradeItem> buysToRemove = new List<ITradeItem>();

					foreach (ITradeItem buy in buys)
					{
						ITradeItem sell = sells.Find(sellItem => sellItem.Price <= buy.Price);
						if (sell != null && sell.Quantity > 0)
						{
							ITradeItem buyNotification = new TradeItem(buy);
							ITradeItem sellNotification = new TradeItem(sell);

							if (buy.Quantity > sell.Quantity)
							{
								buyNotification.Quantity = sell.Quantity;
								buy.Quantity -= sell.Quantity;
								sellsToRemove.Add(sell);
								sell.Quantity = 0;
							}
							else if (buy.Quantity == sell.Quantity)
							{
								sellsToRemove.Add(sell);
								buysToRemove.Add(buy);
								sell.Quantity = 0;
								buy.Quantity = 0;
							}
							else if (buy.Quantity < sell.Quantity)
							{
								sellNotification.Quantity = buy.Quantity;
								sell.Quantity -= buy.Quantity;
								buysToRemove.Add(buy);
								buy.Quantity = 0;
							}
							
							NotifyPlayerTrade(sellNotification, MarketOpCodes.Sold);
							NotifyPlayerTrade(buyNotification, MarketOpCodes.Bought);
						}
					}

					foreach (var item in sellsToRemove)
					{
						sells.Remove(item);
					}

					foreach (var item in buysToRemove)
					{
						buys.Remove(item);
					}
				}
			}
		}

		private void NotifyPlayerTrade(ITradeItem trade, MarketOpCodes opCode)
		{
			deviation.Notification.NotifyPlayer(trade.PlayerID, (short) opCode, trade);
		}
	}
}
