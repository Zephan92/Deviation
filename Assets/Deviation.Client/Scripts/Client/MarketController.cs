using Assets.Deviation.Client.Scripts.Client.Market;
using Assets.Deviation.Client.Scripts.Match;
using Assets.Deviation.Client.Scripts.UserInterface;
using Assets.Deviation.Exchange.Scripts.Client;
using Assets.Deviation.Exchange.Scripts.DTO.Exchange;
using Assets.Deviation.MasterServer.Scripts;
using Assets.Deviation.MasterServer.Scripts.Notification;
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

			Msf.Client.SetHandler((short)MarketOpCodes.MarketUpdate, HandleMarketUpdate);
			//Msf.Client.SetHandler((short)MarketOpCodes.Updated, HandleUpdated);
			Msf.Client.SetHandler((short)MarketOpCodes.Canceled, HandleCanceled);
		}

		public void Start()
		{
			List<ITradeItem> orders = ClientDataRepository.Instance.MarketOrders;
			foreach (ITradeItem trade in orders)
			{
				tradeWindow.Fill(trade);
			}

			List<ISerializablePacket> marketUpdates = ClientDataRepository.Instance.GetNotifications(NotificationType.MarketUpdate);
			foreach (ITradeItem trade in marketUpdates)
			{
				tradeWindow.MarketUpdate(trade);
			}
		}

		public void Buy(ITradeItem trade, UnityAction<ITradeItem> offerCallback)
		{
			trade.OrderType = OrderType.Buy;

			Debug.Log($"Buy: {trade}");
			if (tradeWindow.OpenOfferPanel())
			{
				Msf.Client.Connection.SendMessage((short)MarketOpCodes.Buy, trade, (response, data) => { offerCallback(DeserializeReceipt(data, trade)); });
			}
		}

		public void Sell(ITradeItem trade, UnityAction<ITradeItem> offerCallback)
		{
			trade.OrderType = OrderType.Sell;

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

		private void HandleMarketUpdate(IIncommingMessage message)
		{
			TradeItem trade = message.Deserialize(new TradeItem());
			Debug.Log($"MarketUpdate: {trade}");
			tradeWindow.MarketUpdate(trade);
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

			ActionLibrary.GetActionLibrary_ByName().Keys.ToList().ForEach( action => items.Add(new TradeItem(0, action, 4564, 0, 0, ResourceType.Action, OrderType.None)));
			MaterialLibrary.GetMaterials().ToList().ForEach(material => items.Add(new TradeItem(0, material.Name, 4564, 0, 0, ResourceType.Material, OrderType.None)));
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
