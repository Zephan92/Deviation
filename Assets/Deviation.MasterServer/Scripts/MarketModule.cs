using Assets.Deviation.Client.Scripts.Client.Market;
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
			server.SetHandler((short)MarketOpCodes.Update, HandleUpdate);
			server.SetHandler((short)MarketOpCodes.Cancel, HandleCancel);

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

		private void HandleUpdate(IIncommingMessage message){}
		private void HandleCancel(IIncommingMessage message)
		{
			TradeReceipt tradeReceipt = message.Deserialize(new TradeReceipt());
			Debug.LogError($"Handle Cancel: {tradeReceipt}");
			_market.AddCancelOrder(tradeReceipt);
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
