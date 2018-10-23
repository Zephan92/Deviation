using Assets.Deviation.Client.Scripts.Client.Market;
using Assets.Deviation.MasterServer.Scripts.Exchange;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Deviation.MasterServer.Scripts.Market
{
	public class MarketModule : ServerModuleBehaviour
	{
		private DeviationServer deviation;
		private Market _market;

		public override void Initialize(IServer server)
		{
			base.Initialize(server);

			deviation = FindObjectOfType<DeviationServer>();

			server.SetHandler((short)MarketOpCodes.Buy, HandleBuy);
			server.SetHandler((short)MarketOpCodes.Sell, HandleSell);
			server.SetHandler((short)MarketOpCodes.Collect, HandleCollect);
			server.SetHandler((short)MarketOpCodes.Cancel, HandleCancel);
			server.SetHandler((short)MarketOpCodes.GetPlayerOrders, HandleGetPlayerOrders);

			_market = new Market(deviation);

			Debug.Log("Market Module initialized");
		}

		private void HandleBuy(IIncommingMessage message)
		{
			TradeItem trade = message.Deserialize(new TradeItem());
			Debug.LogError($"Handle Buy: {trade}");
			long tradeID = _market.AddBuyOrder(trade);
			message.Respond(new TradeReceipt(trade.Name, tradeID), ResponseStatus.Success);
		}

		private void HandleSell(IIncommingMessage message)
		{
			TradeItem trade = message.Deserialize(new TradeItem());
			Debug.LogError($"Handle Sell: {trade}");
			long tradeID = _market.AddSellOrder(trade);
			message.Respond(new TradeReceipt(trade.Name, tradeID), ResponseStatus.Success);
		}

		private void HandleCollect(IIncommingMessage message)
		{

		}

		private void HandleCancel(IIncommingMessage message)
		{
			TradeReceipt tradeReceipt = message.Deserialize(new TradeReceipt());
			Debug.LogError($"Handle Cancel: {tradeReceipt}");
			_market.AddCancelOrder(tradeReceipt);
		}

		private void HandleGetPlayerOrders(IIncommingMessage message)
		{
			PlayerAccount player = message.Deserialize(new PlayerAccount());
			Debug.LogError($"Handle Get Player Orders: {player}");
			_market.ResponseWithPlayerOrders(player.Id);
		}

		private void FixedUpdate()
		{
			if (_market != null)
			{
				_market.UpdateQueues();
				_market.FindTrades();
			}
		}
	}
}
