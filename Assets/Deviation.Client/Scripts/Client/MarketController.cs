using Assets.Deviation.Client.Scripts.Client.Market;
using Assets.Deviation.Client.Scripts.Match;
using Assets.Deviation.Client.Scripts.UserInterface;
using Assets.Deviation.Exchange.Scripts.DTO.Exchange;
using Assets.Deviation.MasterServer.Scripts;
using Assets.Deviation.Materials;
using Assets.Scripts.DTO.Exchange;
using Assets.Scripts.Library;
using Barebones.MasterServer;
using Barebones.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Client
{
	public class MarketController : MonoBehaviour
	{
		private Transform menuBar;
		private TradeWindow tradeWindow;

		public void Awake()
		{
			var parent = GameObject.Find("MarketUI");
			menuBar = parent.transform.Find("Menu Bar");
			tradeWindow = transform.Find("TradingUI").GetComponent<TradeWindow>();

			Msf.Client.SetHandler((short)MarketOpCodes.Bought, HandleBought);
			Msf.Client.SetHandler((short)MarketOpCodes.Sold, HandleSold);
			Msf.Client.SetHandler((short)MarketOpCodes.Updated, HandleUpdated);
			Msf.Client.SetHandler((short)MarketOpCodes.Canceled, HandleCanceled);
		}

		public void Buy(ITradeItem trade, UnityAction<ITradeItem> offerCallback)
		{
			Debug.Log($"Buy: {trade}");
			if (tradeWindow.OpenOfferPanel())
			{
				Msf.Client.Connection.SendMessage((short)MarketOpCodes.Buy, trade, (response, data) => { offerCallback(DeserializeReceipt(data, trade)); });
			}
		}

		public void Sell(ITradeItem trade, UnityAction<ITradeItem> offerCallback)
		{
			Debug.Log($"Sell: {trade}");
			if (tradeWindow.OpenOfferPanel())
			{
				Msf.Client.Connection.SendMessage((short)MarketOpCodes.Sell, trade, (response, data) => { offerCallback(DeserializeReceipt(data, trade)); });
			}
		}

		public void Cancel(ITradeReceipt trade)
		{
			Debug.Log($"Cancel: {trade}");
			Msf.Client.Connection.SendMessage((short)MarketOpCodes.Cancel, trade);
		}

		private void HandleBought(IIncommingMessage message)
		{
			TradeItem trade = message.Deserialize(new TradeItem());
			Debug.Log($"Bought: {trade}");
			tradeWindow.Bought(trade);
		}

		private void HandleSold(IIncommingMessage message)
		{
			TradeItem trade = message.Deserialize(new TradeItem());
			Debug.Log($"Sold: {trade}");
			tradeWindow.Sold(trade);

		}

		private void HandleUpdated(IIncommingMessage message)
		{

		}

		private void HandleCanceled(IIncommingMessage message)
		{
			ITradeItem trade = message.Deserialize(new TradeItem());
			Debug.Log($"Canceled: {trade}");
			tradeWindow.CancelOffer(trade);
		}

		public List<ITradeItem> SearchForItems(string searchTerm)
		{
			if (searchTerm.Length < 1)
			{
				return new List<ITradeItem>();
			}

			List<ITradeItem> items = new List<ITradeItem>();

			ActionLibrary.GetActionLibrary_ByName().Keys.ToList().ForEach( action => items.Add(new TradeItem(0, action, 4564, 0, 0, TradeType.Action)));
			MaterialLibrary.GetMaterials().ToList().ForEach(material => items.Add(new TradeItem(0, material.Name, 4564, 0, 0, TradeType.Material)));
			items = items.OrderBy(x => x.Name).ToList();
			return items.FindAll(i => i.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
		}

		private ITradeItem DeserializeReceipt(IIncommingMessage message, ITradeItem trade)
		{
			ITradeReceipt receipt =  message.Deserialize(new TradeReceipt());
			trade.ID = receipt.ID;
			return trade;
		}
	}
}
