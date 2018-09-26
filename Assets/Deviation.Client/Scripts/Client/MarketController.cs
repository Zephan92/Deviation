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
using UnityEngine.UI;

namespace Assets.Deviation.Client.Scripts.Client
{
	public class MarketController : MonoBehaviour
	{
		public Transform MenuBar;

		public void Awake()
		{
			var parent = GameObject.Find("MarketUI");
			MenuBar = parent.transform.Find("Menu Bar");

			Msf.Client.SetHandler((short)MarketOpCodes.Bought, HandleBought);
			Msf.Client.SetHandler((short)MarketOpCodes.Sold, HandleSold);
			Msf.Client.SetHandler((short)MarketOpCodes.Updated, HandleUpdated);
			Msf.Client.SetHandler((short)MarketOpCodes.Canceled, HandleCanceled);
		}

		public void Buy(ITradeItem trade)
		{
			Debug.Log($"Buy: {trade}");
			Msf.Client.Connection.SendMessage((short)MarketOpCodes.Buy, trade);
		}

		public void Sell(ITradeItem trade)
		{
			Debug.Log($"Sell: {trade}");
			Msf.Client.Connection.SendMessage((short)MarketOpCodes.Sell, trade);
		}

		private void HandleBought(IIncommingMessage message)
		{
			TradeItem trade = message.Deserialize(new TradeItem());
			Debug.Log($"Bought: {trade}");
			message.Respond(ResponseStatus.Success);
		}

		private void HandleSold(IIncommingMessage message)
		{
			TradeItem trade = message.Deserialize(new TradeItem());
			Debug.Log($"Sold: {trade}");
			message.Respond(ResponseStatus.Success);
		}

		private void HandleUpdated(IIncommingMessage message)
		{

		}

		private void HandleCanceled(IIncommingMessage message)
		{

		}

		public List<ITradeItem> SearchForItems(string searchTerm)
		{
			if (searchTerm.Length < 1)
			{
				return new List<ITradeItem>();
			}

			List<ITradeItem> items = new List<ITradeItem>();

			ActionLibrary.GetActionLibrary_ByName().Keys.ToList().ForEach( action => items.Add(new TradeItem(action, 4564, 0, 0, TradeType.Action)));
			MaterialLibrary.GetMaterials().ToList().ForEach(material => items.Add(new TradeItem(material.Name, 4564, 0, 0, TradeType.Material)));
			items = items.OrderBy(x => x.Name).ToList();
			return items.FindAll(i => i.Name.ToLower().Contains(searchTerm.ToLower())).ToList();
		}
	}
}
