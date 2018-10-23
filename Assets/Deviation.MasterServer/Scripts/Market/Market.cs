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

namespace Assets.Deviation.MasterServer.Scripts.Market
{
	public class Market
	{
		private MarketDataAccess mda = new MarketDataAccess();

		private ConcurrentQueue<ITradeItem> _buyOrders;
		private ConcurrentQueue<ITradeItem> _sellOrders;
		private ConcurrentQueue<ITradeReceipt> _cancelOrders;

		private Dictionary<string, List<ITradeItem>> buysByItemName;
		private Dictionary<string, List<ITradeItem>> sellsByItemName;

		private DeviationServer deviation;
		private long orderCount;

		public Market(DeviationServer server)
		{
			deviation = server;
			_buyOrders = new ConcurrentQueue<ITradeItem>();
			_sellOrders = new ConcurrentQueue<ITradeItem>();

			foreach (var buy in mda.GetBuyOrders())
			{
				orderCount++;
				_buyOrders.Enqueue(buy);
			}

			foreach (var sell in mda.GetSellOrders())
			{
				orderCount++;
				_buyOrders.Enqueue(sell);
			}

			_cancelOrders = new ConcurrentQueue<ITradeReceipt>();
			buysByItemName = new Dictionary<string, List<ITradeItem>>();
			sellsByItemName = new Dictionary<string, List<ITradeItem>>();
		}

		public long AddBuyOrder(ITradeItem trade)
		{
			orderCount++;
			trade.ID = orderCount;
			mda.SaveBuyOrder(new TradeItem(trade));
			_buyOrders.Enqueue(trade);
			return orderCount;
		}

		public long AddSellOrder(ITradeItem trade)
		{
			orderCount++;
			trade.ID = orderCount;
			mda.SaveSellOrder(new TradeItem(trade));
			_sellOrders.Enqueue(trade);
			return orderCount;
		}

		public void AddUpdateOrder(){ }
		public void AddCancelOrder(ITradeReceipt trade)
		{
			_cancelOrders.Enqueue(trade);
		}

		public void ResponseWithPlayerOrders(long playerId)
		{
			var sells = mda.GetSellOrders(playerId);
			foreach (var sell in sells)
			{
				NotifyPlayerTrade(sell, MarketOpCodes.Sell);
			}

			var buys = mda.GetBuyOrders(playerId);
			foreach (var buy in buys)
			{
				NotifyPlayerTrade(buy, MarketOpCodes.Buy);
			}
		}

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

			ITradeReceipt cancelation;

			while (_cancelOrders.TryPeek(out cancelation))
			{
				_cancelOrders.TryDequeue(out cancelation);
				if (sellsByItemName.ContainsKey(cancelation.Name))
				{
					List<ITradeItem> sells = sellsByItemName[cancelation.Name];
					List<ITradeItem> sellsToRemove = new List<ITradeItem>();

					foreach (var sell in sells)
					{
						if (sell.ID == cancelation.ID)
						{
							NotifyPlayerTrade(sell, MarketOpCodes.Canceled);
							sellsToRemove.Add(sell);
						}
					}

					foreach (var sell in sellsToRemove)
					{
						mda.RemoveSellOrder(sell.ID);
						sells.Remove(sell);
					}
				}

				if (buysByItemName.ContainsKey(cancelation.Name))
				{
					List<ITradeItem> buys = buysByItemName[cancelation.Name];
					List<ITradeItem> buysToRemove = new List<ITradeItem>();

					foreach (var buy in buys)
					{
						if (buy.ID == cancelation.ID)
						{
							NotifyPlayerTrade(buy, MarketOpCodes.Canceled);
							buysToRemove.Add(buy);
						}
					}

					foreach (var buy in buysToRemove)
					{
						mda.RemoveBuyOrder(buy.ID);
						buys.Remove(buy);
					}
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
								mda.UpdateBuyOrder(new TradeItem(buy));
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
								mda.UpdateSellOrder(new TradeItem(sell));
								buysToRemove.Add(buy);
								buy.Quantity = 0;
							}
							
							NotifyPlayerTrade(sellNotification, MarketOpCodes.Sold);
							NotifyPlayerTrade(buyNotification, MarketOpCodes.Bought);
						}
					}

					foreach (var item in sellsToRemove)
					{
						mda.RemoveSellOrder(item.ID);
						sells.Remove(item);
					}

					foreach (var item in buysToRemove)
					{
						mda.RemoveBuyOrder(item.ID);
						buys.Remove(item);
					}
				}
			}
		}

		private void NotifyPlayerTrade(ITradeItem trade, MarketOpCodes opCode)
		{
			deviation.Notification.NotifyPlayer(trade.PlayerID, (short)opCode, trade);
		}
	}
}
